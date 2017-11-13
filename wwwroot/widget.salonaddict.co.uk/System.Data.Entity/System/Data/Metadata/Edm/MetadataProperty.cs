namespace System.Data.Metadata.Edm
{
    using System;
    using System.Data;

    public sealed class MetadataProperty : MetadataItem
    {
        private string _name;
        private System.Data.Metadata.Edm.PropertyKind _propertyKind;
        private System.Data.Metadata.Edm.TypeUsage _typeUsage;
        private object _value;

        internal MetadataProperty(string name, System.Data.Metadata.Edm.TypeUsage typeUsage, object value)
        {
            EntityUtil.GenericCheckArgumentNull<System.Data.Metadata.Edm.TypeUsage>(typeUsage, "typeUsage");
            this._name = name;
            this._value = value;
            this._typeUsage = typeUsage;
            this._propertyKind = System.Data.Metadata.Edm.PropertyKind.Extended;
        }

        internal MetadataProperty(string name, EdmType edmType, bool isCollectionType, object value)
        {
            EntityUtil.CheckArgumentNull<EdmType>(edmType, "edmType");
            this._name = name;
            this._value = value;
            if (isCollectionType)
            {
                this._typeUsage = System.Data.Metadata.Edm.TypeUsage.Create(edmType.GetCollectionType());
            }
            else
            {
                this._typeUsage = System.Data.Metadata.Edm.TypeUsage.Create(edmType);
            }
            this._propertyKind = System.Data.Metadata.Edm.PropertyKind.System;
        }

        internal override void SetReadOnly()
        {
            if (!base.IsReadOnly)
            {
                base.SetReadOnly();
            }
        }

        public override System.Data.Metadata.Edm.BuiltInTypeKind BuiltInTypeKind =>
            System.Data.Metadata.Edm.BuiltInTypeKind.MetadataProperty;

        internal override string Identity =>
            this.Name;

        [MetadataProperty(PrimitiveTypeKind.String, false)]
        public string Name =>
            this._name;

        public System.Data.Metadata.Edm.PropertyKind PropertyKind =>
            this._propertyKind;

        [MetadataProperty(System.Data.Metadata.Edm.BuiltInTypeKind.TypeUsage, false)]
        public System.Data.Metadata.Edm.TypeUsage TypeUsage =>
            this._typeUsage;

        [MetadataProperty(typeof(object), false)]
        public object Value
        {
            get
            {
                MetadataPropertyValue value2 = this._value as MetadataPropertyValue;
                if (value2 != null)
                {
                    return value2.GetValue();
                }
                return this._value;
            }
        }
    }
}

