namespace System.Data.Query.PlanCompiler
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data.Common;
    using System.Data.Metadata.Edm;
    using System.Data.Query.InternalTrees;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Threading;

    internal class TypeInfo
    {
        private List<TypeInfo> m_immediateSubTypes;
        private readonly RootTypeInfo m_rootType;
        private readonly TypeInfo m_superType;
        private readonly TypeUsage m_type;
        private object m_typeId;

        protected TypeInfo(TypeUsage type, TypeInfo superType)
        {
            this.m_type = type;
            this.m_immediateSubTypes = new List<TypeInfo>();
            this.m_superType = superType;
            if (superType != null)
            {
                superType.m_immediateSubTypes.Add(this);
                this.m_rootType = superType.RootType;
            }
        }

        internal static TypeInfo Create(TypeUsage type, TypeInfo superTypeInfo, ExplicitDiscriminatorMap discriminatorMap)
        {
            if (superTypeInfo == null)
            {
                return new RootTypeInfo(type, discriminatorMap);
            }
            return new TypeInfo(type, superTypeInfo);
        }

        internal IEnumerable<EdmProperty> GetAllProperties()
        {
            foreach (EdmProperty iteratorVariable0 in this.FlattenedType.Properties)
            {
                yield return iteratorVariable0;
            }
        }

        internal IEnumerable<PropertyRef> GetAllPropertyRefs()
        {
            foreach (PropertyRef iteratorVariable0 in this.PropertyRefList)
            {
                yield return iteratorVariable0;
            }
        }

        internal IEnumerable<PropertyRef> GetIdentityPropertyRefs()
        {
            if (this.HasEntitySetIdProperty)
            {
                yield return EntitySetIdPropertyRef.Instance;
            }
            IEnumerator<PropertyRef> enumerator = this.GetKeyPropertyRefs().GetEnumerator();
            while (enumerator.MoveNext())
            {
                PropertyRef current = enumerator.Current;
                yield return current;
            }
        }

        internal IEnumerable<PropertyRef> GetKeyPropertyRefs()
        {
            EntityTypeBase elementType = null;
            RefType type = null;
            if (TypeHelpers.TryGetEdmType<RefType>(this.m_type, out type))
            {
                elementType = type.ElementType;
            }
            else
            {
                elementType = TypeHelpers.GetEdmType<EntityTypeBase>(this.m_type);
            }
            ReadOnlyMetadataCollection<EdmMember>.Enumerator enumerator = elementType.KeyMembers.GetEnumerator();
            while (enumerator.MoveNext())
            {
                EdmMember current = enumerator.Current;
                System.Data.Query.PlanCompiler.PlanCompiler.Assert(current is EdmProperty, "Non-EdmProperty key members are not supported");
                SimplePropertyRef iteratorVariable3 = new SimplePropertyRef(current);
                yield return iteratorVariable3;
            }
        }

        internal EdmProperty GetNewProperty(PropertyRef propertyRef) => 
            this.RootType.GetNewProperty(propertyRef);

        internal List<TypeInfo> GetTypeHierarchy()
        {
            List<TypeInfo> result = new List<TypeInfo>();
            this.GetTypeHierarchy(result);
            return result;
        }

        private void GetTypeHierarchy(List<TypeInfo> result)
        {
            result.Add(this);
            foreach (TypeInfo info in this.ImmediateSubTypes)
            {
                info.GetTypeHierarchy(result);
            }
        }

        internal virtual EdmProperty EntitySetIdProperty =>
            this.RootType.EntitySetIdProperty;

        internal virtual RowType FlattenedType =>
            this.RootType.FlattenedType;

        internal virtual TypeUsage FlattenedTypeUsage =>
            this.RootType.FlattenedTypeUsage;

        internal bool HasEntitySetIdProperty =>
            (this.RootType.EntitySetIdProperty != null);

        internal bool HasNullSentinelProperty =>
            (this.RootType.NullSentinelProperty != null);

        internal bool HasTypeIdProperty =>
            (this.RootType.TypeIdProperty != null);

        internal List<TypeInfo> ImmediateSubTypes =>
            this.m_immediateSubTypes;

        internal bool IsRootType =>
            (this.m_rootType == null);

        internal virtual EdmProperty NullSentinelProperty =>
            this.RootType.NullSentinelProperty;

        internal virtual IEnumerable<PropertyRef> PropertyRefList =>
            this.RootType.PropertyRefList;

        internal RootTypeInfo RootType =>
            (this.m_rootType ?? ((RootTypeInfo) this));

        internal TypeInfo SuperType =>
            this.m_superType;

        internal TypeUsage Type =>
            this.m_type;

        internal object TypeId
        {
            get => 
                this.m_typeId;
            set
            {
                this.m_typeId = value;
            }
        }

        internal virtual EdmProperty TypeIdProperty =>
            this.RootType.TypeIdProperty;




    }
}

