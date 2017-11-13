namespace System.Data.EntityModel.SchemaObjectModel
{
    using System;
    using System.Data.Entity;
    using System.Data.Metadata.Edm;
    using System.Xml;

    internal sealed class FunctionCommandText : SchemaElement
    {
        private string _commandText;

        public FunctionCommandText(Function parentElement) : base(parentElement)
        {
        }

        protected override bool HandleText(XmlReader reader)
        {
            this._commandText = reader.Value;
            return true;
        }

        internal override void Validate()
        {
            base.Validate();
            if (string.IsNullOrEmpty(this._commandText))
            {
                base.AddError(ErrorCode.EmptyCommandText, EdmSchemaErrorSeverity.Error, Strings.EmptyCommandText);
            }
        }

        public string CommandText =>
            this._commandText;
    }
}

