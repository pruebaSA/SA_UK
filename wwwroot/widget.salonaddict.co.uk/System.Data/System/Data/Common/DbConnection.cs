namespace System.Data.Common
{
    using System;
    using System.ComponentModel;
    using System.Data;
    using System.Transactions;

    public abstract class DbConnection : Component, IDbConnection, IDisposable
    {
        [ResCategory("DataCategory_StateChange"), ResDescription("DbConnection_StateChange")]
        public event StateChangeEventHandler StateChange;

        protected DbConnection()
        {
        }

        protected abstract DbTransaction BeginDbTransaction(System.Data.IsolationLevel isolationLevel);
        public DbTransaction BeginTransaction() => 
            this.BeginDbTransaction(System.Data.IsolationLevel.Unspecified);

        public DbTransaction BeginTransaction(System.Data.IsolationLevel isolationLevel) => 
            this.BeginDbTransaction(isolationLevel);

        public abstract void ChangeDatabase(string databaseName);
        public abstract void Close();
        public DbCommand CreateCommand() => 
            this.CreateDbCommand();

        protected abstract DbCommand CreateDbCommand();
        public virtual void EnlistTransaction(Transaction transaction)
        {
            throw ADP.NotSupported();
        }

        public virtual DataTable GetSchema()
        {
            throw ADP.NotSupported();
        }

        public virtual DataTable GetSchema(string collectionName)
        {
            throw ADP.NotSupported();
        }

        public virtual DataTable GetSchema(string collectionName, string[] restrictionValues)
        {
            throw ADP.NotSupported();
        }

        protected virtual void OnStateChange(StateChangeEventArgs stateChange)
        {
            StateChangeEventHandler handler = this._stateChangeEventHandler;
            if (handler != null)
            {
                handler(this, stateChange);
            }
        }

        public abstract void Open();
        IDbTransaction IDbConnection.BeginTransaction() => 
            this.BeginDbTransaction(System.Data.IsolationLevel.Unspecified);

        IDbTransaction IDbConnection.BeginTransaction(System.Data.IsolationLevel isolationLevel) => 
            this.BeginDbTransaction(isolationLevel);

        IDbCommand IDbConnection.CreateCommand() => 
            this.CreateDbCommand();

        [RecommendedAsConfigurable(true), DefaultValue(""), ResCategory("DataCategory_Data"), RefreshProperties(RefreshProperties.All)]
        public abstract string ConnectionString { get; set; }

        [ResCategory("DataCategory_Data")]
        public virtual int ConnectionTimeout =>
            15;

        [ResCategory("DataCategory_Data")]
        public abstract string Database { get; }

        [ResCategory("DataCategory_Data")]
        public abstract string DataSource { get; }

        protected virtual System.Data.Common.DbProviderFactory DbProviderFactory =>
            null;

        internal System.Data.Common.DbProviderFactory ProviderFactory =>
            this.DbProviderFactory;

        [Browsable(false)]
        public abstract string ServerVersion { get; }

        [ResDescription("DbConnection_State"), Browsable(false)]
        public abstract ConnectionState State { get; }
    }
}

