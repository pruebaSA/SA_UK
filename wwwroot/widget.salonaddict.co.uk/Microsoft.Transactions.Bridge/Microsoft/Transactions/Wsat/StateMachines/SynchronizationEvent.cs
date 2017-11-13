namespace Microsoft.Transactions.Wsat.StateMachines
{
    using Microsoft.Transactions.Wsat.Protocol;
    using System;

    internal abstract class SynchronizationEvent
    {
        protected TransactionEnlistment enlistment;
        protected ProtocolState state;

        protected SynchronizationEvent(TransactionEnlistment enlistment)
        {
            this.enlistment = enlistment;
            this.state = enlistment.State;
        }

        public abstract void Execute(Microsoft.Transactions.Wsat.StateMachines.StateMachine stateMachine);
        public override string ToString() => 
            base.GetType().Name;

        public TransactionEnlistment Enlistment =>
            this.enlistment;

        public Microsoft.Transactions.Wsat.StateMachines.StateMachine StateMachine =>
            this.enlistment.StateMachine;
    }
}

