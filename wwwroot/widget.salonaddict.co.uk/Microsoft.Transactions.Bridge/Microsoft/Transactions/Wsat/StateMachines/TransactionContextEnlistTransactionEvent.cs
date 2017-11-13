﻿namespace Microsoft.Transactions.Wsat.StateMachines
{
    using Microsoft.Transactions.Wsat.Messaging;
    using Microsoft.Transactions.Wsat.Protocol;
    using System;

    internal class TransactionContextEnlistTransactionEvent : TransactionContextEvent
    {
        private CreateCoordinationContext create;
        private RequestAsyncResult result;

        public TransactionContextEnlistTransactionEvent(TransactionContextManager contextManager, ref CreateCoordinationContext create, RequestAsyncResult result) : base(contextManager)
        {
            this.create = create;
            this.result = result;
        }

        public override void Execute(StateMachine stateMachine)
        {
            if (DebugTrace.Info)
            {
                base.state.DebugTraceSink.OnEvent(this);
            }
            stateMachine.State.OnEvent(this);
        }

        public CreateCoordinationContext Body =>
            this.create;

        public RequestAsyncResult Result =>
            this.result;
    }
}

