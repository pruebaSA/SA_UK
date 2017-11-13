namespace System.Data.Query.InternalTrees
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Data.Mapping;
    using System.Data.Metadata.Edm;
    using System.Runtime.InteropServices;

    internal static class ColumnMapFactory
    {
        private static CollectionColumnMap CreateColumnMapFromReaderAndType(DbDataReader storeDataReader, EdmType edmType, EntitySet entitySet)
        {
            if (!Helper.IsEntityType(edmType) && (entitySet != null))
            {
                throw EntityUtil.EntitySetForNonEntityType();
            }
            ColumnMap[] columnMapsForType = GetColumnMapsForType(storeDataReader, edmType);
            ColumnMap elementMap = null;
            if (Helper.IsRowType(edmType))
            {
                elementMap = new RecordColumnMap(TypeUsage.Create(edmType), edmType.Name, columnMapsForType, null);
            }
            else if (Helper.IsComplexType(edmType))
            {
                elementMap = new ComplexTypeColumnMap(TypeUsage.Create(edmType), edmType.Name, columnMapsForType, null);
            }
            else if (Helper.IsPrimitiveType(edmType))
            {
                if (storeDataReader.FieldCount != 1)
                {
                    throw EntityUtil.CommandExecutionDataReaderFieldCountForPrimitiveType();
                }
                elementMap = new ScalarColumnMap(TypeUsage.Create(edmType), edmType.Name, 0, 0);
            }
            else if (Helper.IsEntityType(edmType))
            {
                elementMap = CreateEntityTypeElementColumnMap(storeDataReader, edmType, entitySet, columnMapsForType);
            }
            return new SimpleCollectionColumnMap(edmType.GetCollectionType().TypeUsage, edmType.Name, elementMap, null, null, null);
        }

        private static ScalarColumnMap[] CreateDiscriminatorColumnMaps(DbDataReader storeDataReader, FunctionImportMapping mapping)
        {
            TypeUsage type = TypeUsage.Create(MetadataItem.EdmProviderManifest.GetPrimitiveType(PrimitiveTypeKind.String));
            ScalarColumnMap[] mapArray = new ScalarColumnMap[mapping.DiscriminatorColumns.Count];
            for (int i = 0; i < mapArray.Length; i++)
            {
                string name = mapping.DiscriminatorColumns[i];
                mapArray[i] = new ScalarColumnMap(type, name, 0, GetDiscriminatorOrdinalFromReader(storeDataReader, name, mapping.FunctionImport));
            }
            return mapArray;
        }

        private static EntityColumnMap CreateEntityTypeElementColumnMap(DbDataReader storeDataReader, EdmType edmType, EntitySet entitySet, ColumnMap[] propertyColumnMaps)
        {
            SimpleEntityIdentity noEntityIdentity = EntityIdentity.NoEntityIdentity;
            if (entitySet != null)
            {
                EntityType elementType = entitySet.ElementType;
                ColumnMap[] mapArray = new ColumnMap[storeDataReader.FieldCount];
                foreach (ColumnMap map in propertyColumnMaps)
                {
                    int columnPos = ((ScalarColumnMap) map).ColumnPos;
                    mapArray[columnPos] = map;
                }
                IList<EdmMember> keyMembers = elementType.KeyMembers;
                SimpleColumnMap[] keyColumns = new SimpleColumnMap[keyMembers.Count];
                int index = 0;
                foreach (EdmMember member in keyMembers)
                {
                    int memberOrdinalFromReader = GetMemberOrdinalFromReader(storeDataReader, member);
                    ColumnMap map2 = mapArray[memberOrdinalFromReader];
                    keyColumns[index] = (SimpleColumnMap) map2;
                    index++;
                }
                noEntityIdentity = new SimpleEntityIdentity(entitySet, keyColumns);
            }
            return new EntityColumnMap(TypeUsage.Create(edmType), edmType.Name, propertyColumnMaps, noEntityIdentity);
        }

        internal static CollectionColumnMap CreateFunctionImportEntityColumnMap(DbDataReader storeDataReader, FunctionImportMapping mapping, EntitySet entitySet, EntityType baseEntityType)
        {
            if (mapping.NormalizedEntityTypeMappings.Count == 0)
            {
                return CreateColumnMapFromReaderAndType(storeDataReader, baseEntityType, entitySet);
            }
            ScalarColumnMap[] typeDiscriminators = CreateDiscriminatorColumnMaps(storeDataReader, mapping);
            HashSet<EntityType> set = new HashSet<EntityType>(mapping.MappedEntityTypes) {
                baseEntityType
            };
            Dictionary<EntityType, TypedColumnMap> typeChoices = new Dictionary<EntityType, TypedColumnMap>(set.Count);
            ColumnMap[] baseTypeColumns = null;
            foreach (EntityType type in set)
            {
                ColumnMap[] columnMapsForType = GetColumnMapsForType(storeDataReader, type);
                EntityColumnMap map = CreateEntityTypeElementColumnMap(storeDataReader, type, entitySet, columnMapsForType);
                typeChoices.Add(type, map);
                if (type == baseEntityType)
                {
                    baseTypeColumns = columnMapsForType;
                }
            }
            return new SimpleCollectionColumnMap(baseEntityType.GetCollectionType().TypeUsage, baseEntityType.Name, new MultipleDiscriminatorPolymorphicColumnMap(TypeUsage.Create(baseEntityType), baseEntityType.Name, baseTypeColumns, typeDiscriminators, typeChoices, new Func<object[], EntityType>(mapping.Discriminate)), null, null, null);
        }

        private static ColumnMap[] GetColumnMapsForType(DbDataReader storeDataReader, EdmType edmType)
        {
            IBaseList<EdmMember> allStructuralMembers = TypeHelpers.GetAllStructuralMembers(edmType);
            ColumnMap[] mapArray = new ColumnMap[allStructuralMembers.Count];
            int index = 0;
            foreach (EdmMember member in allStructuralMembers)
            {
                int memberOrdinalFromReader = GetMemberOrdinalFromReader(storeDataReader, member);
                mapArray[index] = new ScalarColumnMap(member.TypeUsage, member.Name, 0, memberOrdinalFromReader);
                index++;
            }
            return mapArray;
        }

        private static int GetDiscriminatorOrdinalFromReader(DbDataReader storeDataReader, string columnName, EdmFunction functionImport)
        {
            int num;
            if (!TryGetColumnOrdinalFromReader(storeDataReader, columnName, out num))
            {
                throw EntityUtil.CommandExecutionDataReaderMissinDiscriminatorColumn(columnName, functionImport);
            }
            return num;
        }

        private static int GetMemberOrdinalFromReader(DbDataReader storeDataReader, EdmMember member)
        {
            int num;
            if (!TryGetColumnOrdinalFromReader(storeDataReader, member.Name, out num))
            {
                throw EntityUtil.CommandExecutionDataReaderMissingColumnForType(member);
            }
            return num;
        }

        private static bool TryGetColumnOrdinalFromReader(DbDataReader storeDataReader, string columnName, out int ordinal)
        {
            if (storeDataReader.FieldCount == 0)
            {
                ordinal = 0;
                return false;
            }
            try
            {
                ordinal = storeDataReader.GetOrdinal(columnName);
                return true;
            }
            catch (IndexOutOfRangeException)
            {
                ordinal = 0;
                return false;
            }
        }
    }
}

