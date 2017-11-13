namespace Microsoft.Transactions.Wsat.Protocol
{
    using Microsoft.Transactions.Wsat.Messaging;
    using Microsoft.Transactions.Wsat.StateMachines;
    using System;
    using System.Collections.Generic;

    internal class TransactionContextManager : TransactionEnlistment
    {
        private Microsoft.Transactions.Wsat.Protocol.TransactionContext context;
        private Microsoft.Transactions.Wsat.Messaging.Fault fault;
        private string identifier;
        private Queue<TransactionContextEnlistTransactionEvent> requests;

        public TransactionContextManager(ProtocolState state, string identifier) : base(state)
        {
            this.identifier = identifier;
            this.requests = new Queue<TransactionContextEnlistTransactionEvent>();
            base.stateMachine = new TransactionContextStateMachine(this);
            base.stateMachine.ChangeState(state.States.TransactionContextInitializing);
        }

        public override void OnStateMachineComplete()
        {
            base.state.Lookup.RemoveTransactionContextManager(this);
        }

        public Microsoft.Transactions.Wsat.Messaging.Fault Fault
        {
            get
            {
                if (this.fault == null)
                {
                    return base.state.Faults.CannotCreateContext;
                }
                return this.fault;
            }
            set
            {
                this.fault = value;
            }
        }

        public string Identifier =>
            this.identifier;

        public Queue<TransactionContextEnlistTransactionEvent> Requests =>
            this.requests;

        public Microsoft.Transactions.Wsat.Protocol.TransactionContext TransactionContext
        {
            get => 
                this.context;
            set
            {
                this.context = value;
            }
        }
    }
}

