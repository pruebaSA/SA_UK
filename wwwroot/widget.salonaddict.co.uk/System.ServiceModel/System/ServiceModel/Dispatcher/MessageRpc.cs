namespace System.ServiceModel.Dispatcher
{
    using System;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Security;
    using System.ServiceModel;
    using System.ServiceModel.Activation;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Diagnostics;
    using System.Threading;
    using System.Xml;

    [StructLayout(LayoutKind.Sequential)]
    internal struct MessageRpc
    {
        internal ServiceModelActivity Activity;
        internal ManualResetEvent AsyncOperationWaitEvent;
        internal Guid ResponseActivityId;
        internal IAsyncResult AsyncResult;
        internal readonly ServiceChannel Channel;
        internal readonly ChannelHandler channelHandler;
        internal RefBool CompletedSynchronously;
        internal readonly object[] Correlation;
        internal object[] InputParameters;
        internal object[] OutputParameters;
        internal object ReturnParameter;
        internal bool ParametersDisposed;
        internal bool DidDeserializeRequestBody;
        internal System.ServiceModel.Channels.TransactionMessageProperty TransactionMessageProperty;
        internal System.ServiceModel.Dispatcher.TransactedBatchContext TransactedBatchContext;
        internal Exception Error;
        internal MessageRpcProcessor ErrorProcessor;
        internal ErrorHandlerFaultInfo FaultInfo;
        internal bool HasSecurityContext;
        internal readonly ServiceHostBase Host;
        internal object Instance;
        internal bool MessageRpcOwnsInstanceContextThrottle;
        internal MessageRpcProcessor NextProcessor;
        internal Collection<MessageHeaderInfo> NotUnderstoodHeaders;
        internal DispatchOperationRuntime Operation;
        internal readonly System.ServiceModel.OperationContext OperationContext;
        private bool paused;
        internal Message Request;
        internal System.ServiceModel.Channels.RequestContext RequestContext;
        internal bool RequestContextThrewOnReply;
        internal UniqueId RequestID;
        internal Message Reply;
        internal System.ServiceModel.Channels.RequestReplyCorrelator.ReplyToInfo ReplyToInfo;
        internal MessageVersion RequestVersion;
        internal ServiceSecurityContext SecurityContext;
        internal System.ServiceModel.InstanceContext InstanceContext;
        internal bool SuccessfullyBoundInstance;
        internal bool SuccessfullyIncrementedActivity;
        internal bool SuccessfullyLockedInstance;
        private bool switchedThreads;
        private bool isInstanceContextSingleton;
        internal TransactionRpcFacet transaction;
        internal HostingMessageProperty HostingProperty;
        internal MessageRpc(System.ServiceModel.Channels.RequestContext requestContext, Message request, DispatchOperationRuntime operation, ServiceChannel channel, ServiceHostBase host, ChannelHandler channelHandler, bool cleanThread, System.ServiceModel.OperationContext operationContext, System.ServiceModel.InstanceContext instanceContext)
        {
            object obj2;
            this.AsyncOperationWaitEvent = null;
            this.Activity = null;
            this.AsyncResult = null;
            this.Channel = channel;
            this.channelHandler = channelHandler;
            this.CompletedSynchronously = null;
            this.Correlation = EmptyArray.Allocate(operation.Parent.CorrelationCount);
            this.DidDeserializeRequestBody = false;
            this.TransactionMessageProperty = null;
            this.TransactedBatchContext = null;
            this.Error = null;
            this.ErrorProcessor = null;
            this.FaultInfo = new ErrorHandlerFaultInfo(request.Version.Addressing.DefaultFaultAction);
            this.HasSecurityContext = false;
            this.Host = host;
            this.Instance = null;
            this.MessageRpcOwnsInstanceContextThrottle = false;
            this.NextProcessor = null;
            this.NotUnderstoodHeaders = null;
            this.Operation = operation;
            this.OperationContext = operationContext;
            this.paused = false;
            this.ParametersDisposed = false;
            this.Request = request;
            this.RequestContext = requestContext;
            this.RequestContextThrewOnReply = false;
            this.RequestVersion = request.Version;
            this.Reply = null;
            this.SecurityContext = null;
            this.InstanceContext = instanceContext;
            this.SuccessfullyBoundInstance = false;
            this.SuccessfullyIncrementedActivity = false;
            this.SuccessfullyLockedInstance = false;
            this.switchedThreads = !cleanThread;
            this.transaction = null;
            this.InputParameters = null;
            this.OutputParameters = null;
            this.ReturnParameter = null;
            this.isInstanceContextSingleton = InstanceContextProviderBase.IsProviderSingleton(this.Channel.DispatchRuntime.InstanceContextProvider);
            if (!operation.IsOneWay && !operation.Parent.ManualAddressing)
            {
                this.RequestID = request.Headers.MessageId;
                this.ReplyToInfo = new System.ServiceModel.Channels.RequestReplyCorrelator.ReplyToInfo(request);
            }
            else
            {
                this.RequestID = null;
                this.ReplyToInfo = new System.ServiceModel.Channels.RequestReplyCorrelator.ReplyToInfo();
            }
            this.HostingProperty = null;
            if (ServiceHostingEnvironment.IsHosted && request.Properties.TryGetValue(HostingMessageProperty.Name, out obj2))
            {
                this.HostingProperty = (HostingMessageProperty) obj2;
                request.Properties.Remove(HostingMessageProperty.Name);
            }
            if (DiagnosticUtility.ShouldUseActivity)
            {
                this.Activity = TraceUtility.ExtractActivity(this.Request);
            }
            if (DiagnosticUtility.ShouldUseActivity || TraceUtility.ShouldPropagateActivity)
            {
                this.ResponseActivityId = ActivityIdHeader.ExtractActivityId(this.Request);
            }
            else
            {
                this.ResponseActivityId = Guid.Empty;
            }
        }

        internal bool IsPaused =>
            this.paused;
        internal bool SwitchedThreads =>
            this.switchedThreads;
        internal bool IsInstanceContextSingleton
        {
            set
            {
                this.isInstanceContextSingleton = value;
            }
        }
        internal TransactionRpcFacet Transaction
        {
            get
            {
                if (this.transaction == null)
                {
                    this.transaction = new TransactionRpcFacet(ref this);
                }
                return this.transaction;
            }
        }
        internal void Abort()
        {
            this.AbortRequestContext();
            this.AbortChannel();
            this.AbortInstanceContext();
        }

        private void AbortRequestContext(System.ServiceModel.Channels.RequestContext requestContext)
        {
            try
            {
                requestContext.Abort();
            }
            catch (Exception exception)
            {
                if (DiagnosticUtility.IsFatal(exception))
                {
                    throw;
                }
                this.channelHandler.HandleError(exception);
            }
        }

        internal void AbortRequestContext()
        {
            if (this.OperationContext.RequestContext != null)
            {
                this.AbortRequestContext(this.OperationContext.RequestContext);
            }
            if ((this.RequestContext != null) && (this.RequestContext != this.OperationContext.RequestContext))
            {
                this.AbortRequestContext(this.RequestContext);
            }
        }

        internal void CloseRequestContext()
        {
            if (this.OperationContext.RequestContext != null)
            {
                this.DisposeRequestContext(this.OperationContext.RequestContext);
            }
            if ((this.RequestContext != null) && (this.RequestContext != this.OperationContext.RequestContext))
            {
                this.DisposeRequestContext(this.RequestContext);
            }
        }

        private void DisposeRequestContext(System.ServiceModel.Channels.RequestContext context)
        {
            try
            {
                context.Close();
            }
            catch (Exception exception)
            {
                if (DiagnosticUtility.IsFatal(exception))
                {
                    throw;
                }
                this.AbortRequestContext(context);
                this.channelHandler.HandleError(exception);
            }
        }

        internal void AbortChannel()
        {
            if ((this.Channel != null) && this.Channel.HasSession)
            {
                try
                {
                    this.Channel.Abort();
                }
                catch (Exception exception)
                {
                    if (DiagnosticUtility.IsFatal(exception))
                    {
                        throw;
                    }
                    this.channelHandler.HandleError(exception);
                }
            }
        }

        internal void CloseChannel()
        {
            if ((this.Channel != null) && this.Channel.HasSession)
            {
                try
                {
                    this.Channel.Close(ChannelHandler.CloseAfterFaultTimeout);
                }
                catch (Exception exception)
                {
                    if (DiagnosticUtility.IsFatal(exception))
                    {
                        throw;
                    }
                    this.channelHandler.HandleError(exception);
                }
            }
        }

        internal void AbortInstanceContext()
        {
            if ((this.InstanceContext != null) && !this.isInstanceContextSingleton)
            {
                try
                {
                    this.InstanceContext.Abort();
                }
                catch (Exception exception)
                {
                    if (DiagnosticUtility.IsFatal(exception))
                    {
                        throw;
                    }
                    this.channelHandler.HandleError(exception);
                }
            }
        }

        internal void EnsureReceive()
        {
            using (ServiceModelActivity.BoundOperation(this.Activity))
            {
                ChannelHandler.Register(this.channelHandler);
            }
        }

        private bool ProcessError(Exception e)
        {
            MessageRpcProcessor errorProcessor = this.ErrorProcessor;
            try
            {
                if (e.GetType().IsAssignableFrom(typeof(FaultException)))
                {
                    if (DiagnosticUtility.ShouldTraceInformation)
                    {
                        DiagnosticUtility.ExceptionUtility.TraceHandledException(e, TraceEventType.Information);
                    }
                }
                else if (DiagnosticUtility.ShouldTraceError)
                {
                    DiagnosticUtility.ExceptionUtility.TraceHandledException(e, TraceEventType.Error);
                }
                this.Error = e;
                if (this.ErrorProcessor != null)
                {
                    this.ErrorProcessor(ref this);
                }
                return (this.Error == null);
            }
            catch (Exception exception)
            {
                if (DiagnosticUtility.IsFatal(exception))
                {
                    throw;
                }
                return ((errorProcessor != this.ErrorProcessor) && this.ProcessError(exception));
            }
        }

        internal void DisposeParameters()
        {
            if (this.Operation.DisposeParameters)
            {
                this.DisposeParametersCore();
            }
        }

        internal void DisposeParametersCore()
        {
            if (!this.ParametersDisposed)
            {
                this.DisposeParameterList(this.InputParameters);
                this.DisposeParameterList(this.OutputParameters);
                IDisposable returnParameter = this.ReturnParameter as IDisposable;
                if (returnParameter != null)
                {
                    try
                    {
                        returnParameter.Dispose();
                    }
                    catch (Exception exception)
                    {
                        if (DiagnosticUtility.IsFatal(exception))
                        {
                            throw;
                        }
                        this.channelHandler.HandleError(exception);
                    }
                }
                this.ParametersDisposed = true;
            }
        }

        private void DisposeParameterList(object[] parameters)
        {
            IDisposable disposable = null;
            if (parameters != null)
            {
                foreach (object obj2 in parameters)
                {
                    disposable = obj2 as IDisposable;
                    if (disposable != null)
                    {
                        try
                        {
                            disposable.Dispose();
                        }
                        catch (Exception exception)
                        {
                            if (DiagnosticUtility.IsFatal(exception))
                            {
                                throw;
                            }
                            this.channelHandler.HandleError(exception);
                        }
                    }
                }
            }
        }

        internal IResumeMessageRpc Pause()
        {
            Wrapper wrapper = new Wrapper(ref this);
            this.paused = true;
            return wrapper;
        }

        [SecurityCritical]
        private IDisposable ApplyHostingIntegrationContext()
        {
            if (this.HostingProperty != null)
            {
                return this.ApplyHostingIntegrationContextNoInline();
            }
            return null;
        }

        [MethodImpl(MethodImplOptions.NoInlining), SecurityCritical]
        private IDisposable ApplyHostingIntegrationContextNoInline() => 
            this.HostingProperty.ApplyIntegrationContext();

        [SecurityCritical, SecurityTreatAsSafe]
        internal bool Process(bool isOperationContextSet)
        {
            using (ServiceModelActivity.BoundOperation(this.Activity))
            {
                if (this.NextProcessor != null)
                {
                    System.ServiceModel.OperationContext current;
                    MessageRpcProcessor nextProcessor = this.NextProcessor;
                    this.NextProcessor = null;
                    if (!isOperationContextSet)
                    {
                        current = System.ServiceModel.OperationContext.Current;
                    }
                    else
                    {
                        current = null;
                    }
                    ServiceHostingEnvironment.IncrementBusyCount();
                    IDisposable disposable = this.ApplyHostingIntegrationContext();
                    try
                    {
                        if (!isOperationContextSet)
                        {
                            System.ServiceModel.OperationContext.Current = this.OperationContext;
                        }
                        nextProcessor(ref this);
                        bool flag = !this.paused;
                        if (flag)
                        {
                            this.OperationContext.SetClientReply(null, false);
                        }
                        return flag;
                    }
                    catch (Exception exception)
                    {
                        if (DiagnosticUtility.IsFatal(exception))
                        {
                            throw;
                        }
                        if (!this.ProcessError(exception))
                        {
                            this.Abort();
                        }
                    }
                    finally
                    {
                        try
                        {
                            ServiceHostingEnvironment.DecrementBusyCount();
                            if (disposable != null)
                            {
                                disposable.Dispose();
                            }
                            if (!isOperationContextSet)
                            {
                                System.ServiceModel.OperationContext.Current = current;
                            }
                            if (!this.paused)
                            {
                                this.channelHandler.DispatchDone();
                                this.OperationContext.ClearClientReplyNoThrow();
                            }
                        }
                        catch (Exception exception2)
                        {
                            if (DiagnosticUtility.IsFatal(exception2))
                            {
                                throw;
                            }
                            throw DiagnosticUtility.ExceptionUtility.ThrowHelperFatal(exception2.Message, exception2);
                        }
                    }
                }
                return true;
            }
        }

        internal void UnPause()
        {
            this.paused = false;
        }
        private class Wrapper : IResumeMessageRpc
        {
            private bool alreadyResumed;
            private MessageRpc rpc;

            internal Wrapper(ref MessageRpc rpc)
            {
                this.rpc = rpc;
                MessageRpcProcessor nextProcessor = rpc.NextProcessor;
                ServiceHostingEnvironment.IncrementBusyCount();
            }

            public void Resume(out bool alreadyResumedNoLock)
            {
                try
                {
                    alreadyResumedNoLock = this.alreadyResumed;
                    this.alreadyResumed = true;
                    this.rpc.switchedThreads = true;
                    if (this.rpc.Process(false))
                    {
                        this.rpc.EnsureReceive();
                    }
                }
                finally
                {
                    ServiceHostingEnvironment.DecrementBusyCount();
                }
            }

            public void Resume(IAsyncResult result)
            {
                this.rpc.AsyncResult = result;
                using (ServiceModelActivity.BoundOperation(this.rpc.Activity, true))
                {
                    bool flag;
                    this.Resume(out flag);
                    if (flag)
                    {
                        Exception exception = new InvalidOperationException(System.ServiceModel.SR.GetString("SFxMultipleCallbackFromAsyncOperation", new object[] { this.rpc.Operation.Name }));
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(exception);
                    }
                }
            }

            public void SetCompletedSynchronously()
            {
                this.rpc.CompletedSynchronously.Value = true;
            }

            public ManualResetEvent AsyncOperationWaitEvent =>
                this.rpc.AsyncOperationWaitEvent;
        }
    }
}

