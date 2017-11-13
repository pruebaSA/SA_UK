namespace System.Xml.Xsl.XsltOld
{
    using System;
    using System.Xml.XPath;

    internal class NamespaceEvent : Event
    {
        private string name;
        private string namespaceUri;

        public NamespaceEvent(NavigatorInput input)
        {
            this.namespaceUri = input.Value;
            this.name = input.LocalName;
        }

        public override bool Output(Processor processor, ActionFrame frame)
        {
            processor.BeginEvent(XPathNodeType.Namespace, null, this.name, this.namespaceUri, false);
            processor.EndEvent(XPathNodeType.Namespace);
            return true;
        }

        public override void ReplaceNamespaceAlias(Compiler compiler)
        {
            if (this.namespaceUri.Length != 0)
            {
                NamespaceInfo info = compiler.FindNamespaceAlias(this.namespaceUri);
                if (info != null)
                {
                    this.namespaceUri = info.nameSpace;
                    if (info.prefix != null)
                    {
                        this.name = info.prefix;
                    }
                }
            }
        }
    }
}

