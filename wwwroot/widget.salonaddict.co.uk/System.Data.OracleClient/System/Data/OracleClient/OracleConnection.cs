namespace System.Data.OracleClient
{
    using System;
    using System.ComponentModel;
    using System.Data;
    using System.Data.Common;
    using System.Data.ProviderBase;
    using System.Diagnostics;
    using System.EnterpriseServices;
    using System.Globalization;
    using System.Runtime.ConstrainedExecution;
    using System.Security;
    using System.Security.Permissions;
    using System.Threading;
    using System.Transactions;

    [DefaultEvent("InfoMessage")]
    public sealed class OracleConnection : DbConnection, ICloneable
    {
        private int _closeCount;
        private static readonly System.Data.ProviderBase.DbConnectionFactory _connectionFactory = OracleConnectionFactory.SingletonInstance;
        private System.Data.ProviderBase.DbConnectionInternal _innerConnection;
        private static int _objectTypeCount;
        private System.Data.ProviderBase.DbConnectionPoolGroup _poolGroup;
        private System.Data.Common.DbConnectionOptions _userConnectionOptions;
        private static readonly object EventInfoMessage = new object();
        internal static readonly CodeAccessPermission ExecutePermission = CreateExecutePermission();
        internal readonly int ObjectID;

        [System.Data.OracleClient.ResCategory("OracleCategory_InfoMessage"), System.Data.OracleClient.ResDescription("OracleConnection_InfoMessage")]
        public event OracleInfoMessageEventHandler InfoMessage
        {
            add
            {
                base.Events.AddHandler(EventInfoMessage, value);
            }
            remove
            {
                base.Events.RemoveHandler(EventInfoMessage, value);
            }
        }

        public OracleConnection()
        {
            this.ObjectID = Interlocked.Increment(ref _objectTypeCount);
            GC.SuppressFinalize(this);
            this._innerConnection = System.Data.ProviderBase.DbConnectionClosedNeverOpened.SingletonInstance;
        }

        internal OracleConnection(OracleConnection connection) : this()
        {
            this.CopyFrom(connection);
        }

        public OracleConnection(string connectionString) : this()
        {
            this.ConnectionString = connectionString;
        }

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        internal void Abort(Exception e)
        {
            System.Data.ProviderBase.DbConnectionInternal comparand = this._innerConnection;
            if (ConnectionState.Open == comparand.State)
            {
                Interlocked.CompareExchange<System.Data.ProviderBase.DbConnectionInternal>(ref this._innerConnection, System.Data.ProviderBase.DbConnectionClosedPreviouslyOpened.SingletonInstance, comparand);
                comparand.DoomThisConnection();
            }
            if (e is OutOfMemoryException)
            {
                Bid.Trace("<prov.DbConnectionHelper.Abort|RES|INFO|CPOOL> %d#, Aborting operation due to asynchronous exception: %ls\n", this.ObjectID, "OutOfMemory");
            }
            else
            {
                Bid.Trace("<prov.DbConnectionHelper.Abort|RES|INFO|CPOOL> %d#, Aborting operation due to asynchronous exception: %ls\n", this.ObjectID, e.ToString());
            }
        }

        internal void AddWeakReference(object value, int tag)
        {
            this.InnerConnection.AddWeakReference(value, tag);
        }

        protected override DbTransaction BeginDbTransaction(System.Data.IsolationLevel isolationLevel)
        {
            DbTransaction transaction;
            IntPtr ptr;
            Bid.ScopeEnter(out ptr, "<prov.DbConnectionHelper.BeginDbTransaction|API> %d#, isolationLevel=%d{ds.IsolationLevel}", this.ObjectID, (int) isolationLevel);
            try
            {
                transaction = this.InnerConnection.BeginTransaction(isolationLevel);
            }
            finally
            {
                Bid.ScopeLeave(ref ptr);
            }
            return transaction;
        }

        public OracleTransaction BeginTransaction() => 
            this.BeginTransaction(System.Data.IsolationLevel.Unspecified);

        public OracleTransaction BeginTransaction(System.Data.IsolationLevel il) => 
            ((OracleTransaction) base.BeginTransaction(il));

        public override void ChangeDatabase(string value)
        {
            IntPtr ptr;
            Bid.ScopeEnter(out ptr, "<ora.OracleConnection.ChangeDatabase|API> %d#, value='%ls'\n", this.ObjectID, value);
            try
            {
                throw System.Data.Common.ADP.ChangeDatabaseNotSupported();
            }
            finally
            {
                Bid.ScopeLeave(ref ptr);
            }
        }

        internal void CheckError(OciErrorHandle errorHandle, int rc)
        {
            switch (((OCI.RETURNCODE) rc))
            {
                case OCI.RETURNCODE.OCI_INVALID_HANDLE:
                    throw System.Data.Common.ADP.InvalidOperation(System.Data.OracleClient.Res.GetString("ADP_InternalError", new object[] { rc }));

                case OCI.RETURNCODE.OCI_ERROR:
                case OCI.RETURNCODE.OCI_NO_DATA:
                {
                    Exception exception2 = System.Data.Common.ADP.OracleError(errorHandle, rc);
                    if ((errorHandle != null) && errorHandle.ConnectionIsBroken)
                    {
                        OracleInternalConnection openInternalConnection = this.GetOpenInternalConnection();
                        if (openInternalConnection != null)
                        {
                            openInternalConnection.ConnectionIsBroken();
                        }
                    }
                    throw exception2;
                }
                case OCI.RETURNCODE.OCI_SUCCESS_WITH_INFO:
                {
                    OracleInfoMessageEventArgs infoMessageEvent = new OracleInfoMessageEventArgs(OracleException.CreateException(errorHandle, rc));
                    this.OnInfoMessage(infoMessageEvent);
                    return;
                }
            }
            if ((rc < 0) || (rc == 0x63))
            {
                throw System.Data.Common.ADP.Simple(System.Data.OracleClient.Res.GetString("ADP_UnexpectedReturnCode", new object[] { rc.ToString(CultureInfo.CurrentCulture) }));
            }
        }

        public static void ClearAllPools()
        {
            new OraclePermission(PermissionState.Unrestricted).Demand();
            OracleConnectionFactory.SingletonInstance.ClearAllPools();
        }

        public static void ClearPool(OracleConnection connection)
        {
            System.Data.Common.ADP.CheckArgumentNull(connection, "connection");
            System.Data.Common.DbConnectionOptions userConnectionOptions = connection.UserConnectionOptions;
            if (userConnectionOptions != null)
            {
                userConnectionOptions.DemandPermission();
                OracleConnectionFactory.SingletonInstance.ClearPool(connection);
            }
        }

        public override void Close()
        {
            this.InnerConnection.CloseConnection(this, this.ConnectionFactory);
        }

        internal void Commit()
        {
            this.GetOpenInternalConnection().Commit();
        }

        private string ConnectionString_Get()
        {
            Bid.Trace("<prov.DbConnectionHelper.ConnectionString_Get|API> %d#\n", this.ObjectID);
            bool shouldHidePassword = this.InnerConnection.ShouldHidePassword;
            System.Data.Common.DbConnectionOptions userConnectionOptions = this.UserConnectionOptions;
            return userConnectionOptions?.UsersConnectionString(shouldHidePassword);
        }

        private void ConnectionString_Set(string value)
        {
            System.Data.Common.DbConnectionOptions userConnectionOptions = null;
            System.Data.ProviderBase.DbConnectionPoolGroup group = this.ConnectionFactory.GetConnectionPoolGroup(value, null, ref userConnectionOptions);
            System.Data.ProviderBase.DbConnectionInternal innerConnection = this.InnerConnection;
            bool allowSetConnectionString = innerConnection.AllowSetConnectionString;
            if (allowSetConnectionString)
            {
                allowSetConnectionString = this.SetInnerConnectionFrom(System.Data.ProviderBase.DbConnectionClosedBusy.SingletonInstance, innerConnection);
                if (allowSetConnectionString)
                {
                    this._userConnectionOptions = userConnectionOptions;
                    this._poolGroup = group;
                    this._innerConnection = System.Data.ProviderBase.DbConnectionClosedNeverOpened.SingletonInstance;
                }
            }
            if (!allowSetConnectionString)
            {
                throw System.Data.Common.ADP.OpenConnectionPropertySet("ConnectionString", innerConnection.State);
            }
            if (Bid.TraceOn)
            {
                string str = (userConnectionOptions != null) ? userConnectionOptions.UsersConnectionStringForTrace() : "";
                Bid.Trace("<prov.DbConnectionHelper.ConnectionString_Set|API> %d#, '%ls'\n", this.ObjectID, str);
            }
        }

        private void CopyFrom(OracleConnection connection)
        {
            System.Data.Common.ADP.CheckArgumentNull(connection, "connection");
            this._userConnectionOptions = connection.UserConnectionOptions;
            this._poolGroup = connection.PoolGroup;
            if (System.Data.ProviderBase.DbConnectionClosedNeverOpened.SingletonInstance == connection._innerConnection)
            {
                this._innerConnection = System.Data.ProviderBase.DbConnectionClosedNeverOpened.SingletonInstance;
            }
            else
            {
                this._innerConnection = System.Data.ProviderBase.DbConnectionClosedPreviouslyOpened.SingletonInstance;
            }
        }

        public OracleCommand CreateCommand() => 
            new OracleCommand { Connection = this };

        protected override DbCommand CreateDbCommand()
        {
            DbCommand command = null;
            IntPtr ptr;
            Bid.ScopeEnter(out ptr, "<prov.DbConnectionHelper.CreateDbCommand|API> %d#\n", this.ObjectID);
            try
            {
                command = this.ConnectionFactory.ProviderFactory.CreateCommand();
                command.Connection = this;
            }
            finally
            {
                Bid.ScopeLeave(ref ptr);
            }
            return command;
        }

        private static CodeAccessPermission CreateExecutePermission()
        {
            OraclePermission permission = new OraclePermission(PermissionState.None);
            permission.Add(string.Empty, string.Empty, KeyRestrictionBehavior.AllowOnly);
            return permission;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this._userConnectionOptions = null;
                this._poolGroup = null;
                this.Close();
            }
            this.DisposeMe(disposing);
            base.Dispose(disposing);
        }

        private void DisposeMe(bool disposing)
        {
        }

        public void EnlistDistributedTransaction(ITransaction distributedTransaction)
        {
            this.EnlistDistributedTransactionHelper(distributedTransaction);
        }

        private void EnlistDistributedTransactionHelper(ITransaction transaction)
        {
            PermissionSet set = new PermissionSet(PermissionState.None);
            set.AddPermission(ExecutePermission);
            set.AddPermission(new SecurityPermission(SecurityPermissionFlag.UnmanagedCode));
            set.Demand();
            Bid.Trace("<prov.DbConnectionHelper.EnlistDistributedTransactionHelper|RES|TRAN> %d#, Connection enlisting in a transaction.\n", this.ObjectID);
            System.Transactions.Transaction transactionFromDtcTransaction = null;
            if (transaction != null)
            {
                transactionFromDtcTransaction = TransactionInterop.GetTransactionFromDtcTransaction((IDtcTransaction) transaction);
            }
            this.InnerConnection.EnlistTransaction(transactionFromDtcTransaction);
            GC.KeepAlive(this);
        }

        public override void EnlistTransaction(System.Transactions.Transaction transaction)
        {
            ExecutePermission.Demand();
            Bid.Trace("<prov.DbConnectionHelper.EnlistTransaction|RES|TRAN> %d#, Connection enlisting in a transaction.\n", this.ObjectID);
            System.Data.ProviderBase.DbConnectionInternal innerConnection = this.InnerConnection;
            if (innerConnection.HasEnlistedTransaction)
            {
                if (!innerConnection.EnlistedTransaction.Equals(transaction))
                {
                    throw System.Data.Common.ADP.TransactionPresent();
                }
            }
            else
            {
                innerConnection.EnlistTransaction(transaction);
                GC.KeepAlive(this);
            }
        }

        internal byte[] GetBytes(string value, bool useNationalCharacterSet) => 
            this.GetOpenInternalConnection().GetBytes(value, useNationalCharacterSet);

        private System.Data.ProviderBase.DbMetaDataFactory GetMetaDataFactory(System.Data.ProviderBase.DbConnectionInternal internalConnection) => 
            this.ConnectionFactory.GetMetaDataFactory(this._poolGroup, internalConnection);

        internal System.Data.ProviderBase.DbMetaDataFactory GetMetaDataFactoryInternal(System.Data.ProviderBase.DbConnectionInternal internalConnection) => 
            this.GetMetaDataFactory(internalConnection);

        internal OracleInternalConnection GetOpenInternalConnection()
        {
            System.Data.ProviderBase.DbConnectionInternal innerConnection = this.InnerConnection;
            if (!(innerConnection is OracleInternalConnection))
            {
                throw System.Data.Common.ADP.ClosedConnectionError();
            }
            return (innerConnection as OracleInternalConnection);
        }

        public override DataTable GetSchema() => 
            this.GetSchema(DbMetaDataCollectionNames.MetaDataCollections, null);

        public override DataTable GetSchema(string collectionName) => 
            this.GetSchema(collectionName, null);

        public override DataTable GetSchema(string collectionName, string[] restrictionValues)
        {
            ExecutePermission.Demand();
            return this.InnerConnection.GetSchema(this.ConnectionFactory, this.PoolGroup, this, collectionName, restrictionValues);
        }

        internal NativeBuffer GetScratchBuffer(int minSize) => 
            this.GetOpenInternalConnection().GetScratchBuffer(minSize);

        internal string GetString(byte[] bytearray) => 
            this.GetOpenInternalConnection().GetString(bytearray);

        internal string GetString(byte[] bytearray, bool useNationalCharacterSet) => 
            this.GetOpenInternalConnection().GetString(bytearray, useNationalCharacterSet);

        internal void NotifyWeakReference(int message)
        {
            this.InnerConnection.NotifyWeakReference(message);
        }

        internal void OnInfoMessage(OracleInfoMessageEventArgs infoMessageEvent)
        {
            OracleInfoMessageEventHandler handler = (OracleInfoMessageEventHandler) base.Events[EventInfoMessage];
            if (handler != null)
            {
                handler(this, infoMessageEvent);
            }
        }

        public override void Open()
        {
            this.InnerConnection.OpenConnection(this, this.ConnectionFactory);
            OracleInternalConnection innerConnection = this.InnerConnection as OracleInternalConnection;
            if (innerConnection != null)
            {
                innerConnection.FireDeferredInfoMessageEvents(this);
            }
        }

        internal void PermissionDemand()
        {
            System.Data.ProviderBase.DbConnectionPoolGroup poolGroup = this.PoolGroup;
            System.Data.Common.DbConnectionOptions connectionOptions = poolGroup?.ConnectionOptions;
            if ((connectionOptions == null) || connectionOptions.IsEmpty)
            {
                throw System.Data.Common.ADP.NoConnectionString();
            }
            this.UserConnectionOptions.DemandPermission();
        }

        internal void RemoveWeakReference(object value)
        {
            this.InnerConnection.RemoveWeakReference(value);
        }

        internal void Rollback()
        {
            OracleInternalConnection innerConnection = this.InnerConnection as OracleInternalConnection;
            if (innerConnection != null)
            {
                innerConnection.Rollback();
            }
        }

        internal void RollbackDeadTransaction()
        {
            this.GetOpenInternalConnection().RollbackDeadTransaction();
        }

        internal void SetInnerConnectionEvent(System.Data.ProviderBase.DbConnectionInternal to)
        {
            ConnectionState originalState = this._innerConnection.State & ConnectionState.Open;
            ConnectionState currentState = to.State & ConnectionState.Open;
            if ((originalState != currentState) && (currentState == ConnectionState.Closed))
            {
                this._closeCount++;
            }
            this._innerConnection = to;
            if ((originalState == ConnectionState.Closed) && (ConnectionState.Open == currentState))
            {
                this.OnStateChange(System.Data.ProviderBase.DbConnectionInternal.StateChangeOpen);
            }
            else if ((ConnectionState.Open == originalState) && (currentState == ConnectionState.Closed))
            {
                this.OnStateChange(System.Data.ProviderBase.DbConnectionInternal.StateChangeClosed);
            }
            else if (originalState != currentState)
            {
                this.OnStateChange(new StateChangeEventArgs(originalState, currentState));
            }
        }

        internal bool SetInnerConnectionFrom(System.Data.ProviderBase.DbConnectionInternal to, System.Data.ProviderBase.DbConnectionInternal from) => 
            (from == Interlocked.CompareExchange<System.Data.ProviderBase.DbConnectionInternal>(ref this._innerConnection, to, from));

        internal void SetInnerConnectionTo(System.Data.ProviderBase.DbConnectionInternal to)
        {
            this._innerConnection = to;
        }

        object ICloneable.Clone()
        {
            OracleConnection connection = new OracleConnection(this);
            Bid.Trace("<ora.OracleConnection.Clone|API> %d#, clone=%d#\n", this.ObjectID, connection.ObjectID);
            return connection;
        }

        [Conditional("DEBUG")]
        internal static void VerifyExecutePermission()
        {
            try
            {
                ExecutePermission.Demand();
            }
            catch (SecurityException)
            {
                throw;
            }
        }

        internal int CloseCount =>
            this._closeCount;

        internal System.Data.ProviderBase.DbConnectionFactory ConnectionFactory =>
            _connectionFactory;

        internal System.Data.Common.DbConnectionOptions ConnectionOptions
        {
            get
            {
                System.Data.ProviderBase.DbConnectionPoolGroup poolGroup = this.PoolGroup;
                return poolGroup?.ConnectionOptions;
            }
        }

        [RefreshProperties(RefreshProperties.All), Editor("Microsoft.VSDesigner.Data.Oracle.Design.OracleConnectionStringEditor, Microsoft.VSDesigner, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.Drawing.Design.UITypeEditor, System.Drawing, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"), System.Data.OracleClient.ResDescription("OracleConnection_ConnectionString"), RecommendedAsConfigurable(true), System.Data.OracleClient.ResCategory("OracleCategory_Data"), DefaultValue("")]
        public override string ConnectionString
        {
            get => 
                this.ConnectionString_Get();
            set
            {
                this.ConnectionString_Set(value);
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
        public override int ConnectionTimeout =>
            0;

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public override string Database =>
            string.Empty;

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), System.Data.OracleClient.ResDescription("OracleConnection_DataSource")]
        public override string DataSource
        {
            get
            {
                OracleConnectionString connectionOptions = (OracleConnectionString) this.ConnectionOptions;
                string dataSource = string.Empty;
                if (connectionOptions != null)
                {
                    dataSource = connectionOptions.DataSource;
                }
                return dataSource;
            }
        }

        internal OciEnvironmentHandle EnvironmentHandle =>
            this.GetOpenInternalConnection().EnvironmentHandle;

        internal OciErrorHandle ErrorHandle =>
            this.GetOpenInternalConnection().ErrorHandle;

        internal bool HasTransaction =>
            this.GetOpenInternalConnection().HasTransaction;

        internal System.Data.ProviderBase.DbConnectionInternal InnerConnection =>
            this._innerConnection;

        internal System.Data.ProviderBase.DbConnectionPoolGroup PoolGroup
        {
            get => 
                this._poolGroup;
            set
            {
                this._poolGroup = value;
            }
        }

        internal TimeSpan ServerTimeZoneAdjustmentToUTC =>
            this.GetOpenInternalConnection().GetServerTimeZoneAdjustmentToUTC(this);

        [Browsable(false), System.Data.OracleClient.ResDescription("OracleConnection_ServerVersion"), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override string ServerVersion =>
            this.GetOpenInternalConnection().ServerVersion;

        internal bool ServerVersionAtLeastOracle8 =>
            this.GetOpenInternalConnection().ServerVersionAtLeastOracle8;

        internal bool ServerVersionAtLeastOracle9i =>
            this.GetOpenInternalConnection().ServerVersionAtLeastOracle9i;

        internal OciServiceContextHandle ServiceContextHandle =>
            this.GetOpenInternalConnection().ServiceContextHandle;

        [Browsable(false), System.Data.OracleClient.ResDescription("DbConnection_State"), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override ConnectionState State =>
            this.InnerConnection.State;

        internal OracleTransaction Transaction =>
            this.GetOpenInternalConnection().Transaction;

        internal System.Data.OracleClient.TransactionState TransactionState
        {
            get => 
                this.GetOpenInternalConnection().TransactionState;
            set
            {
                this.GetOpenInternalConnection().TransactionState = value;
            }
        }

        internal bool UnicodeEnabled =>
            this.GetOpenInternalConnection().UnicodeEnabled;

        internal System.Data.Common.DbConnectionOptions UserConnectionOptions =>
            this._userConnectionOptions;
    }
}

