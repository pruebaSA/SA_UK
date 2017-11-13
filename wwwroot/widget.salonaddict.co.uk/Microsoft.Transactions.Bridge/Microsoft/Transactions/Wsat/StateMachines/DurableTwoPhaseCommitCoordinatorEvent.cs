namespace Microsoft.Transactions.Wsat.StateMachines
{
    using Microsoft.Transactions.Wsat.Messaging;
    using Microsoft.Transactions.Wsat.Protocol;
    using System;
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using System.Xml;

    internal abstract class DurableTwoPhaseCommitCoordinatorEvent : CoordinatorEvent
    {
        private EndpointAddress faultTo;
        private UniqueId messageID;
        private EndpointAddress replyTo;

        public DurableTwoPhaseCommitCoordinatorEvent(CoordinatorEnlistment coordinator) : base(coordinator)
        {
            MessageHeaders incomingMessageHeaders = OperationContext.Current.IncomingMessageHeaders;
            this.faultTo = Library.GetFaultToHeader(incomingMessageHeaders, base.state.ProtocolVersion);
            this.replyTo = Library.GetReplyToHeader(incomingMessageHeaders);
            this.messageID = incomingMessageHeaders.MessageId;
        }

        public EndpointAddress FaultTo =>
            this.faultTo;

        public UniqueId MessageId =>
            this.messageID;

        public EndpointAddress ReplyTo =>
            this.replyTo;
    }
}

