namespace System.ServiceModel.Channels
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.ServiceModel;
    using System.ServiceModel.Diagnostics;
    using System.Text;
    using System.Xml;

    internal class WebMessageEncoderFactory : MessageEncoderFactory
    {
        private WebMessageEncoder messageEncoder;

        public WebMessageEncoderFactory(Encoding writeEncoding, int maxReadPoolSize, int maxWritePoolSize, XmlDictionaryReaderQuotas quotas, WebContentTypeMapper contentTypeMapper)
        {
            this.messageEncoder = new WebMessageEncoder(writeEncoding, maxReadPoolSize, maxWritePoolSize, quotas, contentTypeMapper);
        }

        public override MessageEncoder Encoder =>
            this.messageEncoder;

        public override System.ServiceModel.Channels.MessageVersion MessageVersion =>
            this.messageEncoder.MessageVersion;

        private class WebMessageEncoder : MessageEncoder
        {
            private WebContentTypeMapper contentTypeMapper;
            private string defaultContentType;
            private const string defaultMediaType = "application/xml";
            private MessageEncoder jsonMessageEncoder;
            private int maxReadPoolSize;
            private int maxWritePoolSize;
            private MessageEncoder rawMessageEncoder;
            private XmlDictionaryReaderQuotas readerQuotas;
            private MessageEncoder textMessageEncoder;
            private object thisLock;
            private Encoding writeEncoding;

            public WebMessageEncoder(Encoding writeEncoding, int maxReadPoolSize, int maxWritePoolSize, XmlDictionaryReaderQuotas quotas, WebContentTypeMapper contentTypeMapper)
            {
                if (writeEncoding == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("writeEncoding");
                }
                this.thisLock = new object();
                TextEncoderDefaults.ValidateEncoding(writeEncoding);
                this.writeEncoding = writeEncoding;
                this.maxReadPoolSize = maxReadPoolSize;
                this.maxWritePoolSize = maxWritePoolSize;
                this.contentTypeMapper = contentTypeMapper;
                this.readerQuotas = new XmlDictionaryReaderQuotas();
                quotas.CopyTo(this.readerQuotas);
                this.defaultContentType = string.Format(CultureInfo.InvariantCulture, "{0}; charset={1}", new object[] { "application/xml", TextEncoderDefaults.EncodingToCharSet(writeEncoding) });
            }

            private WebContentFormat ExtractFormatFromMessage(Message message)
            {
                object obj2;
                message.Properties.TryGetValue("WebBodyFormatMessageProperty", out obj2);
                if (obj2 != null)
                {
                    WebBodyFormatMessageProperty property = obj2 as WebBodyFormatMessageProperty;
                    if ((property != null) && (property.Format != WebContentFormat.Default))
                    {
                        return property.Format;
                    }
                }
                return WebContentFormat.Xml;
            }

            private WebContentFormat GetFormatForContentType(string contentType)
            {
                WebContentFormat format;
                if (this.TryGetContentTypeMapping(contentType, out format) && (format != WebContentFormat.Default))
                {
                    return format;
                }
                if (contentType != null)
                {
                    if (this.JsonMessageEncoder.IsContentTypeSupported(contentType))
                    {
                        return WebContentFormat.Json;
                    }
                    if (this.TextMessageEncoder.IsContentTypeSupported(contentType))
                    {
                        return WebContentFormat.Xml;
                    }
                }
                return WebContentFormat.Raw;
            }

            internal override bool IsCharSetSupported(string charSet)
            {
                Encoding encoding;
                return TextEncoderDefaults.TryGetEncoding(charSet, out encoding);
            }

            public override bool IsContentTypeSupported(string contentType)
            {
                WebContentFormat format;
                if (contentType == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("contentType");
                }
                if ((!this.TryGetContentTypeMapping(contentType, out format) || (format == WebContentFormat.Default)) && (!this.RawMessageEncoder.IsContentTypeSupported(contentType) && !this.JsonMessageEncoder.IsContentTypeSupported(contentType)))
                {
                    return this.TextMessageEncoder.IsContentTypeSupported(contentType);
                }
                return true;
            }

            public override Message ReadMessage(ArraySegment<byte> buffer, BufferManager bufferManager, string contentType)
            {
                Message message;
                if (bufferManager == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("bufferManager"));
                }
                switch (this.GetFormatForContentType(contentType))
                {
                    case WebContentFormat.Xml:
                        message = this.TextMessageEncoder.ReadMessage(buffer, bufferManager, contentType);
                        message.Properties.Add("WebBodyFormatMessageProperty", WebBodyFormatMessageProperty.XmlProperty);
                        return message;

                    case WebContentFormat.Json:
                        message = this.JsonMessageEncoder.ReadMessage(buffer, bufferManager, contentType);
                        message.Properties.Add("WebBodyFormatMessageProperty", WebBodyFormatMessageProperty.JsonProperty);
                        return message;

                    case WebContentFormat.Raw:
                        message = this.RawMessageEncoder.ReadMessage(buffer, bufferManager, contentType);
                        message.Properties.Add("WebBodyFormatMessageProperty", WebBodyFormatMessageProperty.RawProperty);
                        return message;
                }
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperInternal(false);
            }

            public override Message ReadMessage(Stream stream, int maxSizeOfHeaders, string contentType)
            {
                Message message;
                if (stream == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("stream"));
                }
                switch (this.GetFormatForContentType(contentType))
                {
                    case WebContentFormat.Xml:
                        message = this.TextMessageEncoder.ReadMessage(stream, maxSizeOfHeaders, contentType);
                        message.Properties.Add("WebBodyFormatMessageProperty", WebBodyFormatMessageProperty.XmlProperty);
                        return message;

                    case WebContentFormat.Json:
                        message = this.JsonMessageEncoder.ReadMessage(stream, maxSizeOfHeaders, contentType);
                        message.Properties.Add("WebBodyFormatMessageProperty", WebBodyFormatMessageProperty.JsonProperty);
                        return message;

                    case WebContentFormat.Raw:
                        message = this.RawMessageEncoder.ReadMessage(stream, maxSizeOfHeaders, contentType);
                        message.Properties.Add("WebBodyFormatMessageProperty", WebBodyFormatMessageProperty.RawProperty);
                        return message;
                }
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperInternal(false);
            }

            private bool TryGetContentTypeMapping(string contentType, out WebContentFormat format)
            {
                bool flag;
                if (this.contentTypeMapper == null)
                {
                    format = WebContentFormat.Default;
                    return false;
                }
                try
                {
                    format = this.contentTypeMapper.GetMessageFormatForContentType(contentType);
                    if (!WebContentFormatHelper.IsDefined(format))
                    {
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentException(SR2.GetString(SR2.UnknownWebEncodingFormat, new object[] { contentType, (WebContentFormat) format })));
                    }
                    flag = true;
                }
                catch (Exception exception)
                {
                    if (DiagnosticUtility.IsFatal(exception))
                    {
                        throw;
                    }
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new CommunicationException(SR2.GetString(SR2.ErrorEncounteredInContentTypeMapper, new object[0]), exception));
                }
                return flag;
            }

            public override void WriteMessage(Message message, Stream stream)
            {
                if (message == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("message"));
                }
                if (stream == null)
                {
                    throw TraceUtility.ThrowHelperError(new ArgumentNullException("stream"), message);
                }
                base.ThrowIfMismatchedMessageVersion(message);
                switch (this.ExtractFormatFromMessage(message))
                {
                    case WebContentFormat.Xml:
                        this.TextMessageEncoder.WriteMessage(message, stream);
                        return;

                    case WebContentFormat.Json:
                        this.JsonMessageEncoder.WriteMessage(message, stream);
                        return;

                    case WebContentFormat.Raw:
                        this.RawMessageEncoder.WriteMessage(message, stream);
                        return;
                }
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperInternal(false);
            }

            public override ArraySegment<byte> WriteMessage(Message message, int maxMessageSize, BufferManager bufferManager, int messageOffset)
            {
                if (message == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("message"));
                }
                if (bufferManager == null)
                {
                    throw TraceUtility.ThrowHelperError(new ArgumentNullException("bufferManager"), message);
                }
                if (maxMessageSize < 0)
                {
                    throw TraceUtility.ThrowHelperError(new ArgumentOutOfRangeException("maxMessageSize", maxMessageSize, SR2.GetString(SR2.ValueMustBeNonNegative, new object[0])), message);
                }
                if ((messageOffset < 0) || (messageOffset > maxMessageSize))
                {
                    throw TraceUtility.ThrowHelperError(new ArgumentOutOfRangeException("messageOffset", messageOffset, SR2.GetString(SR2.JsonValueMustBeInRange, new object[] { 0, maxMessageSize })), message);
                }
                base.ThrowIfMismatchedMessageVersion(message);
                switch (this.ExtractFormatFromMessage(message))
                {
                    case WebContentFormat.Xml:
                        return this.TextMessageEncoder.WriteMessage(message, maxMessageSize, bufferManager, messageOffset);

                    case WebContentFormat.Json:
                        return this.JsonMessageEncoder.WriteMessage(message, maxMessageSize, bufferManager, messageOffset);

                    case WebContentFormat.Raw:
                        return this.RawMessageEncoder.WriteMessage(message, maxMessageSize, bufferManager, messageOffset);
                }
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperInternal(false);
            }

            public override string ContentType =>
                this.defaultContentType;

            private MessageEncoder JsonMessageEncoder
            {
                get
                {
                    if (this.jsonMessageEncoder == null)
                    {
                        lock (this.ThisLock)
                        {
                            if (this.jsonMessageEncoder == null)
                            {
                                this.jsonMessageEncoder = new JsonMessageEncoderFactory(this.writeEncoding, this.maxReadPoolSize, this.maxWritePoolSize, this.readerQuotas).Encoder;
                            }
                        }
                    }
                    return this.jsonMessageEncoder;
                }
            }

            public override string MediaType =>
                "application/xml";

            public override System.ServiceModel.Channels.MessageVersion MessageVersion =>
                System.ServiceModel.Channels.MessageVersion.None;

            private MessageEncoder RawMessageEncoder
            {
                get
                {
                    if (this.rawMessageEncoder == null)
                    {
                        lock (this.ThisLock)
                        {
                            if (this.rawMessageEncoder == null)
                            {
                                this.rawMessageEncoder = new HttpStreamMessageEncoderFactory(this.readerQuotas).Encoder;
                            }
                        }
                    }
                    return this.rawMessageEncoder;
                }
            }

            private MessageEncoder TextMessageEncoder
            {
                get
                {
                    if (this.textMessageEncoder == null)
                    {
                        lock (this.ThisLock)
                        {
                            if (this.textMessageEncoder == null)
                            {
                                this.textMessageEncoder = new TextMessageEncoderFactory(System.ServiceModel.Channels.MessageVersion.None, this.writeEncoding, this.maxReadPoolSize, this.maxWritePoolSize, this.readerQuotas).Encoder;
                            }
                        }
                    }
                    return this.textMessageEncoder;
                }
            }

            private object ThisLock =>
                this.thisLock;
        }
    }
}

