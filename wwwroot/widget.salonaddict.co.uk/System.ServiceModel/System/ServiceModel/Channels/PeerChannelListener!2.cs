namespace System.ServiceModel.Channels
{
    using System;
    using System.ServiceModel;

    internal abstract class PeerChannelListener<TChannel, TChannelAcceptor> : PeerChannelListenerBase, IChannelListener<TChannel>, IChannelListener, ICommunicationObject where TChannel: class, IChannel where TChannelAcceptor: ChannelAcceptor<TChannel>
    {
        public PeerChannelListener(PeerTransportBindingElement bindingElement, BindingContext context, PeerResolver peerResolver) : base(bindingElement, context, peerResolver)
        {
        }

        public TChannel AcceptChannel() => 
            this.AcceptChannel(this.DefaultReceiveTimeout);

        public TChannel AcceptChannel(TimeSpan timeout)
        {
            base.ThrowIfNotOpened();
            return this.ChannelAcceptor.AcceptChannel(timeout);
        }

        public IAsyncResult BeginAcceptChannel(AsyncCallback callback, object state) => 
            this.BeginAcceptChannel(this.DefaultReceiveTimeout, callback, state);

        public IAsyncResult BeginAcceptChannel(TimeSpan timeout, AsyncCallback callback, object state)
        {
            base.ThrowIfNotOpened();
            return this.ChannelAcceptor.BeginAcceptChannel(timeout, callback, state);
        }

        protected abstract void CreateAcceptor();
        internal override ITransportManagerRegistration CreateTransportManagerRegistration(Uri listenUri) => 
            null;

        public TChannel EndAcceptChannel(IAsyncResult result) => 
            this.ChannelAcceptor.EndAcceptChannel(result);

        protected override void OnAbort()
        {
            if (this.ChannelAcceptor != null)
            {
                this.ChannelAcceptor.Abort();
            }
            base.OnAbort();
        }

        protected override IAsyncResult OnBeginClose(TimeSpan timeout, AsyncCallback callback, object state) => 
            new TypedCompletedAsyncResult<TimeoutHelper>(new TimeoutHelper(timeout), callback, state);

        protected override IAsyncResult OnBeginOpen(TimeSpan timeout, AsyncCallback callback, object state) => 
            new TypedCompletedAsyncResult<TimeoutHelper>(new TimeoutHelper(timeout), callback, state);

        protected override IAsyncResult OnBeginWaitForChannel(TimeSpan timeout, AsyncCallback callback, object state) => 
            this.ChannelAcceptor.BeginWaitForChannel(timeout, callback, state);

        protected override void OnClose(TimeSpan timeout)
        {
            this.OnCloseCore(timeout);
        }

        private void OnCloseCore(TimeSpan timeout)
        {
            TimeoutHelper helper = new TimeoutHelper(timeout);
            this.ChannelAcceptor.Close(helper.RemainingTime());
            base.OnClose(helper.RemainingTime());
        }

        protected override void OnEndClose(IAsyncResult result)
        {
            TypedCompletedAsyncResult<TimeoutHelper> result2 = result as TypedCompletedAsyncResult<TimeoutHelper>;
            if (result2 != null)
            {
                this.OnCloseCore(result2.Data.RemainingTime());
            }
            else
            {
                base.OnEndClose(result);
            }
        }

        protected override void OnEndOpen(IAsyncResult result)
        {
            TypedCompletedAsyncResult<TimeoutHelper> result2 = result as TypedCompletedAsyncResult<TimeoutHelper>;
            if (result2 != null)
            {
                this.OnOpenCore(result2.Data.RemainingTime());
            }
            else
            {
                base.OnEndOpen(result);
            }
        }

        protected override bool OnEndWaitForChannel(IAsyncResult result) => 
            this.ChannelAcceptor.EndWaitForChannel(result);

        protected override void OnOpen(TimeSpan timeout)
        {
            this.OnOpenCore(new TimeoutHelper(timeout).RemainingTime());
        }

        private void OnOpenCore(TimeSpan timeout)
        {
            TimeoutHelper helper = new TimeoutHelper(timeout);
            base.OnOpen(helper.RemainingTime());
            this.CreateAcceptor();
            this.ChannelAcceptor.Open(helper.RemainingTime());
        }

        protected override bool OnWaitForChannel(TimeSpan timeout) => 
            this.ChannelAcceptor.WaitForChannel(timeout);

        protected abstract TChannelAcceptor ChannelAcceptor { get; }
    }
}

