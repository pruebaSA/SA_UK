namespace Microsoft.Transactions.Wsat.StateMachines
{
    using Microsoft.Transactions.Wsat.Protocol;
    using System;

    internal class VolatilePhaseOneUnregistered : ActiveState
    {
        public VolatilePhaseOneUnregistered(ProtocolState state) : base(state)
        {
        }

        public override void OnEvent(MsgReadOnlyEvent e)
        {
        }

        public override void OnEvent(TimerParticipantEvent e)
        {
        }

        public override void OnEvent(TmPrepareEvent e)
        {
            ParticipantEnlistment participant = e.Participant;
            participant.SetCallback(e.Callback, e.CallbackState);
            base.state.TransactionManagerSend.ReadOnly(participant);
            e.StateMachine.ChangeState(base.state.States.VolatileInDoubt);
        }

        public override void OnEvent(TmRollbackEvent e)
        {
            ParticipantEnlistment participant = e.Participant;
            participant.SetCallback(e.Callback, e.CallbackState);
            base.state.TransactionManagerSend.Aborted(participant);
            e.StateMachine.ChangeState(base.state.States.VolatileAborted);
        }

        public override void OnEvent(TmSinglePhaseCommitEvent e)
        {
            ParticipantEnlistment participant = e.Participant;
            participant.SetCallback(e.Callback, e.CallbackState);
            base.state.TransactionManagerSend.Committed(participant);
            e.StateMachine.ChangeState(base.state.States.VolatileCommitted);
        }
    }
}

