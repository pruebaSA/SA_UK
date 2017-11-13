namespace System.Data.Mapping.Update.Internal
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Data;
    using System.Data.Common.Utils;
    using System.Data.Mapping;
    using System.Data.Metadata.Edm;
    using System.Diagnostics;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Threading;

    internal class ViewLoader
    {
        private readonly Dictionary<EntitySetBase, System.Data.Common.Utils.Set<EntitySet>> m_affectedTables = new Dictionary<EntitySetBase, System.Data.Common.Utils.Set<EntitySet>>();
        private readonly Dictionary<AssociationSet, AssociationSetMetadata> m_associationSetMetadata = new Dictionary<AssociationSet, AssociationSetMetadata>();
        private readonly Dictionary<EntitySetBase, FunctionMappingTranslator> m_functionMappingTranslators = new Dictionary<EntitySetBase, FunctionMappingTranslator>(EqualityComparer<EntitySetBase>.Default);
        private readonly System.Data.Common.Utils.Set<EdmMember> m_isNullConditionProperties = new System.Data.Common.Utils.Set<EdmMember>();
        private readonly StorageMappingItemCollection m_mappingItemCollection;
        private readonly ReaderWriterLockSlim m_readerWriterLock = new ReaderWriterLockSlim();
        private readonly System.Data.Common.Utils.Set<EdmMember> m_serverGenProperties = new System.Data.Common.Utils.Set<EdmMember>();

        internal ViewLoader(StorageMappingItemCollection mappingItemCollection)
        {
            this.m_mappingItemCollection = mappingItemCollection;
        }

        private static IEnumerable<EdmMember> FindIsNullConditionColumns(StorageMappingFragment mappingFragment)
        {
            foreach (StorageConditionPropertyMapping iteratorVariable0 in FlattenPropertyMappings(mappingFragment.AllProperties).OfType<StorageConditionPropertyMapping>())
            {
                if ((iteratorVariable0.ColumnProperty != null) && iteratorVariable0.IsNull.HasValue)
                {
                    yield return iteratorVariable0.ColumnProperty;
                }
            }
        }

        private static IEnumerable<EdmMember> FindPropertiesMappedToColumns(System.Data.Common.Utils.Set<EdmMember> columns, StorageMappingFragment mappingFragment)
        {
            foreach (StorageScalarPropertyMapping iteratorVariable0 in FlattenPropertyMappings(mappingFragment.AllProperties).OfType<StorageScalarPropertyMapping>())
            {
                if (columns.Contains(iteratorVariable0.ColumnProperty))
                {
                    yield return iteratorVariable0.EdmProperty;
                }
            }
        }

        private static IEnumerable<EdmMember> FindServerGenMembers(StorageMappingFragment mappingFragment)
        {
            foreach (StorageScalarPropertyMapping iteratorVariable0 in FlattenPropertyMappings(mappingFragment.AllProperties).OfType<StorageScalarPropertyMapping>())
            {
                if (MetadataHelper.GetStoreGeneratedPattern(iteratorVariable0.ColumnProperty) != StoreGeneratedPattern.None)
                {
                    yield return iteratorVariable0.EdmProperty;
                }
            }
        }

        private static IEnumerable<StoragePropertyMapping> FlattenPropertyMappings(ReadOnlyCollection<StoragePropertyMapping> propertyMappings)
        {
            foreach (StoragePropertyMapping iteratorVariable0 in propertyMappings)
            {
                StorageComplexPropertyMapping iteratorVariable1 = iteratorVariable0 as StorageComplexPropertyMapping;
                if (iteratorVariable1 != null)
                {
                    foreach (StorageComplexTypeMapping iteratorVariable2 in iteratorVariable1.TypeMappings)
                    {
                        foreach (StoragePropertyMapping iteratorVariable3 in FlattenPropertyMappings(iteratorVariable2.AllProperties))
                        {
                            yield return iteratorVariable3;
                        }
                    }
                    continue;
                }
                yield return iteratorVariable0;
            }
        }

        internal System.Data.Common.Utils.Set<EntitySet> GetAffectedTables(EntitySetBase extent) => 
            this.SyncGetValue<EntitySetBase, System.Data.Common.Utils.Set<EntitySet>>(this.m_affectedTables, extent);

        internal AssociationSetMetadata GetAssociationSetMetadata(AssociationSet associationSet) => 
            this.SyncGetValue<AssociationSet, AssociationSetMetadata>(this.m_associationSetMetadata, associationSet);

        internal FunctionMappingTranslator GetFunctionMappingTranslator(EntitySetBase extent) => 
            this.SyncGetValue<EntitySetBase, FunctionMappingTranslator>(this.m_functionMappingTranslators, extent);

        private static IEnumerable<StorageMappingFragment> GetMappingFragments(StorageSetMapping setMapping)
        {
            foreach (StorageTypeMapping iteratorVariable0 in setMapping.TypeMappings)
            {
                foreach (StorageMappingFragment iteratorVariable1 in iteratorVariable0.MappingFragments)
                {
                    yield return iteratorVariable1;
                }
            }
        }

        private IEnumerable<EdmMember> GetMembersWithResultBinding(StorageEntitySetMapping entitySetMapping)
        {
            foreach (StorageEntityTypeFunctionMapping iteratorVariable0 in entitySetMapping.FunctionMappings)
            {
                if (iteratorVariable0.InsertFunctionMapping.ResultBindings != null)
                {
                    foreach (StorageFunctionResultBinding iteratorVariable1 in iteratorVariable0.InsertFunctionMapping.ResultBindings)
                    {
                        yield return iteratorVariable1.Property;
                    }
                }
                if (iteratorVariable0.UpdateFunctionMapping.ResultBindings != null)
                {
                    foreach (StorageFunctionResultBinding iteratorVariable2 in iteratorVariable0.UpdateFunctionMapping.ResultBindings)
                    {
                        yield return iteratorVariable2.Property;
                    }
                }
            }
        }

        private void InitializeEntitySet(EntitySetBase entitySetBase, MetadataWorkspace workspace)
        {
            this.m_mappingItemCollection.GetGeneratedView(entitySetBase, workspace);
            System.Data.Common.Utils.Set<EntitySet> set = new System.Data.Common.Utils.Set<EntitySet>();
            StorageEntityContainerMapping map = (StorageEntityContainerMapping) this.m_mappingItemCollection.GetMap(entitySetBase.EntityContainer);
            if (map != null)
            {
                StorageSetMapping entitySetMapping;
                System.Data.Common.Utils.Set<EdmMember> columns = new System.Data.Common.Utils.Set<EdmMember>();
                if (entitySetBase.BuiltInTypeKind == BuiltInTypeKind.EntitySet)
                {
                    entitySetMapping = map.GetEntitySetMapping(entitySetBase.Name);
                    this.m_serverGenProperties.Unite(this.GetMembersWithResultBinding((StorageEntitySetMapping) entitySetMapping));
                }
                else
                {
                    if (entitySetBase.BuiltInTypeKind != BuiltInTypeKind.AssociationSet)
                    {
                        throw EntityUtil.NotSupported();
                    }
                    entitySetMapping = map.GetRelationshipSetMapping(entitySetBase.Name);
                }
                foreach (StorageMappingFragment fragment in GetMappingFragments(entitySetMapping))
                {
                    set.Add(fragment.TableSet);
                    this.m_serverGenProperties.AddRange(FindServerGenMembers(fragment));
                    columns.AddRange(FindIsNullConditionColumns(fragment));
                }
                if (0 < columns.Count)
                {
                    foreach (StorageMappingFragment fragment2 in GetMappingFragments(entitySetMapping))
                    {
                        this.m_isNullConditionProperties.AddRange(FindPropertiesMappedToColumns(columns, fragment2));
                    }
                }
            }
            this.m_affectedTables.Add(entitySetBase, set.MakeReadOnly());
            this.InitializeFunctionMappingTranslators(entitySetBase, map);
            if (entitySetBase.BuiltInTypeKind == BuiltInTypeKind.AssociationSet)
            {
                AssociationSet key = (AssociationSet) entitySetBase;
                if (!this.m_associationSetMetadata.ContainsKey(key))
                {
                    this.m_associationSetMetadata.Add(key, new AssociationSetMetadata(this.m_affectedTables[key], key, workspace));
                }
            }
        }

        private void InitializeFunctionMappingTranslators(EntitySetBase entitySetBase, StorageEntityContainerMapping mapping)
        {
            KeyToListMap<AssociationSet, AssociationEndMember> map = new KeyToListMap<AssociationSet, AssociationEndMember>(EqualityComparer<AssociationSet>.Default);
            if (!this.m_functionMappingTranslators.ContainsKey(entitySetBase))
            {
                foreach (StorageEntitySetMapping mapping2 in mapping.EntitySetMaps)
                {
                    if (0 < mapping2.FunctionMappings.Count)
                    {
                        this.m_functionMappingTranslators.Add(mapping2.Set, FunctionMappingTranslator.CreateEntitySetFunctionMappingTranslator(mapping2));
                        foreach (AssociationSetEnd end in mapping2.ImplicitlyMappedAssociationSetEnds)
                        {
                            AssociationSet parentAssociationSet = end.ParentAssociationSet;
                            if (!this.m_functionMappingTranslators.ContainsKey(parentAssociationSet))
                            {
                                this.m_functionMappingTranslators.Add(parentAssociationSet, FunctionMappingTranslator.CreateAssociationSetFunctionMappingTranslator(null));
                            }
                            AssociationSetEnd oppositeEnd = MetadataHelper.GetOppositeEnd(end);
                            map.Add(parentAssociationSet, oppositeEnd.CorrespondingAssociationEndMember);
                        }
                    }
                    else
                    {
                        this.m_functionMappingTranslators.Add(mapping2.Set, null);
                    }
                }
                foreach (StorageAssociationSetMapping mapping3 in mapping.RelationshipSetMaps)
                {
                    if (mapping3.FunctionMapping != null)
                    {
                        AssociationSet set = (AssociationSet) mapping3.Set;
                        this.m_functionMappingTranslators.Add(set, FunctionMappingTranslator.CreateAssociationSetFunctionMappingTranslator(mapping3));
                        map.AddRange(set, Enumerable.Empty<AssociationEndMember>());
                    }
                    else if (!this.m_functionMappingTranslators.ContainsKey(mapping3.Set))
                    {
                        this.m_functionMappingTranslators.Add(mapping3.Set, null);
                    }
                }
            }
            foreach (AssociationSet set3 in map.Keys)
            {
                this.m_associationSetMetadata.Add(set3, new AssociationSetMetadata(map.EnumerateValues(set3)));
            }
        }

        internal bool IsNullConditionMember(EdmMember member) => 
            this.SyncContains<EdmMember>(this.m_isNullConditionProperties, member);

        internal bool IsServerGen(EdmMember member) => 
            this.SyncContains<EdmMember>(this.m_serverGenProperties, member);

        private bool SyncContains<T_Element>(System.Data.Common.Utils.Set<T_Element> set, T_Element element)
        {
            bool flag;
            this.m_readerWriterLock.EnterReadLock();
            try
            {
                flag = set.Contains(element);
            }
            finally
            {
                this.m_readerWriterLock.ExitReadLock();
            }
            return flag;
        }

        private T_Value SyncGetValue<T_Key, T_Value>(Dictionary<T_Key, T_Value> dictionary, T_Key key)
        {
            T_Value local;
            this.m_readerWriterLock.EnterReadLock();
            try
            {
                local = dictionary[key];
            }
            finally
            {
                this.m_readerWriterLock.ExitReadLock();
            }
            return local;
        }

        internal void SyncInitializeEntitySet(EntitySetBase entitySetBase, MetadataWorkspace workspace)
        {
            this.m_readerWriterLock.EnterReadLock();
            try
            {
                if (this.m_affectedTables.ContainsKey(entitySetBase))
                {
                    return;
                }
            }
            finally
            {
                this.m_readerWriterLock.ExitReadLock();
            }
            this.m_readerWriterLock.EnterWriteLock();
            try
            {
                if (!this.m_affectedTables.ContainsKey(entitySetBase))
                {
                    this.InitializeEntitySet(entitySetBase, workspace);
                }
            }
            finally
            {
                this.m_readerWriterLock.ExitWriteLock();
            }
        }






    }
}

