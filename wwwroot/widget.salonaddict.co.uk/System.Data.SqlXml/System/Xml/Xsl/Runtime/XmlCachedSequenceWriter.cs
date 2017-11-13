namespace System.Xml.Xsl.Runtime
{
    using System;
    using System.Xml;
    using System.Xml.XPath;

    internal class XmlCachedSequenceWriter : XmlSequenceWriter
    {
        private XPathDocument doc;
        private XmlQueryItemSequence seqTyped = new XmlQueryItemSequence();
        private XmlRawWriter writer;

        public override void EndTree()
        {
            this.writer.Close();
            this.seqTyped.Add(this.doc.CreateNavigator());
        }

        public override XmlRawWriter StartTree(XPathNodeType rootType, IXmlNamespaceResolver nsResolver, XmlNameTable nameTable)
        {
            this.doc = new XPathDocument(nameTable);
            this.writer = this.doc.LoadFromWriter(XPathDocument.LoadFlags.AtomizeNames | ((rootType == XPathNodeType.Root) ? XPathDocument.LoadFlags.None : XPathDocument.LoadFlags.Fragment), string.Empty);
            this.writer.NamespaceResolver = nsResolver;
            return this.writer;
        }

        public override void WriteItem(XPathItem item)
        {
            this.seqTyped.AddClone(item);
        }

        public XmlQueryItemSequence ResultSequence =>
            this.seqTyped;
    }
}

