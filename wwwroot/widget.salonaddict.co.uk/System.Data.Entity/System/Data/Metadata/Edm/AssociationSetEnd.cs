namespace System.Data.Metadata.Edm
{
    using System;
    using System.Data;

    public sealed class AssociationSetEnd : MetadataItem
    {
        private readonly AssociationEndMember _endMember;
        private readonly System.Data.Metadata.Edm.EntitySet _entitySet;
        private readonly AssociationSet _parentSet;

        internal AssociationSetEnd(System.Data.Metadata.Edm.EntitySet entitySet, AssociationSet parentSet, AssociationEndMember endMember)
        {
            this._entitySet = EntityUtil.GenericCheckArgumentNull<System.Data.Metadata.Edm.EntitySet>(entitySet, "entitySet");
            this._parentSet = EntityUtil.GenericCheckArgumentNull<AssociationSet>(parentSet, "parentSet");
            this._endMember = EntityUtil.GenericCheckArgumentNull<AssociationEndMember>(endMember, "endMember");
        }

        internal override void SetReadOnly()
        {
            if (!base.IsReadOnly)
            {
                base.SetReadOnly();
                AssociationSet parentAssociationSet = this.ParentAssociationSet;
                if (parentAssociationSet != null)
                {
                    parentAssociationSet.SetReadOnly();
                }
                AssociationEndMember correspondingAssociationEndMember = this.CorrespondingAssociationEndMember;
                if (correspondingAssociationEndMember != null)
                {
                    correspondingAssociationEndMember.SetReadOnly();
                }
                System.Data.Metadata.Edm.EntitySet entitySet = this.EntitySet;
                if (entitySet != null)
                {
                    entitySet.SetReadOnly();
                }
            }
        }

        public override string ToString() => 
            this.Name;

        public override System.Data.Metadata.Edm.BuiltInTypeKind BuiltInTypeKind =>
            System.Data.Metadata.Edm.BuiltInTypeKind.AssociationSetEnd;

        [MetadataProperty(System.Data.Metadata.Edm.BuiltInTypeKind.AssociationEndMember, false)]
        public AssociationEndMember CorrespondingAssociationEndMember =>
            this._endMember;

        [MetadataProperty(System.Data.Metadata.Edm.BuiltInTypeKind.EntitySet, false)]
        public System.Data.Metadata.Edm.EntitySet EntitySet =>
            this._entitySet;

        internal override string Identity =>
            this.Name;

        [MetadataProperty(PrimitiveTypeKind.String, false)]
        public string Name =>
            this.CorrespondingAssociationEndMember.Name;

        [MetadataProperty(System.Data.Metadata.Edm.BuiltInTypeKind.AssociationSet, false)]
        public AssociationSet ParentAssociationSet =>
            this._parentSet;

        [Obsolete("This property is going away, please use the Name property instead"), MetadataProperty(PrimitiveTypeKind.String, false)]
        public string Role =>
            this.Name;
    }
}

