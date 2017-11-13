namespace Microsoft.Transactions.Wsat.Messaging
{
    using System;
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using System.Xml;

    internal class RequestAsyncResult : AsyncResult
    {
        private UniqueId messageID;
        private System.ServiceModel.Channels.MessageVersion messageVersion;
        private Message reply;

        public RequestAsyncResult(Message message, AsyncCallback callback, object state) : base(callback, state)
        {
            this.messageVersion = message.Version;
            this.messageID = message.Headers.MessageId;
        }

        public void End()
        {
            AsyncResult.End<Microsoft.Transactions.Wsat.Messaging.RequestAsyncResult>(this);
        }

        public void Finished(Exception exception)
        {
            base.Complete(false, exception);
        }

        public void Finished(Message reply)
        {
            this.reply = reply;
            base.Complete(false);
        }

        public UniqueId MessageId =>
            this.messageID;

        public System.ServiceModel.Channels.MessageVersion MessageVersion =>
            this.messageVersion;

        public Message Reply =>
            this.reply;
    }
}

