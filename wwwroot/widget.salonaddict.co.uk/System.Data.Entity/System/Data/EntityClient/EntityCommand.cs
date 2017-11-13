namespace System.Data.EntityClient
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data;
    using System.Data.Common;
    using System.Data.Common.CommandTrees;
    using System.Data.Common.EntitySql;
    using System.Data.Common.QueryCache;
    using System.Data.Common.Utils;
    using System.Data.Entity;
    using System.Data.Metadata.Edm;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    public sealed class EntityCommand : DbCommand
    {
        private EntityCommandDefinition _commandDefinition;
        private int? _commandTimeout;
        private System.Data.CommandType _commandType;
        private EntityConnection _connection;
        private DbDataReader _dataReader;
        private bool _designTimeVisible;
        private bool _enableQueryPlanCaching;
        private string _esqlStatement;
        private EntityParameterCollection _parameters;
        private DbCommandTree _preparedCommandTree;
        private DbCommand _storeProviderCommand;
        private EntityTransaction _transaction;
        private UpdateRowSource _updatedRowSource;
        private const int InvalidCloseCount = -1;

        internal event EventHandler OnDataReaderClosing;

        public EntityCommand()
        {
            GC.SuppressFinalize(this);
            this._designTimeVisible = true;
            this._commandType = System.Data.CommandType.Text;
            this._updatedRowSource = UpdateRowSource.Both;
            this._parameters = new EntityParameterCollection();
            this._enableQueryPlanCaching = true;
        }

        internal EntityCommand(EntityCommandDefinition commandDefinition) : this()
        {
            this._commandDefinition = commandDefinition;
            this._parameters = new EntityParameterCollection();
            foreach (EntityParameter parameter in commandDefinition.Parameters)
            {
                this._parameters.Add(parameter.Clone());
            }
            this._parameters.ResetIsDirty();
        }

        public EntityCommand(string statement) : this()
        {
            this._esqlStatement = statement;
        }

        internal EntityCommand(EntityConnection connection, EntityCommandDefinition entityCommandDefinition) : this()
        {
            this._commandDefinition = entityCommandDefinition;
            this._parameters = new EntityParameterCollection();
            foreach (EntityParameter parameter in entityCommandDefinition.Parameters)
            {
                this._parameters.Add(parameter.Clone());
            }
            this._parameters.ResetIsDirty();
            this._connection = connection;
        }

        public EntityCommand(string statement, EntityConnection connection) : this(statement)
        {
            this._connection = connection;
        }

        public EntityCommand(string statement, EntityConnection connection, EntityTransaction transaction) : this(statement, connection)
        {
            this._transaction = transaction;
        }

        public override void Cancel()
        {
        }

        private void CheckConnectionPresent()
        {
            if (this._connection == null)
            {
                throw EntityUtil.InvalidOperation(Strings.EntityClient_NoConnectionForCommand);
            }
        }

        private void CheckIfReadyToPrepare()
        {
            this.CheckConnectionPresent();
            if ((this._connection.StoreProviderFactory == null) || (this._connection.StoreConnection == null))
            {
                throw EntityUtil.InvalidOperation(Strings.EntityClient_ConnectionStringNeededBeforeOperation);
            }
            if ((this._connection.State == ConnectionState.Closed) || (this._connection.State == ConnectionState.Broken))
            {
                throw EntityUtil.InvalidOperation(Strings.EntityClient_ExecutingOnClosedConnection((this._connection.State == ConnectionState.Closed) ? Strings.EntityClient_ConnectionStateClosed : Strings.EntityClient_ConnectionStateBroken));
            }
        }

        private EntityCommandDefinition CreateCommandDefinition()
        {
            this.MakeCommandTree();
            if (!this._preparedCommandTree.MetadataWorkspace.IsMetadataWorkspaceCSCompatible(this.Connection.GetMetadataWorkspace()))
            {
                throw EntityUtil.InvalidOperation(Strings.EntityClient_CommandTreeMetadataIncompatible);
            }
            return EntityProviderServices.Instance.CreateCommandDefinition(this._connection.StoreProviderFactory, this._preparedCommandTree);
        }

        protected override DbParameter CreateDbParameter() => 
            this.CreateParameter();

        public EntityParameter CreateParameter() => 
            new EntityParameter();

        private EdmFunction DetermineFunctionImport()
        {
            string str;
            string str2;
            if (string.IsNullOrEmpty(this.CommandText) || string.IsNullOrEmpty(this.CommandText.Trim()))
            {
                throw EntityUtil.InvalidOperation(Strings.EntityClient_FunctionImportEmptyCommandText);
            }
            this._connection.GetMetadataWorkspace();
            string defaultContainerName = null;
            CommandHelper.ParseFunctionImportCommandText(this.CommandText, defaultContainerName, out str, out str2);
            return CommandHelper.FindFunctionImport(this._connection.GetMetadataWorkspace(), str, str2);
        }

        protected override DbDataReader ExecuteDbDataReader(CommandBehavior behavior) => 
            this.ExecuteReader(behavior);

        public override int ExecuteNonQuery() => 
            this.ExecuteScalar<int>(delegate (DbDataReader reader) {
                CommandHelper.ConsumeReader(reader);
                return reader.RecordsAffected;
            });

        public EntityDataReader ExecuteReader() => 
            this.ExecuteReader(CommandBehavior.Default);

        public EntityDataReader ExecuteReader(CommandBehavior behavior)
        {
            this.Prepare();
            EntityDataReader reader = new EntityDataReader(this, this._commandDefinition.Execute(this, behavior), behavior);
            this._dataReader = reader;
            return reader;
        }

        public override object ExecuteScalar() => 
            this.ExecuteScalar<object>(delegate (DbDataReader reader) {
                object obj2 = reader.Read() ? reader.GetValue(0) : null;
                CommandHelper.ConsumeReader(reader);
                return obj2;
            });

        private T_Result ExecuteScalar<T_Result>(Func<DbDataReader, T_Result> resultSelector)
        {
            using (EntityDataReader reader = this.ExecuteReader(CommandBehavior.SequentialAccess))
            {
                return resultSelector(reader);
            }
        }

        internal EntityCommandDefinition GetCommandDefinition()
        {
            EntityCommandDefinition entityCommandDefinition = this._commandDefinition;
            if (entityCommandDefinition == null)
            {
                if (!this.TryGetEntityCommandDefinitionFromQueryCache(out entityCommandDefinition))
                {
                    entityCommandDefinition = this.CreateCommandDefinition();
                }
                this._commandDefinition = entityCommandDefinition;
            }
            return entityCommandDefinition;
        }

        internal Dictionary<string, TypeUsage> GetParameterTypeUsage()
        {
            Dictionary<string, TypeUsage> dictionary = new Dictionary<string, TypeUsage>(this._parameters.Count);
            foreach (EntityParameter parameter in this._parameters)
            {
                string parameterName = parameter.ParameterName;
                if (string.IsNullOrEmpty(parameterName))
                {
                    throw EntityUtil.InvalidOperation(Strings.EntityClient_EmptyParameterName);
                }
                if ((this.CommandType == System.Data.CommandType.Text) && (parameter.Direction != ParameterDirection.Input))
                {
                    throw EntityUtil.InvalidOperation(Strings.EntityClient_InvalidParameterDirection(parameter.ParameterName));
                }
                if ((parameter.DbType == DbType.Object) && ((parameter.Value == null) || (parameter.Value is DBNull)))
                {
                    throw EntityUtil.InvalidOperation(Strings.EntityClient_UnknownParameterType(parameterName));
                }
                TypeUsage typeUsage = null;
                typeUsage = parameter.GetTypeUsage(this._connection.GetMetadataWorkspace());
                try
                {
                    dictionary.Add(parameterName, typeUsage);
                }
                catch (ArgumentException exception)
                {
                    throw EntityUtil.InvalidOperation(Strings.EntityClient_DuplicateParameterNames(parameter.ParameterName), exception);
                }
            }
            return dictionary;
        }

        private void InnerPrepare()
        {
            if (this._parameters.IsDirty)
            {
                this.Unprepare();
            }
            this._commandDefinition = this.GetCommandDefinition();
        }

        private void MakeCommandTree()
        {
            if (this._preparedCommandTree == null)
            {
                DbCommandTree tree = null;
                if (System.Data.CommandType.Text == this.CommandType)
                {
                    if (string.IsNullOrEmpty(this._esqlStatement))
                    {
                        throw EntityUtil.InvalidOperation(Strings.EntityClient_NoCommandText);
                    }
                    ParserOptions parserOptions = new ParserOptions();
                    Perspective perspective = new ModelPerspective(this._connection.GetMetadataWorkspace());
                    Dictionary<string, TypeUsage> parameterTypeUsage = this.GetParameterTypeUsage();
                    tree = CqlQuery.Compile(this._esqlStatement, perspective, parserOptions, parameterTypeUsage);
                }
                else if (System.Data.CommandType.StoredProcedure == this.CommandType)
                {
                    EdmFunction edmFunction = this.DetermineFunctionImport();
                    tree = new DbFunctionCommandTree(this.Connection.GetMetadataWorkspace(), DataSpace.CSpace, edmFunction, null);
                    foreach (KeyValuePair<string, TypeUsage> pair in this.GetParameterTypeUsage())
                    {
                        tree.AddParameter(pair.Key, pair.Value);
                    }
                }
                this._preparedCommandTree = tree;
            }
        }

        internal void NotifyDataReaderClosing()
        {
            this._dataReader = null;
            if (this._storeProviderCommand != null)
            {
                CommandHelper.SetEntityParameterValues(this, this._storeProviderCommand);
                this._storeProviderCommand = null;
            }
            if (this.OnDataReaderClosing != null)
            {
                this.OnDataReaderClosing(this, new EventArgs());
            }
        }

        public override void Prepare()
        {
            this.ThrowIfDataReaderIsOpen();
            this.CheckIfReadyToPrepare();
            this.InnerPrepare();
        }

        internal void SetStoreProviderCommand(DbCommand storeProviderCommand)
        {
            this._storeProviderCommand = storeProviderCommand;
        }

        private void ThrowIfDataReaderIsOpen()
        {
            if (this._dataReader != null)
            {
                throw EntityUtil.InvalidOperation(Strings.EntityClient_DataReaderIsStillOpen);
            }
        }

        [Browsable(false)]
        public string ToTraceString()
        {
            this.CheckConnectionPresent();
            this.InnerPrepare();
            EntityCommandDefinition definition = this._commandDefinition;
            if (definition != null)
            {
                return definition.ToTraceString();
            }
            return string.Empty;
        }

        private bool TryGetEntityCommandDefinitionFromQueryCache(out EntityCommandDefinition entityCommandDefinition)
        {
            entityCommandDefinition = null;
            if (!this._enableQueryPlanCaching || string.IsNullOrEmpty(this._esqlStatement))
            {
                return false;
            }
            EntityClientCacheKey entityClientCacheKey = new EntityClientCacheKey(this);
            QueryCacheManager queryCacheManager = this._connection.GetMetadataWorkspace().GetQueryCacheManager();
            if (!queryCacheManager.TryCacheLookup(entityClientCacheKey, out entityCommandDefinition))
            {
                entityCommandDefinition = this.CreateCommandDefinition();
                QueryCacheEntry outQueryCacheEntry = null;
                if (queryCacheManager.TryLookupAndAdd(new EntityClientCacheEntry(entityClientCacheKey, entityCommandDefinition), out outQueryCacheEntry))
                {
                    entityCommandDefinition = (EntityCommandDefinition) outQueryCacheEntry.GetTarget();
                }
            }
            return true;
        }

        internal void Unprepare()
        {
            this._commandDefinition = null;
            this._preparedCommandTree = null;
            this._parameters.ResetIsDirty();
        }

        public override string CommandText
        {
            get => 
                (this._esqlStatement ?? "");
            set
            {
                this.ThrowIfDataReaderIsOpen();
                if (this._esqlStatement != value)
                {
                    this._esqlStatement = value;
                    this.Unprepare();
                }
            }
        }

        public override int CommandTimeout
        {
            get
            {
                if (this._commandTimeout.HasValue)
                {
                    return this._commandTimeout.Value;
                }
                if ((this._connection != null) && (this._connection.StoreProviderFactory != null))
                {
                    DbCommand command = this._connection.StoreProviderFactory.CreateCommand();
                    if (command != null)
                    {
                        return command.CommandTimeout;
                    }
                }
                return 0;
            }
            set
            {
                this.ThrowIfDataReaderIsOpen();
                this._commandTimeout = new int?(value);
            }
        }

        public override System.Data.CommandType CommandType
        {
            get => 
                this._commandType;
            set
            {
                this.ThrowIfDataReaderIsOpen();
                if ((value != System.Data.CommandType.Text) && (value != System.Data.CommandType.StoredProcedure))
                {
                    throw EntityUtil.NotSupported(Strings.EntityClient_UnsupportedCommandType);
                }
                this._commandType = value;
            }
        }

        public EntityConnection Connection
        {
            get => 
                this._connection;
            set
            {
                this.ThrowIfDataReaderIsOpen();
                if (this._connection != value)
                {
                    if (this._connection != null)
                    {
                        this.Unprepare();
                    }
                    this._connection = value;
                    this._transaction = null;
                }
            }
        }

        protected override System.Data.Common.DbConnection DbConnection
        {
            get => 
                this.Connection;
            set
            {
                this.Connection = (EntityConnection) value;
            }
        }

        protected override System.Data.Common.DbParameterCollection DbParameterCollection =>
            this.Parameters;

        protected override System.Data.Common.DbTransaction DbTransaction
        {
            get => 
                this.Transaction;
            set
            {
                this.Transaction = (EntityTransaction) value;
            }
        }

        public override bool DesignTimeVisible
        {
            get => 
                this._designTimeVisible;
            set
            {
                this.ThrowIfDataReaderIsOpen();
                this._designTimeVisible = value;
                TypeDescriptor.Refresh(this);
            }
        }

        public bool EnablePlanCaching
        {
            get => 
                this._enableQueryPlanCaching;
            set
            {
                this.ThrowIfDataReaderIsOpen();
                this._enableQueryPlanCaching = value;
            }
        }

        public EntityParameterCollection Parameters =>
            this._parameters;

        public EntityTransaction Transaction
        {
            get => 
                this._transaction;
            set
            {
                this.ThrowIfDataReaderIsOpen();
                this._transaction = value;
            }
        }

        public override UpdateRowSource UpdatedRowSource
        {
            get => 
                this._updatedRowSource;
            set
            {
                this.ThrowIfDataReaderIsOpen();
                this._updatedRowSource = value;
            }
        }
    }
}

