namespace System.Xml.Xsl.Runtime
{
    using System;
    using System.Xml;

    internal sealed class XmlRawWriterWrapper : XmlRawWriter
    {
        private XmlWriter wrapped;

        public XmlRawWriterWrapper(XmlWriter writer)
        {
            this.wrapped = writer;
        }

        public override void Close()
        {
            this.wrapped.Close();
        }

        protected override void Dispose(bool disposing)
        {
            try
            {
                ((IDisposable) this.wrapped).Dispose();
            }
            finally
            {
                base.Dispose(disposing);
            }
        }

        public override void Flush()
        {
            this.wrapped.Flush();
        }

        internal override void StartElementContent()
        {
        }

        public override void WriteCData(string text)
        {
            this.wrapped.WriteCData(text);
        }

        public override void WriteCharEntity(char ch)
        {
            this.wrapped.WriteCharEntity(ch);
        }

        public override void WriteChars(char[] buffer, int index, int count)
        {
            this.wrapped.WriteChars(buffer, index, count);
        }

        public override void WriteComment(string text)
        {
            this.wrapped.WriteComment(text);
        }

        public override void WriteDocType(string name, string pubid, string sysid, string subset)
        {
            this.wrapped.WriteDocType(name, pubid, sysid, subset);
        }

        public override void WriteEndAttribute()
        {
            this.wrapped.WriteEndAttribute();
        }

        internal override void WriteEndElement(string prefix, string localName, string ns)
        {
            this.wrapped.WriteEndElement();
        }

        public override void WriteEntityRef(string name)
        {
            this.wrapped.WriteEntityRef(name);
        }

        internal override void WriteFullEndElement(string prefix, string localName, string ns)
        {
            this.wrapped.WriteFullEndElement();
        }

        internal override void WriteNamespaceDeclaration(string prefix, string ns)
        {
            if (prefix.Length == 0)
            {
                this.wrapped.WriteAttributeString(string.Empty, "xmlns", "http://www.w3.org/2000/xmlns/", ns);
            }
            else
            {
                this.wrapped.WriteAttributeString("xmlns", prefix, "http://www.w3.org/2000/xmlns/", ns);
            }
        }

        public override void WriteProcessingInstruction(string name, string text)
        {
            this.wrapped.WriteProcessingInstruction(name, text);
        }

        public override void WriteRaw(string data)
        {
            this.wrapped.WriteRaw(data);
        }

        public override void WriteRaw(char[] buffer, int index, int count)
        {
            this.wrapped.WriteRaw(buffer, index, count);
        }

        public override void WriteStartAttribute(string prefix, string localName, string ns)
        {
            this.wrapped.WriteStartAttribute(prefix, localName, ns);
        }

        public override void WriteStartElement(string prefix, string localName, string ns)
        {
            this.wrapped.WriteStartElement(prefix, localName, ns);
        }

        public override void WriteString(string text)
        {
            this.wrapped.WriteString(text);
        }

        public override void WriteSurrogateCharEntity(char lowChar, char highChar)
        {
            this.wrapped.WriteSurrogateCharEntity(lowChar, highChar);
        }

        public override void WriteValue(bool value)
        {
            this.wrapped.WriteValue(value);
        }

        public override void WriteValue(DateTime value)
        {
            this.wrapped.WriteValue(value);
        }

        public override void WriteValue(decimal value)
        {
            this.wrapped.WriteValue(value);
        }

        public override void WriteValue(double value)
        {
            this.wrapped.WriteValue(value);
        }

        public override void WriteValue(int value)
        {
            this.wrapped.WriteValue(value);
        }

        public override void WriteValue(long value)
        {
            this.wrapped.WriteValue(value);
        }

        public override void WriteValue(object value)
        {
            this.wrapped.WriteValue(value);
        }

        public override void WriteValue(float value)
        {
            this.wrapped.WriteValue(value);
        }

        public override void WriteValue(string value)
        {
            this.wrapped.WriteValue(value);
        }

        public override void WriteWhitespace(string ws)
        {
            this.wrapped.WriteWhitespace(ws);
        }

        internal override void WriteXmlDeclaration(string xmldecl)
        {
        }

        internal override void WriteXmlDeclaration(XmlStandalone standalone)
        {
        }

        public override XmlWriterSettings Settings =>
            this.wrapped.Settings;
    }
}

