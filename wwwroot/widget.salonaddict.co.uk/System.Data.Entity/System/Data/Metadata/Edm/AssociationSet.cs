namespace System.Data.Metadata.Edm
{
    using System;

    public sealed class AssociationSet : RelationshipSet
    {
        private readonly ReadOnlyMetadataCollection<AssociationSetEnd> _associationSetEnds;

        internal AssociationSet(string name, AssociationType associationType) : base(name, null, null, null, associationType)
        {
            this._associationSetEnds = new ReadOnlyMetadataCollection<AssociationSetEnd>(new MetadataCollection<AssociationSetEnd>());
        }

        internal void AddAssociationSetEnd(AssociationSetEnd associationSetEnd)
        {
            this.AssociationSetEnds.Source.Add(associationSetEnd);
        }

        internal override void SetReadOnly()
        {
            if (!base.IsReadOnly)
            {
                base.SetReadOnly();
                this.AssociationSetEnds.Source.SetReadOnly();
            }
        }

        [MetadataProperty(System.Data.Metadata.Edm.BuiltInTypeKind.AssociationSetEnd, true)]
        public ReadOnlyMetadataCollection<AssociationSetEnd> AssociationSetEnds =>
            this._associationSetEnds;

        public override System.Data.Metadata.Edm.BuiltInTypeKind BuiltInTypeKind =>
            System.Data.Metadata.Edm.BuiltInTypeKind.AssociationSet;

        public AssociationType ElementType =>
            ((AssociationType) base.ElementType);
    }
}

