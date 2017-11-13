namespace System.Data.EntityModel.SchemaObjectModel
{
    using System;
    using System.Data;
    using System.Data.Entity;
    using System.Data.Metadata.Edm;
    using System.Xml;

    internal class Parameter : FacetEnabledSchemaElement
    {
        private System.Data.Metadata.Edm.CollectionKind _collectionKind;
        private System.Data.ParameterDirection _parameterDirection;

        internal Parameter(System.Data.EntityModel.SchemaObjectModel.Function parentElement) : base(parentElement)
        {
            this._parameterDirection = System.Data.ParameterDirection.Input;
            base._typeUsageBuilder = new TypeUsageBuilder(this);
        }

        internal override SchemaElement Clone(SchemaElement parentElement) => 
            new Parameter((System.Data.EntityModel.SchemaObjectModel.Function) parentElement) { 
                _collectionKind = this._collectionKind,
                _parameterDirection = this._parameterDirection,
                _type = base._type,
                Name = this.Name,
                _typeUsageBuilder = base._typeUsageBuilder
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
            if (SchemaElement.CanHandleAttribute(reader, "Mode"))
            {
                this.HandleModeAttribute(reader);
                return true;
            }
            return base._typeUsageBuilder.HandleAttribute(reader);
        }

        private void HandleModeAttribute(XmlReader reader)
        {
            int num;
            string str = reader.Value;
            if (!string.IsNullOrEmpty(str))
            {
                str = str.Trim();
                if (string.IsNullOrEmpty(str))
                {
                    return;
                }
                string str2 = str;
                if (str2 == null)
                {
                    goto Label_0068;
                }
                if (str2 != "In")
                {
                    if (str2 == "Out")
                    {
                        this._parameterDirection = System.Data.ParameterDirection.Output;
                        return;
                    }
                    if (str2 == "InOut")
                    {
                        this._parameterDirection = System.Data.ParameterDirection.InputOutput;
                        return;
                    }
                    goto Label_0068;
                }
                this._parameterDirection = System.Data.ParameterDirection.Input;
            }
            return;
        Label_0068:
            num = base.ParentElement.Parameters.Count;
            base.AddError(ErrorCode.BadParameterDirection, EdmSchemaErrorSeverity.Error, reader, Strings.BadParameterDirection(str, num, base.ParentElement.Name, base.ParentElement.ParentElement.FQName));
        }

        private void HandleTypeAttribute(XmlReader reader)
        {
            string str;
            if (Utils.GetString(base.Schema, reader, out str))
            {
                switch (System.Data.EntityModel.SchemaObjectModel.Function.RemoveTypeModifier(ref str))
                {
                    case TypeModifier.None:
                        break;

                    case TypeModifier.Array:
                        this.CollectionKind = System.Data.Metadata.Edm.CollectionKind.Bag;
                        break;

                    default:
                        base.AddError(ErrorCode.BadType, EdmSchemaErrorSeverity.Error, Strings.BadTypeModifier(this.FQName, reader.Value));
                        break;
                }
                if (Utils.ValidateDottedName(base.Schema, reader, str))
                {
                    base.UnresolvedType = str;
                }
            }
        }

        public System.Data.Metadata.Edm.CollectionKind CollectionKind
        {
            get => 
                this._collectionKind;
            internal set
            {
                this._collectionKind = value;
            }
        }

        public System.Data.ParameterDirection ParameterDirection =>
            this._parameterDirection;
    }
}

