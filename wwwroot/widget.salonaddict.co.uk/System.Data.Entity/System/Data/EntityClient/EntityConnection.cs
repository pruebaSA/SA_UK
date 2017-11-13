namespace System.Data.EntityClient
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data;
    using System.Data.Common;
    using System.Data.Entity;
    using System.Data.Mapping;
    using System.Data.Metadata.Edm;
    using System.Data.ProviderBase;
    using System.Globalization;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Threading;
    using System.Transactions;

    public sealed class EntityConnection : DbConnection
    {
        private MetadataArtifactLoader _artifactLoader;
        private readonly object _connectionStringLock;
        private EntityTransaction _currentTransaction;
        private DbConnectionOptions _effectiveConnectionOptions;
        private ConnectionState _entityClientConnectionState;
        private bool _initialized;
        private Guid _metadataID;
        private MetadataWorkspace _metadataWorkspace;
        private static int _objectTypeCount;
        private System.Data.Common.DbProviderFactory _providerFactory;
        private DbConnection _storeConnection;
        private DbConnectionOptions _userConnectionOptions;
        internal readonly int ObjectID;
        private const string s_dataDirectory = "|datadirectory|";
        private static readonly DbConnectionOptions s_emptyConnectionOptions = new DbConnectionOptions(string.Empty, null, false);
        private const string s_entityClientProviderName = "System.Data.EntityClient";
        private const string s_metadataid = "metadata id";
        private const string s_metadataPathSeparator = "|";
        private const string s_providerConnectionString = "provider connection string";
        private const string s_providerInvariantName = "provider";
        private const string s_semicolonSeparator = ";";

        public EntityConnection() : this(string.Empty)
        {
        }

        public EntityConnection(string connectionString)
        {
            this._connectionStringLock = new object();
            this.ObjectID = Interlocked.Increment(ref _objectTypeCount);
            GC.SuppressFinalize(this);
            this.ChangeConnectionString(connectionString);
        }

        public EntityConnection(MetadataWorkspace workspace, DbConnection connection)
        {
            this._connectionStringLock = new object();
            this.ObjectID = Interlocked.Increment(ref _objectTypeCount);
            EntityUtil.CheckArgumentNull<MetadataWorkspace>(workspace, "workspace");
            EntityUtil.CheckArgumentNull<DbConnection>(connection, "connection");
            if (!workspace.IsItemCollectionAlreadyRegistered(DataSpace.CSpace))
            {
                throw EntityUtil.Argument(Strings.EntityClient_ItemCollectionsNotRegisteredInWorkspace("EdmItemCollection"));
            }
            if (!workspace.IsItemCollectionAlreadyRegistered(DataSpace.SSpace))
            {
                throw EntityUtil.Argument(Strings.EntityClient_ItemCollectionsNotRegisteredInWorkspace("StoreItemCollection"));
            }
            if (!workspace.IsItemCollectionAlreadyRegistered(DataSpace.CSSpace))
            {
                throw EntityUtil.Argument(Strings.EntityClient_ItemCollectionsNotRegisteredInWorkspace("StorageMappingItemCollection"));
            }
            if (connection.State != ConnectionState.Closed)
            {
                throw EntityUtil.Argument(Strings.EntityClient_ConnectionMustBeClosed);
            }
            if (connection.ProviderFactory == null)
            {
                throw EntityUtil.ProviderIncompatible(Strings.EntityClient_DbConnectionHasNoProvider(connection));
            }
            StoreItemCollection itemCollection = (StoreItemCollection) workspace.GetItemCollection(DataSpace.SSpace);
            ValidateThatConnectionAndMetadataProvidersAreEqual(connection.ProviderFactory, null, itemCollection.StoreProviderFactory);
            GC.SuppressFinalize(this);
            this._providerFactory = connection.ProviderFactory;
            this._storeConnection = connection;
            this._metadataWorkspace = workspace;
            this._initialized = true;
            this._metadataID = Guid.NewGuid();
        }

        protected override DbTransaction BeginDbTransaction(System.Data.IsolationLevel isolationLevel)
        {
            if (this.CurrentTransaction != null)
            {
                throw EntityUtil.InvalidOperation(Strings.EntityClient_TransactionAlreadyStarted);
            }
            if (this._storeConnection == null)
            {
                throw EntityUtil.InvalidOperation(Strings.EntityClient_ConnectionStringNeededBeforeOperation);
            }
            if (this.State != ConnectionState.Open)
            {
                throw EntityUtil.InvalidOperation(Strings.EntityClient_ConnectionNotOpen);
            }
            DbTransaction storeTransaction = null;
            try
            {
                storeTransaction = this._storeConnection.BeginTransaction(isolationLevel);
            }
            catch (Exception exception)
            {
                if (EntityUtil.IsCatchableExceptionType(exception))
                {
                    throw EntityUtil.ProviderExceptionWithMessage(Strings.EntityClient_ErrorInBeginningTransaction, exception);
                }
                throw;
            }
            if (storeTransaction == null)
            {
                throw EntityUtil.ProviderIncompatible(Strings.EntityClient_ReturnedNullOnProviderMethod("BeginTransaction", this._storeConnection.GetType().Name));
            }
            this._currentTransaction = new EntityTransaction(this, storeTransaction);
            return this._currentTransaction;
        }

        public EntityTransaction BeginTransaction() => 
            (base.BeginTransaction() as EntityTransaction);

        public EntityTransaction BeginTransaction(System.Data.IsolationLevel isolationLevel) => 
            (base.BeginTransaction(isolationLevel) as EntityTransaction);

        private void ChangeConnectionString(string newConnectionString)
        {
            DbConnectionOptions options = s_emptyConnectionOptions;
            if (!string.IsNullOrEmpty(newConnectionString))
            {
                options = new DbConnectionOptions(newConnectionString, EntityConnectionStringBuilder.Synonyms, false);
            }
            System.Data.Common.DbProviderFactory factory = null;
            DbConnection newConnection = null;
            DbConnectionOptions effectiveConnectionOptions = options;
            if (!options.IsEmpty)
            {
                string str = options["name"];
                if (!string.IsNullOrEmpty(str))
                {
                    if (1 < options.Parsetable.Count)
                    {
                        throw EntityUtil.Argument(Strings.EntityClient_ExtraParametersWithNamedConnection);
                    }
                    ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings[str];
                    if ((settings == null) || (settings.ProviderName != "System.Data.EntityClient"))
                    {
                        throw EntityUtil.Argument(Strings.EntityClient_InvalidNamedConnection);
                    }
                    effectiveConnectionOptions = new DbConnectionOptions(settings.ConnectionString, EntityConnectionStringBuilder.Synonyms, false);
                    string str2 = effectiveConnectionOptions["name"];
                    if (!string.IsNullOrEmpty(str2))
                    {
                        throw EntityUtil.Argument(Strings.EntityClient_NestedNamedConnection(str));
                    }
                }
                ValidateValueForTheKeyword(effectiveConnectionOptions, "metadata");
                string providerString = ValidateValueForTheKeyword(effectiveConnectionOptions, "provider");
                factory = this.GetFactory(providerString);
                newConnection = this.GetStoreConnection(factory);
                try
                {
                    string str4 = effectiveConnectionOptions["provider connection string"];
                    if (str4 != null)
                    {
                        newConnection.ConnectionString = str4;
                    }
                }
                catch (Exception exception)
                {
                    if (EntityUtil.IsCatchableExceptionType(exception))
                    {
                        throw EntityUtil.Provider("ConnectionString", exception);
                    }
                    throw;
                }
            }
            lock (this._connectionStringLock)
            {
                this._providerFactory = factory;
                this._metadataWorkspace = null;
                this.ClearCurrentTransaction();
                this.ResetStoreConnection(newConnection, null, false);
                this._userConnectionOptions = options;
                this._effectiveConnectionOptions = effectiveConnectionOptions;
            }
        }

        public override void ChangeDatabase(string databaseName)
        {
            throw EntityUtil.NotSupported();
        }

        internal void ClearCurrentTransaction()
        {
            this._currentTransaction = null;
        }

        internal EntityConnection Clone() => 
            new EntityConnection(this._userConnectionOptions.UsersConnectionString(false));

        public override void Close()
        {
            if (this._storeConnection != null)
            {
                this.CloseHelper();
            }
        }

        private void CloseHelper()
        {
            ConnectionState previousState = this.State;
            this.StoreCloseHelper();
            this.EntityCloseHelper(true, previousState);
        }

        public EntityCommand CreateCommand() => 
            new EntityCommand(null, this);

        protected override DbCommand CreateDbCommand() => 
            this.CreateCommand();

        private static string CreateMetadataCacheKey(IList<string> paths, string providerName, string providerConnectionString)
        {
            string str;
            int resultCount = 0;
            CreateMetadataCacheKeyWithCount(paths, providerName, providerConnectionString, false, ref resultCount, out str);
            CreateMetadataCacheKeyWithCount(paths, providerName, providerConnectionString, true, ref resultCount, out str);
            return str;
        }

        private static void CreateMetadataCacheKeyWithCount(IList<string> paths, string providerName, string providerConnectionString, bool buildResult, ref int resultCount, out string result)
        {
            StringBuilder builder;
            if (buildResult)
            {
                builder = new StringBuilder(resultCount);
            }
            else
            {
                builder = null;
            }
            resultCount = 0;
            if (!string.IsNullOrEmpty(providerName))
            {
                resultCount += providerName.Length + 1;
                if (buildResult)
                {
                    builder.Append(providerName);
                    builder.Append(";");
                }
            }
            if (paths != null)
            {
                for (int i = 0; i < paths.Count; i++)
                {
                    if (paths[i].Length > 0)
                    {
                        if (i > 0)
                        {
                            resultCount++;
                            if (buildResult)
                            {
                                builder.Append("|");
                            }
                        }
                        resultCount += paths[i].Length;
                        if (buildResult)
                        {
                            builder.Append(paths[i]);
                        }
                    }
                }
                resultCount++;
                if (buildResult)
                {
                    builder.Append(";");
                }
            }
            if (!string.IsNullOrEmpty(providerConnectionString))
            {
                resultCount += providerConnectionString.Length;
                if (buildResult)
                {
                    builder.Append(providerConnectionString);
                }
            }
            if (buildResult)
            {
                result = builder.ToString();
            }
            else
            {
                result = null;
            }
        }

        protected override void Dispose(bool disposing)
        {
            this.ClearCurrentTransaction();
            bool flag = this.EntityCloseHelper(false, this.State);
            if (disposing && (this._storeConnection != null))
            {
                this.StoreCloseHelper();
                if (this._storeConnection != null)
                {
                    this._storeConnection.Dispose();
                    this._storeConnection = null;
                }
            }
            this.ChangeConnectionString(string.Empty);
            if (flag)
            {
                this.OnStateChange(DbConnectionInternal.StateChangeClosed);
            }
            base.Dispose(disposing);
        }

        public override void EnlistTransaction(Transaction transaction)
        {
            if (this._storeConnection == null)
            {
                throw EntityUtil.InvalidOperation(Strings.EntityClient_ConnectionStringNeededBeforeOperation);
            }
            if (this.State != ConnectionState.Open)
            {
                throw EntityUtil.InvalidOperation(Strings.EntityClient_ConnectionNotOpen);
            }
            try
            {
                this._storeConnection.EnlistTransaction(transaction);
            }
            catch (Exception exception)
            {
                if (EntityUtil.IsCatchableExceptionType(exception))
                {
                    throw EntityUtil.Provider("EnlistTransaction", exception);
                }
                throw;
            }
        }

        private bool EntityCloseHelper(bool fireEventOnStateChange, ConnectionState previousState)
        {
            bool flag = false;
            this._entityClientConnectionState = ConnectionState.Closed;
            if (previousState != ConnectionState.Open)
            {
                return flag;
            }
            if (fireEventOnStateChange)
            {
                this.OnStateChange(DbConnectionInternal.StateChangeClosed);
                return flag;
            }
            return true;
        }

        private static string FormatProviderString(string providerString) => 
            providerString.Trim().Replace("\"", "\\\"");

        private static string GetErrorMessageWorthyProviderName(System.Data.Common.DbProviderFactory factory)
        {
            string fullName;
            EntityUtil.CheckArgumentNull<System.Data.Common.DbProviderFactory>(factory, "factory");
            if (!EntityUtil.TryGetProviderInvariantName(factory, out fullName))
            {
                fullName = factory.GetType().FullName;
            }
            return fullName;
        }

        private System.Data.Common.DbProviderFactory GetFactory(string providerString)
        {
            System.Data.Common.DbProviderFactory factory;
            try
            {
                factory = DbProviderFactories.GetFactory(providerString);
            }
            catch (ArgumentException exception)
            {
                throw EntityUtil.Argument(Strings.EntityClient_InvalidStoreProvider, exception);
            }
            return factory;
        }

        public MetadataWorkspace GetMetadataWorkspace() => 
            this.GetMetadataWorkspace(true);

        internal MetadataWorkspace GetMetadataWorkspace(bool initializeAllCollections)
        {
            if ((this._metadataWorkspace == null) || (initializeAllCollections && !this._metadataWorkspace.IsItemCollectionAlreadyRegistered(DataSpace.SSpace)))
            {
                lock (this._connectionStringLock)
                {
                    EdmItemCollection edmItemCollection = null;
                    if (this._metadataWorkspace == null)
                    {
                        MetadataWorkspace workspace = new MetadataWorkspace();
                        this._artifactLoader = SplitPaths(this._effectiveConnectionOptions["metadata"]);
                        edmItemCollection = LoadEdmItemCollection(workspace, this._artifactLoader);
                        this._metadataWorkspace = workspace;
                    }
                    else
                    {
                        edmItemCollection = (EdmItemCollection) this._metadataWorkspace.GetItemCollection(DataSpace.CSpace);
                    }
                    if (initializeAllCollections && !this._metadataWorkspace.IsItemCollectionAlreadyRegistered(DataSpace.SSpace))
                    {
                        LoadStoreItemCollections(this._metadataWorkspace, this._storeConnection, this._providerFactory, this._effectiveConnectionOptions, edmItemCollection, this._artifactLoader);
                        this._artifactLoader = null;
                        this._initialized = true;
                    }
                }
            }
            return this._metadataWorkspace;
        }

        private DbConnection GetStoreConnection(System.Data.Common.DbProviderFactory factory)
        {
            DbConnection connection = factory.CreateConnection();
            if (connection == null)
            {
                throw EntityUtil.ProviderIncompatible(Strings.EntityClient_ReturnedNullOnProviderMethod("CreateConnection", factory.GetType().Name));
            }
            return connection;
        }

        private void InitializeMetadata(DbConnection newConnection, DbConnection originalConnection, bool closeOriginalConnectionOnFailure)
        {
            try
            {
                this.GetMetadataWorkspace();
            }
            catch (Exception exception)
            {
                if (EntityUtil.IsCatchableExceptionType(exception))
                {
                    this.ResetStoreConnection(newConnection, originalConnection, closeOriginalConnectionOnFailure);
                }
                throw;
            }
        }

        private static EdmItemCollection LoadEdmItemCollection(MetadataWorkspace workspace, MetadataArtifactLoader artifactLoader)
        {
            object obj2;
            EdmItemCollection collection = MetadataCache.GetOrCreateEdmItemCollection(CreateMetadataCacheKey(artifactLoader.GetOriginalPaths(DataSpace.CSpace), null, null), artifactLoader, out obj2);
            workspace.RegisterItemCollection(collection);
            workspace.AddMetadataEntryToken(obj2);
            return collection;
        }

        private static void LoadStoreItemCollections(MetadataWorkspace workspace, DbConnection storeConnection, System.Data.Common.DbProviderFactory factory, DbConnectionOptions connectionOptions, EdmItemCollection edmItemCollection, MetadataArtifactLoader artifactLoader)
        {
            object obj2;
            string connectionString = connectionOptions["provider connection string"];
            if (string.IsNullOrEmpty(connectionString) && (storeConnection != null))
            {
                connectionString = storeConnection.ConnectionString;
            }
            StorageMappingItemCollection collection = MetadataCache.GetOrCreateStoreAndMappingItemCollections(CreateMetadataCacheKey(artifactLoader.GetOriginalPaths(), connectionOptions["provider"], connectionString), artifactLoader, edmItemCollection, out obj2);
            ValidateThatConnectionAndMetadataProvidersAreEqual(factory, connectionOptions["provider"], collection.StoreItemCollection.StoreProviderFactory);
            workspace.RegisterItemCollection(collection.StoreItemCollection);
            workspace.RegisterItemCollection(collection);
            workspace.AddMetadataEntryToken(obj2);
        }

        public override void Open()
        {
            if (this._storeConnection == null)
            {
                throw EntityUtil.InvalidOperation(Strings.EntityClient_ConnectionStringNeededBeforeOperation);
            }
            if (this.State != ConnectionState.Closed)
            {
                throw EntityUtil.InvalidOperation(Strings.EntityClient_CannotReopenConnection);
            }
            bool closeStoreConnectionOnFailure = false;
            this.OpenStoreConnectionIf(this._storeConnection.State != ConnectionState.Open, this._storeConnection, null, "EntityClient_ProviderSpecificError", "Open", ref closeStoreConnectionOnFailure);
            if ((this._storeConnection == null) || (this._storeConnection.State != ConnectionState.Open))
            {
                throw EntityUtil.InvalidOperation(Strings.EntityClient_ConnectionNotOpen);
            }
            this.InitializeMetadata(this._storeConnection, this._storeConnection, closeStoreConnectionOnFailure);
            this.SetEntityClientConnectionStateToOpen();
        }

        private void OpenStoreConnectionIf(bool openCondition, DbConnection storeConnectionToOpen, DbConnection originalConnection, string exceptionCode, string attemptedOperation, ref bool closeStoreConnectionOnFailure)
        {
            try
            {
                if (openCondition)
                {
                    storeConnectionToOpen.Open();
                    closeStoreConnectionOnFailure = true;
                }
                this.ResetStoreConnection(storeConnectionToOpen, originalConnection, false);
                this.ClearCurrentTransaction();
            }
            catch (Exception exception)
            {
                if (EntityUtil.IsCatchableExceptionType(exception))
                {
                    string message = string.IsNullOrEmpty(attemptedOperation) ? EntityRes.GetString(exceptionCode) : EntityRes.GetString(exceptionCode, new object[] { attemptedOperation });
                    throw EntityUtil.ProviderExceptionWithMessage(message, exception);
                }
                throw;
            }
        }

        private void ResetStoreConnection(DbConnection newConnection, DbConnection originalConnection, bool closeOriginalConnection)
        {
            this._storeConnection = newConnection;
            if (closeOriginalConnection && (originalConnection != null))
            {
                originalConnection.Close();
            }
        }

        private void SetEntityClientConnectionStateToOpen()
        {
            this._entityClientConnectionState = ConnectionState.Open;
            this.OnStateChange(DbConnectionInternal.StateChangeOpen);
        }

        private static MetadataArtifactLoader SplitPaths(string paths)
        {
            HashSet<string> uriRegistry = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            List<MetadataArtifactLoader> allCollections = new List<MetadataArtifactLoader>();
            if (!string.IsNullOrEmpty(paths))
            {
                List<string> list2 = new List<string>();
                for (int i = paths.IndexOf("|datadirectory|", StringComparison.OrdinalIgnoreCase); i != -1; i = paths.IndexOf("|datadirectory|", StringComparison.OrdinalIgnoreCase))
                {
                    int num2 = (i == 0) ? -1 : paths.LastIndexOf("|", i - 1, StringComparison.Ordinal);
                    int startIndex = num2 + 1;
                    int num4 = paths.IndexOf("|", i + "|datadirectory|".Length, StringComparison.Ordinal);
                    if (num4 == -1)
                    {
                        list2.Add(paths.Substring(startIndex));
                        paths = paths.Remove(startIndex);
                        break;
                    }
                    list2.Add(paths.Substring(startIndex, num4 - startIndex));
                    paths = paths.Remove(startIndex, num4 - startIndex);
                }
                string[] collection = paths.Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
                if (list2.Count > 0)
                {
                    list2.AddRange(collection);
                    collection = list2.ToArray();
                }
                for (int j = 0; j < collection.Length; j++)
                {
                    collection[j] = collection[j].Trim();
                    if (collection[j].Length > 0)
                    {
                        allCollections.Add(MetadataArtifactLoader.Create(collection[j], MetadataArtifactLoader.ExtensionCheck.All, null, uriRegistry));
                    }
                }
            }
            return MetadataArtifactLoader.Create(allCollections);
        }

        private void StoreCloseHelper()
        {
            try
            {
                if ((this._storeConnection != null) && (this._storeConnection.State != ConnectionState.Closed))
                {
                    this._storeConnection.Close();
                }
                this.ClearCurrentTransaction();
            }
            catch (Exception exception)
            {
                if (EntityUtil.IsCatchableExceptionType(exception))
                {
                    throw EntityUtil.ProviderExceptionWithMessage(Strings.EntityClient_ErrorInClosingConnection, exception);
                }
                throw;
            }
        }

        private void ValidateChangesPermitted()
        {
            if (this._initialized)
            {
                throw EntityUtil.InvalidOperation(Strings.EntityClient_SettingsCannotBeChangedOnOpenConnection);
            }
        }

        private static void ValidateThatConnectionAndMetadataProvidersAreEqual(System.Data.Common.DbProviderFactory connectionFactory, string connectionProviderName, System.Data.Common.DbProviderFactory metadataFactory)
        {
            if (metadataFactory.GetType() != connectionFactory.GetType())
            {
                string errorMessageWorthyProviderName = GetErrorMessageWorthyProviderName(metadataFactory);
                if (string.IsNullOrEmpty(connectionProviderName))
                {
                    connectionProviderName = GetErrorMessageWorthyProviderName(connectionFactory);
                }
                throw EntityUtil.InvalidOperation(Strings.EntityClient_ConnectionAndMetadataProviderMismatch(errorMessageWorthyProviderName, connectionProviderName));
            }
        }

        private static string ValidateValueForTheKeyword(DbConnectionOptions effectiveConnectionOptions, string keywordName)
        {
            string str = effectiveConnectionOptions[keywordName];
            if (!string.IsNullOrEmpty(str))
            {
                str = str.Trim();
            }
            if (string.IsNullOrEmpty(str))
            {
                throw EntityUtil.Argument(Strings.EntityClient_ConnectionStringMissingInfo(keywordName));
            }
            return str;
        }

        public override string ConnectionString
        {
            get
            {
                if (this._userConnectionOptions == null)
                {
                    string str;
                    if (!EntityUtil.TryGetProviderInvariantName(this._storeConnection.ProviderFactory, out str))
                    {
                        str = "";
                    }
                    return string.Format(CultureInfo.InvariantCulture, "{0}={3};{1}={4};{2}=\"{5}\";", new object[] { "metadata id", "provider", "provider connection string", this._metadataID.ToString(), str, FormatProviderString(this._storeConnection.ConnectionString) });
                }
                string connectionString = this._userConnectionOptions.UsersConnectionString(false);
                if (!object.ReferenceEquals(this._userConnectionOptions, this._effectiveConnectionOptions) || (this._storeConnection == null))
                {
                    return connectionString;
                }
                string str3 = null;
                try
                {
                    str3 = this._storeConnection.ConnectionString;
                }
                catch (Exception exception)
                {
                    if (EntityUtil.IsCatchableExceptionType(exception))
                    {
                        throw EntityUtil.Provider("ConnectionString", exception);
                    }
                    throw;
                }
                string str4 = this._userConnectionOptions["provider connection string"];
                if ((str3 == str4) || (string.IsNullOrEmpty(str3) && string.IsNullOrEmpty(str4)))
                {
                    return connectionString;
                }
                EntityConnectionStringBuilder builder = new EntityConnectionStringBuilder(connectionString) {
                    ProviderConnectionString = str3
                };
                return builder.ConnectionString;
            }
            set
            {
                this.ValidateChangesPermitted();
                this.ChangeConnectionString(value);
            }
        }

        public override int ConnectionTimeout
        {
            get
            {
                int connectionTimeout;
                if (this._storeConnection == null)
                {
                    return 0;
                }
                try
                {
                    connectionTimeout = this._storeConnection.ConnectionTimeout;
                }
                catch (Exception exception)
                {
                    if (EntityUtil.IsCatchableExceptionType(exception))
                    {
                        throw EntityUtil.Provider("ConnectionTimeout", exception);
                    }
                    throw;
                }
                return connectionTimeout;
            }
        }

        internal EntityTransaction CurrentTransaction
        {
            get
            {
                if ((this._currentTransaction != null) && ((this._currentTransaction.StoreTransaction.Connection == null) || (this.State == ConnectionState.Closed)))
                {
                    this.ClearCurrentTransaction();
                }
                return this._currentTransaction;
            }
        }

        public override string Database =>
            string.Empty;

        public override string DataSource
        {
            get
            {
                string dataSource;
                if (this._storeConnection == null)
                {
                    return string.Empty;
                }
                try
                {
                    dataSource = this._storeConnection.DataSource;
                }
                catch (Exception exception)
                {
                    if (EntityUtil.IsCatchableExceptionType(exception))
                    {
                        throw EntityUtil.Provider("DataSource", exception);
                    }
                    throw;
                }
                return dataSource;
            }
        }

        protected override System.Data.Common.DbProviderFactory DbProviderFactory =>
            EntityProviderFactory.Instance;

        public override string ServerVersion
        {
            get
            {
                string serverVersion;
                if (this._storeConnection == null)
                {
                    throw EntityUtil.InvalidOperation(Strings.EntityClient_ConnectionStringNeededBeforeOperation);
                }
                if (this.State != ConnectionState.Open)
                {
                    throw EntityUtil.InvalidOperation(Strings.EntityClient_ConnectionNotOpen);
                }
                try
                {
                    serverVersion = this._storeConnection.ServerVersion;
                }
                catch (Exception exception)
                {
                    if (EntityUtil.IsCatchableExceptionType(exception))
                    {
                        throw EntityUtil.Provider("ServerVersion", exception);
                    }
                    throw;
                }
                return serverVersion;
            }
        }

        public override ConnectionState State
        {
            get
            {
                ConnectionState state;
                try
                {
                    if ((this._entityClientConnectionState == ConnectionState.Open) && (this.StoreConnection.State != ConnectionState.Open))
                    {
                        return ConnectionState.Broken;
                    }
                    state = this._entityClientConnectionState;
                }
                catch (Exception exception)
                {
                    if (EntityUtil.IsCatchableExceptionType(exception))
                    {
                        throw EntityUtil.Provider("State", exception);
                    }
                    throw;
                }
                return state;
            }
        }

        public DbConnection StoreConnection =>
            this._storeConnection;

        internal System.Data.Common.DbProviderFactory StoreProviderFactory =>
            this._providerFactory;
    }
}

