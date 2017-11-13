namespace System.ServiceModel.Dispatcher
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Diagnostics;
    using System.ServiceModel.Security;
    using System.Threading;
    using System.Xml;

    internal class DuplexChannelBinder : IChannelBinder
    {
        private IDuplexChannel channel;
        private System.ServiceModel.Dispatcher.ChannelHandler channelHandler;
        private IRequestReplyCorrelator correlator;
        private TimeSpan defaultCloseTimeout;
        private TimeSpan defaultSendTimeout;
        private bool faulted;
        private System.ServiceModel.Security.IdentityVerifier identityVerifier;
        private bool isSession;
        private Uri listenUri;
        private int pending;
        private List<IDuplexRequest> requests;
        private bool syncPumpEnabled;

        internal DuplexChannelBinder(IDuplexChannel channel, IRequestReplyCorrelator correlator) : this(channel, correlator, null)
        {
        }

        internal DuplexChannelBinder(IDuplexSessionChannel channel, IRequestReplyCorrelator correlator) : this(channel, correlator, null)
        {
        }

        internal DuplexChannelBinder(IDuplexChannel channel, IRequestReplyCorrelator correlator, Uri listenUri)
        {
            if (channel == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("channel");
            }
            if (correlator == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("correlator");
            }
            this.channel = channel;
            this.correlator = correlator;
            this.listenUri = listenUri;
            this.channel.Faulted += new EventHandler(this.OnFaulted);
        }

        internal DuplexChannelBinder(IDuplexSessionChannel channel, IRequestReplyCorrelator correlator, Uri listenUri) : this((IDuplexChannel) channel, correlator, listenUri)
        {
            this.isSession = true;
        }

        public void Abort()
        {
            this.channel.Abort();
            this.AbortRequests();
        }

        private void AbortRequests()
        {
            lock (this.ThisLock)
            {
                if (this.requests != null)
                {
                    foreach (IDuplexRequest request in this.requests.ToArray())
                    {
                        request.Abort();
                    }
                }
                this.requests = null;
            }
        }

        public IAsyncResult BeginRequest(Message message, TimeSpan timeout, AsyncCallback callback, object state)
        {
            IAsyncResult result2;
            bool flag = false;
            AsyncDuplexRequest request = null;
            try
            {
                RequestReplyCorrelator.PrepareRequest(message);
                request = new AsyncDuplexRequest(message, this, timeout, callback, state);
                lock (this.ThisLock)
                {
                    this.RequestStarting(message, request);
                }
                IAsyncResult sendResult = this.channel.BeginSend(message, timeout, DiagnosticUtility.ThunkAsyncCallback(new AsyncCallback(this.SendCallback)), request);
                if (sendResult.CompletedSynchronously)
                {
                    request.FinishedSend(sendResult, true);
                }
                this.EnsurePumping();
                flag = true;
                result2 = request;
            }
            finally
            {
                lock (this.ThisLock)
                {
                    if (flag)
                    {
                        request.EnableCompletion();
                    }
                    else
                    {
                        this.RequestCompleting(request);
                    }
                }
            }
            return result2;
        }

        public IAsyncResult BeginSend(Message message, TimeSpan timeout, AsyncCallback callback, object state) => 
            this.channel.BeginSend(message, timeout, callback, state);

        public IAsyncResult BeginTryReceive(TimeSpan timeout, AsyncCallback callback, object state)
        {
            if (this.channel.State == CommunicationState.Faulted)
            {
                return new ChannelFaultedAsyncResult(callback, state);
            }
            return this.channel.BeginTryReceive(timeout, callback, state);
        }

        public IAsyncResult BeginWaitForMessage(TimeSpan timeout, AsyncCallback callback, object state) => 
            this.channel.BeginWaitForMessage(timeout, callback, state);

        public void CloseAfterFault(TimeSpan timeout)
        {
            this.channel.Close(timeout);
            this.AbortRequests();
        }

        public Message EndRequest(IAsyncResult result)
        {
            AsyncDuplexRequest request = result as AsyncDuplexRequest;
            return request?.End();
        }

        public void EndSend(IAsyncResult result)
        {
            this.channel.EndSend(result);
        }

        public bool EndTryReceive(IAsyncResult result, out RequestContext requestContext)
        {
            Message message;
            if (result is ChannelFaultedAsyncResult)
            {
                this.AbortRequests();
                requestContext = null;
                return true;
            }
            if (this.channel.EndTryReceive(result, out message))
            {
                if (message != null)
                {
                    requestContext = new DuplexRequestContext(this.channel, message, this);
                }
                else
                {
                    this.AbortRequests();
                    requestContext = null;
                }
                return true;
            }
            requestContext = null;
            return false;
        }

        public bool EndWaitForMessage(IAsyncResult result) => 
            this.channel.EndWaitForMessage(result);

        [MethodImpl(MethodImplOptions.NoInlining)]
        private void EnsureIncomingIdentity(SecurityMessageProperty property, EndpointAddress address, Message reply)
        {
            this.IdentityVerifier.EnsureIncomingIdentity(address, property.ServiceSecurityContext.AuthorizationContext);
        }

        public void EnsurePumping()
        {
            lock (this.ThisLock)
            {
                if (!this.syncPumpEnabled && !this.ChannelHandler.HasRegisterBeenCalled)
                {
                    System.ServiceModel.Dispatcher.ChannelHandler.Register(this.ChannelHandler);
                }
            }
        }

        private TimeoutException GetReceiveTimeoutException(TimeSpan timeout)
        {
            EndpointAddress address = this.channel.RemoteAddress ?? this.channel.LocalAddress;
            if (address != null)
            {
                return new TimeoutException(System.ServiceModel.SR.GetString("SFxRequestTimedOut2", new object[] { address, timeout }));
            }
            return new TimeoutException(System.ServiceModel.SR.GetString("SFxRequestTimedOut1", new object[] { timeout }));
        }

        internal bool HandleRequestAsReply(Message message)
        {
            UniqueId relatesTo = null;
            try
            {
                relatesTo = message.Headers.RelatesTo;
            }
            catch (MessageHeaderException)
            {
            }
            if (relatesTo == null)
            {
                return false;
            }
            return this.HandleRequestAsReplyCore(message);
        }

        private bool HandleRequestAsReplyCore(Message message)
        {
            IDuplexRequest request = this.correlator.Find<IDuplexRequest>(message, true);
            if (request != null)
            {
                request.GotReply(message);
                return true;
            }
            return false;
        }

        private void OnFaulted(object sender, EventArgs e)
        {
            this.AbortRequests();
        }

        public Message Request(Message message, TimeSpan timeout)
        {
            SyncDuplexRequest request = null;
            bool flag = false;
            RequestReplyCorrelator.PrepareRequest(message);
            lock (this.ThisLock)
            {
                if (!this.Pumping)
                {
                    flag = true;
                    this.syncPumpEnabled = true;
                }
                if (!flag)
                {
                    request = new SyncDuplexRequest(this);
                }
                this.RequestStarting(message, request);
            }
            if (flag)
            {
                TimeoutHelper helper = new TimeoutHelper(timeout);
                UniqueId messageId = message.Headers.MessageId;
                try
                {
                    this.channel.Send(message, helper.RemainingTime());
                    if ((DiagnosticUtility.ShouldUseActivity && (ServiceModelActivity.Current != null)) && (ServiceModelActivity.Current.ActivityType == ActivityType.ProcessAction))
                    {
                        ServiceModelActivity.Current.Suspend();
                    }
                    while (true)
                    {
                        Message message2;
                        TimeSpan span = helper.RemainingTime();
                        if (!this.channel.TryReceive(span, out message2))
                        {
                            throw TraceUtility.ThrowHelperError(this.GetReceiveTimeoutException(timeout), message);
                        }
                        if (message2 == null)
                        {
                            this.AbortRequests();
                            return null;
                        }
                        if (message2.Headers.RelatesTo == messageId)
                        {
                            this.ThrowIfInvalidReplyIdentity(message2);
                            return message2;
                        }
                        if (!this.HandleRequestAsReply(message2))
                        {
                            if (DiagnosticUtility.ShouldTraceInformation)
                            {
                                EndpointDispatcher endpointDispatcher = null;
                                if ((this.ChannelHandler != null) && (this.ChannelHandler.Channel != null))
                                {
                                    endpointDispatcher = this.ChannelHandler.Channel.EndpointDispatcher;
                                }
                                TraceUtility.TraceDroppedMessage(message2, endpointDispatcher);
                            }
                            message2.Close();
                        }
                    }
                }
                finally
                {
                    lock (this.ThisLock)
                    {
                        this.RequestCompleting(null);
                        this.syncPumpEnabled = false;
                        if (this.pending > 0)
                        {
                            this.EnsurePumping();
                        }
                    }
                }
            }
            TimeoutHelper helper2 = new TimeoutHelper(timeout);
            this.channel.Send(message, helper2.RemainingTime());
            this.EnsurePumping();
            return request.WaitForReply(helper2.RemainingTime());
        }

        private void RequestCompleting(IDuplexRequest request)
        {
            this.pending--;
            if (this.pending == 0)
            {
                this.requests = null;
            }
            else if ((request != null) && (this.requests != null))
            {
                this.requests.Remove(request);
            }
        }

        private void RequestStarting(Message message, IDuplexRequest request)
        {
            if (request != null)
            {
                this.Requests.Add(request);
                this.correlator.Add<IDuplexRequest>(message, request);
            }
            this.pending++;
        }

        public void Send(Message message, TimeSpan timeout)
        {
            this.channel.Send(message, timeout);
        }

        private void SendCallback(IAsyncResult result)
        {
            AsyncDuplexRequest asyncState = result.AsyncState as AsyncDuplexRequest;
            if (!result.CompletedSynchronously)
            {
                asyncState.FinishedSend(result, false);
            }
        }

        private void ThrowIfInvalidReplyIdentity(Message reply)
        {
            if (!this.isSession)
            {
                SecurityMessageProperty security = reply.Properties.Security;
                EndpointAddress remoteAddress = this.channel.RemoteAddress;
                if ((security != null) && (remoteAddress != null))
                {
                    this.EnsureIncomingIdentity(security, remoteAddress, reply);
                }
            }
        }

        public bool TryReceive(TimeSpan timeout, out RequestContext requestContext)
        {
            Message message;
            if (this.channel.State == CommunicationState.Faulted)
            {
                this.AbortRequests();
                requestContext = null;
                return true;
            }
            if (this.channel.TryReceive(timeout, out message))
            {
                if (message != null)
                {
                    requestContext = new DuplexRequestContext(this.channel, message, this);
                }
                else
                {
                    this.AbortRequests();
                    requestContext = null;
                }
                return true;
            }
            requestContext = null;
            return false;
        }

        public bool WaitForMessage(TimeSpan timeout) => 
            this.channel.WaitForMessage(timeout);

        public IChannel Channel =>
            this.channel;

        internal System.ServiceModel.Dispatcher.ChannelHandler ChannelHandler
        {
            get
            {
                System.ServiceModel.Dispatcher.ChannelHandler channelHandler = this.channelHandler;
                return this.channelHandler;
            }
            set
            {
                System.ServiceModel.Dispatcher.ChannelHandler channelHandler = this.channelHandler;
                this.channelHandler = value;
            }
        }

        public TimeSpan DefaultCloseTimeout
        {
            get => 
                this.defaultCloseTimeout;
            set
            {
                this.defaultCloseTimeout = value;
            }
        }

        public TimeSpan DefaultSendTimeout
        {
            get => 
                this.defaultSendTimeout;
            set
            {
                this.defaultSendTimeout = value;
            }
        }

        public bool HasSession =>
            this.isSession;

        internal System.ServiceModel.Security.IdentityVerifier IdentityVerifier
        {
            get
            {
                if (this.identityVerifier == null)
                {
                    this.identityVerifier = System.ServiceModel.Security.IdentityVerifier.CreateDefault();
                }
                return this.identityVerifier;
            }
            set
            {
                if (value == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("value");
                }
                this.identityVerifier = value;
            }
        }

        internal bool IsFaulted =>
            this.faulted;

        public Uri ListenUri =>
            this.listenUri;

        public EndpointAddress LocalAddress =>
            this.channel.LocalAddress;

        private bool Pumping =>
            (this.syncPumpEnabled || ((this.channelHandler != null) && this.channelHandler.HasRegisterBeenCalled));

        public EndpointAddress RemoteAddress =>
            this.channel.RemoteAddress;

        private List<IDuplexRequest> Requests
        {
            get
            {
                lock (this.ThisLock)
                {
                    if (this.requests == null)
                    {
                        this.requests = new List<IDuplexRequest>();
                    }
                    return this.requests;
                }
            }
        }

        private object ThisLock =>
            this;

        private class AsyncDuplexRequest : AsyncResult, DuplexChannelBinder.IDuplexRequest
        {
            private bool aborted;
            private ServiceModelActivity activity;
            private bool enableComplete;
            private bool gotReply;
            private DuplexChannelBinder parent;
            private Message reply;
            private Exception sendException;
            private IAsyncResult sendResult;
            private bool timedOut;
            private TimeSpan timeout;
            private IOThreadTimer timer;
            private static WaitCallback timerCallback = new WaitCallback(DuplexChannelBinder.AsyncDuplexRequest.TimerCallback);

            internal AsyncDuplexRequest(Message message, DuplexChannelBinder parent, TimeSpan timeout, AsyncCallback callback, object state) : base(callback, state)
            {
                this.parent = parent;
                this.timeout = timeout;
                if (timeout != TimeSpan.MaxValue)
                {
                    this.timer = new IOThreadTimer(timerCallback, this, true);
                    this.timer.Set(timeout);
                }
                if (DiagnosticUtility.ShouldUseActivity)
                {
                    this.activity = TraceUtility.ExtractActivity(message);
                }
            }

            public void Abort()
            {
                bool flag;
                lock (this.parent.ThisLock)
                {
                    bool isDone = this.IsDone;
                    this.aborted = true;
                    flag = !isDone && this.IsDone;
                }
                if (flag)
                {
                    this.Done(false);
                }
            }

            private void Done(bool completedSynchronously)
            {
                ServiceModelActivity activity = DiagnosticUtility.ShouldUseActivity ? TraceUtility.ExtractActivity(this.reply) : null;
                using (ServiceModelActivity.BoundOperation(activity))
                {
                    if (this.timer != null)
                    {
                        this.timer.Cancel();
                        this.timer = null;
                    }
                    lock (this.parent.ThisLock)
                    {
                        this.parent.RequestCompleting(this);
                    }
                    if (this.sendException != null)
                    {
                        base.Complete(completedSynchronously, this.sendException);
                    }
                    else if (this.timedOut)
                    {
                        base.Complete(completedSynchronously, this.parent.GetReceiveTimeoutException(this.timeout));
                    }
                    else
                    {
                        base.Complete(completedSynchronously);
                    }
                }
            }

            public void EnableCompletion()
            {
                bool flag;
                lock (this.parent.ThisLock)
                {
                    bool isDone = this.IsDone;
                    this.enableComplete = true;
                    flag = !isDone && this.IsDone;
                }
                if (flag)
                {
                    this.Done(true);
                }
            }

            internal Message End()
            {
                AsyncResult.End<DuplexChannelBinder.AsyncDuplexRequest>(this);
                this.parent.ThrowIfInvalidReplyIdentity(this.reply);
                return this.reply;
            }

            public void FinishedSend(IAsyncResult sendResult, bool completedSynchronously)
            {
                Exception exception = null;
                bool flag;
                try
                {
                    this.parent.channel.EndSend(sendResult);
                }
                catch (Exception exception2)
                {
                    if (DiagnosticUtility.IsFatal(exception2))
                    {
                        throw;
                    }
                    exception = exception2;
                }
                lock (this.parent.ThisLock)
                {
                    bool isDone = this.IsDone;
                    this.sendResult = sendResult;
                    this.sendException = exception;
                    flag = !isDone && this.IsDone;
                }
                if (flag)
                {
                    this.Done(completedSynchronously);
                }
            }

            public void GotReply(Message reply)
            {
                bool flag;
                ServiceModelActivity activity = DiagnosticUtility.ShouldUseActivity ? TraceUtility.ExtractActivity(reply) : null;
                using (ServiceModelActivity.BoundOperation(activity))
                {
                    lock (this.parent.ThisLock)
                    {
                        bool isDone = this.IsDone;
                        this.reply = reply;
                        this.gotReply = true;
                        flag = !isDone && this.IsDone;
                    }
                    if ((activity != null) && DiagnosticUtility.ShouldUseActivity)
                    {
                        TraceUtility.SetActivity(reply, this.activity);
                        if (DiagnosticUtility.ShouldUseActivity && (this.activity != null))
                        {
                            DiagnosticUtility.DiagnosticTrace.TraceTransfer(this.activity.Id);
                        }
                    }
                }
                if (DiagnosticUtility.ShouldUseActivity && (activity != null))
                {
                    activity.Stop();
                }
                if (flag)
                {
                    this.Done(false);
                }
            }

            private void TimedOut()
            {
                bool flag;
                lock (this.parent.ThisLock)
                {
                    bool isDone = this.IsDone;
                    this.timedOut = true;
                    flag = !isDone && this.IsDone;
                }
                if (flag)
                {
                    this.Done(false);
                }
            }

            private static void TimerCallback(object state)
            {
                ((DuplexChannelBinder.AsyncDuplexRequest) state).TimedOut();
            }

            private bool IsDone
            {
                get
                {
                    if (!this.enableComplete)
                    {
                        return false;
                    }
                    if (((this.sendResult == null) || !this.gotReply) && ((this.sendException == null) && !this.timedOut))
                    {
                        return this.aborted;
                    }
                    return true;
                }
            }
        }

        private class ChannelFaultedAsyncResult : CompletedAsyncResult
        {
            public ChannelFaultedAsyncResult(AsyncCallback callback, object state) : base(callback, state)
            {
            }
        }

        private class DuplexRequestContext : RequestContextBase
        {
            private DuplexChannelBinder binder;
            private IDuplexChannel channel;

            internal DuplexRequestContext(IDuplexChannel channel, Message request, DuplexChannelBinder binder) : base(request, binder.DefaultCloseTimeout, binder.DefaultSendTimeout)
            {
                this.channel = channel;
                this.binder = binder;
            }

            protected override void OnAbort()
            {
            }

            protected override IAsyncResult OnBeginReply(Message message, TimeSpan timeout, AsyncCallback callback, object state) => 
                new ReplyAsyncResult(this, message, timeout, callback, state);

            protected override void OnClose(TimeSpan timeout)
            {
            }

            protected override void OnEndReply(IAsyncResult result)
            {
                ReplyAsyncResult.End(result);
            }

            protected override void OnReply(Message message, TimeSpan timeout)
            {
                if (message != null)
                {
                    this.channel.Send(message, timeout);
                }
            }

            private class ReplyAsyncResult : AsyncResult
            {
                private DuplexChannelBinder.DuplexRequestContext context;
                private static AsyncCallback onSend;

                public ReplyAsyncResult(DuplexChannelBinder.DuplexRequestContext context, Message message, TimeSpan timeout, AsyncCallback callback, object state) : base(callback, state)
                {
                    if (message != null)
                    {
                        if (onSend == null)
                        {
                            onSend = DiagnosticUtility.ThunkAsyncCallback(new AsyncCallback(DuplexChannelBinder.DuplexRequestContext.ReplyAsyncResult.OnSend));
                        }
                        this.context = context;
                        IAsyncResult result = context.channel.BeginSend(message, timeout, onSend, this);
                        if (!result.CompletedSynchronously)
                        {
                            return;
                        }
                        context.channel.EndSend(result);
                    }
                    base.Complete(true);
                }

                public static void End(IAsyncResult result)
                {
                    AsyncResult.End<DuplexChannelBinder.DuplexRequestContext.ReplyAsyncResult>(result);
                }

                private static void OnSend(IAsyncResult result)
                {
                    if (!result.CompletedSynchronously)
                    {
                        Exception exception = null;
                        DuplexChannelBinder.DuplexRequestContext.ReplyAsyncResult asyncState = (DuplexChannelBinder.DuplexRequestContext.ReplyAsyncResult) result.AsyncState;
                        try
                        {
                            asyncState.context.channel.EndSend(result);
                        }
                        catch (Exception exception2)
                        {
                            if (DiagnosticUtility.IsFatal(exception2))
                            {
                                throw;
                            }
                            exception = exception2;
                        }
                        asyncState.Complete(false, exception);
                    }
                }
            }
        }

        private interface IDuplexRequest
        {
            void Abort();
            void GotReply(Message reply);
        }

        private class SyncDuplexRequest : DuplexChannelBinder.IDuplexRequest
        {
            private DuplexChannelBinder parent;
            private Message reply;
            private ManualResetEvent wait = new ManualResetEvent(false);
            private int waitCount;

            internal SyncDuplexRequest(DuplexChannelBinder parent)
            {
                this.parent = parent;
            }

            public void Abort()
            {
                this.wait.Set();
            }

            private void CloseWaitHandle()
            {
                if (Interlocked.Increment(ref this.waitCount) == 2)
                {
                    this.wait.Close();
                }
            }

            public void GotReply(Message reply)
            {
                lock (this.parent.ThisLock)
                {
                    this.parent.RequestCompleting(this);
                }
                this.reply = reply;
                this.wait.Set();
                this.CloseWaitHandle();
            }

            internal Message WaitForReply(TimeSpan timeout)
            {
                try
                {
                    if (!TimeoutHelper.WaitOne(this.wait, timeout, false))
                    {
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(this.parent.GetReceiveTimeoutException(timeout));
                    }
                }
                finally
                {
                    this.CloseWaitHandle();
                }
                this.parent.ThrowIfInvalidReplyIdentity(this.reply);
                return this.reply;
            }
        }
    }
}

