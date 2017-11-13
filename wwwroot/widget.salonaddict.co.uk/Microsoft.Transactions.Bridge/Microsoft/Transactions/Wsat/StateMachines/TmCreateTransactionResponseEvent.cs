namespace Microsoft.Transactions.Wsat.StateMachines
{
    using Microsoft.Transactions.Bridge;
    using Microsoft.Transactions.Wsat.Messaging;
    using Microsoft.Transactions.Wsat.Protocol;
    using System;

    internal class TmCreateTransactionResponseEvent : CompletionStatusEvent
    {
        private MsgCreateTransactionEvent e;

        public TmCreateTransactionResponseEvent(CompletionEnlistment completion, Status status, MsgCreateTransactionEvent e) : base(completion, status)
        {
            this.e = e;
        }

        public override void Execute(StateMachine stateMachine)
        {
            if (DebugTrace.Info)
            {
                base.state.DebugTraceSink.OnEvent(this);
            }
            stateMachine.State.OnEvent(this);
        }

        public MsgCreateTransactionEvent SourceEvent =>
            this.e;
    }
}

