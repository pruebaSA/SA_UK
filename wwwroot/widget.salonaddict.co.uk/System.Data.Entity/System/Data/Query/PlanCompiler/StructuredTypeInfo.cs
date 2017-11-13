namespace System.Data.Query.PlanCompiler
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data.Common;
    using System.Data.Common.Utils;
    using System.Data.Metadata.Edm;
    using System.Data.Query.InternalTrees;
    using System.Globalization;
    using System.Runtime.InteropServices;

    internal class StructuredTypeInfo
    {
        private Dictionary<EntitySetBase, ExplicitDiscriminatorMap> m_discriminatorMaps;
        private EntitySet[] m_entitySetIdToEntitySetMap;
        private Dictionary<EntitySet, int> m_entitySetToEntitySetIdMap;
        private Dictionary<EntityTypeBase, EntitySet> m_entityTypeToEntitySetMap;
        private TypeUsage m_intType;
        private System.Data.Query.InternalTrees.RelPropertyHelper m_relPropertyHelper;
        private TypeUsage m_stringType;
        private Dictionary<TypeUsage, TypeInfo> m_typeInfoMap = new Dictionary<TypeUsage, TypeInfo>(TypeUsageEqualityComparer.Instance);
        private bool m_typeInfoMapPopulated = false;
        private HashSet<string> m_typesNeedingNullSentinel;

        private StructuredTypeInfo(HashSet<string> typesNeedingNullSentinel)
        {
            this.m_typesNeedingNullSentinel = typesNeedingNullSentinel;
        }

        private void AddEntityTypeToSetEntry(EntityType entityType, EntitySet entitySet)
        {
            EntityTypeBase rootType = GetRootType(entityType);
            bool flag = true;
            if (entitySet == null)
            {
                flag = false;
            }
            else
            {
                EntitySet set;
                if (this.m_entityTypeToEntitySetMap.TryGetValue(rootType, out set) && (set != entitySet))
                {
                    flag = false;
                }
            }
            if (flag)
            {
                this.m_entityTypeToEntitySetMap[rootType] = entitySet;
            }
            else
            {
                this.m_entityTypeToEntitySetMap[rootType] = null;
            }
        }

        private void AddRelProperties(TypeInfo typeInfo)
        {
            EntityTypeBase edmType = (EntityTypeBase) typeInfo.Type.EdmType;
            foreach (RelProperty property in this.m_relPropertyHelper.GetDeclaredOnlyRelProperties(edmType))
            {
                EdmType type1 = property.ToEnd.TypeUsage.EdmType;
                TypeInfo info = this.GetTypeInfo(property.ToEnd.TypeUsage);
                this.ExplodeType(info);
                foreach (PropertyRef ref2 in info.PropertyRefList)
                {
                    typeInfo.RootType.AddPropertyRef(ref2.CreateNestedPropertyRef(property));
                }
            }
            foreach (TypeInfo info2 in typeInfo.ImmediateSubTypes)
            {
                this.AddRelProperties(info2);
            }
        }

        private void AssignEntitySetIds(List<EntitySet> referencedEntitySets)
        {
            this.m_entitySetIdToEntitySetMap = new EntitySet[referencedEntitySets.Count];
            this.m_entitySetToEntitySetIdMap = new Dictionary<EntitySet, int>();
            int index = 0;
            foreach (EntitySet set in referencedEntitySets)
            {
                if (!this.m_entitySetToEntitySetIdMap.ContainsKey(set))
                {
                    this.m_entitySetIdToEntitySetMap[index] = set;
                    this.m_entitySetToEntitySetIdMap[set] = index;
                    index++;
                }
            }
        }

        private void AssignRootTypeId(TypeInfo typeInfo, string typeId)
        {
            typeInfo.TypeId = typeId;
            this.AssignTypeIdsToSubTypes(typeInfo);
        }

        private void AssignTypeId(TypeInfo typeInfo, int subtypeNum)
        {
            typeInfo.TypeId = string.Format(CultureInfo.InvariantCulture, "{0}{1}X", new object[] { typeInfo.SuperType.TypeId, subtypeNum });
            this.AssignTypeIdsToSubTypes(typeInfo);
        }

        private void AssignTypeIds()
        {
            int num = 0;
            foreach (KeyValuePair<TypeUsage, TypeInfo> pair in this.m_typeInfoMap)
            {
                if (pair.Value.RootType.DiscriminatorMap != null)
                {
                    EntityType edmType = (EntityType) pair.Key.EdmType;
                    pair.Value.TypeId = pair.Value.RootType.DiscriminatorMap.GetTypeId(edmType);
                }
                else if (pair.Value.IsRootType && (TypeSemantics.IsEntityType(pair.Key) || TypeSemantics.IsComplexType(pair.Key)))
                {
                    this.AssignRootTypeId(pair.Value, string.Format(CultureInfo.InvariantCulture, "{0}X", new object[] { num }));
                    num++;
                }
            }
        }

        private void AssignTypeIdsToSubTypes(TypeInfo typeInfo)
        {
            int subtypeNum = 0;
            foreach (TypeInfo info in typeInfo.ImmediateSubTypes)
            {
                this.AssignTypeId(info, subtypeNum);
                subtypeNum++;
            }
        }

        private void CreateFlattenedRecordType(RootTypeInfo type)
        {
            bool flag;
            if (TypeSemantics.IsEntityType(type.Type) && (type.ImmediateSubTypes.Count == 0))
            {
                flag = true;
            }
            else
            {
                flag = false;
            }
            List<KeyValuePair<string, TypeUsage>> columns = new List<KeyValuePair<string, TypeUsage>>();
            int num = 0;
            foreach (PropertyRef ref2 in type.PropertyRefList)
            {
                string key = null;
                if (flag)
                {
                    SimplePropertyRef ref3 = ref2 as SimplePropertyRef;
                    if (ref3 != null)
                    {
                        key = ref3.Property.Name;
                    }
                }
                TypeUsage propertyType = this.GetPropertyType(type, ref2);
                if (key == null)
                {
                    key = "F" + num.ToString(CultureInfo.InvariantCulture);
                }
                columns.Add(new KeyValuePair<string, TypeUsage>(key, propertyType));
                num++;
            }
            type.FlattenedType = TypeHelpers.CreateRowType(columns);
            IEnumerator<PropertyRef> enumerator = type.PropertyRefList.GetEnumerator();
            foreach (EdmProperty property in type.FlattenedType.Properties)
            {
                if (!enumerator.MoveNext())
                {
                    System.Data.Query.PlanCompiler.PlanCompiler.Assert(false, "property refs count and flattened type member count mismatch?");
                }
                type.AddPropertyMapping(enumerator.Current, property);
            }
        }

        private TypeInfo CreateTypeInfoForStructuredType(TypeUsage type, ExplicitDiscriminatorMap discriminatorMap)
        {
            System.Data.Query.PlanCompiler.PlanCompiler.Assert(TypeUtils.IsStructuredType(type), "expected structured type. Found " + type);
            TypeInfo typeInfo = this.GetTypeInfo(type);
            if (typeInfo == null)
            {
                EntityTypeBase base2;
                TypeInfo superTypeInfo = null;
                if (type.EdmType.BaseType != null)
                {
                    superTypeInfo = this.CreateTypeInfoForStructuredType(TypeUsage.Create(type.EdmType.BaseType), discriminatorMap);
                }
                else
                {
                    RefType type2;
                    if (TypeHelpers.TryGetEdmType<RefType>(type, out type2))
                    {
                        EntityType elementType = type2.ElementType as EntityType;
                        if ((elementType != null) && (elementType.BaseType != null))
                        {
                            TypeUsage usage = TypeHelpers.CreateReferenceTypeUsage(elementType.BaseType as EntityType);
                            superTypeInfo = this.CreateTypeInfoForStructuredType(usage, discriminatorMap);
                        }
                    }
                }
                foreach (EdmMember member in TypeHelpers.GetDeclaredStructuralMembers(type))
                {
                    this.CreateTypeInfoForType(member.TypeUsage);
                }
                if (TypeHelpers.TryGetEdmType<EntityTypeBase>(type, out base2))
                {
                    foreach (RelProperty property in this.m_relPropertyHelper.GetDeclaredOnlyRelProperties(base2))
                    {
                        this.CreateTypeInfoForType(property.ToEnd.TypeUsage);
                    }
                }
                typeInfo = TypeInfo.Create(type, superTypeInfo, discriminatorMap);
                this.m_typeInfoMap.Add(type, typeInfo);
            }
            return typeInfo;
        }

        private void CreateTypeInfoForType(TypeUsage type)
        {
            while (TypeUtils.IsCollectionType(type))
            {
                type = TypeHelpers.GetEdmType<CollectionType>(type).TypeUsage;
            }
            if (TypeUtils.IsStructuredType(type))
            {
                ExplicitDiscriminatorMap map;
                this.TryGetDiscriminatorMap(type.EdmType, out map);
                this.CreateTypeInfoForStructuredType(type, map);
            }
        }

        private void ExplodeRootStructuredType(RootTypeInfo rootType)
        {
            if (rootType.FlattenedType == null)
            {
                if (this.NeedsTypeIdProperty(rootType))
                {
                    rootType.AddPropertyRef(TypeIdPropertyRef.Instance);
                    if (rootType.DiscriminatorMap != null)
                    {
                        rootType.TypeIdKind = TypeIdKind.UserSpecified;
                        rootType.TypeIdType = Helper.GetModelTypeUsage(rootType.DiscriminatorMap.DiscriminatorProperty);
                    }
                    else
                    {
                        rootType.TypeIdKind = TypeIdKind.Generated;
                        rootType.TypeIdType = this.m_stringType;
                    }
                }
                if (this.NeedsEntitySetIdProperty(rootType))
                {
                    rootType.AddPropertyRef(EntitySetIdPropertyRef.Instance);
                }
                if (this.NeedsNullSentinelProperty(rootType))
                {
                    rootType.AddPropertyRef(NullSentinelPropertyRef.Instance);
                }
                this.ExplodeRootStructuredTypeHelper(rootType);
                if (TypeSemantics.IsEntityType(rootType.Type))
                {
                    this.AddRelProperties(rootType);
                }
                this.CreateFlattenedRecordType(rootType);
            }
        }

        private void ExplodeRootStructuredTypeHelper(TypeInfo typeInfo)
        {
            RefType type;
            RootTypeInfo rootType = typeInfo.RootType;
            IEnumerable keyMembers = null;
            if (TypeHelpers.TryGetEdmType<RefType>(typeInfo.Type, out type))
            {
                if (!typeInfo.IsRootType)
                {
                    return;
                }
                keyMembers = type.ElementType.KeyMembers;
            }
            else
            {
                keyMembers = TypeHelpers.GetDeclaredStructuralMembers(typeInfo.Type);
            }
            foreach (EdmMember member in keyMembers)
            {
                TypeInfo info2 = this.ExplodeType(member.TypeUsage);
                if (info2 == null)
                {
                    rootType.AddPropertyRef(new SimplePropertyRef(member));
                }
                else
                {
                    foreach (PropertyRef ref2 in info2.PropertyRefList)
                    {
                        rootType.AddPropertyRef(ref2.CreateNestedPropertyRef(member));
                    }
                }
            }
            foreach (TypeInfo info3 in typeInfo.ImmediateSubTypes)
            {
                this.ExplodeRootStructuredTypeHelper(info3);
            }
        }

        private TypeInfo ExplodeType(TypeUsage type)
        {
            if (TypeUtils.IsStructuredType(type))
            {
                TypeInfo typeInfo = this.GetTypeInfo(type);
                this.ExplodeType(typeInfo);
                return typeInfo;
            }
            if (TypeUtils.IsCollectionType(type))
            {
                TypeUsage typeUsage = TypeHelpers.GetEdmType<CollectionType>(type).TypeUsage;
                this.ExplodeType(typeUsage);
                return null;
            }
            return null;
        }

        private void ExplodeType(TypeInfo typeInfo)
        {
            this.ExplodeRootStructuredType(typeInfo.RootType);
        }

        private void ExplodeTypes()
        {
            foreach (KeyValuePair<TypeUsage, TypeInfo> pair in this.m_typeInfoMap)
            {
                if (pair.Value.IsRootType)
                {
                    this.ExplodeType(pair.Value);
                }
            }
        }

        internal EntitySet GetEntitySet(EntityTypeBase type)
        {
            EntitySet set;
            EntityTypeBase rootType = GetRootType(type);
            if (!this.m_entityTypeToEntitySetMap.TryGetValue(rootType, out set))
            {
                return null;
            }
            return set;
        }

        internal int GetEntitySetId(EntitySet e)
        {
            int num = 0;
            if (!this.m_entitySetToEntitySetIdMap.TryGetValue(e, out num))
            {
                System.Data.Query.PlanCompiler.PlanCompiler.Assert(false, "no such entity set?");
            }
            return num;
        }

        internal Set<EntitySet> GetEntitySets() => 
            new Set<EntitySet>(this.m_entitySetIdToEntitySetMap).MakeReadOnly();

        private TypeUsage GetNewType(TypeUsage type)
        {
            TypeUsage usage;
            if (TypeUtils.IsStructuredType(type))
            {
                return this.GetTypeInfo(type).FlattenedTypeUsage;
            }
            if (!TypeHelpers.TryGetCollectionElementType(type, out usage))
            {
                return type;
            }
            TypeUsage newType = this.GetNewType(usage);
            if (newType.EdmEquals(usage))
            {
                return type;
            }
            return TypeHelpers.CreateCollectionTypeUsage(newType);
        }

        private TypeUsage GetPropertyType(RootTypeInfo typeInfo, PropertyRef p)
        {
            TypeUsage type = null;
            PropertyRef innerProperty = null;
            while (p is NestedPropertyRef)
            {
                NestedPropertyRef ref3 = (NestedPropertyRef) p;
                p = ref3.OuterProperty;
                innerProperty = ref3.InnerProperty;
            }
            if (p is TypeIdPropertyRef)
            {
                if ((innerProperty != null) && (innerProperty is SimplePropertyRef))
                {
                    TypeUsage typeUsage = ((SimplePropertyRef) innerProperty).Property.TypeUsage;
                    type = this.GetTypeInfo(typeUsage).RootType.TypeIdType;
                }
                else
                {
                    type = typeInfo.TypeIdType;
                }
            }
            else if ((p is EntitySetIdPropertyRef) || (p is NullSentinelPropertyRef))
            {
                type = this.m_intType;
            }
            else if (p is RelPropertyRef)
            {
                type = (p as RelPropertyRef).Property.ToEnd.TypeUsage;
            }
            else
            {
                SimplePropertyRef ref4 = p as SimplePropertyRef;
                if (ref4 != null)
                {
                    type = Helper.GetModelTypeUsage(ref4.Property);
                }
            }
            type = this.GetNewType(type);
            System.Data.Query.PlanCompiler.PlanCompiler.Assert(null != type, "unrecognized property type?");
            return type;
        }

        private static EntityTypeBase GetRootType(EntityTypeBase type)
        {
            while (type.BaseType != null)
            {
                type = (EntityTypeBase) type.BaseType;
            }
            return type;
        }

        internal TypeInfo GetTypeInfo(TypeUsage type)
        {
            if (!TypeUtils.IsStructuredType(type))
            {
                return null;
            }
            TypeInfo info = null;
            if (!this.m_typeInfoMap.TryGetValue(type, out info))
            {
                System.Data.Query.PlanCompiler.PlanCompiler.Assert(!TypeUtils.IsStructuredType(type) || !this.m_typeInfoMapPopulated, "cannot find typeInfo for type " + type);
            }
            return info;
        }

        private bool NeedsEntitySetIdProperty(TypeInfo typeInfo)
        {
            EntityType elementType;
            RefType edmType = typeInfo.Type.EdmType as RefType;
            if (edmType != null)
            {
                elementType = edmType.ElementType as EntityType;
            }
            else
            {
                elementType = typeInfo.Type.EdmType as EntityType;
            }
            return ((elementType != null) && (this.GetEntitySet(elementType) == null));
        }

        private bool NeedsNullSentinelProperty(TypeInfo typeInfo) => 
            this.m_typesNeedingNullSentinel.Contains(typeInfo.Type.EdmType.Identity);

        private bool NeedsTypeIdProperty(TypeInfo typeInfo) => 
            ((typeInfo.ImmediateSubTypes.Count > 0) && !TypeSemantics.IsReferenceType(typeInfo.Type));

        private void PopulateTypeInfoMap(List<TypeUsage> referencedTypes)
        {
            foreach (TypeUsage usage in referencedTypes)
            {
                this.CreateTypeInfoForType(usage);
            }
            this.m_typeInfoMapPopulated = true;
        }

        private void Process(Command itree, List<TypeUsage> referencedTypes, List<EntitySet> referencedEntitySets, List<EntityType> freeFloatingEntityConstructorTypes, Dictionary<EntitySetBase, DiscriminatorMapInfo> discriminatorMaps, System.Data.Query.InternalTrees.RelPropertyHelper relPropertyHelper)
        {
            System.Data.Query.PlanCompiler.PlanCompiler.Assert(null != itree, "null itree?");
            this.m_stringType = itree.StringType;
            this.m_intType = itree.IntegerType;
            this.m_relPropertyHelper = relPropertyHelper;
            this.ProcessEntitySets(referencedEntitySets, freeFloatingEntityConstructorTypes);
            this.ProcessDiscriminatorMaps(discriminatorMaps);
            this.ProcessTypes(referencedTypes);
        }

        internal static void Process(Command itree, List<TypeUsage> referencedTypes, List<EntitySet> referencedEntitySets, List<EntityType> freeFloatingEntityConstructorTypes, Dictionary<EntitySetBase, DiscriminatorMapInfo> discriminatorMaps, System.Data.Query.InternalTrees.RelPropertyHelper relPropertyHelper, HashSet<string> typesNeedingNullSentinel, out StructuredTypeInfo structuredTypeInfo)
        {
            structuredTypeInfo = new StructuredTypeInfo(typesNeedingNullSentinel);
            structuredTypeInfo.Process(itree, referencedTypes, referencedEntitySets, freeFloatingEntityConstructorTypes, discriminatorMaps, relPropertyHelper);
        }

        private void ProcessDiscriminatorMaps(Dictionary<EntitySetBase, DiscriminatorMapInfo> discriminatorMaps)
        {
            Dictionary<EntitySetBase, ExplicitDiscriminatorMap> dictionary = null;
            if (discriminatorMaps != null)
            {
                dictionary = new Dictionary<EntitySetBase, ExplicitDiscriminatorMap>(discriminatorMaps.Count, discriminatorMaps.Comparer);
                foreach (KeyValuePair<EntitySetBase, DiscriminatorMapInfo> pair in discriminatorMaps)
                {
                    EntitySetBase key = pair.Key;
                    ExplicitDiscriminatorMap discriminatorMap = pair.Value.DiscriminatorMap;
                    if (discriminatorMap != null)
                    {
                        EntityTypeBase rootType = GetRootType(key.ElementType);
                        if (this.GetEntitySet(rootType) != null)
                        {
                            dictionary.Add(key, discriminatorMap);
                        }
                    }
                }
                if (dictionary.Count == 0)
                {
                    dictionary = null;
                }
            }
            this.m_discriminatorMaps = dictionary;
        }

        private void ProcessEntitySets(List<EntitySet> referencedEntitySets, List<EntityType> freeFloatingEntityConstructorTypes)
        {
            this.AssignEntitySetIds(referencedEntitySets);
            this.m_entityTypeToEntitySetMap = new Dictionary<EntityTypeBase, EntitySet>();
            foreach (EntitySet set in referencedEntitySets)
            {
                this.AddEntityTypeToSetEntry(set.ElementType, set);
            }
            foreach (EntityType type in freeFloatingEntityConstructorTypes)
            {
                this.AddEntityTypeToSetEntry(type, null);
            }
        }

        private void ProcessTypes(List<TypeUsage> referencedTypes)
        {
            this.PopulateTypeInfoMap(referencedTypes);
            this.AssignTypeIds();
            this.ExplodeTypes();
        }

        private bool TryGetDiscriminatorMap(EdmType type, out ExplicitDiscriminatorMap discriminatorMap)
        {
            EntitySet set;
            discriminatorMap = null;
            if (this.m_discriminatorMaps == null)
            {
                return false;
            }
            if (type.BuiltInTypeKind != BuiltInTypeKind.EntityType)
            {
                return false;
            }
            EntityTypeBase rootType = GetRootType((EntityType) type);
            if (!this.m_entityTypeToEntitySetMap.TryGetValue(rootType, out set))
            {
                return false;
            }
            if (set == null)
            {
                return false;
            }
            return this.m_discriminatorMaps.TryGetValue(set, out discriminatorMap);
        }

        internal EntitySet[] EntitySetIdToEntitySetMap =>
            this.m_entitySetIdToEntitySetMap;

        internal System.Data.Query.InternalTrees.RelPropertyHelper RelPropertyHelper =>
            this.m_relPropertyHelper;
    }
}

