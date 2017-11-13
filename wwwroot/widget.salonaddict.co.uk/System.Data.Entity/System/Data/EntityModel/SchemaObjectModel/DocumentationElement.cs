namespace System.Data.EntityModel.SchemaObjectModel
{
    using System;
    using System.Data.Common.Utils;
    using System.Data.Entity;
    using System.Data.Metadata.Edm;
    using System.Xml;

    internal sealed class DocumentationElement : SchemaElement
    {
        private Documentation _metdataDocumentation;

        public DocumentationElement(SchemaElement parentElement) : base(parentElement)
        {
            this._metdataDocumentation = new Documentation();
        }

        public static string Entityize(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return "";
            }
            text = text.Replace("&", "&amp;");
            text = text.Replace("<", "&lt;").Replace(">", "&gt;");
            return text.Replace("'", "&apos;").Replace("\"", "&quot;");
        }

        protected override bool HandleElement(XmlReader reader)
        {
            if (base.HandleElement(reader))
            {
                return true;
            }
            if (base.CanHandleElement(reader, "Summary"))
            {
                this.HandleSummaryElement(reader);
                return true;
            }
            if (base.CanHandleElement(reader, "LongDescription"))
            {
                this.HandleLongDescriptionElement(reader);
                return true;
            }
            return false;
        }

        private void HandleLongDescriptionElement(XmlReader reader)
        {
            TextElement element = new TextElement(this);
            element.Parse(reader);
            this._metdataDocumentation.LongDescription = element.Value;
        }

        private void HandleSummaryElement(XmlReader reader)
        {
            TextElement element = new TextElement(this);
            element.Parse(reader);
            this._metdataDocumentation.Summary = element.Value;
        }

        protected override bool HandleText(XmlReader reader)
        {
            if (!StringUtil.IsNullOrEmptyOrWhiteSpace(reader.Value))
            {
                base.AddError(ErrorCode.UnexpectedXmlElement, EdmSchemaErrorSeverity.Error, Strings.InvalidDocumentationBothTextAndStructure);
            }
            return true;
        }

        public Documentation MetadataDocumentation
        {
            get
            {
                this._metdataDocumentation.SetReadOnly();
                return this._metdataDocumentation;
            }
        }
    }
}

