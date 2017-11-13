namespace System.ServiceModel.Dispatcher
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Diagnostics;
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Diagnostics;
    using System.Threading;

    internal class ImmutableDispatchRuntime
    {
        private readonly AuthorizationBehavior authorizationBehavior;
        private readonly ConcurrencyBehavior concurrency;
        private readonly int correlationCount;
        private readonly IDemuxer demuxer;
        private bool didTraceProcessMessage2;
        private bool didTraceProcessMessage3;
        private bool didTraceProcessMessage4;
        private readonly bool enableFaults;
        private readonly System.ServiceModel.Dispatcher.ErrorBehavior error;
        private readonly bool ignoreTransactionFlow;
        private readonly IInputSessionShutdown[] inputSessionShutdownHandlers;
        private readonly System.ServiceModel.Dispatcher.InstanceBehavior instance;
        private readonly bool isOnServer;
        private readonly bool manualAddressing;
        private readonly IDispatchMessageInspector[] messageInspectors;
        private readonly int parameterInspectorCorrelationOffset;
        private readonly MessageRpcProcessor processMessage1;
        private readonly MessageRpcProcessor processMessage2;
        private readonly MessageRpcProcessor processMessage3;
        private readonly MessageRpcProcessor processMessage4;
        private readonly MessageRpcProcessor processMessage5;
        [ThreadStatic]
        internal static ManualResetEvent processMessage5IsOnTheStack;
        private readonly MessageRpcProcessor processMessage6;
        private readonly MessageRpcProcessor processMessage7;
        private readonly MessageRpcProcessor processMessageCleanup;
        private readonly MessageRpcProcessor processMessageCleanupError;
        private readonly IRequestReplyCorrelator requestReplyCorrelator;
        private readonly SecurityImpersonationBehavior securityImpersonation;
        private readonly TerminatingOperationBehavior terminate;
        private readonly ThreadBehavior thread;
        private readonly TransactionBehavior transaction;
        private readonly bool validateMustUnderstand;

        internal ImmutableDispatchRuntime(DispatchRuntime dispatch)
        {
            this.authorizationBehavior = AuthorizationBehavior.TryCreate(dispatch);
            this.concurrency = new ConcurrencyBehavior(dispatch);
            this.error = new System.ServiceModel.Dispatcher.ErrorBehavior(dispatch.ChannelDispatcher);
            this.enableFaults = dispatch.EnableFaults;
            this.inputSessionShutdownHandlers = EmptyArray<IInputSessionShutdown>.ToArray(dispatch.InputSessionShutdownHandlers);
            this.instance = new System.ServiceModel.Dispatcher.InstanceBehavior(dispatch, this);
            this.isOnServer = dispatch.IsOnServer;
            this.manualAddressing = dispatch.ManualAddressing;
            this.messageInspectors = EmptyArray<IDispatchMessageInspector>.ToArray(dispatch.MessageInspectors);
            this.requestReplyCorrelator = new System.ServiceModel.Channels.RequestReplyCorrelator();
            this.securityImpersonation = SecurityImpersonationBehavior.CreateIfNecessary(dispatch);
            this.terminate = TerminatingOperationBehavior.CreateIfNecessary(dispatch);
            this.terminate = new TerminatingOperationBehavior();
            this.thread = new ThreadBehavior(dispatch);
            this.validateMustUnderstand = dispatch.ValidateMustUnderstand;
            this.ignoreTransactionFlow = dispatch.IgnoreTransactionMessageProperty;
            this.transaction = TransactionBehavior.CreateIfNeeded(dispatch);
            this.parameterInspectorCorrelationOffset = dispatch.MessageInspectors.Count + dispatch.MaxCallContextInitializers;
            this.correlationCount = this.parameterInspectorCorrelationOffset + dispatch.MaxParameterInspectors;
            DispatchOperationRuntime runtime = new DispatchOperationRuntime(dispatch.UnhandledDispatchOperation, this);
            if (dispatch.OperationSelector == null)
            {
                ActionDemuxer demuxer = new ActionDemuxer();
                for (int i = 0; i < dispatch.Operations.Count; i++)
                {
                    DispatchOperation operation = dispatch.Operations[i];
                    DispatchOperationRuntime runtime2 = new DispatchOperationRuntime(operation, this);
                    demuxer.Add(operation.Action, runtime2);
                }
                demuxer.SetUnhandled(runtime);
                this.demuxer = demuxer;
            }
            else
            {
                CustomDemuxer demuxer2 = new CustomDemuxer(dispatch.OperationSelector);
                for (int j = 0; j < dispatch.Operations.Count; j++)
                {
                    DispatchOperation operation2 = dispatch.Operations[j];
                    DispatchOperationRuntime runtime3 = new DispatchOperationRuntime(operation2, this);
                    demuxer2.Add(operation2.Name, runtime3);
                }
                demuxer2.SetUnhandled(runtime);
                this.demuxer = demuxer2;
            }
            this.processMessage1 = new MessageRpcProcessor(this.ProcessMessage1);
            this.processMessage2 = new MessageRpcProcessor(this.ProcessMessage2);
            this.processMessage3 = new MessageRpcProcessor(this.ProcessMessage3);
            this.processMessage4 = new MessageRpcProcessor(this.ProcessMessage4);
            this.processMessage5 = new MessageRpcProcessor(this.ProcessMessage5);
            this.processMessage6 = new MessageRpcProcessor(this.ProcessMessage6);
            this.processMessage7 = new MessageRpcProcessor(this.ProcessMessage7);
            this.processMessageCleanup = new MessageRpcProcessor(this.ProcessMessageCleanup);
            this.processMessageCleanupError = new MessageRpcProcessor(this.ProcessMessageCleanupError);
        }

        private bool AcquireDynamicInstanceContext(ref MessageRpc rpc)
        {
            if (rpc.InstanceContext.QuotaThrottle != null)
            {
                return this.AcquireDynamicInstanceContextCore(ref rpc);
            }
            return true;
        }

        private bool AcquireDynamicInstanceContextCore(ref MessageRpc rpc)
        {
            bool flag = rpc.InstanceContext.QuotaThrottle.Acquire(rpc.Pause());
            if (flag)
            {
                rpc.UnPause();
            }
            return flag;
        }

        private void AddMessageProperties(Message message, OperationContext context, ServiceChannel replyChannel)
        {
            if (context.InternalServiceChannel == replyChannel)
            {
                if (context.HasOutgoingMessageHeaders)
                {
                    message.Headers.CopyHeadersFrom(context.OutgoingMessageHeaders);
                }
                if (context.HasOutgoingMessageProperties)
                {
                    message.Properties.CopyProperties(context.OutgoingMessageProperties);
                }
            }
        }

        internal void AfterReceiveRequest(ref MessageRpc rpc)
        {
            if (this.messageInspectors.Length > 0)
            {
                this.AfterReceiveRequestCore(ref rpc);
            }
        }

        internal void AfterReceiveRequestCore(ref MessageRpc rpc)
        {
            int messageInspectorCorrelationOffset = this.MessageInspectorCorrelationOffset;
            try
            {
                for (int i = 0; i < this.messageInspectors.Length; i++)
                {
                    rpc.Correlation[messageInspectorCorrelationOffset + i] = this.messageInspectors[i].AfterReceiveRequest(ref rpc.Request, (IClientChannel) rpc.Channel.Proxy, rpc.InstanceContext);
                }
            }
            catch (Exception exception)
            {
                if (DiagnosticUtility.IsFatal(exception))
                {
                    throw;
                }
                if (System.ServiceModel.Dispatcher.ErrorBehavior.ShouldRethrowExceptionAsIs(exception))
                {
                    throw;
                }
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperCallback(exception);
            }
        }

        private void BeforeSendReply(ref MessageRpc rpc, ref Exception exception, ref bool thereIsAnUnhandledException)
        {
            if (this.messageInspectors.Length > 0)
            {
                this.BeforeSendReplyCore(ref rpc, ref exception, ref thereIsAnUnhandledException);
            }
        }

        internal void BeforeSendReplyCore(ref MessageRpc rpc, ref Exception exception, ref bool thereIsAnUnhandledException)
        {
            int messageInspectorCorrelationOffset = this.MessageInspectorCorrelationOffset;
            for (int i = 0; i < this.messageInspectors.Length; i++)
            {
                try
                {
                    Message reply = rpc.Reply;
                    Message message2 = reply;
                    this.messageInspectors[i].BeforeSendReply(ref message2, rpc.Correlation[messageInspectorCorrelationOffset + i]);
                    if ((message2 == null) && (reply != null))
                    {
                        object[] args = new object[] { this.messageInspectors[i].GetType().ToString(), rpc.Operation.Name ?? "" };
                        System.ServiceModel.Dispatcher.ErrorBehavior.ThrowAndCatch(new InvalidOperationException(System.ServiceModel.SR.GetString("SFxNullReplyFromExtension2", args)));
                    }
                    rpc.Reply = message2;
                }
                catch (Exception exception2)
                {
                    if (DiagnosticUtility.IsFatal(exception2))
                    {
                        throw;
                    }
                    if (!System.ServiceModel.Dispatcher.ErrorBehavior.ShouldRethrowExceptionAsIs(exception2))
                    {
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperCallback(exception2);
                    }
                    if (exception == null)
                    {
                        exception = exception2;
                    }
                    thereIsAnUnhandledException = !this.error.HandleError(exception2) ? true : thereIsAnUnhandledException;
                }
            }
        }

        internal bool Dispatch(ref MessageRpc rpc, bool isOperationContextSet)
        {
            rpc.ErrorProcessor = this.processMessageCleanup;
            rpc.NextProcessor = this.processMessage1;
            return rpc.Process(isOperationContextSet);
        }

        internal DispatchOperationRuntime GetOperation(ref Message message) => 
            this.demuxer.GetOperation(ref message);

        internal static void GotDynamicInstanceContext(object state)
        {
            bool flag;
            ((IResumeMessageRpc) state).Resume(out flag);
        }

        internal void InputSessionDoneReceiving(ServiceChannel channel)
        {
            if (this.inputSessionShutdownHandlers.Length > 0)
            {
                this.InputSessionDoneReceivingCore(channel);
            }
        }

        private void InputSessionDoneReceivingCore(ServiceChannel channel)
        {
            IDuplexContextChannel proxy = channel.Proxy as IDuplexContextChannel;
            if (proxy != null)
            {
                IInputSessionShutdown[] inputSessionShutdownHandlers = this.inputSessionShutdownHandlers;
                try
                {
                    for (int i = 0; i < inputSessionShutdownHandlers.Length; i++)
                    {
                        inputSessionShutdownHandlers[i].DoneReceiving(proxy);
                    }
                }
                catch (Exception exception)
                {
                    if (DiagnosticUtility.IsFatal(exception))
                    {
                        throw;
                    }
                    if (!this.error.HandleError(exception))
                    {
                        proxy.Abort();
                    }
                }
            }
        }

        internal void InputSessionFaulted(ServiceChannel channel)
        {
            if (this.inputSessionShutdownHandlers.Length > 0)
            {
                this.InputSessionFaultedCore(channel);
            }
        }

        private void InputSessionFaultedCore(ServiceChannel channel)
        {
            IDuplexContextChannel proxy = channel.Proxy as IDuplexContextChannel;
            if (proxy != null)
            {
                IInputSessionShutdown[] inputSessionShutdownHandlers = this.inputSessionShutdownHandlers;
                try
                {
                    for (int i = 0; i < inputSessionShutdownHandlers.Length; i++)
                    {
                        inputSessionShutdownHandlers[i].ChannelFaulted(proxy);
                    }
                }
                catch (Exception exception)
                {
                    if (DiagnosticUtility.IsFatal(exception))
                    {
                        throw;
                    }
                    if (!this.error.HandleError(exception))
                    {
                        proxy.Abort();
                    }
                }
            }
        }

        private bool PrepareAndAddressReply(ref MessageRpc rpc)
        {
            bool flag = true;
            if (!this.manualAddressing)
            {
                if (!object.ReferenceEquals(rpc.RequestID, null))
                {
                    System.ServiceModel.Channels.RequestReplyCorrelator.PrepareReply(rpc.Reply, rpc.RequestID);
                }
                if (!rpc.Channel.HasSession)
                {
                    flag = System.ServiceModel.Channels.RequestReplyCorrelator.AddressReply(rpc.Reply, rpc.ReplyToInfo);
                }
            }
            this.AddMessageProperties(rpc.Reply, rpc.OperationContext, rpc.Channel);
            return flag;
        }

        internal void ProcessMessage1(ref MessageRpc rpc)
        {
            rpc.NextProcessor = this.processMessage2;
            if (rpc.Operation.IsOneWay)
            {
                rpc.RequestContext.Reply(null);
                rpc.OperationContext.RequestContext = null;
            }
            else
            {
                if ((!rpc.Channel.IsReplyChannel && (rpc.RequestID == null)) && (rpc.Operation.Action != "*"))
                {
                    CommunicationException exception = new CommunicationException(System.ServiceModel.SR.GetString("SFxOneWayMessageToTwoWayMethod0"));
                    throw TraceUtility.ThrowHelperError(exception, rpc.Request);
                }
                if (!this.manualAddressing)
                {
                    EndpointAddress replyTo = rpc.ReplyToInfo.ReplyTo;
                    if (((replyTo != null) && replyTo.IsNone) && rpc.Channel.IsReplyChannel)
                    {
                        CommunicationException exception2 = new CommunicationException(System.ServiceModel.SR.GetString("SFxRequestReplyNone"));
                        throw TraceUtility.ThrowHelperError(exception2, rpc.Request);
                    }
                    if (this.isOnServer)
                    {
                        EndpointAddress remoteAddress = rpc.Channel.RemoteAddress;
                        if ((remoteAddress != null) && !remoteAddress.IsAnonymous)
                        {
                            MessageHeaders headers = rpc.Request.Headers;
                            Uri uri = remoteAddress.Uri;
                            if (((replyTo != null) && !replyTo.IsAnonymous) && (uri != replyTo.Uri))
                            {
                                Exception exception3 = new InvalidOperationException(System.ServiceModel.SR.GetString("SFxRequestHasInvalidReplyToOnServer", new object[] { replyTo.Uri, uri }));
                                throw TraceUtility.ThrowHelperError(exception3, rpc.Request);
                            }
                            EndpointAddress faultTo = headers.FaultTo;
                            if (((faultTo != null) && !faultTo.IsAnonymous) && (uri != faultTo.Uri))
                            {
                                Exception exception4 = new InvalidOperationException(System.ServiceModel.SR.GetString("SFxRequestHasInvalidFaultToOnServer", new object[] { faultTo.Uri, uri }));
                                throw TraceUtility.ThrowHelperError(exception4, rpc.Request);
                            }
                            if (rpc.RequestVersion.Addressing == AddressingVersion.WSAddressingAugust2004)
                            {
                                EndpointAddress from = headers.From;
                                if (((from != null) && !from.IsAnonymous) && (uri != from.Uri))
                                {
                                    Exception exception5 = new InvalidOperationException(System.ServiceModel.SR.GetString("SFxRequestHasInvalidFromOnServer", new object[] { from.Uri, uri }));
                                    throw TraceUtility.ThrowHelperError(exception5, rpc.Request);
                                }
                            }
                        }
                    }
                }
            }
            if (this.concurrency.IsConcurrent(ref rpc))
            {
                rpc.Channel.IncrementActivity();
                rpc.SuccessfullyIncrementedActivity = true;
            }
            if (this.authorizationBehavior != null)
            {
                this.authorizationBehavior.Authorize(ref rpc);
            }
            this.instance.EnsureInstanceContext(ref rpc);
            this.TransferChannelFromPendingList(ref rpc);
            this.AcquireDynamicInstanceContext(ref rpc);
            if (!rpc.IsPaused)
            {
                this.ProcessMessage2(ref rpc);
            }
        }

        private void ProcessMessage2(ref MessageRpc rpc)
        {
            rpc.NextProcessor = this.processMessage3;
            this.AfterReceiveRequest(ref rpc);
            if (!this.ignoreTransactionFlow)
            {
                rpc.TransactionMessageProperty = TransactionMessageProperty.TryGet(rpc.Request);
            }
            this.concurrency.LockInstance(ref rpc);
            if (!rpc.IsPaused)
            {
                this.ProcessMessage3(ref rpc);
            }
            else if ((this.isOnServer && DiagnosticUtility.ShouldTraceInformation) && !this.didTraceProcessMessage2)
            {
                this.didTraceProcessMessage2 = true;
                DiagnosticUtility.DiagnosticTrace.TraceEvent(TraceEventType.Information, TraceCode.MessageProcessingPaused, System.ServiceModel.SR.GetString("TraceCodeProcessMessage2Paused", new object[] { rpc.Channel.DispatchRuntime.EndpointDispatcher.ContractName, rpc.Channel.DispatchRuntime.EndpointDispatcher.EndpointAddress }));
            }
        }

        private void ProcessMessage3(ref MessageRpc rpc)
        {
            rpc.NextProcessor = this.processMessage4;
            rpc.SuccessfullyLockedInstance = true;
            if (this.transaction != null)
            {
                this.transaction.ResolveTransaction(ref rpc);
                if (rpc.Operation.TransactionRequired)
                {
                    this.transaction.SetCurrent(ref rpc);
                }
            }
            if (!rpc.IsPaused)
            {
                this.ProcessMessage4(ref rpc);
            }
            else if ((this.isOnServer && DiagnosticUtility.ShouldTraceInformation) && !this.didTraceProcessMessage3)
            {
                this.didTraceProcessMessage3 = true;
                DiagnosticUtility.DiagnosticTrace.TraceEvent(TraceEventType.Information, TraceCode.MessageProcessingPaused, System.ServiceModel.SR.GetString("TraceCodeProcessMessage3Paused", new object[] { rpc.Channel.DispatchRuntime.EndpointDispatcher.ContractName, rpc.Channel.DispatchRuntime.EndpointDispatcher.EndpointAddress }));
            }
        }

        private void ProcessMessage4(ref MessageRpc rpc)
        {
            rpc.NextProcessor = this.processMessage5;
            try
            {
                this.thread.BindThread(ref rpc);
            }
            catch (Exception exception)
            {
                if (DiagnosticUtility.IsFatal(exception))
                {
                    throw;
                }
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperFatal(exception.Message, exception);
            }
            if (!rpc.IsPaused)
            {
                this.ProcessMessage5(ref rpc);
            }
            else if ((this.isOnServer && DiagnosticUtility.ShouldTraceInformation) && !this.didTraceProcessMessage4)
            {
                this.didTraceProcessMessage4 = true;
                DiagnosticUtility.DiagnosticTrace.TraceEvent(TraceEventType.Information, TraceCode.MessageProcessingPaused, System.ServiceModel.SR.GetString("TraceCodeProcessMessage4Paused", new object[] { rpc.Channel.DispatchRuntime.EndpointDispatcher.ContractName, rpc.Channel.DispatchRuntime.EndpointDispatcher.EndpointAddress }));
            }
        }

        private void ProcessMessage5(ref MessageRpc rpc)
        {
            rpc.NextProcessor = this.processMessage6;
            if (this.concurrency.IsConcurrent(ref rpc))
            {
                rpc.EnsureReceive();
            }
            this.instance.ReleaseBefore(ref rpc);
            rpc.Instance = rpc.InstanceContext.GetServiceInstance(rpc.Request);
            try
            {
                try
                {
                    if (!rpc.Operation.IsSynchronous)
                    {
                        rpc.AsyncOperationWaitEvent = new ManualResetEvent(false);
                        processMessage5IsOnTheStack = rpc.AsyncOperationWaitEvent;
                        rpc.CompletedSynchronously = new RefBool();
                    }
                    if (this.transaction != null)
                    {
                        this.transaction.InitializeCallContext(ref rpc);
                    }
                    rpc.Operation.InvokeBegin(ref rpc);
                }
                catch
                {
                    if (this.transaction != null)
                    {
                        try
                        {
                            this.transaction.ClearCallContext(ref rpc);
                        }
                        catch (Exception exception)
                        {
                            if (DiagnosticUtility.IsFatal(exception))
                            {
                                throw;
                            }
                            this.error.HandleError(exception);
                        }
                    }
                    throw;
                }
                try
                {
                    if (this.transaction != null)
                    {
                        this.transaction.ClearCallContext(ref rpc);
                    }
                }
                catch (Exception exception2)
                {
                    if (DiagnosticUtility.IsFatal(exception2))
                    {
                        throw;
                    }
                    if (rpc.Operation.IsSynchronous || !rpc.IsPaused)
                    {
                        throw;
                    }
                    this.error.HandleError(exception2);
                }
                if (!rpc.IsPaused)
                {
                    if (rpc.Operation.IsSynchronous)
                    {
                        this.ProcessMessageCleanup(ref rpc);
                    }
                    else
                    {
                        rpc.AsyncOperationWaitEvent.Set();
                        this.ProcessMessage6(ref rpc);
                    }
                }
            }
            finally
            {
                if ((rpc.AsyncOperationWaitEvent != null) && rpc.IsPaused)
                {
                    rpc.AsyncOperationWaitEvent.Set();
                }
                processMessage5IsOnTheStack = null;
            }
        }

        private void ProcessMessage6(ref MessageRpc rpc)
        {
            rpc.NextProcessor = this.processMessage7;
            if (rpc.AsyncOperationWaitEvent != null)
            {
                rpc.AsyncOperationWaitEvent.WaitOne();
                rpc.AsyncOperationWaitEvent.Close();
                rpc.AsyncOperationWaitEvent = null;
            }
            try
            {
                this.thread.BindEndThread(ref rpc);
            }
            catch (Exception exception)
            {
                if (DiagnosticUtility.IsFatal(exception))
                {
                    throw;
                }
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperFatal(exception.Message, exception);
            }
            if (!rpc.IsPaused)
            {
                this.ProcessMessage7(ref rpc);
            }
        }

        private void ProcessMessage7(ref MessageRpc rpc)
        {
            rpc.NextProcessor = null;
            try
            {
                if (this.transaction != null)
                {
                    this.transaction.InitializeCallContext(ref rpc);
                }
                rpc.Operation.InvokeEnd(ref rpc);
            }
            catch
            {
                throw;
            }
            finally
            {
                if (this.transaction != null)
                {
                    this.transaction.ClearCallContext(ref rpc);
                }
            }
            this.ProcessMessageCleanup(ref rpc);
        }

        private void ProcessMessageCleanup(ref MessageRpc rpc)
        {
            rpc.ErrorProcessor = this.processMessageCleanupError;
            try
            {
                if (this.transaction != null)
                {
                    try
                    {
                        this.transaction.BeforeReply(ref rpc);
                    }
                    catch (FaultException exception)
                    {
                        if (rpc.Error == null)
                        {
                            rpc.Error = exception;
                        }
                    }
                    catch (Exception exception2)
                    {
                        if (DiagnosticUtility.IsFatal(exception2))
                        {
                            throw;
                        }
                        this.error.HandleError(exception2);
                    }
                }
                try
                {
                    this.error.ProvideMessageFault(ref rpc);
                }
                catch (Exception exception3)
                {
                    if (DiagnosticUtility.IsFatal(exception3))
                    {
                        throw;
                    }
                    this.error.HandleError(exception3);
                }
                bool flag = this.Reply(ref rpc);
                try
                {
                    if (rpc.DidDeserializeRequestBody)
                    {
                        rpc.Request.Close();
                    }
                }
                catch (Exception exception4)
                {
                    if (DiagnosticUtility.IsFatal(exception4))
                    {
                        throw;
                    }
                    this.error.HandleError(exception4);
                }
                if (rpc.HostingProperty != null)
                {
                    try
                    {
                        rpc.HostingProperty.Close();
                    }
                    catch (Exception exception5)
                    {
                        if (DiagnosticUtility.IsFatal(exception5))
                        {
                            throw;
                        }
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperFatal(exception5.Message, exception5);
                    }
                }
                rpc.DisposeParameters();
                if (rpc.FaultInfo.IsConsideredUnhandled)
                {
                    if (!flag)
                    {
                        rpc.AbortRequestContext();
                        rpc.AbortChannel();
                    }
                    else
                    {
                        rpc.CloseRequestContext();
                        rpc.CloseChannel();
                    }
                    rpc.AbortInstanceContext();
                }
                else if (rpc.RequestContextThrewOnReply)
                {
                    rpc.AbortRequestContext();
                }
                else
                {
                    rpc.CloseRequestContext();
                }
                if ((rpc.Reply != null) && (rpc.Reply != rpc.ReturnParameter))
                {
                    try
                    {
                        rpc.Reply.Close();
                    }
                    catch (Exception exception6)
                    {
                        if (DiagnosticUtility.IsFatal(exception6))
                        {
                            throw;
                        }
                        this.error.HandleError(exception6);
                    }
                }
                if ((rpc.FaultInfo.Fault != null) && (rpc.FaultInfo.Fault.State != MessageState.Closed))
                {
                    try
                    {
                        rpc.FaultInfo.Fault.Close();
                    }
                    catch (Exception exception7)
                    {
                        if (DiagnosticUtility.IsFatal(exception7))
                        {
                            throw;
                        }
                        this.error.HandleError(exception7);
                    }
                }
                try
                {
                    rpc.OperationContext.FireOperationCompleted();
                }
                catch (Exception exception8)
                {
                    if (DiagnosticUtility.IsFatal(exception8))
                    {
                        throw;
                    }
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperCallback(exception8);
                }
                this.instance.AfterReply(ref rpc, this.error);
                if (rpc.SuccessfullyLockedInstance)
                {
                    try
                    {
                        this.concurrency.UnlockInstance(ref rpc);
                    }
                    catch (Exception exception9)
                    {
                        if (DiagnosticUtility.IsFatal(exception9))
                        {
                            throw;
                        }
                        rpc.InstanceContext.FaultInternal();
                        this.error.HandleError(exception9);
                    }
                }
                if (this.terminate != null)
                {
                    try
                    {
                        this.terminate.AfterReply(ref rpc);
                    }
                    catch (Exception exception10)
                    {
                        if (DiagnosticUtility.IsFatal(exception10))
                        {
                            throw;
                        }
                        this.error.HandleError(exception10);
                    }
                }
                if (rpc.SuccessfullyIncrementedActivity)
                {
                    try
                    {
                        rpc.Channel.DecrementActivity();
                    }
                    catch (Exception exception11)
                    {
                        if (DiagnosticUtility.IsFatal(exception11))
                        {
                            throw;
                        }
                        this.error.HandleError(exception11);
                    }
                }
            }
            finally
            {
                if (rpc.MessageRpcOwnsInstanceContextThrottle && (rpc.channelHandler.InstanceContextServiceThrottle != null))
                {
                    rpc.channelHandler.InstanceContextServiceThrottle.DeactivateInstanceContext();
                }
                if ((rpc.Activity != null) && DiagnosticUtility.ShouldUseActivity)
                {
                    rpc.Activity.Stop();
                }
            }
            this.error.HandleError(ref rpc);
        }

        private void ProcessMessageCleanupError(ref MessageRpc rpc)
        {
            this.error.HandleError(ref rpc);
        }

        private bool Reply(ref MessageRpc rpc)
        {
            RequestContext requestContext = rpc.OperationContext.RequestContext;
            bool flag = true;
            bool flag2 = false;
            Exception exception = null;
            bool thereIsAnUnhandledException = false;
            if (!rpc.Operation.IsOneWay)
            {
                if (DiagnosticUtility.ShouldTraceWarning)
                {
                    if ((rpc.Reply == null) && (requestContext != null))
                    {
                        object[] args = new object[] { rpc.Operation.Name ?? string.Empty };
                        DiagnosticUtility.DiagnosticTrace.TraceEvent(TraceEventType.Warning, TraceCode.ServiceOperationMissingReply, System.ServiceModel.SR.GetString("TraceCodeServiceOperationMissingReply", args), null, null);
                    }
                    else if ((requestContext == null) && (rpc.Reply != null))
                    {
                        object[] objArray2 = new object[] { rpc.Operation.Name ?? string.Empty };
                        DiagnosticUtility.DiagnosticTrace.TraceEvent(TraceEventType.Warning, TraceCode.ServiceOperationMissingReplyContext, System.ServiceModel.SR.GetString("TraceCodeServiceOperationMissingReplyContext", objArray2), null, null);
                    }
                }
                if ((requestContext != null) && (rpc.Reply != null))
                {
                    try
                    {
                        flag = this.PrepareAndAddressReply(ref rpc);
                    }
                    catch (Exception exception2)
                    {
                        if (DiagnosticUtility.IsFatal(exception2))
                        {
                            throw;
                        }
                        thereIsAnUnhandledException = !this.error.HandleError(exception2) || thereIsAnUnhandledException;
                        exception = exception2;
                    }
                }
            }
            this.BeforeSendReply(ref rpc, ref exception, ref thereIsAnUnhandledException);
            if ((!rpc.Operation.IsOneWay && (requestContext != null)) && (rpc.Reply != null))
            {
                if (exception != null)
                {
                    rpc.Error = exception;
                    this.error.ProvideOnlyFaultOfLastResort(ref rpc);
                    try
                    {
                        flag = this.PrepareAndAddressReply(ref rpc);
                    }
                    catch (Exception exception3)
                    {
                        if (DiagnosticUtility.IsFatal(exception3))
                        {
                            throw;
                        }
                        this.error.HandleError(exception3);
                    }
                }
                try
                {
                    if (flag)
                    {
                        rpc.RequestContextThrewOnReply = true;
                        requestContext.Reply(rpc.Reply);
                        flag2 = true;
                        rpc.RequestContextThrewOnReply = false;
                    }
                    return flag2;
                }
                catch (CommunicationException exception4)
                {
                    this.error.HandleError(exception4);
                    return flag2;
                }
                catch (TimeoutException exception5)
                {
                    this.error.HandleError(exception5);
                    return flag2;
                }
                catch (Exception exception6)
                {
                    if (DiagnosticUtility.IsFatal(exception6))
                    {
                        throw;
                    }
                    if (DiagnosticUtility.ShouldTraceError)
                    {
                        TraceUtility.TraceEvent(TraceEventType.Error, TraceCode.ServiceOperationExceptionOnReply, this, exception6);
                    }
                    if (!this.error.HandleError(exception6))
                    {
                        rpc.Abort();
                    }
                    return flag2;
                }
            }
            if ((exception != null) && thereIsAnUnhandledException)
            {
                rpc.Abort();
            }
            return flag2;
        }

        private void TransferChannelFromPendingList(ref MessageRpc rpc)
        {
            if (rpc.Channel.IsPending)
            {
                rpc.Channel.IsPending = false;
                ChannelDispatcher channelDispatcher = rpc.Channel.ChannelDispatcher;
                IInstanceContextProvider instanceContextProvider = this.instance.InstanceContextProvider;
                if (!InstanceContextProviderBase.IsProviderSessionful(instanceContextProvider) && !InstanceContextProviderBase.IsProviderSingleton(instanceContextProvider))
                {
                    IChannel proxy = rpc.Channel.Proxy as IChannel;
                    if (!rpc.InstanceContext.IncomingChannels.Contains(proxy))
                    {
                        channelDispatcher.Channels.Add(proxy);
                    }
                }
                channelDispatcher.PendingChannels.Remove(rpc.Channel.Binder.Channel);
            }
        }

        internal int CallContextCorrelationOffset =>
            this.messageInspectors.Length;

        internal int CorrelationCount =>
            this.correlationCount;

        internal bool EnableFaults =>
            this.enableFaults;

        internal System.ServiceModel.Dispatcher.ErrorBehavior ErrorBehavior =>
            this.error;

        internal System.ServiceModel.Dispatcher.InstanceBehavior InstanceBehavior =>
            this.instance;

        internal bool ManualAddressing =>
            this.manualAddressing;

        internal int MessageInspectorCorrelationOffset =>
            0;

        internal int ParameterInspectorCorrelationOffset =>
            this.parameterInspectorCorrelationOffset;

        internal IRequestReplyCorrelator RequestReplyCorrelator =>
            this.requestReplyCorrelator;

        internal SecurityImpersonationBehavior SecurityImpersonation =>
            this.securityImpersonation;

        internal bool ValidateMustUnderstand =>
            this.validateMustUnderstand;

        private class ActionDemuxer : ImmutableDispatchRuntime.IDemuxer
        {
            private HybridDictionary map = new HybridDictionary();
            private DispatchOperationRuntime unhandled;

            internal ActionDemuxer()
            {
            }

            internal void Add(string action, DispatchOperationRuntime operation)
            {
                if (this.map.Contains(action))
                {
                    DispatchOperationRuntime runtime = (DispatchOperationRuntime) this.map[action];
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(System.ServiceModel.SR.GetString("SFxActionDemuxerDuplicate", new object[] { runtime.Name, operation.Name, action })));
                }
                this.map.Add(action, operation);
            }

            public DispatchOperationRuntime GetOperation(ref Message request)
            {
                string action = request.Headers.Action;
                if (action == null)
                {
                    action = "*";
                }
                DispatchOperationRuntime runtime = (DispatchOperationRuntime) this.map[action];
                if (runtime != null)
                {
                    return runtime;
                }
                return this.unhandled;
            }

            internal void SetUnhandled(DispatchOperationRuntime operation)
            {
                this.unhandled = operation;
            }
        }

        private class CustomDemuxer : ImmutableDispatchRuntime.IDemuxer
        {
            private Dictionary<string, DispatchOperationRuntime> map;
            private IDispatchOperationSelector selector;
            private DispatchOperationRuntime unhandled;

            internal CustomDemuxer(IDispatchOperationSelector selector)
            {
                this.selector = selector;
                this.map = new Dictionary<string, DispatchOperationRuntime>();
            }

            internal void Add(string name, DispatchOperationRuntime operation)
            {
                this.map.Add(name, operation);
            }

            public DispatchOperationRuntime GetOperation(ref Message request)
            {
                string key = this.selector.SelectOperation(ref request);
                DispatchOperationRuntime runtime = null;
                if (this.map.TryGetValue(key, out runtime))
                {
                    return runtime;
                }
                return this.unhandled;
            }

            internal void SetUnhandled(DispatchOperationRuntime operation)
            {
                this.unhandled = operation;
            }
        }

        private interface IDemuxer
        {
            DispatchOperationRuntime GetOperation(ref Message request);
        }
    }
}

