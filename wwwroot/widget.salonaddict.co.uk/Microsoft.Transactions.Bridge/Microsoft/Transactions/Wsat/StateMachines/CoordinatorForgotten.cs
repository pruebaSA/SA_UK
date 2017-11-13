﻿namespace Microsoft.Transactions.Wsat.StateMachines
{
    using Microsoft.Transactions.Wsat.Protocol;
    using System;

    internal class CoordinatorForgotten : TerminalState
    {
        public CoordinatorForgotten(ProtocolState state) : base(state)
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

        public override void OnEvent(MsgDurableCommitEvent e)
        {
        }

        public override void OnEvent(MsgDurablePrepareEvent e)
        {
        }

        public override void OnEvent(MsgDurableRollbackEvent e)
        {
        }

        public override void OnEvent(MsgVolatilePrepareEvent e)
        {
        }

        public override void OnEvent(MsgVolatileRollbackEvent e)
        {
        }

        public override void OnEvent(TimerCoordinatorEvent e)
        {
        }
    }
}

