namespace System.Xml.Xsl.XsltOld
{
    using System;
    using System.Xml.XPath;

    internal class BeginEvent : Event
    {
        private bool empty;
        private object htmlProps;
        private string name;
        private string namespaceUri;
        private XPathNodeType nodeType;
        private string prefix;

        public BeginEvent(Compiler compiler)
        {
            NavigatorInput input = compiler.Input;
            this.nodeType = input.NodeType;
            this.namespaceUri = input.NamespaceURI;
            this.name = input.LocalName;
            this.prefix = input.Prefix;
            this.empty = input.IsEmptyTag;
            if (this.nodeType == XPathNodeType.Element)
            {
                this.htmlProps = HtmlElementProps.GetProps(this.name);
            }
            else if (this.nodeType == XPathNodeType.Attribute)
            {
                this.htmlProps = HtmlAttributeProps.GetProps(this.name);
            }
        }

        public override bool Output(Processor processor, ActionFrame frame) => 
            processor.BeginEvent(this.nodeType, this.prefix, this.name, this.namespaceUri, this.empty, this.htmlProps, false);

        public override void ReplaceNamespaceAlias(Compiler compiler)
        {
            if ((this.nodeType != XPathNodeType.Attribute) || (this.namespaceUri.Length != 0))
            {
                NamespaceInfo info = compiler.FindNamespaceAlias(this.namespaceUri);
                if (info != null)
                {
                    this.namespaceUri = info.nameSpace;
                    if (info.prefix != null)
                    {
                        this.prefix = info.prefix;
                    }
                }
            }
        }
    }
}

