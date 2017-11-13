namespace System.Data.EntityModel.SchemaObjectModel
{
    using System;
    using System.Xml;

    internal sealed class TextElement : SchemaElement
    {
        private string _value;

        public TextElement(SchemaElement parentElement) : base(parentElement)
        {
        }

        protected override bool HandleText(XmlReader reader)
        {
            this.TextElementTextHandler(reader);
            return true;
        }

        private void TextElementTextHandler(XmlReader reader)
        {
            string str = reader.Value;
            if (!string.IsNullOrEmpty(str))
            {
                if (string.IsNullOrEmpty(this.Value))
                {
                    this.Value = str;
                }
                else
                {
                    this.Value = this.Value + str;
                }
            }
        }

        public string Value
        {
            get => 
                this._value;
            private set
            {
                this._value = value;
            }
        }
    }
}

