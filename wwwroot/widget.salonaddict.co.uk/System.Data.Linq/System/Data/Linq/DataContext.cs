namespace System.Data.Linq
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Data.Linq.Mapping;
    using System.Data.Linq.Provider;
    using System.IO;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Transactions;

    public class DataContext : IDisposable
    {
        private static MethodInfo _miExecuteQuery;
        private ChangeConflictCollection conflicts;
        private bool deferredLoadingEnabled;
        private bool disposed;
        private bool isInSubmitChanges;
        private DataLoadOptions loadOptions;
        private bool objectTrackingEnabled;
        private IProvider provider;
        private CommonDataServices services;
        private Dictionary<MetaTable, ITable> tables;

        private DataContext()
        {
            this.objectTrackingEnabled = true;
            this.deferredLoadingEnabled = true;
        }

        public DataContext(IDbConnection connection)
        {
            this.objectTrackingEnabled = true;
            this.deferredLoadingEnabled = true;
            if (connection == null)
            {
                throw System.Data.Linq.Error.ArgumentNull("connection");
            }
            this.InitWithDefaultMapping(connection);
        }

        internal DataContext(DataContext context)
        {
            this.objectTrackingEnabled = true;
            this.deferredLoadingEnabled = true;
            if (context == null)
            {
                throw System.Data.Linq.Error.ArgumentNull("context");
            }
            this.Init(context.Connection, context.Mapping.MappingSource);
            this.LoadOptions = context.LoadOptions;
            this.Transaction = context.Transaction;
            this.Log = context.Log;
            this.CommandTimeout = context.CommandTimeout;
        }

        public DataContext(string fileOrServerOrConnection)
        {
            this.objectTrackingEnabled = true;
            this.deferredLoadingEnabled = true;
            if (fileOrServerOrConnection == null)
            {
                throw System.Data.Linq.Error.ArgumentNull("fileOrServerOrConnection");
            }
            this.InitWithDefaultMapping(fileOrServerOrConnection);
        }

        public DataContext(IDbConnection connection, MappingSource mapping)
        {
            this.objectTrackingEnabled = true;
            this.deferredLoadingEnabled = true;
            if (connection == null)
            {
                throw System.Data.Linq.Error.ArgumentNull("connection");
            }
            if (mapping == null)
            {
                throw System.Data.Linq.Error.ArgumentNull("mapping");
            }
            this.Init(connection, mapping);
        }

        public DataContext(string fileOrServerOrConnection, MappingSource mapping)
        {
            this.objectTrackingEnabled = true;
            this.deferredLoadingEnabled = true;
            if (fileOrServerOrConnection == null)
            {
                throw System.Data.Linq.Error.ArgumentNull("fileOrServerOrConnection");
            }
            if (mapping == null)
            {
                throw System.Data.Linq.Error.ArgumentNull("mapping");
            }
            this.Init(fileOrServerOrConnection, mapping);
        }

        private void AcceptChanges()
        {
            this.CheckDispose();
            this.VerifyTrackingEnabled();
            this.services.ChangeTracker.AcceptChanges();
        }

        internal void CheckDispose()
        {
            if (this.disposed)
            {
                throw System.Data.Linq.Error.DataContextCannotBeUsedAfterDispose();
            }
        }

        internal void CheckInSubmitChanges()
        {
            this.CheckDispose();
            if (!this.isInSubmitChanges)
            {
                throw System.Data.Linq.Error.CannotPerformOperationOutsideSubmitChanges();
            }
        }

        internal void CheckNotInSubmitChanges()
        {
            this.CheckDispose();
            if (this.isInSubmitChanges)
            {
                throw System.Data.Linq.Error.CannotPerformOperationDuringSubmitChanges();
            }
        }

        internal void ClearCache()
        {
            this.CheckDispose();
            this.services.ResetServices();
        }

        internal object Clone()
        {
            this.CheckDispose();
            return Activator.CreateInstance(base.GetType(), new object[] { this.Connection, this.Mapping.MappingSource });
        }

        public void CreateDatabase()
        {
            this.CheckDispose();
            this.provider.CreateDatabase();
        }

        protected internal IQueryable<TResult> CreateMethodCallQuery<TResult>(object instance, MethodInfo methodInfo, params object[] parameters)
        {
            this.CheckDispose();
            if (instance == null)
            {
                throw System.Data.Linq.Error.ArgumentNull("instance");
            }
            if (methodInfo == null)
            {
                throw System.Data.Linq.Error.ArgumentNull("methodInfo");
            }
            if (parameters == null)
            {
                throw System.Data.Linq.Error.ArgumentNull("parameters");
            }
            if (!typeof(IQueryable<TResult>).IsAssignableFrom(methodInfo.ReturnType))
            {
                throw System.Data.Linq.Error.ExpectedQueryableArgument("methodInfo", typeof(IQueryable<TResult>));
            }
            return new DataQuery<TResult>(this, this.GetMethodCall(instance, methodInfo, parameters));
        }

        internal DataContext CreateRefreshContext()
        {
            this.CheckDispose();
            return new DataContext(this);
        }

        public bool DatabaseExists()
        {
            this.CheckDispose();
            return this.provider.DatabaseExists();
        }

        public void DeleteDatabase()
        {
            this.CheckDispose();
            this.provider.DeleteDatabase();
        }

        public void Dispose()
        {
            this.disposed = true;
            this.Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.provider != null)
                {
                    this.provider.Dispose();
                    this.provider = null;
                }
                this.services = null;
                this.tables = null;
                this.loadOptions = null;
            }
        }

        public int ExecuteCommand(string command, params object[] parameters)
        {
            this.CheckDispose();
            if (command == null)
            {
                throw System.Data.Linq.Error.ArgumentNull("command");
            }
            if (parameters == null)
            {
                throw System.Data.Linq.Error.ArgumentNull("parameters");
            }
            return (int) this.ExecuteMethodCall(this, (MethodInfo) MethodBase.GetCurrentMethod(), new object[] { command, parameters }).ReturnValue;
        }

        protected internal void ExecuteDynamicDelete(object entity)
        {
            this.CheckDispose();
            if (entity == null)
            {
                throw System.Data.Linq.Error.ArgumentNull("entity");
            }
            this.CheckInSubmitChanges();
            TrackedObject trackedObject = this.services.ChangeTracker.GetTrackedObject(entity);
            if (trackedObject == null)
            {
                throw System.Data.Linq.Error.CannotPerformOperationForUntrackedObject();
            }
            if (this.services.ChangeDirector.DynamicDelete(trackedObject) == 0)
            {
                throw new ChangeConflictException();
            }
        }

        protected internal void ExecuteDynamicInsert(object entity)
        {
            this.CheckDispose();
            if (entity == null)
            {
                throw System.Data.Linq.Error.ArgumentNull("entity");
            }
            this.CheckInSubmitChanges();
            TrackedObject trackedObject = this.services.ChangeTracker.GetTrackedObject(entity);
            if (trackedObject == null)
            {
                throw System.Data.Linq.Error.CannotPerformOperationForUntrackedObject();
            }
            this.services.ChangeDirector.DynamicInsert(trackedObject);
        }

        protected internal void ExecuteDynamicUpdate(object entity)
        {
            this.CheckDispose();
            if (entity == null)
            {
                throw System.Data.Linq.Error.ArgumentNull("entity");
            }
            this.CheckInSubmitChanges();
            TrackedObject trackedObject = this.services.ChangeTracker.GetTrackedObject(entity);
            if (trackedObject == null)
            {
                throw System.Data.Linq.Error.CannotPerformOperationForUntrackedObject();
            }
            if (this.services.ChangeDirector.DynamicUpdate(trackedObject) == 0)
            {
                throw new ChangeConflictException();
            }
        }

        protected internal IExecuteResult ExecuteMethodCall(object instance, MethodInfo methodInfo, params object[] parameters)
        {
            this.CheckDispose();
            if (instance == null)
            {
                throw System.Data.Linq.Error.ArgumentNull("instance");
            }
            if (methodInfo == null)
            {
                throw System.Data.Linq.Error.ArgumentNull("methodInfo");
            }
            if (parameters == null)
            {
                throw System.Data.Linq.Error.ArgumentNull("parameters");
            }
            return this.provider.Execute(this.GetMethodCall(instance, methodInfo, parameters));
        }

        public IEnumerable<TResult> ExecuteQuery<TResult>(string query, params object[] parameters)
        {
            this.CheckDispose();
            if (query == null)
            {
                throw System.Data.Linq.Error.ArgumentNull("query");
            }
            if (parameters == null)
            {
                throw System.Data.Linq.Error.ArgumentNull("parameters");
            }
            return (IEnumerable<TResult>) this.ExecuteMethodCall(this, ((MethodInfo) MethodBase.GetCurrentMethod()).MakeGenericMethod(new Type[] { typeof(TResult) }), new object[] { query, parameters }).ReturnValue;
        }

        public IEnumerable ExecuteQuery(Type elementType, string query, params object[] parameters)
        {
            this.CheckDispose();
            if (elementType == null)
            {
                throw System.Data.Linq.Error.ArgumentNull("elementType");
            }
            if (query == null)
            {
                throw System.Data.Linq.Error.ArgumentNull("query");
            }
            if (parameters == null)
            {
                throw System.Data.Linq.Error.ArgumentNull("parameters");
            }
            if (_miExecuteQuery == null)
            {
                _miExecuteQuery = typeof(DataContext).GetMethods().Single<MethodInfo>(m => (m.Name == "ExecuteQuery") && (m.GetParameters().Length == 2));
            }
            return (IEnumerable) this.ExecuteMethodCall(this, _miExecuteQuery.MakeGenericMethod(new Type[] { elementType }), new object[] { query, parameters }).ReturnValue;
        }

        public ChangeSet GetChangeSet()
        {
            this.CheckDispose();
            return new ChangeProcessor(this.services, this).GetChangeSet();
        }

        internal string GetChangeText()
        {
            this.CheckDispose();
            this.VerifyTrackingEnabled();
            return new ChangeProcessor(this.services, this).GetChangeText();
        }

        public DbCommand GetCommand(IQueryable query)
        {
            this.CheckDispose();
            if (query == null)
            {
                throw System.Data.Linq.Error.ArgumentNull("query");
            }
            return this.provider.GetCommand(query.Expression);
        }

        private Expression GetMethodCall(object instance, MethodInfo methodInfo, params object[] parameters)
        {
            this.CheckDispose();
            if (parameters.Length <= 0)
            {
                return Expression.Call(Expression.Constant(instance), methodInfo);
            }
            ParameterInfo[] infoArray = methodInfo.GetParameters();
            List<Expression> arguments = new List<Expression>(parameters.Length);
            int index = 0;
            int length = parameters.Length;
            while (index < length)
            {
                Type parameterType = infoArray[index].ParameterType;
                if (parameterType.IsByRef)
                {
                    parameterType = parameterType.GetElementType();
                }
                arguments.Add(Expression.Constant(parameters[index], parameterType));
                index++;
            }
            return Expression.Call(Expression.Constant(instance), methodInfo, arguments);
        }

        internal string GetQueryText(IQueryable query)
        {
            this.CheckDispose();
            if (query == null)
            {
                throw System.Data.Linq.Error.ArgumentNull("query");
            }
            return this.provider.GetQueryText(query.Expression);
        }

        public Table<TEntity> GetTable<TEntity>() where TEntity: class
        {
            this.CheckDispose();
            MetaTable metaTable = this.services.Model.GetTable(typeof(TEntity));
            if (metaTable == null)
            {
                throw System.Data.Linq.Error.TypeIsNotMarkedAsTable(typeof(TEntity));
            }
            ITable table = this.GetTable(metaTable);
            if (table.ElementType != typeof(TEntity))
            {
                throw System.Data.Linq.Error.CouldNotGetTableForSubtype(typeof(TEntity), metaTable.RowType.Type);
            }
            return (Table<TEntity>) table;
        }

        private ITable GetTable(MetaTable metaTable)
        {
            ITable table;
            if (!this.tables.TryGetValue(metaTable, out table))
            {
                ValidateTable(metaTable);
                table = (ITable) Activator.CreateInstance(typeof(Table<>).MakeGenericType(new Type[] { metaTable.RowType.Type }), BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance, null, new object[] { this, metaTable }, null);
                this.tables.Add(metaTable, table);
            }
            return table;
        }

        public ITable GetTable(Type type)
        {
            this.CheckDispose();
            if (type == null)
            {
                throw System.Data.Linq.Error.ArgumentNull("type");
            }
            MetaTable metaTable = this.services.Model.GetTable(type);
            if (metaTable == null)
            {
                throw System.Data.Linq.Error.TypeIsNotMarkedAsTable(type);
            }
            if (metaTable.RowType.Type != type)
            {
                throw System.Data.Linq.Error.CouldNotGetTableForSubtype(type, metaTable.RowType.Type);
            }
            return this.GetTable(metaTable);
        }

        private void Init(object connection, MappingSource mapping)
        {
            MetaModel model = mapping.GetModel(base.GetType());
            this.services = new CommonDataServices(this, model);
            this.conflicts = new ChangeConflictCollection();
            if (model.ProviderType == null)
            {
                throw System.Data.Linq.Error.ProviderTypeNull();
            }
            Type providerType = model.ProviderType;
            if (!typeof(IProvider).IsAssignableFrom(providerType))
            {
                throw System.Data.Linq.Error.ProviderDoesNotImplementRequiredInterface(providerType, typeof(IProvider));
            }
            this.provider = (IProvider) Activator.CreateInstance(providerType);
            this.provider.Initialize(this.services, connection);
            this.tables = new Dictionary<MetaTable, ITable>();
            this.InitTables(this);
        }

        private void InitTables(object schema)
        {
            foreach (FieldInfo info in schema.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance))
            {
                Type fieldType = info.FieldType;
                if ((fieldType.IsGenericType && (fieldType.GetGenericTypeDefinition() == typeof(Table<>))) && (((ITable) info.GetValue(schema)) == null))
                {
                    Type type = fieldType.GetGenericArguments()[0];
                    ITable table = this.GetTable(type);
                    info.SetValue(schema, table);
                }
            }
        }

        private void InitWithDefaultMapping(object connection)
        {
            this.Init(connection, new AttributeMappingSource());
        }

        public void Refresh(RefreshMode mode, params object[] entities)
        {
            this.CheckDispose();
            if (entities == null)
            {
                throw System.Data.Linq.Error.ArgumentNull("entities");
            }
            this.Refresh(mode, (IEnumerable) entities);
        }

        public void Refresh(RefreshMode mode, IEnumerable entities)
        {
            this.CheckDispose();
            this.CheckNotInSubmitChanges();
            this.VerifyTrackingEnabled();
            if (entities == null)
            {
                throw System.Data.Linq.Error.ArgumentNull("entities");
            }
            List<object> list = entities.Cast<object>().ToList<object>();
            DataContext context = this.CreateRefreshContext();
            foreach (object obj2 in list)
            {
                MetaType inheritanceRoot = this.services.Model.GetMetaType(obj2.GetType()).InheritanceRoot;
                this.GetTable(inheritanceRoot.Type);
                TrackedObject trackedObject = this.services.ChangeTracker.GetTrackedObject(obj2);
                if (trackedObject == null)
                {
                    throw System.Data.Linq.Error.UnrecognizedRefreshObject();
                }
                object[] keyValues = CommonDataServices.GetKeyValues(trackedObject.Type, trackedObject.Original);
                object objectByKey = context.Services.GetObjectByKey(trackedObject.Type, keyValues);
                if (objectByKey == null)
                {
                    throw System.Data.Linq.Error.RefreshOfDeletedObject();
                }
                trackedObject.Refresh(mode, objectByKey);
            }
        }

        public void Refresh(RefreshMode mode, object entity)
        {
            this.CheckDispose();
            this.CheckNotInSubmitChanges();
            this.VerifyTrackingEnabled();
            if (entity == null)
            {
                throw System.Data.Linq.Error.ArgumentNull("entity");
            }
            Array array = Array.CreateInstance(entity.GetType(), 1);
            array.SetValue(entity, 0);
            this.Refresh(mode, (IEnumerable) array);
        }

        internal void ResetLoadOptions()
        {
            this.CheckDispose();
            this.loadOptions = null;
        }

        public void SubmitChanges()
        {
            this.CheckDispose();
            this.SubmitChanges(ConflictMode.FailOnFirstConflict);
        }

        public virtual void SubmitChanges(ConflictMode failureMode)
        {
            this.CheckDispose();
            this.CheckNotInSubmitChanges();
            this.VerifyTrackingEnabled();
            this.conflicts.Clear();
            try
            {
                this.isInSubmitChanges = true;
                if ((System.Transactions.Transaction.Current == null) && (this.provider.Transaction == null))
                {
                    bool flag = false;
                    DbTransaction transaction = null;
                    try
                    {
                        try
                        {
                            if (this.provider.Connection.State == ConnectionState.Open)
                            {
                                this.provider.ClearConnection();
                            }
                            if (this.provider.Connection.State == ConnectionState.Closed)
                            {
                                this.provider.Connection.Open();
                                flag = true;
                            }
                            transaction = this.provider.Connection.BeginTransaction(System.Data.IsolationLevel.ReadCommitted);
                            this.provider.Transaction = transaction;
                            new ChangeProcessor(this.services, this).SubmitChanges(failureMode);
                            this.AcceptChanges();
                            this.provider.ClearConnection();
                            transaction.Commit();
                        }
                        catch
                        {
                            if (transaction != null)
                            {
                                try
                                {
                                    transaction.Rollback();
                                }
                                catch
                                {
                                }
                            }
                            throw;
                        }
                        return;
                    }
                    finally
                    {
                        this.provider.Transaction = null;
                        if (flag)
                        {
                            this.provider.Connection.Close();
                        }
                    }
                }
                new ChangeProcessor(this.services, this).SubmitChanges(failureMode);
                this.AcceptChanges();
            }
            finally
            {
                this.isInSubmitChanges = false;
            }
        }

        public IEnumerable<TResult> Translate<TResult>(DbDataReader reader)
        {
            this.CheckDispose();
            return (IEnumerable<TResult>) this.Translate(typeof(TResult), reader);
        }

        public System.Data.Linq.IMultipleResults Translate(DbDataReader reader)
        {
            this.CheckDispose();
            if (reader == null)
            {
                throw System.Data.Linq.Error.ArgumentNull("reader");
            }
            return this.provider.Translate(reader);
        }

        public IEnumerable Translate(Type elementType, DbDataReader reader)
        {
            this.CheckDispose();
            if (elementType == null)
            {
                throw System.Data.Linq.Error.ArgumentNull("elementType");
            }
            if (reader == null)
            {
                throw System.Data.Linq.Error.ArgumentNull("reader");
            }
            return this.provider.Translate(elementType, reader);
        }

        private static void ValidateTable(MetaTable metaTable)
        {
            foreach (MetaAssociation association in metaTable.RowType.Associations)
            {
                if (!association.ThisMember.DeclaringType.IsEntity)
                {
                    throw System.Data.Linq.Error.NonEntityAssociationMapping(association.ThisMember.DeclaringType.Type, association.ThisMember.Name, association.ThisMember.DeclaringType.Type);
                }
                if (!association.OtherType.IsEntity)
                {
                    throw System.Data.Linq.Error.NonEntityAssociationMapping(association.ThisMember.DeclaringType.Type, association.ThisMember.Name, association.OtherType.Type);
                }
            }
        }

        internal void VerifyTrackingEnabled()
        {
            this.CheckDispose();
            if (!this.ObjectTrackingEnabled)
            {
                throw System.Data.Linq.Error.ObjectTrackingRequired();
            }
        }

        public ChangeConflictCollection ChangeConflicts
        {
            get
            {
                this.CheckDispose();
                return this.conflicts;
            }
        }

        public int CommandTimeout
        {
            get
            {
                this.CheckDispose();
                return this.provider.CommandTimeout;
            }
            set
            {
                this.CheckDispose();
                this.provider.CommandTimeout = value;
            }
        }

        public DbConnection Connection
        {
            get
            {
                this.CheckDispose();
                return this.provider.Connection;
            }
        }

        public bool DeferredLoadingEnabled
        {
            get
            {
                this.CheckDispose();
                return this.deferredLoadingEnabled;
            }
            set
            {
                this.CheckDispose();
                if (this.Services.HasCachedObjects)
                {
                    throw System.Data.Linq.Error.OptionsCannotBeModifiedAfterQuery();
                }
                if (!this.ObjectTrackingEnabled && value)
                {
                    throw System.Data.Linq.Error.DeferredLoadingRequiresObjectTracking();
                }
                this.deferredLoadingEnabled = value;
            }
        }

        public DataLoadOptions LoadOptions
        {
            get
            {
                this.CheckDispose();
                return this.loadOptions;
            }
            set
            {
                this.CheckDispose();
                if (this.services.HasCachedObjects && (value != this.loadOptions))
                {
                    throw System.Data.Linq.Error.LoadOptionsChangeNotAllowedAfterQuery();
                }
                if (value != null)
                {
                    value.Freeze();
                }
                this.loadOptions = value;
            }
        }

        public TextWriter Log
        {
            get
            {
                this.CheckDispose();
                return this.provider.Log;
            }
            set
            {
                this.CheckDispose();
                this.provider.Log = value;
            }
        }

        public MetaModel Mapping
        {
            get
            {
                this.CheckDispose();
                return this.services.Model;
            }
        }

        public bool ObjectTrackingEnabled
        {
            get
            {
                this.CheckDispose();
                return this.objectTrackingEnabled;
            }
            set
            {
                this.CheckDispose();
                if (this.Services.HasCachedObjects)
                {
                    throw System.Data.Linq.Error.OptionsCannotBeModifiedAfterQuery();
                }
                this.objectTrackingEnabled = value;
                if (!this.objectTrackingEnabled)
                {
                    this.deferredLoadingEnabled = false;
                }
                this.services.ResetServices();
            }
        }

        internal IProvider Provider
        {
            get
            {
                this.CheckDispose();
                return this.provider;
            }
        }

        internal CommonDataServices Services
        {
            get
            {
                this.CheckDispose();
                return this.services;
            }
        }

        public DbTransaction Transaction
        {
            get
            {
                this.CheckDispose();
                return this.provider.Transaction;
            }
            set
            {
                this.CheckDispose();
                this.provider.Transaction = value;
            }
        }
    }
}

