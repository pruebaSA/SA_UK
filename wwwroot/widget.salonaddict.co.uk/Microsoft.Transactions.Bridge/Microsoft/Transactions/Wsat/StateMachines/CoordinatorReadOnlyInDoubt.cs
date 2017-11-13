namespace Microsoft.Transactions.Wsat.StateMachines
{
    using Microsoft.Transactions.Wsat.Protocol;
    using System;

    internal class CoordinatorReadOnlyInDoubt : TerminalState
    {
        public CoordinatorReadOnlyInDoubt(ProtocolState state) : base(state)
        {
        }

        public override void Enter(StateMachine stateMachine)
        {
            base.Enter(stateMachine);
            if (CoordinatorStateMachineFinishedRecord.ShouldTrace)
            {
                CoordinatorStateMachineFinishedRecord.Trace(stateMachine.Enlistment.EnlistmentId, stateMachine.Enlistment.Enlistment.RemoteTransactionId, TransactionOutcome.InDoubt);
            }
        }

        public override void OnEvent(MsgDurablePrepareEvent e)
        {
            base.state.TwoPhaseCommitParticipant.SendReadOnly(e.ReplyTo);
        }

        public override void OnEvent(MsgDurableRollbackEvent e)
        {
            base.state.TwoPhaseCommitParticipant.SendAborted(e.ReplyTo);
        }

        public override void OnEvent(MsgVolatilePrepareEvent e)
        {
        }
    }
}

