namespace System.ServiceModel.Channels
{
    using System;

    internal class SendFaultHelper : TypedFaultHelper<Message>
    {
        public SendFaultHelper(TimeSpan defaultSendTimeout, TimeSpan defaultCloseTimeout) : base(defaultSendTimeout, defaultCloseTimeout)
        {
        }

        protected override void AbortState(Message message)
        {
            message.Close();
        }

        protected override IAsyncResult BeginSendFault(IReliableChannelBinder binder, Message message, TimeSpan timeout, AsyncCallback callback, object state) => 
            binder.BeginSend(message, timeout, callback, state);

        protected override void EndSendFault(IReliableChannelBinder binder, Message message, IAsyncResult result)
        {
            binder.EndSend(result);
            message.Close();
        }

        protected override Message GetState(RequestContext requestContext, Message faultMessage) => 
            faultMessage;
    }
}

