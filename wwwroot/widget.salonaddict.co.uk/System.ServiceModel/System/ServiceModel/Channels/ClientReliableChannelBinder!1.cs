﻿namespace System.ServiceModel.Channels
{
    using System;
    using System.Runtime.InteropServices;
    using System.ServiceModel;
    using System.ServiceModel.Security;

    internal abstract class ClientReliableChannelBinder<TChannel> : ReliableChannelBinder<TChannel>, IClientReliableChannelBinder, IReliableChannelBinder where TChannel: class, IChannel
    {
        private ChannelParameterCollection channelParameters;
        private IChannelFactory<TChannel> factory;
        private EndpointAddress to;
        private Uri via;

        protected ClientReliableChannelBinder(EndpointAddress to, Uri via, IChannelFactory<TChannel> factory, MaskingMode maskingMode, TolerateFaultsMode faultMode, ChannelParameterCollection channelParameters, TimeSpan defaultCloseTimeout, TimeSpan defaultSendTimeout) : base(factory.CreateChannel(to, via), maskingMode, faultMode, defaultCloseTimeout, defaultSendTimeout)
        {
            if (channelParameters == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("channelParameters");
            }
            this.to = to;
            this.via = via;
            this.factory = factory;
            this.channelParameters = channelParameters;
        }

        public IAsyncResult BeginRequest(Message message, TimeSpan timeout, AsyncCallback callback, object state) => 
            this.BeginRequest(message, timeout, base.DefaultMaskingMode, callback, state);

        public IAsyncResult BeginRequest(Message message, TimeSpan timeout, MaskingMode maskingMode, AsyncCallback callback, object state)
        {
            RequestAsyncResult<TChannel> result = new RequestAsyncResult<TChannel>((ClientReliableChannelBinder<TChannel>) this, callback, state);
            result.Start(message, timeout, maskingMode);
            return result;
        }

        protected override IAsyncResult BeginTryGetChannel(TimeSpan timeout, AsyncCallback callback, object state)
        {
            TChannel local;
            switch (base.State)
            {
                case CommunicationState.Created:
                case CommunicationState.Opening:
                case CommunicationState.Opened:
                    local = this.factory.CreateChannel(this.to, this.via);
                    break;

                default:
                    local = default(TChannel);
                    break;
            }
            return new TypedCompletedAsyncResult<TChannel>(local, callback, state);
        }

        public static IClientReliableChannelBinder CreateBinder(EndpointAddress to, Uri via, IChannelFactory<TChannel> factory, MaskingMode maskingMode, TolerateFaultsMode faultMode, ChannelParameterCollection channelParameters, TimeSpan defaultCloseTimeout, TimeSpan defaultSendTimeout)
        {
            Type type = typeof(TChannel);
            if (type == typeof(IDuplexChannel))
            {
                return new DuplexClientReliableChannelBinder<TChannel>(to, via, (IChannelFactory<IDuplexChannel>) factory, maskingMode, channelParameters, defaultCloseTimeout, defaultSendTimeout);
            }
            if (type == typeof(IDuplexSessionChannel))
            {
                return new DuplexSessionClientReliableChannelBinder<TChannel>(to, via, (IChannelFactory<IDuplexSessionChannel>) factory, maskingMode, faultMode, channelParameters, defaultCloseTimeout, defaultSendTimeout);
            }
            if (type == typeof(IRequestChannel))
            {
                return new RequestClientReliableChannelBinder<TChannel>(to, via, (IChannelFactory<IRequestChannel>) factory, maskingMode, channelParameters, defaultCloseTimeout, defaultSendTimeout);
            }
            if (type != typeof(IRequestSessionChannel))
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperInternal(false);
            }
            return new RequestSessionClientReliableChannelBinder<TChannel>(to, via, (IChannelFactory<IRequestSessionChannel>) factory, maskingMode, faultMode, channelParameters, defaultCloseTimeout, defaultSendTimeout);
        }

        public Message EndRequest(IAsyncResult result) => 
            RequestAsyncResult<TChannel>.End(result);

        protected override bool EndTryGetChannel(IAsyncResult result)
        {
            TChannel channel = TypedCompletedAsyncResult<TChannel>.End(result);
            if ((channel != null) && !base.Synchronizer.SetChannel(channel))
            {
                channel.Abort();
            }
            return true;
        }

        public bool EnsureChannelForRequest() => 
            base.Synchronizer.EnsureChannel();

        protected override void OnAbort()
        {
        }

        protected override IAsyncResult OnBeginClose(TimeSpan timeout, AsyncCallback callback, object state) => 
            new CompletedAsyncResult(callback, state);

        protected override IAsyncResult OnBeginOpen(TimeSpan timeout, AsyncCallback callback, object state) => 
            new CompletedAsyncResult(callback, state);

        protected virtual IAsyncResult OnBeginRequest(TChannel channel, Message message, TimeSpan timeout, MaskingMode maskingMode, AsyncCallback callback, object state)
        {
            throw DiagnosticUtility.ExceptionUtility.ThrowHelperInternal(false);
        }

        protected override void OnClose(TimeSpan timeout)
        {
        }

        protected override void OnEndClose(IAsyncResult result)
        {
            CompletedAsyncResult.End(result);
        }

        protected override void OnEndOpen(IAsyncResult result)
        {
            CompletedAsyncResult.End(result);
        }

        protected virtual Message OnEndRequest(TChannel channel, MaskingMode maskingMode, IAsyncResult result)
        {
            throw DiagnosticUtility.ExceptionUtility.ThrowHelperInternal(false);
        }

        protected override void OnOpen(TimeSpan timeout)
        {
        }

        protected virtual Message OnRequest(TChannel channel, Message message, TimeSpan timeout, MaskingMode maskingMode)
        {
            throw DiagnosticUtility.ExceptionUtility.ThrowHelperInternal(false);
        }

        public Message Request(Message message, TimeSpan timeout) => 
            this.Request(message, timeout, base.DefaultMaskingMode);

        public Message Request(Message message, TimeSpan timeout, MaskingMode maskingMode)
        {
            Message message2;
            if (!base.ValidateOutputOperation(message, timeout, maskingMode))
            {
                return null;
            }
            bool autoAborted = false;
            try
            {
                TChannel local;
                TimeoutHelper helper = new TimeoutHelper(timeout);
                if (!base.Synchronizer.TryGetChannelForOutput(helper.RemainingTime(), maskingMode, out local))
                {
                    if (!ReliableChannelBinderHelper.MaskHandled(maskingMode))
                    {
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new TimeoutException(System.ServiceModel.SR.GetString("TimeoutOnRequest", new object[] { timeout })));
                    }
                    return null;
                }
                if (local == null)
                {
                    message2 = null;
                }
                else
                {
                    try
                    {
                        message2 = this.OnRequest(local, message, helper.RemainingTime(), maskingMode);
                    }
                    finally
                    {
                        autoAborted = base.Synchronizer.Aborting;
                        base.Synchronizer.ReturnChannel();
                    }
                }
            }
            catch (Exception exception)
            {
                if (DiagnosticUtility.IsFatal(exception))
                {
                    throw;
                }
                if (!base.HandleException(exception, maskingMode, autoAborted))
                {
                    throw;
                }
                message2 = null;
            }
            return message2;
        }

        protected override bool TryGetChannel(TimeSpan timeout)
        {
            CommunicationState state = base.State;
            TChannel channel = default(TChannel);
            switch (state)
            {
                case CommunicationState.Created:
                case CommunicationState.Opening:
                case CommunicationState.Opened:
                    channel = this.factory.CreateChannel(this.to, this.via);
                    if (!base.Synchronizer.SetChannel(channel))
                    {
                        channel.Abort();
                    }
                    break;

                default:
                    channel = default(TChannel);
                    break;
            }
            return true;
        }

        protected override bool CanGetChannelForReceive =>
            false;

        public override bool CanSendAsynchronously =>
            true;

        public override ChannelParameterCollection ChannelParameters =>
            this.channelParameters;

        protected override bool MustCloseChannel =>
            true;

        protected override bool MustOpenChannel =>
            true;

        public Uri Via =>
            this.via;

        private sealed class DuplexClientReliableChannelBinder : ClientReliableChannelBinder<TChannel>.DuplexClientReliableChannelBinder<IDuplexChannel>
        {
            public DuplexClientReliableChannelBinder(EndpointAddress to, Uri via, IChannelFactory<IDuplexChannel> factory, MaskingMode maskingMode, ChannelParameterCollection channelParameters, TimeSpan defaultCloseTimeout, TimeSpan defaultSendTimeout) : base(to, via, factory, maskingMode, TolerateFaultsMode.Never, channelParameters, defaultCloseTimeout, defaultSendTimeout)
            {
            }

            public override ISession GetInnerSession() => 
                null;

            protected override bool HasSecuritySession(IDuplexChannel channel) => 
                false;

            public override bool HasSession =>
                false;
        }

        private abstract class DuplexClientReliableChannelBinder<TDuplexChannel> : ClientReliableChannelBinder<TDuplexChannel> where TDuplexChannel: class, IDuplexChannel
        {
            public DuplexClientReliableChannelBinder(EndpointAddress to, Uri via, IChannelFactory<TDuplexChannel> factory, MaskingMode maskingMode, TolerateFaultsMode faultMode, ChannelParameterCollection channelParameters, TimeSpan defaultCloseTimeout, TimeSpan defaultSendTimeout) : base(to, via, factory, maskingMode, faultMode, channelParameters, defaultCloseTimeout, defaultSendTimeout)
            {
            }

            protected override IAsyncResult OnBeginSend(TDuplexChannel channel, Message message, TimeSpan timeout, AsyncCallback callback, object state) => 
                channel.BeginSend(message, timeout, callback, state);

            protected override IAsyncResult OnBeginTryReceive(TDuplexChannel channel, TimeSpan timeout, AsyncCallback callback, object state) => 
                channel.BeginTryReceive(timeout, callback, state);

            protected override void OnEndSend(TDuplexChannel channel, IAsyncResult result)
            {
                channel.EndSend(result);
            }

            protected override bool OnEndTryReceive(TDuplexChannel channel, IAsyncResult result, out RequestContext requestContext)
            {
                Message message;
                bool flag = channel.EndTryReceive(result, out message);
                if (flag && (message == null))
                {
                    this.OnReadNullMessage();
                }
                requestContext = base.WrapMessage(message);
                return flag;
            }

            protected virtual void OnReadNullMessage()
            {
            }

            protected override void OnSend(TDuplexChannel channel, Message message, TimeSpan timeout)
            {
                channel.Send(message, timeout);
            }

            protected override bool OnTryReceive(TDuplexChannel channel, TimeSpan timeout, out RequestContext requestContext)
            {
                Message message;
                bool flag = channel.TryReceive(timeout, out message);
                if (flag && (message == null))
                {
                    this.OnReadNullMessage();
                }
                requestContext = base.WrapMessage(message);
                return flag;
            }

            public override EndpointAddress LocalAddress
            {
                get
                {
                    IDuplexChannel currentChannel = base.Synchronizer.CurrentChannel;
                    return currentChannel?.LocalAddress;
                }
            }

            public override EndpointAddress RemoteAddress
            {
                get
                {
                    IDuplexChannel currentChannel = base.Synchronizer.CurrentChannel;
                    return currentChannel?.RemoteAddress;
                }
            }
        }

        private sealed class DuplexSessionClientReliableChannelBinder : ClientReliableChannelBinder<TChannel>.DuplexClientReliableChannelBinder<IDuplexSessionChannel>
        {
            public DuplexSessionClientReliableChannelBinder(EndpointAddress to, Uri via, IChannelFactory<IDuplexSessionChannel> factory, MaskingMode maskingMode, TolerateFaultsMode faultMode, ChannelParameterCollection channelParameters, TimeSpan defaultCloseTimeout, TimeSpan defaultSendTimeout) : base(to, via, factory, maskingMode, faultMode, channelParameters, defaultCloseTimeout, defaultSendTimeout)
            {
            }

            protected override IAsyncResult BeginCloseChannel(IDuplexSessionChannel channel, TimeSpan timeout, AsyncCallback callback, object state) => 
                ReliableChannelBinderHelper.BeginCloseDuplexSessionChannel(this, channel, timeout, callback, state);

            protected override void CloseChannel(IDuplexSessionChannel channel, TimeSpan timeout)
            {
                ReliableChannelBinderHelper.CloseDuplexSessionChannel(this, channel, timeout);
            }

            protected override void EndCloseChannel(IDuplexSessionChannel channel, IAsyncResult result)
            {
                ReliableChannelBinderHelper.EndCloseDuplexSessionChannel(channel, result);
            }

            public override ISession GetInnerSession() => 
                base.Synchronizer.CurrentChannel.Session;

            protected override bool HasSecuritySession(IDuplexSessionChannel channel) => 
                (channel.Session is ISecuritySession);

            protected override void OnReadNullMessage()
            {
                base.Synchronizer.OnReadEof();
            }

            public override bool HasSession =>
                true;
        }

        private sealed class RequestAsyncResult : ReliableChannelBinder<TChannel>.OutputAsyncResult<ClientReliableChannelBinder<TChannel>>
        {
            private Message reply;

            public RequestAsyncResult(ClientReliableChannelBinder<TChannel> binder, AsyncCallback callback, object state) : base(binder, callback, state)
            {
            }

            protected override IAsyncResult BeginOutput(ClientReliableChannelBinder<TChannel> binder, TChannel channel, Message message, TimeSpan timeout, MaskingMode maskingMode, AsyncCallback callback, object state) => 
                binder.OnBeginRequest(channel, message, timeout, maskingMode, callback, state);

            public static Message End(IAsyncResult result) => 
                AsyncResult.End<ClientReliableChannelBinder<TChannel>.RequestAsyncResult>(result).reply;

            protected override void EndOutput(ClientReliableChannelBinder<TChannel> binder, TChannel channel, MaskingMode maskingMode, IAsyncResult result)
            {
                this.reply = binder.OnEndRequest(channel, maskingMode, result);
            }

            protected override string GetTimeoutString(TimeSpan timeout) => 
                System.ServiceModel.SR.GetString("TimeoutOnRequest", new object[] { timeout });
        }

        private sealed class RequestClientReliableChannelBinder : ClientReliableChannelBinder<TChannel>.RequestClientReliableChannelBinder<IRequestChannel>
        {
            public RequestClientReliableChannelBinder(EndpointAddress to, Uri via, IChannelFactory<IRequestChannel> factory, MaskingMode maskingMode, ChannelParameterCollection channelParameters, TimeSpan defaultCloseTimeout, TimeSpan defaultSendTimeout) : base(to, via, factory, maskingMode, TolerateFaultsMode.Never, channelParameters, defaultCloseTimeout, defaultSendTimeout)
            {
            }

            public override ISession GetInnerSession() => 
                null;

            protected override bool HasSecuritySession(IRequestChannel channel) => 
                false;

            public override bool HasSession =>
                false;
        }

        private abstract class RequestClientReliableChannelBinder<TRequestChannel> : ClientReliableChannelBinder<TRequestChannel> where TRequestChannel: class, IRequestChannel
        {
            private InputQueue<Message> inputMessages;

            public RequestClientReliableChannelBinder(EndpointAddress to, Uri via, IChannelFactory<TRequestChannel> factory, MaskingMode maskingMode, TolerateFaultsMode faultMode, ChannelParameterCollection channelParameters, TimeSpan defaultCloseTimeout, TimeSpan defaultSendTimeout) : base(to, via, factory, maskingMode, faultMode, channelParameters, defaultCloseTimeout, defaultSendTimeout)
            {
            }

            public override IAsyncResult BeginTryReceive(TimeSpan timeout, AsyncCallback callback, object state) => 
                this.GetInputMessages().BeginDequeue(timeout, callback, state);

            public override bool EndTryReceive(IAsyncResult result, out RequestContext requestContext)
            {
                Message message;
                bool flag = this.GetInputMessages().EndDequeue(result, out message);
                requestContext = base.WrapMessage(message);
                return flag;
            }

            protected void EnqueueMessageIfNotNull(Message message)
            {
                if (message != null)
                {
                    this.GetInputMessages().EnqueueAndDispatch(message);
                }
            }

            private InputQueue<Message> GetInputMessages()
            {
                lock (base.ThisLock)
                {
                    if (base.State == CommunicationState.Created)
                    {
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperInternal(false);
                    }
                    if (base.State == CommunicationState.Opening)
                    {
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperInternal(false);
                    }
                    if (this.inputMessages == null)
                    {
                        this.inputMessages = new InputQueue<Message>();
                    }
                }
                return this.inputMessages;
            }

            protected override IAsyncResult OnBeginRequest(TRequestChannel channel, Message message, TimeSpan timeout, MaskingMode maskingMode, AsyncCallback callback, object state) => 
                channel.BeginRequest(message, timeout, callback, state);

            protected override IAsyncResult OnBeginSend(TRequestChannel channel, Message message, TimeSpan timeout, AsyncCallback callback, object state) => 
                channel.BeginRequest(message, timeout, callback, state);

            protected override Message OnEndRequest(TRequestChannel channel, MaskingMode maskingMode, IAsyncResult result) => 
                channel.EndRequest(result);

            protected override void OnEndSend(TRequestChannel channel, IAsyncResult result)
            {
                Message message = channel.EndRequest(result);
                this.EnqueueMessageIfNotNull(message);
            }

            protected override Message OnRequest(TRequestChannel channel, Message message, TimeSpan timeout, MaskingMode maskingMode) => 
                channel.Request(message, timeout);

            protected override void OnSend(TRequestChannel channel, Message message, TimeSpan timeout)
            {
                message = channel.Request(message, timeout);
                this.EnqueueMessageIfNotNull(message);
            }

            protected override void OnShutdown()
            {
                if (this.inputMessages != null)
                {
                    this.inputMessages.Close();
                }
            }

            public override bool TryReceive(TimeSpan timeout, out RequestContext requestContext)
            {
                Message message;
                bool flag = this.GetInputMessages().Dequeue(timeout, out message);
                requestContext = base.WrapMessage(message);
                return flag;
            }

            public override EndpointAddress LocalAddress =>
                EndpointAddress.AnonymousAddress;

            public override EndpointAddress RemoteAddress
            {
                get
                {
                    IRequestChannel currentChannel = base.Synchronizer.CurrentChannel;
                    return currentChannel?.RemoteAddress;
                }
            }
        }

        private sealed class RequestSessionClientReliableChannelBinder : ClientReliableChannelBinder<TChannel>.RequestClientReliableChannelBinder<IRequestSessionChannel>
        {
            public RequestSessionClientReliableChannelBinder(EndpointAddress to, Uri via, IChannelFactory<IRequestSessionChannel> factory, MaskingMode maskingMode, TolerateFaultsMode faultMode, ChannelParameterCollection channelParameters, TimeSpan defaultCloseTimeout, TimeSpan defaultSendTimeout) : base(to, via, factory, maskingMode, faultMode, channelParameters, defaultCloseTimeout, defaultSendTimeout)
            {
            }

            public override ISession GetInnerSession() => 
                base.Synchronizer.CurrentChannel.Session;

            protected override bool HasSecuritySession(IRequestSessionChannel channel) => 
                (channel.Session is ISecuritySession);

            public override bool HasSession =>
                true;
        }
    }
}

