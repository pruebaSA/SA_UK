namespace Microsoft.Transactions.Wsat.StateMachines
{
    using Microsoft.Transactions.Bridge;
    using Microsoft.Transactions.Wsat.Protocol;
    using System;

    internal class CoordinatorRecovering : DecidedState
    {
        public CoordinatorRecovering(ProtocolState state) : base(state)
        {
        }

        public override void OnEvent(TmReplayEvent e)
        {
            if (!base.state.Recovering)
            {
                DiagnosticUtility.FailFast("Replay events should only be delivered during recovery");
            }
            CoordinatorEnlistment coordinator = e.Coordinator;
            base.state.EnqueueRecoveryReplay(e);
            coordinator.SetCallback(e.Callback, e.CallbackState);
            base.state.TransactionManagerSend.Replayed(coordinator);
            e.StateMachine.ChangeState(base.state.States.CoordinatorAwaitingEndOfRecovery);
        }
    }
}

