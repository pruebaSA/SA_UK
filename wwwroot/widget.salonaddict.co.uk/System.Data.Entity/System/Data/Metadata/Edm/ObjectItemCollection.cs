namespace System.Data.Metadata.Edm
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Entity;
    using System.Data.EntityModel.SchemaObjectModel;
    using System.Data.Mapping;
    using System.Data.Objects.DataClasses;
    using System.Diagnostics;
    using System.IO;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Threading;

    public sealed class ObjectItemCollection : ItemCollection
    {
        private Dictionary<Assembly, bool> _knownAssemblies;
        private object _loadAssemblyLock;
        private Dictionary<string, EdmType> _ocMapping;
        private readonly CacheForPrimitiveTypes _primitiveTypeMaps;
        private const string EcmPublicKeyToken = "PublicKeyToken=b77a5c561934e089";

        public ObjectItemCollection() : base(DataSpace.OSpace)
        {
            this._primitiveTypeMaps = new CacheForPrimitiveTypes();
            this._knownAssemblies = new Dictionary<Assembly, bool>();
            this._ocMapping = new Dictionary<string, EdmType>();
            this._loadAssemblyLock = new object();
            foreach (PrimitiveType type in ClrProviderManifest.Instance.GetStoreTypes())
            {
                base.AddInternal(type);
                this._primitiveTypeMaps.Add(type);
            }
        }

        private static void CollectIfViewGenAssembly(Assembly assembly)
        {
            if (assembly.IsDefined(typeof(EntityViewGenerationAttribute), false) && !AssemblyCacheEntry.ViewGenerationAssemblies.Contains(assembly))
            {
                AssemblyCacheEntry.ViewGenerationAssemblies.Add(assembly);
            }
        }

        internal static IEnumerable<AssociationType> GetAllRelationshipTypesExpensiveWay(Assembly assembly)
        {
            Dictionary<string, EdmType> iteratorVariable0 = LoadTypesExpensiveWay(assembly);
            if (iteratorVariable0 != null)
            {
                foreach (EdmType iteratorVariable1 in iteratorVariable0.Values)
                {
                    if (Helper.IsAssociationType(iteratorVariable1))
                    {
                        yield return (AssociationType) iteratorVariable1;
                    }
                }
            }
        }

        public Type GetClrType(StructuralType objectSpaceType)
        {
            Type type;
            if (!this.TryGetClrType(objectSpaceType, out type))
            {
                throw EntityUtil.Argument(Strings.FailedToFindClrTypeMapping(objectSpaceType.Identity));
            }
            return type;
        }

        internal override PrimitiveType GetMappedPrimitiveType(PrimitiveTypeKind modelType)
        {
            PrimitiveType type = null;
            this._primitiveTypeMaps.TryGetType(modelType, null, out type);
            return type;
        }

        private static string GetModelNamespaceForEntityType(Dictionary<string, EdmType> typesInLoading, Type entityClrType)
        {
            EdmType type;
            if (typesInLoading.TryGetValue(entityClrType.FullName, out type) && Helper.IsEntityType(type))
            {
                return Utils.GetEverythingBeforeLastPeriod(((ClrEntityType) type).CSpaceTypeName);
            }
            return null;
        }

        public IEnumerable<PrimitiveType> GetPrimitiveTypes() => 
            this._primitiveTypeMaps.GetTypes();

        internal AssociationType GetRelationshipType(Type entityClrType, string relationshipName, out string fullSearchName)
        {
            AssociationType type;
            EntityType type2;
            fullSearchName = relationshipName;
            if (base.TryGetItem<AssociationType>(relationshipName, out type))
            {
                return type;
            }
            if (!IsQualifiedName(relationshipName) && base.TryGetItem<EntityType>(entityClrType.FullName, out type2))
            {
                string everythingBeforeLastPeriod = Utils.GetEverythingBeforeLastPeriod(((ClrEntityType) type2).CSpaceTypeName);
                fullSearchName = everythingBeforeLastPeriod + "." + relationshipName;
                if (base.TryGetItem<AssociationType>(fullSearchName, out type))
                {
                    return type;
                }
            }
            return null;
        }

        internal static AssociationType GetRelationshipTypeExpensiveWay(Type entityClrType, string relationshipName, out string fullSearchName)
        {
            fullSearchName = relationshipName;
            Dictionary<string, EdmType> typesInLoading = LoadTypesExpensiveWay(entityClrType.Assembly);
            if (typesInLoading != null)
            {
                EdmType type;
                if (typesInLoading.TryGetValue(relationshipName, out type) && Helper.IsRelationshipType(type))
                {
                    return (AssociationType) type;
                }
                if (!IsQualifiedName(relationshipName))
                {
                    string modelNamespaceForEntityType = GetModelNamespaceForEntityType(typesInLoading, entityClrType);
                    fullSearchName = modelNamespaceForEntityType + "." + relationshipName;
                    if (((modelNamespaceForEntityType != null) && typesInLoading.TryGetValue(fullSearchName, out type)) && Helper.IsRelationshipType(type))
                    {
                        return (AssociationType) type;
                    }
                }
            }
            return null;
        }

        internal static bool IsCompiledViewGenAttributePresent(Assembly assembly) => 
            assembly.IsDefined(typeof(EntityViewGenerationAttribute), false);

        private static bool IsQualifiedName(string relationshipName) => 
            relationshipName.Contains(".");

        internal static bool IsSchemaAttributePresent(Assembly assembly) => 
            assembly.IsDefined(typeof(EdmSchemaAttribute), false);

        internal void LoadAllReferencedAssemblies(Assembly assembly)
        {
            this.LoadAllReferencedAssemblies(assembly, false);
        }

        internal void LoadAllReferencedAssemblies(Assembly assembly, bool loadOnlyViewGenInfo)
        {
            if (loadOnlyViewGenInfo)
            {
                if (ShouldFilterAssembly(assembly.FullName))
                {
                    return;
                }
                lock (this)
                {
                    CollectIfViewGenAssembly(assembly);
                    foreach (AssemblyName name in assembly.GetReferencedAssemblies())
                    {
                        string fullName = name.FullName;
                        if (!ShouldFilterAssembly(fullName))
                        {
                            Bid.Trace("<md.ObjectItemCollection.LoadAllReferencedAssemblies|Info> loadededAssembly='%ls'\n", fullName);
                            Assembly assembly2 = AssemblyCacheEntry.SafeLoadReferencedAssembly(fullName);
                            if (assembly2 != null)
                            {
                                CollectIfViewGenAssembly(assembly2);
                            }
                        }
                    }
                    return;
                }
            }
            LoadAssemblyFromCache(this, assembly, true);
        }

        internal bool LoadAssemblyForType(Type type)
        {
            bool flag = LoadAssemblyFromCache(this, type.Assembly, false);
            if (type.IsGenericType)
            {
                foreach (Type type2 in type.GetGenericArguments())
                {
                    flag |= this.LoadAssemblyForType(type2);
                }
            }
            return flag;
        }

        private static bool LoadAssemblyFromCache(ObjectItemCollection objectItemCollection, Assembly assembly, bool loadReferencedAssemblies)
        {
            bool flag;
            if (ShouldFilterAssembly(assembly.FullName))
            {
                return false;
            }
            if (!objectItemCollection._knownAssemblies.TryGetValue(assembly, out flag) || (loadReferencedAssemblies && !flag))
            {
                if (!IsSchemaAttributePresent(assembly) && !loadReferencedAssemblies)
                {
                    return false;
                }
                lock (objectItemCollection.LoadAssemblyLock)
                {
                    Dictionary<string, EdmType> dictionary;
                    List<EdmItemError> list;
                    Dictionary<Assembly, bool> dictionary2;
                    if (objectItemCollection._knownAssemblies.TryGetValue(assembly, out flag) && (!loadReferencedAssemblies || flag))
                    {
                        return true;
                    }
                    if (objectItemCollection != null)
                    {
                        dictionary2 = new Dictionary<Assembly, bool>(objectItemCollection._knownAssemblies);
                    }
                    else
                    {
                        dictionary2 = new Dictionary<Assembly, bool>();
                    }
                    AssemblyCacheEntry.LoadAssemblyFromCache(assembly, loadReferencedAssemblies, dictionary2, out dictionary, out list);
                    if (list.Count != 0)
                    {
                        throw EntityUtil.InvalidSchemaEncountered(Helper.CombineErrorMessage(list));
                    }
                    if (dictionary.Count != 0)
                    {
                        List<GlobalItem> items = new List<GlobalItem>();
                        foreach (EdmType type in dictionary.Values)
                        {
                            items.Add(type);
                            try
                            {
                                if (BuiltInTypeKind.EntityType == type.BuiltInTypeKind)
                                {
                                    objectItemCollection._ocMapping.Add(((ClrEntityType) type).CSpaceTypeName, type);
                                }
                                else if (BuiltInTypeKind.ComplexType == type.BuiltInTypeKind)
                                {
                                    objectItemCollection._ocMapping.Add(((ClrComplexType) type).CSpaceTypeName, type);
                                }
                            }
                            catch (ArgumentException exception)
                            {
                                throw new MappingException(Strings.Mapping_CannotMapCLRTypeMultipleTimes(type.Identity), exception);
                            }
                        }
                        objectItemCollection.AtomicAddRange(items);
                        objectItemCollection._knownAssemblies = dictionary2;
                        foreach (Assembly assembly2 in dictionary2.Keys)
                        {
                            CollectIfViewGenAssembly(assembly2);
                        }
                    }
                }
            }
            return true;
        }

        public void LoadFromAssembly(Assembly assembly)
        {
            EntityUtil.CheckArgumentNull<Assembly>(assembly, "assembly");
            EntityBid.Trace("<md.ObjectItemCollection.LoadFromAssembly|Info> assembly='%ls'\n", assembly.FullName);
            LoadAssemblyFromCache(this, assembly, false);
            if (IsCompiledViewGenAttributePresent(assembly) && !IsSchemaAttributePresent(assembly))
            {
                CollectIfViewGenAssembly(assembly);
            }
        }

        private static Dictionary<string, EdmType> LoadTypesExpensiveWay(Assembly assembly)
        {
            Dictionary<string, EdmType> typesInLoading = null;
            if (!ShouldFilterAssembly(assembly.FullName) && IsSchemaAttributePresent(assembly))
            {
                List<EdmItemError> list;
                Dictionary<Assembly, bool> knownAssemblies = new Dictionary<Assembly, bool>();
                AssemblyCacheEntry.LoadAssemblyFromCache(assembly, false, knownAssemblies, out typesInLoading, out list);
                if (list.Count != 0)
                {
                    throw EntityUtil.InvalidSchemaEncountered(Helper.CombineErrorMessage(list));
                }
            }
            return typesInLoading;
        }

        internal static bool ShouldFilterAssembly(string fullName) => 
            fullName.EndsWith("PublicKeyToken=b77a5c561934e089", StringComparison.OrdinalIgnoreCase);

        public bool TryGetClrType(StructuralType objectSpaceType, out Type clrType)
        {
            EntityUtil.CheckArgumentNull<StructuralType>(objectSpaceType, "objectSpaceType");
            if (objectSpaceType.DataSpace != DataSpace.OSpace)
            {
                throw EntityUtil.Argument(Strings.ArgumentMustBeOSpaceType, "objectSpaceType");
            }
            clrType = null;
            if (BuiltInTypeKind.EntityType == objectSpaceType.BuiltInTypeKind)
            {
                clrType = ((ClrEntityType) objectSpaceType).ClrType;
            }
            else if (BuiltInTypeKind.ComplexType == objectSpaceType.BuiltInTypeKind)
            {
                clrType = ((ClrComplexType) objectSpaceType).ClrType;
            }
            return (clrType != null);
        }

        internal static string TryGetMappingCSpaceTypeIdentity(EdmType edmType)
        {
            if (BuiltInTypeKind.EntityType == edmType.BuiltInTypeKind)
            {
                return ((ClrEntityType) edmType).CSpaceTypeName;
            }
            if (BuiltInTypeKind.ComplexType == edmType.BuiltInTypeKind)
            {
                return ((ClrComplexType) edmType).CSpaceTypeName;
            }
            return edmType.Identity;
        }

        internal bool TryGetOSpaceType(EdmType cspaceType, out EdmType edmType)
        {
            if ((BuiltInTypeKind.EntityType != cspaceType.BuiltInTypeKind) && (BuiltInTypeKind.ComplexType != cspaceType.BuiltInTypeKind))
            {
                return base.TryGetItem<EdmType>(cspaceType.Identity, out edmType);
            }
            return this._ocMapping.TryGetValue(cspaceType.Identity, out edmType);
        }

        internal object LoadAssemblyLock =>
            this._loadAssemblyLock;

        internal static IList<Assembly> ViewGenerationAssemblies =>
            AssemblyCacheEntry.ViewGenerationAssemblies;


        private class AssemblyCacheEntry
        {
            private static object _assemblyCacheLock = new object();
            private readonly List<Assembly> _referencedAssemblies = new List<Assembly>();
            private readonly List<EdmType> _typesInAssembly = new List<EdmType>();
            private static IList<Assembly> s_viewGenAssemblies = new ThreadSafeList<Assembly>();

            private bool ContainsType(string typeName)
            {
                EdmType edmType = null;
                return this.TryGetEdmType(typeName, out edmType);
            }

            private static void InternalLoadAllReferencedAssemblies(LoadingContext context)
            {
                foreach (AssemblyName name in context.CurrentAssembly.GetReferencedAssemblies())
                {
                    bool flag;
                    string fullName = name.FullName;
                    if (ObjectItemCollection.ShouldFilterAssembly(fullName))
                    {
                        continue;
                    }
                    EntityBid.Trace("<md.ObjectItemCollection.LoadAllReferencedAssemblies|Info> loadededAssembly='%ls'\n", fullName);
                    Assembly assembly = SafeLoadReferencedAssembly(fullName);
                    if (assembly == null)
                    {
                        continue;
                    }
                    context.UpdateCurrentAssembly(assembly, false);
                    if (context.KnownAssemblies.TryGetValue(assembly, out flag))
                    {
                        if (!flag)
                        {
                            goto Label_0068;
                        }
                        continue;
                    }
                    if (ObjectItemCollection.IsSchemaAttributePresent(assembly))
                    {
                        InternalLoadAssemblyFromCache(context);
                    }
                Label_0068:
                    context.KnownAssemblies[assembly] = true;
                    InternalLoadAllReferencedAssemblies(context);
                }
            }

            private static bool InternalLoadAssemblyFromCache(LoadingContext context)
            {
                bool isAssemblyAlreadyLoadedInCache = context.IsAssemblyAlreadyLoadedInCache;
                if (context.IsAssemblyAlreadyLoadedInCache)
                {
                    foreach (EdmType type in context.AssemblyCacheEntry._typesInAssembly)
                    {
                        if (!context.TypesInLoading.ContainsKey(type.Identity))
                        {
                            context.TypesInLoading.Add(type.Identity, type);
                        }
                    }
                }
                else
                {
                    LoadTypesFromAssembly(context);
                }
                context.KnownAssemblies.Add(context.CurrentAssembly, false);
                foreach (Assembly assembly in context.AssemblyCacheEntry._referencedAssemblies)
                {
                    if (!context.KnownAssemblies.ContainsKey(assembly))
                    {
                        context.UpdateCurrentAssembly(assembly, context.IsAssemblyAlreadyLoadedInCache);
                        isAssemblyAlreadyLoadedInCache |= InternalLoadAssemblyFromCache(context);
                    }
                }
                return isAssemblyAlreadyLoadedInCache;
            }

            internal static void LoadAssemblyFromCache(Assembly assembly, bool loadReferencedAssemblies, Dictionary<Assembly, bool> knownAssemblies, out Dictionary<string, EdmType> typesInLoading, out List<EdmItemError> errors)
            {
                typesInLoading = null;
                errors = null;
                lock (_assemblyCacheLock)
                {
                    LoadingContext context = new LoadingContext(assembly, knownAssemblies);
                    if (!context.KnownAssemblies.ContainsKey(context.CurrentAssembly) && ObjectItemCollection.IsSchemaAttributePresent(context.CurrentAssembly))
                    {
                        InternalLoadAssemblyFromCache(context);
                    }
                    if (loadReferencedAssemblies)
                    {
                        InternalLoadAllReferencedAssemblies(context);
                    }
                    ResolveNavigationProperties(context);
                    new EdmValidator { SkipReadOnlyItems = true }.Validate<EdmType>(context.TypesInLoading.Values, context.EdmItemError);
                    context.UpdateCacheWithAssembliesLoaded();
                    typesInLoading = context.TypesInLoading;
                    errors = context.EdmItemError;
                    if ((typesInLoading != null) && (typesInLoading.Count > 0))
                    {
                        foreach (EdmType type in typesInLoading.Values)
                        {
                            type.SetReadOnly();
                        }
                    }
                }
            }

            private static EdmMember LoadComplexTypeProperty(PropertyInfo property, LoadingContext context)
            {
                EdmType edmType = LoadFromType(property.PropertyType, context);
                if ((edmType == null) || (edmType.BuiltInTypeKind != BuiltInTypeKind.ComplexType))
                {
                    context.EdmItemError.Add(new EdmItemError(Strings.Validator_OSpace_ComplexPropertyNotComplex(property.Name, property.DeclaringType.FullName, property.PropertyType.FullName), null));
                    return null;
                }
                FacetValues values = new FacetValues {
                    Nullable = 0
                };
                return new EdmProperty(property.Name, TypeUsage.Create(edmType, values), property);
            }

            private static EdmType LoadFromType(Type clrType, LoadingContext context)
            {
                EdmType edmType = null;
                if (clrType.IsNested)
                {
                    context.EdmItemError.Add(new EdmItemError(Strings.NestedClassNotSupported(clrType.FullName, clrType.Assembly.FullName), null));
                    return null;
                }
                if (!clrType.IsGenericType && context.IsTypeAlreadyLoaded(clrType, out edmType))
                {
                    if (edmType.ClrType != clrType)
                    {
                        context.EdmItemError.Add(new EdmItemError(Strings.NewTypeConflictsWithExistingType(clrType.AssemblyQualifiedName, edmType.ClrType.AssemblyQualifiedName), edmType));
                        return null;
                    }
                    return edmType;
                }
                EdmTypeAttribute[] customAttributes = (EdmTypeAttribute[]) clrType.GetCustomAttributes(typeof(EdmTypeAttribute), false);
                if (customAttributes.Length == 0)
                {
                    return ResolveNonSchemaType(clrType, context);
                }
                EdmTypeAttribute attribute = customAttributes[0];
                string cspaceTypeName = string.IsNullOrEmpty(attribute.Name) ? clrType.Name : attribute.Name;
                if (string.IsNullOrEmpty(attribute.NamespaceName) && (clrType.Namespace == null))
                {
                    context.EdmItemError.Add(new EdmItemError(Strings.Validator_TypeHasNoNamespace, edmType));
                    return null;
                }
                string cspaceNamespaceName = string.IsNullOrEmpty(attribute.NamespaceName) ? clrType.Namespace : attribute.NamespaceName;
                if (attribute.GetType() == typeof(EdmEntityTypeAttribute))
                {
                    edmType = new ClrEntityType(clrType, cspaceNamespaceName, cspaceTypeName);
                }
                else
                {
                    edmType = new ClrComplexType(clrType, cspaceNamespaceName, cspaceTypeName);
                }
                if (clrType.Assembly != context.CurrentAssembly)
                {
                    if (ObjectItemCollection.ShouldFilterAssembly(clrType.Assembly.FullName) || !ObjectItemCollection.IsSchemaAttributePresent(clrType.Assembly))
                    {
                        context.EdmItemError.Add(new EdmItemError(Strings.MissingAssemblyAttribute(clrType.FullName, clrType.Assembly.FullName), edmType));
                        return null;
                    }
                    if (!context.AssemblyCacheEntry._referencedAssemblies.Contains(clrType.Assembly))
                    {
                        context.AssemblyCacheEntry._referencedAssemblies.Add(clrType.Assembly);
                    }
                }
                else
                {
                    context.AssemblyCacheEntry._typesInAssembly.Add(edmType);
                }
                context.TypesInLoading.Add(clrType.FullName, edmType);
                if (Helper.IsStructuralType(edmType))
                {
                    if (Helper.IsEntityType(edmType))
                    {
                        edmType.BaseType = LoadFromType(clrType.BaseType, context);
                    }
                    LoadPropertiesFromType((StructuralType) edmType, context);
                }
                return edmType;
            }

            private static void LoadPropertiesFromType(StructuralType structuralType, LoadingContext context)
            {
                foreach (PropertyInfo info in structuralType.ClrType.GetProperties(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly))
                {
                    EdmMember member = null;
                    bool isEntityKeyProperty = false;
                    if (info.IsDefined(typeof(EdmRelationshipNavigationPropertyAttribute), false))
                    {
                        SaveNavigationProperty(structuralType, info, context);
                    }
                    else if (info.IsDefined(typeof(EdmScalarPropertyAttribute), false))
                    {
                        member = LoadScalarProperty(info, context, out isEntityKeyProperty);
                    }
                    else if (info.IsDefined(typeof(EdmComplexPropertyAttribute), false))
                    {
                        member = LoadComplexTypeProperty(info, context);
                    }
                    if (member != null)
                    {
                        structuralType.AddMember(member);
                        if ((structuralType.BuiltInTypeKind == BuiltInTypeKind.EntityType) && isEntityKeyProperty)
                        {
                            ((EntityType) structuralType).AddKeyMember(member);
                        }
                    }
                }
            }

            private static void LoadRelationshipTypes(LoadingContext context)
            {
                foreach (EdmRelationshipAttribute attribute in context.CurrentAssembly.GetCustomAttributes(typeof(EdmRelationshipAttribute), false))
                {
                    if (!TryFindNullParametersInRelationshipAttribute(attribute, context))
                    {
                        EntityType type;
                        EntityType type2;
                        bool flag = false;
                        if (attribute.Role1Name == attribute.Role2Name)
                        {
                            context.EdmItemError.Add(new EdmItemError(Strings.SameRoleNameOnRelationshipAttribute(attribute.RelationshipName, attribute.Role2Name), null));
                            flag = true;
                        }
                        if (!TryGetRelationshipEndEntityType(context, attribute.Role1Type, out type))
                        {
                            context.EdmItemError.Add(new EdmItemError(Strings.RoleTypeInEdmRelationshipAttributeIsInvalidType(attribute.RelationshipName, attribute.Role1Name, attribute.Role1Type), null));
                            flag = true;
                        }
                        if (!TryGetRelationshipEndEntityType(context, attribute.Role2Type, out type2))
                        {
                            context.EdmItemError.Add(new EdmItemError(Strings.RoleTypeInEdmRelationshipAttributeIsInvalidType(attribute.RelationshipName, attribute.Role2Name, attribute.Role2Type), null));
                            flag = true;
                        }
                        if (!flag)
                        {
                            AssociationType type3 = new AssociationType(attribute.RelationshipName, attribute.RelationshipNamespaceName, DataSpace.OSpace);
                            type3.AddKeyMember(new AssociationEndMember(attribute.Role1Name, type.GetReferenceType(), attribute.Role1Multiplicity));
                            type3.AddKeyMember(new AssociationEndMember(attribute.Role2Name, type2.GetReferenceType(), attribute.Role2Multiplicity));
                            context.TypesInLoading.Add(type3.FullName, type3);
                            context.AssemblyCacheEntry._typesInAssembly.Add(type3);
                        }
                    }
                }
            }

            private static EdmMember LoadScalarProperty(PropertyInfo property, LoadingContext context, out bool isEntityKeyProperty)
            {
                EdmMember member = null;
                isEntityKeyProperty = false;
                EdmType edmType = LoadFromType(property.PropertyType, context);
                if ((edmType == null) || (edmType.BuiltInTypeKind != BuiltInTypeKind.PrimitiveType))
                {
                    context.EdmItemError.Add(new EdmItemError(Strings.Validator_OSpace_ScalarPropertyNotPrimitive(property.Name, property.DeclaringType.FullName, property.PropertyType.FullName), null));
                    return member;
                }
                object[] customAttributes = property.GetCustomAttributes(typeof(EdmScalarPropertyAttribute), false);
                isEntityKeyProperty = ((EdmScalarPropertyAttribute) customAttributes[0]).EntityKeyProperty;
                bool isNullable = ((EdmScalarPropertyAttribute) customAttributes[0]).IsNullable;
                FacetValues values = new FacetValues {
                    Nullable = new bool?(isNullable)
                };
                return new EdmProperty(property.Name, TypeUsage.Create(edmType, values), property);
            }

            private static void LoadTypesFromAssembly(LoadingContext context)
            {
                LoadRelationshipTypes(context);
                foreach (Type type in context.CurrentAssembly.GetTypes())
                {
                    if (type.IsDefined(typeof(EdmTypeAttribute), false))
                    {
                        LoadFromType(type, context);
                    }
                }
            }

            private static void ResolveNavigationProperties(LoadingContext context)
            {
                foreach (NavigationPropertyInfo info in context.UnresolvedNavigationProperties)
                {
                    info.ResolveNavigationProperty(context);
                }
            }

            private static EdmType ResolveNonSchemaType(Type clrType, LoadingContext context)
            {
                PrimitiveType type4;
                if (clrType.IsGenericType)
                {
                    Type genericTypeDefinition = clrType.GetGenericTypeDefinition();
                    EdmType type2 = LoadFromType(clrType.GetGenericArguments()[0], context);
                    if (type2 == null)
                    {
                        return null;
                    }
                    if (genericTypeDefinition == typeof(Nullable<>))
                    {
                        return type2;
                    }
                    if (genericTypeDefinition == typeof(EntityReference<>))
                    {
                        return null;
                    }
                    if (typeof(IEnumerable).IsAssignableFrom(clrType))
                    {
                        EntityType type3 = type2 as EntityType;
                        return type3?.GetCollectionType();
                    }
                }
                if (ClrProviderManifest.Instance.TryGetPrimitiveType(clrType, out type4))
                {
                    return type4;
                }
                return null;
            }

            internal static Assembly SafeLoadReferencedAssembly(string assemblyFullName)
            {
                Assembly assembly = null;
                try
                {
                    assembly = Assembly.Load(assemblyFullName);
                }
                catch (FileNotFoundException)
                {
                }
                return assembly;
            }

            private static void SaveNavigationProperty(StructuralType declaringType, PropertyInfo property, LoadingContext context)
            {
                object[] customAttributes = property.GetCustomAttributes(typeof(EdmRelationshipNavigationPropertyAttribute), false);
                EdmType propertyType = LoadFromType(property.PropertyType, context);
                if ((propertyType == null) || ((propertyType.BuiltInTypeKind != BuiltInTypeKind.EntityType) && (propertyType.BuiltInTypeKind != BuiltInTypeKind.CollectionType)))
                {
                    context.EdmItemError.Add(new EdmItemError(Strings.Validator_OSpace_InvalidNavPropReturnType(property.Name, property.DeclaringType.FullName, property.PropertyType.FullName), null));
                }
                else
                {
                    EdmRelationshipNavigationPropertyAttribute attribute = (EdmRelationshipNavigationPropertyAttribute) customAttributes[0];
                    context.UnresolvedNavigationProperties.Add(new NavigationPropertyInfo(declaringType, property, propertyType, attribute));
                }
            }

            private static bool TryFindNullParametersInRelationshipAttribute(EdmRelationshipAttribute roleAttribute, LoadingContext context)
            {
                if (roleAttribute.RelationshipName == null)
                {
                    context.EdmItemError.Add(new EdmItemError(Strings.NullRelationshipNameforEdmRelationshipAttribute(context.CurrentAssembly.FullName), null));
                    return true;
                }
                bool flag = false;
                if (roleAttribute.RelationshipNamespaceName == null)
                {
                    context.EdmItemError.Add(new EdmItemError(Strings.NullParameterForEdmRelationshipAttribute("RelationshipNamespaceName", roleAttribute.RelationshipName), null));
                    flag = true;
                }
                if (roleAttribute.Role1Name == null)
                {
                    context.EdmItemError.Add(new EdmItemError(Strings.NullParameterForEdmRelationshipAttribute("Role1Name", roleAttribute.RelationshipName), null));
                    flag = true;
                }
                if (roleAttribute.Role1Type == null)
                {
                    context.EdmItemError.Add(new EdmItemError(Strings.NullParameterForEdmRelationshipAttribute("Role1Type", roleAttribute.RelationshipName), null));
                    flag = true;
                }
                if (roleAttribute.Role2Name == null)
                {
                    context.EdmItemError.Add(new EdmItemError(Strings.NullParameterForEdmRelationshipAttribute("Role2Name", roleAttribute.RelationshipName), null));
                    flag = true;
                }
                if (roleAttribute.Role2Type == null)
                {
                    context.EdmItemError.Add(new EdmItemError(Strings.NullParameterForEdmRelationshipAttribute("Role2Type", roleAttribute.RelationshipName), null));
                    flag = true;
                }
                return flag;
            }

            private bool TryGetEdmType(string typeName, out EdmType edmType)
            {
                edmType = null;
                foreach (EdmType type in this._typesInAssembly)
                {
                    if (type.Identity == typeName)
                    {
                        edmType = type;
                        break;
                    }
                }
                return (edmType != null);
            }

            private static bool TryGetRelationshipEndEntityType(LoadingContext context, Type type, out EntityType entityType)
            {
                if (type == null)
                {
                    entityType = null;
                    return false;
                }
                EdmType type2 = LoadFromType(type, context);
                if ((type2 == null) || !Helper.IsEntityType(type2))
                {
                    entityType = null;
                    return false;
                }
                entityType = (EntityType) type2;
                return true;
            }

            internal static IList<Assembly> ViewGenerationAssemblies =>
                s_viewGenAssemblies;

            private class LoadingContext
            {
                private Assembly _currentAssembly;
                private System.Data.Metadata.Edm.ObjectItemCollection.AssemblyCacheEntry _currentCacheEntry;
                private readonly List<System.Data.Metadata.Edm.EdmItemError> _errors = new List<System.Data.Metadata.Edm.EdmItemError>();
                private bool _isAssemblyLoadedFromGlobalCache;
                private readonly Dictionary<Assembly, bool> _knownAssemblies;
                private readonly Dictionary<Assembly, System.Data.Metadata.Edm.ObjectItemCollection.AssemblyCacheEntry> _listOfAssembliesLoaded = new Dictionary<Assembly, System.Data.Metadata.Edm.ObjectItemCollection.AssemblyCacheEntry>();
                private readonly Dictionary<string, EdmType> _typesInLoading = new Dictionary<string, EdmType>(StringComparer.Ordinal);
                private readonly List<System.Data.Metadata.Edm.ObjectItemCollection.AssemblyCacheEntry.NavigationPropertyInfo> _unresolvedNavigationProperties = new List<System.Data.Metadata.Edm.ObjectItemCollection.AssemblyCacheEntry.NavigationPropertyInfo>();
                private static readonly Dictionary<Assembly, System.Data.Metadata.Edm.ObjectItemCollection.AssemblyCacheEntry> s_globalAssemblyCache = new Dictionary<Assembly, System.Data.Metadata.Edm.ObjectItemCollection.AssemblyCacheEntry>();

                internal LoadingContext(Assembly assembly, Dictionary<Assembly, bool> knownAssemblies)
                {
                    this._knownAssemblies = knownAssemblies;
                    this.UpdateCurrentAssembly(assembly, false);
                }

                private bool IsTypeAlreadyInCache(Type clrType, out EdmType edmType)
                {
                    System.Data.Metadata.Edm.ObjectItemCollection.AssemblyCacheEntry entry;
                    edmType = null;
                    if (clrType.Assembly == this._currentAssembly)
                    {
                        return false;
                    }
                    if (s_globalAssemblyCache.TryGetValue(clrType.Assembly, out entry))
                    {
                        return entry.TryGetEdmType(clrType.FullName, out edmType);
                    }
                    return (this._listOfAssembliesLoaded.TryGetValue(clrType.Assembly, out entry) && entry.TryGetEdmType(clrType.FullName, out edmType));
                }

                internal bool IsTypeAlreadyLoaded(Type clrType, out EdmType edmType)
                {
                    edmType = null;
                    bool flag = false;
                    bool flag2 = false;
                    if ((!clrType.IsGenericType && ((flag2 = this.TypesInLoading.TryGetValue(clrType.FullName, out edmType)) || (flag = this.IsTypeAlreadyInCache(clrType, out edmType)))) && !Helper.IsPrimitiveType(edmType))
                    {
                        if (clrType.Assembly != this._currentAssembly)
                        {
                            if (!this._currentCacheEntry._referencedAssemblies.Contains(clrType.Assembly))
                            {
                                this._currentCacheEntry._referencedAssemblies.Add(clrType.Assembly);
                            }
                        }
                        else if (flag2 && !this._currentCacheEntry.ContainsType(edmType.Identity))
                        {
                            this._currentCacheEntry._typesInAssembly.Add(edmType);
                        }
                        if (flag && !this.KnownAssemblies.ContainsKey(clrType.Assembly))
                        {
                            this.TypesInLoading.Add(clrType.FullName, edmType);
                        }
                    }
                    return (edmType != null);
                }

                internal void UpdateCacheWithAssembliesLoaded()
                {
                    if (this._errors.Count == 0)
                    {
                        foreach (KeyValuePair<Assembly, System.Data.Metadata.Edm.ObjectItemCollection.AssemblyCacheEntry> pair in this._listOfAssembliesLoaded)
                        {
                            s_globalAssemblyCache.Add(pair.Key, pair.Value);
                        }
                        this._listOfAssembliesLoaded.Clear();
                    }
                }

                internal void UpdateCurrentAssembly(Assembly assembly, bool mustAssemblyBeAlreadyLoaded)
                {
                    this._currentAssembly = assembly;
                    if (mustAssemblyBeAlreadyLoaded)
                    {
                        this._currentCacheEntry = s_globalAssemblyCache[assembly];
                        this._isAssemblyLoadedFromGlobalCache = true;
                    }
                    else if (s_globalAssemblyCache.TryGetValue(assembly, out this._currentCacheEntry))
                    {
                        this._isAssemblyLoadedFromGlobalCache = true;
                    }
                    else
                    {
                        if (!this._listOfAssembliesLoaded.TryGetValue(assembly, out this._currentCacheEntry))
                        {
                            this._currentCacheEntry = new System.Data.Metadata.Edm.ObjectItemCollection.AssemblyCacheEntry();
                            this._listOfAssembliesLoaded.Add(this._currentAssembly, this._currentCacheEntry);
                        }
                        this._isAssemblyLoadedFromGlobalCache = false;
                    }
                }

                internal System.Data.Metadata.Edm.ObjectItemCollection.AssemblyCacheEntry AssemblyCacheEntry =>
                    this._currentCacheEntry;

                internal Assembly CurrentAssembly =>
                    this._currentAssembly;

                internal List<System.Data.Metadata.Edm.EdmItemError> EdmItemError =>
                    this._errors;

                internal bool IsAssemblyAlreadyLoadedInCache =>
                    this._isAssemblyLoadedFromGlobalCache;

                internal Dictionary<Assembly, bool> KnownAssemblies =>
                    this._knownAssemblies;

                internal Dictionary<string, EdmType> TypesInLoading =>
                    this._typesInLoading;

                internal List<System.Data.Metadata.Edm.ObjectItemCollection.AssemblyCacheEntry.NavigationPropertyInfo> UnresolvedNavigationProperties =>
                    this._unresolvedNavigationProperties;
            }

            private class NavigationPropertyInfo
            {
                private EdmRelationshipNavigationPropertyAttribute _attribute;
                private StructuralType _declaringType;
                private PropertyInfo _propertyInfo;
                private EdmType _propertyType;

                internal NavigationPropertyInfo(StructuralType declaringType, PropertyInfo propertyInfo, EdmType propertyType, EdmRelationshipNavigationPropertyAttribute attribute)
                {
                    this._declaringType = declaringType;
                    this._propertyInfo = propertyInfo;
                    this._propertyType = propertyType;
                    this._attribute = attribute;
                }

                internal void ResolveNavigationProperty(ObjectItemCollection.AssemblyCacheEntry.LoadingContext context)
                {
                    EdmMember member = null;
                    EdmType type;
                    if (context.TypesInLoading.TryGetValue(this._attribute.RelationshipNamespaceName + "." + this._attribute.RelationshipName, out type) && Helper.IsAssociationType(type))
                    {
                        AssociationType type2 = (AssociationType) type;
                        if (type2 != null)
                        {
                            System.Data.Metadata.Edm.NavigationProperty item = new System.Data.Metadata.Edm.NavigationProperty(this._propertyInfo.Name, TypeUsage.Create(this._propertyType), this._propertyInfo) {
                                RelationshipType = type2
                            };
                            member = item;
                            if (type2.Members[0].Name == this._attribute.TargetRoleName)
                            {
                                item.ToEndMember = (RelationshipEndMember) type2.Members[0];
                                item.FromEndMember = (RelationshipEndMember) type2.Members[1];
                            }
                            else if (type2.Members[1].Name == this._attribute.TargetRoleName)
                            {
                                item.ToEndMember = (RelationshipEndMember) type2.Members[1];
                                item.FromEndMember = (RelationshipEndMember) type2.Members[0];
                            }
                            else
                            {
                                context.EdmItemError.Add(new EdmItemError(Strings.TargetRoleNameInNavigationPropertyNotValid(this._propertyInfo.Name, this._propertyInfo.DeclaringType.FullName, this._attribute.TargetRoleName, this._attribute.RelationshipName), item));
                                member = null;
                            }
                            if ((member != null) && (((RefType) item.FromEndMember.TypeUsage.EdmType).ElementType.ClrType != this._declaringType.ClrType))
                            {
                                context.EdmItemError.Add(new EdmItemError(Strings.NavigationPropertyRelationshipEndTypeMismatch(this._declaringType.FullName, item.Name, type2.FullName, item.FromEndMember.Name, ((RefType) item.FromEndMember.TypeUsage.EdmType).ElementType.ClrType), item));
                                member = null;
                            }
                        }
                    }
                    else
                    {
                        context.EdmItemError.Add(new EdmItemError(Strings.RelationshipNameInNavigationPropertyNotValid(this._propertyInfo.Name, this._propertyInfo.DeclaringType.FullName, this._attribute.RelationshipName), this._declaringType));
                    }
                    if (member != null)
                    {
                        this._declaringType.AddMember(member);
                    }
                }
            }
        }
    }
}

