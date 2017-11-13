namespace Microsoft.Transactions.Wsat.StateMachines
{
    using Microsoft.Transactions.Bridge;
    using Microsoft.Transactions.Wsat.Protocol;
    using System;

    internal abstract class ParticipantStatusEvent : ParticipantEvent
    {
        protected Microsoft.Transactions.Bridge.Status status;

        protected ParticipantStatusEvent(ParticipantEnlistment participant, Microsoft.Transactions.Bridge.Status status) : base(participant)
        {
            this.status = status;
        }

        public Microsoft.Transactions.Bridge.Status Status =>
            this.status;
    }
}

