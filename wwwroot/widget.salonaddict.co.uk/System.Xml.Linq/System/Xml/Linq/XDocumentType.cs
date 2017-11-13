namespace System.Xml.Linq
{
    using System;
    using System.Xml;

    public class XDocumentType : XNode
    {
        private string internalSubset;
        private string name;
        private string publicId;
        private string systemId;

        public XDocumentType(XDocumentType other)
        {
            if (other == null)
            {
                throw new ArgumentNullException("other");
            }
            this.name = other.name;
            this.publicId = other.publicId;
            this.systemId = other.systemId;
            this.internalSubset = other.internalSubset;
        }

        internal XDocumentType(XmlReader r)
        {
            this.name = r.Name;
            this.publicId = r.GetAttribute("PUBLIC");
            this.systemId = r.GetAttribute("SYSTEM");
            this.internalSubset = r.Value;
            r.Read();
        }

        public XDocumentType(string name, string publicId, string systemId, string internalSubset)
        {
            this.name = XmlConvert.VerifyName(name);
            this.publicId = publicId;
            this.systemId = systemId;
            this.internalSubset = internalSubset;
        }

        internal override XNode CloneNode() => 
            new XDocumentType(this);

        internal override bool DeepEquals(XNode node)
        {
            XDocumentType type = node as XDocumentType;
            return ((((type != null) && (this.name == type.name)) && ((this.publicId == type.publicId) && (this.systemId == type.SystemId))) && (this.internalSubset == type.internalSubset));
        }

        internal override int GetDeepHashCode() => 
            (((this.name.GetHashCode() ^ ((this.publicId != null) ? this.publicId.GetHashCode() : 0)) ^ ((this.systemId != null) ? this.systemId.GetHashCode() : 0)) ^ ((this.internalSubset != null) ? this.internalSubset.GetHashCode() : 0));

        public override void WriteTo(XmlWriter writer)
        {
            if (writer == null)
            {
                throw new ArgumentNullException("writer");
            }
            writer.WriteDocType(this.name, this.publicId, this.systemId, this.internalSubset);
        }

        public string InternalSubset
        {
            get => 
                this.internalSubset;
            set
            {
                bool flag = base.NotifyChanging(this, XObjectChangeEventArgs.Value);
                this.internalSubset = value;
                if (flag)
                {
                    base.NotifyChanged(this, XObjectChangeEventArgs.Value);
                }
            }
        }

        public string Name
        {
            get => 
                this.name;
            set
            {
                value = XmlConvert.VerifyName(value);
                bool flag = base.NotifyChanging(this, XObjectChangeEventArgs.Name);
                this.name = value;
                if (flag)
                {
                    base.NotifyChanged(this, XObjectChangeEventArgs.Name);
                }
            }
        }

        public override XmlNodeType NodeType =>
            XmlNodeType.DocumentType;

        public string PublicId
        {
            get => 
                this.publicId;
            set
            {
                bool flag = base.NotifyChanging(this, XObjectChangeEventArgs.Value);
                this.publicId = value;
                if (flag)
                {
                    base.NotifyChanged(this, XObjectChangeEventArgs.Value);
                }
            }
        }

        public string SystemId
        {
            get => 
                this.systemId;
            set
            {
                bool flag = base.NotifyChanging(this, XObjectChangeEventArgs.Value);
                this.systemId = value;
                if (flag)
                {
                    base.NotifyChanged(this, XObjectChangeEventArgs.Value);
                }
            }
        }
    }
}

