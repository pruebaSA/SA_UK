namespace Microsoft.Transactions.Wsat.StateMachines
{
    using Microsoft.Transactions.Wsat.Messaging;
    using Microsoft.Transactions.Wsat.Protocol;
    using System;

    internal class InternalEnlistSubordinateTransactionEvent : ParticipantEvent
    {
        private MsgEnlistTransactionEvent source;

        public InternalEnlistSubordinateTransactionEvent(ParticipantEnlistment participant, MsgEnlistTransactionEvent source) : base(participant)
        {
            this.source = source;
        }

        public override void Execute(StateMachine stateMachine)
        {
            if (DebugTrace.Info)
            {
                base.state.DebugTraceSink.OnEvent(this);
            }
            stateMachine.State.OnEvent(this);
        }

        public MsgEnlistTransactionEvent SourceEvent =>
            this.source;
    }
}

