namespace Microsoft.Transactions.Bridge
{
    using System;

    internal abstract class TransactionManagerCoordinatorService
    {
        protected TransactionManagerCoordinatorService()
        {
        }

        public abstract void Commit(Enlistment enlistment, TransactionManagerCallback callback, object state);
        public abstract void Prepare(Enlistment enlistment, TransactionManagerCallback callback, object state);
        public abstract void PrePrepare(Enlistment enlistment, TransactionManagerCallback callback, object state);
        public abstract void Rollback(Enlistment enlistment, TransactionManagerCallback callback, object state);
        public abstract void SinglePhaseCommit(Enlistment enlistment, TransactionManagerCallback callback, object state);
    }
}

