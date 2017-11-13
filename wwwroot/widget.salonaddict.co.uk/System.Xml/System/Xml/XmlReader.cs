namespace System.Xml
{
    using System;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Xml.Schema;

    [DebuggerDisplay("{debuggerDisplayProxy}")]
    public abstract class XmlReader : IDisposable
    {
        internal const int BiggerBufferSize = 0x2000;
        private static uint CanReadContentAsBitmap = 0x1e1bc;
        internal const int DefaultBufferSize = 0x1000;
        private static uint HasValueBitmap = 0x2659c;
        private static uint IsTextualNodeBitmap = 0x6018;
        internal const int MaxStreamLengthForDefaultBufferSize = 0x10000;

        protected XmlReader()
        {
        }

        private static string AddLineInfo(string message, IXmlLineInfo lineInfo)
        {
            if (lineInfo != null)
            {
                string[] args = new string[] { lineInfo.LineNumber.ToString(CultureInfo.InvariantCulture), lineInfo.LinePosition.ToString(CultureInfo.InvariantCulture) };
                message = message + " " + Res.GetString("Xml_ErrorPosition", args);
            }
            return message;
        }

        private static XmlReader AddValidation(XmlReader reader, XmlReaderSettings settings)
        {
            if (settings.ValidationType == ValidationType.Schema)
            {
                reader = new XsdValidatingReader(reader, settings.GetXmlResolver_CheckConfig(), settings);
                return reader;
            }
            if (settings.ValidationType == ValidationType.DTD)
            {
                reader = CreateDtdValidatingReader(reader, settings);
            }
            return reader;
        }

        private static XmlReader AddWrapper(XmlReader baseReader, XmlReaderSettings settings, XmlReaderSettings baseReaderSettings)
        {
            bool checkCharacters = false;
            bool ignoreWhitespace = false;
            bool ignoreComments = false;
            bool ignorePis = false;
            bool flag5 = false;
            bool prohibitDtd = false;
            if (baseReaderSettings == null)
            {
                if ((settings.ConformanceLevel != ConformanceLevel.Auto) && (settings.ConformanceLevel != GetV1ConformanceLevel(baseReader)))
                {
                    throw new InvalidOperationException(Res.GetString("Xml_IncompatibleConformanceLevel", new object[] { settings.ConformanceLevel.ToString() }));
                }
                if (settings.IgnoreWhitespace)
                {
                    WhitespaceHandling all = WhitespaceHandling.All;
                    XmlTextReader reader = baseReader as XmlTextReader;
                    if (reader != null)
                    {
                        all = reader.WhitespaceHandling;
                    }
                    else
                    {
                        XmlValidatingReader reader2 = baseReader as XmlValidatingReader;
                        if (reader2 != null)
                        {
                            all = ((XmlTextReader) reader2.Reader).WhitespaceHandling;
                        }
                    }
                    if (all == WhitespaceHandling.All)
                    {
                        ignoreWhitespace = true;
                        flag5 = true;
                    }
                }
                if (settings.IgnoreComments)
                {
                    ignoreComments = true;
                    flag5 = true;
                }
                if (settings.IgnoreProcessingInstructions)
                {
                    ignorePis = true;
                    flag5 = true;
                }
                if (settings.ProhibitDtd)
                {
                    XmlTextReader reader3 = baseReader as XmlTextReader;
                    if (reader3 == null)
                    {
                        XmlValidatingReader reader4 = baseReader as XmlValidatingReader;
                        if (reader4 != null)
                        {
                            reader3 = (XmlTextReader) reader4.Reader;
                        }
                    }
                    if ((reader3 == null) || !reader3.ProhibitDtd)
                    {
                        prohibitDtd = true;
                        flag5 = true;
                    }
                }
            }
            else
            {
                if ((settings.ConformanceLevel != baseReaderSettings.ConformanceLevel) && (settings.ConformanceLevel != ConformanceLevel.Auto))
                {
                    throw new InvalidOperationException(Res.GetString("Xml_IncompatibleConformanceLevel", new object[] { settings.ConformanceLevel.ToString() }));
                }
                if (settings.CheckCharacters && !baseReaderSettings.CheckCharacters)
                {
                    checkCharacters = true;
                    flag5 = true;
                }
                if (settings.IgnoreWhitespace && !baseReaderSettings.IgnoreWhitespace)
                {
                    ignoreWhitespace = true;
                    flag5 = true;
                }
                if (settings.IgnoreComments && !baseReaderSettings.IgnoreComments)
                {
                    ignoreComments = true;
                    flag5 = true;
                }
                if (settings.IgnoreProcessingInstructions && !baseReaderSettings.IgnoreProcessingInstructions)
                {
                    ignorePis = true;
                    flag5 = true;
                }
                if (settings.ProhibitDtd && !baseReaderSettings.ProhibitDtd)
                {
                    prohibitDtd = true;
                    flag5 = true;
                }
            }
            if (!flag5)
            {
                return baseReader;
            }
            IXmlNamespaceResolver readerAsNSResolver = baseReader as IXmlNamespaceResolver;
            if (readerAsNSResolver != null)
            {
                return new XmlCharCheckingReaderWithNS(baseReader, readerAsNSResolver, checkCharacters, ignoreWhitespace, ignoreComments, ignorePis, prohibitDtd);
            }
            return new XmlCharCheckingReader(baseReader, checkCharacters, ignoreWhitespace, ignoreComments, ignorePis, prohibitDtd);
        }

        internal static int CalcBufferSize(Stream input)
        {
            int num = 0x1000;
            if (input.CanSeek)
            {
                long length = input.Length;
                if (length < num)
                {
                    return (int) length;
                }
                if (length > 0x10000L)
                {
                    num = 0x2000;
                }
            }
            return num;
        }

        internal bool CanReadContentAs() => 
            CanReadContentAs(this.NodeType);

        internal static bool CanReadContentAs(XmlNodeType nodeType) => 
            (0L != (CanReadContentAsBitmap & (((int) 1) << nodeType)));

        internal void CheckElement(string localName, string namespaceURI)
        {
            if ((localName == null) || (localName.Length == 0))
            {
                throw XmlConvert.CreateInvalidNameArgumentException(localName, "localName");
            }
            if (namespaceURI == null)
            {
                throw new ArgumentNullException("namespaceURI");
            }
            if (this.NodeType != XmlNodeType.Element)
            {
                throw new XmlException("Xml_InvalidNodeType", this.NodeType.ToString(), this as IXmlLineInfo);
            }
            if ((this.LocalName != localName) || (this.NamespaceURI != namespaceURI))
            {
                throw new XmlException("Xml_ElementNotFoundNs", new string[] { localName, namespaceURI }, this as IXmlLineInfo);
            }
        }

        public abstract void Close();
        public static XmlReader Create(Stream input)
        {
            XmlReaderSettings settings = new XmlReaderSettings();
            return CreateReaderImpl(input, settings, null, string.Empty, null, settings.CloseInput);
        }

        public static XmlReader Create(TextReader input) => 
            CreateReaderImpl(input, null, string.Empty, null);

        public static XmlReader Create(string inputUri) => 
            Create(inputUri, null, null);

        public static XmlReader Create(Stream input, XmlReaderSettings settings) => 
            Create(input, settings, string.Empty);

        public static XmlReader Create(TextReader input, XmlReaderSettings settings) => 
            Create(input, settings, string.Empty);

        public static XmlReader Create(string inputUri, XmlReaderSettings settings) => 
            Create(inputUri, settings, null);

        public static XmlReader Create(XmlReader reader, XmlReaderSettings settings)
        {
            if (reader == null)
            {
                throw new ArgumentNullException("reader");
            }
            if (settings == null)
            {
                settings = new XmlReaderSettings();
            }
            return CreateReaderImpl(reader, settings);
        }

        public static XmlReader Create(Stream input, XmlReaderSettings settings, string baseUri)
        {
            if (settings == null)
            {
                settings = new XmlReaderSettings();
            }
            return CreateReaderImpl(input, settings, null, baseUri, null, settings.CloseInput);
        }

        public static XmlReader Create(Stream input, XmlReaderSettings settings, XmlParserContext inputContext)
        {
            if (settings == null)
            {
                settings = new XmlReaderSettings();
            }
            return CreateReaderImpl(input, settings, null, string.Empty, inputContext, settings.CloseInput);
        }

        public static XmlReader Create(TextReader input, XmlReaderSettings settings, string baseUri) => 
            CreateReaderImpl(input, settings, baseUri, null);

        public static XmlReader Create(TextReader input, XmlReaderSettings settings, XmlParserContext inputContext) => 
            CreateReaderImpl(input, settings, string.Empty, inputContext);

        public static XmlReader Create(string inputUri, XmlReaderSettings settings, XmlParserContext inputContext)
        {
            XmlReader reader;
            if (inputUri == null)
            {
                throw new ArgumentNullException("inputUri");
            }
            if (inputUri.Length == 0)
            {
                throw new ArgumentException(Res.GetString("XmlConvert_BadUri"), "inputUri");
            }
            if (settings == null)
            {
                settings = new XmlReaderSettings();
            }
            XmlResolver xmlResolver = settings.GetXmlResolver();
            if (xmlResolver == null)
            {
                xmlResolver = new XmlUrlResolver();
            }
            Uri absoluteUri = xmlResolver.ResolveUri(null, inputUri);
            Stream input = (Stream) xmlResolver.GetEntity(absoluteUri, string.Empty, typeof(Stream));
            if (input == null)
            {
                throw new XmlException("Xml_CannotResolveUrl", inputUri);
            }
            try
            {
                reader = CreateReaderImpl(input, settings, absoluteUri, absoluteUri.ToString(), inputContext, true);
            }
            catch
            {
                input.Close();
                throw;
            }
            return reader;
        }

        private static XmlValidatingReaderImpl CreateDtdValidatingReader(XmlReader baseReader, XmlReaderSettings settings) => 
            new XmlValidatingReaderImpl(baseReader, settings.GetEventHandler(), (settings.ValidationFlags & XmlSchemaValidationFlags.ProcessIdentityConstraints) != XmlSchemaValidationFlags.None);

        internal Exception CreateReadContentAsException(string methodName) => 
            CreateReadContentAsException(methodName, this.NodeType, this as IXmlLineInfo);

        internal static Exception CreateReadContentAsException(string methodName, XmlNodeType nodeType, IXmlLineInfo lineInfo) => 
            new InvalidOperationException(AddLineInfo(Res.GetString("Xml_InvalidReadContentAs", new string[] { methodName, nodeType.ToString() }), lineInfo));

        internal Exception CreateReadElementContentAsException(string methodName) => 
            CreateReadElementContentAsException(methodName, this.NodeType, this as IXmlLineInfo);

        internal static Exception CreateReadElementContentAsException(string methodName, XmlNodeType nodeType, IXmlLineInfo lineInfo) => 
            new InvalidOperationException(AddLineInfo(Res.GetString("Xml_InvalidReadElementContentAs", new string[] { methodName, nodeType.ToString() }), lineInfo));

        private static XmlReader CreateReaderImpl(XmlReader baseReader, XmlReaderSettings settings)
        {
            XmlReader reader = baseReader;
            if (settings.ValidationType == ValidationType.DTD)
            {
                reader = CreateDtdValidatingReader(reader, settings);
            }
            reader = AddWrapper(reader, settings, reader.Settings);
            if (settings.ValidationType == ValidationType.Schema)
            {
                reader = new XsdValidatingReader(reader, settings.GetXmlResolver_CheckConfig(), settings);
            }
            return reader;
        }

        private static XmlReader CreateReaderImpl(TextReader input, XmlReaderSettings settings, string baseUriStr, XmlParserContext context)
        {
            if (input == null)
            {
                throw new ArgumentNullException("input");
            }
            if (settings == null)
            {
                settings = new XmlReaderSettings();
            }
            if (baseUriStr == null)
            {
                baseUriStr = string.Empty;
            }
            XmlReader reader = new XmlTextReaderImpl(input, settings, baseUriStr, context);
            if (settings.ValidationType == ValidationType.Schema)
            {
                return new XsdValidatingReader(reader, settings.GetXmlResolver_CheckConfig(), settings);
            }
            if (settings.ValidationType == ValidationType.DTD)
            {
                reader = CreateDtdValidatingReader(reader, settings);
            }
            return reader;
        }

        private static XmlReader CreateReaderImpl(Stream input, XmlReaderSettings settings, Uri baseUri, string baseUriStr, XmlParserContext inputContext, bool closeInput)
        {
            if (input == null)
            {
                throw new ArgumentNullException("input");
            }
            if (baseUriStr == null)
            {
                baseUriStr = string.Empty;
            }
            XmlReader reader = new XmlTextReaderImpl(input, null, 0, settings, baseUri, baseUriStr, inputContext, closeInput);
            if (settings.ValidationType != ValidationType.None)
            {
                reader = AddValidation(reader, settings);
            }
            return reader;
        }

        internal static XmlReader CreateSqlReader(Stream input, XmlReaderSettings settings, XmlParserContext inputContext)
        {
            XmlReader reader;
            int num2;
            if (input == null)
            {
                throw new ArgumentNullException("input");
            }
            if (settings == null)
            {
                settings = new XmlReaderSettings();
            }
            byte[] buffer = new byte[CalcBufferSize(input)];
            int offset = 0;
            do
            {
                num2 = input.Read(buffer, offset, buffer.Length - offset);
                offset += num2;
            }
            while ((num2 > 0) && (offset < 2));
            if (((offset >= 2) && (buffer[0] == 0xdf)) && (buffer[1] == 0xff))
            {
                if (inputContext != null)
                {
                    throw new ArgumentException(Res.GetString("XmlBinary_NoParserContext"), "inputContext");
                }
                reader = new XmlSqlBinaryReader(input, buffer, offset, string.Empty, settings.CloseInput, settings);
            }
            else
            {
                reader = new XmlTextReaderImpl(input, buffer, offset, settings, null, string.Empty, inputContext, settings.CloseInput);
            }
            if (settings.ValidationType != ValidationType.None)
            {
                reader = AddValidation(reader, settings);
            }
            return reader;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (this.ReadState != System.Xml.ReadState.Closed)
            {
                this.Close();
            }
        }

        private void FinishReadElementContentAsXxx()
        {
            if (this.NodeType != XmlNodeType.EndElement)
            {
                throw new XmlException("Xml_InvalidNodeType", this.NodeType.ToString());
            }
            this.Read();
        }

        public abstract string GetAttribute(int i);
        public abstract string GetAttribute(string name);
        public abstract string GetAttribute(string name, string namespaceURI);
        internal static System.Xml.Schema.SchemaInfo GetDtdSchemaInfo(XmlReader reader)
        {
            XmlWrappingReader reader2 = reader as XmlWrappingReader;
            if (reader2 != null)
            {
                return reader2.DtdSchemaInfo;
            }
            XmlTextReaderImpl xmlTextReaderImpl = GetXmlTextReaderImpl(reader);
            return xmlTextReaderImpl?.DtdSchemaInfo;
        }

        internal static Encoding GetEncoding(XmlReader reader)
        {
            XmlTextReaderImpl xmlTextReaderImpl = GetXmlTextReaderImpl(reader);
            return xmlTextReaderImpl?.Encoding;
        }

        internal static ConformanceLevel GetV1ConformanceLevel(XmlReader reader)
        {
            XmlTextReaderImpl xmlTextReaderImpl = GetXmlTextReaderImpl(reader);
            return xmlTextReaderImpl?.V1ComformanceLevel;
        }

        private static XmlTextReaderImpl GetXmlTextReaderImpl(XmlReader reader)
        {
            XmlTextReaderImpl impl = reader as XmlTextReaderImpl;
            if (impl != null)
            {
                return impl;
            }
            XmlTextReader reader2 = reader as XmlTextReader;
            if (reader2 != null)
            {
                return reader2.Impl;
            }
            XmlValidatingReaderImpl impl2 = reader as XmlValidatingReaderImpl;
            if (impl2 != null)
            {
                return impl2.ReaderImpl;
            }
            XmlValidatingReader reader3 = reader as XmlValidatingReader;
            if (reader3 != null)
            {
                return reader3.Impl.ReaderImpl;
            }
            return null;
        }

        internal static bool HasValueInternal(XmlNodeType nodeType) => 
            (0L != (HasValueBitmap & (((int) 1) << nodeType)));

        internal string InternalReadContentAsString()
        {
            string str = string.Empty;
            BufferBuilder builder = null;
            do
            {
                switch (this.NodeType)
                {
                    case XmlNodeType.Attribute:
                        return this.Value;

                    case XmlNodeType.Text:
                    case XmlNodeType.CDATA:
                    case XmlNodeType.Whitespace:
                    case XmlNodeType.SignificantWhitespace:
                        if (str.Length != 0)
                        {
                            if (builder == null)
                            {
                                builder = new BufferBuilder();
                                builder.Append(str);
                            }
                            builder.Append(this.Value);
                            break;
                        }
                        str = this.Value;
                        break;

                    case XmlNodeType.EntityReference:
                        if (!this.CanResolveEntity)
                        {
                            goto Label_00B4;
                        }
                        this.ResolveEntity();
                        break;

                    case XmlNodeType.Entity:
                    case XmlNodeType.Document:
                    case XmlNodeType.DocumentType:
                    case XmlNodeType.DocumentFragment:
                    case XmlNodeType.Notation:
                    case XmlNodeType.EndElement:
                        goto Label_00B4;

                    case XmlNodeType.ProcessingInstruction:
                    case XmlNodeType.Comment:
                    case XmlNodeType.EndEntity:
                        break;

                    default:
                        goto Label_00B4;
                }
            }
            while ((this.AttributeCount != 0) ? this.ReadAttributeValue() : this.Read());
        Label_00B4:
            if (builder != null)
            {
                return builder.ToString();
            }
            return str;
        }

        public static bool IsName(string str) => 
            XmlCharType.Instance.IsName(str);

        public static bool IsNameToken(string str) => 
            XmlCharType.Instance.IsNmToken(str);

        public virtual bool IsStartElement() => 
            (this.MoveToContent() == XmlNodeType.Element);

        public virtual bool IsStartElement(string name) => 
            ((this.MoveToContent() == XmlNodeType.Element) && (this.Name == name));

        public virtual bool IsStartElement(string localname, string ns)
        {
            if (this.MoveToContent() != XmlNodeType.Element)
            {
                return false;
            }
            return ((this.LocalName == localname) && (this.NamespaceURI == ns));
        }

        internal static bool IsTextualNode(XmlNodeType nodeType) => 
            (0L != (IsTextualNodeBitmap & (((int) 1) << nodeType)));

        public abstract string LookupNamespace(string prefix);
        public virtual void MoveToAttribute(int i)
        {
            if ((i < 0) || (i >= this.AttributeCount))
            {
                throw new ArgumentOutOfRangeException("i");
            }
            this.MoveToElement();
            this.MoveToFirstAttribute();
            for (int j = 0; j < i; j++)
            {
                this.MoveToNextAttribute();
            }
        }

        public abstract bool MoveToAttribute(string name);
        public abstract bool MoveToAttribute(string name, string ns);
        public virtual XmlNodeType MoveToContent()
        {
        Label_0000:
            switch (this.NodeType)
            {
                case XmlNodeType.Element:
                case XmlNodeType.Text:
                case XmlNodeType.CDATA:
                case XmlNodeType.EntityReference:
                case XmlNodeType.EndElement:
                case XmlNodeType.EndEntity:
                    break;

                case XmlNodeType.Attribute:
                    this.MoveToElement();
                    break;

                default:
                    if (this.Read())
                    {
                        goto Label_0000;
                    }
                    return this.NodeType;
            }
            return this.NodeType;
        }

        public abstract bool MoveToElement();
        public abstract bool MoveToFirstAttribute();
        public abstract bool MoveToNextAttribute();
        public abstract bool Read();
        public abstract bool ReadAttributeValue();
        public virtual object ReadContentAs(Type returnType, IXmlNamespaceResolver namespaceResolver)
        {
            object obj2;
            if (!this.CanReadContentAs())
            {
                throw this.CreateReadContentAsException("ReadContentAs");
            }
            string str = this.InternalReadContentAsString();
            if (returnType == typeof(string))
            {
                return str;
            }
            try
            {
                obj2 = XmlUntypedConverter.Untyped.ChangeType(str, returnType, this as IXmlNamespaceResolver);
            }
            catch (FormatException exception)
            {
                throw new XmlException("Xml_ReadContentAsFormatException", returnType.ToString(), exception, this as IXmlLineInfo);
            }
            catch (InvalidCastException exception2)
            {
                throw new XmlException("Xml_ReadContentAsFormatException", returnType.ToString(), exception2, this as IXmlLineInfo);
            }
            return obj2;
        }

        public virtual int ReadContentAsBase64(byte[] buffer, int index, int count)
        {
            throw new NotSupportedException(Res.GetString("Xml_ReadBinaryContentNotSupported", new object[] { "ReadContentAsBase64" }));
        }

        public virtual int ReadContentAsBinHex(byte[] buffer, int index, int count)
        {
            throw new NotSupportedException(Res.GetString("Xml_ReadBinaryContentNotSupported", new object[] { "ReadContentAsBinHex" }));
        }

        public virtual bool ReadContentAsBoolean()
        {
            bool flag;
            if (!this.CanReadContentAs())
            {
                throw this.CreateReadContentAsException("ReadContentAsBoolean");
            }
            try
            {
                flag = XmlConvert.ToBoolean(this.InternalReadContentAsString());
            }
            catch (FormatException exception)
            {
                throw new XmlException("Xml_ReadContentAsFormatException", "Boolean", exception, this as IXmlLineInfo);
            }
            return flag;
        }

        public virtual DateTime ReadContentAsDateTime()
        {
            DateTime time;
            if (!this.CanReadContentAs())
            {
                throw this.CreateReadContentAsException("ReadContentAsDateTime");
            }
            try
            {
                time = XmlConvert.ToDateTime(this.InternalReadContentAsString(), XmlDateTimeSerializationMode.RoundtripKind);
            }
            catch (FormatException exception)
            {
                throw new XmlException("Xml_ReadContentAsFormatException", "DateTime", exception, this as IXmlLineInfo);
            }
            return time;
        }

        public virtual decimal ReadContentAsDecimal()
        {
            decimal num;
            if (!this.CanReadContentAs())
            {
                throw this.CreateReadContentAsException("ReadContentAsDecimal");
            }
            try
            {
                num = XmlConvert.ToDecimal(this.InternalReadContentAsString());
            }
            catch (FormatException exception)
            {
                throw new XmlException("Xml_ReadContentAsFormatException", "Decimal", exception, this as IXmlLineInfo);
            }
            return num;
        }

        public virtual double ReadContentAsDouble()
        {
            double num;
            if (!this.CanReadContentAs())
            {
                throw this.CreateReadContentAsException("ReadContentAsDouble");
            }
            try
            {
                num = XmlConvert.ToDouble(this.InternalReadContentAsString());
            }
            catch (FormatException exception)
            {
                throw new XmlException("Xml_ReadContentAsFormatException", "Double", exception, this as IXmlLineInfo);
            }
            return num;
        }

        public virtual float ReadContentAsFloat()
        {
            float num;
            if (!this.CanReadContentAs())
            {
                throw this.CreateReadContentAsException("ReadContentAsFloat");
            }
            try
            {
                num = XmlConvert.ToSingle(this.InternalReadContentAsString());
            }
            catch (FormatException exception)
            {
                throw new XmlException("Xml_ReadContentAsFormatException", "Float", exception, this as IXmlLineInfo);
            }
            return num;
        }

        public virtual int ReadContentAsInt()
        {
            int num;
            if (!this.CanReadContentAs())
            {
                throw this.CreateReadContentAsException("ReadContentAsInt");
            }
            try
            {
                num = XmlConvert.ToInt32(this.InternalReadContentAsString());
            }
            catch (FormatException exception)
            {
                throw new XmlException("Xml_ReadContentAsFormatException", "Int", exception, this as IXmlLineInfo);
            }
            return num;
        }

        public virtual long ReadContentAsLong()
        {
            long num;
            if (!this.CanReadContentAs())
            {
                throw this.CreateReadContentAsException("ReadContentAsLong");
            }
            try
            {
                num = XmlConvert.ToInt64(this.InternalReadContentAsString());
            }
            catch (FormatException exception)
            {
                throw new XmlException("Xml_ReadContentAsFormatException", "Long", exception, this as IXmlLineInfo);
            }
            return num;
        }

        public virtual object ReadContentAsObject()
        {
            if (!this.CanReadContentAs())
            {
                throw this.CreateReadContentAsException("ReadContentAsObject");
            }
            return this.InternalReadContentAsString();
        }

        public virtual string ReadContentAsString()
        {
            if (!this.CanReadContentAs())
            {
                throw this.CreateReadContentAsException("ReadContentAsString");
            }
            return this.InternalReadContentAsString();
        }

        public virtual object ReadElementContentAs(Type returnType, IXmlNamespaceResolver namespaceResolver)
        {
            if (this.SetupReadElementContentAsXxx("ReadElementContentAs"))
            {
                object obj2 = this.ReadContentAs(returnType, namespaceResolver);
                this.FinishReadElementContentAsXxx();
                return obj2;
            }
            if (returnType != typeof(string))
            {
                return XmlUntypedConverter.Untyped.ChangeType(string.Empty, returnType, namespaceResolver);
            }
            return string.Empty;
        }

        public virtual object ReadElementContentAs(Type returnType, IXmlNamespaceResolver namespaceResolver, string localName, string namespaceURI)
        {
            this.CheckElement(localName, namespaceURI);
            return this.ReadElementContentAs(returnType, namespaceResolver);
        }

        public virtual int ReadElementContentAsBase64(byte[] buffer, int index, int count)
        {
            throw new NotSupportedException(Res.GetString("Xml_ReadBinaryContentNotSupported", new object[] { "ReadElementContentAsBase64" }));
        }

        public virtual int ReadElementContentAsBinHex(byte[] buffer, int index, int count)
        {
            throw new NotSupportedException(Res.GetString("Xml_ReadBinaryContentNotSupported", new object[] { "ReadElementContentAsBinHex" }));
        }

        public virtual bool ReadElementContentAsBoolean()
        {
            if (this.SetupReadElementContentAsXxx("ReadElementContentAsBoolean"))
            {
                bool flag = this.ReadContentAsBoolean();
                this.FinishReadElementContentAsXxx();
                return flag;
            }
            return XmlConvert.ToBoolean(string.Empty);
        }

        public virtual bool ReadElementContentAsBoolean(string localName, string namespaceURI)
        {
            this.CheckElement(localName, namespaceURI);
            return this.ReadElementContentAsBoolean();
        }

        public virtual DateTime ReadElementContentAsDateTime()
        {
            if (this.SetupReadElementContentAsXxx("ReadElementContentAsDateTime"))
            {
                DateTime time = this.ReadContentAsDateTime();
                this.FinishReadElementContentAsXxx();
                return time;
            }
            return XmlConvert.ToDateTime(string.Empty, XmlDateTimeSerializationMode.RoundtripKind);
        }

        public virtual DateTime ReadElementContentAsDateTime(string localName, string namespaceURI)
        {
            this.CheckElement(localName, namespaceURI);
            return this.ReadElementContentAsDateTime();
        }

        public virtual decimal ReadElementContentAsDecimal()
        {
            if (this.SetupReadElementContentAsXxx("ReadElementContentAsDecimal"))
            {
                decimal num = this.ReadContentAsDecimal();
                this.FinishReadElementContentAsXxx();
                return num;
            }
            return XmlConvert.ToDecimal(string.Empty);
        }

        public virtual decimal ReadElementContentAsDecimal(string localName, string namespaceURI)
        {
            this.CheckElement(localName, namespaceURI);
            return this.ReadElementContentAsDecimal();
        }

        public virtual double ReadElementContentAsDouble()
        {
            if (this.SetupReadElementContentAsXxx("ReadElementContentAsDouble"))
            {
                double num = this.ReadContentAsDouble();
                this.FinishReadElementContentAsXxx();
                return num;
            }
            return XmlConvert.ToDouble(string.Empty);
        }

        public virtual double ReadElementContentAsDouble(string localName, string namespaceURI)
        {
            this.CheckElement(localName, namespaceURI);
            return this.ReadElementContentAsDouble();
        }

        public virtual float ReadElementContentAsFloat()
        {
            if (this.SetupReadElementContentAsXxx("ReadElementContentAsFloat"))
            {
                float num = this.ReadContentAsFloat();
                this.FinishReadElementContentAsXxx();
                return num;
            }
            return XmlConvert.ToSingle(string.Empty);
        }

        public virtual float ReadElementContentAsFloat(string localName, string namespaceURI)
        {
            this.CheckElement(localName, namespaceURI);
            return this.ReadElementContentAsFloat();
        }

        public virtual int ReadElementContentAsInt()
        {
            if (this.SetupReadElementContentAsXxx("ReadElementContentAsInt"))
            {
                int num = this.ReadContentAsInt();
                this.FinishReadElementContentAsXxx();
                return num;
            }
            return XmlConvert.ToInt32(string.Empty);
        }

        public virtual int ReadElementContentAsInt(string localName, string namespaceURI)
        {
            this.CheckElement(localName, namespaceURI);
            return this.ReadElementContentAsInt();
        }

        public virtual long ReadElementContentAsLong()
        {
            if (this.SetupReadElementContentAsXxx("ReadElementContentAsLong"))
            {
                long num = this.ReadContentAsLong();
                this.FinishReadElementContentAsXxx();
                return num;
            }
            return XmlConvert.ToInt64(string.Empty);
        }

        public virtual long ReadElementContentAsLong(string localName, string namespaceURI)
        {
            this.CheckElement(localName, namespaceURI);
            return this.ReadElementContentAsLong();
        }

        public virtual object ReadElementContentAsObject()
        {
            if (this.SetupReadElementContentAsXxx("ReadElementContentAsObject"))
            {
                object obj2 = this.ReadContentAsObject();
                this.FinishReadElementContentAsXxx();
                return obj2;
            }
            return string.Empty;
        }

        public virtual object ReadElementContentAsObject(string localName, string namespaceURI)
        {
            this.CheckElement(localName, namespaceURI);
            return this.ReadElementContentAsObject();
        }

        public virtual string ReadElementContentAsString()
        {
            if (this.SetupReadElementContentAsXxx("ReadElementContentAsString"))
            {
                string str = this.ReadContentAsString();
                this.FinishReadElementContentAsXxx();
                return str;
            }
            return string.Empty;
        }

        public virtual string ReadElementContentAsString(string localName, string namespaceURI)
        {
            this.CheckElement(localName, namespaceURI);
            return this.ReadElementContentAsString();
        }

        public virtual string ReadElementString()
        {
            string str = string.Empty;
            if (this.MoveToContent() != XmlNodeType.Element)
            {
                throw new XmlException("Xml_InvalidNodeType", this.NodeType.ToString(), this as IXmlLineInfo);
            }
            if (!this.IsEmptyElement)
            {
                this.Read();
                str = this.ReadString();
                if (this.NodeType != XmlNodeType.EndElement)
                {
                    throw new XmlException("Xml_UnexpectedNodeInSimpleContent", new string[] { this.NodeType.ToString(), "ReadElementString" }, this as IXmlLineInfo);
                }
                this.Read();
                return str;
            }
            this.Read();
            return str;
        }

        public virtual string ReadElementString(string name)
        {
            string str = string.Empty;
            if (this.MoveToContent() != XmlNodeType.Element)
            {
                throw new XmlException("Xml_InvalidNodeType", this.NodeType.ToString(), this as IXmlLineInfo);
            }
            if (this.Name != name)
            {
                throw new XmlException("Xml_ElementNotFound", name, this as IXmlLineInfo);
            }
            if (!this.IsEmptyElement)
            {
                str = this.ReadString();
                if (this.NodeType != XmlNodeType.EndElement)
                {
                    throw new XmlException("Xml_InvalidNodeType", this.NodeType.ToString(), this as IXmlLineInfo);
                }
                this.Read();
                return str;
            }
            this.Read();
            return str;
        }

        public virtual string ReadElementString(string localname, string ns)
        {
            string str = string.Empty;
            if (this.MoveToContent() != XmlNodeType.Element)
            {
                throw new XmlException("Xml_InvalidNodeType", this.NodeType.ToString(), this as IXmlLineInfo);
            }
            if ((this.LocalName != localname) || (this.NamespaceURI != ns))
            {
                throw new XmlException("Xml_ElementNotFoundNs", new string[] { localname, ns }, this as IXmlLineInfo);
            }
            if (!this.IsEmptyElement)
            {
                str = this.ReadString();
                if (this.NodeType != XmlNodeType.EndElement)
                {
                    throw new XmlException("Xml_InvalidNodeType", this.NodeType.ToString(), this as IXmlLineInfo);
                }
                this.Read();
                return str;
            }
            this.Read();
            return str;
        }

        public virtual void ReadEndElement()
        {
            if (this.MoveToContent() != XmlNodeType.EndElement)
            {
                throw new XmlException("Xml_InvalidNodeType", this.NodeType.ToString(), this as IXmlLineInfo);
            }
            this.Read();
        }

        public virtual string ReadInnerXml()
        {
            if (this.ReadState != System.Xml.ReadState.Interactive)
            {
                return string.Empty;
            }
            if ((this.NodeType != XmlNodeType.Attribute) && (this.NodeType != XmlNodeType.Element))
            {
                this.Read();
                return string.Empty;
            }
            StringWriter w = new StringWriter(CultureInfo.InvariantCulture);
            XmlTextWriter xtw = new XmlTextWriter(w);
            try
            {
                this.SetNamespacesFlag(xtw);
                if (this.NodeType == XmlNodeType.Attribute)
                {
                    xtw.QuoteChar = this.QuoteChar;
                    this.WriteAttributeValue(xtw);
                }
                if (this.NodeType == XmlNodeType.Element)
                {
                    this.WriteNode(xtw, false);
                }
            }
            finally
            {
                xtw.Close();
            }
            return w.ToString();
        }

        public virtual string ReadOuterXml()
        {
            if (this.ReadState != System.Xml.ReadState.Interactive)
            {
                return string.Empty;
            }
            if ((this.NodeType != XmlNodeType.Attribute) && (this.NodeType != XmlNodeType.Element))
            {
                this.Read();
                return string.Empty;
            }
            StringWriter w = new StringWriter(CultureInfo.InvariantCulture);
            XmlTextWriter xtw = new XmlTextWriter(w);
            try
            {
                this.SetNamespacesFlag(xtw);
                if (this.NodeType == XmlNodeType.Attribute)
                {
                    xtw.WriteStartAttribute(this.Prefix, this.LocalName, this.NamespaceURI);
                    this.WriteAttributeValue(xtw);
                    xtw.WriteEndAttribute();
                }
                else
                {
                    xtw.WriteNode(this, false);
                }
            }
            finally
            {
                xtw.Close();
            }
            return w.ToString();
        }

        public virtual void ReadStartElement()
        {
            if (this.MoveToContent() != XmlNodeType.Element)
            {
                throw new XmlException("Xml_InvalidNodeType", this.NodeType.ToString(), this as IXmlLineInfo);
            }
            this.Read();
        }

        public virtual void ReadStartElement(string name)
        {
            if (this.MoveToContent() != XmlNodeType.Element)
            {
                throw new XmlException("Xml_InvalidNodeType", this.NodeType.ToString(), this as IXmlLineInfo);
            }
            if (this.Name != name)
            {
                throw new XmlException("Xml_ElementNotFound", name, this as IXmlLineInfo);
            }
            this.Read();
        }

        public virtual void ReadStartElement(string localname, string ns)
        {
            if (this.MoveToContent() != XmlNodeType.Element)
            {
                throw new XmlException("Xml_InvalidNodeType", this.NodeType.ToString(), this as IXmlLineInfo);
            }
            if ((this.LocalName != localname) || (this.NamespaceURI != ns))
            {
                throw new XmlException("Xml_ElementNotFoundNs", new string[] { localname, ns }, this as IXmlLineInfo);
            }
            this.Read();
        }

        public virtual string ReadString()
        {
            if (this.ReadState != System.Xml.ReadState.Interactive)
            {
                return string.Empty;
            }
            this.MoveToElement();
            if (this.NodeType == XmlNodeType.Element)
            {
                if (this.IsEmptyElement)
                {
                    return string.Empty;
                }
                if (!this.Read())
                {
                    throw new InvalidOperationException(Res.GetString("Xml_InvalidOperation"));
                }
                if (this.NodeType == XmlNodeType.EndElement)
                {
                    return string.Empty;
                }
            }
            string str = string.Empty;
            while (IsTextualNode(this.NodeType))
            {
                str = str + this.Value;
                if (!this.Read())
                {
                    return str;
                }
            }
            return str;
        }

        public virtual XmlReader ReadSubtree()
        {
            if (this.NodeType != XmlNodeType.Element)
            {
                throw new InvalidOperationException(Res.GetString("Xml_ReadSubtreeNotOnElement"));
            }
            return new XmlSubtreeReader(this);
        }

        public virtual bool ReadToDescendant(string name)
        {
            if ((name == null) || (name.Length == 0))
            {
                throw XmlConvert.CreateInvalidNameArgumentException(name, "name");
            }
            int depth = this.Depth;
            if (this.NodeType != XmlNodeType.Element)
            {
                if (this.ReadState != System.Xml.ReadState.Initial)
                {
                    return false;
                }
                depth--;
            }
            else if (this.IsEmptyElement)
            {
                return false;
            }
            name = this.NameTable.Add(name);
            while (this.Read() && (this.Depth > depth))
            {
                if ((this.NodeType == XmlNodeType.Element) && Ref.Equal(name, this.Name))
                {
                    return true;
                }
            }
            return false;
        }

        public virtual bool ReadToDescendant(string localName, string namespaceURI)
        {
            if ((localName == null) || (localName.Length == 0))
            {
                throw XmlConvert.CreateInvalidNameArgumentException(localName, "localName");
            }
            if (namespaceURI == null)
            {
                throw new ArgumentNullException("namespaceURI");
            }
            int depth = this.Depth;
            if (this.NodeType != XmlNodeType.Element)
            {
                if (this.ReadState != System.Xml.ReadState.Initial)
                {
                    return false;
                }
                depth--;
            }
            else if (this.IsEmptyElement)
            {
                return false;
            }
            localName = this.NameTable.Add(localName);
            namespaceURI = this.NameTable.Add(namespaceURI);
            while (this.Read() && (this.Depth > depth))
            {
                if (((this.NodeType == XmlNodeType.Element) && Ref.Equal(localName, this.LocalName)) && Ref.Equal(namespaceURI, this.NamespaceURI))
                {
                    return true;
                }
            }
            return false;
        }

        public virtual bool ReadToFollowing(string name)
        {
            if ((name == null) || (name.Length == 0))
            {
                throw XmlConvert.CreateInvalidNameArgumentException(name, "name");
            }
            name = this.NameTable.Add(name);
            while (this.Read())
            {
                if ((this.NodeType == XmlNodeType.Element) && Ref.Equal(name, this.Name))
                {
                    return true;
                }
            }
            return false;
        }

        public virtual bool ReadToFollowing(string localName, string namespaceURI)
        {
            if ((localName == null) || (localName.Length == 0))
            {
                throw XmlConvert.CreateInvalidNameArgumentException(localName, "localName");
            }
            if (namespaceURI == null)
            {
                throw new ArgumentNullException("namespaceURI");
            }
            localName = this.NameTable.Add(localName);
            namespaceURI = this.NameTable.Add(namespaceURI);
            while (this.Read())
            {
                if (((this.NodeType == XmlNodeType.Element) && Ref.Equal(localName, this.LocalName)) && Ref.Equal(namespaceURI, this.NamespaceURI))
                {
                    return true;
                }
            }
            return false;
        }

        public virtual bool ReadToNextSibling(string name)
        {
            XmlNodeType nodeType;
            if ((name == null) || (name.Length == 0))
            {
                throw XmlConvert.CreateInvalidNameArgumentException(name, "name");
            }
            name = this.NameTable.Add(name);
            do
            {
                this.SkipSubtree();
                nodeType = this.NodeType;
                if ((nodeType == XmlNodeType.Element) && Ref.Equal(name, this.Name))
                {
                    return true;
                }
            }
            while ((nodeType != XmlNodeType.EndElement) && !this.EOF);
            return false;
        }

        public virtual bool ReadToNextSibling(string localName, string namespaceURI)
        {
            XmlNodeType nodeType;
            if ((localName == null) || (localName.Length == 0))
            {
                throw XmlConvert.CreateInvalidNameArgumentException(localName, "localName");
            }
            if (namespaceURI == null)
            {
                throw new ArgumentNullException("namespaceURI");
            }
            localName = this.NameTable.Add(localName);
            namespaceURI = this.NameTable.Add(namespaceURI);
            do
            {
                this.SkipSubtree();
                nodeType = this.NodeType;
                if (((nodeType == XmlNodeType.Element) && Ref.Equal(localName, this.LocalName)) && Ref.Equal(namespaceURI, this.NamespaceURI))
                {
                    return true;
                }
            }
            while ((nodeType != XmlNodeType.EndElement) && !this.EOF);
            return false;
        }

        public virtual int ReadValueChunk(char[] buffer, int index, int count)
        {
            throw new NotSupportedException(Res.GetString("Xml_ReadValueChunkNotSupported"));
        }

        public abstract void ResolveEntity();
        private void SetNamespacesFlag(XmlTextWriter xtw)
        {
            XmlTextReader reader = this as XmlTextReader;
            if (reader != null)
            {
                xtw.Namespaces = reader.Namespaces;
            }
            else
            {
                XmlValidatingReader reader2 = this as XmlValidatingReader;
                if (reader2 != null)
                {
                    xtw.Namespaces = reader2.Namespaces;
                }
            }
        }

        private bool SetupReadElementContentAsXxx(string methodName)
        {
            if (this.NodeType != XmlNodeType.Element)
            {
                throw this.CreateReadElementContentAsException(methodName);
            }
            bool isEmptyElement = this.IsEmptyElement;
            this.Read();
            if (isEmptyElement)
            {
                return false;
            }
            switch (this.NodeType)
            {
                case XmlNodeType.EndElement:
                    this.Read();
                    return false;

                case XmlNodeType.Element:
                    throw new XmlException("Xml_MixedReadElementContentAs", string.Empty, this as IXmlLineInfo);
            }
            return true;
        }

        public virtual void Skip()
        {
            this.SkipSubtree();
        }

        private void SkipSubtree()
        {
            if (this.ReadState == System.Xml.ReadState.Interactive)
            {
                this.MoveToElement();
                if ((this.NodeType == XmlNodeType.Element) && !this.IsEmptyElement)
                {
                    int depth = this.Depth;
                    while (this.Read() && (depth < this.Depth))
                    {
                    }
                    if (this.NodeType == XmlNodeType.EndElement)
                    {
                        this.Read();
                    }
                }
                else
                {
                    this.Read();
                }
            }
        }

        void IDisposable.Dispose()
        {
            this.Dispose(true);
        }

        private void WriteAttributeValue(XmlTextWriter xtw)
        {
            string name = this.Name;
            while (this.ReadAttributeValue())
            {
                if (this.NodeType == XmlNodeType.EntityReference)
                {
                    xtw.WriteEntityRef(this.Name);
                }
                else
                {
                    xtw.WriteString(this.Value);
                }
            }
            this.MoveToAttribute(name);
        }

        private void WriteNode(XmlTextWriter xtw, bool defattr)
        {
            int num = (this.NodeType == XmlNodeType.None) ? -1 : this.Depth;
            while (this.Read() && (num < this.Depth))
            {
                switch (this.NodeType)
                {
                    case XmlNodeType.Element:
                        xtw.WriteStartElement(this.Prefix, this.LocalName, this.NamespaceURI);
                        xtw.QuoteChar = this.QuoteChar;
                        xtw.WriteAttributes(this, defattr);
                        if (this.IsEmptyElement)
                        {
                            xtw.WriteEndElement();
                        }
                        break;

                    case XmlNodeType.Text:
                        xtw.WriteString(this.Value);
                        break;

                    case XmlNodeType.CDATA:
                        xtw.WriteCData(this.Value);
                        break;

                    case XmlNodeType.EntityReference:
                        xtw.WriteEntityRef(this.Name);
                        break;

                    case XmlNodeType.ProcessingInstruction:
                    case XmlNodeType.XmlDeclaration:
                        xtw.WriteProcessingInstruction(this.Name, this.Value);
                        break;

                    case XmlNodeType.Comment:
                        xtw.WriteComment(this.Value);
                        break;

                    case XmlNodeType.DocumentType:
                        xtw.WriteDocType(this.Name, this.GetAttribute("PUBLIC"), this.GetAttribute("SYSTEM"), this.Value);
                        break;

                    case XmlNodeType.Whitespace:
                    case XmlNodeType.SignificantWhitespace:
                        xtw.WriteWhitespace(this.Value);
                        break;

                    case XmlNodeType.EndElement:
                        xtw.WriteFullEndElement();
                        break;
                }
            }
            if ((num == this.Depth) && (this.NodeType == XmlNodeType.EndElement))
            {
                this.Read();
            }
        }

        public abstract int AttributeCount { get; }

        public abstract string BaseURI { get; }

        public virtual bool CanReadBinaryContent =>
            false;

        public virtual bool CanReadValueChunk =>
            false;

        public virtual bool CanResolveEntity =>
            false;

        private object debuggerDisplayProxy =>
            new DebuggerDisplayProxy(this);

        public abstract int Depth { get; }

        public abstract bool EOF { get; }

        public virtual bool HasAttributes =>
            (this.AttributeCount > 0);

        public abstract bool HasValue { get; }

        public virtual bool IsDefault =>
            false;

        public abstract bool IsEmptyElement { get; }

        public virtual string this[int i] =>
            this.GetAttribute(i);

        public virtual string this[string name] =>
            this.GetAttribute(name);

        public virtual string this[string name, string namespaceURI] =>
            this.GetAttribute(name, namespaceURI);

        public abstract string LocalName { get; }

        public virtual string Name
        {
            get
            {
                if (this.Prefix.Length == 0)
                {
                    return this.LocalName;
                }
                return this.NameTable.Add(this.Prefix + ":" + this.LocalName);
            }
        }

        internal virtual XmlNamespaceManager NamespaceManager =>
            null;

        public abstract string NamespaceURI { get; }

        public abstract XmlNameTable NameTable { get; }

        public abstract XmlNodeType NodeType { get; }

        public abstract string Prefix { get; }

        public virtual char QuoteChar =>
            '"';

        public abstract System.Xml.ReadState ReadState { get; }

        public virtual IXmlSchemaInfo SchemaInfo =>
            (this as IXmlSchemaInfo);

        public virtual XmlReaderSettings Settings =>
            null;

        public abstract string Value { get; }

        public virtual Type ValueType =>
            typeof(string);

        public virtual string XmlLang =>
            string.Empty;

        public virtual System.Xml.XmlSpace XmlSpace =>
            System.Xml.XmlSpace.None;

        [StructLayout(LayoutKind.Sequential), DebuggerDisplay("{ToString()}")]
        private struct DebuggerDisplayProxy
        {
            private XmlReader reader;
            internal DebuggerDisplayProxy(XmlReader reader)
            {
                this.reader = reader;
            }

            public override string ToString()
            {
                XmlNodeType nodeType = this.reader.NodeType;
                string str = nodeType.ToString();
                switch (nodeType)
                {
                    case XmlNodeType.Element:
                    case XmlNodeType.EntityReference:
                    case XmlNodeType.EndElement:
                    case XmlNodeType.EndEntity:
                    {
                        object obj2 = str;
                        return string.Concat(new object[] { obj2, ", Name=\"", this.reader.Name, '"' });
                    }
                    case XmlNodeType.Attribute:
                    case XmlNodeType.ProcessingInstruction:
                    {
                        object obj3 = str;
                        return string.Concat(new object[] { obj3, ", Name=\"", this.reader.Name, "\", Value=\"", XmlConvert.EscapeValueForDebuggerDisplay(this.reader.Value), '"' });
                    }
                    case XmlNodeType.Text:
                    case XmlNodeType.CDATA:
                    case XmlNodeType.Comment:
                    case XmlNodeType.Whitespace:
                    case XmlNodeType.SignificantWhitespace:
                    case XmlNodeType.XmlDeclaration:
                    {
                        object obj4 = str;
                        return string.Concat(new object[] { obj4, ", Value=\"", XmlConvert.EscapeValueForDebuggerDisplay(this.reader.Value), '"' });
                    }
                    case XmlNodeType.Entity:
                    case XmlNodeType.Document:
                    case XmlNodeType.DocumentFragment:
                    case XmlNodeType.Notation:
                        return str;

                    case XmlNodeType.DocumentType:
                    {
                        object obj5 = str + ", Name=\"" + this.reader.Name + "'";
                        object obj6 = string.Concat(new object[] { obj5, ", SYSTEM=\"", this.reader.GetAttribute("SYSTEM"), '"' });
                        object obj7 = string.Concat(new object[] { obj6, ", PUBLIC=\"", this.reader.GetAttribute("PUBLIC"), '"' });
                        return string.Concat(new object[] { obj7, ", Value=\"", XmlConvert.EscapeValueForDebuggerDisplay(this.reader.Value), '"' });
                    }
                }
                return str;
            }
        }
    }
}

