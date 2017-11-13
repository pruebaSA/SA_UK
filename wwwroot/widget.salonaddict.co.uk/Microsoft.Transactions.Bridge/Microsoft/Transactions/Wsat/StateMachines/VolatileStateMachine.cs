namespace Microsoft.Transactions.Wsat.StateMachines
{
    using Microsoft.Transactions.Wsat.Protocol;
    using System;

    internal class VolatileStateMachine : ParticipantStateMachine
    {
        public VolatileStateMachine(ParticipantEnlistment participant) : base(participant)
        {
        }

        public override State AbortedState =>
            base.state.States.VolatileAborted;
    }
}

