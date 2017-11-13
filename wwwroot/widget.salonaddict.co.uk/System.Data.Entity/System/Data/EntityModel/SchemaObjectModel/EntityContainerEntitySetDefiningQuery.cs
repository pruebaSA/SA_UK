namespace System.Data.EntityModel.SchemaObjectModel
{
    using System;
    using System.Data.Entity;
    using System.Data.Metadata.Edm;
    using System.Xml;

    internal sealed class EntityContainerEntitySetDefiningQuery : SchemaElement
    {
        private string _query;

        public EntityContainerEntitySetDefiningQuery(EntityContainerEntitySet parentElement) : base(parentElement)
        {
        }

        protected override bool HandleText(XmlReader reader)
        {
            this._query = reader.Value;
            return true;
        }

        internal override void Validate()
        {
            base.Validate();
            if (string.IsNullOrEmpty(this._query))
            {
                base.AddError(ErrorCode.EmptyDefiningQuery, EdmSchemaErrorSeverity.Error, Strings.EmptyDefiningQuery);
            }
        }

        public string Query =>
            this._query;
    }
}

