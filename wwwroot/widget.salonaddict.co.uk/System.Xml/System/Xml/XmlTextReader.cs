namespace System.Xml
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Security.Permissions;
    using System.Text;

    [PermissionSet(SecurityAction.InheritanceDemand, Name="FullTrust")]
    public class XmlTextReader : XmlReader, IXmlLineInfo, IXmlNamespaceResolver
    {
        private XmlTextReaderImpl impl;

        protected XmlTextReader()
        {
            this.impl = new XmlTextReaderImpl();
            this.impl.OuterReader = this;
        }

        public XmlTextReader(Stream input)
        {
            this.impl = new XmlTextReaderImpl(input);
            this.impl.OuterReader = this;
        }

        public XmlTextReader(TextReader input)
        {
            this.impl = new XmlTextReaderImpl(input);
            this.impl.OuterReader = this;
        }

        public XmlTextReader(string url)
        {
            this.impl = new XmlTextReaderImpl(url, new System.Xml.NameTable());
            this.impl.OuterReader = this;
        }

        protected XmlTextReader(XmlNameTable nt)
        {
            this.impl = new XmlTextReaderImpl(nt);
            this.impl.OuterReader = this;
        }

        public XmlTextReader(Stream input, XmlNameTable nt)
        {
            this.impl = new XmlTextReaderImpl(input, nt);
            this.impl.OuterReader = this;
        }

        public XmlTextReader(TextReader input, XmlNameTable nt)
        {
            this.impl = new XmlTextReaderImpl(input, nt);
            this.impl.OuterReader = this;
        }

        public XmlTextReader(string url, Stream input)
        {
            this.impl = new XmlTextReaderImpl(url, input);
            this.impl.OuterReader = this;
        }

        public XmlTextReader(string url, TextReader input)
        {
            this.impl = new XmlTextReaderImpl(url, input);
            this.impl.OuterReader = this;
        }

        public XmlTextReader(string url, XmlNameTable nt)
        {
            this.impl = new XmlTextReaderImpl(url, nt);
            this.impl.OuterReader = this;
        }

        public XmlTextReader(Stream xmlFragment, XmlNodeType fragType, XmlParserContext context)
        {
            this.impl = new XmlTextReaderImpl(xmlFragment, fragType, context);
            this.impl.OuterReader = this;
        }

        public XmlTextReader(string url, Stream input, XmlNameTable nt)
        {
            this.impl = new XmlTextReaderImpl(url, input, nt);
            this.impl.OuterReader = this;
        }

        public XmlTextReader(string url, TextReader input, XmlNameTable nt)
        {
            this.impl = new XmlTextReaderImpl(url, input, nt);
            this.impl.OuterReader = this;
        }

        public XmlTextReader(string xmlFragment, XmlNodeType fragType, XmlParserContext context)
        {
            this.impl = new XmlTextReaderImpl(xmlFragment, fragType, context);
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

        public IDictionary<string, string> GetNamespacesInScope(XmlNamespaceScope scope) => 
            this.impl.GetNamespacesInScope(scope);

        public TextReader GetRemainder() => 
            this.impl.GetRemainder();

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

        public int ReadBase64(byte[] array, int offset, int len) => 
            this.impl.ReadBase64(array, offset, len);

        public int ReadBinHex(byte[] array, int offset, int len) => 
            this.impl.ReadBinHex(array, offset, len);

        public int ReadChars(char[] buffer, int index, int count) => 
            this.impl.ReadChars(buffer, index, count);

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

        public void ResetState()
        {
            this.impl.ResetState();
        }

        public override void ResolveEntity()
        {
            this.impl.ResolveEntity();
        }

        public override void Skip()
        {
            this.impl.Skip();
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

        public override bool CanReadValueChunk =>
            false;

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

        internal XmlTextReaderImpl Impl =>
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

        internal override XmlNamespaceManager NamespaceManager =>
            this.impl.NamespaceManager;

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

        public bool Normalization
        {
            get => 
                this.impl.Normalization;
            set
            {
                this.impl.Normalization = value;
            }
        }

        public override string Prefix =>
            this.impl.Prefix;

        public bool ProhibitDtd
        {
            get => 
                this.impl.ProhibitDtd;
            set
            {
                this.impl.ProhibitDtd = value;
            }
        }

        public override char QuoteChar =>
            this.impl.QuoteChar;

        public override System.Xml.ReadState ReadState =>
            this.impl.ReadState;

        public override XmlReaderSettings Settings =>
            null;

        public override string Value =>
            this.impl.Value;

        public System.Xml.WhitespaceHandling WhitespaceHandling
        {
            get => 
                this.impl.WhitespaceHandling;
            set
            {
                this.impl.WhitespaceHandling = value;
            }
        }

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

        internal bool XmlValidatingReaderCompatibilityMode
        {
            set
            {
                this.impl.XmlValidatingReaderCompatibilityMode = value;
            }
        }
    }
}

