namespace System.Data.Mapping
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Data;
    using System.Data.Common.Utils;
    using System.Data.Entity;
    using System.Data.EntityModel.SchemaObjectModel;
    using System.Data.Mapping.Update.Internal;
    using System.Data.Mapping.ViewGeneration;
    using System.Data.Metadata.Edm;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Xml;

    public class StorageMappingItemCollection : MappingItemCollection
    {
        private Memoizer<Pair<EntitySetBase, EntityTypeBase>, ReadOnlyCollection<EdmMember>> _cacheRequiredOriginalValueMembers;
        private ViewLoader _viewLoader;
        private System.Data.Metadata.Edm.EdmItemCollection m_edmCollection;
        private Dictionary<EdmMember, KeyValuePair<TypeUsage, TypeUsage>> m_memberMappings;
        private System.Data.Metadata.Edm.StoreItemCollection m_storeItemCollection;
        private ViewDictionary m_viewDictionary;

        public StorageMappingItemCollection(System.Data.Metadata.Edm.EdmItemCollection edmCollection, System.Data.Metadata.Edm.StoreItemCollection storeCollection, IEnumerable<XmlReader> xmlReaders) : base(DataSpace.CSSpace)
        {
            this.m_memberMappings = new Dictionary<EdmMember, KeyValuePair<TypeUsage, TypeUsage>>();
            EntityUtil.CheckArgumentNull<IEnumerable<XmlReader>>(xmlReaders, "xmlReaders");
            MetadataArtifactLoader loader = MetadataArtifactLoader.CreateCompositeFromXmlReaders(xmlReaders);
            this.Init(edmCollection, storeCollection, loader.GetReaders(), loader.GetPaths(), true);
        }

        public StorageMappingItemCollection(System.Data.Metadata.Edm.EdmItemCollection edmCollection, System.Data.Metadata.Edm.StoreItemCollection storeCollection, params string[] filePaths) : base(DataSpace.CSSpace)
        {
            this.m_memberMappings = new Dictionary<EdmMember, KeyValuePair<TypeUsage, TypeUsage>>();
            EntityUtil.CheckArgumentNull<System.Data.Metadata.Edm.EdmItemCollection>(edmCollection, "edmCollection");
            EntityUtil.CheckArgumentNull<System.Data.Metadata.Edm.StoreItemCollection>(storeCollection, "storeCollection");
            EntityUtil.CheckArgumentNull<string[]>(filePaths, "filePaths");
            this.m_edmCollection = edmCollection;
            this.m_storeItemCollection = storeCollection;
            MetadataArtifactLoader loader = null;
            List<XmlReader> xmlReaders = null;
            try
            {
                loader = MetadataArtifactLoader.CreateCompositeFromFilePaths(filePaths, ".msl");
                xmlReaders = loader.CreateReaders(DataSpace.CSSpace);
                this.Init(edmCollection, storeCollection, xmlReaders, loader.GetPaths(DataSpace.CSSpace), true);
            }
            finally
            {
                if (xmlReaders != null)
                {
                    Helper.DisposeXmlReaders(xmlReaders);
                }
            }
        }

        internal StorageMappingItemCollection(System.Data.Metadata.Edm.EdmItemCollection edmCollection, System.Data.Metadata.Edm.StoreItemCollection storeCollection, IEnumerable<XmlReader> xmlReaders, List<string> filePaths) : base(DataSpace.CSSpace)
        {
            this.m_memberMappings = new Dictionary<EdmMember, KeyValuePair<TypeUsage, TypeUsage>>();
            this.Init(edmCollection, storeCollection, xmlReaders, filePaths, true);
        }

        internal StorageMappingItemCollection(System.Data.Metadata.Edm.EdmItemCollection edmCollection, System.Data.Metadata.Edm.StoreItemCollection storeCollection, IEnumerable<XmlReader> xmlReaders, List<string> filePaths, out IList<EdmSchemaError> errors) : base(DataSpace.CSSpace)
        {
            this.m_memberMappings = new Dictionary<EdmMember, KeyValuePair<TypeUsage, TypeUsage>>();
            errors = this.Init(edmCollection, storeCollection, xmlReaders, filePaths, false);
        }

        private void AddInternal(StorageEntityContainerMapping storageMap)
        {
            storageMap.DataSpace = DataSpace.CSSpace;
            try
            {
                base.AddInternal(storageMap);
            }
            catch (ArgumentException exception)
            {
                throw new MappingException(System.Data.Entity.Strings.Mapping_Duplicate_Type_1(storageMap.EdmItem.Identity), exception);
            }
        }

        private static void CheckForDuplicateItems(System.Data.Metadata.Edm.EdmItemCollection edmItemCollection, System.Data.Metadata.Edm.StoreItemCollection storeItemCollection, List<EdmSchemaError> errorCollection)
        {
            foreach (GlobalItem item in edmItemCollection)
            {
                if (storeItemCollection.Contains(item.Identity))
                {
                    errorCollection.Add(new EdmSchemaError(System.Data.Entity.Strings.Mapping_ItemWithSameNameExistsBothInCSpaceAndSSpace(item.Identity), 0x816, EdmSchemaErrorSeverity.Error));
                }
            }
        }

        private ReadOnlyCollection<EdmMember> ComputeRequiredOriginalValueMembers(Pair<EntitySetBase, EntityTypeBase> arguments)
        {
            EntitySetBase first = arguments.First;
            EntityTypeBase second = arguments.Second;
            List<EdmMember> result = new List<EdmMember>();
            foreach (StoragePropertyMapping mapping in from stm in MappingMetadataHelper.GetMappingsForEntitySetAndSuperTypes(this, first.EntityContainer, first, second) select from mf in stm.MappingFragments select mf.AllProperties)
            {
                this.FindIfPropertyRequiresOriginalValue(mapping, ref result);
            }
            foreach (StorageEntityTypeFunctionMapping mapping2 in MappingMetadataHelper.GetFunctionMappingsForEntitySetAndType(this, first.EntityContainer, first, second))
            {
                if (mapping2.UpdateFunctionMapping != null)
                {
                    foreach (StorageFunctionParameterBinding binding in from p in mapping2.UpdateFunctionMapping.ParameterBindings
                        where !p.IsCurrent
                        select p)
                    {
                        result.Add(binding.MemberPath.Members.Last<EdmMember>());
                    }
                }
            }
            return new ReadOnlyCollection<EdmMember>(result.Distinct<EdmMember>().ToList<EdmMember>());
        }

        internal bool ContainsStorageEntityContainer(string storageEntityContainerName) => 
            base.GetItems<StorageEntityContainerMapping>().Any<StorageEntityContainerMapping>(map => map.StorageEntityContainer.Name.Equals(storageEntityContainerName, StringComparison.Ordinal));

        private void FindIfPropertyRequiresOriginalValue(StoragePropertyMapping propMap, ref List<EdmMember> result)
        {
            StorageScalarPropertyMapping mapping = propMap as StorageScalarPropertyMapping;
            StorageComplexPropertyMapping complexMapping = propMap as StorageComplexPropertyMapping;
            StorageEndPropertyMapping mapping3 = propMap as StorageEndPropertyMapping;
            StorageConditionPropertyMapping mapping4 = propMap as StorageConditionPropertyMapping;
            if ((mapping != null) && (mapping.EdmProperty != null))
            {
                if (MetadataHelper.IsPartOfEntityTypeKey(mapping.EdmProperty))
                {
                    result.Add(mapping.EdmProperty);
                }
                else if (MetadataHelper.GetConcurrencyMode(mapping.EdmProperty) == ConcurrencyMode.Fixed)
                {
                    result.Add(mapping.EdmProperty);
                }
            }
            else if (complexMapping != null)
            {
                if ((MetadataHelper.GetConcurrencyMode(complexMapping.EdmProperty) == ConcurrencyMode.Fixed) || this.HasFixedConcurrencyModeInAnyChildProperty(complexMapping))
                {
                    result.Add(complexMapping.EdmProperty);
                }
            }
            else if (mapping3 != null)
            {
                result.Add(mapping3.EndMember);
            }
            else if ((mapping4 != null) && (mapping4.EdmProperty != null))
            {
                result.Add(mapping4.EdmProperty);
            }
        }

        internal GeneratedView GetGeneratedView(EntitySetBase extent, MetadataWorkspace workspace) => 
            this.m_viewDictionary.GetGeneratedView(extent, workspace, this);

        internal override Map GetMap(GlobalItem item)
        {
            EntityUtil.CheckArgumentNull<GlobalItem>(item, "item");
            DataSpace dataSpace = item.DataSpace;
            if (dataSpace != DataSpace.CSpace)
            {
                throw EntityUtil.InvalidOperation(System.Data.Entity.Strings.Mapping_Storage_InvalidSpace_1(dataSpace));
            }
            return this.GetMap(item.Identity, dataSpace);
        }

        internal override Map GetMap(string identity, DataSpace typeSpace) => 
            this.GetMap(identity, typeSpace, false);

        internal override Map GetMap(string identity, DataSpace typeSpace, bool ignoreCase)
        {
            EntityUtil.CheckArgumentNull<string>(identity, "identity");
            if (typeSpace != DataSpace.CSpace)
            {
                throw EntityUtil.InvalidOperation(System.Data.Entity.Strings.Mapping_Storage_InvalidSpace_1(typeSpace));
            }
            return base.GetItem<Map>(identity, ignoreCase);
        }

        internal ReadOnlyCollection<EdmMember> GetRequiredOriginalValueMembers(Pair<EntitySetBase, EntityTypeBase> arguments) => 
            this._cacheRequiredOriginalValueMembers?.Evaluate(arguments);

        internal ViewLoader GetUpdateViewLoader()
        {
            if (this._viewLoader == null)
            {
                this._viewLoader = new ViewLoader(this);
            }
            return this._viewLoader;
        }

        internal Dictionary<EntitySetBase, string> GetViews(MetadataWorkspace workspace, out IList<EdmSchemaError> errors)
        {
            Dictionary<EntitySetBase, string> dictionary = new Dictionary<EntitySetBase, string>();
            errors = new List<EdmSchemaError>();
            ReadOnlyCollection<Map> items = base.GetItems<Map>();
            foreach (StorageEntityContainerMapping mapping in items)
            {
                if (mapping.IsEmpty)
                {
                    return dictionary;
                }
                if (!this.HasMappingFragment(mapping))
                {
                    errors.Add(new EdmSchemaError(System.Data.Entity.Strings.Mapping_AllQueryViewAtCompileTime(mapping.Identity), 0x828, EdmSchemaErrorSeverity.Warning));
                }
                else
                {
                    ViewGenResults results = ViewGenerator.GenerateViewsForSchema(mapping, workspace, new ConfigViewGenerator());
                    if (results.HasErrors)
                    {
                        errors = new List<EdmSchemaError>(results.Errors);
                    }
                    foreach (KeyValuePair<EntitySetBase, List<GeneratedView>> pair in results.Views.KeyValuePairs)
                    {
                        List<GeneratedView> list = pair.Value;
                        dictionary.Add(pair.Key, list[0].CqlString);
                    }
                }
            }
            bool flag = (from e in errors
                where e.ErrorCode == 0x828
                select e).Count<EdmSchemaError>() == items.Count;
            if ((items.Count > 0) && flag)
            {
                errors.Add(new EdmSchemaError(System.Data.Entity.Strings.Mapping_NoViewsCanBeGenerated, 0x829, EdmSchemaErrorSeverity.Error));
            }
            return dictionary;
        }

        private bool HasFixedConcurrencyModeInAnyChildProperty(StorageComplexPropertyMapping complexMapping)
        {
            foreach (StoragePropertyMapping mapping in from m in complexMapping.TypeMappings select m.AllProperties)
            {
                StorageScalarPropertyMapping mapping2 = mapping as StorageScalarPropertyMapping;
                StorageComplexPropertyMapping mapping3 = mapping as StorageComplexPropertyMapping;
                if ((mapping2 != null) && (MetadataHelper.GetConcurrencyMode(mapping2.EdmProperty) == ConcurrencyMode.Fixed))
                {
                    return true;
                }
                if ((mapping3 != null) && ((MetadataHelper.GetConcurrencyMode(mapping3.EdmProperty) == ConcurrencyMode.Fixed) || this.HasFixedConcurrencyModeInAnyChildProperty(mapping3)))
                {
                    return true;
                }
            }
            return false;
        }

        private bool HasMappingFragment(StorageEntityContainerMapping mapping)
        {
            foreach (StorageSetMapping mapping2 in mapping.AllSetMaps)
            {
                foreach (StorageTypeMapping mapping3 in mapping2.TypeMappings)
                {
                    if (mapping3.MappingFragments.Count > 0)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private IList<EdmSchemaError> Init(System.Data.Metadata.Edm.EdmItemCollection edmCollection, System.Data.Metadata.Edm.StoreItemCollection storeCollection, IEnumerable<XmlReader> xmlReaders, List<string> filePaths, bool throwOnError)
        {
            Dictionary<EntitySetBase, GeneratedView> dictionary;
            Dictionary<Pair<EntitySetBase, Pair<EntityTypeBase, bool>>, GeneratedView> dictionary2;
            EntityUtil.CheckArgumentNull<IEnumerable<XmlReader>>(xmlReaders, "xmlReaders");
            EntityUtil.CheckArgumentNull<System.Data.Metadata.Edm.EdmItemCollection>(edmCollection, "edmCollection");
            EntityUtil.CheckArgumentNull<System.Data.Metadata.Edm.StoreItemCollection>(storeCollection, "storeCollection");
            this.m_edmCollection = edmCollection;
            this.m_storeItemCollection = storeCollection;
            this.m_viewDictionary = new ViewDictionary(this, out dictionary, out dictionary2);
            IList<EdmSchemaError> schemaErrors = this.LoadItems(xmlReaders, filePaths, dictionary, dictionary2);
            if ((throwOnError && (schemaErrors.Count != 0)) && !MetadataHelper.CheckIfAllErrorsAreWarnings(schemaErrors))
            {
                throw new MappingException(string.Format(CultureInfo.CurrentCulture, EntityRes.GetString("InvalidSchemaEncountered"), new object[] { Helper.CombineErrorMessage(schemaErrors) }));
            }
            return schemaErrors;
        }

        private List<EdmSchemaError> LoadItems(IEnumerable<XmlReader> xmlReaders, List<string> mappingSchemaUris, Dictionary<EntitySetBase, GeneratedView> userDefinedQueryViewsDict, Dictionary<Pair<EntitySetBase, Pair<EntityTypeBase, bool>>, GeneratedView> userDefinedQueryViewsOfTypeDict)
        {
            List<EdmSchemaError> errorCollection = new List<EdmSchemaError>();
            int num = -1;
            foreach (XmlReader reader in xmlReaders)
            {
                num++;
                string location = null;
                if (mappingSchemaUris == null)
                {
                    SchemaManager.TryGetBaseUri(reader, out location);
                }
                else
                {
                    location = mappingSchemaUris[num];
                }
                StorageMappingItemLoader loader = new StorageMappingItemLoader(this, location, this.m_memberMappings);
                IList<EdmSchemaError> parsingErrors = loader.ParsingErrors;
                StorageEntityContainerMapping entityContainerMapping = loader.LoadMappingItems(reader);
                if (loader.HasQueryViews && (entityContainerMapping != null))
                {
                    ViewDictionary.CollectViews(entityContainerMapping, this, parsingErrors, userDefinedQueryViewsDict, userDefinedQueryViewsOfTypeDict);
                }
                if (MetadataHelper.CheckIfAllErrorsAreWarnings(parsingErrors) && !base.Contains(entityContainerMapping))
                {
                    this.AddInternal(entityContainerMapping);
                }
                errorCollection.AddRange(parsingErrors);
            }
            CheckForDuplicateItems(this.EdmItemCollection, this.StoreItemCollection, errorCollection);
            return errorCollection;
        }

        internal static bool TryGetFunctionImportTarget(EdmFunction functionImport, MetadataWorkspace workspace, out FunctionImportMapping targetFunction)
        {
            foreach (StorageEntityContainerMapping mapping in workspace.GetItems<StorageEntityContainerMapping>(DataSpace.CSSpace))
            {
                if (mapping.TryGetFunctionImportMapping(functionImport, out targetFunction))
                {
                    return true;
                }
            }
            targetFunction = null;
            return false;
        }

        internal bool TryGetGeneratedViewOfType(MetadataWorkspace workspace, EntitySetBase entity, EntityTypeBase type, bool includeSubtypes, out GeneratedView generatedView) => 
            this.m_viewDictionary.TryGetGeneratedViewOfType(workspace, entity, type, includeSubtypes, out generatedView);

        internal override bool TryGetMap(GlobalItem item, out Map map)
        {
            if (item == null)
            {
                map = null;
                return false;
            }
            DataSpace dataSpace = item.DataSpace;
            if (dataSpace != DataSpace.CSpace)
            {
                map = null;
                return false;
            }
            return this.TryGetMap(item.Identity, dataSpace, out map);
        }

        internal override bool TryGetMap(string identity, DataSpace typeSpace, out Map map) => 
            this.TryGetMap(identity, typeSpace, false, out map);

        internal override bool TryGetMap(string identity, DataSpace typeSpace, bool ignoreCase, out Map map)
        {
            if (typeSpace != DataSpace.CSpace)
            {
                throw EntityUtil.InvalidOperation(System.Data.Entity.Strings.Mapping_Storage_InvalidSpace_1(typeSpace));
            }
            return base.TryGetItem<Map>(identity, ignoreCase, out map);
        }

        internal System.Data.Metadata.Edm.EdmItemCollection EdmItemCollection =>
            this.m_edmCollection;

        internal System.Data.Metadata.Edm.StoreItemCollection StoreItemCollection =>
            this.m_storeItemCollection;

        internal delegate bool TryGetUserDefinedQueryView(EntitySetBase extent, out GeneratedView generatedView);

        internal delegate bool TryGetUserDefinedQueryViewOfType(Pair<EntitySetBase, Pair<EntityTypeBase, bool>> extent, out GeneratedView generatedView);

        internal class ViewDictionary
        {
            private static ConfigViewGenerator m_config = new ConfigViewGenerator();
            private readonly Memoizer<Pair<EntitySetBase, Pair<EntityTypeBase, bool>>, GeneratedView> m_generatedViewOfTypeMemoizer;
            private readonly Memoizer<System.Data.Metadata.Edm.EntityContainer, Dictionary<EntitySetBase, GeneratedView>> m_generatedViewsMemoizer;
            private bool m_generatedViewsMode = true;
            private List<Assembly> m_knownViewGenAssemblies = new List<Assembly>();
            private StorageMappingItemCollection m_storageMappingItemCollection;
            private readonly System.Data.Mapping.StorageMappingItemCollection.TryGetUserDefinedQueryView TryGetUserDefinedQueryView;
            private readonly System.Data.Mapping.StorageMappingItemCollection.TryGetUserDefinedQueryViewOfType TryGetUserDefinedQueryViewOfType;

            internal ViewDictionary(StorageMappingItemCollection storageMappingItemCollection, out Dictionary<EntitySetBase, GeneratedView> userDefinedQueryViewsDict, out Dictionary<Pair<EntitySetBase, Pair<EntityTypeBase, bool>>, GeneratedView> userDefinedQueryViewsOfTypeDict)
            {
                this.m_storageMappingItemCollection = storageMappingItemCollection;
                this.m_generatedViewsMemoizer = new Memoizer<System.Data.Metadata.Edm.EntityContainer, Dictionary<EntitySetBase, GeneratedView>>(new Func<System.Data.Metadata.Edm.EntityContainer, Dictionary<EntitySetBase, GeneratedView>>(this.SerializedGetGeneratedViews), null);
                this.m_generatedViewOfTypeMemoizer = new Memoizer<Pair<EntitySetBase, Pair<EntityTypeBase, bool>>, GeneratedView>(new Func<Pair<EntitySetBase, Pair<EntityTypeBase, bool>>, GeneratedView>(this.SerializedGeneratedViewOfType), Pair<EntitySetBase, Pair<EntityTypeBase, bool>>.PairComparer.Instance);
                userDefinedQueryViewsDict = new Dictionary<EntitySetBase, GeneratedView>(EqualityComparer<EntitySetBase>.Default);
                userDefinedQueryViewsOfTypeDict = new Dictionary<Pair<EntitySetBase, Pair<EntityTypeBase, bool>>, GeneratedView>(Pair<EntitySetBase, Pair<EntityTypeBase, bool>>.PairComparer.Instance);
                this.TryGetUserDefinedQueryView = new System.Data.Mapping.StorageMappingItemCollection.TryGetUserDefinedQueryView(userDefinedQueryViewsDict.TryGetValue);
                this.TryGetUserDefinedQueryViewOfType = new System.Data.Mapping.StorageMappingItemCollection.TryGetUserDefinedQueryViewOfType(userDefinedQueryViewsOfTypeDict.TryGetValue);
            }

            internal static void CollectViews(StorageEntityContainerMapping entityContainerMapping, StorageMappingItemCollection storageMappingItemCollection, IList<EdmSchemaError> errors, Dictionary<EntitySetBase, GeneratedView> userDefinedQueryViewsDict, Dictionary<Pair<EntitySetBase, Pair<EntityTypeBase, bool>>, GeneratedView> userDefinedQueryViewsOfTypeDict)
            {
                MetadataWorkspace workSpace = new MetadataWorkspace();
                workSpace.RegisterItemCollection(storageMappingItemCollection.m_edmCollection);
                workSpace.RegisterItemCollection(storageMappingItemCollection.m_storeItemCollection);
                workSpace.RegisterItemCollection(storageMappingItemCollection);
                ConfigViewGenerator config = new ConfigViewGenerator();
                foreach (StorageSetMapping mapping in entityContainerMapping.AllSetMaps)
                {
                    GeneratedView view;
                    if ((mapping.QueryView != null) && !userDefinedQueryViewsDict.TryGetValue(mapping.Set, out view))
                    {
                        try
                        {
                            view = new GeneratedView(mapping.Set, mapping.Set.ElementType, mapping.QueryView.Trim(), workSpace, config);
                            bool flag = false;
                            foreach (EdmSchemaError error in view.ParseUserSpecifiedView(workSpace, storageMappingItemCollection.StoreItemCollection, mapping, mapping.Set.ElementType, true))
                            {
                                flag = true;
                                errors.Add(error);
                            }
                            if (!flag)
                            {
                                userDefinedQueryViewsDict.Add(mapping.Set, view);
                            }
                            foreach (Pair<EntitySetBase, Pair<EntityTypeBase, bool>> pair in mapping.GetTypeSpecificQVKeys())
                            {
                                view = new GeneratedView(mapping.Set, pair.Second.First, mapping.GetTypeSpecificQueryView(pair), workSpace, config);
                                flag = false;
                                foreach (EdmSchemaError error2 in view.ParseUserSpecifiedView(workSpace, storageMappingItemCollection.StoreItemCollection, mapping, pair.Second.First, pair.Second.Second))
                                {
                                    flag = true;
                                    errors.Add(error2);
                                }
                                if (!flag)
                                {
                                    userDefinedQueryViewsOfTypeDict.Add(pair, view);
                                }
                            }
                        }
                        catch (Exception exception)
                        {
                            if (!EntityUtil.IsCatchableExceptionType(exception))
                            {
                                throw;
                            }
                            EdmSchemaError item = new EdmSchemaError(System.Data.Entity.Strings.Mapping_Invalid_QueryView_2(mapping.Set.Name, exception.Message), 0x814, EdmSchemaErrorSeverity.Error, mapping.EntityContainerMapping.SourceLocation, mapping.StartLineNumber, mapping.StartLinePosition, exception);
                            errors.Add(item);
                        }
                    }
                }
            }

            internal GeneratedView GetGeneratedView(EntitySetBase extent, MetadataWorkspace workspace, StorageMappingItemCollection storageMappingItemCollection)
            {
                GeneratedView view;
                if (!this.TryGetUserDefinedQueryView(extent, out view) && !this.m_generatedViewsMemoizer.Evaluate(extent.EntityContainer).TryGetValue(extent, out view))
                {
                    throw EntityUtil.InvalidOperation(System.Data.Entity.Strings.Mapping_Views_For_Extent_Not_Generated((extent.GetDataSpace() == DataSpace.SSpace) ? "Table" : "EntitySet", extent.Name));
                }
                return view;
            }

            private void SerializedAddGeneratedViews(MetadataWorkspace workspace, EntityViewContainer viewContainer, Dictionary<EntitySetBase, GeneratedView> extentMappingViews)
            {
                foreach (KeyValuePair<string, string> pair in viewContainer.ExtentViews)
                {
                    GeneratedView view;
                    System.Data.Metadata.Edm.EntityContainer item = null;
                    EntitySetBase base2 = null;
                    string key = pair.Key;
                    int length = key.LastIndexOf('.');
                    if (length != -1)
                    {
                        string identity = key.Substring(0, length);
                        string str3 = key.Substring(key.LastIndexOf('.') + 1);
                        if (!workspace.TryGetItem<System.Data.Metadata.Edm.EntityContainer>(identity, DataSpace.CSpace, out item))
                        {
                            workspace.TryGetItem<System.Data.Metadata.Edm.EntityContainer>(identity, DataSpace.SSpace, out item);
                        }
                        if (item != null)
                        {
                            item.BaseEntitySets.TryGetValue(str3, false, out base2);
                        }
                    }
                    if (base2 == null)
                    {
                        throw new MappingException(System.Data.Entity.Strings.Generated_Views_Invalid_Extent_1(key));
                    }
                    if (!extentMappingViews.TryGetValue(base2, out view))
                    {
                        view = new GeneratedView(base2, null, pair.Value, workspace, new ConfigViewGenerator());
                        extentMappingViews.Add(base2, view);
                    }
                }
            }

            private bool SerializedAddGeneratedViewsInEntityViewContainer(MetadataWorkspace workspace, EntityViewContainer entityViewContainer, StorageMappingItemCollection storageMappingItemCollection, Dictionary<EntitySetBase, GeneratedView> extentMappingViews)
            {
                StorageEntityContainerMapping mapping;
                if (!this.TryGetCorrespondingStorageEntityContainerMapping(entityViewContainer, storageMappingItemCollection.GetItems<StorageEntityContainerMapping>(), out mapping))
                {
                    return false;
                }
                if (!this.SerializedVerifyHashOverMmClosure(mapping, entityViewContainer, workspace.GetItemCollection(DataSpace.CSpace)))
                {
                    throw new MappingException(System.Data.Entity.Strings.ViewGen_HashOnMappingClosure_Not_Matching(entityViewContainer.EdmEntityContainerName));
                }
                if (!this.VerifyViewsHaveNotChanged(workspace, entityViewContainer))
                {
                    throw new InvalidOperationException(System.Data.Entity.Strings.Generated_Views_Changed);
                }
                this.SerializedAddGeneratedViews(workspace, entityViewContainer, extentMappingViews);
                return true;
            }

            private void SerializedCollectViewsFromObjectCollection(MetadataWorkspace workspace, StorageMappingItemCollection storageMappingItemCollection, Dictionary<EntitySetBase, GeneratedView> extentMappingViews)
            {
                IList<Assembly> viewGenerationAssemblies = ObjectItemCollection.ViewGenerationAssemblies;
                if (viewGenerationAssemblies != null)
                {
                    foreach (Assembly assembly in viewGenerationAssemblies)
                    {
                        object[] customAttributes = assembly.GetCustomAttributes(typeof(EntityViewGenerationAttribute), false);
                        if ((customAttributes != null) && (customAttributes.Length != 0))
                        {
                            foreach (EntityViewGenerationAttribute attribute in customAttributes)
                            {
                                Type viewGenerationType = attribute.ViewGenerationType;
                                if (!viewGenerationType.IsSubclassOf(typeof(EntityViewContainer)))
                                {
                                    throw EntityUtil.InvalidOperation(System.Data.Entity.Strings.Generated_View_Type_Super_Class_1("Edm_EntityMappingGeneratedViews.ViewsForBaseEntitySets"));
                                }
                                EntityViewContainer entityViewContainer = Activator.CreateInstance(viewGenerationType) as EntityViewContainer;
                                if (this.SerializedAddGeneratedViewsInEntityViewContainer(workspace, entityViewContainer, storageMappingItemCollection, extentMappingViews))
                                {
                                    EntityBid.Trace("<map.StorageMappingItemCollection.CollectViewsFromObjectCollection|Info> Collected views from Assembly='%ls'\n", assembly.FullName);
                                }
                            }
                        }
                    }
                }
            }

            private void SerializedCollectViewsFromReferencedAssemblies(MetadataWorkspace workspace, StorageMappingItemCollection storageMappingItemCollection, Dictionary<EntitySetBase, GeneratedView> extentMappingViews)
            {
                ItemCollection items2;
                if (!workspace.TryGetItemCollection(DataSpace.OSpace, out items2))
                {
                    ObjectItemCollection items = new ObjectItemCollection();
                    items2 = items;
                    Assembly entryAssembly = Assembly.GetEntryAssembly();
                    if (entryAssembly != null)
                    {
                        items.LoadAllReferencedAssemblies(entryAssembly, true);
                    }
                }
                this.SerializedCollectViewsFromObjectCollection(workspace, storageMappingItemCollection, extentMappingViews);
            }

            private GeneratedView SerializedGeneratedViewOfType(Pair<EntitySetBase, Pair<EntityTypeBase, bool>> arg)
            {
                GeneratedView view;
                if (!this.TryGetUserDefinedQueryViewOfType(arg, out view))
                {
                    EntitySetBase first = arg.First;
                    EntityTypeBase type = arg.Second.First;
                    bool second = arg.Second.Second;
                    if (!this.TryGenerateQueryViewOfType(first.EntityContainer, first, type, second, out view))
                    {
                        view = null;
                    }
                }
                return view;
            }

            private void SerializedGenerateViews(System.Data.Metadata.Edm.EntityContainer container, Dictionary<EntitySetBase, GeneratedView> resultDictionary)
            {
                MetadataWorkspace workSpace = new MetadataWorkspace();
                workSpace.RegisterItemCollection(this.m_storageMappingItemCollection.EdmItemCollection);
                workSpace.RegisterItemCollection(this.m_storageMappingItemCollection.StoreItemCollection);
                workSpace.RegisterItemCollection(this.m_storageMappingItemCollection);
                StorageEntityContainerMapping entityContainerMap = MappingMetadataHelper.GetEntityContainerMap(this.m_storageMappingItemCollection, container);
                if (!entityContainerMap.IsEmpty)
                {
                    ViewGenResults results = ViewGenerator.GenerateViewsForSchema(entityContainerMap, workSpace, m_config);
                    KeyToListMap<EntitySetBase, GeneratedView> views = results.Views;
                    if (results.HasErrors)
                    {
                        throw new MappingException(Helper.CombineErrorMessage(results.Errors));
                    }
                    foreach (KeyValuePair<EntitySetBase, List<GeneratedView>> pair in views.KeyValuePairs)
                    {
                        GeneratedView view;
                        if (!resultDictionary.TryGetValue(pair.Key, out view))
                        {
                            view = pair.Value[0];
                            resultDictionary.Add(pair.Key, view);
                        }
                    }
                }
            }

            private Dictionary<EntitySetBase, GeneratedView> SerializedGetGeneratedViews(System.Data.Metadata.Edm.EntityContainer container)
            {
                Dictionary<EntitySetBase, GeneratedView> extentMappingViews = new Dictionary<EntitySetBase, GeneratedView>();
                MetadataWorkspace workspace = new MetadataWorkspace();
                workspace.RegisterItemCollection(this.m_storageMappingItemCollection.StoreItemCollection);
                workspace.RegisterItemCollection(this.m_storageMappingItemCollection.EdmItemCollection);
                workspace.RegisterItemCollection(this.m_storageMappingItemCollection);
                if (this.m_generatedViewsMode)
                {
                    if ((ObjectItemCollection.ViewGenerationAssemblies != null) && (ObjectItemCollection.ViewGenerationAssemblies.Count > 0))
                    {
                        this.SerializedCollectViewsFromObjectCollection(workspace, this.m_storageMappingItemCollection, extentMappingViews);
                    }
                    else
                    {
                        this.SerializedCollectViewsFromReferencedAssemblies(workspace, this.m_storageMappingItemCollection, extentMappingViews);
                    }
                    if (this.m_knownViewGenAssemblies.Count != 0)
                    {
                        EntityBid.Trace("<map.StorageMappingItemCollection.GetCqtView|Info> The View generation Mode is Compile Time View Generation\n");
                    }
                }
                if (extentMappingViews.Count == 0)
                {
                    this.m_generatedViewsMode = false;
                    EntityBid.Trace("<map.StorageMappingItemCollection.GetCqtView|Info> The View generation Mode is Runtime View Generation\n");
                    this.SerializedGenerateViews(container, extentMappingViews);
                }
                return extentMappingViews;
            }

            private bool SerializedVerifyHashOverMmClosure(StorageEntityContainerMapping entityContainerMapping, EntityViewContainer entityViewContainer, ItemCollection itemCollection) => 
                (MetadataMappingHasherVisitor.GetMappingClosureHash(entityContainerMapping, itemCollection) == entityViewContainer.HashOverMappingClosure);

            private bool TryGenerateQueryViewOfType(System.Data.Metadata.Edm.EntityContainer entityContainer, EntitySetBase entity, EntityTypeBase type, bool includeSubtypes, out GeneratedView generatedView)
            {
                bool flag;
                MetadataWorkspace workSpace = new MetadataWorkspace();
                workSpace.RegisterItemCollection(this.m_storageMappingItemCollection.EdmItemCollection);
                workSpace.RegisterItemCollection(this.m_storageMappingItemCollection.StoreItemCollection);
                workSpace.RegisterItemCollection(this.m_storageMappingItemCollection);
                if (type.Abstract)
                {
                    generatedView = null;
                    return false;
                }
                ViewGenResults results = ViewGenerator.GenerateQueryViewOfType(MappingMetadataHelper.GetEntityContainerMap(this.m_storageMappingItemCollection, entityContainer), workSpace, m_config, entity, type, includeSubtypes, out flag);
                if (!flag)
                {
                    generatedView = null;
                    return false;
                }
                KeyToListMap<EntitySetBase, GeneratedView> views = results.Views;
                if (results.HasErrors)
                {
                    throw new MappingException(Helper.CombineErrorMessage(results.Errors));
                }
                generatedView = views.AllValues.First<GeneratedView>();
                return true;
            }

            private bool TryGetCorrespondingStorageEntityContainerMapping(EntityViewContainer viewContainer, IEnumerable<StorageEntityContainerMapping> storageEntityContainerMappingList, out StorageEntityContainerMapping storageEntityContainerMapping)
            {
                storageEntityContainerMapping = null;
                foreach (StorageEntityContainerMapping mapping in storageEntityContainerMappingList)
                {
                    if ((mapping.EdmEntityContainer.Name == viewContainer.EdmEntityContainerName) && (mapping.StorageEntityContainer.Name == viewContainer.StoreEntityContainerName))
                    {
                        storageEntityContainerMapping = mapping;
                        return true;
                    }
                }
                return false;
            }

            internal bool TryGetGeneratedViewOfType(MetadataWorkspace workspace, EntitySetBase entity, EntityTypeBase type, bool includeSubtypes, out GeneratedView generatedView)
            {
                Pair<EntitySetBase, Pair<EntityTypeBase, bool>> arg = new Pair<EntitySetBase, Pair<EntityTypeBase, bool>>(entity, new Pair<EntityTypeBase, bool>(type, includeSubtypes));
                generatedView = this.m_generatedViewOfTypeMemoizer.Evaluate(arg);
                return (generatedView != null);
            }

            private bool VerifyViewsHaveNotChanged(MetadataWorkspace workspace, EntityViewContainer viewContainer)
            {
                string str = MetadataHelper.GenerateHashForAllExtentViewsContent(viewContainer.ExtentViews);
                string hashOverAllExtentViews = viewContainer.HashOverAllExtentViews;
                if (str != hashOverAllExtentViews)
                {
                    return false;
                }
                return true;
            }
        }
    }
}

