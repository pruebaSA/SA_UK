namespace System.Data.Mapping.Update.Internal
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Data.Common.CommandTrees;
    using System.Data.Common.Utils;
    using System.Data.Entity;
    using System.Data.EntityClient;
    using System.Data.Mapping;
    using System.Data.Metadata.Edm;
    using System.Data.Objects;
    using System.Diagnostics;
    using System.Globalization;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Threading;

    internal class UpdateTranslator
    {
        internal readonly IEqualityComparer<CompositeKey> KeyComparer;
        internal readonly System.Data.Mapping.Update.Internal.KeyManager KeyManager;
        private readonly Dictionary<EntitySetBase, ChangeNode> m_changes;
        private readonly int? m_commandTimeout;
        private readonly DbCommandTree m_commandTreeContext;
        private readonly EntityConnection m_connection;
        private readonly RelationshipConstraintValidator m_constraintValidator;
        private readonly Dictionary<StructuralType, ExtractorMetadata> m_extractorMetadata;
        private readonly Dictionary<EntitySetBase, List<ExtractedStateEntry>> m_functionChanges;
        private Dictionary<StorageFunctionMapping, DbCommandDefinition> m_functionCommandDefinitions;
        private readonly System.Data.Common.Utils.Set<EntityKey> m_includedValueEntities;
        private readonly System.Data.Common.Utils.Set<EntityKey> m_knownEntityKeys;
        private readonly System.Data.Common.Utils.Set<EntityKey> m_optionalEntities;
        private readonly DbProviderServices m_providerServices;
        private readonly System.Data.Mapping.Update.Internal.RecordConverter m_recordConverter;
        private readonly Dictionary<EntityKey, AssociationSet> m_requiredEntities;
        private readonly List<IEntityStateEntry> m_stateEntries;
        private readonly IEntityStateManager m_stateManager;
        private readonly System.Data.Mapping.Update.Internal.ViewLoader m_viewLoader;
        private static readonly List<string> s_emptyMemberList = new List<string>();

        private UpdateTranslator(IEntityStateManager stateManager, System.Data.Metadata.Edm.MetadataWorkspace metadataWorkspace, EntityConnection connection, int? commandTimeout)
        {
            EntityUtil.CheckArgumentNull<IEntityStateManager>(stateManager, "stateManager");
            EntityUtil.CheckArgumentNull<System.Data.Metadata.Edm.MetadataWorkspace>(metadataWorkspace, "metadataWorkspace");
            EntityUtil.CheckArgumentNull<EntityConnection>(connection, "connection");
            this.m_changes = new Dictionary<EntitySetBase, ChangeNode>();
            this.m_functionChanges = new Dictionary<EntitySetBase, List<ExtractedStateEntry>>();
            this.m_stateEntries = new List<IEntityStateEntry>();
            this.m_knownEntityKeys = new System.Data.Common.Utils.Set<EntityKey>();
            this.m_requiredEntities = new Dictionary<EntityKey, AssociationSet>();
            this.m_optionalEntities = new System.Data.Common.Utils.Set<EntityKey>();
            this.m_includedValueEntities = new System.Data.Common.Utils.Set<EntityKey>();
            this.m_commandTreeContext = new DbQueryCommandTree(metadataWorkspace, DataSpace.CSpace);
            this.m_viewLoader = metadataWorkspace.GetUpdateViewLoader();
            this.m_stateManager = stateManager;
            this.m_recordConverter = new System.Data.Mapping.Update.Internal.RecordConverter(this);
            this.m_constraintValidator = new RelationshipConstraintValidator(this);
            this.m_providerServices = DbProviderServices.GetProviderServices(connection.StoreProviderFactory);
            this.m_connection = connection;
            this.m_commandTimeout = commandTimeout;
            this.m_extractorMetadata = new Dictionary<StructuralType, ExtractorMetadata>(EqualityComparer<StructuralType>.Default);
            this.KeyManager = new System.Data.Mapping.Update.Internal.KeyManager(this);
            this.KeyComparer = CompositeKey.CreateComparer(this.KeyManager);
        }

        private int AcceptChanges(IEntityAdapter adapter)
        {
            int num = 0;
            foreach (IEntityStateEntry entry in this.m_stateEntries)
            {
                if (EntityState.Unchanged != entry.State)
                {
                    if (adapter.AcceptChangesDuringUpdate)
                    {
                        entry.AcceptChanges();
                    }
                    num++;
                }
            }
            return num;
        }

        private void AddValidAncillaryKey(EntityKey key, System.Data.Common.Utils.Set<EntityKey> keySet)
        {
            IEntityStateEntry entry;
            if ((this.m_stateManager.TryGetEntityStateEntry(key, out entry) && !entry.IsKeyEntry) && (entry.State == EntityState.Unchanged))
            {
                keySet.Add(key);
            }
        }

        private static object AlignReturnValue(object value, EdmMember member, PropagatorResult context)
        {
            if (DBNull.Value.Equals(value))
            {
                if ((BuiltInTypeKind.EdmProperty == member.BuiltInTypeKind) && !((EdmProperty) member).Nullable)
                {
                    throw EntityUtil.Update(System.Data.Entity.Strings.Update_NullReturnValueForNonNullableMember(member.Name, member.DeclaringType.FullName), null, new IEntityStateEntry[0]);
                }
                return value;
            }
            PrimitiveType edmType = (PrimitiveType) member.TypeUsage.EdmType;
            Type clrEquivalentType = edmType.ClrEquivalentType;
            try
            {
                value = Convert.ChangeType(value, clrEquivalentType, CultureInfo.InvariantCulture);
            }
            catch (Exception exception)
            {
                if (RequiresContext(exception))
                {
                    throw EntityUtil.Update(System.Data.Entity.Strings.Update_ReturnValueHasUnexpectedType(value.GetType().FullName, clrEquivalentType.FullName, member.Name, member.DeclaringType.FullName), exception, new IEntityStateEntry[0]);
                }
                throw;
            }
            return value;
        }

        private void BackPropagateServerGen(List<KeyValuePair<PropagatorResult, object>> generatedValues)
        {
            foreach (KeyValuePair<PropagatorResult, object> pair in generatedValues)
            {
                PropagatorResult key;
                if ((-1L == pair.Key.Identifier) || !this.KeyManager.TryGetIdentifierOwner(pair.Key.Identifier, out key))
                {
                    key = pair.Key;
                }
                if (key.RecordOrdinal != -1)
                {
                    object obj2 = pair.Value;
                    CurrentValueRecord record = key.Record;
                    IExtendedDataRecord record2 = record;
                    FieldMetadata metadata = record2.DataRecordInfo.FieldMetadata[key.RecordOrdinal];
                    EdmMember fieldType = metadata.FieldType;
                    obj2 = obj2 ?? DBNull.Value;
                    obj2 = AlignReturnValue(obj2, fieldType, key);
                    record.SetValue(key.RecordOrdinal, obj2);
                }
            }
        }

        internal DbCommand CreateCommand(DbModificationCommandTree commandTree)
        {
            DbCommand command;
            try
            {
                command = this.m_providerServices.CreateCommand(commandTree);
            }
            catch (Exception exception)
            {
                if (RequiresContext(exception))
                {
                    throw EntityUtil.CommandCompilation(System.Data.Entity.Strings.EntityClient_CommandDefinitionPreparationFailed, exception);
                }
                throw;
            }
            return command;
        }

        private IEnumerable<IEntityStateEntry> DetermineStateEntriesFromSource(UpdateCommand source) => 
            source?.GetStateEntries(this);

        internal DbCommandDefinition GenerateCommandDefinition(StorageFunctionMapping functionMapping)
        {
            DbCommandDefinition definition;
            if (this.m_functionCommandDefinitions == null)
            {
                this.m_functionCommandDefinitions = new Dictionary<StorageFunctionMapping, DbCommandDefinition>();
            }
            if (this.m_functionCommandDefinitions.TryGetValue(functionMapping, out definition))
            {
                return definition;
            }
            TypeUsage resultType = null;
            if ((functionMapping.ResultBindings != null) && (0 < functionMapping.ResultBindings.Count))
            {
                List<EdmProperty> properties = new List<EdmProperty>(functionMapping.ResultBindings.Count);
                foreach (StorageFunctionResultBinding binding in functionMapping.ResultBindings)
                {
                    properties.Add(new EdmProperty(binding.ColumnName, binding.Property.TypeUsage));
                }
                RowType elementType = new RowType(properties);
                CollectionType edmType = new CollectionType(elementType);
                resultType = TypeUsage.Create(edmType);
            }
            DbFunctionCommandTree commandTree = new DbFunctionCommandTree(this.m_commandTreeContext.MetadataWorkspace, DataSpace.SSpace, functionMapping.Function, resultType);
            foreach (FunctionParameter parameter in functionMapping.Function.Parameters)
            {
                commandTree.AddParameter(parameter.Name, parameter.TypeUsage);
            }
            return this.m_providerServices.CreateCommandDefinition(commandTree);
        }

        private IEnumerable<EntitySetBase> GetDynamicModifiedExtents() => 
            this.m_changes.Keys;

        internal List<ExtractedStateEntry> GetExtentFunctionModifications(EntitySetBase extent)
        {
            List<ExtractedStateEntry> list;
            EntityUtil.CheckArgumentNull<EntitySetBase>(extent, "extent");
            if (!this.m_functionChanges.TryGetValue(extent, out list))
            {
                list = new List<ExtractedStateEntry>();
                this.m_functionChanges.Add(extent, list);
            }
            return list;
        }

        internal ChangeNode GetExtentModifications(EntitySetBase extent)
        {
            ChangeNode node;
            EntityUtil.CheckArgumentNull<EntitySetBase>(extent, "extent");
            if (!this.m_changes.TryGetValue(extent, out node))
            {
                node = new ChangeNode(TypeUsage.Create(extent.ElementType));
                this.m_changes.Add(extent, node);
            }
            return node;
        }

        internal ExtractorMetadata GetExtractorMetadata(StructuralType type)
        {
            ExtractorMetadata metadata;
            if (!this.m_extractorMetadata.TryGetValue(type, out metadata))
            {
                metadata = new ExtractorMetadata(type, this);
                this.m_extractorMetadata.Add(type, metadata);
            }
            return metadata;
        }

        private IEnumerable<EntitySetBase> GetFunctionModifiedExtents() => 
            this.m_functionChanges.Keys;

        private static int GetKeyMemberOffset(RelationshipEndMember role, EdmProperty property, out int keyMemberCount)
        {
            RefType edmType = (RefType) role.TypeUsage.EdmType;
            EntityType elementType = (EntityType) edmType.ElementType;
            keyMemberCount = elementType.KeyMembers.Count;
            return elementType.KeyMembers.IndexOf(property);
        }

        internal IEnumerable<IEntityStateEntry> GetRelationships(EntityKey entityKey) => 
            this.m_stateManager.FindRelationshipsByKey(entityKey);

        private void LoadStateEntry(IEntityStateEntry stateEntry)
        {
            this.ValidateAndRegisterStateEntry(stateEntry);
            ExtractedStateEntry item = new ExtractedStateEntry(this, stateEntry);
            EntitySetBase entitySet = stateEntry.EntitySet;
            if (this.m_viewLoader.GetFunctionMappingTranslator(entitySet) == null)
            {
                ChangeNode extentModifications = this.GetExtentModifications(entitySet);
                if (item.Original != null)
                {
                    extentModifications.Deleted.Add(item.Original);
                }
                if (item.Current != null)
                {
                    extentModifications.Inserted.Add(item.Current);
                }
            }
            else
            {
                this.GetExtentFunctionModifications(entitySet).Add(item);
            }
        }

        private List<UpdateCommand> ProduceCommands()
        {
            this.PullModifiedEntriesFromStateManager();
            this.PullUnchangedEntriesFromStateManager();
            this.m_constraintValidator.ValidateConstraints();
            this.KeyManager.ValidateReferentialIntegrityGraphAcyclic();
            IEnumerable<UpdateCommand> first = this.ProduceDynamicCommands();
            IEnumerable<UpdateCommand> second = this.ProduceFunctionCommands();
            UpdateCommandOrderer orderer = new UpdateCommandOrderer(first.Concat<UpdateCommand>(second), this);
            List<UpdateCommand> list = new List<UpdateCommand>(orderer.Vertices.Count);
            foreach (UpdateCommand[] commandArray in orderer.TryStagedTopologicalSort())
            {
                Array.Sort<UpdateCommand>(commandArray);
                list.AddRange(commandArray);
            }
            this.ValidateGraphPostSort(orderer);
            return list;
        }

        private IEnumerable<UpdateCommand> ProduceDynamicCommands()
        {
            UpdateCompiler compiler = new UpdateCompiler(this);
            System.Data.Common.Utils.Set<EntitySet> iteratorVariable1 = new System.Data.Common.Utils.Set<EntitySet>();
            foreach (EntitySetBase base2 in this.GetDynamicModifiedExtents())
            {
                System.Data.Common.Utils.Set<EntitySet> affectedTables = this.m_viewLoader.GetAffectedTables(base2);
                if (affectedTables.Count == 0)
                {
                    throw EntityUtil.Update(System.Data.Entity.Strings.Update_MappingNotFound(base2.Name), null, new IEntityStateEntry[0]);
                }
                foreach (EntitySet set2 in affectedTables)
                {
                    iteratorVariable1.Add(set2);
                }
            }
            new List<TableChangeProcessor>(iteratorVariable1.Count);
            foreach (EntitySet iteratorVariable2 in iteratorVariable1)
            {
                DbQueryCommandTree cqtView = this.m_connection.GetMetadataWorkspace().GetCqtView(iteratorVariable2);
                ChangeNode changeNode = Propagator.Propagate(this, cqtView);
                TableChangeProcessor iteratorVariable5 = new TableChangeProcessor(iteratorVariable2);
                foreach (UpdateCommand iteratorVariable6 in iteratorVariable5.CompileCommands(changeNode, compiler))
                {
                    yield return iteratorVariable6;
                }
            }
        }

        private IEnumerable<UpdateCommand> ProduceFunctionCommands()
        {
            foreach (EntitySetBase iteratorVariable0 in this.GetFunctionModifiedExtents())
            {
                FunctionMappingTranslator functionMappingTranslator = this.m_viewLoader.GetFunctionMappingTranslator(iteratorVariable0);
                if (functionMappingTranslator != null)
                {
                    foreach (ExtractedStateEntry iteratorVariable2 in this.GetExtentFunctionModifications(iteratorVariable0))
                    {
                        FunctionUpdateCommand iteratorVariable3 = functionMappingTranslator.Translate(this, iteratorVariable2);
                        if (iteratorVariable3 != null)
                        {
                            yield return iteratorVariable3;
                        }
                    }
                }
            }
        }

        private void PullModifiedEntriesFromStateManager()
        {
            foreach (IEntityStateEntry entry in this.m_stateManager.GetEntityStateEntries(EntityState.Deleted | EntityState.Added))
            {
                this.RegisterReferentialConstraints(entry);
            }
            foreach (IEntityStateEntry entry2 in this.m_stateManager.GetEntityStateEntries(EntityState.Modified | EntityState.Deleted | EntityState.Added))
            {
                this.LoadStateEntry(entry2);
            }
        }

        private void PullUnchangedEntriesFromStateManager()
        {
            foreach (KeyValuePair<EntityKey, AssociationSet> pair in this.m_requiredEntities)
            {
                EntityKey element = pair.Key;
                if (!this.m_knownEntityKeys.Contains(element))
                {
                    IEntityStateEntry entry;
                    if (!this.m_stateManager.TryGetEntityStateEntry(element, out entry))
                    {
                        throw EntityUtil.UpdateMissingEntity(pair.Value.Name, TypeHelpers.GetFullName(element.EntityContainerName, element.EntitySetName));
                    }
                    this.LoadStateEntry(entry);
                }
            }
            foreach (EntityKey key2 in this.m_optionalEntities)
            {
                IEntityStateEntry entry2;
                if (!this.m_knownEntityKeys.Contains(key2) && this.m_stateManager.TryGetEntityStateEntry(key2, out entry2))
                {
                    this.LoadStateEntry(entry2);
                }
            }
            foreach (EntityKey key3 in this.m_includedValueEntities)
            {
                IEntityStateEntry entry3;
                if (!this.m_knownEntityKeys.Contains(key3) && this.m_stateManager.TryGetEntityStateEntry(key3, out entry3))
                {
                    this.m_recordConverter.ConvertCurrentValuesToPropagatorResult(entry3, null);
                }
            }
        }

        internal void RegisterReferentialConstraints(IEntityStateEntry stateEntry)
        {
            if (stateEntry.IsRelationship)
            {
                AssociationSet entitySet = (AssociationSet) stateEntry.EntitySet;
                if (0 < entitySet.ElementType.ReferentialConstraints.Count)
                {
                    DbDataRecord record = (stateEntry.State == EntityState.Added) ? stateEntry.CurrentValues : stateEntry.OriginalValues;
                    foreach (ReferentialConstraint constraint in entitySet.ElementType.ReferentialConstraints)
                    {
                        EntityKey entityKey = (EntityKey) record[constraint.FromRole.Name];
                        EntityKey key2 = (EntityKey) record[constraint.ToRole.Name];
                        using (ReadOnlyMetadataCollection<EdmProperty>.Enumerator enumerator = constraint.FromProperties.GetEnumerator())
                        {
                            using (ReadOnlyMetadataCollection<EdmProperty>.Enumerator enumerator2 = constraint.ToProperties.GetEnumerator())
                            {
                                while (enumerator.MoveNext() && enumerator2.MoveNext())
                                {
                                    int num;
                                    int num2;
                                    int memberOffset = GetKeyMemberOffset(constraint.FromRole, enumerator.Current, out num);
                                    int num4 = GetKeyMemberOffset(constraint.ToRole, enumerator2.Current, out num2);
                                    long principalIdentifier = this.KeyManager.GetKeyIdentifierForMemberOffset(entityKey, memberOffset, num);
                                    long dependentIdentifier = this.KeyManager.GetKeyIdentifierForMemberOffset(key2, num4, num2);
                                    this.KeyManager.AddReferentialConstraint(stateEntry, dependentIdentifier, principalIdentifier);
                                }
                            }
                        }
                    }
                }
            }
        }

        internal static bool RequiresContext(Exception e)
        {
            if (!EntityUtil.IsCatchableExceptionType(e))
            {
                return false;
            }
            return (!(e is UpdateException) && !(e is ProviderIncompatibleException));
        }

        internal static int Update(IEntityStateManager stateManager, IEntityAdapter adapter)
        {
            IntPtr ptr;
            int num3;
            EntityBid.ScopeEnter(out ptr, "<upd.Internal.UpdateTranslator.Update>");
            EntityConnection connection = (EntityConnection) adapter.Connection;
            System.Data.Metadata.Edm.MetadataWorkspace metadataWorkspace = connection.GetMetadataWorkspace();
            int? commandTimeout = adapter.CommandTimeout;
            try
            {
                UpdateTranslator translator = new UpdateTranslator(stateManager, metadataWorkspace, connection, commandTimeout);
                Dictionary<long, object> identifierValues = new Dictionary<long, object>();
                List<KeyValuePair<PropagatorResult, object>> generatedValues = new List<KeyValuePair<PropagatorResult, object>>();
                List<UpdateCommand> list2 = translator.ProduceCommands();
                UpdateCommand source = null;
                try
                {
                    foreach (UpdateCommand command2 in list2)
                    {
                        source = command2;
                        int rowsAffected = command2.Execute(translator, connection, identifierValues, generatedValues);
                        translator.ValidateRowsAffected(rowsAffected, source);
                    }
                }
                catch (Exception exception)
                {
                    if (RequiresContext(exception))
                    {
                        throw EntityUtil.Update(System.Data.Entity.Strings.Update_GeneralExecutionException, exception, translator.DetermineStateEntriesFromSource(source));
                    }
                    throw;
                }
                translator.BackPropagateServerGen(generatedValues);
                num3 = translator.AcceptChanges(adapter);
            }
            finally
            {
                EntityBid.ScopeLeave(ref ptr);
            }
            return num3;
        }

        private void ValidateAndRegisterStateEntry(IEntityStateEntry stateEntry)
        {
            EntityUtil.CheckArgumentNull<IEntityStateEntry>(stateEntry, "stateEntry");
            EntitySetBase entitySet = stateEntry.EntitySet;
            if (entitySet == null)
            {
                throw EntityUtil.InternalError(EntityUtil.InternalErrorCode.InvalidStateEntry, 1);
            }
            EntityKey entityKey = stateEntry.EntityKey;
            IExtendedDataRecord currentValues = null;
            if (((EntityState.Modified | EntityState.Added | EntityState.Unchanged) & stateEntry.State) != 0)
            {
                currentValues = stateEntry.CurrentValues;
                this.ValidateRecord(entitySet, currentValues, stateEntry);
            }
            if (((EntityState.Modified | EntityState.Deleted | EntityState.Unchanged) & stateEntry.State) != 0)
            {
                currentValues = (IExtendedDataRecord) stateEntry.OriginalValues;
                this.ValidateRecord(entitySet, currentValues, stateEntry);
            }
            this.m_viewLoader.SyncInitializeEntitySet(entitySet, this.MetadataWorkspace);
            AssociationSet associationSet = entitySet as AssociationSet;
            if (associationSet != null)
            {
                AssociationSetMetadata associationSetMetadata = this.m_viewLoader.GetAssociationSetMetadata(associationSet);
                if (associationSetMetadata.HasEnds)
                {
                    foreach (FieldMetadata metadata2 in currentValues.DataRecordInfo.FieldMetadata)
                    {
                        EntityKey key = (EntityKey) currentValues.GetValue(metadata2.Ordinal);
                        AssociationEndMember fieldType = (AssociationEndMember) metadata2.FieldType;
                        if (associationSetMetadata.RequiredEnds.Contains(fieldType))
                        {
                            if (!this.m_requiredEntities.ContainsKey(key))
                            {
                                this.m_requiredEntities.Add(key, associationSet);
                            }
                        }
                        else if (associationSetMetadata.OptionalEnds.Contains(fieldType))
                        {
                            this.AddValidAncillaryKey(key, this.m_optionalEntities);
                        }
                        else if (associationSetMetadata.IncludedValueEnds.Contains(fieldType))
                        {
                            this.AddValidAncillaryKey(key, this.m_includedValueEntities);
                        }
                    }
                }
                this.m_constraintValidator.RegisterAssociation(associationSet, currentValues, stateEntry);
            }
            else
            {
                this.m_constraintValidator.RegisterEntity(stateEntry);
            }
            this.m_stateEntries.Add(stateEntry);
            if (entityKey != null)
            {
                this.m_knownEntityKeys.Add(entityKey);
            }
        }

        private void ValidateGraphPostSort(UpdateCommandOrderer orderer)
        {
            if (orderer.Remainder.Count != 0)
            {
                System.Data.Common.Utils.Set<IEntityStateEntry> stateEntries = new System.Data.Common.Utils.Set<IEntityStateEntry>();
                foreach (UpdateCommand command in orderer.Remainder)
                {
                    stateEntries.AddRange(command.GetStateEntries(this));
                }
                throw EntityUtil.Update(System.Data.Entity.Strings.Update_ConstraintCycle, null, stateEntries);
            }
        }

        private void ValidateRecord(EntitySetBase extent, IExtendedDataRecord record, IEntityStateEntry entry)
        {
            DataRecordInfo info;
            if (((record == null) || ((info = record.DataRecordInfo) == null)) || (info.RecordType == null))
            {
                throw EntityUtil.InternalError(EntityUtil.InternalErrorCode.InvalidStateEntry, 2);
            }
            VerifyExtent(this.MetadataWorkspace, extent);
        }

        private void ValidateRowsAffected(int rowsAffected, UpdateCommand source)
        {
            if (rowsAffected == 0)
            {
                IEnumerable<IEntityStateEntry> stateEntries = this.DetermineStateEntriesFromSource(source);
                throw EntityUtil.UpdateConcurrency(rowsAffected, null, stateEntries);
            }
        }

        private static void VerifyExtent(System.Data.Metadata.Edm.MetadataWorkspace workspace, EntitySetBase extent)
        {
            EntityContainer entityContainer = extent.EntityContainer;
            EntityContainer container2 = null;
            if (entityContainer != null)
            {
                workspace.TryGetEntityContainer(entityContainer.Name, entityContainer.DataSpace, out container2);
            }
            if (((entityContainer == null) || (container2 == null)) || !object.ReferenceEquals(entityContainer, container2))
            {
                throw EntityUtil.Update(System.Data.Entity.Strings.Update_WorkspaceMismatch, null, new IEntityStateEntry[0]);
            }
        }

        internal int? CommandTimeout =>
            this.m_commandTimeout;

        internal System.Data.Metadata.Edm.MetadataWorkspace MetadataWorkspace =>
            this.m_commandTreeContext.MetadataWorkspace;

        internal System.Data.Mapping.Update.Internal.RecordConverter RecordConverter =>
            this.m_recordConverter;

        internal System.Data.Mapping.Update.Internal.ViewLoader ViewLoader =>
            this.m_viewLoader;



        private class RelationshipConstraintValidator
        {
            private readonly Dictionary<DirectionalRelationship, DirectionalRelationship> m_existingRelationships = new Dictionary<DirectionalRelationship, DirectionalRelationship>(EqualityComparer<DirectionalRelationship>.Default);
            private readonly Dictionary<DirectionalRelationship, IEntityStateEntry> m_impliedRelationships = new Dictionary<DirectionalRelationship, IEntityStateEntry>(EqualityComparer<DirectionalRelationship>.Default);
            private readonly Dictionary<EntitySet, List<AssociationSet>> m_referencingRelationshipSets = new Dictionary<EntitySet, List<AssociationSet>>(EqualityComparer<EntitySet>.Default);
            private readonly UpdateTranslator m_updateTranslator;

            internal RelationshipConstraintValidator(UpdateTranslator updateTranslator)
            {
                this.m_updateTranslator = updateTranslator;
            }

            private void AddExistingRelationship(DirectionalRelationship relationship)
            {
                DirectionalRelationship relationship2;
                if (this.m_existingRelationships.TryGetValue(relationship, out relationship2))
                {
                    relationship2.AddToEquivalenceSet(relationship);
                }
                else
                {
                    this.m_existingRelationships.Add(relationship, relationship);
                }
            }

            private int GetDirectionalRelationshipCountDelta(DirectionalRelationship expectedRelationship)
            {
                DirectionalRelationship relationship;
                if (this.m_existingRelationships.TryGetValue(expectedRelationship, out relationship))
                {
                    int num;
                    int num2;
                    relationship.GetCountsInEquivalenceSet(out num, out num2);
                    return (num - num2);
                }
                return 0;
            }

            private static EntityType GetEntityType(DbDataRecord dbDataRecord)
            {
                IExtendedDataRecord record = dbDataRecord as IExtendedDataRecord;
                return (EntityType) record.DataRecordInfo.RecordType.EdmType;
            }

            private IEnumerable<AssociationSet> GetReferencingAssocationSets(EntitySet entitySet)
            {
                List<AssociationSet> list;
                if (!this.m_referencingRelationshipSets.TryGetValue(entitySet, out list))
                {
                    list = new List<AssociationSet>();
                    foreach (EntitySetBase base2 in entitySet.EntityContainer.BaseEntitySets)
                    {
                        AssociationSet item = base2 as AssociationSet;
                        if (item != null)
                        {
                            foreach (AssociationSetEnd end in item.AssociationSetEnds)
                            {
                                if (end.EntitySet.Equals(entitySet))
                                {
                                    list.Add(item);
                                    break;
                                }
                            }
                        }
                    }
                    this.m_referencingRelationshipSets.Add(entitySet, list);
                }
                return list;
            }

            internal void RegisterAssociation(AssociationSet associationSet, IExtendedDataRecord record, IEntityStateEntry stateEntry)
            {
                EntityUtil.CheckArgumentNull<AssociationSet>(associationSet, "relationshipSet");
                EntityUtil.CheckArgumentNull<IExtendedDataRecord>(record, "record");
                EntityUtil.CheckArgumentNull<IEntityStateEntry>(stateEntry, "stateEntry");
                Dictionary<string, EntityKey> dictionary = new Dictionary<string, EntityKey>(StringComparer.Ordinal);
                foreach (FieldMetadata metadata in record.DataRecordInfo.FieldMetadata)
                {
                    string name = metadata.FieldType.Name;
                    EntityKey key = (EntityKey) record.GetValue(metadata.Ordinal);
                    dictionary.Add(name, key);
                }
                ReadOnlyMetadataCollection<AssociationSetEnd> associationSetEnds = associationSet.AssociationSetEnds;
                foreach (AssociationSetEnd end in associationSetEnds)
                {
                    foreach (AssociationSetEnd end2 in associationSetEnds)
                    {
                        if (!object.ReferenceEquals(end2.CorrespondingAssociationEndMember, end.CorrespondingAssociationEndMember))
                        {
                            EntityKey toEntityKey = dictionary[end2.CorrespondingAssociationEndMember.Name];
                            DirectionalRelationship relationship = new DirectionalRelationship(toEntityKey, end.CorrespondingAssociationEndMember, end2.CorrespondingAssociationEndMember, associationSet, stateEntry);
                            this.AddExistingRelationship(relationship);
                        }
                    }
                }
            }

            internal void RegisterEntity(IEntityStateEntry stateEntry)
            {
                EntityUtil.CheckArgumentNull<IEntityStateEntry>(stateEntry, "stateEntry");
                if ((EntityState.Added == stateEntry.State) || (EntityState.Deleted == stateEntry.State))
                {
                    EntityKey toEntityKey = EntityUtil.CheckArgumentNull<EntityKey>(stateEntry.EntityKey, "stateEntry.EntityKey");
                    EntitySet entitySet = (EntitySet) stateEntry.EntitySet;
                    EntityType otherType = (EntityState.Added == stateEntry.State) ? GetEntityType(stateEntry.CurrentValues) : GetEntityType(stateEntry.OriginalValues);
                    foreach (AssociationSet set2 in this.GetReferencingAssocationSets(entitySet))
                    {
                        ReadOnlyMetadataCollection<AssociationSetEnd> associationSetEnds = set2.AssociationSetEnds;
                        foreach (AssociationSetEnd end in associationSetEnds)
                        {
                            foreach (AssociationSetEnd end2 in associationSetEnds)
                            {
                                if ((!object.ReferenceEquals(end2.CorrespondingAssociationEndMember, end.CorrespondingAssociationEndMember) && end2.EntitySet.EdmEquals(entitySet)) && ((MetadataHelper.GetLowerBoundOfMultiplicity(end.CorrespondingAssociationEndMember.RelationshipMultiplicity) != 0) && MetadataHelper.GetEntityTypeForEnd(end2.CorrespondingAssociationEndMember).IsAssignableFrom(otherType)))
                                {
                                    DirectionalRelationship key = new DirectionalRelationship(toEntityKey, end.CorrespondingAssociationEndMember, end2.CorrespondingAssociationEndMember, set2, stateEntry);
                                    this.m_impliedRelationships.Add(key, stateEntry);
                                }
                            }
                        }
                    }
                }
            }

            internal void ValidateConstraints()
            {
                foreach (KeyValuePair<DirectionalRelationship, IEntityStateEntry> pair in this.m_impliedRelationships)
                {
                    DirectionalRelationship key = pair.Key;
                    IEntityStateEntry stateEntry = pair.Value;
                    int directionalRelationshipCountDelta = this.GetDirectionalRelationshipCountDelta(key);
                    if (EntityState.Deleted == stateEntry.State)
                    {
                        directionalRelationshipCountDelta = -directionalRelationshipCountDelta;
                    }
                    int lowerBoundOfMultiplicity = MetadataHelper.GetLowerBoundOfMultiplicity(key.FromEnd.RelationshipMultiplicity);
                    int? upperBoundOfMultiplicity = MetadataHelper.GetUpperBoundOfMultiplicity(key.FromEnd.RelationshipMultiplicity);
                    int num3 = upperBoundOfMultiplicity.HasValue ? upperBoundOfMultiplicity.Value : directionalRelationshipCountDelta;
                    if ((directionalRelationshipCountDelta < lowerBoundOfMultiplicity) || (directionalRelationshipCountDelta > num3))
                    {
                        throw EntityUtil.UpdateRelationshipCardinalityConstraintViolation(key.AssociationSet.Name, lowerBoundOfMultiplicity, upperBoundOfMultiplicity, TypeHelpers.GetFullName(key.ToEntityKey.EntityContainerName, key.ToEntityKey.EntitySetName), directionalRelationshipCountDelta, key.FromEnd.Name, stateEntry);
                    }
                }
                foreach (DirectionalRelationship relationship2 in this.m_existingRelationships.Keys)
                {
                    int num4;
                    int num5;
                    relationship2.GetCountsInEquivalenceSet(out num4, out num5);
                    int num6 = Math.Abs((int) (num4 - num5));
                    int num7 = MetadataHelper.GetLowerBoundOfMultiplicity(relationship2.FromEnd.RelationshipMultiplicity);
                    int? nullable2 = MetadataHelper.GetUpperBoundOfMultiplicity(relationship2.FromEnd.RelationshipMultiplicity);
                    if (nullable2.HasValue)
                    {
                        EntityState? nullable3 = null;
                        int? nullable4 = null;
                        if (num4 > nullable2.Value)
                        {
                            nullable3 = new EntityState?(EntityState.Added);
                            nullable4 = new int?(num4);
                        }
                        else if (num5 > nullable2.Value)
                        {
                            nullable3 = new EntityState?(EntityState.Deleted);
                            nullable4 = new int?(num5);
                        }
                        if (nullable3.HasValue)
                        {
                            throw EntityUtil.Update(System.Data.Entity.Strings.Update_RelationshipCardinalityViolation(nullable2.Value, nullable3.Value, relationship2.AssociationSet.ElementType.FullName, relationship2.FromEnd.Name, relationship2.ToEnd.Name, nullable4.Value), null, (IEnumerable<IEntityStateEntry>) (from reln in relationship2.GetEquivalenceSet() select reln.StateEntry));
                        }
                    }
                    if (((1 == num6) && (1 == num7)) && (1 == nullable2))
                    {
                        IEntityStateEntry entry2;
                        bool flag = num4 > num5;
                        if ((!this.m_impliedRelationships.TryGetValue(relationship2, out entry2) || (flag && (EntityState.Added != entry2.State))) || (!flag && (EntityState.Deleted != entry2.State)))
                        {
                            throw EntityUtil.UpdateEntityMissingConstraintViolation(relationship2.AssociationSet.Name, relationship2.ToEnd.Name, relationship2.StateEntry);
                        }
                    }
                }
            }

            private class DirectionalRelationship : IEquatable<UpdateTranslator.RelationshipConstraintValidator.DirectionalRelationship>
            {
                private UpdateTranslator.RelationshipConstraintValidator.DirectionalRelationship _equivalenceSetLinkedListNext;
                private readonly int _hashCode;
                internal readonly System.Data.Metadata.Edm.AssociationSet AssociationSet;
                internal readonly AssociationEndMember FromEnd;
                internal readonly IEntityStateEntry StateEntry;
                internal readonly AssociationEndMember ToEnd;
                internal readonly EntityKey ToEntityKey;

                internal DirectionalRelationship(EntityKey toEntityKey, AssociationEndMember fromEnd, AssociationEndMember toEnd, System.Data.Metadata.Edm.AssociationSet associationSet, IEntityStateEntry stateEntry)
                {
                    this.ToEntityKey = EntityUtil.CheckArgumentNull<EntityKey>(toEntityKey, "toEntityKey");
                    this.FromEnd = EntityUtil.CheckArgumentNull<AssociationEndMember>(fromEnd, "fromEnd");
                    this.ToEnd = EntityUtil.CheckArgumentNull<AssociationEndMember>(toEnd, "toEnd");
                    this.AssociationSet = EntityUtil.CheckArgumentNull<System.Data.Metadata.Edm.AssociationSet>(associationSet, "associationSet");
                    this.StateEntry = EntityUtil.CheckArgumentNull<IEntityStateEntry>(stateEntry, "stateEntry");
                    this._equivalenceSetLinkedListNext = this;
                    this._hashCode = ((toEntityKey.GetHashCode() ^ fromEnd.GetHashCode()) ^ toEnd.GetHashCode()) ^ associationSet.GetHashCode();
                }

                internal void AddToEquivalenceSet(UpdateTranslator.RelationshipConstraintValidator.DirectionalRelationship other)
                {
                    UpdateTranslator.RelationshipConstraintValidator.DirectionalRelationship relationship = this._equivalenceSetLinkedListNext;
                    this._equivalenceSetLinkedListNext = other;
                    other._equivalenceSetLinkedListNext = relationship;
                }

                public bool Equals(UpdateTranslator.RelationshipConstraintValidator.DirectionalRelationship other)
                {
                    if (!object.ReferenceEquals(this, other))
                    {
                        if (other == null)
                        {
                            return false;
                        }
                        if (this.ToEntityKey != other.ToEntityKey)
                        {
                            return false;
                        }
                        if (this.AssociationSet != other.AssociationSet)
                        {
                            return false;
                        }
                        if (this.ToEnd != other.ToEnd)
                        {
                            return false;
                        }
                        if (this.FromEnd != other.FromEnd)
                        {
                            return false;
                        }
                    }
                    return true;
                }

                public override bool Equals(object obj) => 
                    this.Equals(obj as UpdateTranslator.RelationshipConstraintValidator.DirectionalRelationship);

                internal void GetCountsInEquivalenceSet(out int addedCount, out int deletedCount)
                {
                    addedCount = 0;
                    deletedCount = 0;
                    UpdateTranslator.RelationshipConstraintValidator.DirectionalRelationship objA = this;
                    do
                    {
                        if (objA.StateEntry.State == EntityState.Added)
                        {
                            addedCount++;
                        }
                        else if (objA.StateEntry.State == EntityState.Deleted)
                        {
                            deletedCount++;
                        }
                        objA = objA._equivalenceSetLinkedListNext;
                    }
                    while (!object.ReferenceEquals(objA, this));
                }

                internal IEnumerable<UpdateTranslator.RelationshipConstraintValidator.DirectionalRelationship> GetEquivalenceSet()
                {
                    UpdateTranslator.RelationshipConstraintValidator.DirectionalRelationship objA = this;
                    do
                    {
                        yield return objA;
                        objA = objA._equivalenceSetLinkedListNext;
                    }
                    while (!object.ReferenceEquals(objA, this));
                }

                public override int GetHashCode() => 
                    this._hashCode;

                public override string ToString() => 
                    string.Format(CultureInfo.InvariantCulture, "{0}.{1}-->{2}: {3}", new object[] { this.AssociationSet.Name, this.FromEnd.Name, this.ToEnd.Name, StringUtil.BuildDelimitedList<EntityKeyMember>(this.ToEntityKey.EntityKeyValues, null, null) });

            }
        }
    }
}

