namespace System.Data.Metadata.Edm
{
    using System;
    using System.Data;
    using System.Threading;

    public sealed class AssociationType : RelationshipType
    {
        private FilteredReadOnlyMetadataCollection<AssociationEndMember, EdmMember> _associationEndMembers;
        private readonly ReadOnlyMetadataCollection<ReferentialConstraint> _referentialConstraints;

        internal AssociationType(string name, string namespaceName, DataSpace dataSpace) : base(name, namespaceName, dataSpace)
        {
            this._referentialConstraints = new ReadOnlyMetadataCollection<ReferentialConstraint>(new MetadataCollection<ReferentialConstraint>());
        }

        internal void AddReferentialConstraint(ReferentialConstraint referentialConstraint)
        {
            this.ReferentialConstraints.Source.Add(referentialConstraint);
        }

        internal override void SetReadOnly()
        {
            if (!base.IsReadOnly)
            {
                base.SetReadOnly();
                this.ReferentialConstraints.Source.SetReadOnly();
            }
        }

        internal override void ValidateMemberForAdd(EdmMember member)
        {
            if (!(member is AssociationEndMember))
            {
                throw EntityUtil.AssociationInvalidMembers();
            }
        }

        public ReadOnlyMetadataCollection<AssociationEndMember> AssociationEndMembers
        {
            get
            {
                if (this._associationEndMembers == null)
                {
                    Interlocked.CompareExchange<FilteredReadOnlyMetadataCollection<AssociationEndMember, EdmMember>>(ref this._associationEndMembers, new FilteredReadOnlyMetadataCollection<AssociationEndMember, EdmMember>(base.Members, new Predicate<EdmMember>(Helper.IsAssociationEndMember)), null);
                }
                return this._associationEndMembers;
            }
        }

        public override System.Data.Metadata.Edm.BuiltInTypeKind BuiltInTypeKind =>
            System.Data.Metadata.Edm.BuiltInTypeKind.AssociationType;

        [MetadataProperty(System.Data.Metadata.Edm.BuiltInTypeKind.ReferentialConstraint, true)]
        public ReadOnlyMetadataCollection<ReferentialConstraint> ReferentialConstraints =>
            this._referentialConstraints;
    }
}

