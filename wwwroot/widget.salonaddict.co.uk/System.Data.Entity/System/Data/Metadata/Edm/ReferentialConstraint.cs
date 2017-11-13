namespace System.Data.Metadata.Edm
{
    using System;
    using System.Collections.Generic;
    using System.Data;

    public sealed class ReferentialConstraint : MetadataItem
    {
        private readonly ReadOnlyMetadataCollection<EdmProperty> _fromProperties;
        private RelationshipEndMember _fromRole;
        private readonly ReadOnlyMetadataCollection<EdmProperty> _toProperties;
        private RelationshipEndMember _toRole;

        internal ReferentialConstraint(RelationshipEndMember fromRole, RelationshipEndMember toRole, IEnumerable<EdmProperty> fromProperties, IEnumerable<EdmProperty> toProperties)
        {
            this._fromRole = EntityUtil.GenericCheckArgumentNull<RelationshipEndMember>(fromRole, "fromRole");
            this._toRole = EntityUtil.GenericCheckArgumentNull<RelationshipEndMember>(toRole, "toRole");
            this._fromProperties = new ReadOnlyMetadataCollection<EdmProperty>(new MetadataCollection<EdmProperty>(EntityUtil.GenericCheckArgumentNull<IEnumerable<EdmProperty>>(fromProperties, "fromProperties")));
            this._toProperties = new ReadOnlyMetadataCollection<EdmProperty>(new MetadataCollection<EdmProperty>(EntityUtil.GenericCheckArgumentNull<IEnumerable<EdmProperty>>(toProperties, "toProperties")));
        }

        internal override void SetReadOnly()
        {
            if (!base.IsReadOnly)
            {
                base.SetReadOnly();
                RelationshipEndMember fromRole = this.FromRole;
                if (fromRole != null)
                {
                    fromRole.SetReadOnly();
                }
                RelationshipEndMember toRole = this.ToRole;
                if (toRole != null)
                {
                    toRole.SetReadOnly();
                }
                this.FromProperties.Source.SetReadOnly();
                this.ToProperties.Source.SetReadOnly();
            }
        }

        public override string ToString() => 
            (this.FromRole.Name + "_" + this.ToRole.Name);

        public override System.Data.Metadata.Edm.BuiltInTypeKind BuiltInTypeKind =>
            System.Data.Metadata.Edm.BuiltInTypeKind.ReferentialConstraint;

        [MetadataProperty(System.Data.Metadata.Edm.BuiltInTypeKind.EdmProperty, true)]
        public ReadOnlyMetadataCollection<EdmProperty> FromProperties =>
            this._fromProperties;

        [MetadataProperty(System.Data.Metadata.Edm.BuiltInTypeKind.RelationshipEndMember, false)]
        public RelationshipEndMember FromRole =>
            this._fromRole;

        internal override string Identity =>
            (this.FromRole.Name + "_" + this.ToRole.Name);

        [MetadataProperty(System.Data.Metadata.Edm.BuiltInTypeKind.EdmProperty, true)]
        public ReadOnlyMetadataCollection<EdmProperty> ToProperties =>
            this._toProperties;

        [MetadataProperty(System.Data.Metadata.Edm.BuiltInTypeKind.RelationshipEndMember, false)]
        public RelationshipEndMember ToRole =>
            this._toRole;
    }
}

