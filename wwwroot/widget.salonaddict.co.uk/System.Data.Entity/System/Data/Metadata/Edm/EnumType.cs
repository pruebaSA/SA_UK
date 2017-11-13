namespace System.Data.Metadata.Edm
{
    using System;

    internal sealed class EnumType : SimpleType
    {
        private readonly ReadOnlyMetadataCollection<EnumMember> _enumMembers;

        internal EnumType()
        {
            this._enumMembers = new ReadOnlyMetadataCollection<EnumMember>(new MetadataCollection<EnumMember>());
        }

        internal EnumType(string name, string namespaceName, DataSpace dataSpace) : base(name, namespaceName, dataSpace)
        {
            this._enumMembers = new ReadOnlyMetadataCollection<EnumMember>(new MetadataCollection<EnumMember>());
        }

        internal void AddMember(EnumMember enumMember)
        {
            this.EnumMembers.Source.Add(enumMember);
        }

        internal override void SetReadOnly()
        {
            if (!base.IsReadOnly)
            {
                base.SetReadOnly();
                this.EnumMembers.Source.SetReadOnly();
            }
        }

        public override System.Data.Metadata.Edm.BuiltInTypeKind BuiltInTypeKind =>
            System.Data.Metadata.Edm.BuiltInTypeKind.EnumType;

        [MetadataProperty(System.Data.Metadata.Edm.BuiltInTypeKind.EnumMember, true)]
        public ReadOnlyMetadataCollection<EnumMember> EnumMembers =>
            this._enumMembers;
    }
}

