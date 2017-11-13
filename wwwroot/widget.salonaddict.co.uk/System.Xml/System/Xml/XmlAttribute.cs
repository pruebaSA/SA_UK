namespace System.Xml
{
    using System;
    using System.Xml.Schema;
    using System.Xml.XPath;

    public class XmlAttribute : XmlNode
    {
        private XmlLinkedNode lastChild;
        private System.Xml.XmlName name;

        internal XmlAttribute(System.Xml.XmlName name, XmlDocument doc) : base(doc)
        {
            base.parentNode = null;
            if (!doc.IsLoading)
            {
                XmlDocument.CheckName(name.Prefix);
                XmlDocument.CheckName(name.LocalName);
            }
            if (name.LocalName.Length == 0)
            {
                throw new ArgumentException(Res.GetString("Xdom_Attr_Name"));
            }
            this.name = name;
        }

        protected internal XmlAttribute(string prefix, string localName, string namespaceURI, XmlDocument doc) : this(doc.AddAttrXmlName(prefix, localName, namespaceURI, null), doc)
        {
        }

        public override XmlNode AppendChild(XmlNode newChild)
        {
            if (this.PrepareOwnerElementInElementIdAttrMap())
            {
                string innerText = this.InnerText;
                XmlNode node = base.AppendChild(newChild);
                this.ResetOwnerElementInElementIdAttrMap(innerText);
                return node;
            }
            return base.AppendChild(newChild);
        }

        internal override XmlNode AppendChildForLoad(XmlNode newChild, XmlDocument doc)
        {
            XmlNodeChangedEventArgs insertEventArgsForLoad = doc.GetInsertEventArgsForLoad(newChild, this);
            if (insertEventArgsForLoad != null)
            {
                doc.BeforeEvent(insertEventArgsForLoad);
            }
            XmlLinkedNode nextNode = (XmlLinkedNode) newChild;
            if (this.lastChild == null)
            {
                nextNode.next = nextNode;
                this.lastChild = nextNode;
                nextNode.SetParentForLoad(this);
            }
            else
            {
                XmlLinkedNode lastChild = this.lastChild;
                nextNode.next = lastChild.next;
                lastChild.next = nextNode;
                this.lastChild = nextNode;
                if (lastChild.IsText && nextNode.IsText)
                {
                    XmlNode.NestTextNodes(lastChild, nextNode);
                }
                else
                {
                    nextNode.SetParentForLoad(this);
                }
            }
            if (insertEventArgsForLoad != null)
            {
                doc.AfterEvent(insertEventArgsForLoad);
            }
            return nextNode;
        }

        public override XmlNode CloneNode(bool deep)
        {
            XmlDocument ownerDocument = this.OwnerDocument;
            XmlAttribute attribute = ownerDocument.CreateAttribute(this.Prefix, this.LocalName, this.NamespaceURI);
            attribute.CopyChildren(ownerDocument, this, true);
            return attribute;
        }

        public override XmlNode InsertAfter(XmlNode newChild, XmlNode refChild)
        {
            if (this.PrepareOwnerElementInElementIdAttrMap())
            {
                string innerText = this.InnerText;
                XmlNode node = base.InsertAfter(newChild, refChild);
                this.ResetOwnerElementInElementIdAttrMap(innerText);
                return node;
            }
            return base.InsertAfter(newChild, refChild);
        }

        public override XmlNode InsertBefore(XmlNode newChild, XmlNode refChild)
        {
            if (this.PrepareOwnerElementInElementIdAttrMap())
            {
                string innerText = this.InnerText;
                XmlNode node = base.InsertBefore(newChild, refChild);
                this.ResetOwnerElementInElementIdAttrMap(innerText);
                return node;
            }
            return base.InsertBefore(newChild, refChild);
        }

        internal override bool IsValidChildType(XmlNodeType type)
        {
            if (type != XmlNodeType.Text)
            {
                return (type == XmlNodeType.EntityReference);
            }
            return true;
        }

        internal bool PrepareOwnerElementInElementIdAttrMap()
        {
            if (this.OwnerDocument.DtdSchemaInfo != null)
            {
                XmlElement ownerElement = this.OwnerElement;
                if (ownerElement != null)
                {
                    return ownerElement.Attributes.PrepareParentInElementIdAttrMap(this.Prefix, this.LocalName);
                }
            }
            return false;
        }

        public override XmlNode PrependChild(XmlNode newChild)
        {
            if (this.PrepareOwnerElementInElementIdAttrMap())
            {
                string innerText = this.InnerText;
                XmlNode node = base.PrependChild(newChild);
                this.ResetOwnerElementInElementIdAttrMap(innerText);
                return node;
            }
            return base.PrependChild(newChild);
        }

        public override XmlNode RemoveChild(XmlNode oldChild)
        {
            if (this.PrepareOwnerElementInElementIdAttrMap())
            {
                string innerText = this.InnerText;
                XmlNode node = base.RemoveChild(oldChild);
                this.ResetOwnerElementInElementIdAttrMap(innerText);
                return node;
            }
            return base.RemoveChild(oldChild);
        }

        public override XmlNode ReplaceChild(XmlNode newChild, XmlNode oldChild)
        {
            if (this.PrepareOwnerElementInElementIdAttrMap())
            {
                string innerText = this.InnerText;
                XmlNode node = base.ReplaceChild(newChild, oldChild);
                this.ResetOwnerElementInElementIdAttrMap(innerText);
                return node;
            }
            return base.ReplaceChild(newChild, oldChild);
        }

        internal void ResetOwnerElementInElementIdAttrMap(string oldInnerText)
        {
            XmlElement ownerElement = this.OwnerElement;
            if (ownerElement != null)
            {
                ownerElement.Attributes.ResetParentInElementIdAttrMap(oldInnerText, this.InnerText);
            }
        }

        internal override void SetParent(XmlNode node)
        {
            base.parentNode = node;
        }

        public override void WriteContentTo(XmlWriter w)
        {
            for (XmlNode node = this.FirstChild; node != null; node = node.NextSibling)
            {
                node.WriteTo(w);
            }
        }

        public override void WriteTo(XmlWriter w)
        {
            w.WriteStartAttribute(this.Prefix, this.LocalName, this.NamespaceURI);
            this.WriteContentTo(w);
            w.WriteEndAttribute();
        }

        public override string BaseURI
        {
            get
            {
                if (this.OwnerElement != null)
                {
                    return this.OwnerElement.BaseURI;
                }
                return string.Empty;
            }
        }

        public override string InnerText
        {
            set
            {
                if (this.PrepareOwnerElementInElementIdAttrMap())
                {
                    string innerText = base.InnerText;
                    base.InnerText = value;
                    this.ResetOwnerElementInElementIdAttrMap(innerText);
                }
                else
                {
                    base.InnerText = value;
                }
            }
        }

        public override string InnerXml
        {
            set
            {
                this.RemoveAll();
                new XmlLoader().LoadInnerXmlAttribute(this, value);
            }
        }

        internal override bool IsContainer =>
            true;

        internal bool IsNamespace =>
            Ref.Equal(this.name.NamespaceURI, this.name.OwnerDocument.strReservedXmlns);

        internal override XmlLinkedNode LastNode
        {
            get => 
                this.lastChild;
            set
            {
                this.lastChild = value;
            }
        }

        public override string LocalName =>
            this.name.LocalName;

        internal int LocalNameHash =>
            this.name.HashCode;

        public override string Name =>
            this.name.Name;

        public override string NamespaceURI =>
            this.name.NamespaceURI;

        public override XmlNodeType NodeType =>
            XmlNodeType.Attribute;

        public override XmlDocument OwnerDocument =>
            this.name.OwnerDocument;

        public virtual XmlElement OwnerElement =>
            (base.parentNode as XmlElement);

        public override XmlNode ParentNode =>
            null;

        public override string Prefix
        {
            get => 
                this.name.Prefix;
            set
            {
                this.name = this.name.OwnerDocument.AddAttrXmlName(value, this.LocalName, this.NamespaceURI, this.SchemaInfo);
            }
        }

        public override IXmlSchemaInfo SchemaInfo =>
            this.name;

        public virtual bool Specified =>
            true;

        public override string Value
        {
            get => 
                this.InnerText;
            set
            {
                this.InnerText = value;
            }
        }

        internal override string XmlLang
        {
            get
            {
                if (this.OwnerElement != null)
                {
                    return this.OwnerElement.XmlLang;
                }
                return string.Empty;
            }
        }

        internal System.Xml.XmlName XmlName
        {
            get => 
                this.name;
            set
            {
                this.name = value;
            }
        }

        internal override System.Xml.XmlSpace XmlSpace
        {
            get
            {
                if (this.OwnerElement != null)
                {
                    return this.OwnerElement.XmlSpace;
                }
                return System.Xml.XmlSpace.None;
            }
        }

        internal override string XPLocalName
        {
            get
            {
                string localName = this.name.LocalName;
                if (localName == this.OwnerDocument.strXmlns)
                {
                    return string.Empty;
                }
                return localName;
            }
        }

        internal override XPathNodeType XPNodeType
        {
            get
            {
                if (this.IsNamespace)
                {
                    return XPathNodeType.Namespace;
                }
                return XPathNodeType.Attribute;
            }
        }
    }
}

