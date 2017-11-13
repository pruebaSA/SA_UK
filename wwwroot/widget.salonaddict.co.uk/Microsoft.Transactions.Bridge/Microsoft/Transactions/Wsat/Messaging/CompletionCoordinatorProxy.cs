namespace Microsoft.Transactions.Wsat.Messaging
{
    using System;
    using System.ServiceModel;
    using System.ServiceModel.Channels;

    internal class CompletionCoordinatorProxy : DatagramProxy
    {
        public CompletionCoordinatorProxy(CoordinationService coordination, EndpointAddress to, EndpointAddress from) : base(coordination, to, from)
        {
        }

        public IAsyncResult BeginSendCommit(AsyncCallback callback, object state)
        {
            Message message = new CommitMessage(base.messageVersion, base.protocolVersion);
            return base.BeginSendMessage(message, callback, state);
        }

        public IAsyncResult BeginSendRollback(AsyncCallback callback, object state)
        {
            Message message = new RollbackMessage(base.messageVersion, base.protocolVersion);
            return base.BeginSendMessage(message, callback, state);
        }
    }
}

