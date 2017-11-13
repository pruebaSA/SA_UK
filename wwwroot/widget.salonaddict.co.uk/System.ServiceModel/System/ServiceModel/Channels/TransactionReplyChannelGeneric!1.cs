namespace System.ServiceModel.Channels
{
    using System;
    using System.Runtime.InteropServices;
    using System.ServiceModel;
    using System.ServiceModel.Description;

    internal class TransactionReplyChannelGeneric<TChannel> : TransactionChannel<TChannel>, IReplyChannel, IChannel, ICommunicationObject where TChannel: class, IReplyChannel
    {
        public TransactionReplyChannelGeneric(ChannelManagerBase channelManager, TChannel innerChannel) : base(channelManager, innerChannel)
        {
        }

        public IAsyncResult BeginReceiveRequest(AsyncCallback callback, object state) => 
            this.BeginReceiveRequest(base.DefaultReceiveTimeout, callback, state);

        public IAsyncResult BeginReceiveRequest(TimeSpan timeout, AsyncCallback callback, object state) => 
            ReplyChannel.HelpBeginReceiveRequest(this, timeout, callback, state);

        public IAsyncResult BeginTryReceiveRequest(TimeSpan timeout, AsyncCallback callback, object state)
        {
            ReceiveTimeoutAsyncResult result = new ReceiveTimeoutAsyncResult(timeout, callback, state);
            result.InnerResult = base.InnerChannel.BeginTryReceiveRequest(timeout, result.InnerCallback, result.InnerState);
            return result;
        }

        public IAsyncResult BeginWaitForRequest(TimeSpan timeout, AsyncCallback callback, object state) => 
            base.InnerChannel.BeginWaitForRequest(timeout, callback, state);

        public RequestContext EndReceiveRequest(IAsyncResult result) => 
            ReplyChannel.HelpEndReceiveRequest(result);

        public bool EndTryReceiveRequest(IAsyncResult asyncResult, out RequestContext requestContext)
        {
            RequestContext context;
            if (asyncResult == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("asyncResult");
            }
            ReceiveTimeoutAsyncResult result = asyncResult as ReceiveTimeoutAsyncResult;
            if (result == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentException(System.ServiceModel.SR.GetString("AsyncEndCalledWithAnIAsyncResult")));
            }
            if (base.InnerChannel.EndTryReceiveRequest(result.InnerResult, out context))
            {
                requestContext = this.FinishReceiveRequest(context, result.TimeoutHelper.RemainingTime());
                return true;
            }
            requestContext = null;
            return false;
        }

        public bool EndWaitForRequest(IAsyncResult result) => 
            base.InnerChannel.EndWaitForRequest(result);

        private RequestContext FinishReceiveRequest(RequestContext innerContext, TimeSpan timeout)
        {
            if (innerContext == null)
            {
                return null;
            }
            try
            {
                this.ReadTransactionDataFromMessage(innerContext.RequestMessage, MessageDirection.Input);
            }
            catch (FaultException exception)
            {
                string action = exception.Action ?? innerContext.RequestMessage.Version.Addressing.DefaultFaultAction;
                Message message = Message.CreateMessage(innerContext.RequestMessage.Version, exception.CreateMessageFault(), action);
                try
                {
                    innerContext.Reply(message, timeout);
                }
                finally
                {
                    message.Close();
                }
                throw;
            }
            return new TransactionRequestContext(this, this, innerContext, this.DefaultCloseTimeout, base.DefaultSendTimeout);
        }

        public RequestContext ReceiveRequest() => 
            this.ReceiveRequest(base.DefaultReceiveTimeout);

        public RequestContext ReceiveRequest(TimeSpan timeout) => 
            ReplyChannel.HelpReceiveRequest(this, timeout);

        public bool TryReceiveRequest(TimeSpan timeout, out RequestContext requestContext)
        {
            RequestContext context;
            TimeoutHelper helper = new TimeoutHelper(timeout);
            if (base.InnerChannel.TryReceiveRequest(helper.RemainingTime(), out context))
            {
                requestContext = this.FinishReceiveRequest(context, helper.RemainingTime());
                return true;
            }
            requestContext = null;
            return false;
        }

        public bool WaitForRequest(TimeSpan timeout) => 
            base.InnerChannel.WaitForRequest(timeout);

        public EndpointAddress LocalAddress =>
            base.InnerChannel.LocalAddress;
    }
}

