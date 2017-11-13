namespace System.Xml
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Text;
    using System.Xml.Schema;
    using System.Xml.XPath;

    public abstract class XmlWriter : IDisposable
    {
        private char[] writeNodeBuffer;
        private const int WriteNodeBufferSize = 0x400;

        protected XmlWriter()
        {
        }

        private static XmlWriter AddConformanceWrapper(XmlWriter baseWriter, XmlWriterSettings baseWriterSettings, XmlWriterSettings settings)
        {
            ConformanceLevel auto = ConformanceLevel.Auto;
            bool checkValues = false;
            bool checkNames = false;
            bool replaceNewLines = false;
            bool flag4 = false;
            if (baseWriterSettings == null)
            {
                if (settings.NewLineHandling == NewLineHandling.Replace)
                {
                    replaceNewLines = true;
                    flag4 = true;
                }
                if (settings.CheckCharacters)
                {
                    checkValues = true;
                    flag4 = true;
                }
            }
            else
            {
                if (settings.ConformanceLevel != baseWriterSettings.ConformanceLevel)
                {
                    auto = settings.ConformanceLevel;
                    flag4 = true;
                }
                if (settings.CheckCharacters && !baseWriterSettings.CheckCharacters)
                {
                    checkValues = true;
                    checkNames = auto == ConformanceLevel.Auto;
                    flag4 = true;
                }
                if ((settings.NewLineHandling == NewLineHandling.Replace) && (baseWriterSettings.NewLineHandling == NewLineHandling.None))
                {
                    replaceNewLines = true;
                    flag4 = true;
                }
            }
            if (!flag4)
            {
                return baseWriter;
            }
            XmlWriter writer = baseWriter;
            if (auto != ConformanceLevel.Auto)
            {
                writer = new XmlWellFormedWriter(writer, settings);
            }
            if (!checkValues && !replaceNewLines)
            {
                return writer;
            }
            return new XmlCharCheckingWriter(writer, checkValues, checkNames, replaceNewLines, settings.NewLineChars);
        }

        public abstract void Close();
        public static XmlWriter Create(Stream output) => 
            Create(output, null);

        public static XmlWriter Create(TextWriter output) => 
            Create(output, null);

        public static XmlWriter Create(string outputFileName) => 
            Create(outputFileName, null);

        public static XmlWriter Create(StringBuilder output) => 
            Create(output, null);

        public static XmlWriter Create(XmlWriter output) => 
            Create(output, null);

        public static XmlWriter Create(Stream output, XmlWriterSettings settings)
        {
            if (output == null)
            {
                throw new ArgumentNullException("output");
            }
            if (settings == null)
            {
                settings = new XmlWriterSettings();
            }
            return CreateWriterImpl(output, settings.Encoding, settings.CloseOutput, settings);
        }

        public static XmlWriter Create(TextWriter output, XmlWriterSettings settings)
        {
            if (output == null)
            {
                throw new ArgumentNullException("output");
            }
            if (settings == null)
            {
                settings = new XmlWriterSettings();
            }
            return CreateWriterImpl(output, settings);
        }

        public static XmlWriter Create(string outputFileName, XmlWriterSettings settings)
        {
            XmlWriter writer;
            if (outputFileName == null)
            {
                throw new ArgumentNullException("outputFileName");
            }
            if (settings == null)
            {
                settings = new XmlWriterSettings();
            }
            FileStream output = null;
            try
            {
                output = new FileStream(outputFileName, FileMode.Create, FileAccess.Write, FileShare.Read);
                writer = CreateWriterImpl(output, settings.Encoding, true, settings);
            }
            catch
            {
                if (output != null)
                {
                    output.Close();
                }
                throw;
            }
            return writer;
        }

        public static XmlWriter Create(StringBuilder output, XmlWriterSettings settings)
        {
            if (output == null)
            {
                throw new ArgumentNullException("output");
            }
            if (settings == null)
            {
                settings = new XmlWriterSettings();
            }
            return CreateWriterImpl(new StringWriter(output, CultureInfo.InvariantCulture), settings);
        }

        public static XmlWriter Create(XmlWriter output, XmlWriterSettings settings)
        {
            if (output == null)
            {
                throw new ArgumentNullException("output");
            }
            if (settings == null)
            {
                settings = new XmlWriterSettings();
            }
            return AddConformanceWrapper(output, output.Settings, settings);
        }

        private static XmlWriter CreateWriterImpl(TextWriter output, XmlWriterSettings settings)
        {
            XmlWriter writer;
            switch (settings.OutputMethod)
            {
                case XmlOutputMethod.Xml:
                    if (!settings.Indent)
                    {
                        writer = new XmlEncodedRawTextWriter(output, settings);
                        break;
                    }
                    writer = new XmlEncodedRawTextWriterIndent(output, settings);
                    break;

                case XmlOutputMethod.Html:
                    if (!settings.Indent)
                    {
                        writer = new HtmlEncodedRawTextWriter(output, settings);
                        break;
                    }
                    writer = new HtmlEncodedRawTextWriterIndent(output, settings);
                    break;

                case XmlOutputMethod.Text:
                    writer = new TextEncodedRawTextWriter(output, settings);
                    break;

                case XmlOutputMethod.AutoDetect:
                    writer = new XmlAutoDetectWriter(output, settings);
                    break;

                default:
                    return null;
            }
            if ((settings.OutputMethod != XmlOutputMethod.AutoDetect) && settings.IsQuerySpecific)
            {
                writer = new QueryOutputWriter((XmlRawWriter) writer, settings);
            }
            return new XmlWellFormedWriter(writer, settings);
        }

        private static XmlWriter CreateWriterImpl(Stream output, Encoding encoding, bool closeOutput, XmlWriterSettings settings)
        {
            XmlWriter writer;
            if (encoding.CodePage != 0xfde9)
            {
                switch (settings.OutputMethod)
                {
                    case XmlOutputMethod.Xml:
                        if (!settings.Indent)
                        {
                            writer = new XmlEncodedRawTextWriter(output, encoding, settings, closeOutput);
                        }
                        else
                        {
                            writer = new XmlEncodedRawTextWriterIndent(output, encoding, settings, closeOutput);
                        }
                        goto Label_010F;

                    case XmlOutputMethod.Html:
                        if (!settings.Indent)
                        {
                            writer = new HtmlEncodedRawTextWriter(output, encoding, settings, closeOutput);
                        }
                        else
                        {
                            writer = new HtmlEncodedRawTextWriterIndent(output, encoding, settings, closeOutput);
                        }
                        goto Label_010F;

                    case XmlOutputMethod.Text:
                        writer = new TextEncodedRawTextWriter(output, encoding, settings, closeOutput);
                        goto Label_010F;

                    case XmlOutputMethod.AutoDetect:
                        writer = new XmlAutoDetectWriter(output, encoding, settings);
                        goto Label_010F;
                }
                return null;
            }
            switch (settings.OutputMethod)
            {
                case XmlOutputMethod.Xml:
                    if (!settings.Indent)
                    {
                        writer = new XmlUtf8RawTextWriter(output, encoding, settings, closeOutput);
                        break;
                    }
                    writer = new XmlUtf8RawTextWriterIndent(output, encoding, settings, closeOutput);
                    break;

                case XmlOutputMethod.Html:
                    if (!settings.Indent)
                    {
                        writer = new HtmlUtf8RawTextWriter(output, encoding, settings, closeOutput);
                        break;
                    }
                    writer = new HtmlUtf8RawTextWriterIndent(output, encoding, settings, closeOutput);
                    break;

                case XmlOutputMethod.Text:
                    writer = new TextUtf8RawTextWriter(output, encoding, settings, closeOutput);
                    break;

                case XmlOutputMethod.AutoDetect:
                    writer = new XmlAutoDetectWriter(output, encoding, settings);
                    break;

                default:
                    return null;
            }
        Label_010F:
            if ((settings.OutputMethod != XmlOutputMethod.AutoDetect) && settings.IsQuerySpecific)
            {
                writer = new QueryOutputWriter((XmlRawWriter) writer, settings);
            }
            return new XmlWellFormedWriter(writer, settings);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (this.WriteState != System.Xml.WriteState.Closed)
            {
                try
                {
                    this.Close();
                }
                catch
                {
                }
            }
        }

        public abstract void Flush();
        public abstract string LookupPrefix(string ns);
        void IDisposable.Dispose()
        {
            this.Dispose(true);
        }

        public virtual void WriteAttributes(XmlReader reader, bool defattr)
        {
            if (reader == null)
            {
                throw new ArgumentNullException("reader");
            }
            if ((reader.NodeType == XmlNodeType.Element) || (reader.NodeType == XmlNodeType.XmlDeclaration))
            {
                if (reader.MoveToFirstAttribute())
                {
                    this.WriteAttributes(reader, defattr);
                    reader.MoveToElement();
                }
            }
            else
            {
                if (reader.NodeType != XmlNodeType.Attribute)
                {
                    throw new XmlException("Xml_InvalidPosition", string.Empty);
                }
                do
                {
                    IXmlSchemaInfo info;
                    if (defattr || (!reader.IsDefault && (((info = reader.SchemaInfo) == null) || !info.IsDefault)))
                    {
                        this.WriteStartAttribute(reader.Prefix, reader.LocalName, reader.NamespaceURI);
                        while (reader.ReadAttributeValue())
                        {
                            if (reader.NodeType == XmlNodeType.EntityReference)
                            {
                                this.WriteEntityRef(reader.Name);
                            }
                            else
                            {
                                this.WriteString(reader.Value);
                            }
                        }
                        this.WriteEndAttribute();
                    }
                }
                while (reader.MoveToNextAttribute());
            }
        }

        public void WriteAttributeString(string localName, string value)
        {
            this.WriteStartAttribute(null, localName, null);
            this.WriteString(value);
            this.WriteEndAttribute();
        }

        public void WriteAttributeString(string localName, string ns, string value)
        {
            this.WriteStartAttribute(null, localName, ns);
            this.WriteString(value);
            this.WriteEndAttribute();
        }

        public void WriteAttributeString(string prefix, string localName, string ns, string value)
        {
            this.WriteStartAttribute(prefix, localName, ns);
            this.WriteString(value);
            this.WriteEndAttribute();
        }

        public abstract void WriteBase64(byte[] buffer, int index, int count);
        public virtual void WriteBinHex(byte[] buffer, int index, int count)
        {
            BinHexEncoder.Encode(buffer, index, count, this);
        }

        public abstract void WriteCData(string text);
        public abstract void WriteCharEntity(char ch);
        public abstract void WriteChars(char[] buffer, int index, int count);
        public abstract void WriteComment(string text);
        public abstract void WriteDocType(string name, string pubid, string sysid, string subset);
        public void WriteElementString(string localName, string value)
        {
            this.WriteElementString(localName, null, value);
        }

        public void WriteElementString(string localName, string ns, string value)
        {
            this.WriteStartElement(localName, ns);
            if ((value != null) && (value.Length != 0))
            {
                this.WriteString(value);
            }
            this.WriteEndElement();
        }

        public void WriteElementString(string prefix, string localName, string ns, string value)
        {
            this.WriteStartElement(prefix, localName, ns);
            if ((value != null) && (value.Length != 0))
            {
                this.WriteString(value);
            }
            this.WriteEndElement();
        }

        public abstract void WriteEndAttribute();
        public abstract void WriteEndDocument();
        public abstract void WriteEndElement();
        public abstract void WriteEntityRef(string name);
        public abstract void WriteFullEndElement();
        private void WriteLocalNamespaces(XPathNavigator nsNav)
        {
            string localName = nsNav.LocalName;
            string str2 = nsNav.Value;
            if (nsNav.MoveToNextNamespace(XPathNamespaceScope.Local))
            {
                this.WriteLocalNamespaces(nsNav);
            }
            if (localName.Length == 0)
            {
                this.WriteAttributeString(string.Empty, "xmlns", "http://www.w3.org/2000/xmlns/", str2);
            }
            else
            {
                this.WriteAttributeString("xmlns", localName, "http://www.w3.org/2000/xmlns/", str2);
            }
        }

        public virtual void WriteName(string name)
        {
            this.WriteString(XmlConvert.VerifyQName(name, ExceptionType.ArgumentException));
        }

        public virtual void WriteNmToken(string name)
        {
            if ((name == null) || (name.Length == 0))
            {
                throw new ArgumentException(Res.GetString("Xml_EmptyName"));
            }
            this.WriteString(XmlConvert.VerifyNMTOKEN(name, ExceptionType.ArgumentException));
        }

        public virtual void WriteNode(XmlReader reader, bool defattr)
        {
            if (reader == null)
            {
                throw new ArgumentNullException("reader");
            }
            bool canReadValueChunk = reader.CanReadValueChunk;
            int num = (reader.NodeType == XmlNodeType.None) ? -1 : reader.Depth;
            do
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        this.WriteStartElement(reader.Prefix, reader.LocalName, reader.NamespaceURI);
                        this.WriteAttributes(reader, defattr);
                        if (reader.IsEmptyElement)
                        {
                            this.WriteEndElement();
                        }
                        break;

                    case XmlNodeType.Text:
                        int num2;
                        if (!canReadValueChunk)
                        {
                            this.WriteString(reader.Value);
                            break;
                        }
                        if (this.writeNodeBuffer == null)
                        {
                            this.writeNodeBuffer = new char[0x400];
                        }
                        while ((num2 = reader.ReadValueChunk(this.writeNodeBuffer, 0, 0x400)) > 0)
                        {
                            this.WriteChars(this.writeNodeBuffer, 0, num2);
                        }
                        break;

                    case XmlNodeType.CDATA:
                        this.WriteCData(reader.Value);
                        break;

                    case XmlNodeType.EntityReference:
                        this.WriteEntityRef(reader.Name);
                        break;

                    case XmlNodeType.ProcessingInstruction:
                    case XmlNodeType.XmlDeclaration:
                        this.WriteProcessingInstruction(reader.Name, reader.Value);
                        break;

                    case XmlNodeType.Comment:
                        this.WriteComment(reader.Value);
                        break;

                    case XmlNodeType.DocumentType:
                        this.WriteDocType(reader.Name, reader.GetAttribute("PUBLIC"), reader.GetAttribute("SYSTEM"), reader.Value);
                        break;

                    case XmlNodeType.Whitespace:
                    case XmlNodeType.SignificantWhitespace:
                        this.WriteWhitespace(reader.Value);
                        break;

                    case XmlNodeType.EndElement:
                        this.WriteFullEndElement();
                        break;
                }
            }
            while (reader.Read() && ((num < reader.Depth) || ((num == reader.Depth) && (reader.NodeType == XmlNodeType.EndElement))));
        }

        public virtual void WriteNode(XPathNavigator navigator, bool defattr)
        {
            bool flag;
            if (navigator == null)
            {
                throw new ArgumentNullException("navigator");
            }
            int num = 0;
            navigator = navigator.Clone();
        Label_0018:
            flag = false;
            switch (navigator.NodeType)
            {
                case XPathNodeType.Root:
                    flag = true;
                    break;

                case XPathNodeType.Element:
                    this.WriteStartElement(navigator.Prefix, navigator.LocalName, navigator.NamespaceURI);
                    if (navigator.MoveToFirstAttribute())
                    {
                        do
                        {
                            IXmlSchemaInfo schemaInfo = navigator.SchemaInfo;
                            if ((defattr || (schemaInfo == null)) || !schemaInfo.IsDefault)
                            {
                                this.WriteStartAttribute(navigator.Prefix, navigator.LocalName, navigator.NamespaceURI);
                                this.WriteString(navigator.Value);
                                this.WriteEndAttribute();
                            }
                        }
                        while (navigator.MoveToNextAttribute());
                        navigator.MoveToParent();
                    }
                    if (navigator.MoveToFirstNamespace(XPathNamespaceScope.Local))
                    {
                        this.WriteLocalNamespaces(navigator);
                        navigator.MoveToParent();
                    }
                    flag = true;
                    break;

                case XPathNodeType.Text:
                    this.WriteString(navigator.Value);
                    break;

                case XPathNodeType.SignificantWhitespace:
                case XPathNodeType.Whitespace:
                    this.WriteWhitespace(navigator.Value);
                    break;

                case XPathNodeType.ProcessingInstruction:
                    this.WriteProcessingInstruction(navigator.LocalName, navigator.Value);
                    break;

                case XPathNodeType.Comment:
                    this.WriteComment(navigator.Value);
                    break;
            }
            if (flag)
            {
                if (navigator.MoveToFirstChild())
                {
                    num++;
                    goto Label_0018;
                }
                if (navigator.NodeType == XPathNodeType.Element)
                {
                    if (navigator.IsEmptyElement)
                    {
                        this.WriteEndElement();
                    }
                    else
                    {
                        this.WriteFullEndElement();
                    }
                }
            }
            while (num != 0)
            {
                if (navigator.MoveToNext())
                {
                    goto Label_0018;
                }
                num--;
                navigator.MoveToParent();
                if (navigator.NodeType == XPathNodeType.Element)
                {
                    this.WriteFullEndElement();
                }
            }
        }

        public abstract void WriteProcessingInstruction(string name, string text);
        public virtual void WriteQualifiedName(string localName, string ns)
        {
            if ((ns != null) && (ns.Length > 0))
            {
                string prefix = this.LookupPrefix(ns);
                if (prefix == null)
                {
                    throw new ArgumentException(Res.GetString("Xml_UndefNamespace", new object[] { ns }));
                }
                this.WriteString(prefix);
                this.WriteString(":");
            }
            this.WriteString(localName);
        }

        public abstract void WriteRaw(string data);
        public abstract void WriteRaw(char[] buffer, int index, int count);
        public void WriteStartAttribute(string localName)
        {
            this.WriteStartAttribute(null, localName, null);
        }

        public void WriteStartAttribute(string localName, string ns)
        {
            this.WriteStartAttribute(null, localName, ns);
        }

        public abstract void WriteStartAttribute(string prefix, string localName, string ns);
        public abstract void WriteStartDocument();
        public abstract void WriteStartDocument(bool standalone);
        public void WriteStartElement(string localName)
        {
            this.WriteStartElement(null, localName, null);
        }

        public void WriteStartElement(string localName, string ns)
        {
            this.WriteStartElement(null, localName, ns);
        }

        public abstract void WriteStartElement(string prefix, string localName, string ns);
        public abstract void WriteString(string text);
        public abstract void WriteSurrogateCharEntity(char lowChar, char highChar);
        public virtual void WriteValue(bool value)
        {
            this.WriteString(XmlUntypedConverter.Untyped.ToString(value));
        }

        public virtual void WriteValue(DateTime value)
        {
            this.WriteString(XmlUntypedConverter.Untyped.ToString(value));
        }

        public virtual void WriteValue(decimal value)
        {
            this.WriteString(XmlUntypedConverter.Untyped.ToString(value));
        }

        public virtual void WriteValue(double value)
        {
            this.WriteString(XmlUntypedConverter.Untyped.ToString(value));
        }

        public virtual void WriteValue(int value)
        {
            this.WriteString(XmlUntypedConverter.Untyped.ToString(value));
        }

        public virtual void WriteValue(long value)
        {
            this.WriteString(XmlUntypedConverter.Untyped.ToString(value));
        }

        public virtual void WriteValue(object value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            this.WriteString(XmlUntypedConverter.Untyped.ToString(value, null));
        }

        public virtual void WriteValue(float value)
        {
            this.WriteString(XmlUntypedConverter.Untyped.ToString(value));
        }

        public virtual void WriteValue(string value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            this.WriteString(value);
        }

        public abstract void WriteWhitespace(string ws);

        public virtual XmlWriterSettings Settings =>
            null;

        public abstract System.Xml.WriteState WriteState { get; }

        public virtual string XmlLang =>
            string.Empty;

        public virtual System.Xml.XmlSpace XmlSpace =>
            System.Xml.XmlSpace.Default;
    }
}

