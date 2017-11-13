namespace System.Data.Services.Client.Xml
{
    using System;
    using System.Collections.Generic;
    using System.Data.Services.Client;
    using System.Runtime.CompilerServices;
    using System.Xml;
    using System.Xml.Schema;

    internal class XmlWrappingReader : XmlReader, IXmlLineInfo
    {
        private string previousReaderBaseUri;
        private XmlReader reader;
        private IXmlLineInfo readerAsIXmlLineInfo;
        private Stack<XmlBaseState> xmlBaseStack;

        internal XmlWrappingReader(XmlReader baseReader)
        {
            this.Reader = baseReader;
        }

        public override void Close()
        {
            this.reader.Close();
        }

        internal static System.Data.Services.Client.Xml.XmlWrappingReader CreateReader(string currentBaseUri, XmlReader newReader) => 
            new System.Data.Services.Client.Xml.XmlWrappingReader(newReader) { previousReaderBaseUri = currentBaseUri };

        protected override void Dispose(bool disposing)
        {
            if (this.reader != null)
            {
                ((IDisposable) this.reader).Dispose();
            }
            base.Dispose(disposing);
        }

        public override string GetAttribute(int i) => 
            this.reader.GetAttribute(i);

        public override string GetAttribute(string name) => 
            this.reader.GetAttribute(name);

        public override string GetAttribute(string name, string namespaceURI) => 
            this.reader.GetAttribute(name, namespaceURI);

        public virtual bool HasLineInfo() => 
            ((this.readerAsIXmlLineInfo != null) && this.readerAsIXmlLineInfo.HasLineInfo());

        public override string LookupNamespace(string prefix) => 
            this.reader.LookupNamespace(prefix);

        public override void MoveToAttribute(int i)
        {
            this.reader.MoveToAttribute(i);
        }

        public override bool MoveToAttribute(string name) => 
            this.reader.MoveToAttribute(name);

        public override bool MoveToAttribute(string name, string ns) => 
            this.reader.MoveToAttribute(name, ns);

        public override bool MoveToElement() => 
            this.reader.MoveToElement();

        public override bool MoveToFirstAttribute() => 
            this.reader.MoveToFirstAttribute();

        public override bool MoveToNextAttribute() => 
            this.reader.MoveToNextAttribute();

        private void PopXmlBase()
        {
            if (((this.xmlBaseStack != null) && (this.xmlBaseStack.Count > 0)) && (this.reader.Depth == this.xmlBaseStack.Peek().Depth))
            {
                this.xmlBaseStack.Pop();
            }
        }

        public override bool Read()
        {
            if (this.reader.NodeType == XmlNodeType.EndElement)
            {
                this.PopXmlBase();
            }
            else
            {
                this.reader.MoveToElement();
                if (this.reader.IsEmptyElement)
                {
                    this.PopXmlBase();
                }
            }
            bool flag = this.reader.Read();
            if ((flag && (this.reader.NodeType == XmlNodeType.Element)) && this.reader.HasAttributes)
            {
                string attribute = this.reader.GetAttribute("xml:base");
                if (string.IsNullOrEmpty(attribute))
                {
                    return flag;
                }
                Uri requestUri = null;
                requestUri = Util.CreateUri(attribute, UriKind.RelativeOrAbsolute);
                if (this.xmlBaseStack == null)
                {
                    this.xmlBaseStack = new Stack<XmlBaseState>();
                }
                if (this.xmlBaseStack.Count > 0)
                {
                    requestUri = Util.CreateUri(this.xmlBaseStack.Peek().BaseUri, requestUri);
                }
                this.xmlBaseStack.Push(new XmlBaseState(requestUri, this.reader.Depth));
            }
            return flag;
        }

        public override bool ReadAttributeValue() => 
            this.reader.ReadAttributeValue();

        public override void ResolveEntity()
        {
            this.reader.ResolveEntity();
        }

        public override void Skip()
        {
            this.reader.Skip();
        }

        public override int AttributeCount =>
            this.reader.AttributeCount;

        public override string BaseURI
        {
            get
            {
                if ((this.xmlBaseStack != null) && (this.xmlBaseStack.Count > 0))
                {
                    return this.xmlBaseStack.Peek().BaseUri.AbsoluteUri;
                }
                if (!string.IsNullOrEmpty(this.previousReaderBaseUri))
                {
                    return this.previousReaderBaseUri;
                }
                return this.reader.BaseURI;
            }
        }

        public override bool CanResolveEntity =>
            this.reader.CanResolveEntity;

        public override int Depth =>
            this.reader.Depth;

        public override bool EOF =>
            this.reader.EOF;

        public override bool HasAttributes =>
            this.reader.HasAttributes;

        public override bool HasValue =>
            this.reader.HasValue;

        public override bool IsDefault =>
            this.reader.IsDefault;

        public override bool IsEmptyElement =>
            this.reader.IsEmptyElement;

        public virtual int LineNumber
        {
            get
            {
                if (this.readerAsIXmlLineInfo != null)
                {
                    return this.readerAsIXmlLineInfo.LineNumber;
                }
                return 0;
            }
        }

        public virtual int LinePosition
        {
            get
            {
                if (this.readerAsIXmlLineInfo != null)
                {
                    return this.readerAsIXmlLineInfo.LinePosition;
                }
                return 0;
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

        protected XmlReader Reader
        {
            get => 
                this.reader;
            set
            {
                this.reader = value;
                this.readerAsIXmlLineInfo = value as IXmlLineInfo;
            }
        }

        public override System.Xml.ReadState ReadState =>
            this.reader.ReadState;

        public override IXmlSchemaInfo SchemaInfo =>
            this.reader.SchemaInfo;

        public override XmlReaderSettings Settings =>
            this.reader.Settings;

        public override string Value =>
            this.reader.Value;

        public override Type ValueType =>
            this.reader.ValueType;

        public override string XmlLang =>
            this.reader.XmlLang;

        public override System.Xml.XmlSpace XmlSpace =>
            this.reader.XmlSpace;

        private class XmlBaseState
        {
            internal XmlBaseState(Uri baseUri, int depth)
            {
                this.BaseUri = baseUri;
                this.Depth = depth;
            }

            public Uri BaseUri { get; private set; }

            public int Depth { get; private set; }
        }
    }
}

