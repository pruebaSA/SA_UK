namespace System.Runtime.Serialization
{
    using System;
    using System.Reflection;
    using System.Xml;
    using System.Xml.Schema;
    using System.Xml.Serialization;

    internal class XmlSerializableReader : XmlReader, IXmlLineInfo, IXmlTextParser
    {
        private XmlReader innerReader;
        private bool isRootEmptyElement;
        private int startDepth;
        private XmlReaderDelegator xmlReader;

        internal void BeginRead(XmlReaderDelegator xmlReader)
        {
            if (xmlReader.NodeType != XmlNodeType.Element)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlObjectSerializerReadContext.CreateUnexpectedStateException(XmlNodeType.Element, xmlReader));
            }
            this.xmlReader = xmlReader;
            this.startDepth = xmlReader.Depth;
            this.innerReader = xmlReader.UnderlyingReader;
            this.isRootEmptyElement = this.InnerReader.IsEmptyElement;
        }

        public override void Close()
        {
            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlObjectSerializer.CreateSerializationException(System.Runtime.Serialization.SR.GetString("IXmlSerializableIllegalOperation")));
        }

        internal void EndRead()
        {
            if (this.isRootEmptyElement)
            {
                this.xmlReader.Read();
            }
            else
            {
                if (this.xmlReader.IsStartElement() && (this.xmlReader.Depth == this.startDepth))
                {
                    this.xmlReader.Read();
                }
                while (this.xmlReader.Depth > this.startDepth)
                {
                    if (!this.xmlReader.Read())
                    {
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(XmlObjectSerializerReadContext.CreateUnexpectedStateException(XmlNodeType.EndElement, this.xmlReader));
                    }
                }
            }
        }

        public override string GetAttribute(int i) => 
            this.InnerReader.GetAttribute(i);

        public override string GetAttribute(string name) => 
            this.InnerReader.GetAttribute(name);

        public override string GetAttribute(string name, string namespaceURI) => 
            this.InnerReader.GetAttribute(name, namespaceURI);

        public override bool IsStartElement() => 
            this.InnerReader.IsStartElement();

        public override bool IsStartElement(string name) => 
            this.InnerReader.IsStartElement(name);

        public override bool IsStartElement(string localname, string ns) => 
            this.InnerReader.IsStartElement(localname, ns);

        public override string LookupNamespace(string prefix) => 
            this.InnerReader.LookupNamespace(prefix);

        public override void MoveToAttribute(int i)
        {
            this.InnerReader.MoveToAttribute(i);
        }

        public override bool MoveToAttribute(string name) => 
            this.InnerReader.MoveToAttribute(name);

        public override bool MoveToAttribute(string name, string ns) => 
            this.InnerReader.MoveToAttribute(name, ns);

        public override XmlNodeType MoveToContent() => 
            this.InnerReader.MoveToContent();

        public override bool MoveToElement() => 
            this.InnerReader.MoveToElement();

        public override bool MoveToFirstAttribute() => 
            this.InnerReader.MoveToFirstAttribute();

        public override bool MoveToNextAttribute() => 
            this.InnerReader.MoveToNextAttribute();

        public override bool Read()
        {
            XmlReader innerReader = this.InnerReader;
            return (((innerReader.Depth != this.startDepth) || ((innerReader.NodeType != XmlNodeType.EndElement) && ((innerReader.NodeType != XmlNodeType.Element) || !innerReader.IsEmptyElement))) && innerReader.Read());
        }

        public override bool ReadAttributeValue() => 
            this.InnerReader.ReadAttributeValue();

        public override object ReadContentAs(Type returnType, IXmlNamespaceResolver namespaceResolver) => 
            this.InnerReader.ReadContentAs(returnType, namespaceResolver);

        public override int ReadContentAsBase64(byte[] buffer, int index, int count) => 
            this.InnerReader.ReadContentAsBase64(buffer, index, count);

        public override int ReadContentAsBinHex(byte[] buffer, int index, int count) => 
            this.InnerReader.ReadContentAsBinHex(buffer, index, count);

        public override bool ReadContentAsBoolean() => 
            this.InnerReader.ReadContentAsBoolean();

        public override DateTime ReadContentAsDateTime() => 
            this.InnerReader.ReadContentAsDateTime();

        public override double ReadContentAsDouble() => 
            this.InnerReader.ReadContentAsDouble();

        public override int ReadContentAsInt() => 
            this.InnerReader.ReadContentAsInt();

        public override long ReadContentAsLong() => 
            this.InnerReader.ReadContentAsLong();

        public override object ReadContentAsObject() => 
            this.InnerReader.ReadContentAsObject();

        public override string ReadContentAsString() => 
            this.InnerReader.ReadContentAsString();

        public override string ReadString() => 
            this.InnerReader.ReadString();

        public override int ReadValueChunk(char[] buffer, int index, int count) => 
            this.InnerReader.ReadValueChunk(buffer, index, count);

        public override void ResolveEntity()
        {
            this.InnerReader.ResolveEntity();
        }

        bool IXmlLineInfo.HasLineInfo()
        {
            IXmlLineInfo innerReader = this.InnerReader as IXmlLineInfo;
            if (innerReader != null)
            {
                return innerReader.HasLineInfo();
            }
            return this.xmlReader.HasLineInfo();
        }

        public override int AttributeCount =>
            this.InnerReader.AttributeCount;

        public override string BaseURI =>
            this.InnerReader.BaseURI;

        public override bool CanReadBinaryContent =>
            this.InnerReader.CanReadBinaryContent;

        public override bool CanReadValueChunk =>
            this.InnerReader.CanReadValueChunk;

        public override bool CanResolveEntity =>
            this.InnerReader.CanResolveEntity;

        public override int Depth =>
            this.InnerReader.Depth;

        public override bool EOF =>
            this.InnerReader.EOF;

        public override bool HasAttributes =>
            this.InnerReader.HasAttributes;

        public override bool HasValue =>
            this.InnerReader.HasValue;

        private XmlReader InnerReader =>
            this.innerReader;

        public override bool IsDefault =>
            this.InnerReader.IsDefault;

        public override bool IsEmptyElement =>
            this.InnerReader.IsEmptyElement;

        public override string this[int i] =>
            this.InnerReader[i];

        public override string this[string name] =>
            this.InnerReader[name];

        public override string this[string name, string namespaceURI] =>
            this.InnerReader[name, namespaceURI];

        public override string LocalName =>
            this.InnerReader.LocalName;

        public override string Name =>
            this.InnerReader.Name;

        public override string NamespaceURI =>
            this.InnerReader.NamespaceURI;

        public override XmlNameTable NameTable =>
            this.InnerReader.NameTable;

        public override XmlNodeType NodeType =>
            this.InnerReader.NodeType;

        public override string Prefix =>
            this.InnerReader.Prefix;

        public override char QuoteChar =>
            this.InnerReader.QuoteChar;

        public override System.Xml.ReadState ReadState =>
            this.InnerReader.ReadState;

        public override IXmlSchemaInfo SchemaInfo =>
            this.InnerReader.SchemaInfo;

        public override XmlReaderSettings Settings =>
            this.InnerReader.Settings;

        int IXmlLineInfo.LineNumber
        {
            get
            {
                IXmlLineInfo innerReader = this.InnerReader as IXmlLineInfo;
                if (innerReader != null)
                {
                    return innerReader.LineNumber;
                }
                return this.xmlReader.LineNumber;
            }
        }

        int IXmlLineInfo.LinePosition
        {
            get
            {
                IXmlLineInfo innerReader = this.InnerReader as IXmlLineInfo;
                if (innerReader != null)
                {
                    return innerReader.LinePosition;
                }
                return this.xmlReader.LinePosition;
            }
        }

        bool IXmlTextParser.Normalized
        {
            get
            {
                IXmlTextParser innerReader = this.InnerReader as IXmlTextParser;
                if (innerReader != null)
                {
                    return innerReader.Normalized;
                }
                return this.xmlReader.Normalized;
            }
            set
            {
                IXmlTextParser innerReader = this.InnerReader as IXmlTextParser;
                if (innerReader == null)
                {
                    this.xmlReader.Normalized = value;
                }
                else
                {
                    innerReader.Normalized = value;
                }
            }
        }

        WhitespaceHandling IXmlTextParser.WhitespaceHandling
        {
            get
            {
                IXmlTextParser innerReader = this.InnerReader as IXmlTextParser;
                if (innerReader != null)
                {
                    return innerReader.WhitespaceHandling;
                }
                return this.xmlReader.WhitespaceHandling;
            }
            set
            {
                IXmlTextParser innerReader = this.InnerReader as IXmlTextParser;
                if (innerReader == null)
                {
                    this.xmlReader.WhitespaceHandling = value;
                }
                else
                {
                    innerReader.WhitespaceHandling = value;
                }
            }
        }

        public override string Value =>
            this.InnerReader.Value;

        public override Type ValueType =>
            this.InnerReader.ValueType;

        public override string XmlLang =>
            this.InnerReader.XmlLang;

        public override System.Xml.XmlSpace XmlSpace =>
            this.InnerReader.XmlSpace;
    }
}

