namespace Microsoft.Transactions.Wsat.Messaging
{
    using Microsoft.Transactions.Wsat.Protocol;
    using System;
    using System.Runtime.InteropServices;
    using System.ServiceModel;
    using System.ServiceModel.Channels;

    internal class SupportingTokenDuplexChannel : SupportingTokenChannel<IDuplexChannel>, IDuplexChannel, IInputChannel, IOutputChannel, IChannel, ICommunicationObject
    {
        public SupportingTokenDuplexChannel(ChannelManagerBase manager, IDuplexChannel innerChannel, SupportingTokenSecurityTokenResolver tokenResolver, ProtocolVersion protocolVersion) : base(manager, innerChannel, tokenResolver, protocolVersion)
        {
        }

        public IAsyncResult BeginReceive(AsyncCallback callback, object state) => 
            base.innerChannel.BeginReceive(callback, state);

        public IAsyncResult BeginReceive(TimeSpan timeout, AsyncCallback callback, object state) => 
            base.innerChannel.BeginReceive(timeout, callback, state);

        public IAsyncResult BeginSend(Message message, AsyncCallback callback, object state) => 
            this.BeginSend(message, base.DefaultSendTimeout, callback, state);

        public IAsyncResult BeginSend(Message message, TimeSpan timeout, AsyncCallback asyncCallback, object state) => 
            base.innerChannel.BeginSend(message, timeout, asyncCallback, state);

        public IAsyncResult BeginTryReceive(TimeSpan timeout, AsyncCallback callback, object state) => 
            base.innerChannel.BeginTryReceive(timeout, callback, state);

        public IAsyncResult BeginWaitForMessage(TimeSpan timeout, AsyncCallback callback, object state) => 
            base.innerChannel.BeginWaitForMessage(timeout, callback, state);

        public Message EndReceive(IAsyncResult result)
        {
            Message message = base.innerChannel.EndReceive(result);
            base.OnReceive(message);
            return message;
        }

        public void EndSend(IAsyncResult result)
        {
            base.innerChannel.EndSend(result);
        }

        public bool EndTryReceive(IAsyncResult result, out Message message)
        {
            if (base.innerChannel.EndTryReceive(result, out message))
            {
                base.OnReceive(message);
                return true;
            }
            return false;
        }

        public bool EndWaitForMessage(IAsyncResult result) => 
            base.innerChannel.EndWaitForMessage(result);

        public Message Receive()
        {
            Message message = base.innerChannel.Receive();
            base.OnReceive(message);
            return message;
        }

        public Message Receive(TimeSpan timeout)
        {
            Message message = base.innerChannel.Receive(timeout);
            base.OnReceive(message);
            return message;
        }

        public void Send(Message message)
        {
            base.innerChannel.Send(message);
        }

        public void Send(Message message, TimeSpan timeout)
        {
            base.innerChannel.Send(message, timeout);
        }

        public bool TryReceive(TimeSpan timeout, out Message message)
        {
            if (base.innerChannel.TryReceive(timeout, out message))
            {
                base.OnReceive(message);
                return true;
            }
            return false;
        }

        protected override void TrySendFaultReply(Message faultMessage)
        {
            base.innerChannel.Send(faultMessage);
        }

        public bool WaitForMessage(TimeSpan timeout) => 
            base.innerChannel.WaitForMessage(timeout);

        public EndpointAddress LocalAddress =>
            base.innerChannel.LocalAddress;

        public EndpointAddress RemoteAddress =>
            base.innerChannel.RemoteAddress;

        public Uri Via =>
            base.innerChannel.Via;
    }
}

