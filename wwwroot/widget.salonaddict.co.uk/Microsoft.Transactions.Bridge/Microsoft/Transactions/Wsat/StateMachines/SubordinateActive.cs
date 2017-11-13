namespace Microsoft.Transactions.Wsat.StateMachines
{
    using Microsoft.Transactions.Wsat.Protocol;
    using System;

    internal class SubordinateActive : InactiveState
    {
        public SubordinateActive(ProtocolState state) : base(state)
        {
        }

        public override void OnEvent(TmPrepareEvent e)
        {
            ParticipantEnlistment participant = e.Participant;
            participant.SetCallback(e.Callback, e.CallbackState);
            base.state.TransactionManagerSend.ReadOnly(participant);
            participant.StateMachine.ChangeState(base.state.States.SubordinateFinished);
        }

        public override void OnEvent(TmRollbackEvent e)
        {
            ParticipantEnlistment participant = e.Participant;
            participant.SetCallback(e.Callback, e.CallbackState);
            base.state.TransactionManagerSend.Aborted(participant);
            participant.StateMachine.ChangeState(base.state.States.SubordinateFinished);
        }
    }
}

