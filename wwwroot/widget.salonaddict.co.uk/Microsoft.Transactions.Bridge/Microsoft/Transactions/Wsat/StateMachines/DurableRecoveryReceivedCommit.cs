namespace Microsoft.Transactions.Wsat.StateMachines
{
    using Microsoft.Transactions.Wsat.Protocol;
    using System;

    internal class DurableRecoveryReceivedCommit : DecidedState
    {
        public DurableRecoveryReceivedCommit(ProtocolState state) : base(state)
        {
        }

        public override void Enter(StateMachine stateMachine)
        {
            ParticipantEnlistment participant = (ParticipantEnlistment) stateMachine.Enlistment;
            participant.CreateCoordinatorService();
            base.state.TwoPhaseCommitCoordinator.SendCommit(participant);
            stateMachine.ChangeState(base.state.States.DurableCommitting);
        }
    }
}

