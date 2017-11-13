namespace System.Xml
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Security.Permissions;
    using System.Text;
    using System.Xml.Schema;

    [Obsolete("Use XmlReader created by XmlReader.Create() method using appropriate XmlReaderSettings instead. http://go.microsoft.com/fwlink/?linkid=14202"), PermissionSet(SecurityAction.InheritanceDemand, Name="FullTrust")]
    public class XmlValidatingReader : XmlReader, IXmlLineInfo, IXmlNamespaceResolver
    {
        private XmlValidatingReaderImpl impl;

        public event System.Xml.Schema.ValidationEventHandler ValidationEventHandler
        {
            add
            {
                this.impl.ValidationEventHandler += value;
            }
            remove
            {
                this.impl.ValidationEventHandler -= value;
            }
        }

        public XmlValidatingReader(XmlReader reader)
        {
            this.impl = new XmlValidatingReaderImpl(reader);
            this.impl.OuterReader = this;
        }

        public XmlValidatingReader(Stream xmlFragment, XmlNodeType fragType, XmlParserContext context)
        {
            this.impl = new XmlValidatingReaderImpl(xmlFragment, fragType, context);
            this.impl.OuterReader = this;
        }

        public XmlValidatingReader(string xmlFragment, XmlNodeType fragType, XmlParserContext context)
        {
            this.impl = new XmlValidatingReaderImpl(xmlFragment, fragType, context);
            this.impl.OuterReader = this;
        }

        public override void Close()
        {
            this.impl.Close();
        }

        public override string GetAttribute(int i) => 
            this.impl.GetAttribute(i);

        public override string GetAttribute(string name) => 
            this.impl.GetAttribute(name);

        public override string GetAttribute(string localName, string namespaceURI) => 
            this.impl.GetAttribute(localName, namespaceURI);

        public bool HasLineInfo() => 
            true;

        public override string LookupNamespace(string prefix)
        {
            string str = this.impl.LookupNamespace(prefix);
            if ((str != null) && (str.Length == 0))
            {
                str = null;
            }
            return str;
        }

        public override void MoveToAttribute(int i)
        {
            this.impl.MoveToAttribute(i);
        }

        public override bool MoveToAttribute(string name) => 
            this.impl.MoveToAttribute(name);

        public override bool MoveToAttribute(string localName, string namespaceURI) => 
            this.impl.MoveToAttribute(localName, namespaceURI);

        public override bool MoveToElement() => 
            this.impl.MoveToElement();

        public override bool MoveToFirstAttribute() => 
            this.impl.MoveToFirstAttribute();

        public override bool MoveToNextAttribute() => 
            this.impl.MoveToNextAttribute();

        public override bool Read() => 
            this.impl.Read();

        public override bool ReadAttributeValue() => 
            this.impl.ReadAttributeValue();

        public override int ReadContentAsBase64(byte[] buffer, int index, int count) => 
            this.impl.ReadContentAsBase64(buffer, index, count);

        public override int ReadContentAsBinHex(byte[] buffer, int index, int count) => 
            this.impl.ReadContentAsBinHex(buffer, index, count);

        public override int ReadElementContentAsBase64(byte[] buffer, int index, int count) => 
            this.impl.ReadElementContentAsBase64(buffer, index, count);

        public override int ReadElementContentAsBinHex(byte[] buffer, int index, int count) => 
            this.impl.ReadElementContentAsBinHex(buffer, index, count);

        public override string ReadString()
        {
            this.impl.MoveOffEntityReference();
            return base.ReadString();
        }

        public object ReadTypedValue() => 
            this.impl.ReadTypedValue();

        public override void ResolveEntity()
        {
            this.impl.ResolveEntity();
        }

        IDictionary<string, string> IXmlNamespaceResolver.GetNamespacesInScope(XmlNamespaceScope scope) => 
            this.impl.GetNamespacesInScope(scope);

        string IXmlNamespaceResolver.LookupNamespace(string prefix) => 
            this.impl.LookupNamespace(prefix);

        string IXmlNamespaceResolver.LookupPrefix(string namespaceName) => 
            this.impl.LookupPrefix(namespaceName);

        public override int AttributeCount =>
            this.impl.AttributeCount;

        public override string BaseURI =>
            this.impl.BaseURI;

        public override bool CanReadBinaryContent =>
            true;

        public override bool CanResolveEntity =>
            true;

        public override int Depth =>
            this.impl.Depth;

        public System.Text.Encoding Encoding =>
            this.impl.Encoding;

        public System.Xml.EntityHandling EntityHandling
        {
            get => 
                this.impl.EntityHandling;
            set
            {
                this.impl.EntityHandling = value;
            }
        }

        public override bool EOF =>
            this.impl.EOF;

        public override bool HasValue =>
            this.impl.HasValue;

        internal XmlValidatingReaderImpl Impl =>
            this.impl;

        public override bool IsDefault =>
            this.impl.IsDefault;

        public override bool IsEmptyElement =>
            this.impl.IsEmptyElement;

        public int LineNumber =>
            this.impl.LineNumber;

        public int LinePosition =>
            this.impl.LinePosition;

        public override string LocalName =>
            this.impl.LocalName;

        public override string Name =>
            this.impl.Name;

        public bool Namespaces
        {
            get => 
                this.impl.Namespaces;
            set
            {
                this.impl.Namespaces = value;
            }
        }

        public override string NamespaceURI =>
            this.impl.NamespaceURI;

        public override XmlNameTable NameTable =>
            this.impl.NameTable;

        public override XmlNodeType NodeType =>
            this.impl.NodeType;

        public override string Prefix =>
            this.impl.Prefix;

        public override char QuoteChar =>
            this.impl.QuoteChar;

        public XmlReader Reader =>
            this.impl.Reader;

        public override System.Xml.ReadState ReadState =>
            this.impl.ReadState;

        public XmlSchemaCollection Schemas =>
            this.impl.Schemas;

        public object SchemaType =>
            this.impl.SchemaType;

        public override XmlReaderSettings Settings =>
            null;

        public System.Xml.ValidationType ValidationType
        {
            get => 
                this.impl.ValidationType;
            set
            {
                this.impl.ValidationType = value;
            }
        }

        public override string Value =>
            this.impl.Value;

        public override string XmlLang =>
            this.impl.XmlLang;

        public System.Xml.XmlResolver XmlResolver
        {
            set
            {
                this.impl.XmlResolver = value;
            }
        }

        public override System.Xml.XmlSpace XmlSpace =>
            this.impl.XmlSpace;
    }
}

