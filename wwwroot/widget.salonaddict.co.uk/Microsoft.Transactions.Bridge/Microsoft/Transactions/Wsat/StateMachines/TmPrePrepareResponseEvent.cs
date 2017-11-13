namespace Microsoft.Transactions.Wsat.StateMachines
{
    using Microsoft.Transactions.Bridge;
    using Microsoft.Transactions.Wsat.Messaging;
    using Microsoft.Transactions.Wsat.Protocol;
    using System;

    internal class TmPrePrepareResponseEvent : VolatileCoordinatorStatusEvent
    {
        public TmPrePrepareResponseEvent(VolatileCoordinatorEnlistment coordinator, Status status) : base(coordinator, status)
        {
        }

        public override void Execute(StateMachine stateMachine)
        {
            if (DebugTrace.Info)
            {
                base.state.DebugTraceSink.OnEvent(this);
            }
            stateMachine.State.OnEvent(this);
        }
    }
}

