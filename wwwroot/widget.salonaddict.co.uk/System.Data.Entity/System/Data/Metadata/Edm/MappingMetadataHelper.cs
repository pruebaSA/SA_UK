namespace System.Data.Metadata.Edm
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Data;
    using System.Data.Common.Utils;
    using System.Data.Entity;
    using System.Data.Mapping;
    using System.Diagnostics;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Threading;

    internal static class MappingMetadataHelper
    {
        internal static StorageEntityContainerMapping GetEntityContainerMap(StorageMappingItemCollection mappingCollection, EntityContainer entityContainer)
        {
            ReadOnlyCollection<StorageEntityContainerMapping> items = mappingCollection.GetItems<StorageEntityContainerMapping>();
            StorageEntityContainerMapping mapping = null;
            foreach (StorageEntityContainerMapping mapping2 in items)
            {
                if (entityContainer.Equals(mapping2.EdmEntityContainer) || entityContainer.Equals(mapping2.StorageEntityContainer))
                {
                    mapping = mapping2;
                    break;
                }
            }
            if (mapping == null)
            {
                throw new MappingException(System.Data.Entity.Strings.Mapping_NotFound_EntityContainer(entityContainer.Name));
            }
            return mapping;
        }

        internal static IEnumerable<StorageEntityTypeFunctionMapping> GetFunctionMappingsForEntitySetAndType(StorageMappingItemCollection mappingCollection, EntityContainer container, EntitySetBase entitySet, EntityTypeBase entityType)
        {
            Func<StorageEntityTypeFunctionMapping, bool> predicate = null;
            StorageSetMapping setMapping = GetEntityContainerMap(mappingCollection, container).GetSetMapping(entitySet.Name);
            StorageEntitySetMapping iteratorVariable2 = setMapping as StorageEntitySetMapping;
            if ((iteratorVariable2 != null) && (iteratorVariable2 != null))
            {
                if (predicate == null)
                {
                    predicate = functionMap => functionMap.EntityType.Equals(entityType);
                }
                foreach (StorageEntityTypeFunctionMapping iteratorVariable3 in iteratorVariable2.FunctionMappings.Where<StorageEntityTypeFunctionMapping>(predicate))
                {
                    yield return iteratorVariable3;
                }
            }
        }

        private static IEnumerable<StorageTypeMapping> GetIsTypeOfMappingsForEntitySetAndType(StorageMappingItemCollection mappingCollection, EntityContainer container, EntitySetBase entitySet, EntityTypeBase entityType, EntityTypeBase childEntityType)
        {
            Func<EdmType, bool> predicate = null;
            foreach (StorageTypeMapping iteratorVariable0 in GetMappingsForEntitySetAndType(mappingCollection, container, entitySet, entityType))
            {
                if (predicate == null)
                {
                    predicate = parentType => parentType.IsAssignableFrom(childEntityType);
                }
                if (iteratorVariable0.IsOfTypes.Any<EdmType>(predicate) || iteratorVariable0.Types.Contains(childEntityType))
                {
                    yield return iteratorVariable0;
                }
            }
        }

        internal static IEnumerable<StorageTypeMapping> GetMappingsForEntitySetAndSuperTypes(StorageMappingItemCollection mappingCollection, EntityContainer container, EntitySetBase entitySet, EntityTypeBase childEntityType) => 
            MetadataHelper.GetTypeAndParentTypesOf(childEntityType, mappingCollection.EdmItemCollection, true).SelectMany<EdmType, StorageTypeMapping>(delegate (EdmType edmType) {
                if (!edmType.EdmEquals(childEntityType))
                {
                    return GetIsTypeOfMappingsForEntitySetAndType(mappingCollection, container, entitySet, edmType as EntityTypeBase, childEntityType);
                }
                return GetMappingsForEntitySetAndType(mappingCollection, container, entitySet, edmType as EntityTypeBase);
            }).ToList<StorageTypeMapping>();

        internal static IEnumerable<StorageTypeMapping> GetMappingsForEntitySetAndType(StorageMappingItemCollection mappingCollection, EntityContainer container, EntitySetBase entitySet, EntityTypeBase entityType)
        {
            Func<StorageTypeMapping, bool> predicate = null;
            StorageSetMapping setMapping = GetEntityContainerMap(mappingCollection, container).GetSetMapping(entitySet.Name);
            if (setMapping != null)
            {
                if (predicate == null)
                {
                    predicate = map => map.Types.Union<EdmType>(map.IsOfTypes).Contains<EdmType>(entityType);
                }
                foreach (StorageTypeMapping iteratorVariable2 in setMapping.TypeMappings.Where<StorageTypeMapping>(predicate))
                {
                    yield return iteratorVariable2;
                }
            }
        }



    }
}

