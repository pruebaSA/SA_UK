namespace Microsoft.Transactions.Wsat.StateMachines
{
    using Microsoft.Transactions.Wsat.Protocol;
    using System;

    internal abstract class ParticipantEvent : SynchronizationEvent
    {
        protected ParticipantEnlistment participant;

        protected ParticipantEvent(ParticipantEnlistment participant) : base(participant)
        {
            this.participant = participant;
        }

        public ParticipantEnlistment Participant =>
            this.participant;
    }
}

