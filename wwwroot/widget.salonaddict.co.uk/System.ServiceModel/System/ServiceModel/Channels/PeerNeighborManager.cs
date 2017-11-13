namespace System.ServiceModel.Channels
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Net;
    using System.Runtime.CompilerServices;
    using System.ServiceModel;
    using System.ServiceModel.Description;
    using System.ServiceModel.Diagnostics;
    using System.ServiceModel.Dispatcher;
    using System.Threading;

    internal class PeerNeighborManager
    {
        private PeerNodeConfig config;
        private List<IPeerNeighbor> connectedNeighborList;
        private PeerIPHelper ipHelper;
        private bool isOnline;
        private IPeerNodeMessageHandling messageHandler;
        private List<PeerNeighbor> neighborList;
        private PeerService service;
        private Binding serviceBinding;
        private ManualResetEvent shutdownEvent;
        private State state;
        private object thisLock;
        private PeerNodeTraceRecord traceRecord;

        public event EventHandler<PeerNeighborCloseEventArgs> NeighborClosed;

        public event EventHandler<PeerNeighborCloseEventArgs> NeighborClosing;

        public event EventHandler NeighborConnected;

        public event EventHandler NeighborOpened;

        public event EventHandler Offline;

        public event EventHandler Online;

        public PeerNeighborManager(PeerIPHelper ipHelper, PeerNodeConfig config) : this(ipHelper, config, null)
        {
        }

        public PeerNeighborManager(PeerIPHelper ipHelper, PeerNodeConfig config, IPeerNodeMessageHandling messageHandler)
        {
            this.neighborList = new List<PeerNeighbor>();
            this.connectedNeighborList = new List<IPeerNeighbor>();
            this.ipHelper = ipHelper;
            this.messageHandler = messageHandler;
            this.config = config;
            this.thisLock = new object();
            this.traceRecord = new PeerNodeTraceRecord(config.NodeId);
            this.state = State.Created;
        }

        private void Abort(PeerNeighbor[] neighbors)
        {
            foreach (PeerNeighbor neighbor in neighbors)
            {
                neighbor.Abort(PeerCloseReason.LeavingMesh, PeerCloseInitiator.LocalNode);
            }
        }

        public IAsyncResult BeginOpenNeighbor(PeerNodeAddress remoteAddress, TimeSpan timeout, AsyncCallback callback, object asyncState)
        {
            this.ThrowIfNotOpen();
            ReadOnlyCollection<IPAddress> ipAddresses = this.ipHelper.SortAddresses(remoteAddress.IPAddresses);
            PeerNodeAddress address = new PeerNodeAddress(remoteAddress.EndpointAddress, ipAddresses);
            return this.BeginOpenNeighborInternal(address, timeout, callback, asyncState);
        }

        internal IAsyncResult BeginOpenNeighborInternal(PeerNodeAddress remoteAddress, TimeSpan timeout, AsyncCallback callback, object asyncState)
        {
            PeerNeighbor neighbor = new PeerNeighbor(this.config, this.messageHandler);
            this.RegisterForNeighborEvents(neighbor);
            return new NeighborOpenAsyncResult(neighbor, remoteAddress, this.serviceBinding, this.service, new ClosedCallback(this.Closed), timeout, callback, asyncState);
        }

        private void Cleanup(bool graceful)
        {
            lock (this.ThisLock)
            {
                if (graceful && (this.shutdownEvent != null))
                {
                    this.shutdownEvent.Close();
                }
                this.state = State.Shutdown;
            }
        }

        private void ClearNeighborList()
        {
            lock (this.ThisLock)
            {
                this.neighborList.Clear();
                this.connectedNeighborList.Clear();
            }
        }

        public void Close()
        {
            lock (this.ThisLock)
            {
                this.state = State.Closed;
            }
        }

        private bool Closed() => 
            (this.state != State.Opened);

        public void CloseNeighbor(IPeerNeighbor neighbor, PeerCloseReason closeReason, PeerCloseInitiator closeInitiator)
        {
            this.CloseNeighbor(neighbor, closeReason, closeInitiator, null);
        }

        public void CloseNeighbor(IPeerNeighbor neighbor, PeerCloseReason closeReason, PeerCloseInitiator closeInitiator, Exception closeException)
        {
            PeerNeighbor item = (PeerNeighbor) neighbor;
            lock (this.ThisLock)
            {
                if (this.state == State.Created)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperInternal(false);
                }
                if (!this.neighborList.Contains(item))
                {
                    return;
                }
            }
            if (closeReason != PeerCloseReason.InvalidNeighbor)
            {
                if (!item.IsClosing)
                {
                    this.InvokeAsyncNeighborClose(item, closeReason, closeInitiator, closeException, null);
                }
            }
            else
            {
                item.Abort(closeReason, closeInitiator);
            }
        }

        public IPeerNeighbor EndOpenNeighbor(IAsyncResult result) => 
            NeighborOpenAsyncResult.End(result);

        public IPeerNeighbor FindDuplicateNeighbor(PeerNodeAddress address) => 
            this.FindDuplicateNeighbor(address, null);

        public IPeerNeighbor FindDuplicateNeighbor(ulong nodeId) => 
            this.FindDuplicateNeighbor(nodeId, null);

        public IPeerNeighbor FindDuplicateNeighbor(PeerNodeAddress address, IPeerNeighbor skipNeighbor)
        {
            PeerNeighbor neighbor = null;
            lock (this.ThisLock)
            {
                foreach (PeerNeighbor neighbor2 in this.neighborList)
                {
                    if ((((neighbor2 != ((PeerNeighbor) skipNeighbor)) && (neighbor2.ListenAddress != null)) && ((neighbor2.ListenAddress.ServicePath == address.ServicePath) && !neighbor2.IsClosing)) && (neighbor2.State < PeerNeighborState.Disconnecting))
                    {
                        return neighbor2;
                    }
                }
                return neighbor;
            }
            return neighbor;
        }

        public IPeerNeighbor FindDuplicateNeighbor(ulong nodeId, IPeerNeighbor skipNeighbor)
        {
            PeerNeighbor neighbor = null;
            if (nodeId != 0L)
            {
                lock (this.ThisLock)
                {
                    foreach (PeerNeighbor neighbor2 in this.neighborList)
                    {
                        if (((neighbor2 != ((PeerNeighbor) skipNeighbor)) && (neighbor2.NodeId == nodeId)) && (!neighbor2.IsClosing && (neighbor2.State < PeerNeighborState.Disconnecting)))
                        {
                            return neighbor2;
                        }
                    }
                    return neighbor;
                }
            }
            return neighbor;
        }

        private static void FireEvent(EventHandler handler, PeerNeighborManager manager)
        {
            if (handler != null)
            {
                handler(manager, EventArgs.Empty);
            }
        }

        private static void FireEvent(EventHandler handler, PeerNeighbor neighbor)
        {
            if (handler != null)
            {
                handler(neighbor, EventArgs.Empty);
            }
        }

        private static void FireEvent(EventHandler<PeerNeighborCloseEventArgs> handler, PeerNeighbor neighbor, PeerCloseReason closeReason, PeerCloseInitiator closeInitiator, Exception closeException)
        {
            if (handler != null)
            {
                PeerNeighborCloseEventArgs e = new PeerNeighborCloseEventArgs(closeReason, closeInitiator, closeException);
                handler(neighbor, e);
            }
        }

        public List<IPeerNeighbor> GetConnectedNeighbors()
        {
            lock (this.ThisLock)
            {
                return new List<IPeerNeighbor>(this.connectedNeighborList);
            }
        }

        public IPeerNeighbor GetNeighborFromProxy(IPeerProxy proxy)
        {
            PeerNeighbor neighbor = null;
            lock (this.ThisLock)
            {
                if (this.state == State.Opened)
                {
                    foreach (PeerNeighbor neighbor2 in this.neighborList)
                    {
                        if (neighbor2.Proxy == proxy)
                        {
                            return neighbor2;
                        }
                    }
                }
                return neighbor;
            }
            return neighbor;
        }

        private void InvokeAsyncNeighborClose(PeerNeighbor neighbor, PeerCloseReason closeReason, PeerCloseInitiator closeInitiator, Exception closeException, IAsyncResult endResult)
        {
            try
            {
                if (endResult == null)
                {
                    IAsyncResult result = neighbor.BeginClose(closeReason, closeInitiator, closeException, DiagnosticUtility.ThunkAsyncCallback(new AsyncCallback(this.OnNeighborClosedCallback)), neighbor);
                    if (result.CompletedSynchronously)
                    {
                        neighbor.EndClose(result);
                    }
                }
                else
                {
                    neighbor.EndClose(endResult);
                }
            }
            catch (Exception exception)
            {
                if (DiagnosticUtility.IsFatal(exception))
                {
                    throw;
                }
                DiagnosticUtility.ExceptionUtility.TraceHandledException(exception, TraceEventType.Information);
                neighbor.TraceEventHelper(TraceEventType.Warning, TraceCode.PeerNeighborCloseFailed, exception);
                if ((!(exception is InvalidOperationException) && !(exception is CommunicationException)) && !(exception is TimeoutException))
                {
                    throw;
                }
                neighbor.Abort();
            }
        }

        private void OnNeighborClosed(object source, EventArgs args)
        {
            this.RemoveNeighbor((PeerNeighbor) source);
        }

        private void OnNeighborClosedCallback(IAsyncResult result)
        {
            if (!result.CompletedSynchronously)
            {
                this.InvokeAsyncNeighborClose((PeerNeighbor) result.AsyncState, PeerCloseReason.None, PeerCloseInitiator.LocalNode, null, result);
            }
        }

        private void OnNeighborClosing(object source, EventArgs args)
        {
            lock (this.ThisLock)
            {
                this.connectedNeighborList.Remove((IPeerNeighbor) source);
            }
        }

        private void OnNeighborConnected(object source, EventArgs args)
        {
            PeerNeighbor item = (PeerNeighbor) source;
            bool flag = false;
            bool flag2 = false;
            lock (this.ThisLock)
            {
                if (this.neighborList.Contains(item))
                {
                    flag = true;
                    this.connectedNeighborList.Add(item);
                    if (!this.isOnline)
                    {
                        this.isOnline = true;
                        flag2 = true;
                    }
                }
            }
            if (flag)
            {
                FireEvent(this.NeighborConnected, item);
            }
            else
            {
                item.TraceEventHelper(TraceEventType.Warning, TraceCode.PeerNeighborNotFound);
            }
            if (flag2)
            {
                if (DiagnosticUtility.ShouldTraceInformation)
                {
                    TraceUtility.TraceEvent(TraceEventType.Information, TraceCode.PeerNeighborManagerOnline, this.traceRecord, this, null);
                }
                FireEvent(this.Online, this);
            }
        }

        private void OnNeighborOpened(object source, EventArgs args)
        {
            PeerNeighbor item = (PeerNeighbor) source;
            bool flag = false;
            lock (this.ThisLock)
            {
                if (this.state == State.Opened)
                {
                    if (item.State != PeerNeighborState.Opened)
                    {
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperInternal(false);
                    }
                    this.neighborList.Add(item);
                    flag = true;
                }
            }
            if (flag)
            {
                FireEvent(this.NeighborOpened, item);
            }
            else
            {
                item.Abort();
                item.TraceEventHelper(TraceEventType.Warning, TraceCode.PeerNeighborNotAccepted);
            }
        }

        public void Open(Binding serviceBinding, PeerService service)
        {
            lock (this.ThisLock)
            {
                this.service = service;
                this.serviceBinding = serviceBinding;
                if (this.state != State.Created)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperInternal(false);
                }
                this.state = State.Opened;
            }
        }

        public bool PingNeighbor(IPeerNeighbor peer)
        {
            bool flag = true;
            Message request = Message.CreateMessage(MessageVersion.Soap12WSAddressing10, "http://schemas.microsoft.com/net/2006/05/peer/Ping");
            try
            {
                peer.Ping(request);
            }
            catch (Exception exception)
            {
                if (DiagnosticUtility.IsFatal(exception))
                {
                    throw;
                }
                DiagnosticUtility.ExceptionUtility.TraceHandledException(exception, TraceEventType.Information);
                peer.Abort(PeerCloseReason.InternalFailure, PeerCloseInitiator.LocalNode);
                flag = false;
            }
            return flag;
        }

        public void PingNeighbors()
        {
            foreach (IPeerNeighbor neighbor in this.GetConnectedNeighbors())
            {
                this.PingNeighbor(neighbor);
            }
        }

        public bool ProcessIncomingChannel(IClientChannel channel)
        {
            IPeerProxy callbackInstance = (IPeerProxy) channel;
            if (this.state == State.Opened)
            {
                PeerNeighbor neighbor = new PeerNeighbor(this.config, this.messageHandler);
                this.RegisterForNeighborEvents(neighbor);
                neighbor.Open(callbackInstance);
                return true;
            }
            if (DiagnosticUtility.ShouldTraceWarning)
            {
                TraceUtility.TraceEvent(TraceEventType.Warning, TraceCode.PeerNeighborNotAccepted, this.traceRecord, this, null);
            }
            return false;
        }

        private void RegisterForNeighborEvents(PeerNeighbor neighbor)
        {
            neighbor.Opened += new EventHandler(this.OnNeighborOpened);
            neighbor.Connected += new EventHandler(this.OnNeighborConnected);
            neighbor.Closed += new EventHandler(this.OnNeighborClosed);
            neighbor.Closing += this.NeighborClosing;
            neighbor.Disconnecting += new EventHandler(this.OnNeighborClosing);
            neighbor.Disconnected += new EventHandler(this.OnNeighborClosing);
        }

        private void RemoveNeighbor(PeerNeighbor neighbor)
        {
            bool flag = false;
            bool flag2 = false;
            lock (this.ThisLock)
            {
                if (this.neighborList.Contains(neighbor))
                {
                    flag = true;
                    this.neighborList.Remove(neighbor);
                    this.connectedNeighborList.Remove(neighbor);
                    if (this.isOnline && (this.connectedNeighborList.Count == 0))
                    {
                        this.isOnline = false;
                        flag2 = true;
                    }
                    if ((this.neighborList.Count == 0) && (this.shutdownEvent != null))
                    {
                        this.shutdownEvent.Set();
                    }
                }
            }
            if (flag)
            {
                FireEvent(this.NeighborClosed, neighbor, neighbor.CloseReason, neighbor.CloseInitiator, neighbor.CloseException);
            }
            else if (DiagnosticUtility.ShouldTraceWarning)
            {
                neighbor.TraceEventHelper(TraceEventType.Warning, TraceCode.PeerNeighborNotFound);
            }
            if (flag2)
            {
                if (DiagnosticUtility.ShouldTraceInformation)
                {
                    TraceUtility.TraceEvent(TraceEventType.Information, TraceCode.PeerNeighborManagerOffline, this.traceRecord, this, null);
                }
                FireEvent(this.Offline, this);
            }
        }

        public void Shutdown(bool graceful, TimeSpan timeout)
        {
            PeerNeighbor[] neighbors = null;
            TimeoutHelper helper = new TimeoutHelper(timeout);
            try
            {
                lock (this.ThisLock)
                {
                    if ((this.state == State.Shutdown) || (this.state == State.Closed))
                    {
                        return;
                    }
                    this.state = State.ShuttingDown;
                    neighbors = this.neighborList.ToArray();
                    if (graceful && (neighbors.Length > 0))
                    {
                        this.shutdownEvent = new ManualResetEvent(false);
                    }
                }
                if (graceful)
                {
                    this.Shutdown(neighbors, helper.RemainingTime());
                }
                else
                {
                    this.Abort(neighbors);
                }
            }
            catch (Exception exception)
            {
                if (DiagnosticUtility.IsFatal(exception))
                {
                    throw;
                }
                try
                {
                    this.ClearNeighborList();
                }
                catch (Exception exception2)
                {
                    if (DiagnosticUtility.IsFatal(exception2))
                    {
                        throw;
                    }
                    DiagnosticUtility.ExceptionUtility.TraceHandledException(exception2, TraceEventType.Information);
                }
                throw;
            }
            finally
            {
                this.Cleanup(graceful);
            }
        }

        private void Shutdown(PeerNeighbor[] neighbors, TimeSpan timeout)
        {
            TimeoutHelper helper = new TimeoutHelper(timeout);
            foreach (PeerNeighbor neighbor in neighbors)
            {
                this.CloseNeighbor(neighbor, PeerCloseReason.LeavingMesh, PeerCloseInitiator.LocalNode, null);
            }
            if ((neighbors.Length > 0) && !TimeoutHelper.WaitOne(this.shutdownEvent, helper.RemainingTime(), false))
            {
                this.Abort(neighbors);
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new TimeoutException());
            }
        }

        public IPeerNeighbor SlowestNeighbor()
        {
            List<IPeerNeighbor> connectedNeighbors = this.GetConnectedNeighbors();
            IPeerNeighbor neighbor = null;
            UtilityExtension utility = null;
            int pendingMessages = 0x20;
            foreach (IPeerNeighbor neighbor2 in connectedNeighbors)
            {
                utility = neighbor2.Utility;
                if (((utility != null) && neighbor2.IsConnected) && (utility.PendingMessages > pendingMessages))
                {
                    neighbor = neighbor2;
                    pendingMessages = utility.PendingMessages;
                }
            }
            return neighbor;
        }

        private void ThrowIfNotOpen()
        {
            if (this.state == State.Created)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperInternal(false);
            }
            if (this.Closed())
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ObjectDisposedException(this.ToString()));
            }
        }

        public int ConnectedNeighborCount =>
            this.connectedNeighborList.Count;

        public bool IsOnline =>
            this.isOnline;

        public int NeighborCount =>
            this.neighborList.Count;

        public int NonClosingNeighborCount
        {
            get
            {
                int num = 0;
                foreach (PeerNeighbor neighbor in this.connectedNeighborList)
                {
                    if (!neighbor.IsClosing)
                    {
                        num++;
                    }
                }
                return num;
            }
        }

        private object ThisLock =>
            this.thisLock;

        private delegate bool ClosedCallback();

        private class NeighborOpenAsyncResult : AsyncResult
        {
            private PeerNeighborManager.PeerNeighbor neighbor;

            public NeighborOpenAsyncResult(PeerNeighborManager.PeerNeighbor neighbor, PeerNodeAddress remoteAddress, Binding binding, PeerService service, PeerNeighborManager.ClosedCallback closedCallback, TimeSpan timeout, AsyncCallback callback, object state) : base(callback, state)
            {
                this.neighbor = neighbor;
                IAsyncResult result = null;
                try
                {
                    result = neighbor.BeginOpen(remoteAddress, binding, service, closedCallback, timeout, DiagnosticUtility.ThunkAsyncCallback(new AsyncCallback(this.OnOpen)), null);
                    if (result.CompletedSynchronously)
                    {
                        neighbor.EndOpen(result);
                    }
                }
                catch (Exception exception)
                {
                    if (DiagnosticUtility.IsFatal(exception))
                    {
                        throw;
                    }
                    neighbor.TraceEventHelper(TraceEventType.Warning, TraceCode.PeerNeighborOpenFailed);
                    throw;
                }
                if (result.CompletedSynchronously)
                {
                    base.Complete(true);
                }
            }

            public static IPeerNeighbor End(IAsyncResult result) => 
                AsyncResult.End<PeerNeighborManager.NeighborOpenAsyncResult>(result).neighbor;

            private void OnOpen(IAsyncResult result)
            {
                if (!result.CompletedSynchronously)
                {
                    Exception exception = null;
                    try
                    {
                        this.neighbor.EndOpen(result);
                    }
                    catch (Exception exception2)
                    {
                        if (DiagnosticUtility.IsFatal(exception2))
                        {
                            throw;
                        }
                        DiagnosticUtility.ExceptionUtility.TraceHandledException(exception2, TraceEventType.Information);
                        this.neighbor.TraceEventHelper(TraceEventType.Warning, TraceCode.PeerNeighborOpenFailed);
                        exception = exception2;
                    }
                    base.Complete(result.CompletedSynchronously, exception);
                }
            }
        }

        private class PeerNeighbor : IPeerNeighbor, IExtensibleObject<IPeerNeighbor>, IInputSessionShutdown
        {
            private ChannelFactory<IPeerProxy> channelFactory;
            private Exception closeException;
            private PeerCloseInitiator closeInitiator = PeerCloseInitiator.LocalNode;
            private PeerCloseReason closeReason = PeerCloseReason.None;
            private PeerNodeConfig config;
            private IPAddress connectIPAddress;
            private ExtensionCollection<IPeerNeighbor> extensions;
            private bool initiator;
            private bool isClosing;
            private PeerNodeAddress listenAddress;
            private IPeerNodeMessageHandling messageHandler;
            private ulong nodeId;
            private IPeerProxy proxy;
            private IClientChannel proxyChannel;
            private PeerNeighborState state;
            private object thisLock = new object();
            private UtilityExtension utility;

            public event EventHandler Closed;

            public event EventHandler<PeerNeighborCloseEventArgs> Closing;

            public event EventHandler Connected;

            public event EventHandler Disconnected;

            public event EventHandler Disconnecting;

            public event EventHandler Opened;

            public PeerNeighbor(PeerNodeConfig config, IPeerNodeMessageHandling messageHandler)
            {
                this.config = config;
                this.state = PeerNeighborState.Created;
                this.extensions = new ExtensionCollection<IPeerNeighbor>(this, this.thisLock);
                this.messageHandler = messageHandler;
            }

            public void Abort()
            {
                if (this.channelFactory != null)
                {
                    this.channelFactory.Abort();
                }
                else
                {
                    this.proxyChannel.Abort();
                }
            }

            public void Abort(PeerCloseReason reason, PeerCloseInitiator closeInit)
            {
                lock (this.ThisLock)
                {
                    if (!this.isClosing)
                    {
                        this.isClosing = true;
                        this.closeReason = reason;
                        this.closeInitiator = closeInit;
                    }
                }
                this.Abort();
            }

            public IAsyncResult BeginClose(PeerCloseReason reason, PeerCloseInitiator closeInit, Exception exception, AsyncCallback callback, object asyncState)
            {
                bool flag = false;
                lock (this.ThisLock)
                {
                    if (!this.isClosing)
                    {
                        flag = true;
                        this.isClosing = true;
                        this.closeReason = reason;
                        this.closeInitiator = closeInit;
                        this.closeException = exception;
                    }
                }
                if (flag)
                {
                    EventHandler<PeerNeighborCloseEventArgs> closing = this.Closing;
                    if (closing != null)
                    {
                        try
                        {
                            PeerNeighborCloseEventArgs e = new PeerNeighborCloseEventArgs(reason, this.closeInitiator, exception);
                            closing(this, e);
                        }
                        catch (Exception exception2)
                        {
                            if (DiagnosticUtility.IsFatal(exception2))
                            {
                                throw;
                            }
                            this.Abort();
                            throw;
                        }
                    }
                }
                if (this.channelFactory != null)
                {
                    return this.channelFactory.BeginClose(callback, asyncState);
                }
                return this.proxyChannel.BeginClose(callback, asyncState);
            }

            public IAsyncResult BeginOpen(PeerNodeAddress remoteAddress, Binding binding, PeerService service, PeerNeighborManager.ClosedCallback closedCallback, TimeSpan timeout, AsyncCallback callback, object asyncState)
            {
                this.initiator = true;
                this.listenAddress = remoteAddress;
                return new OpenAsyncResult(this, remoteAddress, binding, service, closedCallback, timeout, callback, this.state);
            }

            public IAsyncResult BeginOpenProxy(EndpointAddress remoteAddress, Binding binding, InstanceContext instanceContext, TimeSpan timeout, AsyncCallback callback, object state)
            {
                TimeoutHelper helper = new TimeoutHelper(timeout);
                if (this.channelFactory != null)
                {
                    this.Abort();
                }
                EndpointAddressBuilder builder = new EndpointAddressBuilder(remoteAddress) {
                    Uri = this.config.GetMeshUri()
                };
                this.channelFactory = new DuplexChannelFactory<IPeerProxy>(instanceContext, binding, builder.ToEndpointAddress());
                this.channelFactory.Endpoint.Behaviors.Add(new ClientViaBehavior(remoteAddress.Uri));
                this.channelFactory.Endpoint.Behaviors.Add(new PeerNeighborManager.PeerNeighborBehavior(this));
                this.channelFactory.Endpoint.Contract.Behaviors.Add(new PeerOperationSelectorBehavior(this.messageHandler));
                this.config.SecurityManager.ApplyClientSecurity(this.channelFactory);
                this.channelFactory.Open(helper.RemainingTime());
                this.Proxy = this.channelFactory.CreateChannel();
                IAsyncResult result = this.proxyChannel.BeginOpen(helper.RemainingTime(), callback, state);
                if (result.CompletedSynchronously)
                {
                    this.proxyChannel.EndOpen(result);
                }
                return result;
            }

            public IAsyncResult BeginSend(Message message, AsyncCallback callback, object asyncState) => 
                this.proxy.BeginSend(message, callback, asyncState);

            public IAsyncResult BeginSend(Message message, TimeSpan timeout, AsyncCallback callback, object asyncState) => 
                this.proxy.BeginSend(message, timeout, callback, asyncState);

            public void CleanupProxy()
            {
                this.channelFactory.Abort();
            }

            public void EndClose(IAsyncResult result)
            {
                if (this.channelFactory != null)
                {
                    this.channelFactory.EndClose(result);
                }
                else
                {
                    this.proxyChannel.EndClose(result);
                }
            }

            public void EndOpen(IAsyncResult result)
            {
                OpenAsyncResult.End(result);
            }

            public void EndOpenProxy(IAsyncResult result)
            {
                if (!result.CompletedSynchronously)
                {
                    this.proxyChannel.EndOpen(result);
                }
            }

            public void EndSend(IAsyncResult result)
            {
                this.proxy.EndSend(result);
            }

            private void OnChannelClosed(object source, EventArgs args)
            {
                if (this.state < PeerNeighborState.Closed)
                {
                    this.OnChannelClosedOrFaulted(PeerCloseReason.Closed);
                }
                if ((this.closeInitiator != PeerCloseInitiator.LocalNode) && (this.channelFactory != null))
                {
                    this.channelFactory.Abort();
                }
            }

            private void OnChannelClosedOrFaulted(PeerCloseReason reason)
            {
                lock (this.ThisLock)
                {
                    PeerNeighborState previousState = this.state;
                    this.state = PeerNeighborState.Closed;
                    if (!this.isClosing)
                    {
                        this.isClosing = true;
                        this.closeReason = reason;
                        this.closeInitiator = PeerCloseInitiator.RemoteNode;
                    }
                    this.TraceClosedEvent(previousState);
                }
                this.OnStateChanged(PeerNeighborState.Closed);
            }

            private void OnChannelFaulted(object source, EventArgs args)
            {
                try
                {
                    this.OnChannelClosedOrFaulted(PeerCloseReason.Faulted);
                }
                finally
                {
                    this.Abort();
                }
            }

            private void OnChannelOpened(object source, EventArgs args)
            {
                this.SetState(PeerNeighborState.Opened, SetStateBehavior.TrySet);
            }

            private void OnStateChanged(PeerNeighborState newState)
            {
                EventHandler opened = null;
                switch (newState)
                {
                    case PeerNeighborState.Opened:
                        opened = this.Opened;
                        break;

                    case PeerNeighborState.Connected:
                        opened = this.Connected;
                        break;

                    case PeerNeighborState.Disconnecting:
                        opened = this.Disconnecting;
                        break;

                    case PeerNeighborState.Disconnected:
                        opened = this.Disconnected;
                        break;

                    case PeerNeighborState.Closed:
                        opened = this.Closed;
                        break;
                }
                if (opened != null)
                {
                    opened(this, EventArgs.Empty);
                }
            }

            public void Open(IPeerProxy callbackInstance)
            {
                this.initiator = false;
                this.Proxy = callbackInstance;
            }

            public void Ping(Message request)
            {
                this.proxy.Ping(request);
            }

            private void RegisterForChannelEvents()
            {
                this.state = PeerNeighborState.Created;
                this.proxyChannel.Opened += new EventHandler(this.OnChannelOpened);
                this.proxyChannel.Closed += new EventHandler(this.OnChannelClosed);
                this.proxyChannel.Faulted += new EventHandler(this.OnChannelFaulted);
            }

            public Message RequestSecurityToken(Message request) => 
                this.proxy.ProcessRequestSecurityToken(request);

            public void Send(Message message)
            {
                this.proxy.Send(message);
            }

            private bool SetState(PeerNeighborState newState, SetStateBehavior behavior)
            {
                bool flag = false;
                lock (this.ThisLock)
                {
                    PeerNeighborState previousOrAttemptedState = this.State;
                    if (behavior == SetStateBehavior.ThrowException)
                    {
                        this.ThrowIfInvalidState(newState);
                    }
                    if (newState > this.state)
                    {
                        this.state = newState;
                        flag = true;
                        if (DiagnosticUtility.ShouldTraceInformation)
                        {
                            this.TraceEventHelper(TraceEventType.Information, TraceCode.PeerNeighborStateChanged, null, null, newState, previousOrAttemptedState);
                        }
                    }
                    else if (DiagnosticUtility.ShouldTraceInformation)
                    {
                        this.TraceEventHelper(TraceEventType.Information, TraceCode.PeerNeighborStateChangeFailed, null, null, previousOrAttemptedState, newState);
                    }
                }
                if (flag)
                {
                    this.OnStateChanged(newState);
                }
                return flag;
            }

            void IInputSessionShutdown.ChannelFaulted(IDuplexContextChannel channel)
            {
            }

            void IInputSessionShutdown.DoneReceiving(IDuplexContextChannel channel)
            {
                if (channel.State == CommunicationState.Opened)
                {
                    channel.Close();
                }
            }

            public void ThrowIfClosed()
            {
                if (this.state == PeerNeighborState.Closed)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ObjectDisposedException(this.ToString()));
                }
            }

            private void ThrowIfInvalidState(PeerNeighborState newState)
            {
                if (this.state == PeerNeighborState.Closed)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ObjectDisposedException(this.ToString()));
                }
                if (this.state >= newState)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(System.ServiceModel.SR.GetString("PeerNeighborInvalidState", new object[] { this.state.ToString(), newState.ToString() })));
                }
            }

            public void TraceClosedEvent(PeerNeighborState previousState)
            {
                if (DiagnosticUtility.ShouldTraceInformation)
                {
                    TraceEventType information = TraceEventType.Information;
                    switch (this.closeReason)
                    {
                        case PeerCloseReason.DuplicateNodeId:
                        case PeerCloseReason.InvalidNeighbor:
                            information = TraceEventType.Error;
                            break;

                        case PeerCloseReason.ConnectTimedOut:
                        case PeerCloseReason.Faulted:
                        case PeerCloseReason.InternalFailure:
                            information = TraceEventType.Warning;
                            break;
                    }
                    PeerNeighborCloseTraceRecord extendedData = new PeerNeighborCloseTraceRecord(this.nodeId, this.config.NodeId, null, null, this.GetHashCode(), this.initiator, PeerNeighborState.Closed.ToString(), previousState.ToString(), null, this.closeInitiator.ToString(), this.closeReason.ToString());
                    TraceUtility.TraceEvent(information, TraceCode.PeerNeighborStateChanged, extendedData, this, this.closeException);
                }
            }

            public void TraceEventHelper(TraceEventType severity, TraceCode traceCode)
            {
                PeerNeighborState nbrState = this.state;
                this.TraceEventHelper(severity, traceCode, null, null, nbrState, nbrState);
            }

            public void TraceEventHelper(TraceEventType severity, TraceCode traceCode, Exception e)
            {
                PeerNeighborState nbrState = this.state;
                this.TraceEventHelper(severity, traceCode, e, null, nbrState, nbrState);
            }

            public void TraceEventHelper(TraceEventType severity, TraceCode traceCode, Exception e, string action, PeerNeighborState nbrState, PeerNeighborState previousOrAttemptedState)
            {
                if (DiagnosticUtility.ShouldTrace(severity))
                {
                    string attemptedState = null;
                    string previousState = null;
                    PeerNodeAddress listenAddress = null;
                    IPAddress connectIPAddress = null;
                    if ((nbrState >= PeerNeighborState.Opened) && (nbrState <= PeerNeighborState.Connected))
                    {
                        listenAddress = this.ListenAddress;
                        connectIPAddress = this.ConnectIPAddress;
                    }
                    if (traceCode == TraceCode.PeerNeighborStateChangeFailed)
                    {
                        attemptedState = previousOrAttemptedState.ToString();
                    }
                    else if (traceCode == TraceCode.PeerNeighborStateChanged)
                    {
                        previousState = previousOrAttemptedState.ToString();
                    }
                    PeerNeighborTraceRecord extendedData = new PeerNeighborTraceRecord(this.nodeId, this.config.NodeId, listenAddress, connectIPAddress, this.GetHashCode(), this.initiator, nbrState.ToString(), previousState, attemptedState, action);
                    if ((severity == TraceEventType.Verbose) && (e != null))
                    {
                        severity = TraceEventType.Information;
                    }
                    TraceUtility.TraceEvent(severity, traceCode, extendedData, this, e);
                }
            }

            public bool TrySetState(PeerNeighborState newState)
            {
                if (!PeerNeighborStateHelper.IsSettable(newState))
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperInternal(false);
                }
                return this.SetState(newState, SetStateBehavior.TrySet);
            }

            public Exception CloseException =>
                this.closeException;

            public PeerCloseInitiator CloseInitiator =>
                this.closeInitiator;

            public PeerCloseReason CloseReason =>
                this.closeReason;

            public IPAddress ConnectIPAddress
            {
                get => 
                    this.connectIPAddress;
                set
                {
                    this.connectIPAddress = value;
                }
            }

            public IExtensionCollection<IPeerNeighbor> Extensions =>
                this.extensions;

            public bool IsClosing =>
                this.isClosing;

            public bool IsConnected =>
                PeerNeighborStateHelper.IsConnected(this.state);

            public bool IsInitiator =>
                this.initiator;

            public PeerNodeAddress ListenAddress
            {
                get
                {
                    PeerNodeAddress listenAddress = this.listenAddress;
                    if (listenAddress != null)
                    {
                        return new PeerNodeAddress(listenAddress.EndpointAddress, PeerIPHelper.CloneAddresses(listenAddress.IPAddresses, true));
                    }
                    return listenAddress;
                }
                set
                {
                    lock (this.ThisLock)
                    {
                        if (this.initiator)
                        {
                            throw DiagnosticUtility.ExceptionUtility.ThrowHelperInternal(false);
                        }
                        this.ThrowIfClosed();
                        if (value != null)
                        {
                            this.listenAddress = value;
                        }
                    }
                }
            }

            public ulong NodeId
            {
                get => 
                    this.nodeId;
                set
                {
                    lock (this.ThisLock)
                    {
                        this.ThrowIfClosed();
                        this.nodeId = value;
                    }
                }
            }

            public IPeerProxy Proxy
            {
                get => 
                    this.proxy;
                set
                {
                    this.proxy = value;
                    this.proxyChannel = (IClientChannel) this.proxy;
                    this.RegisterForChannelEvents();
                }
            }

            public PeerNeighborState State
            {
                get => 
                    this.state;
                set
                {
                    if (!PeerNeighborStateHelper.IsSettable(value))
                    {
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperInternal(false);
                    }
                    this.SetState(value, SetStateBehavior.ThrowException);
                }
            }

            private object ThisLock =>
                this.thisLock;

            public UtilityExtension Utility
            {
                get
                {
                    if (this.utility == null)
                    {
                        this.utility = this.Extensions.Find<UtilityExtension>();
                    }
                    return this.utility;
                }
            }

            private class OpenAsyncResult : AsyncResult
            {
                private Binding binding;
                private PeerNeighborManager.ClosedCallback closed;
                private bool completedSynchronously;
                private int currentIndex;
                private Exception lastException;
                private PeerNeighborManager.PeerNeighbor neighbor;
                private AsyncCallback onOpen;
                private PeerNodeAddress remoteAddress;
                private PeerService service;
                private TimeoutHelper timeoutHelper;

                public OpenAsyncResult(PeerNeighborManager.PeerNeighbor neighbor, PeerNodeAddress remoteAddress, Binding binding, PeerService service, PeerNeighborManager.ClosedCallback closedCallback, TimeSpan timeout, AsyncCallback callback, object state) : base(callback, state)
                {
                    this.timeoutHelper = new TimeoutHelper(timeout);
                    this.neighbor = neighbor;
                    this.currentIndex = 0;
                    this.completedSynchronously = true;
                    this.remoteAddress = remoteAddress;
                    this.service = service;
                    this.binding = binding;
                    this.onOpen = DiagnosticUtility.ThunkAsyncCallback(new AsyncCallback(this.OnOpen));
                    this.closed = closedCallback;
                    this.BeginOpen();
                }

                private void BeginOpen()
                {
                    bool flag = false;
                    try
                    {
                        while (this.currentIndex < this.remoteAddress.IPAddresses.Count)
                        {
                            EndpointAddress iPEndpointAddress = PeerIPHelper.GetIPEndpointAddress(this.remoteAddress.EndpointAddress, this.remoteAddress.IPAddresses[this.currentIndex]);
                            if (this.closed())
                            {
                                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ObjectDisposedException(base.GetType().ToString()));
                            }
                            try
                            {
                                this.neighbor.ConnectIPAddress = this.remoteAddress.IPAddresses[this.currentIndex];
                                IAsyncResult result = this.neighbor.BeginOpenProxy(iPEndpointAddress, this.binding, new InstanceContext(null, this.service, false), this.timeoutHelper.RemainingTime(), this.onOpen, null);
                                if (!result.CompletedSynchronously)
                                {
                                    return;
                                }
                                this.neighbor.EndOpenProxy(result);
                                this.lastException = null;
                                flag = true;
                                this.neighbor.isClosing = false;
                                goto Label_0167;
                            }
                            catch (Exception exception)
                            {
                                if (DiagnosticUtility.IsFatal(exception))
                                {
                                    throw;
                                }
                                try
                                {
                                    this.neighbor.CleanupProxy();
                                }
                                catch (Exception exception2)
                                {
                                    if (DiagnosticUtility.IsFatal(exception2))
                                    {
                                        throw;
                                    }
                                    DiagnosticUtility.ExceptionUtility.TraceHandledException(exception2, TraceEventType.Information);
                                }
                                DiagnosticUtility.ExceptionUtility.TraceHandledException(exception, TraceEventType.Information);
                                if (!this.ContinuableException(exception))
                                {
                                    throw;
                                }
                                continue;
                            }
                        }
                    }
                    catch (Exception exception3)
                    {
                        if (DiagnosticUtility.IsFatal(exception3))
                        {
                            throw;
                        }
                        DiagnosticUtility.ExceptionUtility.TraceHandledException(exception3, TraceEventType.Information);
                        this.lastException = exception3;
                    }
                Label_0167:
                    base.Complete(this.completedSynchronously, this.lastException);
                }

                private bool ContinuableException(Exception exception)
                {
                    if (((exception is EndpointNotFoundException) || (exception is TimeoutException)) && (this.timeoutHelper.RemainingTime() > TimeSpan.Zero))
                    {
                        this.lastException = exception;
                        this.currentIndex++;
                        return true;
                    }
                    return false;
                }

                public static void End(IAsyncResult result)
                {
                    AsyncResult.End<PeerNeighborManager.PeerNeighbor.OpenAsyncResult>(result);
                }

                private void OnOpen(IAsyncResult result)
                {
                    Exception exception = null;
                    bool flag = false;
                    if (!result.CompletedSynchronously)
                    {
                        this.completedSynchronously = false;
                        try
                        {
                            this.neighbor.EndOpenProxy(result);
                            flag = true;
                            this.neighbor.isClosing = false;
                        }
                        catch (Exception exception2)
                        {
                            if (DiagnosticUtility.IsFatal(exception2))
                            {
                                throw;
                            }
                            try
                            {
                                this.neighbor.CleanupProxy();
                            }
                            catch (Exception exception3)
                            {
                                if (DiagnosticUtility.IsFatal(exception3))
                                {
                                    throw;
                                }
                                DiagnosticUtility.ExceptionUtility.TraceHandledException(exception3, TraceEventType.Information);
                            }
                            exception = exception2;
                            if (this.ContinuableException(exception))
                            {
                                try
                                {
                                    this.BeginOpen();
                                    goto Label_0096;
                                }
                                catch (Exception exception4)
                                {
                                    if (DiagnosticUtility.IsFatal(exception4))
                                    {
                                        throw;
                                    }
                                    DiagnosticUtility.ExceptionUtility.TraceHandledException(exception4, TraceEventType.Information);
                                    goto Label_0096;
                                }
                            }
                            flag = true;
                        }
                    }
                Label_0096:
                    if (flag)
                    {
                        base.Complete(this.completedSynchronously, exception);
                    }
                }
            }

            private enum SetStateBehavior
            {
                ThrowException,
                TrySet
            }
        }

        private class PeerNeighborBehavior : IEndpointBehavior
        {
            private PeerNeighborManager.PeerNeighbor neighbor;

            public PeerNeighborBehavior(PeerNeighborManager.PeerNeighbor neighbor)
            {
                this.neighbor = neighbor;
            }

            public void AddBindingParameters(ServiceEndpoint serviceEndpoint, BindingParameterCollection bindingParameters)
            {
            }

            public void ApplyClientBehavior(ServiceEndpoint serviceEndpoint, ClientRuntime behavior)
            {
                behavior.DispatchRuntime.InputSessionShutdownHandlers.Add(this.neighbor);
            }

            public void ApplyDispatchBehavior(ServiceEndpoint serviceEndpoint, EndpointDispatcher endpointDispatcher)
            {
            }

            public void Validate(ServiceEndpoint serviceEndpoint)
            {
            }
        }

        private enum State
        {
            Created,
            Opened,
            ShuttingDown,
            Shutdown,
            Closed
        }
    }
}

