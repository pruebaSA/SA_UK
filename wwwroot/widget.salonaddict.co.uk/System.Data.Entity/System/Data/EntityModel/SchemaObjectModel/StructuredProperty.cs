namespace System.Data.EntityModel.SchemaObjectModel
{
    using System;
    using System.Data.Entity;
    using System.Data.Metadata.Edm;
    using System.Xml;

    internal class StructuredProperty : Property
    {
        private System.Data.Metadata.Edm.CollectionKind _collectionKind;
        private SchemaType _type;
        private TypeUsageBuilder _typeUsageBuilder;
        private string _unresolvedType;

        internal StructuredProperty(StructuredType parentElement) : base(parentElement)
        {
            this._typeUsageBuilder = new TypeUsageBuilder(this);
        }

        protected override bool HandleAttribute(XmlReader reader)
        {
            if (base.HandleAttribute(reader))
            {
                return true;
            }
            if (SchemaElement.CanHandleAttribute(reader, "Type"))
            {
                this.HandleTypeAttribute(reader);
                return true;
            }
            if (SchemaElement.CanHandleAttribute(reader, "CollectionKind"))
            {
                this.HandleCollectionKindAttribute(reader);
                return true;
            }
            return this._typeUsageBuilder.HandleAttribute(reader);
        }

        private void HandleCollectionKindAttribute(XmlReader reader)
        {
            switch (reader.Value)
            {
                case "None":
                    this._collectionKind = System.Data.Metadata.Edm.CollectionKind.None;
                    return;

                case "List":
                    this._collectionKind = System.Data.Metadata.Edm.CollectionKind.List;
                    break;

                case "Bag":
                    this._collectionKind = System.Data.Metadata.Edm.CollectionKind.Bag;
                    break;
            }
        }

        private void HandleTypeAttribute(XmlReader reader)
        {
            if (this.UnresolvedType != null)
            {
                base.AddError(ErrorCode.AlreadyDefined, EdmSchemaErrorSeverity.Error, reader, Strings.PropertyTypeAlreadyDefined(reader.Name));
            }
            else
            {
                string str;
                if (Utils.GetDottedName(base.Schema, reader, out str))
                {
                    this.UnresolvedType = str;
                }
            }
        }

        internal override void ResolveTopLevelNames()
        {
            base.ResolveTopLevelNames();
            if (this._type == null)
            {
                this._type = this.ResolveType(this.UnresolvedType);
                this._typeUsageBuilder.ValidateDefaultValue(this._type);
                ScalarType scalar = this._type as ScalarType;
                if (scalar != null)
                {
                    this._typeUsageBuilder.ValidateAndSetTypeUsage(scalar, true);
                }
            }
        }

        protected virtual SchemaType ResolveType(string typeName)
        {
            SchemaType type;
            if (base.Schema.ResolveTypeName(this, typeName, out type))
            {
                if ((type is SchemaComplexType) || (type is ScalarType))
                {
                    return type;
                }
                base.AddError(ErrorCode.InvalidPropertyType, EdmSchemaErrorSeverity.Error, Strings.InvalidPropertyType(this.UnresolvedType));
            }
            return null;
        }

        internal override void Validate()
        {
            base.Validate();
            if (this._collectionKind != System.Data.Metadata.Edm.CollectionKind.Bag)
            {
                System.Data.Metadata.Edm.CollectionKind kind1 = this._collectionKind;
            }
            if ((this.Nullable && (base.Schema.EdmVersion == 1.0)) && (this._type is SchemaComplexType))
            {
                base.AddError(ErrorCode.NullableComplexType, EdmSchemaErrorSeverity.Error, Strings.NullableComplexType(this.FQName));
            }
        }

        public System.Data.Metadata.Edm.CollectionKind CollectionKind =>
            this._collectionKind;

        public string Default =>
            this._typeUsageBuilder.Default;

        public object DefaultAsObject =>
            this._typeUsageBuilder.DefaultAsObject;

        public bool Nullable =>
            this._typeUsageBuilder.Nullable;

        public override SchemaType Type =>
            this._type;

        public System.Data.Metadata.Edm.TypeUsage TypeUsage =>
            this._typeUsageBuilder.TypeUsage;

        internal string UnresolvedType
        {
            get => 
                this._unresolvedType;
            set
            {
                this._unresolvedType = value;
            }
        }
    }
}

