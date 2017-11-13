namespace System.Xml
{
    using System;
    using System.Xml.Schema;

    internal class XmlName : IXmlSchemaInfo
    {
        private int hashCode;
        private string localName;
        private string name;
        internal XmlName next;
        private string ns;
        internal XmlDocument ownerDoc;
        private string prefix;

        internal XmlName(string prefix, string localName, string ns, int hashCode, XmlDocument ownerDoc, XmlName next)
        {
            this.prefix = prefix;
            this.localName = localName;
            this.ns = ns;
            this.name = null;
            this.hashCode = hashCode;
            this.ownerDoc = ownerDoc;
            this.next = next;
        }

        public static XmlName Create(string prefix, string localName, string ns, int hashCode, XmlDocument ownerDoc, XmlName next, IXmlSchemaInfo schemaInfo)
        {
            if (schemaInfo == null)
            {
                return new XmlName(prefix, localName, ns, hashCode, ownerDoc, next);
            }
            return new XmlNameEx(prefix, localName, ns, hashCode, ownerDoc, next, schemaInfo);
        }

        public virtual bool Equals(IXmlSchemaInfo schemaInfo) => 
            (schemaInfo == null);

        public static int GetHashCode(string name)
        {
            int num = 0;
            if (name == null)
            {
                return num;
            }
            for (int i = name.Length - 1; i >= 0; i--)
            {
                char ch = name[i];
                if (ch == ':')
                {
                    break;
                }
                num += (num << 7) ^ ch;
            }
            num -= num >> 0x11;
            num -= num >> 11;
            return (num - (num >> 5));
        }

        public int HashCode =>
            this.hashCode;

        public virtual bool IsDefault =>
            false;

        public virtual bool IsNil =>
            false;

        public string LocalName =>
            this.localName;

        public virtual XmlSchemaSimpleType MemberType =>
            null;

        public string Name
        {
            get
            {
                if (this.name == null)
                {
                    if (this.prefix.Length > 0)
                    {
                        if (this.localName.Length > 0)
                        {
                            string array = this.prefix + ":" + this.localName;
                            lock (this.ownerDoc.NameTable)
                            {
                                if (this.name == null)
                                {
                                    this.name = this.ownerDoc.NameTable.Add(array);
                                }
                                goto Label_0092;
                            }
                        }
                        this.name = this.prefix;
                    }
                    else
                    {
                        this.name = this.localName;
                    }
                }
            Label_0092:
                return this.name;
            }
        }

        public string NamespaceURI =>
            this.ns;

        public XmlDocument OwnerDocument =>
            this.ownerDoc;

        public string Prefix =>
            this.prefix;

        public virtual XmlSchemaAttribute SchemaAttribute =>
            null;

        public virtual XmlSchemaElement SchemaElement =>
            null;

        public virtual XmlSchemaType SchemaType =>
            null;

        public virtual XmlSchemaValidity Validity =>
            XmlSchemaValidity.NotKnown;
    }
}

