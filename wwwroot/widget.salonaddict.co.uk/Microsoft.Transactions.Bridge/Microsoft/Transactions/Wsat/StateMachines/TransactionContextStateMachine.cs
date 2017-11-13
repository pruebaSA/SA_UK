namespace Microsoft.Transactions.Wsat.StateMachines
{
    using Microsoft.Transactions.Wsat.Protocol;
    using System;

    internal class TransactionContextStateMachine : StateMachine
    {
        private TransactionContextManager contextManager;

        public TransactionContextStateMachine(TransactionContextManager contextManager) : base(contextManager)
        {
            this.contextManager = contextManager;
        }

        public override State AbortedState =>
            base.state.States.TransactionContextFinished;

        public TransactionContextManager ContextManager =>
            this.contextManager;
    }
}

