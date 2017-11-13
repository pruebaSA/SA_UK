namespace Microsoft.Transactions.Wsat.StateMachines
{
    using Microsoft.Transactions.Wsat.Protocol;
    using System;

    internal class SubordinateStateMachine : StateMachine
    {
        public SubordinateStateMachine(ParticipantEnlistment participant) : base(participant)
        {
        }

        public override State AbortedState =>
            base.state.States.SubordinateFinished;
    }
}

