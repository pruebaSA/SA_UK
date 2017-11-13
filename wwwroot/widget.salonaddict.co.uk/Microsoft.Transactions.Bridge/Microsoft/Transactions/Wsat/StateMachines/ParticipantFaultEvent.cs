namespace Microsoft.Transactions.Wsat.StateMachines
{
    using Microsoft.Transactions.Wsat.Protocol;
    using System;
    using System.ServiceModel.Channels;

    internal abstract class ParticipantFaultEvent : ParticipantEvent
    {
        protected MessageFault fault;

        protected ParticipantFaultEvent(ParticipantEnlistment participant, MessageFault fault) : base(participant)
        {
            this.fault = fault;
        }

        public MessageFault Fault =>
            this.fault;
    }
}

