namespace System.Xml
{
    using System;
    using System.Xml.XPath;

    internal sealed class DataDocumentXPathNavigator : XPathNavigator, IHasXmlNode
    {
        private XPathNodePointer _curNode;
        private XmlDataDocument _doc;
        private XPathNodePointer _temp;

        private DataDocumentXPathNavigator(DataDocumentXPathNavigator other)
        {
            this._curNode = other._curNode.Clone(this);
            this._temp = other._temp.Clone(this);
            this._doc = other._doc;
        }

        internal DataDocumentXPathNavigator(XmlDataDocument doc, XmlNode node)
        {
            this._curNode = new XPathNodePointer(this, doc, node);
            this._temp = new XPathNodePointer(this, doc, node);
            this._doc = doc;
        }

        public override XPathNavigator Clone() => 
            new DataDocumentXPathNavigator(this);

        public override XmlNodeOrder ComparePosition(XPathNavigator other)
        {
            if (other != null)
            {
                DataDocumentXPathNavigator navigator = other as DataDocumentXPathNavigator;
                if ((navigator != null) && (navigator.Document == this._doc))
                {
                    return this._curNode.ComparePosition(navigator.CurNode);
                }
            }
            return XmlNodeOrder.Unknown;
        }

        public override string GetAttribute(string localName, string namespaceURI)
        {
            if (this._curNode.NodeType == XPathNodeType.Element)
            {
                this._temp.MoveTo(this._curNode);
                if (this._temp.MoveToAttribute(localName, namespaceURI))
                {
                    return this._temp.Value;
                }
            }
            return string.Empty;
        }

        public override string GetNamespace(string name) => 
            this._curNode.GetNamespace(name);

        public override bool IsSamePosition(XPathNavigator other)
        {
            if (other == null)
            {
                return false;
            }
            DataDocumentXPathNavigator navigator = other as DataDocumentXPathNavigator;
            return (((navigator != null) && (this._doc == navigator.Document)) && this._curNode.IsSamePosition(navigator.CurNode));
        }

        public override bool MoveTo(XPathNavigator other)
        {
            if (other != null)
            {
                DataDocumentXPathNavigator navigator = other as DataDocumentXPathNavigator;
                if (navigator == null)
                {
                    return false;
                }
                if (this._curNode.MoveTo(navigator.CurNode))
                {
                    this._doc = this._curNode.Document;
                    return true;
                }
            }
            return false;
        }

        public override bool MoveToAttribute(string localName, string namespaceURI)
        {
            if (this._curNode.NodeType != XPathNodeType.Element)
            {
                return false;
            }
            return this._curNode.MoveToAttribute(localName, namespaceURI);
        }

        public override bool MoveToFirst()
        {
            if (this._curNode.NodeType == XPathNodeType.Attribute)
            {
                return false;
            }
            return this._curNode.MoveToFirst();
        }

        public override bool MoveToFirstAttribute()
        {
            if (this._curNode.NodeType != XPathNodeType.Element)
            {
                return false;
            }
            return this._curNode.MoveToNextAttribute(true);
        }

        public override bool MoveToFirstChild() => 
            this._curNode.MoveToFirstChild();

        public override bool MoveToFirstNamespace(XPathNamespaceScope namespaceScope)
        {
            if (this._curNode.NodeType != XPathNodeType.Element)
            {
                return false;
            }
            return this._curNode.MoveToFirstNamespace(namespaceScope);
        }

        public override bool MoveToId(string id) => 
            false;

        public override bool MoveToNamespace(string name)
        {
            if (this._curNode.NodeType != XPathNodeType.Element)
            {
                return false;
            }
            return this._curNode.MoveToNamespace(name);
        }

        public override bool MoveToNext()
        {
            if (this._curNode.NodeType == XPathNodeType.Attribute)
            {
                return false;
            }
            return this._curNode.MoveToNextSibling();
        }

        public override bool MoveToNextAttribute()
        {
            if (this._curNode.NodeType != XPathNodeType.Attribute)
            {
                return false;
            }
            return this._curNode.MoveToNextAttribute(false);
        }

        public override bool MoveToNextNamespace(XPathNamespaceScope namespaceScope)
        {
            if (this._curNode.NodeType != XPathNodeType.Namespace)
            {
                return false;
            }
            return this._curNode.MoveToNextNamespace(namespaceScope);
        }

        public override bool MoveToParent() => 
            this._curNode.MoveToParent();

        public override bool MoveToPrevious()
        {
            if (this._curNode.NodeType == XPathNodeType.Attribute)
            {
                return false;
            }
            return this._curNode.MoveToPreviousSibling();
        }

        public override void MoveToRoot()
        {
            this._curNode.MoveToRoot();
        }

        XmlNode IHasXmlNode.GetNode() => 
            this._curNode.Node;

        public override string BaseURI =>
            this._curNode.BaseURI;

        internal XPathNodePointer CurNode =>
            this._curNode;

        internal XmlDataDocument Document =>
            this._doc;

        public override bool HasAttributes =>
            (this._curNode.AttributeCount > 0);

        public override bool HasChildren =>
            this._curNode.HasChildren;

        public override bool IsEmptyElement =>
            this._curNode.IsEmptyElement;

        public override string LocalName =>
            this._curNode.LocalName;

        public override string Name =>
            this._curNode.Name;

        public override string NamespaceURI =>
            this._curNode.NamespaceURI;

        public override XmlNameTable NameTable =>
            this._doc.NameTable;

        public override XPathNodeType NodeType =>
            this._curNode.NodeType;

        public override string Prefix =>
            this._curNode.Prefix;

        public override string Value
        {
            get
            {
                XPathNodeType nodeType = this._curNode.NodeType;
                if ((nodeType != XPathNodeType.Element) && (nodeType != XPathNodeType.Root))
                {
                    return this._curNode.Value;
                }
                return this._curNode.InnerText;
            }
        }

        public override string XmlLang =>
            this._curNode.XmlLang;
    }
}

