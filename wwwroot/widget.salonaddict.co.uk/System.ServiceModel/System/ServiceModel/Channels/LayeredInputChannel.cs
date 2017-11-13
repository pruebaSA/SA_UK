namespace System.ServiceModel.Channels
{
    using System;
    using System.Runtime.InteropServices;
    using System.ServiceModel;

    internal class LayeredInputChannel : LayeredChannel<IInputChannel>, IInputChannel, IChannel, ICommunicationObject
    {
        public LayeredInputChannel(ChannelManagerBase channelManager, IInputChannel innerChannel) : base(channelManager, innerChannel)
        {
        }

        public IAsyncResult BeginReceive(AsyncCallback callback, object state) => 
            base.InnerChannel.BeginReceive(callback, state);

        public IAsyncResult BeginReceive(TimeSpan timeout, AsyncCallback callback, object state) => 
            base.InnerChannel.BeginReceive(timeout, callback, state);

        public IAsyncResult BeginTryReceive(TimeSpan timeout, AsyncCallback callback, object state) => 
            base.InnerChannel.BeginTryReceive(timeout, callback, state);

        public IAsyncResult BeginWaitForMessage(TimeSpan timeout, AsyncCallback callback, object state) => 
            base.InnerChannel.BeginWaitForMessage(timeout, callback, state);

        public Message EndReceive(IAsyncResult result) => 
            base.InnerChannel.EndReceive(result);

        public bool EndTryReceive(IAsyncResult result, out Message message) => 
            base.InnerChannel.EndTryReceive(result, out message);

        public bool EndWaitForMessage(IAsyncResult result) => 
            base.InnerChannel.EndWaitForMessage(result);

        public Message Receive() => 
            base.InnerChannel.Receive();

        public Message Receive(TimeSpan timeout) => 
            base.InnerChannel.Receive(timeout);

        public bool TryReceive(TimeSpan timeout, out Message message) => 
            base.InnerChannel.TryReceive(timeout, out message);

        public bool WaitForMessage(TimeSpan timeout) => 
            base.InnerChannel.WaitForMessage(timeout);

        public virtual EndpointAddress LocalAddress =>
            base.InnerChannel.LocalAddress;
    }
}

