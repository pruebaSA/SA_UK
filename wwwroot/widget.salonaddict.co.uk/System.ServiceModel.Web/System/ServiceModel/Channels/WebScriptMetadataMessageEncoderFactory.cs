namespace System.ServiceModel.Channels
{
    using System;
    using System.IO;
    using System.ServiceModel;
    using System.ServiceModel.Diagnostics;
    using System.Text;
    using System.Xml;

    internal class WebScriptMetadataMessageEncoderFactory : MessageEncoderFactory
    {
        private const string applicationJavaScriptMediaType = "application/x-javascript";
        private WebScriptMetadataMessageEncoder messageEncoder;

        public WebScriptMetadataMessageEncoderFactory(XmlDictionaryReaderQuotas quotas)
        {
            this.messageEncoder = new WebScriptMetadataMessageEncoder(quotas);
        }

        public override MessageEncoder Encoder =>
            this.messageEncoder;

        public override System.ServiceModel.Channels.MessageVersion MessageVersion =>
            this.messageEncoder.MessageVersion;

        private class WebScriptMetadataMessageEncoder : MessageEncoder
        {
            private string contentType;
            private MessageEncoder innerReadMessageEncoder;
            private string mediaType;
            private XmlDictionaryReaderQuotas readerQuotas = new XmlDictionaryReaderQuotas();
            private static UTF8Encoding UTF8EncodingWithoutByteOrderMark = new UTF8Encoding(false);

            public WebScriptMetadataMessageEncoder(XmlDictionaryReaderQuotas quotas)
            {
                quotas.CopyTo(this.readerQuotas);
                this.mediaType = this.contentType = "application/x-javascript";
                this.innerReadMessageEncoder = new TextMessageEncodingBindingElement(System.ServiceModel.Channels.MessageVersion.None, Encoding.UTF8).CreateMessageEncoderFactory().Encoder;
            }

            private XmlDictionaryWriter CreateWriter(Stream stream)
            {
                XmlWriterSettings settings = new XmlWriterSettings {
                    OmitXmlDeclaration = true,
                    Encoding = UTF8EncodingWithoutByteOrderMark
                };
                return XmlDictionaryWriter.CreateDictionaryWriter(XmlWriter.Create(stream, settings));
            }

            public override bool IsContentTypeSupported(string contentType) => 
                this.innerReadMessageEncoder.IsContentTypeSupported(contentType);

            public override Message ReadMessage(ArraySegment<byte> buffer, BufferManager bufferManager, string contentType) => 
                this.innerReadMessageEncoder.ReadMessage(buffer, bufferManager, contentType);

            public override Message ReadMessage(Stream stream, int maxSizeOfHeaders, string contentType) => 
                this.innerReadMessageEncoder.ReadMessage(stream, maxSizeOfHeaders, contentType);

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
                message.Properties.Encoder = this;
                XmlDictionaryWriter writer = this.CreateWriter(stream);
                writer.WriteStartDocument();
                message.WriteMessage(writer);
                writer.WriteEndDocument();
                writer.Flush();
                writer.Close();
                if (MessageLogger.LogMessagesAtTransportLevel)
                {
                    MessageLogger.LogMessage(ref message, MessageLoggingSource.TransportSend);
                }
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
                message.Properties.Encoder = this;
                ArraySegment<byte> segment = new WebScriptMetadataBufferedMessageWriter(this).WriteMessage(message, bufferManager, messageOffset, maxMessageSize);
                if (MessageLogger.LogMessagesAtTransportLevel)
                {
                    MessageLogger.LogMessage(ref message, MessageLoggingSource.TransportSend);
                }
                return segment;
            }

            public override string ContentType =>
                this.contentType;

            public override string MediaType =>
                this.mediaType;

            public override System.ServiceModel.Channels.MessageVersion MessageVersion =>
                System.ServiceModel.Channels.MessageVersion.None;

            private class WebScriptMetadataBufferedMessageWriter : BufferedMessageWriter
            {
                private WebScriptMetadataMessageEncoderFactory.WebScriptMetadataMessageEncoder messageEncoder;

                public WebScriptMetadataBufferedMessageWriter(WebScriptMetadataMessageEncoderFactory.WebScriptMetadataMessageEncoder messageEncoder)
                {
                    this.messageEncoder = messageEncoder;
                }

                protected override void OnWriteEndMessage(XmlDictionaryWriter writer)
                {
                }

                protected override void OnWriteStartMessage(XmlDictionaryWriter writer)
                {
                }

                protected override void ReturnXmlWriter(XmlDictionaryWriter writer)
                {
                    writer.Close();
                }

                protected override XmlDictionaryWriter TakeXmlWriter(Stream stream) => 
                    this.messageEncoder.CreateWriter(stream);
            }
        }
    }
}

