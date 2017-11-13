namespace System.Windows.Markup
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Xml;
    using System.Xml.Schema;

    internal class XmlWrappingReader : XmlReader, IXmlLineInfo, IXmlNamespaceResolver
    {
        protected XmlReader _reader;
        protected IXmlLineInfo _readerAsIXmlLineInfo;
        protected IXmlNamespaceResolver _readerAsResolver;

        internal XmlWrappingReader(XmlReader baseReader)
        {
            this.Reader = baseReader;
        }

        public override void Close()
        {
            this._reader.Close();
        }

        protected override void Dispose(bool disposing)
        {
            try
            {
                if (disposing)
                {
                    ((IDisposable) this._reader).Dispose();
                }
            }
            finally
            {
                base.Dispose(disposing);
            }
        }

        public override string GetAttribute(int i) => 
            this._reader.GetAttribute(i);

        public override string GetAttribute(string name) => 
            this._reader.GetAttribute(name);

        public override string GetAttribute(string name, string namespaceURI) => 
            this._reader.GetAttribute(name, namespaceURI);

        public virtual bool HasLineInfo() => 
            ((this._readerAsIXmlLineInfo != null) && this._readerAsIXmlLineInfo.HasLineInfo());

        public override string LookupNamespace(string prefix) => 
            this._reader.LookupNamespace(prefix);

        public override void MoveToAttribute(int i)
        {
            this._reader.MoveToAttribute(i);
        }

        public override bool MoveToAttribute(string name) => 
            this._reader.MoveToAttribute(name);

        public override bool MoveToAttribute(string name, string ns) => 
            this._reader.MoveToAttribute(name, ns);

        public override bool MoveToElement() => 
            this._reader.MoveToElement();

        public override bool MoveToFirstAttribute() => 
            this._reader.MoveToFirstAttribute();

        public override bool MoveToNextAttribute() => 
            this._reader.MoveToNextAttribute();

        public override bool Read() => 
            this._reader.Read();

        public override bool ReadAttributeValue() => 
            this._reader.ReadAttributeValue();

        public override void ResolveEntity()
        {
            this._reader.ResolveEntity();
        }

        public override void Skip()
        {
            this._reader.Skip();
        }

        IDictionary<string, string> IXmlNamespaceResolver.GetNamespacesInScope(XmlNamespaceScope scope)
        {
            if (this._readerAsResolver != null)
            {
                return this._readerAsResolver.GetNamespacesInScope(scope);
            }
            return null;
        }

        string IXmlNamespaceResolver.LookupPrefix(string namespaceName)
        {
            if (this._readerAsResolver != null)
            {
                return this._readerAsResolver.LookupPrefix(namespaceName);
            }
            return null;
        }

        public override int AttributeCount =>
            this._reader.AttributeCount;

        public override string BaseURI =>
            this._reader.BaseURI;

        public override bool CanResolveEntity =>
            this._reader.CanResolveEntity;

        public override int Depth =>
            this._reader.Depth;

        public override bool EOF =>
            this._reader.EOF;

        public override bool HasAttributes =>
            this._reader.HasAttributes;

        public override bool HasValue =>
            this._reader.HasValue;

        public override bool IsDefault =>
            this._reader.IsDefault;

        public override bool IsEmptyElement =>
            this._reader.IsEmptyElement;

        public override string this[int i] =>
            this._reader[i];

        public override string this[string name] =>
            this._reader[name];

        public override string this[string name, string namespaceURI] =>
            this._reader[name, namespaceURI];

        public virtual int LineNumber
        {
            get
            {
                if (this._readerAsIXmlLineInfo != null)
                {
                    return this._readerAsIXmlLineInfo.LineNumber;
                }
                return 0;
            }
        }

        public virtual int LinePosition
        {
            get
            {
                if (this._readerAsIXmlLineInfo != null)
                {
                    return this._readerAsIXmlLineInfo.LinePosition;
                }
                return 0;
            }
        }

        public override string LocalName =>
            this._reader.LocalName;

        public override string Name =>
            this._reader.Name;

        public override string NamespaceURI =>
            this._reader.NamespaceURI;

        public override XmlNameTable NameTable =>
            this._reader.NameTable;

        public override XmlNodeType NodeType =>
            this._reader.NodeType;

        public override string Prefix =>
            this._reader.Prefix;

        public override char QuoteChar =>
            this._reader.QuoteChar;

        protected XmlReader Reader
        {
            get => 
                this._reader;
            set
            {
                this._reader = value;
                this._readerAsIXmlLineInfo = value as IXmlLineInfo;
                this._readerAsResolver = value as IXmlNamespaceResolver;
            }
        }

        public override System.Xml.ReadState ReadState =>
            this._reader.ReadState;

        public override IXmlSchemaInfo SchemaInfo =>
            this._reader.SchemaInfo;

        public override XmlReaderSettings Settings =>
            this._reader.Settings;

        public override string Value =>
            this._reader.Value;

        public override Type ValueType =>
            this._reader.ValueType;

        public override string XmlLang =>
            this._reader.XmlLang;

        public override System.Xml.XmlSpace XmlSpace =>
            this._reader.XmlSpace;
    }
}

