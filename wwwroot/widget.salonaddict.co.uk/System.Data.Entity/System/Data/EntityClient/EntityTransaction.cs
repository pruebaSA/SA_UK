namespace System.Data.EntityClient
{
    using System;
    using System.Data;
    using System.Data.Common;

    public sealed class EntityTransaction : DbTransaction
    {
        private EntityConnection _connection;
        private DbTransaction _storeTransaction;

        internal EntityTransaction(EntityConnection connection, DbTransaction storeTransaction)
        {
            this._connection = connection;
            this._storeTransaction = storeTransaction;
        }

        private void ClearCurrentTransaction()
        {
            if (this._connection.CurrentTransaction == this)
            {
                this._connection.ClearCurrentTransaction();
            }
        }

        public override void Commit()
        {
            try
            {
                this._storeTransaction.Commit();
            }
            catch (Exception exception)
            {
                if (EntityUtil.IsCatchableExceptionType(exception))
                {
                    throw EntityUtil.Provider("Commit", exception);
                }
                throw;
            }
            this.ClearCurrentTransaction();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.ClearCurrentTransaction();
                this._storeTransaction.Dispose();
            }
            base.Dispose(disposing);
        }

        public override void Rollback()
        {
            try
            {
                this._storeTransaction.Rollback();
            }
            catch (Exception exception)
            {
                if (EntityUtil.IsCatchableExceptionType(exception))
                {
                    throw EntityUtil.Provider("Rollback", exception);
                }
                throw;
            }
            this.ClearCurrentTransaction();
        }

        public EntityConnection Connection
        {
            get
            {
                if (this._storeTransaction.Connection == null)
                {
                    return null;
                }
                return this._connection;
            }
        }

        protected override System.Data.Common.DbConnection DbConnection
        {
            get
            {
                if (this._storeTransaction.Connection == null)
                {
                    return null;
                }
                return this._connection;
            }
        }

        public override System.Data.IsolationLevel IsolationLevel =>
            this._storeTransaction.IsolationLevel;

        internal DbTransaction StoreTransaction =>
            this._storeTransaction;
    }
}

