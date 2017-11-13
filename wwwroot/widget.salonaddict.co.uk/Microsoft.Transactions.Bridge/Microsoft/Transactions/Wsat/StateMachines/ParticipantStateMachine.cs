﻿namespace Microsoft.Transactions.Wsat.StateMachines
{
    using Microsoft.Transactions.Wsat.Protocol;
    using System;

    internal abstract class ParticipantStateMachine : StateMachine
    {
        private ParticipantEnlistment participant;

        public ParticipantStateMachine(ParticipantEnlistment participant) : base(participant)
        {
            this.participant = participant;
        }

        protected override void OnTimer(TimerProfile profile)
        {
            base.OnTimer(profile);
            base.synchronization.Enqueue(new TimerParticipantEvent(this.participant, profile));
        }
    }
}

