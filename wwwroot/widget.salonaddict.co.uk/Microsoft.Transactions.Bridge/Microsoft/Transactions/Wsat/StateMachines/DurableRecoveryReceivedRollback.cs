namespace Microsoft.Transactions.Wsat.StateMachines
{
    using Microsoft.Transactions.Wsat.Protocol;
    using System;

    internal class DurableRecoveryReceivedRollback : DecidedState
    {
        public DurableRecoveryReceivedRollback(ProtocolState state) : base(state)
        {
        }

        public override void Enter(StateMachine stateMachine)
        {
            ParticipantEnlistment participant = (ParticipantEnlistment) stateMachine.Enlistment;
            participant.CreateCoordinatorService();
            base.state.TwoPhaseCommitCoordinator.SendRollback(participant);
            base.state.TransactionManagerSend.Aborted(participant);
            stateMachine.ChangeState(base.state.States.DurableAborted);
        }
    }
}

