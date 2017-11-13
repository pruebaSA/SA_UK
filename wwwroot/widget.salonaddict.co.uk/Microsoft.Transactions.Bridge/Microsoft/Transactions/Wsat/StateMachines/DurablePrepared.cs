namespace Microsoft.Transactions.Wsat.StateMachines
{
    using Microsoft.Transactions.Wsat.Protocol;
    using System;

    internal class DurablePrepared : DecidedState
    {
        public DurablePrepared(ProtocolState state) : base(state)
        {
        }

        public override void OnEvent(MsgPreparedEvent e)
        {
        }

        public override void OnEvent(MsgReplayEvent e)
        {
            ProtocolVersionHelper.AssertProtocolVersion10(base.state.ProtocolVersion, typeof(DurablePrepared), "OnEvent(replay)");
        }

        public override void OnEvent(TimerParticipantEvent e)
        {
        }

        public override void OnEvent(TmCommitEvent e)
        {
            ParticipantEnlistment participant = e.Participant;
            base.state.TwoPhaseCommitCoordinator.SendCommit(participant);
            participant.SetCallback(e.Callback, e.CallbackState);
            e.StateMachine.ChangeState(base.state.States.DurableCommitting);
        }

        public override void OnEvent(TmRollbackEvent e)
        {
            ParticipantEnlistment participant = e.Participant;
            base.state.TwoPhaseCommitCoordinator.SendRollback(participant);
            participant.SetCallback(e.Callback, e.CallbackState);
            base.state.TransactionManagerSend.Aborted(participant);
            e.StateMachine.ChangeState(base.state.States.DurableAborted);
        }
    }
}

