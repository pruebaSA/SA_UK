namespace System.Xml.Xsl.Runtime
{
    using System;
    using System.Xml;
    using System.Xml.XPath;
    using System.Xml.Xsl;

    internal class XmlMergeSequenceWriter : XmlSequenceWriter
    {
        private bool lastItemWasAtomic;
        private XmlRawWriter xwrt;

        public XmlMergeSequenceWriter(XmlRawWriter xwrt)
        {
            this.xwrt = xwrt;
            this.lastItemWasAtomic = false;
        }

        private void CopyNamespaces(XPathNavigator nav, XPathNamespaceScope nsScope)
        {
            string localName = nav.LocalName;
            string ns = nav.Value;
            if (nav.MoveToNextNamespace(nsScope))
            {
                this.CopyNamespaces(nav, nsScope);
            }
            this.xwrt.WriteNamespaceDeclaration(localName, ns);
        }

        private void CopyNode(XPathNavigator nav)
        {
            int num = 0;
        Label_0002:
            if (this.CopyShallowNode(nav))
            {
                if (nav.NodeType == XPathNodeType.Element)
                {
                    if (nav.MoveToFirstAttribute())
                    {
                        do
                        {
                            this.CopyShallowNode(nav);
                        }
                        while (nav.MoveToNextAttribute());
                        nav.MoveToParent();
                    }
                    XPathNamespaceScope namespaceScope = (num == 0) ? XPathNamespaceScope.ExcludeXml : XPathNamespaceScope.Local;
                    if (nav.MoveToFirstNamespace(namespaceScope))
                    {
                        this.CopyNamespaces(nav, namespaceScope);
                        nav.MoveToParent();
                    }
                    this.xwrt.StartElementContent();
                }
                if (nav.MoveToFirstChild())
                {
                    num++;
                    goto Label_0002;
                }
                if (nav.NodeType == XPathNodeType.Element)
                {
                    this.xwrt.WriteEndElement(nav.Prefix, nav.LocalName, nav.NamespaceURI);
                }
            }
            while (num != 0)
            {
                if (nav.MoveToNext())
                {
                    goto Label_0002;
                }
                num--;
                nav.MoveToParent();
                if (nav.NodeType == XPathNodeType.Element)
                {
                    this.xwrt.WriteFullEndElement(nav.Prefix, nav.LocalName, nav.NamespaceURI);
                }
            }
        }

        private bool CopyShallowNode(XPathNavigator nav)
        {
            bool flag = false;
            switch (nav.NodeType)
            {
                case XPathNodeType.Root:
                    return true;

                case XPathNodeType.Element:
                    this.xwrt.WriteStartElement(nav.Prefix, nav.LocalName, nav.NamespaceURI);
                    return true;

                case XPathNodeType.Attribute:
                    this.xwrt.WriteStartAttribute(nav.Prefix, nav.LocalName, nav.NamespaceURI);
                    this.xwrt.WriteString(nav.Value);
                    this.xwrt.WriteEndAttribute();
                    return flag;

                case XPathNodeType.Namespace:
                    this.xwrt.WriteNamespaceDeclaration(nav.LocalName, nav.Value);
                    return flag;

                case XPathNodeType.Text:
                    this.xwrt.WriteString(nav.Value);
                    return flag;

                case XPathNodeType.SignificantWhitespace:
                case XPathNodeType.Whitespace:
                    this.xwrt.WriteWhitespace(nav.Value);
                    return flag;

                case XPathNodeType.ProcessingInstruction:
                    this.xwrt.WriteProcessingInstruction(nav.LocalName, nav.Value);
                    return flag;

                case XPathNodeType.Comment:
                    this.xwrt.WriteComment(nav.Value);
                    return flag;
            }
            return flag;
        }

        public override void EndTree()
        {
            this.lastItemWasAtomic = false;
        }

        public override XmlRawWriter StartTree(XPathNodeType rootType, IXmlNamespaceResolver nsResolver, XmlNameTable nameTable)
        {
            if ((rootType == XPathNodeType.Attribute) || (rootType == XPathNodeType.Namespace))
            {
                throw new XslTransformException("XmlIl_TopLevelAttrNmsp", new string[] { string.Empty });
            }
            this.xwrt.NamespaceResolver = nsResolver;
            return this.xwrt;
        }

        public override void WriteItem(XPathItem item)
        {
            if (item.IsNode)
            {
                XPathNavigator nav = item as XPathNavigator;
                if ((nav.NodeType == XPathNodeType.Attribute) || (nav.NodeType == XPathNodeType.Namespace))
                {
                    throw new XslTransformException("XmlIl_TopLevelAttrNmsp", new string[] { string.Empty });
                }
                this.CopyNode(nav);
                this.lastItemWasAtomic = false;
            }
            else
            {
                this.WriteString(item.Value);
            }
        }

        private void WriteString(string value)
        {
            if (this.lastItemWasAtomic)
            {
                this.xwrt.WriteWhitespace(" ");
            }
            else
            {
                this.lastItemWasAtomic = true;
            }
            this.xwrt.WriteString(value);
        }
    }
}

