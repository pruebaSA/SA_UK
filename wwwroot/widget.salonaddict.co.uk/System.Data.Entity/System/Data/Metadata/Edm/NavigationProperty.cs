namespace System.Data.Metadata.Edm
{
    using System;
    using System.Data;
    using System.Reflection;
    using System.Threading;

    public sealed class NavigationProperty : EdmMember
    {
        private RelationshipEndMember _fromEndMember;
        private Func<object, object> _memberGetter;
        private System.Data.Metadata.Edm.RelationshipType _relationshipType;
        private RelationshipEndMember _toEndMember;
        internal readonly RuntimeMethodHandle PropertyGetterHandle;
        internal const string RelationshipTypeNamePropertyName = "RelationshipType";
        internal const string ToEndMemberNamePropertyName = "ToEndMember";

        internal NavigationProperty(string name, TypeUsage typeUsage) : base(name, typeUsage)
        {
            EntityUtil.CheckStringArgument(name, "name");
            EntityUtil.GenericCheckArgumentNull<TypeUsage>(typeUsage, "typeUsage");
        }

        internal NavigationProperty(string name, TypeUsage typeUsage, PropertyInfo propertyInfo) : this(name, typeUsage)
        {
            if (propertyInfo != null)
            {
                MethodInfo getMethod = propertyInfo.GetGetMethod();
                this.PropertyGetterHandle = (getMethod != null) ? getMethod.MethodHandle : new RuntimeMethodHandle();
            }
        }

        public override System.Data.Metadata.Edm.BuiltInTypeKind BuiltInTypeKind =>
            System.Data.Metadata.Edm.BuiltInTypeKind.NavigationProperty;

        [MetadataProperty(System.Data.Metadata.Edm.BuiltInTypeKind.RelationshipEndMember, false)]
        public RelationshipEndMember FromEndMember
        {
            get => 
                this._fromEndMember;
            internal set
            {
                this._fromEndMember = value;
            }
        }

        [MetadataProperty(System.Data.Metadata.Edm.BuiltInTypeKind.RelationshipType, false)]
        public System.Data.Metadata.Edm.RelationshipType RelationshipType
        {
            get => 
                this._relationshipType;
            internal set
            {
                this._relationshipType = value;
            }
        }

        [MetadataProperty(System.Data.Metadata.Edm.BuiltInTypeKind.RelationshipEndMember, false)]
        public RelationshipEndMember ToEndMember
        {
            get => 
                this._toEndMember;
            internal set
            {
                this._toEndMember = value;
            }
        }

        internal Func<object, object> ValueGetter
        {
            get => 
                this._memberGetter;
            set
            {
                Interlocked.CompareExchange<Func<object, object>>(ref this._memberGetter, value, null);
            }
        }
    }
}

