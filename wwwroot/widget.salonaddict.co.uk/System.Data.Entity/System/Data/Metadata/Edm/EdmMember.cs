namespace System.Data.Metadata.Edm
{
    using System;
    using System.Data;

    public abstract class EdmMember : MetadataItem
    {
        private StructuralType _declaringType;
        private string _name;
        private System.Data.Metadata.Edm.TypeUsage _typeUsage;

        internal EdmMember(string name, System.Data.Metadata.Edm.TypeUsage memberTypeUsage)
        {
            EntityUtil.CheckStringArgument(name, "name");
            EntityUtil.GenericCheckArgumentNull<System.Data.Metadata.Edm.TypeUsage>(memberTypeUsage, "memberTypeUsage");
            this._name = name;
            this._typeUsage = memberTypeUsage;
        }

        internal void ChangeDeclaringTypeWithoutCollectionFixup(StructuralType newDeclaringType)
        {
            this._declaringType = newDeclaringType;
        }

        internal override void SetReadOnly()
        {
            if (!base.IsReadOnly)
            {
                base.SetReadOnly();
            }
        }

        public override string ToString() => 
            this.Name;

        public StructuralType DeclaringType =>
            this._declaringType;

        internal override string Identity =>
            this.Name;

        internal bool IsStoreGeneratedComputed
        {
            get
            {
                Facet item = null;
                return (this.TypeUsage.Facets.TryGetValue("StoreGeneratedPattern", false, out item) && (((StoreGeneratedPattern) item.Value) == StoreGeneratedPattern.Computed));
            }
        }

        internal bool IsStoreGeneratedIdentity
        {
            get
            {
                Facet item = null;
                return (this.TypeUsage.Facets.TryGetValue("StoreGeneratedPattern", false, out item) && (((StoreGeneratedPattern) item.Value) == StoreGeneratedPattern.Identity));
            }
        }

        [MetadataProperty(PrimitiveTypeKind.String, false)]
        public string Name =>
            this._name;

        [MetadataProperty(BuiltInTypeKind.TypeUsage, false)]
        public System.Data.Metadata.Edm.TypeUsage TypeUsage =>
            this._typeUsage;
    }
}

