namespace System.Data.Metadata.Edm
{
    using System;
    using System.Data;

    internal sealed class EnumMember : MetadataItem
    {
        private readonly string _name;

        internal EnumMember(string name) : base(MetadataItem.MetadataFlags.Readonly)
        {
            EntityUtil.CheckStringArgument(name, "name");
            this._name = name;
        }

        public override string ToString() => 
            this.Name;

        public override System.Data.Metadata.Edm.BuiltInTypeKind BuiltInTypeKind =>
            System.Data.Metadata.Edm.BuiltInTypeKind.EnumMember;

        internal override string Identity =>
            this.Name;

        [MetadataProperty(PrimitiveTypeKind.String, false)]
        public string Name =>
            this._name;
    }
}

