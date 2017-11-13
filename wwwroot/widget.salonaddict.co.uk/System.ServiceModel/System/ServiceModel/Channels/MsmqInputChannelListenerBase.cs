namespace System.ServiceModel.Channels
{
    using System;
    using System.ServiceModel;

    internal abstract class MsmqInputChannelListenerBase : MsmqChannelListenerBase<IInputChannel>
    {
        private InputQueueChannelAcceptor<IInputChannel> acceptor;

        internal MsmqInputChannelListenerBase(MsmqBindingElementBase bindingElement, BindingContext context, MsmqReceiveParameters receiveParameters) : this(bindingElement, context, receiveParameters, TransportDefaults.GetDefaultMessageEncoderFactory())
        {
        }

        internal MsmqInputChannelListenerBase(MsmqBindingElementBase bindingElement, BindingContext context, MsmqReceiveParameters receiveParameters, MessageEncoderFactory encoderFactory) : base(bindingElement, context, receiveParameters, encoderFactory)
        {
            this.acceptor = new InputQueueChannelAcceptor<IInputChannel>(this);
        }

        public override IInputChannel AcceptChannel() => 
            this.AcceptChannel(this.DefaultReceiveTimeout);

        public override IInputChannel AcceptChannel(TimeSpan timeout) => 
            this.acceptor.AcceptChannel(timeout);

        public override IAsyncResult BeginAcceptChannel(AsyncCallback callback, object state) => 
            this.BeginAcceptChannel(this.DefaultReceiveTimeout, callback, state);

        public override IAsyncResult BeginAcceptChannel(TimeSpan timeout, AsyncCallback callback, object state) => 
            this.acceptor.BeginAcceptChannel(timeout, callback, state);

        protected abstract IInputChannel CreateInputChannel(MsmqInputChannelListenerBase listener);
        public override IInputChannel EndAcceptChannel(IAsyncResult result) => 
            this.acceptor.EndAcceptChannel(result);

        protected override IAsyncResult OnBeginWaitForChannel(TimeSpan timeout, AsyncCallback callback, object state) => 
            this.acceptor.BeginWaitForChannel(timeout, callback, state);

        protected override void OnCloseCore(bool aborting)
        {
            this.acceptor.Close();
            base.OnCloseCore(aborting);
        }

        protected override bool OnEndWaitForChannel(IAsyncResult result) => 
            this.acceptor.EndWaitForChannel(result);

        private void OnNewChannelNeeded(object sender, EventArgs ea)
        {
            if (!base.IsDisposed && ((CommunicationState.Opened == base.State) || (CommunicationState.Opening == base.State)))
            {
                IInputChannel channel = this.CreateInputChannel(this);
                channel.Closed += new EventHandler(this.OnNewChannelNeeded);
                this.acceptor.EnqueueAndDispatch(channel);
            }
        }

        protected override void OnOpenCore(TimeSpan timeout)
        {
            base.OnOpenCore(timeout);
            this.acceptor.Open();
            this.OnNewChannelNeeded(this, EventArgs.Empty);
        }

        protected override bool OnWaitForChannel(TimeSpan timeout) => 
            this.acceptor.WaitForChannel(timeout);
    }
}

