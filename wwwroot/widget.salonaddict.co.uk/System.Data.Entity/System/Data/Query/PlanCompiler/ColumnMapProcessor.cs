namespace System.Data.Query.PlanCompiler
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Data.Common.Utils;
    using System.Data.Metadata.Edm;
    using System.Data.Query.InternalTrees;

    internal class ColumnMapProcessor
    {
        private const string c_EntitySetIdColumnName = "__EntitySetId";
        private const string c_NullSentinelColumnName = "__NullSentinel";
        private const string c_TypeIdColumnName = "__TypeId";
        private VarRefColumnMap m_columnMap;
        private StructuredTypeInfo m_typeInfo;
        private System.Data.Query.PlanCompiler.VarInfo m_varInfo;
        private IEnumerator<Var> m_varList;

        internal ColumnMapProcessor(VarRefColumnMap columnMap, System.Data.Query.PlanCompiler.VarInfo varInfo, StructuredTypeInfo typeInfo)
        {
            this.m_columnMap = columnMap;
            this.m_varInfo = varInfo;
            System.Data.Query.PlanCompiler.PlanCompiler.Assert((varInfo.NewVars != null) && (varInfo.NewVars.Count > 0), "No new Vars specified");
            this.m_varList = varInfo.NewVars.GetEnumerator();
            this.m_typeInfo = typeInfo;
        }

        private void BuildRelPropertyColumnMaps(TypeInfo typeInfo, bool includeSupertypeRelProperties)
        {
            IEnumerable<RelProperty> relProperties = null;
            if (includeSupertypeRelProperties)
            {
                relProperties = this.m_typeInfo.RelPropertyHelper.GetRelProperties(typeInfo.Type.EdmType as EntityTypeBase);
            }
            else
            {
                relProperties = this.m_typeInfo.RelPropertyHelper.GetDeclaredOnlyRelProperties(typeInfo.Type.EdmType as EntityTypeBase);
            }
            foreach (RelProperty property in relProperties)
            {
                this.CreateColumnMap(property.ToEnd.TypeUsage, property.ToString());
            }
            foreach (TypeInfo info in typeInfo.ImmediateSubTypes)
            {
                this.BuildRelPropertyColumnMaps(info, false);
            }
        }

        private ColumnMap CreateColumnMap(TypeUsage type, string name)
        {
            if (!TypeUtils.IsStructuredType(type))
            {
                return this.CreateSimpleColumnMap(type, name);
            }
            return this.CreateStructuralColumnMap(type, name);
        }

        private ComplexTypeColumnMap CreateComplexTypeColumnMap(TypeInfo typeInfo, string name, ComplexTypeColumnMap superTypeColumnMap, Dictionary<object, TypedColumnMap> discriminatorMap, List<TypedColumnMap> allMaps)
        {
            List<ColumnMap> list = new List<ColumnMap>();
            IEnumerable declaredStructuralMembers = null;
            SimpleColumnMap nullSentinel = null;
            if (typeInfo.HasNullSentinelProperty)
            {
                nullSentinel = this.CreateSimpleColumnMap(Helper.GetModelTypeUsage(typeInfo.NullSentinelProperty), "__NullSentinel");
            }
            if (superTypeColumnMap != null)
            {
                foreach (ColumnMap map2 in superTypeColumnMap.Properties)
                {
                    list.Add(map2);
                }
                declaredStructuralMembers = TypeHelpers.GetDeclaredStructuralMembers(typeInfo.Type);
            }
            else
            {
                declaredStructuralMembers = TypeHelpers.GetAllStructuralMembers(typeInfo.Type);
            }
            foreach (EdmMember member in declaredStructuralMembers)
            {
                ColumnMap map3 = this.CreateColumnMap(Helper.GetModelTypeUsage(member), member.Name);
                list.Add(map3);
            }
            ComplexTypeColumnMap item = new ComplexTypeColumnMap(typeInfo.Type, name, list.ToArray(), nullSentinel);
            if (discriminatorMap != null)
            {
                discriminatorMap[typeInfo.TypeId] = item;
            }
            if (allMaps != null)
            {
                allMaps.Add(item);
            }
            foreach (TypeInfo info in typeInfo.ImmediateSubTypes)
            {
                this.CreateComplexTypeColumnMap(info, name, item, discriminatorMap, allMaps);
            }
            return item;
        }

        private EntityColumnMap CreateEntityColumnMap(TypeInfo typeInfo, string name, EntityColumnMap superTypeColumnMap, Dictionary<object, TypedColumnMap> discriminatorMap, List<TypedColumnMap> allMaps, bool handleRelProperties)
        {
            EntityColumnMap item = null;
            List<ColumnMap> list = new List<ColumnMap>();
            if (superTypeColumnMap != null)
            {
                foreach (ColumnMap map2 in superTypeColumnMap.Properties)
                {
                    list.Add(map2);
                }
                foreach (EdmMember member in TypeHelpers.GetDeclaredStructuralMembers(typeInfo.Type))
                {
                    ColumnMap map3 = this.CreateColumnMap(Helper.GetModelTypeUsage(member), member.Name);
                    list.Add(map3);
                }
                item = new EntityColumnMap(typeInfo.Type, name, list.ToArray(), superTypeColumnMap.EntityIdentity);
            }
            else
            {
                SimpleColumnMap entitySetIdColumnMap = null;
                if (typeInfo.HasEntitySetIdProperty)
                {
                    entitySetIdColumnMap = this.CreateEntitySetIdColumnMap(typeInfo.EntitySetIdProperty);
                }
                List<SimpleColumnMap> list2 = new List<SimpleColumnMap>();
                Dictionary<EdmProperty, ColumnMap> dictionary = new Dictionary<EdmProperty, ColumnMap>();
                foreach (EdmMember member2 in TypeHelpers.GetDeclaredStructuralMembers(typeInfo.Type))
                {
                    ColumnMap map5 = this.CreateColumnMap(Helper.GetModelTypeUsage(member2), member2.Name);
                    list.Add(map5);
                    if (TypeSemantics.IsPartOfKey(member2))
                    {
                        EdmProperty property = member2 as EdmProperty;
                        System.Data.Query.PlanCompiler.PlanCompiler.Assert(property != null, "EntityType key member is not property?");
                        dictionary[property] = map5;
                    }
                }
                foreach (EdmMember member3 in TypeHelpers.GetEdmType<EntityType>(typeInfo.Type).KeyMembers)
                {
                    EdmProperty property2 = member3 as EdmProperty;
                    System.Data.Query.PlanCompiler.PlanCompiler.Assert(property2 != null, "EntityType key member is not property?");
                    SimpleColumnMap map6 = dictionary[property2] as SimpleColumnMap;
                    System.Data.Query.PlanCompiler.PlanCompiler.Assert(map6 != null, "keyColumnMap is null");
                    list2.Add(map6);
                }
                EntityIdentity entityIdentity = this.CreateEntityIdentity((EntityType) typeInfo.Type.EdmType, entitySetIdColumnMap, list2.ToArray());
                item = new EntityColumnMap(typeInfo.Type, name, list.ToArray(), entityIdentity);
            }
            if ((discriminatorMap != null) && (typeInfo.TypeId != null))
            {
                discriminatorMap[typeInfo.TypeId] = item;
            }
            if (allMaps != null)
            {
                allMaps.Add(item);
            }
            foreach (TypeInfo info in typeInfo.ImmediateSubTypes)
            {
                this.CreateEntityColumnMap(info, name, item, discriminatorMap, allMaps, false);
            }
            if (handleRelProperties)
            {
                this.BuildRelPropertyColumnMaps(typeInfo, true);
            }
            return item;
        }

        private EntityIdentity CreateEntityIdentity(EntityType entityType, SimpleColumnMap entitySetIdColumnMap, SimpleColumnMap[] keyColumnMaps)
        {
            if (entitySetIdColumnMap != null)
            {
                return new DiscriminatedEntityIdentity(entitySetIdColumnMap, this.m_typeInfo.EntitySetIdToEntitySetMap, keyColumnMaps);
            }
            EntitySet entitySet = this.m_typeInfo.GetEntitySet(entityType);
            System.Data.Query.PlanCompiler.PlanCompiler.Assert(entitySet != null, "Expected non-null entityset when no entitysetid is required. Entity type = " + entityType);
            return new SimpleEntityIdentity(entitySet, keyColumnMaps);
        }

        private SimpleColumnMap CreateEntitySetIdColumnMap(EdmProperty prop) => 
            this.CreateSimpleColumnMap(Helper.GetModelTypeUsage(prop), "__EntitySetId");

        private SimplePolymorphicColumnMap CreatePolymorphicColumnMap(TypeInfo typeInfo, string name)
        {
            Dictionary<object, TypedColumnMap> discriminatorMap = new Dictionary<object, TypedColumnMap>((typeInfo.RootType.DiscriminatorMap == null) ? null : TrailingSpaceComparer.Instance);
            List<TypedColumnMap> allMaps = new List<TypedColumnMap>();
            TypeInfo rootType = typeInfo.RootType;
            SimpleColumnMap typeDiscriminator = this.CreateTypeIdColumnMap(rootType.TypeIdProperty);
            if (TypeSemantics.IsComplexType(typeInfo.Type))
            {
                this.CreateComplexTypeColumnMap(rootType, name, null, discriminatorMap, allMaps);
            }
            else
            {
                this.CreateEntityColumnMap(rootType, name, null, discriminatorMap, allMaps, true);
            }
            TypedColumnMap map2 = null;
            foreach (TypedColumnMap map3 in allMaps)
            {
                if (TypeSemantics.IsEquivalent(map3.Type, typeInfo.Type))
                {
                    map2 = map3;
                    break;
                }
            }
            System.Data.Query.PlanCompiler.PlanCompiler.Assert(null != map2, "Didn't find requested type in polymorphic type hierarchy?");
            return new SimplePolymorphicColumnMap(typeInfo.Type, name, map2.Properties, typeDiscriminator, discriminatorMap);
        }

        private RecordColumnMap CreateRecordColumnMap(TypeInfo typeInfo, string name)
        {
            System.Data.Query.PlanCompiler.PlanCompiler.Assert(typeInfo.Type.EdmType is RowType, "not RowType");
            SimpleColumnMap nullSentinel = null;
            if (typeInfo.HasNullSentinelProperty)
            {
                nullSentinel = this.CreateSimpleColumnMap(Helper.GetModelTypeUsage(typeInfo.NullSentinelProperty), "__NullSentinel");
            }
            ReadOnlyMetadataCollection<EdmProperty> properties = TypeHelpers.GetProperties(typeInfo.Type);
            ColumnMap[] mapArray = new ColumnMap[properties.Count];
            for (int i = 0; i < mapArray.Length; i++)
            {
                EdmMember member = properties[i];
                mapArray[i] = this.CreateColumnMap(Helper.GetModelTypeUsage(member), member.Name);
            }
            return new RecordColumnMap(typeInfo.Type, name, mapArray, nullSentinel);
        }

        private RefColumnMap CreateRefColumnMap(TypeInfo typeInfo, string name)
        {
            SimpleColumnMap entitySetIdColumnMap = null;
            if (typeInfo.HasEntitySetIdProperty)
            {
                entitySetIdColumnMap = this.CreateSimpleColumnMap(Helper.GetModelTypeUsage(typeInfo.EntitySetIdProperty), "__EntitySetId");
            }
            EntityType elementType = (EntityType) TypeHelpers.GetEdmType<RefType>(typeInfo.Type).ElementType;
            SimpleColumnMap[] keyColumnMaps = new SimpleColumnMap[elementType.KeyMembers.Count];
            for (int i = 0; i < keyColumnMaps.Length; i++)
            {
                EdmMember member = elementType.KeyMembers[i];
                keyColumnMaps[i] = this.CreateSimpleColumnMap(Helper.GetModelTypeUsage(member), member.Name);
            }
            return new RefColumnMap(typeInfo.Type, name, this.CreateEntityIdentity(elementType, entitySetIdColumnMap, keyColumnMaps));
        }

        private SimpleColumnMap CreateSimpleColumnMap(TypeUsage type, string name) => 
            new VarRefColumnMap(type, name, this.GetNextVar());

        private ColumnMap CreateStructuralColumnMap(TypeUsage type, string name)
        {
            TypeInfo typeInfo = this.m_typeInfo.GetTypeInfo(type);
            if (TypeSemantics.IsRowType(type))
            {
                return this.CreateRecordColumnMap(typeInfo, name);
            }
            if (TypeSemantics.IsReferenceType(type))
            {
                return this.CreateRefColumnMap(typeInfo, name);
            }
            if (typeInfo.HasTypeIdProperty)
            {
                return this.CreatePolymorphicColumnMap(typeInfo, name);
            }
            if (TypeSemantics.IsComplexType(type))
            {
                return this.CreateComplexTypeColumnMap(typeInfo, name, null, null, null);
            }
            if (!TypeSemantics.IsEntityType(type))
            {
                throw EntityUtil.NotSupported(type.GetType().ToString());
            }
            return this.CreateEntityColumnMap(typeInfo, name, null, null, null, true);
        }

        private SimpleColumnMap CreateTypeIdColumnMap(EdmProperty prop) => 
            this.CreateSimpleColumnMap(Helper.GetModelTypeUsage(prop), "__TypeId");

        internal ColumnMap ExpandColumnMap()
        {
            CollectionVarInfo varInfo = this.m_varInfo as CollectionVarInfo;
            if (varInfo != null)
            {
                return new VarRefColumnMap(this.m_columnMap.Var.Type, this.m_columnMap.Name, varInfo.NewVar);
            }
            return this.CreateColumnMap(this.m_columnMap.Var.Type, this.m_columnMap.Name);
        }

        private Var GetNextVar()
        {
            if (this.m_varList.MoveNext())
            {
                return this.m_varList.Current;
            }
            System.Data.Query.PlanCompiler.PlanCompiler.Assert(false, "Could not GetNextVar");
            return null;
        }
    }
}

