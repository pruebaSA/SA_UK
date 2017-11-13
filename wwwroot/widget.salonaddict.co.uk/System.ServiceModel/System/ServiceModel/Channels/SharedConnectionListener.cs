namespace System.ServiceModel.Channels
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Net;
    using System.Net.Sockets;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Security.AccessControl;
    using System.Security.Principal;
    using System.ServiceModel;
    using System.ServiceModel.Activation;
    using System.ServiceModel.Description;
    using System.ServiceModel.Diagnostics;
    using System.ServiceModel.Dispatcher;
    using System.ServiceProcess;
    using System.Threading;

    internal sealed class SharedConnectionListener : IConnectionListener, IDisposable
    {
        private BaseUriWithWildcard baseAddress;
        private InputQueue<DuplicateConnectionAsyncResult> connectionQueue;
        private SharedListenerProxy listenerProxy;
        private OnDuplicatedViaDelegate onDuplicatedViaCallback;
        private int queueId;
        private WaitCallback reconnectCallback;
        private ManualResetEvent reconnectEvent;
        private CommunicationState state;
        private object syncRoot = new object();
        private Guid token;

        internal SharedConnectionListener(BaseUriWithWildcard baseAddress, int queueId, Guid token, OnDuplicatedViaDelegate onDuplicatedViaCallback)
        {
            this.baseAddress = baseAddress;
            this.queueId = queueId;
            this.token = token;
            this.onDuplicatedViaCallback = onDuplicatedViaCallback;
            this.connectionQueue = new InputQueue<DuplicateConnectionAsyncResult>();
            this.state = CommunicationState.Created;
            this.reconnectEvent = new ManualResetEvent(true);
            this.StartListen(false);
        }

        private void Close()
        {
            lock (this.ThisLock)
            {
                if (this.state == CommunicationState.Closed)
                {
                    return;
                }
                this.state = CommunicationState.Closed;
            }
            if (this.connectionQueue != null)
            {
                this.connectionQueue.Close();
            }
            if (this.reconnectEvent != null)
            {
                this.reconnectEvent.Close();
            }
        }

        private static string GetServiceName(bool isTcp)
        {
            if (!isTcp)
            {
                return "NetPipeActivator";
            }
            return "NetTcpPortSharing";
        }

        private void OnConnectionAvailable(DuplicateConnectionAsyncResult result)
        {
            this.connectionQueue.EnqueueAndDispatch(result, null, false);
        }

        private void OnListenerFaulted(bool shouldReconnect)
        {
            lock (this.ThisLock)
            {
                if ((this.state == CommunicationState.Closing) || (this.state == CommunicationState.Closed))
                {
                    return;
                }
                this.listenerProxy.Abort();
                if (shouldReconnect)
                {
                    this.state = CommunicationState.Opening;
                    this.reconnectEvent.Reset();
                }
                else
                {
                    this.state = CommunicationState.Faulted;
                }
            }
            if (shouldReconnect)
            {
                if (this.reconnectCallback == null)
                {
                    this.reconnectCallback = new WaitCallback(this.ReconnectCallback);
                }
                IOThreadScheduler.ScheduleCallback(this.reconnectCallback, this);
            }
        }

        private void ReconnectCallback(object state)
        {
            BackoffTimeoutHelper helper = new BackoffTimeoutHelper(TimeSpan.MaxValue, TimeSpan.FromMinutes(5.0), TimeSpan.FromSeconds(30.0));
            while (this.state == CommunicationState.Opening)
            {
                try
                {
                    this.StartListen(true);
                }
                catch (Exception exception)
                {
                    if (DiagnosticUtility.IsFatal(exception))
                    {
                        throw;
                    }
                    if (DiagnosticUtility.ShouldTraceError)
                    {
                        DiagnosticUtility.ExceptionUtility.TraceHandledException(exception, TraceEventType.Error);
                    }
                }
                if (this.state == CommunicationState.Opening)
                {
                    helper.WaitAndBackoff();
                }
            }
        }

        private void StartListen(bool isReconnecting)
        {
            this.listenerProxy = new SharedListenerProxy(this);
            if (isReconnecting)
            {
                this.reconnectEvent.Set();
            }
            this.listenerProxy.Open(isReconnecting);
            lock (this.ThisLock)
            {
                if ((this.state == CommunicationState.Created) || (this.state == CommunicationState.Opening))
                {
                    this.state = CommunicationState.Opened;
                }
            }
        }

        public void Stop()
        {
            bool flag = false;
            lock (this.ThisLock)
            {
                if ((this.state == CommunicationState.Closing) || (this.state == CommunicationState.Closed))
                {
                    return;
                }
                if (this.state == CommunicationState.Opening)
                {
                    flag = true;
                }
                this.state = CommunicationState.Closing;
            }
            if (flag)
            {
                this.reconnectEvent.WaitOne();
            }
            if (this.listenerProxy != null)
            {
                this.listenerProxy.Close();
            }
        }

        void IDisposable.Dispose()
        {
            this.Close();
        }

        IAsyncResult IConnectionListener.BeginAccept(AsyncCallback callback, object state) => 
            this.connectionQueue.BeginDequeue(TimeSpan.MaxValue, callback, state);

        IConnection IConnectionListener.EndAccept(IAsyncResult result)
        {
            lock (this.ThisLock)
            {
                if ((this.state != CommunicationState.Opening) && (this.state != CommunicationState.Opened))
                {
                    return null;
                }
                DuplicateConnectionAsyncResult result2 = this.connectionQueue.EndDequeue(result);
                result2.CompleteOperation();
                return result2.Connection;
            }
        }

        void IConnectionListener.Listen()
        {
        }

        private object ThisLock =>
            this.syncRoot;

        private class DuplicateConnectionAsyncResult : AsyncResult
        {
            private IConnection connection;

            public DuplicateConnectionAsyncResult(AsyncCallback callback, object state) : base(callback, state)
            {
                base.Complete(true);
            }

            public DuplicateConnectionAsyncResult(IConnection connection, AsyncCallback callback, object state) : base(callback, state)
            {
                this.connection = connection;
            }

            public void CompleteOperation()
            {
                base.Complete(false);
            }

            public static void End(IAsyncResult result)
            {
                AsyncResult.End<SharedConnectionListener.DuplicateConnectionAsyncResult>(result);
            }

            public IConnection Connection =>
                this.connection;
        }

        [CallbackBehavior(ConcurrencyMode=ConcurrencyMode.Multiple)]
        private class SharedListenerProxy : IConnectionDuplicator, IInputSessionShutdown
        {
            private IDisposable allowContext;
            private BaseUriWithWildcard baseAddress;
            private ChannelFactory channelFactory;
            private bool closed;
            private int connectionBufferSize;
            private IDuplexContextChannel controlSessionWithListener;
            private static byte[] drainBuffer;
            private bool isTcp;
            private bool listenerClosed;
            private string listenerEndPoint;
            private SecurityIdentifier listenerUniqueSid;
            private SecurityIdentifier listenerUserSid;
            private OnDuplicatedViaDelegate onDuplicatedViaCallback;
            private bool opened;
            private SharedConnectionListener parent;
            private int queueId;
            private string securityEventName;
            private string serviceName;
            private object syncRoot = new object();
            private Guid token;

            public SharedListenerProxy(SharedConnectionListener parent)
            {
                this.parent = parent;
                this.baseAddress = parent.baseAddress;
                this.queueId = parent.queueId;
                this.token = parent.token;
                this.onDuplicatedViaCallback = parent.onDuplicatedViaCallback;
                this.isTcp = parent.baseAddress.BaseAddress.Scheme.Equals(Uri.UriSchemeNetTcp);
                this.securityEventName = Guid.NewGuid().ToString();
                this.serviceName = SharedConnectionListener.GetServiceName(this.isTcp);
            }

            public void Abort()
            {
                this.Close(true);
            }

            private IConnection BuildConnectionFromData(DuplicateContext duplicateContext, int connectionBufferSize)
            {
                if (this.isTcp)
                {
                    return this.BuildDuplicatedTcpConnection((TcpDuplicateContext) duplicateContext, connectionBufferSize);
                }
                return this.BuildDuplicatedNamedPipeConnection((NamedPipeDuplicateContext) duplicateContext, connectionBufferSize);
            }

            private IConnection BuildDuplicatedNamedPipeConnection(NamedPipeDuplicateContext duplicateContext, int connectionBufferSize)
            {
                if (DiagnosticUtility.ShouldTraceVerbose)
                {
                    TraceUtility.TraceEvent(TraceEventType.Verbose, TraceCode.PortSharingDuplicatedPipe, System.ServiceModel.SR.GetString("TraceCodePortSharingDuplicatedPipe"));
                }
                PipeHandle pipe = new PipeHandle(duplicateContext.Handle);
                PipeConnection innerConnection = new PipeConnection(pipe, connectionBufferSize, false, true);
                return new NamedPipeValidatingConnection(new PreReadConnection(innerConnection, duplicateContext.ReadData), this);
            }

            private IConnection BuildDuplicatedTcpConnection(TcpDuplicateContext duplicateContext, int connectionBufferSize)
            {
                if (DiagnosticUtility.ShouldTraceVerbose)
                {
                    TraceUtility.TraceEvent(TraceEventType.Verbose, TraceCode.PortSharingDuplicatedSocket, System.ServiceModel.SR.GetString("TraceCodePortSharingDuplicatedSocket"));
                }
                Socket socket = new Socket(duplicateContext.SocketInformation);
                SocketConnection innerConnection = new SocketConnection(socket, connectionBufferSize, true);
                return new TcpValidatingConnection(new PreReadConnection(innerConnection, duplicateContext.ReadData), this);
            }

            private void Cleanup(bool isAborting)
            {
                bool flag = false;
                if (this.controlSessionWithListener != null)
                {
                    if (!isAborting)
                    {
                        try
                        {
                            this.Unregister();
                            this.controlSessionWithListener.Close(ListenerConstants.RegistrationCloseTimeout);
                            flag = true;
                        }
                        catch (Exception exception)
                        {
                            if (DiagnosticUtility.IsFatal(exception))
                            {
                                throw;
                            }
                            if (DiagnosticUtility.ShouldTraceError)
                            {
                                DiagnosticUtility.ExceptionUtility.TraceHandledException(exception, TraceEventType.Error);
                            }
                        }
                    }
                    if (isAborting || !flag)
                    {
                        this.controlSessionWithListener.Abort();
                    }
                }
                if (this.channelFactory != null)
                {
                    flag = false;
                    if (!isAborting)
                    {
                        try
                        {
                            this.channelFactory.Close(ListenerConstants.RegistrationCloseTimeout);
                            flag = true;
                        }
                        catch (Exception exception2)
                        {
                            if (DiagnosticUtility.IsFatal(exception2))
                            {
                                throw;
                            }
                            if (DiagnosticUtility.ShouldTraceError)
                            {
                                DiagnosticUtility.ExceptionUtility.TraceHandledException(exception2, TraceEventType.Error);
                            }
                        }
                    }
                    if (isAborting || !flag)
                    {
                        this.channelFactory.Abort();
                    }
                }
                if (this.allowContext != null)
                {
                    this.allowContext.Dispose();
                }
            }

            public void Close()
            {
                this.Close(false);
            }

            private void Close(bool isAborting)
            {
                lock (this.ThisLock)
                {
                    if (this.closed)
                    {
                        return;
                    }
                    bool flag = false;
                    try
                    {
                        this.Cleanup(isAborting);
                        flag = true;
                    }
                    finally
                    {
                        if (!flag && !isAborting)
                        {
                            this.Close(true);
                        }
                        this.closed = true;
                    }
                }
                if (DiagnosticUtility.ShouldTraceInformation)
                {
                    TraceUtility.TraceEvent(TraceEventType.Information, TraceCode.PortSharingClosed, System.ServiceModel.SR.GetString("TraceCodePortSharingClosed"));
                }
            }

            private void CreateControlProxy()
            {
                EndpointAddress remoteAddress = new EndpointAddress(System.ServiceModel.Activation.Utility.FormatListenerEndpoint(this.serviceName, this.listenerEndPoint), new AddressHeader[0]);
                NamedPipeTransportBindingElement element = new NamedPipeTransportBindingElement();
                CustomBinding binding = new CustomBinding(new BindingElement[] { element });
                InstanceContext callbackInstance = new InstanceContext(null, this, false);
                ChannelFactory<IConnectionRegister> factory = new DuplexChannelFactory<IConnectionRegister>(callbackInstance, binding, remoteAddress);
                factory.Endpoint.Behaviors.Add(new SharedListenerProxyBehavior(this));
                IConnectionRegister register = factory.CreateChannel();
                this.channelFactory = factory;
                this.controlSessionWithListener = register as IDuplexContextChannel;
            }

            private ServiceControllerStatus ExitServiceStatus(ServiceController service, int pollMin, int pollMax, ServiceControllerStatus status)
            {
                ServiceControllerStatus status2;
                BackoffTimeoutHelper helper = new BackoffTimeoutHelper(TimeSpan.MaxValue, TimeSpan.FromMilliseconds((double) pollMax), TimeSpan.FromMilliseconds((double) pollMin));
                do
                {
                    if (this.closed)
                    {
                        return service.Status;
                    }
                    helper.WaitAndBackoff();
                    service.Refresh();
                    status2 = service.Status;
                }
                while (status2 == status);
                return status2;
            }

            private void HandleAllowDupHandlePermission(int myPid)
            {
                if (OSEnvironmentHelper.IsVistaOrGreater || !this.listenerUserSid.Equals(new SecurityIdentifier(WellKnownSidType.LocalSystemSid, null)))
                {
                    SecurityIdentifier userSidForPid;
                    try
                    {
                        userSidForPid = System.ServiceModel.Activation.Utility.GetUserSidForPid(myPid);
                    }
                    catch (Win32Exception exception)
                    {
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new CommunicationException(System.ServiceModel.SR.GetString("SharedManagerBase", new object[] { this.serviceName, System.ServiceModel.SR.GetString("SharedManagerCurrentUserSidLookupFailure", new object[] { exception.NativeErrorCode }) }), exception));
                    }
                    if (OSEnvironmentHelper.IsVistaOrGreater || !userSidForPid.Equals(this.listenerUserSid))
                    {
                        try
                        {
                            this.allowContext = AllowHelper.TryAllow(this.listenerUniqueSid.Value);
                        }
                        catch (Win32Exception exception2)
                        {
                            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new CommunicationException(System.ServiceModel.SR.GetString("SharedManagerBase", new object[] { this.serviceName, System.ServiceModel.SR.GetString("SharedManagerAllowDupHandleFailed", new object[] { this.listenerUniqueSid.Value }) }), exception2));
                        }
                        if (DiagnosticUtility.ShouldTraceInformation)
                        {
                            DiagnosticUtility.DiagnosticTrace.TraceEvent(TraceEventType.Information, TraceCode.PortSharingDupHandleGranted, System.ServiceModel.SR.GetString("TraceCodePortSharingDupHandleGranted", new object[] { this.serviceName, this.listenerUniqueSid.Value }), null, null);
                        }
                    }
                }
            }

            private bool HandleOnVia(DuplicateContext duplicateContext)
            {
                if (this.onDuplicatedViaCallback != null)
                {
                    lock (this.ThisLock)
                    {
                        if (this.onDuplicatedViaCallback == null)
                        {
                            return true;
                        }
                        if (this.onDuplicatedViaCallback != null)
                        {
                            try
                            {
                                int num;
                                this.onDuplicatedViaCallback(duplicateContext.Via, out num);
                                this.connectionBufferSize = num;
                                this.onDuplicatedViaCallback = null;
                            }
                            catch (Exception exception)
                            {
                                if (DiagnosticUtility.ShouldTraceInformation)
                                {
                                    DiagnosticUtility.ExceptionUtility.TraceHandledException(exception, TraceEventType.Information);
                                }
                                string faultCode = null;
                                if (exception is ServiceActivationException)
                                {
                                    faultCode = "http://schemas.microsoft.com/ws/2006/05/framing/faults/ServiceActivationFailed";
                                }
                                else if (exception is EndpointNotFoundException)
                                {
                                    faultCode = "http://schemas.microsoft.com/ws/2006/05/framing/faults/EndpointNotFound";
                                }
                                IConnection connection = this.BuildConnectionFromData(duplicateContext, 0x2000);
                                if (faultCode != null)
                                {
                                    this.SendFault(connection, faultCode);
                                    return false;
                                }
                                connection.Abort();
                                if (!(exception is CommunicationObjectAbortedException))
                                {
                                    throw;
                                }
                                return false;
                            }
                        }
                    }
                }
                return true;
            }

            private string HandleServiceStart(bool isReconnecting)
            {
                string listenerEndpoint = null;
                string str4;
                string sharedMemoryName = this.isTcp ? "NetTcpPortSharing/endpoint" : "NetPipeActivator/endpoint";
                this.serviceName = SharedConnectionListener.GetServiceName(this.isTcp);
                if (!isReconnecting && this.ReadEndpoint(sharedMemoryName, out listenerEndpoint))
                {
                    return listenerEndpoint;
                }
                ServiceController service = new ServiceController(this.serviceName);
                try
                {
                    ServiceControllerStatus status = service.Status;
                    if (isReconnecting && (status == ServiceControllerStatus.Running))
                    {
                        try
                        {
                            string str3 = SharedMemory.Read(sharedMemoryName);
                            if (this.listenerEndPoint != str3)
                            {
                                return str3;
                            }
                        }
                        catch (Win32Exception exception)
                        {
                            if (DiagnosticUtility.ShouldTraceWarning)
                            {
                                DiagnosticUtility.ExceptionUtility.TraceHandledException(exception, TraceEventType.Warning);
                            }
                        }
                        status = this.ExitServiceStatus(service, 50, 50, ServiceControllerStatus.Running);
                    }
                    if (status == ServiceControllerStatus.Running)
                    {
                        goto Label_021B;
                    }
                    if (!isReconnecting)
                    {
                        try
                        {
                            service.Start();
                            goto Label_01FD;
                        }
                        catch (InvalidOperationException exception2)
                        {
                            Win32Exception innerException = exception2.InnerException as Win32Exception;
                            if (innerException == null)
                            {
                                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new CommunicationException(System.ServiceModel.SR.GetString("SharedManagerBase", new object[] { this.serviceName, System.ServiceModel.SR.GetString("SharedManagerServiceStartFailureNoError") }), exception2));
                            }
                            if (innerException.NativeErrorCode == 0x422)
                            {
                                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new CommunicationException(System.ServiceModel.SR.GetString("SharedManagerBase", new object[] { this.serviceName, System.ServiceModel.SR.GetString("SharedManagerServiceStartFailureDisabled", new object[] { this.serviceName }) }), exception2));
                            }
                            if (innerException.NativeErrorCode != 0x420)
                            {
                                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new CommunicationException(System.ServiceModel.SR.GetString("SharedManagerBase", new object[] { this.serviceName, System.ServiceModel.SR.GetString("SharedManagerServiceStartFailure", new object[] { innerException.NativeErrorCode }) }), exception2));
                            }
                            goto Label_01FD;
                        }
                    }
                    switch (status)
                    {
                        case ServiceControllerStatus.StopPending:
                            status = this.ExitServiceStatus(service, 50, 0x3e8, status);
                            break;

                        case ServiceControllerStatus.Stopped:
                            status = this.ExitServiceStatus(service, 50, 0x3e8, status);
                            break;
                    }
                Label_01FD:
                    service.Refresh();
                    status = service.Status;
                    if (status == ServiceControllerStatus.StartPending)
                    {
                        status = this.ExitServiceStatus(service, 50, 50, ServiceControllerStatus.StartPending);
                    }
                Label_021B:
                    if (status != ServiceControllerStatus.Running)
                    {
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new CommunicationException(System.ServiceModel.SR.GetString("SharedManagerBase", new object[] { this.serviceName, System.ServiceModel.SR.GetString("SharedManagerServiceStartFailureNoError") })));
                    }
                }
                finally
                {
                    service.Close();
                }
                try
                {
                    str4 = SharedMemory.Read(sharedMemoryName);
                }
                catch (Win32Exception exception4)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(this.WrapEndpointReadingException(exception4));
                }
                return str4;
            }

            private void LookupListenerSid()
            {
                int pidForService;
                if (OSEnvironmentHelper.IsVistaOrGreater)
                {
                    try
                    {
                        this.listenerUniqueSid = System.ServiceModel.Activation.Utility.GetWindowsServiceSid(this.serviceName);
                        return;
                    }
                    catch (Win32Exception exception)
                    {
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new CommunicationException(System.ServiceModel.SR.GetString("SharedManagerBase", new object[] { this.serviceName, System.ServiceModel.SR.GetString("SharedManagerServiceSidLookupFailure", new object[] { exception.NativeErrorCode }) }), exception));
                    }
                }
                try
                {
                    pidForService = System.ServiceModel.Activation.Utility.GetPidForService(this.serviceName);
                }
                catch (Win32Exception exception2)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new CommunicationException(System.ServiceModel.SR.GetString("SharedManagerBase", new object[] { this.serviceName, System.ServiceModel.SR.GetString("SharedManagerServiceLookupFailure", new object[] { exception2.NativeErrorCode }) }), exception2));
                }
                try
                {
                    this.listenerUserSid = System.ServiceModel.Activation.Utility.GetUserSidForPid(pidForService);
                }
                catch (Win32Exception exception3)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new CommunicationException(System.ServiceModel.SR.GetString("SharedManagerBase", new object[] { this.serviceName, System.ServiceModel.SR.GetString("SharedManagerUserSidLookupFailure", new object[] { exception3.NativeErrorCode }) }), exception3));
                }
                try
                {
                    this.listenerUniqueSid = System.ServiceModel.Activation.Utility.GetLogonSidForPid(pidForService);
                }
                catch (Win32Exception exception4)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new CommunicationException(System.ServiceModel.SR.GetString("SharedManagerBase", new object[] { this.serviceName, System.ServiceModel.SR.GetString("SharedManagerLogonSidLookupFailure", new object[] { exception4.NativeErrorCode }) }), exception4));
                }
            }

            private void OnControlChannelShutdown()
            {
                if (!this.listenerClosed && this.opened)
                {
                    lock (this.ThisLock)
                    {
                        if (this.listenerClosed || !this.opened)
                        {
                            return;
                        }
                        this.listenerClosed = true;
                    }
                    this.parent.OnListenerFaulted(this.queueId == 0);
                }
            }

            public void Open(bool isReconnecting)
            {
                if (!this.closed)
                {
                    this.listenerEndPoint = this.HandleServiceStart(isReconnecting);
                    if (string.IsNullOrEmpty(this.listenerEndPoint))
                    {
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new CommunicationException(System.ServiceModel.SR.GetString("Sharing_EmptyListenerEndpoint", new object[] { this.serviceName })));
                    }
                    if (!this.closed)
                    {
                        this.LookupListenerSid();
                        EventWaitHandle handle = null;
                        bool flag = false;
                        lock (this.ThisLock)
                        {
                            try
                            {
                                bool flag2;
                                this.CreateControlProxy();
                                EventWaitHandleSecurity eventSecurity = new EventWaitHandleSecurity();
                                eventSecurity.AddAccessRule(new EventWaitHandleAccessRule(this.listenerUniqueSid, EventWaitHandleRights.Modify, AccessControlType.Allow));
                                handle = new EventWaitHandle(false, EventResetMode.ManualReset, @"Global\" + this.securityEventName, out flag2, eventSecurity);
                                if (!flag2)
                                {
                                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new CommunicationException(System.ServiceModel.SR.GetString("SharedManagerBase", new object[] { this.serviceName, System.ServiceModel.SR.GetString("SharedManagerServiceSecurityFailed") })));
                                }
                                this.Register();
                                if (!handle.WaitOne(0, false))
                                {
                                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new CommunicationException(System.ServiceModel.SR.GetString("SharedManagerBase", new object[] { this.serviceName, System.ServiceModel.SR.GetString("SharedManagerServiceSecurityFailed") })));
                                }
                                if (DiagnosticUtility.ShouldTraceInformation)
                                {
                                    TraceUtility.TraceEvent(TraceEventType.Information, TraceCode.PortSharingListening, System.ServiceModel.SR.GetString("TraceCodePortSharingListening"));
                                }
                                this.opened = true;
                                flag = true;
                            }
                            finally
                            {
                                if (handle != null)
                                {
                                    handle.Close();
                                }
                                if (!flag)
                                {
                                    this.Cleanup(true);
                                    this.closed = true;
                                }
                            }
                        }
                    }
                }
            }

            private bool ReadEndpoint(string sharedMemoryName, out string listenerEndpoint)
            {
                bool flag;
                try
                {
                    if (SharedMemory.Read(sharedMemoryName, out listenerEndpoint))
                    {
                        return true;
                    }
                    if (DiagnosticUtility.ShouldTraceInformation)
                    {
                        DiagnosticUtility.DiagnosticTrace.TraceEvent(TraceEventType.Information, TraceCode.SharedManagerServiceEndpointNotExist, System.ServiceModel.SR.GetString("TraceCodeSharedManagerServiceEndpointNotExist", new object[] { this.serviceName }), null, null);
                    }
                    flag = false;
                }
                catch (Win32Exception exception)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(this.WrapEndpointReadingException(exception));
                }
                return flag;
            }

            private void Register()
            {
                Version version = Assembly.GetExecutingAssembly().GetName().Version;
                int id = Process.GetCurrentProcess().Id;
                this.HandleAllowDupHandlePermission(id);
                ListenerExceptionStatus status = ((IConnectionRegister) this.controlSessionWithListener).Register(version, id, this.baseAddress, this.queueId, this.token, this.securityEventName);
                switch (status)
                {
                    case ListenerExceptionStatus.ConflictingRegistration:
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new AddressAlreadyInUseException(System.ServiceModel.SR.GetString("SharedManagerBase", new object[] { this.serviceName, System.ServiceModel.SR.GetString("SharedManagerConflictingRegistration") })));

                    case ListenerExceptionStatus.FailedToListen:
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new AddressAlreadyInUseException(System.ServiceModel.SR.GetString("SharedManagerBase", new object[] { this.serviceName, System.ServiceModel.SR.GetString("SharedManagerFailedToListen") })));

                    case ListenerExceptionStatus.Success:
                        return;
                }
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new CommunicationException(System.ServiceModel.SR.GetString("SharedManagerBase", new object[] { this.serviceName, System.ServiceModel.SR.GetString("SharedManager" + status) })));
            }

            private void SendFault(IConnection connection, string faultCode)
            {
                try
                {
                    if (drainBuffer == null)
                    {
                        drainBuffer = new byte[0x400];
                    }
                    InitialServerConnectionReader.SendFault(connection, faultCode, drainBuffer, ListenerConstants.SharedSendTimeout, 0x10000);
                }
                catch (Exception exception)
                {
                    if (DiagnosticUtility.IsFatal(exception))
                    {
                        throw;
                    }
                    if (DiagnosticUtility.ShouldTraceError)
                    {
                        DiagnosticUtility.ExceptionUtility.TraceHandledException(exception, TraceEventType.Error);
                    }
                }
            }

            IAsyncResult IConnectionDuplicator.BeginDuplicate(DuplicateContext duplicateContext, AsyncCallback callback, object state)
            {
                IAsyncResult result2;
                try
                {
                    if (!this.HandleOnVia(duplicateContext))
                    {
                        return new SharedConnectionListener.DuplicateConnectionAsyncResult(callback, state);
                    }
                    SharedConnectionListener.DuplicateConnectionAsyncResult result = new SharedConnectionListener.DuplicateConnectionAsyncResult(this.BuildConnectionFromData(duplicateContext, this.connectionBufferSize), callback, state);
                    this.parent.OnConnectionAvailable(result);
                    result2 = result;
                }
                catch (Exception exception)
                {
                    if (DiagnosticUtility.ShouldTraceError)
                    {
                        DiagnosticUtility.ExceptionUtility.TraceHandledException(exception, TraceEventType.Error);
                    }
                    throw;
                }
                return result2;
            }

            void IConnectionDuplicator.EndDuplicate(IAsyncResult result)
            {
                SharedConnectionListener.DuplicateConnectionAsyncResult.End(result);
            }

            void IInputSessionShutdown.ChannelFaulted(IDuplexContextChannel channel)
            {
                this.OnControlChannelShutdown();
            }

            void IInputSessionShutdown.DoneReceiving(IDuplexContextChannel channel)
            {
                this.OnControlChannelShutdown();
            }

            private void Unregister()
            {
                ((IConnectionRegister) this.controlSessionWithListener).Unregister();
            }

            private bool ValidateUriRoute(Uri uri, IPAddress address, int port)
            {
                bool flag;
                try
                {
                    lock (this.ThisLock)
                    {
                        if (this.closed)
                        {
                            return false;
                        }
                        flag = ((IConnectionRegister) this.controlSessionWithListener).ValidateUriRoute(uri, address, port);
                    }
                }
                catch (Exception exception)
                {
                    if (DiagnosticUtility.ShouldTraceError)
                    {
                        DiagnosticUtility.ExceptionUtility.TraceHandledException(exception, TraceEventType.Error);
                    }
                    if (!(exception is CommunicationException) && !(exception is System.ServiceProcess.TimeoutException))
                    {
                        throw;
                    }
                    flag = false;
                }
                return flag;
            }

            private Exception WrapEndpointReadingException(Win32Exception exception)
            {
                string str;
                if (exception.NativeErrorCode == 2)
                {
                    str = System.ServiceModel.SR.GetString("SharedEndpointReadNotFound", new object[] { this.baseAddress.BaseAddress.ToString(), this.serviceName });
                }
                else if (exception.NativeErrorCode == 5)
                {
                    str = System.ServiceModel.SR.GetString("SharedEndpointReadDenied", new object[] { this.baseAddress.BaseAddress.ToString() });
                }
                else
                {
                    str = System.ServiceModel.SR.GetString("SharedManagerBase", new object[] { this.serviceName, System.ServiceModel.SR.GetString("SharedManagerServiceEndpointReadFailure", new object[] { exception.NativeErrorCode }) });
                }
                return new CommunicationException(str, exception);
            }

            private object ThisLock =>
                this.syncRoot;

            private class NamedPipeValidatingConnection : DelegatingConnection
            {
                private bool initialValidation;
                private SharedConnectionListener.SharedListenerProxy listenerProxy;

                public NamedPipeValidatingConnection(IConnection connection, SharedConnectionListener.SharedListenerProxy listenerProxy) : base(connection)
                {
                    this.listenerProxy = listenerProxy;
                    this.initialValidation = true;
                }

                public override bool Validate(Uri uri)
                {
                    if (this.initialValidation)
                    {
                        this.initialValidation = false;
                        return true;
                    }
                    return this.listenerProxy.ValidateUriRoute(uri, null, -1);
                }
            }

            private class SharedListenerProxyBehavior : IEndpointBehavior
            {
                private SharedConnectionListener.SharedListenerProxy proxy;

                public SharedListenerProxyBehavior(SharedConnectionListener.SharedListenerProxy proxy)
                {
                    this.proxy = proxy;
                }

                public void AddBindingParameters(ServiceEndpoint serviceEndpoint, BindingParameterCollection bindingParameters)
                {
                }

                public void ApplyClientBehavior(ServiceEndpoint serviceEndpoint, ClientRuntime behavior)
                {
                    behavior.DispatchRuntime.InputSessionShutdownHandlers.Add(this.proxy);
                }

                public void ApplyDispatchBehavior(ServiceEndpoint serviceEndpoint, EndpointDispatcher endpointDispatcher)
                {
                }

                public void Validate(ServiceEndpoint serviceEndpoint)
                {
                }
            }

            private class TcpValidatingConnection : DelegatingConnection
            {
                private bool initialValidation;
                private IPAddress ipAddress;
                private SharedConnectionListener.SharedListenerProxy listenerProxy;
                private int port;

                public TcpValidatingConnection(IConnection connection, SharedConnectionListener.SharedListenerProxy listenerProxy) : base(connection)
                {
                    this.listenerProxy = listenerProxy;
                    Socket coreTransport = (Socket) connection.GetCoreTransport();
                    this.ipAddress = ((IPEndPoint) coreTransport.LocalEndPoint).Address;
                    this.port = ((IPEndPoint) coreTransport.LocalEndPoint).Port;
                    this.initialValidation = true;
                }

                public override bool Validate(Uri uri)
                {
                    if (this.initialValidation)
                    {
                        this.initialValidation = false;
                        return true;
                    }
                    return this.listenerProxy.ValidateUriRoute(uri, this.ipAddress, this.port);
                }
            }
        }
    }
}

