namespace System.Data.Metadata.Edm
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Data;
    using System.Data.Common.CommandTrees;
    using System.Data.Common.QueryCache;
    using System.Data.Common.Utils;
    using System.Data.Entity;
    using System.Data.Mapping;
    using System.Data.Mapping.Update.Internal;
    using System.Data.Mapping.ViewGeneration;
    using System.Data.Objects.DataClasses;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.InteropServices;

    public sealed class MetadataWorkspace
    {
        private List<object> _cacheTokens;
        private EdmItemCollection _itemsCSpace;
        private StorageMappingItemCollection _itemsCSSpace;
        private DefaultObjectMappingItemCollection _itemsOCSpace;
        private ObjectItemCollection _itemsOSpace;
        private StoreItemCollection _itemsSSpace;
        public static readonly double MaximumEdmVersionSupported = 1.0;

        public MetadataWorkspace()
        {
        }

        public MetadataWorkspace(IEnumerable<string> paths, IEnumerable<Assembly> assembliesToConsider)
        {
            Func<AssemblyName, Assembly> func2 = null;
            Func<IEnumerable<Assembly>> wildcardAssemblies = null;
            EntityUtil.CheckArgumentNull<IEnumerable<string>>(paths, "paths");
            EntityUtil.CheckArgumentContainsNull<Assembly>(ref assembliesToConsider, "assembliesToConsider");
            if (func2 == null)
            {
                func2 = delegate (AssemblyName referenceName) {
                    foreach (Assembly assembly in assembliesToConsider)
                    {
                        if (AssemblyName.ReferenceMatchesDefinition(referenceName, assembly.GetName()))
                        {
                            return assembly;
                        }
                    }
                    throw EntityUtil.Argument(System.Data.Entity.Strings.AssemblyMissingFromAssembliesToConsider(referenceName.FullName), "assembliesToConsider");
                };
            }
            Func<AssemblyName, Assembly> resolveReference = func2;
            if (wildcardAssemblies == null)
            {
                wildcardAssemblies = () => assembliesToConsider;
            }
            this.CreateMetadataWorkspaceWithResolver(paths, wildcardAssemblies, resolveReference);
        }

        internal void AddMetadataEntryToken(object token)
        {
            if (this._cacheTokens == null)
            {
                this._cacheTokens = new List<object>();
            }
            this._cacheTokens.Add(token);
        }

        public static void ClearCache()
        {
            MetadataCache.Clear();
            ObjectItemCollection.ViewGenerationAssemblies.Clear();
        }

        private void CreateMetadataWorkspaceWithResolver(IEnumerable<string> paths, Func<IEnumerable<Assembly>> wildcardAssemblies, Func<AssemblyName, Assembly> resolveReference)
        {
            EntityUtil.CheckArgumentNull<IEnumerable<string>>(paths, "paths");
            EntityUtil.CheckArgumentNull<Func<IEnumerable<Assembly>>>(wildcardAssemblies, "wildcardAssemblies");
            EntityUtil.CheckArgumentNull<Func<AssemblyName, Assembly>>(resolveReference, "resolveReference");
            MetadataArtifactLoader loader = MetadataArtifactLoader.CreateCompositeFromFilePaths(paths.ToArray<string>(), "", new CustomAssemblyResolver(wildcardAssemblies, resolveReference));
            DataSpace cSpace = DataSpace.CSpace;
            using (DisposableCollectionWrapper<XmlReader> wrapper = new DisposableCollectionWrapper<XmlReader>(loader.CreateReaders(cSpace)))
            {
                if (wrapper.Any<XmlReader>())
                {
                    this._itemsCSpace = new EdmItemCollection(wrapper, loader.GetPaths(cSpace));
                }
            }
            cSpace = DataSpace.SSpace;
            using (DisposableCollectionWrapper<XmlReader> wrapper2 = new DisposableCollectionWrapper<XmlReader>(loader.CreateReaders(cSpace)))
            {
                if (wrapper2.Any<XmlReader>())
                {
                    this._itemsSSpace = new StoreItemCollection(wrapper2, loader.GetPaths(cSpace));
                }
            }
            cSpace = DataSpace.CSSpace;
            using (DisposableCollectionWrapper<XmlReader> wrapper3 = new DisposableCollectionWrapper<XmlReader>(loader.CreateReaders(cSpace)))
            {
                if ((wrapper3.Any<XmlReader>() && (this._itemsCSpace != null)) && (this._itemsSSpace != null))
                {
                    this._itemsCSSpace = new StorageMappingItemCollection(this._itemsCSpace, this._itemsSSpace, wrapper3, loader.GetPaths(cSpace));
                }
            }
        }

        internal TypeUsage GetCanonicalModelTypeUsage(PrimitiveTypeKind primitiveTypeKind) => 
            EdmProviderManifest.Instance.GetCanonicalModelTypeUsage(primitiveTypeKind);

        internal DbQueryCommandTree GetCqtView(EntitySetBase extent) => 
            this.GetGeneratedView(extent).GetCommandTree(this);

        public StructuralType GetEdmSpaceType(StructuralType objectSpaceType)
        {
            StructuralType type;
            if (!this.TryGetEdmSpaceType(objectSpaceType, out type))
            {
                throw EntityUtil.Argument(System.Data.Entity.Strings.FailedToFindCSpaceTypeMapping(objectSpaceType.Identity));
            }
            return type;
        }

        public EntityContainer GetEntityContainer(string name, DataSpace dataSpace) => 
            this.GetItemCollection(dataSpace, true).GetEntityContainer(name);

        public EntityContainer GetEntityContainer(string name, bool ignoreCase, DataSpace dataSpace) => 
            this.GetItemCollection(dataSpace, true).GetEntityContainer(name, ignoreCase);

        public ReadOnlyCollection<EdmFunction> GetFunctions(string name, string namespaceName, DataSpace dataSpace) => 
            this.GetFunctions(name, namespaceName, dataSpace, false);

        public ReadOnlyCollection<EdmFunction> GetFunctions(string name, string namespaceName, DataSpace dataSpace, bool ignoreCase)
        {
            EntityUtil.CheckStringArgument(name, "name");
            EntityUtil.CheckStringArgument(namespaceName, "namespaceName");
            return this.GetItemCollection(dataSpace, true).GetFunctions(namespaceName + "." + name, ignoreCase);
        }

        internal GeneratedView GetGeneratedView(EntitySetBase extent) => 
            ((StorageMappingItemCollection) this.GetItemCollection(DataSpace.CSSpace, true)).GetGeneratedView(extent, this);

        public T GetItem<T>(string identity, DataSpace dataSpace) where T: GlobalItem => 
            this.GetItemCollection(dataSpace, true).GetItem<T>(identity, false);

        public T GetItem<T>(string identity, bool ignoreCase, DataSpace dataSpace) where T: GlobalItem => 
            this.GetItemCollection(dataSpace, true).GetItem<T>(identity, ignoreCase);

        public ItemCollection GetItemCollection(DataSpace dataSpace) => 
            this.GetItemCollection(dataSpace, true);

        internal ItemCollection GetItemCollection(DataSpace dataSpace, bool required)
        {
            ItemCollection items;
            switch (dataSpace)
            {
                case DataSpace.OSpace:
                    items = this._itemsOSpace;
                    break;

                case DataSpace.CSpace:
                    items = this._itemsCSpace;
                    break;

                case DataSpace.SSpace:
                    items = this._itemsSSpace;
                    break;

                case DataSpace.OCSpace:
                    items = this._itemsOCSpace ?? this.RegisterDefaultObjectMappingItemCollection();
                    break;

                case DataSpace.CSSpace:
                    items = this._itemsCSSpace;
                    break;

                default:
                    items = null;
                    break;
            }
            if (required && (items == null))
            {
                throw EntityUtil.NoCollectionForSpace(dataSpace);
            }
            return items;
        }

        public ReadOnlyCollection<GlobalItem> GetItems(DataSpace dataSpace) => 
            this.GetItemCollection(dataSpace, true).GetItems<GlobalItem>();

        public ReadOnlyCollection<T> GetItems<T>(DataSpace dataSpace) where T: GlobalItem => 
            this.GetItemCollection(dataSpace, true).GetItems<T>();

        internal Map GetMap(GlobalItem item, DataSpace dataSpace) => 
            ((MappingItemCollection) this.GetItemCollection(dataSpace, true)).GetMap(item);

        internal Map GetMap(string identity, DataSpace typeSpace, DataSpace dataSpace) => 
            ((MappingItemCollection) this.GetItemCollection(dataSpace, true)).GetMap(identity, typeSpace);

        internal PrimitiveType GetMappedPrimitiveType(PrimitiveTypeKind primitiveTypeKind, DataSpace dataSpace) => 
            this.GetItemCollection(dataSpace, true).GetMappedPrimitiveType(primitiveTypeKind);

        internal PrimitiveType GetModelPrimitiveType(PrimitiveTypeKind primitiveTypeKind) => 
            EdmProviderManifest.Instance.GetPrimitiveType(primitiveTypeKind);

        public StructuralType GetObjectSpaceType(StructuralType edmSpaceType)
        {
            StructuralType type;
            if (!this.TryGetObjectSpaceType(edmSpaceType, out type))
            {
                throw EntityUtil.Argument(System.Data.Entity.Strings.FailedToFindOSpaceTypeMapping(edmSpaceType.Identity));
            }
            return type;
        }

        internal TypeUsage GetOSpaceTypeUsage(TypeUsage edmSpaceTypeUsage)
        {
            EntityUtil.CheckArgumentNull<TypeUsage>(edmSpaceTypeUsage, "edmSpaceTypeUsage");
            if (edmSpaceTypeUsage.EdmType == null)
            {
                throw EntityUtil.TypeUsageHasNoEdmType("edmSpaceTypeUsage");
            }
            EdmType edmType = null;
            if (Helper.IsPrimitiveType(edmSpaceTypeUsage.EdmType))
            {
                edmType = this.GetItemCollection(DataSpace.OSpace, true).GetMappedPrimitiveType(((PrimitiveType) edmSpaceTypeUsage.EdmType).PrimitiveTypeKind);
            }
            else
            {
                edmType = ((ObjectTypeMapping) ((DefaultObjectMappingItemCollection) this.GetItemCollection(DataSpace.OCSpace, true)).GetMap(edmSpaceTypeUsage.EdmType)).ClrType;
            }
            return TypeUsage.Create(edmType, edmSpaceTypeUsage.Facets);
        }

        public ReadOnlyCollection<PrimitiveType> GetPrimitiveTypes(DataSpace dataSpace) => 
            this.GetItemCollection(dataSpace, true).GetItems<PrimitiveType>();

        internal QueryCacheManager GetQueryCacheManager() => 
            this._itemsSSpace.QueryCacheManager;

        public IEnumerable<EdmMember> GetRequiredOriginalValueMembers(EntitySetBase entitySet, EntityTypeBase entityType)
        {
            EntityUtil.CheckArgumentNull<EntitySetBase>(entitySet, "entitySet");
            EntityUtil.CheckArgumentNull<EntityTypeBase>(entityType, "entityType");
            if (entitySet.EntityContainer.GetDataSpace() != DataSpace.CSpace)
            {
                if (entitySet is AssociationSet)
                {
                    throw EntityUtil.AssociationSetNotInCSpace(entitySet.Name);
                }
                throw EntityUtil.EntitySetNotInCSpace(entitySet.Name);
            }
            if (!entitySet.ElementType.IsAssignableFrom(entityType))
            {
                if (entitySet is AssociationSet)
                {
                    throw EntityUtil.TypeNotInAssociationSet(entitySet.Name, entitySet.ElementType.FullName, entityType.FullName);
                }
                throw EntityUtil.TypeNotInEntitySet(entitySet.Name, entitySet.ElementType.FullName, entityType.FullName);
            }
            StorageMappingItemCollection itemCollection = (StorageMappingItemCollection) this.GetItemCollection(DataSpace.CSSpace, true);
            return itemCollection.GetRequiredOriginalValueMembers(new Pair<EntitySetBase, EntityTypeBase>(entitySet, entityType));
        }

        public EdmType GetType(string name, string namespaceName, DataSpace dataSpace) => 
            this.GetItemCollection(dataSpace, true).GetType(name, namespaceName, false);

        public EdmType GetType(string name, string namespaceName, bool ignoreCase, DataSpace dataSpace) => 
            this.GetItemCollection(dataSpace, true).GetType(name, namespaceName, ignoreCase);

        internal ViewLoader GetUpdateViewLoader()
        {
            if (this._itemsCSSpace != null)
            {
                return this._itemsCSSpace.GetUpdateViewLoader();
            }
            return null;
        }

        internal bool IsItemCollectionAlreadyRegistered(DataSpace dataSpace)
        {
            ItemCollection items;
            return this.TryGetItemCollection(dataSpace, out items);
        }

        internal bool IsMetadataWorkspaceCSCompatible(MetadataWorkspace other) => 
            this._itemsCSSpace.MetadataEquals(other._itemsCSSpace);

        internal void LoadAssemblyForType(Type type, Assembly callingAssembly)
        {
            ItemCollection items;
            if (this.TryGetItemCollection(DataSpace.OSpace, out items))
            {
                ObjectItemCollection items2 = (ObjectItemCollection) items;
                if (!items2.LoadAssemblyForType(type) && (callingAssembly != null))
                {
                    items2.LoadAllReferencedAssemblies(callingAssembly);
                }
            }
        }

        public void LoadFromAssembly(Assembly assembly)
        {
            EntityUtil.CheckArgumentNull<Assembly>(assembly, "assembly");
            ((ObjectItemCollection) this.GetItemCollection(DataSpace.OSpace)).LoadFromAssembly(assembly);
        }

        internal void LoadFromEntityType(EntityType type, Assembly callingAssembly)
        {
            Map map;
            if (!this.TryGetMap(type, DataSpace.OCSpace, out map))
            {
                this.LoadAssemblyForType(typeof(IEntityWithKey), callingAssembly);
            }
        }

        private ItemCollection RegisterDefaultObjectMappingItemCollection()
        {
            EdmItemCollection edmCollection = this._itemsCSpace;
            ObjectItemCollection objectCollection = this._itemsOSpace;
            if ((edmCollection != null) && (objectCollection != null))
            {
                this.RegisterItemCollection(new DefaultObjectMappingItemCollection(edmCollection, objectCollection));
            }
            return this._itemsOCSpace;
        }

        public void RegisterItemCollection(ItemCollection collection)
        {
            ItemCollection items;
            EntityUtil.CheckArgumentNull<ItemCollection>(collection, "collection");
            try
            {
                EdmItemCollection items2;
                switch (collection.DataSpace)
                {
                    case DataSpace.OSpace:
                        items = this._itemsOSpace;
                        if (items == null)
                        {
                            this._itemsOSpace = (ObjectItemCollection) collection;
                        }
                        goto Label_00E9;

                    case DataSpace.CSpace:
                        items = this._itemsCSpace;
                        if (items == null)
                        {
                            items2 = (EdmItemCollection) collection;
                            if (items2.EdmVersion > MaximumEdmVersionSupported)
                            {
                                throw EntityUtil.InvalidOperation(System.Data.Entity.Strings.EdmVersionNotSupportedByRuntime(items2.EdmVersion, MaximumEdmVersionSupported));
                            }
                            break;
                        }
                        goto Label_00E9;

                    case DataSpace.SSpace:
                        items = this._itemsSSpace;
                        if (items == null)
                        {
                            this._itemsSSpace = (StoreItemCollection) collection;
                        }
                        goto Label_00E9;

                    case DataSpace.CSSpace:
                        items = this._itemsCSSpace;
                        if (items == null)
                        {
                            this._itemsCSSpace = (StorageMappingItemCollection) collection;
                        }
                        goto Label_00E9;

                    default:
                        goto Label_00C4;
                }
                this._itemsCSpace = items2;
                goto Label_00E9;
            Label_00C4:
                if ((items = this._itemsOCSpace) == null)
                {
                    this._itemsOCSpace = (DefaultObjectMappingItemCollection) collection;
                }
            }
            catch (InvalidCastException)
            {
                throw EntityUtil.InvalidCollectionForMapping(collection.DataSpace);
            }
        Label_00E9:
            if (items != null)
            {
                throw EntityUtil.ItemCollectionAlreadyRegistered(collection.DataSpace);
            }
            if (((collection.DataSpace == DataSpace.CSpace) && (this._itemsCSSpace != null)) && !object.ReferenceEquals(this._itemsCSSpace.EdmItemCollection, collection))
            {
                throw EntityUtil.InvalidCollectionSpecified(collection.DataSpace);
            }
            if (((collection.DataSpace == DataSpace.SSpace) && (this._itemsCSSpace != null)) && !object.ReferenceEquals(this._itemsCSSpace.StoreItemCollection, collection))
            {
                throw EntityUtil.InvalidCollectionSpecified(collection.DataSpace);
            }
            if (collection.DataSpace == DataSpace.CSSpace)
            {
                if ((this._itemsCSpace != null) && !object.ReferenceEquals(this._itemsCSSpace.EdmItemCollection, this._itemsCSpace))
                {
                    throw EntityUtil.InvalidCollectionSpecified(collection.DataSpace);
                }
                if ((this._itemsSSpace != null) && !object.ReferenceEquals(this._itemsCSSpace.StoreItemCollection, this._itemsSSpace))
                {
                    throw EntityUtil.InvalidCollectionSpecified(collection.DataSpace);
                }
            }
        }

        internal MetadataWorkspace ShallowCopy()
        {
            MetadataWorkspace workspace = (MetadataWorkspace) base.MemberwiseClone();
            if (workspace._cacheTokens != null)
            {
                workspace._cacheTokens = new List<object>(workspace._cacheTokens);
            }
            return workspace;
        }

        public bool TryGetEdmSpaceType(StructuralType objectSpaceType, out StructuralType edmSpaceType)
        {
            Map map;
            EntityUtil.CheckArgumentNull<StructuralType>(objectSpaceType, "objectSpaceType");
            if (objectSpaceType.DataSpace != DataSpace.OSpace)
            {
                throw EntityUtil.Argument(System.Data.Entity.Strings.ArgumentMustBeOSpaceType, "objectSpaceType");
            }
            edmSpaceType = null;
            if (!this.TryGetMap(objectSpaceType, DataSpace.OCSpace, out map))
            {
                return false;
            }
            ObjectTypeMapping mapping = map as ObjectTypeMapping;
            if (mapping == null)
            {
                return false;
            }
            edmSpaceType = (StructuralType) mapping.EdmType;
            return true;
        }

        public bool TryGetEntityContainer(string name, DataSpace dataSpace, out EntityContainer entityContainer)
        {
            entityContainer = null;
            EntityUtil.GenericCheckArgumentNull<string>(name, "name");
            ItemCollection itemCollection = this.GetItemCollection(dataSpace, false);
            return ((itemCollection != null) && itemCollection.TryGetEntityContainer(name, out entityContainer));
        }

        public bool TryGetEntityContainer(string name, bool ignoreCase, DataSpace dataSpace, out EntityContainer entityContainer)
        {
            entityContainer = null;
            EntityUtil.GenericCheckArgumentNull<string>(name, "name");
            ItemCollection itemCollection = this.GetItemCollection(dataSpace, false);
            return ((itemCollection != null) && itemCollection.TryGetEntityContainer(name, ignoreCase, out entityContainer));
        }

        internal bool TryGetFunction(string name, string namespaceName, TypeUsage[] parameterTypes, bool ignoreCase, DataSpace dataSpace, out EdmFunction function)
        {
            function = null;
            EntityUtil.GenericCheckArgumentNull<string>(name, "name");
            EntityUtil.GenericCheckArgumentNull<string>(namespaceName, "namespaceName");
            ItemCollection itemCollection = this.GetItemCollection(dataSpace, false);
            return ((itemCollection != null) && itemCollection.TryGetFunction(namespaceName + "." + name, parameterTypes, ignoreCase, out function));
        }

        internal bool TryGetFunctionImportMapping(EdmFunction functionImport, out FunctionImportMapping targetFunctionMapping)
        {
            this.GetItemCollection(DataSpace.CSSpace, true);
            return StorageMappingItemCollection.TryGetFunctionImportTarget(functionImport, this, out targetFunctionMapping);
        }

        internal bool TryGetGeneratedViewOfType(EntitySetBase extent, EntityTypeBase type, bool includeSubtypes, out GeneratedView generatedView) => 
            ((StorageMappingItemCollection) this.GetItemCollection(DataSpace.CSSpace, true)).TryGetGeneratedViewOfType(this, extent, type, includeSubtypes, out generatedView);

        public bool TryGetItem<T>(string identity, DataSpace space, out T item) where T: GlobalItem
        {
            item = default(T);
            ItemCollection itemCollection = this.GetItemCollection(space, false);
            return ((itemCollection != null) && itemCollection.TryGetItem<T>(identity, false, out item));
        }

        public bool TryGetItem<T>(string identity, bool ignoreCase, DataSpace dataSpace, out T item) where T: GlobalItem
        {
            item = default(T);
            ItemCollection itemCollection = this.GetItemCollection(dataSpace, false);
            return ((itemCollection != null) && itemCollection.TryGetItem<T>(identity, ignoreCase, out item));
        }

        public bool TryGetItemCollection(DataSpace dataSpace, out ItemCollection collection)
        {
            collection = this.GetItemCollection(dataSpace, false);
            return (null != collection);
        }

        internal bool TryGetMap(GlobalItem item, DataSpace dataSpace, out Map map)
        {
            map = null;
            ItemCollection itemCollection = this.GetItemCollection(dataSpace, false);
            return ((itemCollection != null) && ((MappingItemCollection) itemCollection).TryGetMap(item, out map));
        }

        internal bool TryGetMap(string typeIdentity, DataSpace typeSpace, bool ignoreCase, DataSpace mappingSpace, out Map map)
        {
            map = null;
            ItemCollection itemCollection = this.GetItemCollection(mappingSpace, false);
            return ((itemCollection != null) && ((MappingItemCollection) itemCollection).TryGetMap(typeIdentity, typeSpace, ignoreCase, out map));
        }

        public bool TryGetObjectSpaceType(StructuralType edmSpaceType, out StructuralType objectSpaceType)
        {
            Map map;
            EntityUtil.CheckArgumentNull<StructuralType>(edmSpaceType, "edmSpaceType");
            if (edmSpaceType.DataSpace != DataSpace.CSpace)
            {
                throw EntityUtil.Argument(System.Data.Entity.Strings.ArgumentMustBeCSpaceType, "edmSpaceType");
            }
            objectSpaceType = null;
            if (!this.TryGetMap(edmSpaceType, DataSpace.OCSpace, out map))
            {
                return false;
            }
            ObjectTypeMapping mapping = map as ObjectTypeMapping;
            if (mapping == null)
            {
                return false;
            }
            objectSpaceType = (StructuralType) mapping.ClrType;
            return true;
        }

        public bool TryGetType(string name, string namespaceName, DataSpace dataSpace, out EdmType type)
        {
            type = null;
            ItemCollection itemCollection = this.GetItemCollection(dataSpace, false);
            return ((itemCollection != null) && itemCollection.TryGetType(name, namespaceName, false, out type));
        }

        public bool TryGetType(string name, string namespaceName, bool ignoreCase, DataSpace dataSpace, out EdmType type)
        {
            type = null;
            ItemCollection itemCollection = this.GetItemCollection(dataSpace, false);
            return ((itemCollection != null) && itemCollection.TryGetType(name, namespaceName, ignoreCase, out type));
        }
    }
}

