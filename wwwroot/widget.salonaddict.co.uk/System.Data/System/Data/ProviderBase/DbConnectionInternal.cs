﻿namespace System.Data.ProviderBase
{
    using System;
    using System.Data;
    using System.Data.Common;
    using System.Data.SqlClient;
    using System.Runtime.ConstrainedExecution;
    using System.Security.Permissions;
    using System.Threading;
    using System.Transactions;

    internal abstract class DbConnectionInternal
    {
        private readonly bool _allowSetConnectionString;
        private bool _cannotBePooled;
        private bool _connectionIsDoomed;
        private DbConnectionPool _connectionPool;
        private DateTime _createTime;
        private Transaction _enlistedTransaction;
        private readonly bool _hidePassword;
        private bool _isInStasis;
        private DbConnectionInternal _nextPooledObject;
        internal readonly int _objectID;
        private static int _objectTypeCount;
        private readonly WeakReference _owningObject;
        private DbConnectionPoolCounters _performanceCounters;
        private int _pooledCount;
        private DbReferenceCollection _referenceCollection;
        private readonly ConnectionState _state;
        internal static readonly StateChangeEventArgs StateChangeClosed = new StateChangeEventArgs(ConnectionState.Open, ConnectionState.Closed);
        internal static readonly StateChangeEventArgs StateChangeOpen = new StateChangeEventArgs(ConnectionState.Closed, ConnectionState.Open);

        protected DbConnectionInternal() : this(ConnectionState.Open, true, false)
        {
        }

        internal DbConnectionInternal(ConnectionState state, bool hidePassword, bool allowSetConnectionString)
        {
            this._objectID = Interlocked.Increment(ref _objectTypeCount);
            this._owningObject = new WeakReference(null, false);
            this._allowSetConnectionString = allowSetConnectionString;
            this._hidePassword = hidePassword;
            this._state = state;
        }

        protected abstract void Activate(Transaction transaction);
        internal void ActivateConnection(Transaction transaction)
        {
            Bid.PoolerTrace("<prov.DbConnectionInternal.ActivateConnection|RES|INFO|CPOOL> %d#, Activating\n", this.ObjectID);
            this.Activate(transaction);
            this.PerformanceCounters.NumberOfActiveConnections.Increment();
        }

        internal void AddWeakReference(object value, int tag)
        {
            if (this._referenceCollection == null)
            {
                this._referenceCollection = this.CreateReferenceCollection();
                if (this._referenceCollection == null)
                {
                    throw ADP.InternalError(ADP.InternalErrorCode.CreateReferenceCollectionReturnedNull);
                }
            }
            this._referenceCollection.Add(value, tag);
        }

        public abstract DbTransaction BeginTransaction(System.Data.IsolationLevel il);
        public virtual void ChangeDatabase(string value)
        {
            throw ADP.MethodNotImplemented("ChangeDatabase");
        }

        internal void CleanupConnectionOnTransactionCompletion(Transaction transaction)
        {
            this.DetachTransaction(transaction);
            DbConnectionPool pool = this.Pool;
            if (pool != null)
            {
                pool.TransactionEnded(transaction, this);
            }
        }

        protected virtual void CleanupTransactionOnCompletion(Transaction transaction)
        {
        }

        internal virtual void CloseConnection(DbConnection owningObject, DbConnectionFactory connectionFactory)
        {
            Bid.PoolerTrace("<prov.DbConnectionInternal.CloseConnection|RES|CPOOL> %d# Closing.\n", this.ObjectID);
            if (connectionFactory.SetInnerConnectionFrom(owningObject, DbConnectionOpenBusy.SingletonInstance, this))
            {
                try
                {
                    DbConnectionPool pool = this.Pool;
                    Transaction enlistedTransaction = this.EnlistedTransaction;
                    if ((null != enlistedTransaction) && (enlistedTransaction.TransactionInformation.Status != TransactionStatus.Active))
                    {
                        this.DetachTransaction(enlistedTransaction);
                    }
                    if (pool != null)
                    {
                        pool.PutObject(this, owningObject);
                    }
                    else
                    {
                        this.Deactivate();
                        this.PerformanceCounters.HardDisconnectsPerSecond.Increment();
                        this._owningObject.Target = null;
                        if (this.IsTransactionRoot)
                        {
                            this.SetInStasis();
                        }
                        else
                        {
                            this.PerformanceCounters.NumberOfNonPooledConnections.Decrement();
                            if (base.GetType() != typeof(SqlInternalConnectionSmi))
                            {
                                this.Dispose();
                            }
                        }
                    }
                }
                finally
                {
                    connectionFactory.SetInnerConnectionEvent(owningObject, DbConnectionClosedPreviouslyOpened.SingletonInstance);
                }
            }
        }

        protected virtual DbReferenceCollection CreateReferenceCollection()
        {
            throw ADP.InternalError(ADP.InternalErrorCode.AttemptingToConstructReferenceCollectionOnStaticObject);
        }

        protected abstract void Deactivate();
        internal void DeactivateConnection()
        {
            Bid.PoolerTrace("<prov.DbConnectionInternal.DeactivateConnection|RES|INFO|CPOOL> %d#, Deactivating\n", this.ObjectID);
            this.PerformanceCounters.NumberOfActiveConnections.Decrement();
            if ((!this._connectionIsDoomed && this.Pool.UseLoadBalancing) && ((DateTime.UtcNow.Ticks - this._createTime.Ticks) > this.Pool.LoadBalanceTimeout.Ticks))
            {
                this.DoNotPoolThisConnection();
            }
            this.Deactivate();
        }

        internal virtual void DelegatedTransactionEnded()
        {
            Bid.Trace("<prov.DbConnectionInternal.DelegatedTransactionEnded|RES|CPOOL> %d#, Delegated Transaction Completed.\n", this.ObjectID);
            if (1 == this._pooledCount)
            {
                this.TerminateStasis(true);
                this.Deactivate();
                DbConnectionPool pool = this.Pool;
                if (pool == null)
                {
                    throw ADP.InternalError(ADP.InternalErrorCode.PooledObjectWithoutPool);
                }
                pool.PutObjectFromTransactedPool(this);
            }
            else if ((-1 == this._pooledCount) && !this._owningObject.IsAlive)
            {
                this.TerminateStasis(false);
                this.Deactivate();
                this.PerformanceCounters.NumberOfNonPooledConnections.Decrement();
                this.Dispose();
            }
        }

        internal void DetachTransaction(Transaction transaction)
        {
            Bid.Trace("<prov.DbConnectionInternal.DetachTransaction|RES|CPOOL> %d#, Transaction Completed. (pooledCount=%d)\n", this.ObjectID, this._pooledCount);
            lock (this)
            {
                DbConnection owner = (DbConnection) this.Owner;
                if ((!this.RequireExplicitTransactionUnbind || (owner == null)) && ((this._enlistedTransaction != null) && transaction.Equals(this._enlistedTransaction)))
                {
                    this.EnlistedTransaction = null;
                    if (this.IsTxRootWaitingForTxEnd)
                    {
                        this.DelegatedTransactionEnded();
                    }
                }
            }
        }

        public virtual void Dispose()
        {
            this._connectionPool = null;
            this._performanceCounters = null;
            this._connectionIsDoomed = true;
            this._enlistedTransaction = null;
        }

        protected internal void DoNotPoolThisConnection()
        {
            this._cannotBePooled = true;
            Bid.PoolerTrace("<prov.DbConnectionInternal.DoNotPoolThisConnection|RES|INFO|CPOOL> %d#, Marking pooled object as non-poolable so it will be disposed\n", this.ObjectID);
        }

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        protected internal void DoomThisConnection()
        {
            this._connectionIsDoomed = true;
            Bid.PoolerTrace("<prov.DbConnectionInternal.DoomThisConnection|RES|INFO|CPOOL> %d#, Dooming\n", this.ObjectID);
        }

        public abstract void EnlistTransaction(Transaction transaction);
        protected internal virtual DataTable GetSchema(DbConnectionFactory factory, DbConnectionPoolGroup poolGroup, DbConnection outerConnection, string collectionName, string[] restrictions) => 
            factory.GetMetaDataFactory(poolGroup, this).GetSchema(outerConnection, collectionName, restrictions);

        internal void MakeNonPooledObject(object owningObject, DbConnectionPoolCounters performanceCounters)
        {
            this._connectionPool = null;
            this._performanceCounters = performanceCounters;
            this._owningObject.Target = owningObject;
            this._pooledCount = -1;
        }

        internal void MakePooledConnection(DbConnectionPool connectionPool)
        {
            this._createTime = DateTime.UtcNow;
            this._connectionPool = connectionPool;
            this._performanceCounters = connectionPool.PerformanceCounters;
        }

        internal void NotifyWeakReference(int message)
        {
            DbReferenceCollection referenceCollection = this.ReferenceCollection;
            if (referenceCollection != null)
            {
                referenceCollection.Notify(message);
            }
        }

        internal virtual void OpenConnection(DbConnection outerConnection, DbConnectionFactory connectionFactory)
        {
            throw ADP.ConnectionAlreadyOpen(this.State);
        }

        internal void PostPop(object newOwner)
        {
            if (this._owningObject.Target != null)
            {
                throw ADP.InternalError(ADP.InternalErrorCode.PooledObjectHasOwner);
            }
            this._owningObject.Target = newOwner;
            this._pooledCount--;
            if (Bid.IsOn(Bid.ApiGroup.Pooling))
            {
                Bid.PoolerTrace("<prov.DbConnectionInternal.PostPop|RES|CPOOL> %d#, Preparing to pop from pool,  owning connection %d#, pooledCount=%d\n", this.ObjectID, 0, this._pooledCount);
            }
            if (this.Pool != null)
            {
                if (this._pooledCount != 0)
                {
                    throw ADP.InternalError(ADP.InternalErrorCode.PooledObjectInPoolMoreThanOnce);
                }
            }
            else if (-1 != this._pooledCount)
            {
                throw ADP.InternalError(ADP.InternalErrorCode.NonPooledObjectUsedMoreThanOnce);
            }
        }

        internal void PrePush(object expectedOwner)
        {
            if (expectedOwner == null)
            {
                if (this._owningObject.Target != null)
                {
                    throw ADP.InternalError(ADP.InternalErrorCode.UnpooledObjectHasOwner);
                }
            }
            else if (this._owningObject.Target != expectedOwner)
            {
                throw ADP.InternalError(ADP.InternalErrorCode.UnpooledObjectHasWrongOwner);
            }
            if (this._pooledCount != 0)
            {
                throw ADP.InternalError(ADP.InternalErrorCode.PushingObjectSecondTime);
            }
            if (Bid.IsOn(Bid.ApiGroup.Pooling))
            {
                Bid.PoolerTrace("<prov.DbConnectionInternal.PrePush|RES|CPOOL> %d#, Preparing to push into pool, owning connection %d#, pooledCount=%d\n", this.ObjectID, 0, this._pooledCount);
            }
            this._pooledCount++;
            this._owningObject.Target = null;
        }

        internal void RemoveWeakReference(object value)
        {
            DbReferenceCollection referenceCollection = this.ReferenceCollection;
            if (referenceCollection != null)
            {
                referenceCollection.Remove(value);
            }
        }

        internal void SetInStasis()
        {
            this._isInStasis = true;
            Bid.PoolerTrace("<prov.DbConnectionInternal.SetInStasis|RES|CPOOL> %d#, Non-Pooled Connection has Delegated Transaction, waiting to Dispose.\n", this.ObjectID);
            this.PerformanceCounters.NumberOfStasisConnections.Increment();
        }

        private void TerminateStasis(bool returningToPool)
        {
            if (returningToPool)
            {
                Bid.PoolerTrace("<prov.DbConnectionInternal.TerminateStasis|RES|CPOOL> %d#, Delegated Transaction has ended, connection is closed.  Returning to general pool.\n", this.ObjectID);
            }
            else
            {
                Bid.PoolerTrace("<prov.DbConnectionInternal.TerminateStasis|RES|CPOOL> %d#, Delegated Transaction has ended, connection is closed/leaked.  Disposing.\n", this.ObjectID);
            }
            this.PerformanceCounters.NumberOfStasisConnections.Decrement();
            this._isInStasis = false;
        }

        private void TransactionCompletedEvent(object sender, TransactionEventArgs e)
        {
            Transaction transaction = e.Transaction;
            Bid.Trace("<prov.DbConnectionInternal.TransactionCompletedEvent|RES|CPOOL> %d#, Transaction Completed. (pooledCount=%d)\n", this.ObjectID, this._pooledCount);
            this.CleanupTransactionOnCompletion(transaction);
            this.CleanupConnectionOnTransactionCompletion(transaction);
        }

        [SecurityPermission(SecurityAction.Assert, Flags=SecurityPermissionFlag.UnmanagedCode)]
        private void TransactionOutcomeEnlist(Transaction transaction)
        {
            transaction.TransactionCompleted += new TransactionCompletedEventHandler(this.TransactionCompletedEvent);
        }

        internal bool AllowSetConnectionString =>
            this._allowSetConnectionString;

        internal bool CanBePooled =>
            ((!this._connectionIsDoomed && !this._cannotBePooled) && !this._owningObject.IsAlive);

        protected internal Transaction EnlistedTransaction
        {
            get => 
                this._enlistedTransaction;
            set
            {
                if (((null == this._enlistedTransaction) && (null != value)) || ((null != this._enlistedTransaction) && !this._enlistedTransaction.Equals(value)))
                {
                    Transaction transaction = null;
                    Transaction transaction2 = null;
                    try
                    {
                        if (null != value)
                        {
                            transaction = value.Clone();
                        }
                        lock (this)
                        {
                            transaction2 = this._enlistedTransaction;
                            this._enlistedTransaction = transaction;
                            value = transaction;
                            transaction = null;
                        }
                    }
                    finally
                    {
                        if (null != transaction2)
                        {
                            transaction2.Dispose();
                        }
                        if (null != transaction)
                        {
                            transaction.Dispose();
                        }
                    }
                    if (null != value)
                    {
                        if (Bid.IsOn(Bid.ApiGroup.Pooling))
                        {
                            int hashCode = value.GetHashCode();
                            Bid.PoolerTrace("<prov.DbConnectionInternal.set_EnlistedTransaction|RES|CPOOL> %d#, Transaction %d#, Enlisting.\n", this.ObjectID, hashCode);
                        }
                        this.TransactionOutcomeEnlist(value);
                    }
                }
            }
        }

        public bool HasEnlistedTransaction =>
            (null != this.EnlistedTransaction);

        protected internal bool IsConnectionDoomed =>
            this._connectionIsDoomed;

        internal bool IsEmancipated =>
            ((!this.IsTxRootWaitingForTxEnd && (this._pooledCount < 1)) && !this._owningObject.IsAlive);

        protected internal virtual bool IsNonPoolableTransactionRoot =>
            false;

        internal virtual bool IsTransactionRoot =>
            false;

        internal bool IsTxRootWaitingForTxEnd =>
            this._isInStasis;

        internal DbConnectionInternal NextPooledObject
        {
            get => 
                this._nextPooledObject;
            set
            {
                this._nextPooledObject = value;
            }
        }

        internal int ObjectID =>
            this._objectID;

        protected internal object Owner =>
            this._owningObject.Target;

        protected DbConnectionPoolCounters PerformanceCounters =>
            this._performanceCounters;

        internal DbConnectionPool Pool =>
            this._connectionPool;

        protected virtual bool ReadyToPrepareTransaction =>
            true;

        protected internal DbReferenceCollection ReferenceCollection =>
            this._referenceCollection;

        internal virtual bool RequireExplicitTransactionUnbind =>
            false;

        public abstract string ServerVersion { get; }

        public virtual string ServerVersionNormalized
        {
            get
            {
                throw ADP.NotSupported();
            }
        }

        public bool ShouldHidePassword =>
            this._hidePassword;

        public ConnectionState State =>
            this._state;
    }
}

