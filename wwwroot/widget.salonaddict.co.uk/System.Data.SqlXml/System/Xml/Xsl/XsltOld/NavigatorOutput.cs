namespace System.Xml.Xsl.XsltOld
{
    using System;
    using System.Xml;
    using System.Xml.XPath;

    internal class NavigatorOutput : RecordOutput
    {
        private XPathDocument doc = new XPathDocument();
        private int documentIndex;
        private XmlRawWriter wr;

        internal NavigatorOutput(string baseUri)
        {
            this.wr = this.doc.LoadFromWriter(XPathDocument.LoadFlags.AtomizeNames, baseUri);
        }

        public Processor.OutputResult RecordDone(RecordBuilder record)
        {
            BuilderInfo mainNode = record.MainNode;
            this.documentIndex++;
            switch (mainNode.NodeType)
            {
                case XmlNodeType.Element:
                    this.wr.WriteStartElement(mainNode.Prefix, mainNode.LocalName, mainNode.NamespaceURI);
                    for (int i = 0; i < record.AttributeCount; i++)
                    {
                        this.documentIndex++;
                        BuilderInfo info2 = (BuilderInfo) record.AttributeList[i];
                        if (info2.NamespaceURI == "http://www.w3.org/2000/xmlns/")
                        {
                            if (info2.Prefix.Length == 0)
                            {
                                this.wr.WriteNamespaceDeclaration(string.Empty, info2.Value);
                            }
                            else
                            {
                                this.wr.WriteNamespaceDeclaration(info2.LocalName, info2.Value);
                            }
                        }
                        else
                        {
                            this.wr.WriteAttributeString(info2.Prefix, info2.LocalName, info2.NamespaceURI, info2.Value);
                        }
                    }
                    this.wr.StartElementContent();
                    if (mainNode.IsEmptyTag)
                    {
                        this.wr.WriteEndElement(mainNode.Prefix, mainNode.LocalName, mainNode.NamespaceURI);
                    }
                    break;

                case XmlNodeType.Text:
                    this.wr.WriteString(mainNode.Value);
                    break;

                case XmlNodeType.ProcessingInstruction:
                    this.wr.WriteProcessingInstruction(mainNode.LocalName, mainNode.Value);
                    break;

                case XmlNodeType.Comment:
                    this.wr.WriteComment(mainNode.Value);
                    break;

                case XmlNodeType.SignificantWhitespace:
                    this.wr.WriteString(mainNode.Value);
                    break;

                case XmlNodeType.EndElement:
                    this.wr.WriteEndElement(mainNode.Prefix, mainNode.LocalName, mainNode.NamespaceURI);
                    break;
            }
            record.Reset();
            return Processor.OutputResult.Continue;
        }

        public void TheEnd()
        {
            this.wr.Close();
        }

        internal XPathNavigator Navigator =>
            this.doc.CreateNavigator();
    }
}

