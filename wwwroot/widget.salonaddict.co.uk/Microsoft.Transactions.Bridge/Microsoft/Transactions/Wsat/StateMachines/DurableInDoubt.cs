﻿namespace Microsoft.Transactions.Wsat.StateMachines
{
    using Microsoft.Transactions.Wsat.Protocol;
    using System;

    internal class DurableInDoubt : TerminalState
    {
        public DurableInDoubt(ProtocolState state) : base(state)
        {
        }

        public override void Enter(StateMachine stateMachine)
        {
            base.Enter(stateMachine);
            if (ParticipantStateMachineFinishedRecord.ShouldTrace)
            {
                ParticipantStateMachineFinishedRecord.Trace(stateMachine.Enlistment.EnlistmentId, stateMachine.Enlistment.Enlistment.RemoteTransactionId, TransactionOutcome.InDoubt);
            }
        }

        public override void OnEvent(MsgAbortedEvent e)
        {
        }

        public override void OnEvent(MsgCommittedEvent e)
        {
        }

        public override void OnEvent(MsgReadOnlyEvent e)
        {
        }

        public override void OnEvent(TimerParticipantEvent e)
        {
        }

        public override void OnEvent(TmRollbackEvent e)
        {
            ParticipantEnlistment participant = e.Participant;
            participant.SetCallback(e.Callback, e.CallbackState);
            base.state.TransactionManagerSend.Aborted(participant);
        }
    }
}

