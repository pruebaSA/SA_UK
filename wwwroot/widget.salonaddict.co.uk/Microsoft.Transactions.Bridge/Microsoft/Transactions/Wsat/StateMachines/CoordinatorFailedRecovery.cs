namespace Microsoft.Transactions.Wsat.StateMachines
{
    using Microsoft.Transactions.Bridge;
    using Microsoft.Transactions.Wsat.Protocol;
    using System;

    internal class CoordinatorFailedRecovery : DecidedState
    {
        public CoordinatorFailedRecovery(ProtocolState state) : base(state)
        {
        }

        public override void OnEvent(TmCoordinatorForgetEvent e)
        {
            CoordinatorEnlistment coordinator = e.Coordinator;
            coordinator.SetCallback(e.Callback, e.CallbackState);
            base.state.TransactionManagerSend.ForgetResponse(coordinator, Status.Success);
            e.StateMachine.ChangeState(base.state.States.CoordinatorForgotten);
        }

        public override void OnEvent(TmReplayEvent e)
        {
            CoordinatorEnlistment coordinator = e.Coordinator;
            coordinator.SetCallback(e.Callback, e.CallbackState);
            base.state.TransactionManagerSend.Replayed(coordinator);
        }
    }
}

