namespace Microsoft.Transactions.Wsat.StateMachines
{
    using Microsoft.Transactions.Wsat.Protocol;
    using System;

    internal class CompletionCommitted : TerminalState
    {
        public CompletionCommitted(ProtocolState state) : base(state)
        {
        }

        public override void Enter(StateMachine stateMachine)
        {
            base.Enter(stateMachine);
            if (ParticipantStateMachineFinishedRecord.ShouldTrace)
            {
                ParticipantStateMachineFinishedRecord.Trace(stateMachine.Enlistment.EnlistmentId, stateMachine.Enlistment.Enlistment.RemoteTransactionId, TransactionOutcome.Committed);
            }
        }

        public override void OnEvent(MsgCompletionCommitEvent e)
        {
            base.state.CompletionCoordinator.SendCommitted(e.ReplyTo);
        }
    }
}

