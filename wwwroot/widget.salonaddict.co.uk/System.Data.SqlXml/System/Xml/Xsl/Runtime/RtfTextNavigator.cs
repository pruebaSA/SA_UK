namespace System.Xml.Xsl.Runtime
{
    using System;
    using System.Xml;
    using System.Xml.XPath;

    internal sealed class RtfTextNavigator : RtfNavigator
    {
        private string baseUri;
        private NavigatorConstructor constr;
        private string text;

        public RtfTextNavigator(RtfTextNavigator that)
        {
            this.text = that.text;
            this.baseUri = that.baseUri;
            this.constr = that.constr;
        }

        public RtfTextNavigator(string text, string baseUri)
        {
            this.text = text;
            this.baseUri = baseUri;
            this.constr = new NavigatorConstructor();
        }

        public override XPathNavigator Clone() => 
            new RtfTextNavigator(this);

        public override void CopyToWriter(XmlWriter writer)
        {
            writer.WriteString(this.Value);
        }

        public override bool MoveTo(XPathNavigator other)
        {
            RtfTextNavigator navigator = other as RtfTextNavigator;
            if (navigator != null)
            {
                this.text = navigator.text;
                this.baseUri = navigator.baseUri;
                this.constr = navigator.constr;
                return true;
            }
            return false;
        }

        public override XPathNavigator ToNavigator() => 
            this.constr.GetNavigator(this.text, this.baseUri, new NameTable());

        public override string BaseURI =>
            this.baseUri;

        public override string Value =>
            this.text;
    }
}

