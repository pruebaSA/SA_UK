namespace Microsoft.Transactions.Wsat.Messaging
{
    using System;
    using System.ServiceModel;

    internal class SendMessageFailureAsyncResult : AsyncResult
    {
        public SendMessageFailureAsyncResult(Exception e, AsyncCallback callback, object state) : base(callback, state)
        {
            base.Complete(true, e);
        }

        public void End()
        {
            AsyncResult.End<SendMessageFailureAsyncResult>(this);
        }
    }
}

