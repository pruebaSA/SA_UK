namespace System.Data.Objects
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Data.Common.CommandTrees;
    using System.Data.Common.Internal.Materialization;
    using System.Data.Common.Utils;
    using System.Data.Entity;
    using System.Data.EntityClient;
    using System.Data.Metadata.Edm;
    using System.Data.Objects.DataClasses;
    using System.Data.Objects.ELinq;
    using System.Data.Objects.Internal;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Threading;
    using System.Transactions;

    public class ObjectContext : IDisposable
    {
        private IEntityAdapter _adapter;
        private System.Data.Objects.ObjectStateManager _cache;
        private EntityConnection _connection;
        private int _connectionRequestCount;
        private readonly bool _createdConnection;
        private bool _disallowSettingDefaultContainerName;
        private Transaction _lastTransaction;
        private static int _objectTypeCount;
        private bool _openedConnection;
        private ClrPerspective _perspective;
        private ObjectQueryProvider _queryProvider;
        private int? _queryTimeout;
        private readonly System.Data.Metadata.Edm.MetadataWorkspace _workspace;
        internal readonly int ObjectID;

        public event EventHandler SavingChanges;

        public ObjectContext(EntityConnection connection) : this(EntityUtil.CheckArgumentNull<EntityConnection>(connection, "connection"), true)
        {
        }

        public ObjectContext(string connectionString) : this(CreateEntityConnection(connectionString), false)
        {
            this._createdConnection = true;
        }

        private ObjectContext(EntityConnection connection, bool isConnectionConstructor)
        {
            this.ObjectID = Interlocked.Increment(ref _objectTypeCount);
            this._connection = connection;
            this._connection.StateChange += new StateChangeEventHandler(this.ConnectionStateChange);
            string connectionString = connection.ConnectionString;
            if ((connectionString == null) || (connectionString.Trim().Length == 0))
            {
                throw EntityUtil.InvalidConnection(isConnectionConstructor, null);
            }
            try
            {
                this._workspace = this.RetrieveMetadataWorkspaceFromConnection();
            }
            catch (InvalidOperationException exception)
            {
                throw EntityUtil.InvalidConnection(isConnectionConstructor, exception);
            }
            if (this._workspace != null)
            {
                if (!this._workspace.IsItemCollectionAlreadyRegistered(DataSpace.OSpace))
                {
                    ObjectItemCollection collection = new ObjectItemCollection();
                    this._workspace.RegisterItemCollection(collection);
                }
                this._workspace.GetItemCollection(DataSpace.OCSpace);
            }
        }

        protected ObjectContext(EntityConnection connection, string defaultContainerName) : this(connection)
        {
            this.DefaultContainerName = defaultContainerName;
            if (!string.IsNullOrEmpty(defaultContainerName))
            {
                this._disallowSettingDefaultContainerName = true;
            }
        }

        protected ObjectContext(string connectionString, string defaultContainerName) : this(connectionString)
        {
            this.DefaultContainerName = defaultContainerName;
            if (!string.IsNullOrEmpty(defaultContainerName))
            {
                this._disallowSettingDefaultContainerName = true;
            }
        }

        public void AcceptAllChanges()
        {
            foreach (ObjectStateEntry entry in this.ObjectStateManager.GetObjectStateEntries(EntityState.Deleted))
            {
                entry.AcceptChanges();
            }
            foreach (ObjectStateEntry entry2 in this.ObjectStateManager.GetObjectStateEntries(EntityState.Modified | EntityState.Added))
            {
                entry2.AcceptChanges();
            }
        }

        public void AddObject(string entitySetName, object entity)
        {
            EntityBid.Trace("<dobj.ObjectContext.AddObject|API>\n");
            EntityUtil.CheckArgumentNull<object>(entity, "entity");
            this.MetadataWorkspace.LoadAssemblyForType(entity.GetType(), null);
            EntitySet entitySetFromName = this.GetEntitySetFromName(entitySetName);
            RelationshipManager relationshipManager = EntityUtil.GetRelationshipManager(entity);
            bool flag = true;
            try
            {
                this.AddSingleObject(entitySetFromName, entity, "entity");
                flag = false;
            }
            finally
            {
                if ((flag && (relationshipManager != null)) && (relationshipManager.Context != null))
                {
                    ObjectStateEntry entry = this.ObjectStateManager.FindObjectStateEntry(entity);
                    if ((entry != null) && entry.EntityKey.IsTemporary)
                    {
                        relationshipManager.NodeVisited = true;
                        IEntityWithRelationships relationships = (IEntityWithRelationships) entity;
                        HashSet<EntityReference> promotedEntityKeyRefs = new HashSet<EntityReference>();
                        RelationshipManager.RemoveRelatedEntitiesFromObjectStateManager(relationships, promotedEntityKeyRefs);
                        RelatedEnd.RemoveEntityFromObjectStateManager(relationships);
                    }
                }
            }
            if (relationshipManager != null)
            {
                relationshipManager.AddRelatedEntitiesToObjectStateManager(false);
            }
        }

        private void AddRefreshKey(object entityLike, Dictionary<EntityKey, ObjectStateEntry> entities, Dictionary<EntitySet, List<EntityKey>> currentKeys)
        {
            if (entityLike == null)
            {
                throw EntityUtil.NthElementIsNull(entities.Count);
            }
            EntityKey key = FindEntityKey(entityLike, this);
            this.RefreshCheck(entities, entityLike, key);
            EntitySet entitySet = key.GetEntitySet(this.MetadataWorkspace);
            List<EntityKey> list = null;
            if (!currentKeys.TryGetValue(entitySet, out list))
            {
                list = new List<EntityKey>();
                currentKeys.Add(entitySet, list);
            }
            list.Add(key);
        }

        internal void AddSingleObject(EntitySet entitySet, object entity, string argumentName)
        {
            RelationshipManager relationshipManager = EntityUtil.GetRelationshipManager(entity);
            EntityKey key = FindEntityKey(entity, this);
            if (key != null)
            {
                EntityUtil.ValidateEntitySetInKey(key, entitySet);
                key.ValidateEntityKey(entitySet);
            }
            this.ObjectStateManager.AddEntry(entity, null, entitySet, argumentName, true);
            if (relationshipManager != null)
            {
                relationshipManager.AttachContext(this, entitySet, MergeOption.AppendOnly);
            }
        }

        public void ApplyPropertyChanges(string entitySetName, object changed)
        {
            EntityUtil.CheckStringArgument(entitySetName, "entitySetName");
            EntityUtil.CheckArgumentNull<object>(changed, "changed");
            this.MetadataWorkspace.LoadAssemblyForType(changed.GetType(), null);
            EntitySet entitySetFromName = this.GetEntitySetFromName(entitySetName);
            EntityKey key = FindEntityKey(changed, this);
            if (key != null)
            {
                EntityUtil.ValidateEntitySetInKey(key, entitySetFromName, "entitySetName");
                key.ValidateEntityKey(entitySetFromName);
            }
            else
            {
                key = this.CreateEntityKey(entitySetFromName, changed);
            }
            ObjectStateEntry entry = this.ObjectStateManager.FindObjectStateEntry(key);
            if ((entry == null) || entry.IsKeyEntry)
            {
                throw EntityUtil.NoEntryExistsForObject(changed);
            }
            if ((entry.State != EntityState.Modified) && (entry.State != EntityState.Unchanged))
            {
                throw EntityUtil.EntityMustBeUnchangedOrModified(entry.State);
            }
            if (entry.Entity.GetType() != changed.GetType())
            {
                throw EntityUtil.EntitiesHaveDifferentType(entry.Entity.GetType().FullName, changed.GetType().FullName);
            }
            entry.CompareKeyProperties(changed);
            Shaper.UpdateRecord(changed, entry.CurrentValues);
        }

        public void Attach(IEntityWithKey entity)
        {
            EntityBid.Trace("<dobj.ObjectContext.Attach|API>\n");
            EntityUtil.CheckArgumentNull<IEntityWithKey>(entity, "entity");
            if (entity.EntityKey == null)
            {
                throw EntityUtil.CannotAttachEntityWithoutKey();
            }
            this.AttachTo(null, entity);
        }

        internal void AttachSingleObject(object entity, EntitySet entitySet, string argumentName)
        {
            RelationshipManager relationshipManager = EntityUtil.GetRelationshipManager(entity);
            EntityKey key = FindEntityKey(entity, this);
            if (key != null)
            {
                EntityUtil.ValidateEntitySetInKey(key, entitySet);
                key.ValidateEntityKey(entitySet);
            }
            else
            {
                key = this.CreateEntityKey(entitySet, entity);
            }
            if (key.IsTemporary)
            {
                throw EntityUtil.CannotAttachEntityWithTemporaryKey();
            }
            IEntityWithKey key2 = entity as IEntityWithKey;
            if ((key2 != null) && (key2.EntityKey == null))
            {
                EntityUtil.SetKeyOntoEntity(entity, key);
            }
            ObjectStateEntry keyEntry = this.ObjectStateManager.FindObjectStateEntry(key);
            if (keyEntry != null)
            {
                if (!keyEntry.IsKeyEntry)
                {
                    throw EntityUtil.ObjectStateManagerContainsThisEntityKey();
                }
                this.ObjectStateManager.PromoteKeyEntryInitialization(keyEntry, entity, null, false);
                if (relationshipManager != null)
                {
                    relationshipManager.AttachContext(this, entitySet, MergeOption.AppendOnly);
                }
                this.ObjectStateManager.PromoteKeyEntry(keyEntry, entity, null, false, false, true, "Attach");
                if (relationshipManager != null)
                {
                    relationshipManager.CheckReferentialConstraintProperties(key);
                }
            }
            else
            {
                this.ObjectStateManager.AttachEntry(key, entity, entitySet, argumentName);
                if (relationshipManager != null)
                {
                    relationshipManager.AttachContext(this, entitySet, MergeOption.AppendOnly);
                    relationshipManager.CheckReferentialConstraintProperties(key);
                }
            }
        }

        public void AttachTo(string entitySetName, object entity)
        {
            EntityBid.Trace("<dobj.ObjectContext.AttachTo|API>\n");
            EntityUtil.CheckArgumentNull<object>(entity, "entity");
            this.MetadataWorkspace.LoadAssemblyForType(entity.GetType(), null);
            EntitySet entitySet = null;
            if (!string.IsNullOrEmpty(entitySetName))
            {
                entitySet = this.GetEntitySetFromName(entitySetName);
            }
            EntitySet set2 = null;
            EntityKey key = FindEntityKey(entity, this);
            if (key != null)
            {
                set2 = key.GetEntitySet(this.MetadataWorkspace);
                if (entitySet != null)
                {
                    EntityUtil.ValidateEntitySetInKey(key, entitySet, "entitySetName");
                }
                key.ValidateEntityKey(set2);
            }
            EntitySet set3 = set2 ?? entitySet;
            if (set3 == null)
            {
                throw EntityUtil.EntitySetNameOrEntityKeyRequired();
            }
            this.ValidateEntitySet(set3, entity.GetType());
            ObjectStateEntry entry = this.ObjectStateManager.FindObjectStateEntry(key);
            if ((entry != null) && !entry.IsKeyEntry)
            {
                if (!object.ReferenceEquals(entry.Entity, entity))
                {
                    throw EntityUtil.ObjectStateManagerContainsThisEntityKey();
                }
                if (entry.State != EntityState.Unchanged)
                {
                    throw EntityUtil.EntityAlreadyExistsInObjectStateManager();
                }
            }
            else
            {
                this.ObjectStateManager.BeginAttachTracking();
                try
                {
                    RelationshipManager relationshipManager = EntityUtil.GetRelationshipManager(entity);
                    bool flag = true;
                    try
                    {
                        this.AttachSingleObject(entity, set3, "entity");
                        flag = false;
                    }
                    finally
                    {
                        if ((flag && (relationshipManager != null)) && (relationshipManager.Context == this))
                        {
                            relationshipManager.NodeVisited = true;
                            IEntityWithRelationships relationships = (IEntityWithRelationships) entity;
                            HashSet<EntityReference> promotedEntityKeyRefs = new HashSet<EntityReference>();
                            RelationshipManager.RemoveRelatedEntitiesFromObjectStateManager(relationships, promotedEntityKeyRefs);
                            RelatedEnd.RemoveEntityFromObjectStateManager(relationships);
                        }
                    }
                    if (relationshipManager != null)
                    {
                        relationshipManager.AddRelatedEntitiesToObjectStateManager(true);
                    }
                }
                finally
                {
                    this.ObjectStateManager.EndAttachTracking();
                }
            }
        }

        private int BatchRefreshEntitiesByKey(RefreshMode refreshMode, Dictionary<EntityKey, ObjectStateEntry> trackedEntities, EntitySet targetSet, List<EntityKey> targetKeys, int startFrom)
        {
            DbQueryCommandTree tree = new DbQueryCommandTree(this.MetadataWorkspace, DataSpace.CSpace);
            DbExpressionBinding input = tree.CreateExpressionBinding(tree.CreateScanExpression(targetSet), "EntitySet");
            DbExpression expression = tree.CreateRefKeyExpression(tree.CreateEntityRefExpression(input.Variable));
            DbVariableReferenceExpression left = tree.CreateVariableReferenceExpression("EntityKey", expression.ResultType);
            int num = Math.Min(250, targetKeys.Count - startFrom);
            DbExpression[] nodes = new DbExpression[num];
            for (int i = 0; i < num; i++)
            {
                KeyValuePair<string, DbExpression>[] keyValueExpressions = targetKeys[startFrom++].GetKeyValueExpressions(tree, targetSet);
                DbExpression right = tree.CreateNewRowExpression(keyValueExpressions);
                nodes[i] = tree.CreateEqualsExpression(left, right);
            }
            DbExpression body = Helpers.BuildBalancedTreeInPlace<DbExpression>(nodes, new System.Func<DbExpression, DbExpression, DbExpression>(tree.CreateOrExpression));
            KeyValuePair<string, TypeUsage>[] formals = new KeyValuePair<string, TypeUsage>[] { new KeyValuePair<string, TypeUsage>(left.VariableName, left.ResultType) };
            DbExpression[] args = new DbExpression[] { expression };
            body = tree.CreateLambdaFunctionExpression(formals, body, args);
            tree.Query = tree.CreateFilterExpression(input, body);
            MergeOption mergeOption = (RefreshMode.StoreWins == refreshMode) ? MergeOption.OverwriteChanges : MergeOption.PreserveChanges;
            this.EnsureConnection();
            try
            {
                foreach (object obj2 in ObjectQueryExecutionPlan.ExecuteCommandTree<object>(this, tree, mergeOption))
                {
                    ObjectStateEntry entry = this.ObjectStateManager.FindObjectStateEntry(obj2);
                    if ((entry != null) && (EntityState.Modified == entry.State))
                    {
                        entry.SetModifiedAll();
                    }
                    EntityKey entityKey = this.ObjectStateManager.GetEntityKey(obj2);
                    trackedEntities.Remove(entityKey);
                }
            }
            catch
            {
                this.ReleaseConnection();
                throw;
            }
            return startFrom;
        }

        private void ConnectionStateChange(object sender, StateChangeEventArgs e)
        {
            if (e.CurrentState == ConnectionState.Closed)
            {
                this._connectionRequestCount = 0;
                this._openedConnection = false;
            }
        }

        private static EntityConnection CreateEntityConnection(string connectionString)
        {
            EntityUtil.CheckStringArgument(connectionString, "connectionString");
            return new EntityConnection(connectionString);
        }

        internal EntityKey CreateEntityKey(EntitySet entitySet, object entity)
        {
            ReadOnlyMetadataCollection<EdmMember> keyMembers = entitySet.ElementType.KeyMembers;
            StateManagerTypeMetadata orAddStateManagerTypeMetadata = this.ObjectStateManager.GetOrAddStateManagerTypeMetadata(entity.GetType(), entitySet);
            object[] compositeKeyValues = new object[keyMembers.Count];
            for (int i = 0; i < keyMembers.Count; i++)
            {
                string name = keyMembers[i].Name;
                int ordinalforCLayerMemberName = orAddStateManagerTypeMetadata.GetOrdinalforCLayerMemberName(name);
                if (ordinalforCLayerMemberName < 0)
                {
                    throw EntityUtil.EntityTypeDoesNotMatchEntitySet(entity.GetType().FullName, entitySet.Name, "entity");
                }
                compositeKeyValues[i] = orAddStateManagerTypeMetadata.Member(ordinalforCLayerMemberName).GetValue(entity);
            }
            if (compositeKeyValues.Length == 1)
            {
                return new EntityKey(entitySet, compositeKeyValues[0]);
            }
            return new EntityKey(entitySet, compositeKeyValues);
        }

        public EntityKey CreateEntityKey(string entitySetName, object entity)
        {
            EntityUtil.CheckStringArgument(entitySetName, "entitySetName");
            EntityUtil.CheckArgumentNull<object>(entity, "entity");
            this.MetadataWorkspace.LoadAssemblyForType(entity.GetType(), null);
            EntitySet entitySetFromName = this.GetEntitySetFromName(entitySetName);
            return this.CreateEntityKey(entitySetFromName, entity);
        }

        private ObjectResult<TElement> CreateFunctionObjectResult<TElement>(EntityCommand entityCommand, EntitySet entitySet, EntityType entityType) where TElement: IEntityWithChangeTracker
        {
            EventHandler handler = null;
            ObjectResult<TElement> result;
            DbDataReader storeReader;
            this.EnsureConnection();
            EntityCommandDefinition commandDefinition = entityCommand.GetCommandDefinition();
            try
            {
                storeReader = commandDefinition.ExecuteStoreCommands(entityCommand, CommandBehavior.Default);
            }
            catch (Exception exception)
            {
                this.ReleaseConnection();
                if (EntityUtil.IsCatchableEntityExceptionType(exception))
                {
                    throw EntityUtil.CommandExecution(System.Data.Entity.Strings.EntityClient_CommandExecutionFailed, exception);
                }
                throw;
            }
            try
            {
                Shaper<TElement> shaper = System.Data.Common.Internal.Materialization.Translator.TranslateColumnMap<TElement>(this.Perspective.MetadataWorkspace.GetQueryCacheManager(), commandDefinition.CreateColumnMap(storeReader), this.MetadataWorkspace, null, MergeOption.AppendOnly, false).Create(storeReader, this, this.MetadataWorkspace, MergeOption.AppendOnly);
                if (handler == null)
                {
                    handler = delegate (object sender, EventArgs e) {
                        CommandHelper.ConsumeReader(storeReader);
                        entityCommand.NotifyDataReaderClosing();
                    };
                }
                shaper.OnDone += handler;
                result = new ObjectResult<TElement>(shaper, entitySet, TypeUsage.Create(entityType));
            }
            catch
            {
                this.ReleaseConnection();
                storeReader.Dispose();
                throw;
            }
            return result;
        }

        public ObjectQuery<T> CreateQuery<T>(string queryString, params ObjectParameter[] parameters)
        {
            EntityUtil.CheckArgumentNull<string>(queryString, "queryString");
            EntityUtil.CheckArgumentNull<ObjectParameter[]>(parameters, "parameters");
            this.MetadataWorkspace.LoadAssemblyForType(typeof(T), Assembly.GetCallingAssembly());
            ObjectQuery<T> query = new ObjectQuery<T>(queryString, this, MergeOption.AppendOnly);
            foreach (ObjectParameter parameter in parameters)
            {
                query.Parameters.Add(parameter);
            }
            return query;
        }

        public void DeleteObject(object entity)
        {
            EntityBid.Trace("<dobj.ObjectContext.DeleteObject|API>\n");
            EntityUtil.CheckArgumentNull<object>(entity, "entity");
            ObjectStateEntry entry = this.ObjectStateManager.FindObjectStateEntry(entity);
            if ((entry == null) || !object.ReferenceEquals(entry.Entity, entity))
            {
                throw EntityUtil.CannotDeleteEntityNotInObjectStateManager();
            }
            entry.Delete();
        }

        public void Detach(object entity)
        {
            EntityBid.Trace("<dobj.ObjectContext.Detach|API>\n");
            EntityUtil.CheckArgumentNull<object>(entity, "entity");
            ObjectStateEntry entry = this.ObjectStateManager.FindObjectStateEntry(entity);
            if (((entry == null) || !object.ReferenceEquals(entry.Entity, entity)) || (entry.Entity == null))
            {
                throw EntityUtil.CannotDetachEntityNotInObjectStateManager();
            }
            entry.Detach();
        }

        public void Dispose()
        {
            EntityBid.Trace("<dobj.ObjectContext.Dispose|API>\n");
            this.Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this._createdConnection && (this._connection != null))
                {
                    this._connection.Dispose();
                }
                this._connection = null;
                this._adapter = null;
            }
        }

        internal void EnsureConnection()
        {
            if (this._connection == null)
            {
                throw EntityUtil.ObjectContextDisposed();
            }
            if (this.Connection.State == ConnectionState.Closed)
            {
                this.Connection.Open();
                this._openedConnection = true;
            }
            if (this._openedConnection)
            {
                this._connectionRequestCount++;
            }
            if ((this._connection.State == ConnectionState.Closed) || (this._connection.State == ConnectionState.Broken))
            {
                throw EntityUtil.InvalidOperation(System.Data.Entity.Strings.EntityClient_ExecutingOnClosedConnection((this._connection.State == ConnectionState.Closed) ? System.Data.Entity.Strings.EntityClient_ConnectionStateClosed : System.Data.Entity.Strings.EntityClient_ConnectionStateBroken));
            }
            try
            {
                this.EnsureMetadata();
                Transaction current = Transaction.Current;
                if (((null != current) && !current.Equals(this._lastTransaction)) || ((null != this._lastTransaction) && !this._lastTransaction.Equals(current)))
                {
                    if (!this._openedConnection || ((null == this._lastTransaction) && (this._connectionRequestCount > 1)))
                    {
                        this._connection.EnlistTransaction(current);
                    }
                    else if (this._connectionRequestCount > 1)
                    {
                        this._connection.Close();
                        this._connection.Open();
                        this._openedConnection = true;
                        this._connectionRequestCount++;
                    }
                }
                this._lastTransaction = current;
            }
            catch (Exception)
            {
                this.ReleaseConnection();
                throw;
            }
        }

        internal void EnsureMetadata()
        {
            if (!this.MetadataWorkspace.IsItemCollectionAlreadyRegistered(DataSpace.SSpace))
            {
                if (this._connection == null)
                {
                    throw EntityUtil.ObjectContextDisposed();
                }
                System.Data.Metadata.Edm.MetadataWorkspace metadataWorkspace = this._connection.GetMetadataWorkspace();
                ItemCollection itemCollection = metadataWorkspace.GetItemCollection(DataSpace.CSpace);
                ItemCollection objB = this.MetadataWorkspace.GetItemCollection(DataSpace.CSpace);
                if (!object.ReferenceEquals(itemCollection, objB))
                {
                    throw EntityUtil.ContextMetadataHasChanged();
                }
                this.MetadataWorkspace.RegisterItemCollection(metadataWorkspace.GetItemCollection(DataSpace.SSpace));
                this.MetadataWorkspace.RegisterItemCollection(metadataWorkspace.GetItemCollection(DataSpace.CSSpace));
            }
        }

        protected ObjectResult<TElement> ExecuteFunction<TElement>(string functionName, params ObjectParameter[] parameters) where TElement: IEntityWithChangeTracker
        {
            string str;
            string str2;
            EntityType type;
            EntityType type2;
            EntityUtil.CheckStringArgument(functionName, "function");
            EntityUtil.CheckArgumentNull<ObjectParameter[]>(parameters, "parameters");
            for (int i = 0; i < parameters.Length; i++)
            {
                if (parameters[i] == null)
                {
                    throw EntityUtil.InvalidOperation(System.Data.Entity.Strings.ObjectContext_ExecuteFunctionCalledWithNullParameter(i));
                }
            }
            EntityConnection connection = (EntityConnection) this.Connection;
            System.Data.Metadata.Edm.MetadataWorkspace metadataWorkspace = this.MetadataWorkspace;
            CommandHelper.ParseFunctionImportCommandText(functionName, this.DefaultContainerName, out str, out str2);
            EdmFunction functionImport = CommandHelper.FindFunctionImport(metadataWorkspace, str, str2);
            if (!MetadataHelper.TryGetFunctionImportReturnEntityType(functionImport, out type))
            {
                throw EntityUtil.ExecuteFunctionCalledWithNonReaderFunction(functionImport);
            }
            if (!MetadataHelper.TryDetermineCSpaceModelType<TElement>(metadataWorkspace, out type2) || !type2.EdmEquals(type))
            {
                throw EntityUtil.ExecuteFunctionTypeMismatch(typeof(TElement), type);
            }
            EntityCommand command = new EntityCommand {
                CommandType = CommandType.StoredProcedure,
                CommandText = str + "." + str2,
                Connection = connection
            };
            if (this.CommandTimeout.HasValue)
            {
                command.CommandTimeout = this.CommandTimeout.Value;
            }
            this.PopulateFunctionEntityCommandParameters(parameters, functionImport, command);
            return this.CreateFunctionObjectResult<TElement>(command, functionImport.EntitySet, type);
        }

        internal static EntityKey FindEntityKey(object entity, ObjectContext context)
        {
            EntityKey key2;
            IEntityWithKey key = entity as IEntityWithKey;
            if (key != null)
            {
                return key.EntityKey;
            }
            if ((context._cache != null) && context.ObjectStateManager.TryGetEntityKey(entity, out key2))
            {
                return key2;
            }
            return null;
        }

        internal EntitySet GetEntitySet(string entitySetName, string entityContainerName)
        {
            EntityContainer entityContainer = null;
            if (string.IsNullOrEmpty(entityContainerName))
            {
                entityContainer = this.Perspective.GetDefaultContainer();
            }
            else if (!this.MetadataWorkspace.TryGetEntityContainer(entityContainerName, DataSpace.CSpace, out entityContainer))
            {
                throw EntityUtil.EntityContainterNotFoundForName(entityContainerName);
            }
            EntitySet entitySet = null;
            if (!entityContainer.TryGetEntitySetByName(entitySetName, false, out entitySet))
            {
                throw EntityUtil.EntitySetNotFoundForName(TypeHelpers.GetFullName(entityContainer.Name, entitySetName));
            }
            return entitySet;
        }

        internal EntitySet GetEntitySetFromName(string entitySetName)
        {
            string str;
            string str2;
            GetEntitySetName(entitySetName, "entitySetName", this, out str, out str2);
            return this.GetEntitySet(str, str2);
        }

        private static void GetEntitySetName(string qualifiedName, string parameterName, ObjectContext context, out string entityset, out string container)
        {
            entityset = null;
            container = null;
            EntityUtil.CheckStringArgument(qualifiedName, parameterName);
            string[] strArray = qualifiedName.Split(new char[] { '.' });
            if (strArray.Length > 2)
            {
                throw EntityUtil.QualfiedEntitySetName(parameterName);
            }
            if (strArray.Length == 1)
            {
                entityset = strArray[0];
            }
            else
            {
                container = strArray[0];
                entityset = strArray[1];
                if ((container == null) || (container.Length == 0))
                {
                    throw EntityUtil.QualfiedEntitySetName(parameterName);
                }
            }
            if ((entityset == null) || (entityset.Length == 0))
            {
                throw EntityUtil.QualfiedEntitySetName(parameterName);
            }
            if (((context != null) && string.IsNullOrEmpty(container)) && (context.Perspective.GetDefaultContainer() == null))
            {
                throw EntityUtil.ContainerQualifiedEntitySetNameRequired(parameterName);
            }
        }

        public object GetObjectByKey(EntityKey key)
        {
            object obj2;
            EntityUtil.CheckArgumentNull<EntityKey>(key, "key");
            EntitySet entitySet = key.GetEntitySet(this.MetadataWorkspace);
            this.MetadataWorkspace.LoadFromEntityType(entitySet.ElementType, Assembly.GetCallingAssembly());
            if (!this.TryGetObjectByKey(key, out obj2))
            {
                throw EntityUtil.ObjectNotFound();
            }
            return obj2;
        }

        private void OnSavingChanges()
        {
            if (this._onSavingChanges != null)
            {
                EntityBid.Trace("<dobj.ObjectContext.OnSavingChanges|INFO>\n");
                this._onSavingChanges(this, new EventArgs());
            }
        }

        private void PopulateFunctionEntityCommandParameters(ObjectParameter[] parameters, EdmFunction functionImport, EntityCommand command)
        {
            foreach (ObjectParameter parameter in parameters)
            {
                FunctionParameter parameter2;
                if (!functionImport.Parameters.TryGetValue(parameter.Name, false, out parameter2))
                {
                    parameter2 = null;
                }
                EntityParameter parameter3 = new EntityParameter {
                    ParameterName = parameter.Name,
                    Value = parameter.Value ?? DBNull.Value
                };
                if (parameter2 != null)
                {
                    parameter3.Direction = MetadataHelper.ParameterModeToParameterDirection(parameter2.Mode);
                }
                if ((DBNull.Value == parameter3.Value) || (parameter3.Direction != ParameterDirection.Input))
                {
                    TypeUsage typeUsage;
                    if (parameter2 != null)
                    {
                        typeUsage = parameter2.TypeUsage;
                    }
                    else if (parameter.TypeUsage == null)
                    {
                        this.Perspective.TryGetTypeByName(parameter.MappableType.FullName, false, out typeUsage);
                    }
                    else
                    {
                        typeUsage = parameter.TypeUsage;
                    }
                    DbCommandDefinition.PopulateParameterFromTypeUsage(parameter3, typeUsage, parameter3.Direction != ParameterDirection.Input);
                }
                if (parameter3.Direction != ParameterDirection.Input)
                {
                    ParameterBinder binder = new ParameterBinder(parameter3, parameter);
                    command.OnDataReaderClosing += new EventHandler(binder.OnDataReaderClosingHandler);
                }
                command.Parameters.Add(parameter3);
            }
        }

        private static string QuoteIdentifier(string identifier) => 
            ("[" + identifier.Replace("]", "]]") + "]");

        public void Refresh(RefreshMode refreshMode, IEnumerable collection)
        {
            IntPtr ptr;
            EntityBid.ScopeEnter(out ptr, "<dobj.ObjectContext.Refresh|API> refreshMode=%d{RefreshMode}, collection", (int) refreshMode);
            try
            {
                EntityUtil.CheckArgumentRefreshMode(refreshMode);
                EntityUtil.CheckArgumentNull<IEnumerable>(collection, "collection");
                this.RefreshEntities(refreshMode, collection);
            }
            finally
            {
                EntityBid.ScopeLeave(ref ptr);
            }
        }

        public void Refresh(RefreshMode refreshMode, object entity)
        {
            IntPtr ptr;
            EntityBid.ScopeEnter(out ptr, "<dobj.ObjectContext.Refresh|API> refreshMode=%d{RefreshMode}, entity, collection", (int) refreshMode);
            try
            {
                EntityUtil.CheckArgumentRefreshMode(refreshMode);
                EntityUtil.CheckArgumentNull<object>(entity, "entity");
                this.RefreshEntities(refreshMode, new object[] { entity });
            }
            finally
            {
                EntityBid.ScopeLeave(ref ptr);
            }
        }

        private void RefreshCheck(Dictionary<EntityKey, ObjectStateEntry> entities, object entity, EntityKey key)
        {
            ObjectStateEntry entry = this.ObjectStateManager.FindObjectStateEntry(key);
            if (entry == null)
            {
                throw EntityUtil.NthElementNotInObjectStateManager(entities.Count);
            }
            if (EntityState.Added == entry.State)
            {
                throw EntityUtil.NthElementInAddedState(entities.Count);
            }
            try
            {
                entities.Add(key, entry);
            }
            catch (ArgumentException exception)
            {
                EntityUtil.TraceExceptionForCapture(exception);
                throw EntityUtil.NthElementIsDuplicate(entities.Count);
            }
        }

        private void RefreshEntities(RefreshMode refreshMode, IEnumerable collection)
        {
            bool flag = false;
            try
            {
                Dictionary<EntityKey, ObjectStateEntry> entities = new Dictionary<EntityKey, ObjectStateEntry>(RefreshEntitiesSize(collection));
                Dictionary<EntitySet, List<EntityKey>> currentKeys = new Dictionary<EntitySet, List<EntityKey>>();
                foreach (object obj2 in collection)
                {
                    this.AddRefreshKey(obj2, entities, currentKeys);
                }
                collection = null;
                EntityBid.Trace("<dobj.ObjectContext.RefreshEntities|Info> minimumExecutions=%d, plannedRefreshCount=%d\n", currentKeys.Count, entities.Count);
                if (currentKeys.Count > 0)
                {
                    this.EnsureConnection();
                    flag = true;
                    foreach (EntitySet set in currentKeys.Keys)
                    {
                        List<EntityKey> targetKeys = currentKeys[set];
                        for (int i = 0; i < targetKeys.Count; i = this.BatchRefreshEntitiesByKey(refreshMode, entities, set, targetKeys, i))
                        {
                        }
                    }
                }
                currentKeys = null;
                EntityBid.Trace("<dobj.ObjectContext.RefreshEntities|Info> unrefreshedCount=%d\n", entities.Count);
                if (RefreshMode.StoreWins == refreshMode)
                {
                    foreach (KeyValuePair<EntityKey, ObjectStateEntry> pair in entities)
                    {
                        if (EntityState.Detached != pair.Value.State)
                        {
                            pair.Value.Delete();
                            pair.Value.AcceptChanges();
                        }
                    }
                }
                else if ((RefreshMode.ClientWins == refreshMode) && (0 < entities.Count))
                {
                    string str = string.Empty;
                    StringBuilder builder = new StringBuilder();
                    foreach (KeyValuePair<EntityKey, ObjectStateEntry> pair2 in entities)
                    {
                        if (pair2.Value.State == EntityState.Deleted)
                        {
                            pair2.Value.AcceptChanges();
                        }
                        else
                        {
                            builder.Append(str).Append(Environment.NewLine);
                            builder.Append('\'').Append(pair2.Key.ConcatKeyValue()).Append('\'');
                            str = ",";
                        }
                    }
                    if (builder.Length > 0)
                    {
                        throw EntityUtil.ClientEntityRemovedFromStore(builder.ToString());
                    }
                }
            }
            finally
            {
                if (flag)
                {
                    this.ReleaseConnection();
                }
            }
        }

        private static int RefreshEntitiesSize(IEnumerable collection)
        {
            ICollection is2 = collection as ICollection;
            return is2?.Count;
        }

        internal void ReleaseConnection()
        {
            if (this._connection == null)
            {
                throw EntityUtil.ObjectContextDisposed();
            }
            if (this._openedConnection)
            {
                if (this._connectionRequestCount > 0)
                {
                    this._connectionRequestCount--;
                }
                if (this._connectionRequestCount == 0)
                {
                    this.Connection.Close();
                    this._openedConnection = false;
                }
            }
        }

        private System.Data.Metadata.Edm.MetadataWorkspace RetrieveMetadataWorkspaceFromConnection() => 
            this._connection?.GetMetadataWorkspace(false).ShallowCopy();

        public int SaveChanges() => 
            this.SaveChanges(true);

        public int SaveChanges(bool acceptChangesDuringSave)
        {
            this.OnSavingChanges();
            bool flag = false;
            int objectStateEntriesCount = this.ObjectStateManager.GetObjectStateEntriesCount(EntityState.Modified | EntityState.Deleted | EntityState.Added);
            using (new EntityBid.ScopeAuto("<dobj.ObjectContext.SaveChanges|API> %d#, affectingEntries=%d", this.ObjectID, objectStateEntriesCount))
            {
                EntityConnection connection = (EntityConnection) this.Connection;
                if (0 >= objectStateEntriesCount)
                {
                    return objectStateEntriesCount;
                }
                if (this._adapter == null)
                {
                    IServiceProvider providerFactory = connection.ProviderFactory as IServiceProvider;
                    if (providerFactory != null)
                    {
                        this._adapter = providerFactory.GetService(typeof(IEntityAdapter)) as IEntityAdapter;
                    }
                    if (this._adapter == null)
                    {
                        throw EntityUtil.InvalidDataAdapter();
                    }
                }
                this._adapter.AcceptChangesDuringUpdate = false;
                this._adapter.Connection = connection;
                this._adapter.CommandTimeout = this.CommandTimeout;
                try
                {
                    this.EnsureConnection();
                    flag = true;
                    Transaction current = Transaction.Current;
                    bool flag2 = false;
                    if (connection.CurrentTransaction == null)
                    {
                        flag2 = null == this._lastTransaction;
                    }
                    using (DbTransaction transaction = null)
                    {
                        if (flag2)
                        {
                            transaction = connection.BeginTransaction();
                        }
                        objectStateEntriesCount = this._adapter.Update(this.ObjectStateManager);
                        if (transaction != null)
                        {
                            transaction.Commit();
                        }
                    }
                }
                finally
                {
                    if (flag)
                    {
                        this.ReleaseConnection();
                    }
                }
                if (!acceptChangesDuringSave)
                {
                    return objectStateEntriesCount;
                }
                try
                {
                    this.AcceptAllChanges();
                }
                catch (Exception exception)
                {
                    if (EntityUtil.IsCatchableExceptionType(exception))
                    {
                        throw EntityUtil.AcceptAllChangesFailure(exception);
                    }
                    throw;
                }
            }
            return objectStateEntriesCount;
        }

        public bool TryGetObjectByKey(EntityKey key, out object value)
        {
            ObjectStateEntry entry;
            this.ObjectStateManager.TryGetObjectStateEntry(key, out entry);
            if ((entry != null) && !entry.IsKeyEntry)
            {
                value = entry.Entity;
                return (value != null);
            }
            if (key.IsTemporary)
            {
                value = null;
                return false;
            }
            EntitySet entitySet = key.GetEntitySet(this.MetadataWorkspace);
            key.ValidateEntityKey(entitySet, true, "key");
            this.MetadataWorkspace.LoadFromEntityType(entitySet.ElementType, Assembly.GetCallingAssembly());
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat("SELECT VALUE X FROM {0}.{1} AS X WHERE ", QuoteIdentifier(entitySet.EntityContainer.Name), QuoteIdentifier(entitySet.Name));
            EntityKeyMember[] entityKeyValues = key.EntityKeyValues;
            ReadOnlyMetadataCollection<EdmMember> keyMembers = entitySet.ElementType.KeyMembers;
            ObjectParameter[] parameters = new ObjectParameter[entityKeyValues.Length];
            for (int i = 0; i < entityKeyValues.Length; i++)
            {
                if (i > 0)
                {
                    builder.Append(" AND ");
                }
                string str = string.Format(CultureInfo.InvariantCulture, "p{0}", new object[] { i.ToString(CultureInfo.InvariantCulture) });
                builder.AppendFormat("X.{0} = @{1}", QuoteIdentifier(entityKeyValues[i].Key), str);
                parameters[i] = new ObjectParameter(str, entityKeyValues[i].Value);
                EdmMember item = null;
                if (keyMembers.TryGetValue(entityKeyValues[i].Key, true, out item))
                {
                    parameters[i].TypeUsage = item.TypeUsage;
                }
            }
            object obj2 = null;
            foreach (object obj3 in this.CreateQuery<object>(builder.ToString(), parameters).Execute(MergeOption.AppendOnly))
            {
                obj2 = obj3;
            }
            value = obj2;
            return (value != null);
        }

        private void ValidateEntitySet(EntitySet entitySet, Type entityType)
        {
            TypeUsage outTypeUsage = null;
            if (!this.Perspective.TryGetType(entityType, out outTypeUsage) || !TypeSemantics.IsEntityType(outTypeUsage))
            {
                throw EntityUtil.InvalidEntityType("entity", entityType);
            }
            if (!entitySet.ElementType.IsAssignableFrom(outTypeUsage.EdmType))
            {
                throw EntityUtil.InvalidEntitySetOnEntity(entitySet.Name, entityType, "entity");
            }
        }

        public int? CommandTimeout
        {
            get => 
                this._queryTimeout;
            set
            {
                if (value.HasValue && (value < 0))
                {
                    throw EntityUtil.InvalidCommandTimeout("value");
                }
                this._queryTimeout = value;
            }
        }

        public DbConnection Connection
        {
            get
            {
                if (this._connection == null)
                {
                    throw EntityUtil.ObjectContextDisposed();
                }
                return this._connection;
            }
        }

        public string DefaultContainerName
        {
            get
            {
                EntityContainer defaultContainer = this.Perspective.GetDefaultContainer();
                return defaultContainer?.Name;
            }
            set
            {
                if (this._disallowSettingDefaultContainerName)
                {
                    throw EntityUtil.CannotSetDefaultContainerName();
                }
                this.Perspective.SetDefaultContainer(value);
            }
        }

        public System.Data.Metadata.Edm.MetadataWorkspace MetadataWorkspace =>
            this._workspace;

        public System.Data.Objects.ObjectStateManager ObjectStateManager
        {
            get
            {
                if (this._cache == null)
                {
                    this._cache = new System.Data.Objects.ObjectStateManager(this._workspace);
                }
                return this._cache;
            }
        }

        internal ClrPerspective Perspective
        {
            get
            {
                if (this._perspective == null)
                {
                    this._perspective = new ClrPerspective(this._workspace);
                }
                return this._perspective;
            }
        }

        internal IQueryProvider Provider
        {
            get
            {
                if (this._queryProvider == null)
                {
                    this._queryProvider = new ObjectQueryProvider(this);
                }
                return this._queryProvider;
            }
        }

        private class ParameterBinder
        {
            private readonly EntityParameter _entityParameter;
            private readonly ObjectParameter _objectParameter;

            internal ParameterBinder(EntityParameter entityParameter, ObjectParameter objectParameter)
            {
                this._entityParameter = entityParameter;
                this._objectParameter = objectParameter;
            }

            internal void OnDataReaderClosingHandler(object sender, EventArgs args)
            {
                this._objectParameter.Value = this._entityParameter.Value;
            }
        }
    }
}

