namespace Microsoft.Transactions.Wsat.StateMachines
{
    using Microsoft.Transactions.Wsat.Protocol;
    using System;

    internal class CoordinatorStateMachine : StateMachine
    {
        private CoordinatorEnlistment coordinator;

        public CoordinatorStateMachine(CoordinatorEnlistment coordinator) : base(coordinator)
        {
            this.coordinator = coordinator;
        }

        protected override void OnTimer(TimerProfile profile)
        {
            base.OnTimer(profile);
            base.synchronization.Enqueue(new TimerCoordinatorEvent(this.coordinator, profile));
        }

        public override State AbortedState =>
            base.state.States.CoordinatorAborted;
    }
}

