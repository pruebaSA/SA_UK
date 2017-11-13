namespace Microsoft.Transactions.Wsat.StateMachines
{
    using Microsoft.Transactions.Wsat.Protocol;
    using System;

    internal class DurableRecoveryAwaitingRollback : DecidedState
    {
        public DurableRecoveryAwaitingRollback(ProtocolState state) : base(state)
        {
        }

        public override void OnEvent(MsgAbortedEvent e)
        {
            e.StateMachine.ChangeState(base.state.States.DurableAborted);
        }

        public override void OnEvent(MsgPreparedEvent e)
        {
        }

        public override void OnEvent(MsgReplayEvent e)
        {
            ProtocolVersionHelper.AssertProtocolVersion10(base.state.ProtocolVersion, typeof(DurableRecoveryAwaitingRollback), "OnEvent(replay)");
        }

        public override void OnEvent(TmRollbackEvent e)
        {
            e.Participant.SetCallback(e.Callback, e.CallbackState);
            e.StateMachine.ChangeState(base.state.States.DurableRecoveryReceivedRollback);
        }
    }
}

