namespace System.Data
{
    using System;
    using System.Xml;

    internal sealed class DataTextReader : XmlReader
    {
        private XmlReader _xmlreader;

        private DataTextReader(XmlReader input)
        {
            this._xmlreader = input;
        }

        public override void Close()
        {
            this._xmlreader.Close();
        }

        internal static XmlReader CreateReader(XmlReader xr) => 
            new DataTextReader(xr);

        public override string GetAttribute(int i) => 
            this._xmlreader.GetAttribute(i);

        public override string GetAttribute(string name) => 
            this._xmlreader.GetAttribute(name);

        public override string GetAttribute(string localName, string namespaceURI) => 
            this._xmlreader.GetAttribute(localName, namespaceURI);

        public override string LookupNamespace(string prefix) => 
            this._xmlreader.LookupNamespace(prefix);

        public override void MoveToAttribute(int i)
        {
            this._xmlreader.MoveToAttribute(i);
        }

        public override bool MoveToAttribute(string name) => 
            this._xmlreader.MoveToAttribute(name);

        public override bool MoveToAttribute(string localName, string namespaceURI) => 
            this._xmlreader.MoveToAttribute(localName, namespaceURI);

        public override bool MoveToElement() => 
            this._xmlreader.MoveToElement();

        public override bool MoveToFirstAttribute() => 
            this._xmlreader.MoveToFirstAttribute();

        public override bool MoveToNextAttribute() => 
            this._xmlreader.MoveToNextAttribute();

        public override bool Read() => 
            this._xmlreader.Read();

        public override bool ReadAttributeValue() => 
            this._xmlreader.ReadAttributeValue();

        public override int ReadContentAsBase64(byte[] buffer, int index, int count) => 
            this._xmlreader.ReadContentAsBase64(buffer, index, count);

        public override int ReadContentAsBinHex(byte[] buffer, int index, int count) => 
            this._xmlreader.ReadContentAsBinHex(buffer, index, count);

        public override int ReadElementContentAsBase64(byte[] buffer, int index, int count) => 
            this._xmlreader.ReadElementContentAsBase64(buffer, index, count);

        public override int ReadElementContentAsBinHex(byte[] buffer, int index, int count) => 
            this._xmlreader.ReadElementContentAsBinHex(buffer, index, count);

        public override string ReadString() => 
            this._xmlreader.ReadString();

        public override void ResolveEntity()
        {
            this._xmlreader.ResolveEntity();
        }

        public override void Skip()
        {
            this._xmlreader.Skip();
        }

        public override int AttributeCount =>
            this._xmlreader.AttributeCount;

        public override string BaseURI =>
            this._xmlreader.BaseURI;

        public override bool CanReadBinaryContent =>
            this._xmlreader.CanReadBinaryContent;

        public override bool CanReadValueChunk =>
            this._xmlreader.CanReadValueChunk;

        public override bool CanResolveEntity =>
            this._xmlreader.CanResolveEntity;

        public override int Depth =>
            this._xmlreader.Depth;

        public override bool EOF =>
            this._xmlreader.EOF;

        public override bool HasValue =>
            this._xmlreader.HasValue;

        public override bool IsDefault =>
            this._xmlreader.IsDefault;

        public override bool IsEmptyElement =>
            this._xmlreader.IsEmptyElement;

        public override string LocalName =>
            this._xmlreader.LocalName;

        public override string Name =>
            this._xmlreader.Name;

        public override string NamespaceURI =>
            this._xmlreader.NamespaceURI;

        public override XmlNameTable NameTable =>
            this._xmlreader.NameTable;

        public override XmlNodeType NodeType =>
            this._xmlreader.NodeType;

        public override string Prefix =>
            this._xmlreader.Prefix;

        public override char QuoteChar =>
            this._xmlreader.QuoteChar;

        public override System.Xml.ReadState ReadState =>
            this._xmlreader.ReadState;

        public override XmlReaderSettings Settings =>
            this._xmlreader.Settings;

        public override string Value =>
            this._xmlreader.Value;

        public override string XmlLang =>
            this._xmlreader.XmlLang;

        public override System.Xml.XmlSpace XmlSpace =>
            this._xmlreader.XmlSpace;
    }
}

