namespace System.ServiceModel.Channels
{
    using System;

    internal class InputChannelDemuxer : DatagramChannelDemuxer<IInputChannel, Message>
    {
        public InputChannelDemuxer(BindingContext context) : base(context)
        {
        }

        protected override void AbortItem(Message message)
        {
            TypedChannelDemuxer.AbortMessage(message);
        }

        protected override IAsyncResult BeginReceive(TimeSpan timeout, AsyncCallback callback, object state) => 
            base.InnerChannel.BeginReceive(timeout, callback, state);

        protected override LayeredChannelListener<IInputChannel> CreateListener<IInputChannel>(ChannelDemuxerFilter filter) where IInputChannel: class, IChannel
        {
            SingletonChannelListener<IInputChannel, InputChannel, Message> channelManager = new SingletonChannelListener<IInputChannel, InputChannel, Message>(filter, this);
            channelManager.Acceptor = new InputChannelAcceptor(channelManager);
            return channelManager;
        }

        protected override void Dispatch(IChannelListener listener)
        {
            ((SingletonChannelListener<IInputChannel, InputChannel, Message>) listener).Dispatch();
        }

        protected override void EndpointNotFound(Message message)
        {
            if (base.DemuxFailureHandler != null)
            {
                base.DemuxFailureHandler.HandleDemuxFailure(message);
            }
            this.AbortItem(message);
        }

        protected override Message EndReceive(IAsyncResult result) => 
            base.InnerChannel.EndReceive(result);

        protected override void EnqueueAndDispatch(IChannelListener listener, Exception exception, ItemDequeuedCallback dequeuedCallback, bool canDispatchOnThisThread)
        {
            ((SingletonChannelListener<IInputChannel, InputChannel, Message>) listener).EnqueueAndDispatch(exception, dequeuedCallback, canDispatchOnThisThread);
        }

        protected override void EnqueueAndDispatch(IChannelListener listener, Message message, ItemDequeuedCallback dequeuedCallback, bool canDispatchOnThisThread)
        {
            ((SingletonChannelListener<IInputChannel, InputChannel, Message>) listener).EnqueueAndDispatch(message, dequeuedCallback, canDispatchOnThisThread);
        }

        protected override Message GetMessage(Message message) => 
            message;
    }
}

