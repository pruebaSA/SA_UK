namespace System.ServiceModel.Channels
{
    using System;
    using System.IO;
    using System.ServiceModel;
    using System.Xml;

    internal class HttpStreamXmlDictionaryReader : XmlDictionaryReader
    {
        private string base64StringValue;
        private const int InitialBufferSize = 0x400;
        private bool isStreamClosed;
        private System.Xml.NameTable nameTable;
        private StreamPosition position;
        private XmlDictionaryReaderQuotas quotas;
        private bool readBase64AsString;
        private Stream stream;

        public HttpStreamXmlDictionaryReader(Stream stream, XmlDictionaryReaderQuotas quotas)
        {
            if (stream == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("stream");
            }
            this.stream = stream;
            this.position = StreamPosition.None;
            if (quotas == null)
            {
                quotas = XmlDictionaryReaderQuotas.Max;
            }
            this.quotas = quotas;
        }

        public override void Close()
        {
            if (!this.isStreamClosed)
            {
                try
                {
                    this.stream.Close();
                }
                finally
                {
                    this.position = StreamPosition.EOF;
                    this.isStreamClosed = true;
                }
            }
        }

        private void EnsureInStream()
        {
            if (this.position != StreamPosition.Stream)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new XmlException(SR2.GetString(SR2.ReaderNotPositionedAtByteStream, new object[0])));
            }
        }

        public override string GetAttribute(int i)
        {
            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("i"));
        }

        public override string GetAttribute(string name) => 
            null;

        public override string GetAttribute(string name, string namespaceURI) => 
            null;

        private string GetStreamAsBase64String()
        {
            if (!this.readBase64AsString)
            {
                this.base64StringValue = Convert.ToBase64String(this.ReadContentAsBase64());
                this.readBase64AsString = true;
            }
            return this.base64StringValue;
        }

        public override string LookupNamespace(string prefix)
        {
            if (prefix == string.Empty)
            {
                return string.Empty;
            }
            if (prefix == "xml")
            {
                return "http://www.w3.org/XML/1998/namespace";
            }
            if (prefix == "xmlns")
            {
                return "http://www.w3.org/2000/xmlns/";
            }
            return null;
        }

        public override bool MoveToAttribute(string name)
        {
            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new XmlException(SR2.GetString(SR2.CannotMoveToAttribute1, new object[] { name })));
        }

        public override bool MoveToAttribute(string name, string ns)
        {
            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new XmlException(SR2.GetString(SR2.CannotMoveToAttribute2, new object[] { name, ns })));
        }

        public override bool MoveToElement()
        {
            if (this.position == StreamPosition.None)
            {
                this.position = StreamPosition.StartElement;
                return true;
            }
            return false;
        }

        public override bool MoveToFirstAttribute() => 
            false;

        public override bool MoveToNextAttribute() => 
            false;

        public override bool Read()
        {
            switch (this.position)
            {
                case StreamPosition.None:
                    this.position = StreamPosition.StartElement;
                    return true;

                case StreamPosition.StartElement:
                    this.position = StreamPosition.Stream;
                    return true;

                case StreamPosition.Stream:
                    this.position = StreamPosition.EndElement;
                    return true;

                case StreamPosition.EndElement:
                    this.position = StreamPosition.EOF;
                    return false;

                case StreamPosition.EOF:
                    return false;
            }
            return false;
        }

        public override bool ReadAttributeValue() => 
            false;

        public override int ReadContentAsBase64(byte[] buffer, int index, int count)
        {
            if (buffer == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("buffer");
            }
            if (index < 0)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("index"));
            }
            if (count < 0)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("count"));
            }
            this.EnsureInStream();
            int num = this.stream.Read(buffer, index, count);
            if (num == 0)
            {
                this.position = StreamPosition.EndElement;
            }
            return num;
        }

        public override int ReadContentAsBinHex(byte[] buffer, int index, int count)
        {
            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException());
        }

        public override void ResolveEntity()
        {
            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException());
        }

        public override int AttributeCount =>
            0;

        public override string BaseURI =>
            string.Empty;

        public override bool CanCanonicalize =>
            false;

        public override bool CanReadBinaryContent =>
            true;

        public override bool CanReadValueChunk =>
            false;

        public override bool CanResolveEntity =>
            false;

        public override int Depth
        {
            get
            {
                if (this.position != StreamPosition.Stream)
                {
                    return 0;
                }
                return 1;
            }
        }

        public override bool EOF =>
            (this.position == StreamPosition.EOF);

        public override bool HasAttributes =>
            false;

        public override bool HasValue =>
            (this.position == StreamPosition.Stream);

        public override bool IsDefault =>
            false;

        public override bool IsEmptyElement =>
            false;

        public override string LocalName
        {
            get
            {
                if (this.position != StreamPosition.StartElement)
                {
                    return null;
                }
                return "Binary";
            }
        }

        public override string NamespaceURI =>
            string.Empty;

        public override XmlNameTable NameTable
        {
            get
            {
                if (this.nameTable == null)
                {
                    this.nameTable = new System.Xml.NameTable();
                    this.nameTable.Add("Binary");
                }
                return this.nameTable;
            }
        }

        public override XmlNodeType NodeType
        {
            get
            {
                switch (this.position)
                {
                    case StreamPosition.StartElement:
                        return XmlNodeType.Element;

                    case StreamPosition.Stream:
                        return XmlNodeType.Text;

                    case StreamPosition.EndElement:
                        return XmlNodeType.EndElement;

                    case StreamPosition.EOF:
                        return XmlNodeType.None;
                }
                return XmlNodeType.None;
            }
        }

        public override string Prefix =>
            string.Empty;

        public override XmlDictionaryReaderQuotas Quotas =>
            this.quotas;

        public override System.Xml.ReadState ReadState
        {
            get
            {
                switch (this.position)
                {
                    case StreamPosition.None:
                        return System.Xml.ReadState.Initial;

                    case StreamPosition.StartElement:
                    case StreamPosition.Stream:
                    case StreamPosition.EndElement:
                        return System.Xml.ReadState.Interactive;

                    case StreamPosition.EOF:
                        return System.Xml.ReadState.Closed;
                }
                return System.Xml.ReadState.Error;
            }
        }

        public override string Value
        {
            get
            {
                if (this.position == StreamPosition.Stream)
                {
                    return this.GetStreamAsBase64String();
                }
                return string.Empty;
            }
        }

        private enum StreamPosition
        {
            None,
            StartElement,
            Stream,
            EndElement,
            EOF
        }
    }
}

