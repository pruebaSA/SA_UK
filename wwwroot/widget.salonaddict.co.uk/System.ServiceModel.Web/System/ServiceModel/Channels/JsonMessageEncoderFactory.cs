namespace System.ServiceModel.Channels
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Runtime.Serialization.Json;
    using System.ServiceModel;
    using System.ServiceModel.Diagnostics;
    using System.Text;
    using System.Xml;

    internal class JsonMessageEncoderFactory : MessageEncoderFactory
    {
        private static readonly TextMessageEncoderFactory.ContentEncoding[] ApplicationJsonContentEncoding = GetContentEncodingMap("application/json");
        private JsonMessageEncoder messageEncoder;

        public JsonMessageEncoderFactory(Encoding writeEncoding, int maxReadPoolSize, int maxWritePoolSize, XmlDictionaryReaderQuotas quotas)
        {
            this.messageEncoder = new JsonMessageEncoder(writeEncoding, maxReadPoolSize, maxWritePoolSize, quotas);
        }

        private static TextMessageEncoderFactory.ContentEncoding[] GetContentEncodingMap(string mediaType)
        {
            Encoding[] supportedEncodings = TextMessageEncoderFactory.GetSupportedEncodings();
            TextMessageEncoderFactory.ContentEncoding[] encodingArray2 = new TextMessageEncoderFactory.ContentEncoding[supportedEncodings.Length];
            for (int i = 0; i < supportedEncodings.Length; i++)
            {
                encodingArray2[i] = new TextMessageEncoderFactory.ContentEncoding { 
                    contentType = GetContentType(mediaType, supportedEncodings[i]),
                    encoding = supportedEncodings[i]
                };
            }
            return encodingArray2;
        }

        internal static string GetContentType(WebMessageEncodingBindingElement encodingElement)
        {
            if (encodingElement == null)
            {
                return GetContentType("application/json", TextEncoderDefaults.Encoding);
            }
            return GetContentType("application/json", encodingElement.WriteEncoding);
        }

        private static string GetContentType(string mediaType, Encoding encoding) => 
            string.Format(CultureInfo.InvariantCulture, "{0}; charset={1}", new object[] { mediaType, TextEncoderDefaults.EncodingToCharSet(encoding) });

        public override MessageEncoder Encoder =>
            this.messageEncoder;

        public override System.ServiceModel.Channels.MessageVersion MessageVersion =>
            this.messageEncoder.MessageVersion;

        private class JsonMessageEncoder : MessageEncoder
        {
            private SynchronizedPool<JsonBufferedMessageData> bufferedReaderPool;
            private SynchronizedPool<JsonBufferedMessageWriter> bufferedWriterPool;
            private string contentType;
            private const int maxPooledXmlReadersPerMessage = 2;
            private int maxReadPoolSize;
            private int maxWritePoolSize;
            private OnXmlDictionaryReaderClose onStreamedReaderClose;
            private XmlDictionaryReaderQuotas readerQuotas;
            private SynchronizedPool<RecycledMessageState> recycledStatePool;
            private SynchronizedPool<XmlDictionaryReader> streamedReaderPool;
            private SynchronizedPool<XmlDictionaryWriter> streamedWriterPool;
            private object thisLock;
            private Encoding writeEncoding;

            public JsonMessageEncoder(Encoding writeEncoding, int maxReadPoolSize, int maxWritePoolSize, XmlDictionaryReaderQuotas quotas)
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
                this.readerQuotas = new XmlDictionaryReaderQuotas();
                this.onStreamedReaderClose = new OnXmlDictionaryReaderClose(this.ReturnStreamedReader);
                quotas.CopyTo(this.readerQuotas);
                this.contentType = JsonMessageEncoderFactory.GetContentType("application/json", writeEncoding);
            }

            internal override bool IsCharSetSupported(string charSet)
            {
                Encoding encoding;
                return TextEncoderDefaults.TryGetEncoding(charSet, out encoding);
            }

            public override bool IsContentTypeSupported(string contentType)
            {
                if (contentType == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("contentType");
                }
                return this.IsJsonContentType(contentType);
            }

            private bool IsJsonContentType(string contentType)
            {
                if (!base.IsContentTypeSupported(contentType, "application/json", "application/json"))
                {
                    return base.IsContentTypeSupported(contentType, "text/json", "text/json");
                }
                return true;
            }

            public override Message ReadMessage(ArraySegment<byte> buffer, BufferManager bufferManager, string contentType)
            {
                if (bufferManager == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("bufferManager"));
                }
                JsonBufferedMessageData messageData = this.TakeBufferedReader();
                messageData.Encoding = TextMessageEncoderFactory.GetEncodingFromContentType(contentType, JsonMessageEncoderFactory.ApplicationJsonContentEncoding);
                messageData.Open(buffer, bufferManager);
                RecycledMessageState recycledMessageState = messageData.TakeMessageState();
                if (recycledMessageState == null)
                {
                    recycledMessageState = new RecycledMessageState();
                }
                Message message = new BufferedMessage(messageData, recycledMessageState) {
                    Properties = { Encoder = this }
                };
                if (MessageLogger.LogMessagesAtTransportLevel)
                {
                    MessageLogger.LogMessage(ref message, MessageLoggingSource.TransportReceive);
                }
                return message;
            }

            public override Message ReadMessage(Stream stream, int maxSizeOfHeaders, string contentType)
            {
                if (stream == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("stream"));
                }
                Message message = Message.CreateMessage((XmlReader) this.TakeStreamedReader(stream, TextMessageEncoderFactory.GetEncodingFromContentType(contentType, JsonMessageEncoderFactory.ApplicationJsonContentEncoding)), maxSizeOfHeaders, System.ServiceModel.Channels.MessageVersion.None);
                message.Properties.Encoder = this;
                if (MessageLogger.LogMessagesAtTransportLevel)
                {
                    MessageLogger.LogMessage(ref message, MessageLoggingSource.TransportReceive);
                }
                return message;
            }

            private void ReturnBufferedData(JsonBufferedMessageData messageData)
            {
                this.bufferedReaderPool.Return(messageData);
            }

            private void ReturnMessageWriter(JsonBufferedMessageWriter messageWriter)
            {
                this.bufferedWriterPool.Return(messageWriter);
            }

            private void ReturnStreamedReader(XmlDictionaryReader xmlReader)
            {
                this.streamedReaderPool.Return(xmlReader);
            }

            private void ReturnStreamedWriter(XmlWriter xmlWriter)
            {
                xmlWriter.Close();
                this.streamedWriterPool.Return((XmlDictionaryWriter) xmlWriter);
            }

            private JsonBufferedMessageData TakeBufferedReader()
            {
                if (this.bufferedReaderPool == null)
                {
                    lock (this.ThisLock)
                    {
                        if (this.bufferedReaderPool == null)
                        {
                            this.bufferedReaderPool = new SynchronizedPool<JsonBufferedMessageData>(this.maxReadPoolSize);
                        }
                    }
                }
                JsonBufferedMessageData data = this.bufferedReaderPool.Take();
                if (data == null)
                {
                    data = new JsonBufferedMessageData(this, 2);
                }
                return data;
            }

            private JsonBufferedMessageWriter TakeBufferedWriter()
            {
                if (this.bufferedWriterPool == null)
                {
                    lock (this.ThisLock)
                    {
                        if (this.bufferedWriterPool == null)
                        {
                            this.bufferedWriterPool = new SynchronizedPool<JsonBufferedMessageWriter>(this.maxWritePoolSize);
                        }
                    }
                }
                JsonBufferedMessageWriter writer = this.bufferedWriterPool.Take();
                if (writer == null)
                {
                    writer = new JsonBufferedMessageWriter(this);
                }
                return writer;
            }

            private XmlDictionaryReader TakeStreamedReader(Stream stream, Encoding enc)
            {
                if (this.streamedReaderPool == null)
                {
                    lock (this.ThisLock)
                    {
                        if (this.streamedReaderPool == null)
                        {
                            this.streamedReaderPool = new SynchronizedPool<XmlDictionaryReader>(this.maxReadPoolSize);
                        }
                    }
                }
                XmlDictionaryReader reader = this.streamedReaderPool.Take();
                if (reader == null)
                {
                    return JsonReaderWriterFactory.CreateJsonReader(stream, enc, this.readerQuotas, this.onStreamedReaderClose);
                }
                ((IXmlJsonReaderInitializer) reader).SetInput(stream, enc, this.readerQuotas, this.onStreamedReaderClose);
                return reader;
            }

            private XmlDictionaryWriter TakeStreamedWriter(Stream stream)
            {
                if (this.streamedWriterPool == null)
                {
                    lock (this.ThisLock)
                    {
                        if (this.streamedWriterPool == null)
                        {
                            this.streamedWriterPool = new SynchronizedPool<XmlDictionaryWriter>(this.maxWritePoolSize);
                        }
                    }
                }
                XmlDictionaryWriter writer = this.streamedWriterPool.Take();
                if (writer == null)
                {
                    return JsonReaderWriterFactory.CreateJsonWriter(stream, this.writeEncoding, false);
                }
                ((IXmlJsonWriterInitializer) writer).SetOutput(stream, this.writeEncoding, false);
                return writer;
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
                message.Properties.Encoder = this;
                XmlDictionaryWriter writer = this.TakeStreamedWriter(stream);
                writer.WriteStartDocument();
                message.WriteMessage(writer);
                writer.WriteEndDocument();
                writer.Flush();
                this.ReturnStreamedWriter(writer);
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
                JsonBufferedMessageWriter messageWriter = this.TakeBufferedWriter();
                ArraySegment<byte> segment = messageWriter.WriteMessage(message, bufferManager, messageOffset, maxMessageSize);
                this.ReturnMessageWriter(messageWriter);
                if (MessageLogger.LogMessagesAtTransportLevel)
                {
                    XmlDictionaryReader reader = JsonReaderWriterFactory.CreateJsonReader(segment.Array, segment.Offset, segment.Count, null, XmlDictionaryReaderQuotas.Max, null);
                    MessageLogger.LogMessage(ref message, reader, MessageLoggingSource.TransportSend);
                }
                return segment;
            }

            public override string ContentType =>
                this.contentType;

            public override string MediaType =>
                "application/json";

            public override System.ServiceModel.Channels.MessageVersion MessageVersion =>
                System.ServiceModel.Channels.MessageVersion.None;

            private SynchronizedPool<RecycledMessageState> RecycledStatePool
            {
                get
                {
                    if (this.recycledStatePool == null)
                    {
                        lock (this.ThisLock)
                        {
                            if (this.recycledStatePool == null)
                            {
                                this.recycledStatePool = new SynchronizedPool<RecycledMessageState>(this.maxReadPoolSize);
                            }
                        }
                    }
                    return this.recycledStatePool;
                }
            }

            private object ThisLock =>
                this.thisLock;

            private class JsonBufferedMessageData : BufferedMessageData
            {
                private System.Text.Encoding encoding;
                private JsonMessageEncoderFactory.JsonMessageEncoder messageEncoder;
                private OnXmlDictionaryReaderClose onClose;
                private Pool<XmlDictionaryReader> readerPool;

                public JsonBufferedMessageData(JsonMessageEncoderFactory.JsonMessageEncoder messageEncoder, int maxReaderPoolSize) : base(messageEncoder.RecycledStatePool)
                {
                    this.messageEncoder = messageEncoder;
                    this.readerPool = new Pool<XmlDictionaryReader>(maxReaderPoolSize);
                    this.onClose = new OnXmlDictionaryReaderClose(this.OnXmlReaderClosed);
                }

                protected override void OnClosed()
                {
                    this.messageEncoder.ReturnBufferedData(this);
                }

                protected override void ReturnXmlReader(XmlDictionaryReader xmlReader)
                {
                    if (xmlReader != null)
                    {
                        this.readerPool.Return(xmlReader);
                    }
                }

                protected override XmlDictionaryReader TakeXmlReader()
                {
                    ArraySegment<byte> buffer = base.Buffer;
                    XmlDictionaryReader reader = this.readerPool.Take();
                    if (reader == null)
                    {
                        return JsonReaderWriterFactory.CreateJsonReader(buffer.Array, buffer.Offset, buffer.Count, this.encoding, this.messageEncoder.readerQuotas, this.onClose);
                    }
                    ((IXmlJsonReaderInitializer) reader).SetInput(buffer.Array, buffer.Offset, buffer.Count, this.encoding, this.messageEncoder.readerQuotas, this.onClose);
                    return reader;
                }

                internal System.Text.Encoding Encoding
                {
                    set
                    {
                        this.encoding = value;
                    }
                }

                public override System.ServiceModel.Channels.MessageEncoder MessageEncoder =>
                    this.messageEncoder;

                public override XmlDictionaryReaderQuotas Quotas =>
                    this.messageEncoder.readerQuotas;
            }

            private class JsonBufferedMessageWriter : BufferedMessageWriter
            {
                private JsonMessageEncoderFactory.JsonMessageEncoder messageEncoder;
                private XmlDictionaryWriter returnedWriter;

                public JsonBufferedMessageWriter(JsonMessageEncoderFactory.JsonMessageEncoder messageEncoder)
                {
                    this.messageEncoder = messageEncoder;
                }

                protected override void OnWriteEndMessage(XmlDictionaryWriter writer)
                {
                    writer.WriteEndDocument();
                }

                protected override void OnWriteStartMessage(XmlDictionaryWriter writer)
                {
                    writer.WriteStartDocument();
                }

                protected override void ReturnXmlWriter(XmlDictionaryWriter writer)
                {
                    writer.Close();
                    if (this.returnedWriter == null)
                    {
                        this.returnedWriter = writer;
                    }
                }

                protected override XmlDictionaryWriter TakeXmlWriter(Stream stream)
                {
                    if (this.returnedWriter == null)
                    {
                        return JsonReaderWriterFactory.CreateJsonWriter(stream, this.messageEncoder.writeEncoding, false);
                    }
                    XmlDictionaryWriter returnedWriter = this.returnedWriter;
                    this.returnedWriter = null;
                    ((IXmlJsonWriterInitializer) returnedWriter).SetOutput(stream, this.messageEncoder.writeEncoding, false);
                    return returnedWriter;
                }
            }
        }
    }
}

