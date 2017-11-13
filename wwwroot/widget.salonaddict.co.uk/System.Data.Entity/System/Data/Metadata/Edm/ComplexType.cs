namespace System.Data.Metadata.Edm
{
    using System;
    using System.Data;
    using System.Threading;

    public class ComplexType : StructuralType
    {
        private ReadOnlyMetadataCollection<EdmProperty> _properties;

        internal ComplexType()
        {
        }

        internal ComplexType(string name, string namespaceName, DataSpace dataSpace) : base(name, namespaceName, dataSpace)
        {
        }

        internal override void ValidateMemberForAdd(EdmMember member)
        {
            if (!Helper.IsEdmProperty(member) && !Helper.IsNavigationProperty(member))
            {
                throw EntityUtil.ComplexTypeInvalidMembers();
            }
        }

        public override System.Data.Metadata.Edm.BuiltInTypeKind BuiltInTypeKind =>
            System.Data.Metadata.Edm.BuiltInTypeKind.ComplexType;

        public ReadOnlyMetadataCollection<EdmProperty> Properties
        {
            get
            {
                if (this._properties == null)
                {
                    Interlocked.CompareExchange<ReadOnlyMetadataCollection<EdmProperty>>(ref this._properties, new FilteredReadOnlyMetadataCollection<EdmProperty, EdmMember>(base.Members, new Predicate<EdmMember>(Helper.IsEdmProperty)), null);
                }
                return this._properties;
            }
        }
    }
}

