namespace Microsoft.Transactions.Wsat.StateMachines
{
    using Microsoft.Transactions.Wsat.Protocol;
    using System;

    internal abstract class TransactionContextEvent : SynchronizationEvent
    {
        private TransactionContextManager contextManager;

        protected TransactionContextEvent(TransactionContextManager contextManager) : base(contextManager)
        {
            this.contextManager = contextManager;
        }

        public TransactionContextManager ContextManager =>
            this.contextManager;
    }
}

