namespace Microsoft.Transactions.Wsat.StateMachines
{
    using Microsoft.Transactions.Wsat.Protocol;
    using System;

    internal class VolatilePhaseZeroUnregistered : ActiveState
    {
        public VolatilePhaseZeroUnregistered(ProtocolState state) : base(state)
        {
        }

        public override void OnEvent(MsgReadOnlyEvent e)
        {
        }

        public override void OnEvent(TmPrePrepareEvent e)
        {
            ParticipantEnlistment participant = e.Participant;
            participant.SetCallback(e.Callback, e.CallbackState);
            base.state.TransactionManagerSend.PrePrepared(participant);
            e.StateMachine.ChangeState(base.state.States.VolatilePhaseOneUnregistered);
        }

        public override void OnEvent(TmRollbackEvent e)
        {
            ParticipantEnlistment participant = e.Participant;
            participant.SetCallback(e.Callback, e.CallbackState);
            base.state.TransactionManagerSend.Aborted(participant);
            e.StateMachine.ChangeState(base.state.States.VolatileAborted);
        }
    }
}

