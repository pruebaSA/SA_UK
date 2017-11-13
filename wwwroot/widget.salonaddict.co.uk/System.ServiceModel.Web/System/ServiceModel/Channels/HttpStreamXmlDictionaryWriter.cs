namespace System.ServiceModel.Channels
{
    using System;
    using System.IO;
    using System.ServiceModel;
    using System.Xml;

    internal class HttpStreamXmlDictionaryWriter : XmlDictionaryWriter
    {
        private System.Xml.WriteState state;
        private Stream stream;

        public HttpStreamXmlDictionaryWriter(Stream stream)
        {
            if (stream == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("stream");
            }
            this.stream = stream;
            this.state = System.Xml.WriteState.Start;
        }

        public override void Close()
        {
            if (this.state != System.Xml.WriteState.Closed)
            {
                this.state = System.Xml.WriteState.Closed;
                this.stream.Close();
            }
        }

        public override void Flush()
        {
            this.stream.Flush();
        }

        public override string LookupPrefix(string ns)
        {
            if (ns == string.Empty)
            {
                return string.Empty;
            }
            if (ns == "http://www.w3.org/XML/1998/namespace")
            {
                return "xml";
            }
            if (ns == "http://www.w3.org/2000/xmlns/")
            {
                return "xmlns";
            }
            return null;
        }

        protected void ThrowIfClosed()
        {
            if (this.state == System.Xml.WriteState.Closed)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SR2.GetString(SR2.XmlWriterClosed, new object[0])));
            }
        }

        public override void WriteBase64(byte[] buffer, int index, int count)
        {
            if (buffer == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("buffer"));
            }
            if (index < 0)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("index"));
            }
            if (count < 0)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("count"));
            }
            if (count > (buffer.Length - index))
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("count", SR2.GetString(SR2.SizeExceedsRemainingBufferSpace, new object[] { buffer.Length - index })));
            }
            this.ThrowIfClosed();
            this.stream.Write(buffer, index, count);
            this.state = System.Xml.WriteState.Content;
        }

        public override void WriteCData(string text)
        {
            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException());
        }

        public override void WriteCharEntity(char ch)
        {
            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException());
        }

        public override void WriteChars(char[] buffer, int index, int count)
        {
            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException());
        }

        public override void WriteComment(string text)
        {
            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException());
        }

        public override void WriteDocType(string name, string pubid, string sysid, string subset)
        {
            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException());
        }

        public override void WriteEndAttribute()
        {
            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException());
        }

        public override void WriteEndDocument()
        {
        }

        public override void WriteEndElement()
        {
            this.ThrowIfClosed();
            if ((this.state != System.Xml.WriteState.Element) && (this.state != System.Xml.WriteState.Content))
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new XmlException(SR2.GetString(SR2.EndElementWithoutStartElement, new object[0])));
            }
        }

        public override void WriteEntityRef(string name)
        {
            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException());
        }

        public override void WriteFullEndElement()
        {
            this.ThrowIfClosed();
            if ((this.state != System.Xml.WriteState.Element) && (this.state != System.Xml.WriteState.Content))
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new XmlException(SR2.GetString(SR2.EndElementWithoutStartElement, new object[0])));
            }
        }

        public override void WriteProcessingInstruction(string name, string text)
        {
            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException());
        }

        public override void WriteRaw(string data)
        {
            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException());
        }

        public override void WriteRaw(char[] buffer, int index, int count)
        {
            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException());
        }

        public override void WriteStartAttribute(string prefix, string localName, string ns)
        {
            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException());
        }

        public override void WriteStartDocument()
        {
            this.ThrowIfClosed();
        }

        public override void WriteStartDocument(bool standalone)
        {
            this.ThrowIfClosed();
        }

        public override void WriteStartElement(string prefix, string localName, string ns)
        {
            this.ThrowIfClosed();
            if ((!string.IsNullOrEmpty(prefix) || !string.IsNullOrEmpty(ns)) || (localName != "Binary"))
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException());
            }
            this.state = System.Xml.WriteState.Element;
        }

        public override void WriteString(string text)
        {
            byte[] buffer = Convert.FromBase64String(text);
            this.WriteBase64(buffer, 0, buffer.Length);
        }

        public override void WriteSurrogateCharEntity(char lowChar, char highChar)
        {
            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException());
        }

        public override void WriteWhitespace(string ws)
        {
            if (ws == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("ws");
            }
            if (ws.Length != 0)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException());
            }
        }

        public override System.Xml.WriteState WriteState =>
            this.state;
    }
}

