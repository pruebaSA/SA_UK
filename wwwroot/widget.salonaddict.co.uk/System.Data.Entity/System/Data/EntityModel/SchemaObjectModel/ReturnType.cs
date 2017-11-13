namespace System.Data.EntityModel.SchemaObjectModel
{
    using System;
    using System.Data.Entity;
    using System.Data.Metadata.Edm;
    using System.Xml;

    internal class ReturnType : FacetEnabledSchemaElement
    {
        internal ReturnType(Function parentElement) : base(parentElement)
        {
            base._typeUsageBuilder = new TypeUsageBuilder(this);
        }

        internal override SchemaElement Clone(SchemaElement parentElement) => 
            new ReturnType((Function) parentElement) { 
                _type = base._type,
                Name = this.Name,
                _typeUsageBuilder = base._typeUsageBuilder,
                _unresolvedType = base._unresolvedType
            };

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
            return false;
        }

        private void HandleTypeAttribute(XmlReader reader)
        {
            string str;
            if (Utils.GetString(base.Schema, reader, out str) && Utils.ValidateDottedName(base.Schema, reader, str))
            {
                base.UnresolvedType = str;
            }
        }

        internal override void ResolveTopLevelNames()
        {
            base.ResolveTopLevelNames();
            if (base.Schema.ResolveTypeName(this, base.UnresolvedType, out this._type) && !(base._type is ScalarType))
            {
                if (base.Schema.DataModel != SchemaDataModelOption.ProviderManifestModel)
                {
                    base.AddError(ErrorCode.FunctionWithNonScalarTypeNotSupported, EdmSchemaErrorSeverity.Error, this, Strings.FunctionWithNonScalarTypeNotSupported(base._type.FQName, this.FQName));
                }
                else
                {
                    base.AddError(ErrorCode.FunctionWithNonScalarTypeNotSupported, EdmSchemaErrorSeverity.Error, this, Strings.FunctionWithNonEdmTypeNotSupported(base._type.FQName, this.FQName));
                }
            }
        }
    }
}

