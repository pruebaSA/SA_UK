namespace Microsoft.Transactions.Wsat.StateMachines
{
    using Microsoft.Transactions.Wsat.Messaging;
    using Microsoft.Transactions.Wsat.Protocol;
    using System;

    internal class TimerCoordinatorEvent : CoordinatorEvent
    {
        private TimerProfile profile;

        public TimerCoordinatorEvent(CoordinatorEnlistment coordinator, TimerProfile profile) : base(coordinator)
        {
            this.profile = profile;
        }

        public override void Execute(StateMachine stateMachine)
        {
            if (DebugTrace.Info)
            {
                base.state.DebugTraceSink.OnEvent(this);
            }
            base.coordinator.StateMachine.State.OnEvent(this);
        }
    }
}

