namespace System.Data.Metadata.Edm
{
    using System;

    public abstract class RelationshipEndMember : EdmMember
    {
        private OperationAction _deleteBehavior;
        private System.Data.Metadata.Edm.RelationshipMultiplicity _relationshipMultiplicity;

        internal RelationshipEndMember(string name, RefType endRefType, System.Data.Metadata.Edm.RelationshipMultiplicity multiplicity) : base(name, TypeUsage.Create(endRefType, values))
        {
            FacetValues values = new FacetValues {
                Nullable = 0
            };
            this._relationshipMultiplicity = multiplicity;
            this._deleteBehavior = OperationAction.None;
        }

        [MetadataProperty(BuiltInTypeKind.OperationAction, true)]
        public OperationAction DeleteBehavior
        {
            get => 
                this._deleteBehavior;
            internal set
            {
                Util.ThrowIfReadOnly(this);
                this._deleteBehavior = value;
            }
        }

        [MetadataProperty(BuiltInTypeKind.RelationshipMultiplicity, false)]
        public System.Data.Metadata.Edm.RelationshipMultiplicity RelationshipMultiplicity =>
            this._relationshipMultiplicity;
    }
}

