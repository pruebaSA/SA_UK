namespace System.Data.Mapping
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Entity;
    using System.Data.Metadata.Edm;
    using System.Runtime.InteropServices;

    internal class DefaultObjectMappingItemCollection : MappingItemCollection
    {
        private Dictionary<string, int> cdmTypeIndexes;
        private Dictionary<string, int> clrTypeIndexes;
        private EdmItemCollection m_edmCollection;
        private ObjectItemCollection m_objectCollection;

        public DefaultObjectMappingItemCollection(EdmItemCollection edmCollection, ObjectItemCollection objectCollection) : base(DataSpace.OCSpace)
        {
            this.clrTypeIndexes = new Dictionary<string, int>(StringComparer.Ordinal);
            this.cdmTypeIndexes = new Dictionary<string, int>(StringComparer.Ordinal);
            EntityUtil.CheckArgumentNull<EdmItemCollection>(edmCollection, "edmCollection");
            EntityUtil.CheckArgumentNull<ObjectItemCollection>(objectCollection, "objectCollection");
            this.m_edmCollection = edmCollection;
            this.m_objectCollection = objectCollection;
            this.LoadPrimitiveMaps();
        }

        private void AddInternalMapping(ObjectTypeMapping objectMap)
        {
            string identity = objectMap.ClrType.Identity;
            string key = objectMap.EdmType.Identity;
            int count = base.Count;
            if (this.clrTypeIndexes.ContainsKey(identity))
            {
                if (((BuiltInTypeKind.PrimitiveType != objectMap.ClrType.BuiltInTypeKind) && (BuiltInTypeKind.RowType != objectMap.ClrType.BuiltInTypeKind)) && (BuiltInTypeKind.CollectionType != objectMap.ClrType.BuiltInTypeKind))
                {
                    throw new MappingException(Strings.Mapping_Duplicate_Type_1(identity));
                }
            }
            else
            {
                this.clrTypeIndexes.Add(identity, count);
            }
            if (this.cdmTypeIndexes.ContainsKey(key))
            {
                if (((BuiltInTypeKind.PrimitiveType != objectMap.EdmType.BuiltInTypeKind) && (BuiltInTypeKind.RowType != objectMap.EdmType.BuiltInTypeKind)) && (BuiltInTypeKind.CollectionType != objectMap.EdmType.BuiltInTypeKind))
                {
                    throw new MappingException(Strings.Mapping_Duplicate_Type_1(identity));
                }
            }
            else
            {
                this.cdmTypeIndexes.Add(key, count);
            }
            objectMap.DataSpace = DataSpace.OCSpace;
            base.AddInternal(objectMap);
        }

        private bool ContainsMap(GlobalItem cspaceItem, out ObjectTypeMapping map)
        {
            int num;
            if (this.cdmTypeIndexes.TryGetValue(cspaceItem.Identity, out num))
            {
                map = (ObjectTypeMapping) base[num];
                return true;
            }
            map = null;
            return false;
        }

        private EdmType ConvertCSpaceToOSpaceType(EdmType cdmType)
        {
            if (Helper.IsCollectionType(cdmType))
            {
                return new CollectionType(this.ConvertCSpaceToOSpaceType(((CollectionType) cdmType).TypeUsage.EdmType));
            }
            if (Helper.IsRowType(cdmType))
            {
                List<EdmProperty> properties = new List<EdmProperty>();
                foreach (EdmProperty property in ((RowType) cdmType).Properties)
                {
                    EdmType edmType = this.ConvertCSpaceToOSpaceType(property.TypeUsage.EdmType);
                    EdmProperty item = new EdmProperty(property.Name, TypeUsage.Create(edmType));
                    properties.Add(item);
                }
                return new RowType(properties, ((RowType) cdmType).InitializerMetadata);
            }
            if (Helper.IsRefType(cdmType))
            {
                return new RefType((EntityType) this.ConvertCSpaceToOSpaceType(((RefType) cdmType).ElementType));
            }
            if (Helper.IsPrimitiveType(cdmType))
            {
                return this.m_objectCollection.GetMappedPrimitiveType(((PrimitiveType) cdmType).PrimitiveTypeKind);
            }
            return ((ObjectTypeMapping) this.GetMap(cdmType)).ClrType;
        }

        private EdmType ConvertOSpaceToCSpaceType(EdmType clrType)
        {
            if (Helper.IsCollectionType(clrType))
            {
                return new CollectionType(this.ConvertOSpaceToCSpaceType(((CollectionType) clrType).TypeUsage.EdmType));
            }
            if (Helper.IsRowType(clrType))
            {
                List<EdmProperty> properties = new List<EdmProperty>();
                foreach (EdmProperty property in ((RowType) clrType).Properties)
                {
                    EdmType edmType = this.ConvertOSpaceToCSpaceType(property.TypeUsage.EdmType);
                    EdmProperty item = new EdmProperty(property.Name, TypeUsage.Create(edmType));
                    properties.Add(item);
                }
                return new RowType(properties, ((RowType) clrType).InitializerMetadata);
            }
            if (Helper.IsRefType(clrType))
            {
                return new RefType((EntityType) this.ConvertOSpaceToCSpaceType(((RefType) clrType).ElementType));
            }
            return ((ObjectTypeMapping) this.GetMap(clrType)).EdmType;
        }

        private Map GetDefaultMapping(EdmType cdmType, EdmType clrType) => 
            LoadObjectMapping(cdmType, clrType, this);

        internal override Map GetMap(GlobalItem item)
        {
            Map map;
            EntityUtil.CheckArgumentNull<GlobalItem>(item, "item");
            if (!this.TryGetMap(item, out map))
            {
                throw new InvalidOperationException(Strings.Mapping_Object_InvalidType(item.Identity));
            }
            return map;
        }

        internal override Map GetMap(string identity, DataSpace typeSpace) => 
            this.GetMap(identity, typeSpace, false);

        internal override Map GetMap(string identity, DataSpace typeSpace, bool ignoreCase)
        {
            Map map;
            if (!this.TryGetMap(identity, typeSpace, ignoreCase, out map))
            {
                throw new InvalidOperationException(Strings.Mapping_Object_InvalidType(identity));
            }
            return map;
        }

        private static EdmMember GetObjectMember(EdmMember edmMember, StructuralType objectType)
        {
            EdmMember member;
            EdmType elementType;
            EdmType edmType;
            if (!objectType.Members.TryGetValue(edmMember.Name, false, out member))
            {
                throw new MappingException(Strings.Mapping_Default_OCMapping_Clr_Member_3(edmMember.Name, edmMember.DeclaringType.FullName, objectType.FullName));
            }
            if (edmMember.BuiltInTypeKind != member.BuiltInTypeKind)
            {
                throw new MappingException(Strings.Mapping_Default_OCMapping_MemberKind_Mismatch_6(edmMember.Name, edmMember.DeclaringType.FullName, edmMember.BuiltInTypeKind, member.Name, objectType.FullName, member.BuiltInTypeKind));
            }
            if (edmMember.TypeUsage.EdmType.BuiltInTypeKind != member.TypeUsage.EdmType.BuiltInTypeKind)
            {
                throw new MappingException(EntityRes.GetString("Mapping_Default_OCMapping_Member_Type_Mismatch", new object[] { edmMember.TypeUsage.EdmType.Name, edmMember.TypeUsage.EdmType.BuiltInTypeKind, edmMember.Name, edmMember.DeclaringType.FullName, member.TypeUsage.EdmType.Name, member.TypeUsage.EdmType.BuiltInTypeKind, member.Name, member.DeclaringType.FullName }));
            }
            if (edmMember.TypeUsage.EdmType.BuiltInTypeKind == BuiltInTypeKind.PrimitiveType)
            {
                if (((PrimitiveType) edmMember.TypeUsage.EdmType).PrimitiveTypeKind != ((PrimitiveType) member.TypeUsage.EdmType).PrimitiveTypeKind)
                {
                    throw new MappingException(Strings.Mapping_Default_OCMapping_Invalid_MemberType_6(edmMember.TypeUsage.EdmType.FullName, edmMember.Name, edmMember.DeclaringType.FullName, member.TypeUsage.EdmType.FullName, member.Name, member.DeclaringType.FullName));
                }
                return member;
            }
            if (edmMember.BuiltInTypeKind == BuiltInTypeKind.AssociationEndMember)
            {
                elementType = ((RefType) edmMember.TypeUsage.EdmType).ElementType;
                edmType = ((RefType) member.TypeUsage.EdmType).ElementType;
            }
            else if ((BuiltInTypeKind.NavigationProperty == edmMember.BuiltInTypeKind) && (BuiltInTypeKind.CollectionType == edmMember.TypeUsage.EdmType.BuiltInTypeKind))
            {
                elementType = ((CollectionType) edmMember.TypeUsage.EdmType).TypeUsage.EdmType;
                edmType = ((CollectionType) member.TypeUsage.EdmType).TypeUsage.EdmType;
            }
            else
            {
                elementType = edmMember.TypeUsage.EdmType;
                edmType = member.TypeUsage.EdmType;
            }
            if (elementType.Identity != ObjectItemCollection.TryGetMappingCSpaceTypeIdentity(edmType))
            {
                throw new MappingException(Strings.Mapping_Default_OCMapping_Invalid_MemberType_6(edmMember.TypeUsage.EdmType.FullName, edmMember.Name, edmMember.DeclaringType.FullName, member.TypeUsage.EdmType.FullName, member.Name, member.DeclaringType.FullName));
            }
            return member;
        }

        private Map GetOCMapForTransientType(EdmType edmType, DataSpace typeSpace)
        {
            EdmType clrType = null;
            EdmType cdmType = null;
            int num = -1;
            if (typeSpace != DataSpace.OSpace)
            {
                if (this.cdmTypeIndexes.TryGetValue(edmType.Identity, out num))
                {
                    return (Map) base[num];
                }
                cdmType = edmType;
                clrType = this.ConvertCSpaceToOSpaceType(edmType);
            }
            else if (typeSpace == DataSpace.OSpace)
            {
                if (this.clrTypeIndexes.TryGetValue(edmType.Identity, out num))
                {
                    return (Map) base[num];
                }
                clrType = edmType;
                cdmType = this.ConvertOSpaceToCSpaceType(clrType);
            }
            ObjectTypeMapping objectMap = new ObjectTypeMapping(clrType, cdmType);
            if (BuiltInTypeKind.RowType == edmType.BuiltInTypeKind)
            {
                RowType type3 = (RowType) clrType;
                RowType type4 = (RowType) cdmType;
                for (int i = 0; i < type3.Properties.Count; i++)
                {
                    objectMap.AddMemberMap(new ObjectPropertyMapping(type4.Properties[i], type3.Properties[i]));
                }
            }
            if (!this.cdmTypeIndexes.ContainsKey(cdmType.Identity) && !this.clrTypeIndexes.ContainsKey(clrType.Identity))
            {
                this.AddInternalMapping(objectMap);
            }
            return objectMap;
        }

        private static void LoadAssociationTypeMapping(ObjectTypeMapping objectMapping, EdmType edmType, EdmType objectType, DefaultObjectMappingItemCollection ocItemCollection, Dictionary<string, ObjectTypeMapping> typeMappings)
        {
            AssociationType type = (AssociationType) edmType;
            AssociationType type2 = (AssociationType) objectType;
            foreach (AssociationEndMember member in type.AssociationEndMembers)
            {
                AssociationEndMember objectMember = (AssociationEndMember) GetObjectMember(member, type2);
                if (member.RelationshipMultiplicity != objectMember.RelationshipMultiplicity)
                {
                    throw new MappingException(Strings.Mapping_Default_OCMapping_MultiplicityMismatch_6(member.RelationshipMultiplicity, member.Name, type.FullName, objectMember.RelationshipMultiplicity, objectMember.Name, type2.FullName));
                }
                LoadTypeMapping(((RefType) member.TypeUsage.EdmType).ElementType, ((RefType) objectMember.TypeUsage.EdmType).ElementType, ocItemCollection, typeMappings);
                objectMapping.AddMemberMap(new ObjectAssociationEndMapping(member, objectMember));
            }
        }

        private static ObjectComplexPropertyMapping LoadComplexMemberMapping(EdmProperty containingEdmMember, EdmProperty containingClrMember, DefaultObjectMappingItemCollection ocItemCollection, Dictionary<string, ObjectTypeMapping> typeMappings)
        {
            ComplexType edmType = (ComplexType) containingEdmMember.TypeUsage.EdmType;
            ComplexType objectType = (ComplexType) containingClrMember.TypeUsage.EdmType;
            return new ObjectComplexPropertyMapping(containingEdmMember, containingClrMember, LoadTypeMapping(edmType, objectType, ocItemCollection, typeMappings));
        }

        private static void LoadEntityTypeOrComplexTypeMapping(ObjectTypeMapping objectMapping, EdmType edmType, EdmType objectType, DefaultObjectMappingItemCollection ocItemCollection, Dictionary<string, ObjectTypeMapping> typeMappings)
        {
            StructuralType type = (StructuralType) edmType;
            StructuralType type2 = (StructuralType) objectType;
            if (type.Members.Count != type2.Members.Count)
            {
                throw new MappingException(Strings.Mapping_Default_OCMapping_Member_Count_Mismatch_2(edmType.FullName, objectType.FullName));
            }
            foreach (EdmMember member in type.Members)
            {
                EdmMember objectMember = GetObjectMember(member, type2);
                if (Helper.IsEdmProperty(member))
                {
                    EdmProperty containingEdmMember = (EdmProperty) member;
                    EdmProperty containingClrMember = (EdmProperty) objectMember;
                    if (Helper.IsComplexType(member.TypeUsage.EdmType))
                    {
                        objectMapping.AddMemberMap(LoadComplexMemberMapping(containingEdmMember, containingClrMember, ocItemCollection, typeMappings));
                    }
                    else
                    {
                        objectMapping.AddMemberMap(LoadScalarPropertyMapping(containingEdmMember, containingClrMember));
                    }
                }
                else
                {
                    NavigationProperty edmNavigationProperty = (NavigationProperty) member;
                    NavigationProperty clrNavigationProperty = (NavigationProperty) objectMember;
                    LoadTypeMapping(edmNavigationProperty.RelationshipType, clrNavigationProperty.RelationshipType, ocItemCollection, typeMappings);
                    objectMapping.AddMemberMap(new ObjectNavigationPropertyMapping(edmNavigationProperty, clrNavigationProperty));
                }
            }
        }

        internal static ObjectTypeMapping LoadObjectMapping(EdmType cdmType, EdmType objectType, DefaultObjectMappingItemCollection ocItemCollection)
        {
            Dictionary<string, ObjectTypeMapping> typeMappings = new Dictionary<string, ObjectTypeMapping>(StringComparer.Ordinal);
            ObjectTypeMapping mapping = LoadObjectMapping(cdmType, objectType, ocItemCollection, typeMappings);
            if (ocItemCollection != null)
            {
                foreach (ObjectTypeMapping mapping2 in typeMappings.Values)
                {
                    ocItemCollection.AddInternalMapping(mapping2);
                }
            }
            return mapping;
        }

        private static ObjectTypeMapping LoadObjectMapping(EdmType edmType, EdmType objectType, DefaultObjectMappingItemCollection ocItemCollection, Dictionary<string, ObjectTypeMapping> typeMappings)
        {
            if (edmType.Abstract != objectType.Abstract)
            {
                throw new MappingException(Strings.Mapping_AbstractTypeMappingToNonAbstractType(edmType.FullName, objectType.FullName));
            }
            ObjectTypeMapping mapping = new ObjectTypeMapping(objectType, edmType);
            typeMappings.Add(edmType.FullName, mapping);
            if ((edmType.BuiltInTypeKind == BuiltInTypeKind.EntityType) || (edmType.BuiltInTypeKind == BuiltInTypeKind.ComplexType))
            {
                LoadEntityTypeOrComplexTypeMapping(mapping, edmType, objectType, ocItemCollection, typeMappings);
                return mapping;
            }
            LoadAssociationTypeMapping(mapping, edmType, objectType, ocItemCollection, typeMappings);
            return mapping;
        }

        private void LoadPrimitiveMaps()
        {
            foreach (PrimitiveType type in this.m_edmCollection.GetPrimitiveTypes())
            {
                PrimitiveType mappedPrimitiveType = this.m_objectCollection.GetMappedPrimitiveType(type.PrimitiveTypeKind);
                this.AddInternalMapping(new ObjectTypeMapping(mappedPrimitiveType, type));
            }
        }

        private static ObjectPropertyMapping LoadScalarPropertyMapping(EdmProperty edmProperty, EdmProperty objectProperty) => 
            new ObjectPropertyMapping(edmProperty, objectProperty);

        private static ObjectTypeMapping LoadTypeMapping(EdmType edmType, EdmType objectType, DefaultObjectMappingItemCollection ocItemCollection, Dictionary<string, ObjectTypeMapping> typeMappings)
        {
            ObjectTypeMapping mapping;
            ObjectTypeMapping mapping2;
            if (typeMappings.TryGetValue(edmType.FullName, out mapping))
            {
                return mapping;
            }
            if ((ocItemCollection != null) && ocItemCollection.ContainsMap(edmType, out mapping2))
            {
                return mapping2;
            }
            return LoadObjectMapping(edmType, objectType, ocItemCollection, typeMappings);
        }

        internal override bool TryGetMap(GlobalItem item, out Map map)
        {
            if (item == null)
            {
                map = null;
                return false;
            }
            DataSpace dataSpace = item.DataSpace;
            EdmType edmType = item as EdmType;
            if ((edmType == null) || !Helper.IsTransientType(edmType))
            {
                return this.TryGetMap(item.Identity, dataSpace, out map);
            }
            map = this.GetOCMapForTransientType(edmType, dataSpace);
            return (map != null);
        }

        internal override bool TryGetMap(string identity, DataSpace typeSpace, out Map map) => 
            this.TryGetMap(identity, typeSpace, false, out map);

        internal override bool TryGetMap(string identity, DataSpace typeSpace, bool ignoreCase, out Map map)
        {
            EdmType item = null;
            EdmType edmType = null;
            if (typeSpace == DataSpace.CSpace)
            {
                int num;
                if (ignoreCase)
                {
                    if (!this.m_edmCollection.TryGetItem<EdmType>(identity, true, out item))
                    {
                        map = null;
                        return false;
                    }
                    identity = item.Identity;
                }
                if (this.cdmTypeIndexes.TryGetValue(identity, out num))
                {
                    map = (Map) base[num];
                    return true;
                }
                if ((item != null) || this.m_edmCollection.TryGetItem<EdmType>(identity, ignoreCase, out item))
                {
                    this.m_objectCollection.TryGetOSpaceType(item, out edmType);
                }
            }
            else if (typeSpace == DataSpace.OSpace)
            {
                int num2;
                if (ignoreCase)
                {
                    if (!this.m_objectCollection.TryGetItem<EdmType>(identity, true, out edmType))
                    {
                        map = null;
                        return false;
                    }
                    identity = edmType.Identity;
                }
                if (this.clrTypeIndexes.TryGetValue(identity, out num2))
                {
                    map = (Map) base[num2];
                    return true;
                }
                if ((edmType != null) || this.m_objectCollection.TryGetItem<EdmType>(identity, ignoreCase, out edmType))
                {
                    string str = ObjectItemCollection.TryGetMappingCSpaceTypeIdentity(edmType);
                    this.m_edmCollection.TryGetItem<EdmType>(str, out item);
                }
            }
            if ((edmType == null) || (item == null))
            {
                map = null;
                return false;
            }
            map = this.GetDefaultMapping(item, edmType);
            return true;
        }
    }
}

