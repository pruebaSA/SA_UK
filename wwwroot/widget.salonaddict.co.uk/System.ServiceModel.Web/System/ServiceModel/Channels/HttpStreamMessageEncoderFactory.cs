namespace System.ServiceModel.Channels
{
    using System;
    using System.IO;
    using System.ServiceModel;
    using System.Xml;

    internal class HttpStreamMessageEncoderFactory : MessageEncoderFactory
    {
        private HttpStreamMessageEncoder encoder;

        public HttpStreamMessageEncoderFactory(XmlDictionaryReaderQuotas quotas)
        {
            this.encoder = new HttpStreamMessageEncoder(quotas);
        }

        public override MessageEncoder CreateSessionEncoder() => 
            this.encoder;

        public override MessageEncoder Encoder =>
            this.encoder;

        public override System.ServiceModel.Channels.MessageVersion MessageVersion =>
            System.ServiceModel.Channels.MessageVersion.None;

        internal class HttpStreamMessageEncoder : MessageEncoder
        {
            private string maxReceivedMessageSizeExceededResourceString;
            private string maxSentMessageSizeExceededResourceString;
            private XmlDictionaryReaderQuotas quotas;

            public HttpStreamMessageEncoder(XmlDictionaryReaderQuotas quotas)
            {
                this.quotas = quotas;
                this.maxSentMessageSizeExceededResourceString = SR2.MaxSentMessageSizeExceeded;
                this.maxReceivedMessageSizeExceededResourceString = SR2.MaxReceivedMessageSizeExceeded;
            }

            public override bool IsContentTypeSupported(string contentType) => 
                true;

            public override Message ReadMessage(ArraySegment<byte> buffer, BufferManager bufferManager, string contentType)
            {
                if (bufferManager == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("bufferManager");
                }
                using (BufferedOutputStream stream = new BufferedOutputStream(this.maxReceivedMessageSizeExceededResourceString, 0, 0x7fffffff, bufferManager))
                {
                    stream.Write(buffer.Array, 0, buffer.Count);
                    Message message = this.ReadMessage(stream.ToMemoryStream(), 0x7fffffff, contentType);
                    bufferManager.ReturnBuffer(buffer.Array);
                    return message;
                }
            }

            public override Message ReadMessage(Stream stream, int maxSizeOfHeaders, string contentType)
            {
                if (stream == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("stream");
                }
                Message message = Message.CreateMessage((XmlDictionaryReader) new HttpStreamXmlDictionaryReader(stream, this.quotas), maxSizeOfHeaders, System.ServiceModel.Channels.MessageVersion.None);
                message.Properties.Encoder = this;
                return message;
            }

            public override void WriteMessage(Message message, Stream stream)
            {
                if (message == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("message");
                }
                if (stream == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("stream");
                }
                message.Properties.Encoder = this;
                using (HttpStreamXmlDictionaryWriter writer = new HttpStreamXmlDictionaryWriter(stream))
                {
                    message.WriteMessage((XmlDictionaryWriter) writer);
                    writer.Flush();
                }
            }

            public override ArraySegment<byte> WriteMessage(Message message, int maxMessageSize, BufferManager bufferManager, int messageOffset)
            {
                if (message == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("message");
                }
                if (bufferManager == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("bufferManager");
                }
                if (maxMessageSize < 0)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("maxMessageSize"));
                }
                using (BufferedOutputStream stream = new BufferedOutputStream(this.maxSentMessageSizeExceededResourceString, 0, maxMessageSize, bufferManager))
                {
                    int num;
                    stream.Skip(messageOffset);
                    this.WriteMessage(message, stream);
                    return new ArraySegment<byte>(stream.ToArray(out num), 0, num - messageOffset);
                }
            }

            public override string ContentType =>
                null;

            public override string MediaType =>
                null;

            public override System.ServiceModel.Channels.MessageVersion MessageVersion =>
                System.ServiceModel.Channels.MessageVersion.None;
        }
    }
}

