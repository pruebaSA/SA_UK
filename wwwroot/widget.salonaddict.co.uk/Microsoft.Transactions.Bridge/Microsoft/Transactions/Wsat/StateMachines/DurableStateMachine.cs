namespace Microsoft.Transactions.Wsat.StateMachines
{
    using Microsoft.Transactions.Wsat.Protocol;
    using System;

    internal class DurableStateMachine : ParticipantStateMachine
    {
        public DurableStateMachine(ParticipantEnlistment participant) : base(participant)
        {
        }

        public override State AbortedState =>
            base.state.States.DurableAborted;
    }
}

