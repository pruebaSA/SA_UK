namespace Microsoft.Transactions.Wsat.Messaging
{
    using System;
    using System.ServiceModel;
    using System.ServiceModel.Channels;

    internal class TwoPhaseCommitParticipantProxy : DatagramProxy
    {
        public TwoPhaseCommitParticipantProxy(CoordinationService coordination, EndpointAddress to, EndpointAddress from) : base(coordination, to, from)
        {
        }

        public IAsyncResult BeginSendCommit(AsyncCallback callback, object state)
        {
            Message message = new CommitMessage(base.messageVersion, base.protocolVersion);
            return base.BeginSendMessage(message, callback, state);
        }

        public IAsyncResult BeginSendPrepare(AsyncCallback callback, object state)
        {
            Message message = new PrepareMessage(base.messageVersion, base.protocolVersion);
            return base.BeginSendMessage(message, callback, state);
        }

        public IAsyncResult BeginSendRollback(AsyncCallback callback, object state)
        {
            Message message = new RollbackMessage(base.messageVersion, base.protocolVersion);
            return base.BeginSendMessage(message, callback, state);
        }
    }
}

