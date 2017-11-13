namespace System.IdentityModel
{
    using System;
    using System.IO;
    using System.Reflection;
    using System.Runtime.Remoting.Metadata.W3cXsd2001;
    using System.Xml;

    internal sealed class WrappedReader : XmlDictionaryReader, IXmlLineInfo
    {
        private TextReader contentReader;
        private MemoryStream contentStream;
        private int depth;
        private readonly XmlDictionaryReader reader;
        private bool recordDone;
        private readonly XmlTokenStream xmlTokens;

        public WrappedReader(XmlDictionaryReader reader)
        {
            if (reader == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("reader"));
            }
            if (!reader.IsStartElement())
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(System.IdentityModel.SR.GetString("InnerReaderMustBeAtElement")));
            }
            this.xmlTokens = new XmlTokenStream(0x20);
            this.reader = reader;
            this.Record();
        }

        public override void Close()
        {
            this.OnEndOfContent();
            this.reader.Close();
        }

        public override string GetAttribute(int index) => 
            this.reader.GetAttribute(index);

        public override string GetAttribute(string name) => 
            this.reader.GetAttribute(name);

        public override string GetAttribute(string name, string ns) => 
            this.reader.GetAttribute(name, ns);

        public bool HasLineInfo()
        {
            IXmlLineInfo reader = this.reader as IXmlLineInfo;
            return ((reader != null) && reader.HasLineInfo());
        }

        public override string LookupNamespace(string ns) => 
            this.reader.LookupNamespace(ns);

        public override void MoveToAttribute(int index)
        {
            this.OnEndOfContent();
            this.reader.MoveToAttribute(index);
        }

        public override bool MoveToAttribute(string name)
        {
            this.OnEndOfContent();
            return this.reader.MoveToAttribute(name);
        }

        public override bool MoveToAttribute(string name, string ns)
        {
            this.OnEndOfContent();
            return this.reader.MoveToAttribute(name, ns);
        }

        public override bool MoveToElement()
        {
            this.OnEndOfContent();
            return this.reader.MoveToElement();
        }

        public override bool MoveToFirstAttribute()
        {
            this.OnEndOfContent();
            return this.reader.MoveToFirstAttribute();
        }

        public override bool MoveToNextAttribute()
        {
            this.OnEndOfContent();
            return this.reader.MoveToNextAttribute();
        }

        private void OnEndOfContent()
        {
            if (this.contentReader != null)
            {
                this.contentReader.Close();
                this.contentReader = null;
            }
            if (this.contentStream != null)
            {
                this.contentStream.Close();
                this.contentStream = null;
            }
        }

        public override bool Read()
        {
            this.OnEndOfContent();
            if (!this.reader.Read())
            {
                return false;
            }
            if (!this.recordDone)
            {
                this.Record();
            }
            return true;
        }

        public override bool ReadAttributeValue() => 
            this.reader.ReadAttributeValue();

        private int ReadBinaryContent(byte[] buffer, int offset, int count, bool isBase64)
        {
            CryptoHelper.ValidateBufferBounds(buffer, offset, count);
            int num = 0;
            while (((count > 0) && (this.NodeType != XmlNodeType.Element)) && (this.NodeType != XmlNodeType.EndElement))
            {
                if (this.contentStream == null)
                {
                    byte[] buffer2 = isBase64 ? Convert.FromBase64String(this.Value) : SoapHexBinary.Parse(this.Value).Value;
                    this.contentStream = new MemoryStream(buffer2);
                }
                int num2 = this.contentStream.Read(buffer, offset, count);
                if ((num2 == 0) && ((this.NodeType == XmlNodeType.Attribute) || !this.Read()))
                {
                    return num;
                }
                num += num2;
                offset += num2;
                count -= num2;
            }
            return num;
        }

        public override int ReadContentAsBase64(byte[] buffer, int offset, int count) => 
            this.ReadBinaryContent(buffer, offset, count, true);

        public override int ReadContentAsBinHex(byte[] buffer, int offset, int count) => 
            this.ReadBinaryContent(buffer, offset, count, false);

        public override int ReadValueChunk(char[] chars, int offset, int count) => 
            this.contentReader?.Read(chars, offset, count);

        private void Record()
        {
            switch (this.NodeType)
            {
                case XmlNodeType.Element:
                {
                    bool isEmptyElement = this.reader.IsEmptyElement;
                    this.xmlTokens.AddElement(this.reader.Prefix, this.reader.LocalName, this.reader.NamespaceURI, isEmptyElement);
                    if (this.reader.MoveToFirstAttribute())
                    {
                        do
                        {
                            this.xmlTokens.AddAttribute(this.reader.Prefix, this.reader.LocalName, this.reader.NamespaceURI, this.reader.Value);
                        }
                        while (this.reader.MoveToNextAttribute());
                        this.reader.MoveToElement();
                    }
                    if (!isEmptyElement)
                    {
                        this.depth++;
                        return;
                    }
                    if (this.depth == 0)
                    {
                        this.recordDone = true;
                    }
                    return;
                }
                case XmlNodeType.Text:
                case XmlNodeType.CDATA:
                case XmlNodeType.EntityReference:
                case XmlNodeType.Comment:
                case XmlNodeType.Whitespace:
                case XmlNodeType.SignificantWhitespace:
                case XmlNodeType.EndEntity:
                    this.xmlTokens.Add(this.NodeType, this.Value);
                    return;

                case XmlNodeType.DocumentType:
                case XmlNodeType.XmlDeclaration:
                    return;

                case XmlNodeType.EndElement:
                    this.xmlTokens.Add(this.NodeType, this.Value);
                    if (--this.depth == 0)
                    {
                        this.recordDone = true;
                    }
                    return;
            }
            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new XmlException(System.IdentityModel.SR.GetString("UnsupportedNodeTypeInReader", new object[] { this.reader.NodeType, this.reader.Name })));
        }

        public override void ResolveEntity()
        {
            this.reader.ResolveEntity();
        }

        public override int AttributeCount =>
            this.reader.AttributeCount;

        public override string BaseURI =>
            this.reader.BaseURI;

        public override int Depth =>
            this.reader.Depth;

        public override bool EOF =>
            this.reader.EOF;

        public override bool HasValue =>
            this.reader.HasValue;

        public override bool IsDefault =>
            this.reader.IsDefault;

        public override bool IsEmptyElement =>
            this.reader.IsEmptyElement;

        public override string this[int index] =>
            this.reader[index];

        public override string this[string name] =>
            this.reader[name];

        public override string this[string name, string ns] =>
            this.reader[name, ns];

        public int LineNumber
        {
            get
            {
                IXmlLineInfo reader = this.reader as IXmlLineInfo;
                return reader?.LineNumber;
            }
        }

        public int LinePosition
        {
            get
            {
                IXmlLineInfo reader = this.reader as IXmlLineInfo;
                return reader?.LinePosition;
            }
        }

        public override string LocalName =>
            this.reader.LocalName;

        public override string Name =>
            this.reader.Name;

        public override string NamespaceURI =>
            this.reader.NamespaceURI;

        public override XmlNameTable NameTable =>
            this.reader.NameTable;

        public override XmlNodeType NodeType =>
            this.reader.NodeType;

        public override string Prefix =>
            this.reader.Prefix;

        public override char QuoteChar =>
            this.reader.QuoteChar;

        public override System.Xml.ReadState ReadState =>
            this.reader.ReadState;

        public override string Value =>
            this.reader.Value;

        public override Type ValueType =>
            this.reader.ValueType;

        public override string XmlLang =>
            this.reader.XmlLang;

        public override System.Xml.XmlSpace XmlSpace =>
            this.reader.XmlSpace;

        public XmlTokenStream XmlTokens =>
            this.xmlTokens;
    }
}

