namespace Microsoft.Transactions.Bridge
{
    using System;

    internal abstract class TransactionManagerPropagationService
    {
        protected TransactionManagerPropagationService()
        {
        }

        public abstract void CreateSubordinateEnlistment(Enlistment enlistment, TransactionManagerCallback callback, object state);
        public abstract void CreateSuperiorEnlistment(Enlistment enlistment, EnlistmentOptions enlistmentOptions, TransactionManagerCallback callback, object state);
        public abstract void CreateTransaction(Enlistment enlistment, EnlistmentOptions enlistmentOptions, TransactionManagerCallback callback, object state);
    }
}

