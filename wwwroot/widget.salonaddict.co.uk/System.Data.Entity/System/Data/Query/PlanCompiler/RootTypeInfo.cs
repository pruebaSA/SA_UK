namespace System.Data.Query.PlanCompiler
{
    using System;
    using System.Collections.Generic;
    using System.Data.Metadata.Edm;
    using System.Data.Query.InternalTrees;

    internal class RootTypeInfo : TypeInfo
    {
        private readonly ExplicitDiscriminatorMap m_discriminatorMap;
        private EdmProperty m_entitySetIdProperty;
        private RowType m_flattenedType;
        private TypeUsage m_flattenedTypeUsage;
        private EdmProperty m_nullSentinelProperty;
        private readonly Dictionary<PropertyRef, EdmProperty> m_propertyMap;
        private readonly List<PropertyRef> m_propertyRefList;
        private System.Data.Query.PlanCompiler.TypeIdKind m_typeIdKind;
        private EdmProperty m_typeIdProperty;
        private TypeUsage m_typeIdType;

        internal RootTypeInfo(TypeUsage type, ExplicitDiscriminatorMap discriminatorMap) : base(type, null)
        {
            System.Data.Query.PlanCompiler.PlanCompiler.Assert(type.EdmType.BaseType == null, "only root types allowed here");
            this.m_propertyMap = new Dictionary<PropertyRef, EdmProperty>();
            this.m_propertyRefList = new List<PropertyRef>();
            this.m_discriminatorMap = discriminatorMap;
            this.m_typeIdKind = System.Data.Query.PlanCompiler.TypeIdKind.Generated;
        }

        internal void AddPropertyMapping(PropertyRef propertyRef, EdmProperty newProperty)
        {
            this.m_propertyMap[propertyRef] = newProperty;
            if (propertyRef is TypeIdPropertyRef)
            {
                this.m_typeIdProperty = newProperty;
            }
            else if (propertyRef is EntitySetIdPropertyRef)
            {
                this.m_entitySetIdProperty = newProperty;
            }
            else if (propertyRef is NullSentinelPropertyRef)
            {
                this.m_nullSentinelProperty = newProperty;
            }
        }

        internal void AddPropertyRef(PropertyRef propertyRef)
        {
            this.m_propertyRefList.Add(propertyRef);
        }

        internal int GetNestedStructureOffset(PropertyRef property)
        {
            for (int i = 0; i < this.m_propertyRefList.Count; i++)
            {
                NestedPropertyRef ref2 = this.m_propertyRefList[i] as NestedPropertyRef;
                if ((ref2 != null) && ref2.InnerProperty.Equals(property))
                {
                    return i;
                }
            }
            System.Data.Query.PlanCompiler.PlanCompiler.Assert(false, "no complex structure " + property + " found in TypeInfo");
            return 0;
        }

        internal EdmProperty GetNewProperty(PropertyRef propertyRef)
        {
            EdmProperty property;
            if (this.m_propertyMap.TryGetValue(propertyRef, out property))
            {
                return property;
            }
            System.Data.Query.PlanCompiler.PlanCompiler.Assert(false, "Unable to find property " + propertyRef.ToString() + " in type " + base.Type.EdmType.Identity);
            return null;
        }

        internal ExplicitDiscriminatorMap DiscriminatorMap =>
            this.m_discriminatorMap;

        internal EdmProperty EntitySetIdProperty =>
            this.m_entitySetIdProperty;

        internal RowType FlattenedType
        {
            get => 
                this.m_flattenedType;
            set
            {
                this.m_flattenedType = value;
                this.m_flattenedTypeUsage = TypeUsage.Create(value);
            }
        }

        internal TypeUsage FlattenedTypeUsage =>
            this.m_flattenedTypeUsage;

        internal EdmProperty NullSentinelProperty =>
            this.m_nullSentinelProperty;

        internal IEnumerable<PropertyRef> PropertyRefList =>
            this.m_propertyRefList;

        internal System.Data.Query.PlanCompiler.TypeIdKind TypeIdKind
        {
            get => 
                this.m_typeIdKind;
            set
            {
                this.m_typeIdKind = value;
            }
        }

        internal EdmProperty TypeIdProperty =>
            this.m_typeIdProperty;

        internal TypeUsage TypeIdType
        {
            get => 
                this.m_typeIdType;
            set
            {
                this.m_typeIdType = value;
            }
        }
    }
}

