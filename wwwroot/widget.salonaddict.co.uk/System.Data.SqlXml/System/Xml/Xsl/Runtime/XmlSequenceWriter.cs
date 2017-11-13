namespace System.Xml.Xsl.Runtime
{
    using System;
    using System.Xml;
    using System.Xml.XPath;

    internal abstract class XmlSequenceWriter
    {
        protected XmlSequenceWriter()
        {
        }

        public abstract void EndTree();
        public abstract XmlRawWriter StartTree(XPathNodeType rootType, IXmlNamespaceResolver nsResolver, XmlNameTable nameTable);
        public abstract void WriteItem(XPathItem item);
    }
}

