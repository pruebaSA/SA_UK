namespace System.ServiceModel.Syndication
{
    using System;
    using System.ServiceModel;
    using System.Xml;

    public class TextSyndicationContent : SyndicationContent
    {
        private string text;
        private TextSyndicationContentKind textKind;

        protected TextSyndicationContent(TextSyndicationContent source) : base(source)
        {
            if (source == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("source");
            }
            this.text = source.text;
            this.textKind = source.textKind;
        }

        public TextSyndicationContent(string text) : this(text, TextSyndicationContentKind.Plaintext)
        {
        }

        public TextSyndicationContent(string text, TextSyndicationContentKind textKind)
        {
            if (!TextSyndicationContentKindHelper.IsDefined(textKind))
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("textKind"));
            }
            this.text = text;
            this.textKind = textKind;
        }

        public override SyndicationContent Clone() => 
            new TextSyndicationContent(this);

        protected override void WriteContentsTo(XmlWriter writer)
        {
            string data = this.text ?? string.Empty;
            if (this.textKind == TextSyndicationContentKind.XHtml)
            {
                writer.WriteRaw(data);
            }
            else
            {
                writer.WriteString(data);
            }
        }

        public string Text =>
            this.text;

        public override string Type
        {
            get
            {
                switch (this.textKind)
                {
                    case TextSyndicationContentKind.Html:
                        return "html";

                    case TextSyndicationContentKind.XHtml:
                        return "xhtml";
                }
                return "text";
            }
        }
    }
}

