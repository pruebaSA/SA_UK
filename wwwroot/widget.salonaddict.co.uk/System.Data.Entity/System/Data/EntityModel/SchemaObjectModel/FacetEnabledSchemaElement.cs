namespace System.Data.EntityModel.SchemaObjectModel
{
    using System;
    using System.Data.Entity;
    using System.Data.Metadata.Edm;
    using System.Xml;

    internal abstract class FacetEnabledSchemaElement : SchemaElement
    {
        protected SchemaType _type;
        protected TypeUsageBuilder _typeUsageBuilder;
        protected string _unresolvedType;

        internal FacetEnabledSchemaElement(Function parentElement) : base(parentElement)
        {
        }

        protected override bool HandleAttribute(XmlReader reader) => 
            (base.HandleAttribute(reader) || this._typeUsageBuilder.HandleAttribute(reader));

        internal override void ResolveTopLevelNames()
        {
            base.ResolveTopLevelNames();
            if (base.Schema.ResolveTypeName(this, this.UnresolvedType, out this._type))
            {
                if (!(this._type is ScalarType))
                {
                    base.AddError(ErrorCode.FunctionWithNonScalarTypeNotSupported, EdmSchemaErrorSeverity.Error, this, Strings.FunctionWithNonScalarTypeNotSupported(this._type.FQName, this.ParentElement.FQName));
                }
                else if (this._typeUsageBuilder.HasUserDefinedFacets)
                {
                    bool flag = base.Schema.DataModel == SchemaDataModelOption.ProviderManifestModel;
                    this._typeUsageBuilder.ValidateAndSetTypeUsage((ScalarType) this._type, !flag);
                }
            }
        }

        public bool HasUserDefinedFacets =>
            this._typeUsageBuilder.HasUserDefinedFacets;

        public Function ParentElement =>
            (base.ParentElement as Function);

        public SchemaType Type =>
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

