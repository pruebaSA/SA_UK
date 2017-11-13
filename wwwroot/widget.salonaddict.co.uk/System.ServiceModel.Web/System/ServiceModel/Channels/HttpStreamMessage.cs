namespace System.ServiceModel.Channels
{
    using System;
    using System.IO;
    using System.ServiceModel;
    using System.Xml;

    internal class HttpStreamMessage : Message
    {
        private BodyWriter bodyWriter;
        private MessageHeaders headers;
        private MessageProperties properties;
        internal const string StreamElementName = "Binary";

        public HttpStreamMessage(Stream stream)
        {
            if (stream == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("stream");
            }
            this.bodyWriter = new HttpStreamBodyWriter(stream);
            this.headers = new MessageHeaders(MessageVersion.None, 1);
            this.properties = new MessageProperties();
        }

        public HttpStreamMessage(MessageHeaders headers, MessageProperties properties, BodyWriter bodyWriter)
        {
            if (bodyWriter == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("bodyWriter");
            }
            this.headers = new MessageHeaders(headers);
            this.properties = new MessageProperties(properties);
            this.bodyWriter = bodyWriter;
        }

        private Exception CreateDisposedException() => 
            new ObjectDisposedException("", SR2.GetString(SR2.MessageClosed, new object[0]));

        protected override void OnBodyToString(XmlDictionaryWriter writer)
        {
            if (this.bodyWriter.IsBuffered)
            {
                this.bodyWriter.WriteBodyContents(writer);
            }
            else
            {
                writer.WriteString(SR2.GetString(SR2.MessageBodyIsStream, new object[0]));
            }
        }

        protected override void OnClose()
        {
            Exception exception = null;
            try
            {
                base.OnClose();
            }
            catch (Exception exception2)
            {
                if (DiagnosticUtility.IsFatal(exception2))
                {
                    throw;
                }
                exception = exception2;
            }
            try
            {
                if (this.properties != null)
                {
                    this.properties.Dispose();
                }
            }
            catch (Exception exception3)
            {
                if (DiagnosticUtility.IsFatal(exception3))
                {
                    throw;
                }
                if (exception == null)
                {
                    exception = exception3;
                }
            }
            if (exception != null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(exception);
            }
            this.bodyWriter = null;
        }

        protected override MessageBuffer OnCreateBufferedCopy(int maxBufferSize)
        {
            BodyWriter bodyWriter;
            if (this.bodyWriter.IsBuffered)
            {
                bodyWriter = this.bodyWriter;
            }
            else
            {
                bodyWriter = this.bodyWriter.CreateBufferedCopy(maxBufferSize);
            }
            return new HttpStreamMessageBuffer(this.Headers, new MessageProperties(this.Properties), bodyWriter);
        }

        protected override void OnWriteBodyContents(XmlDictionaryWriter writer)
        {
            this.bodyWriter.WriteBodyContents(writer);
        }

        public override MessageHeaders Headers
        {
            get
            {
                if (base.IsDisposed)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(this.CreateDisposedException());
                }
                return this.headers;
            }
        }

        public override bool IsEmpty =>
            false;

        public override bool IsFault =>
            false;

        public override MessageProperties Properties
        {
            get
            {
                if (base.IsDisposed)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(this.CreateDisposedException());
                }
                return this.properties;
            }
        }

        public override MessageVersion Version
        {
            get
            {
                if (base.IsDisposed)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(this.CreateDisposedException());
                }
                return MessageVersion.None;
            }
        }

        private class HttpStreamBodyWriter : BodyWriter
        {
            private Stream stream;
            private object thisLock;

            public HttpStreamBodyWriter(Stream stream) : base(false)
            {
                if (stream == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("stream");
                }
                this.stream = stream;
                this.thisLock = new object();
            }

            protected override BodyWriter OnCreateBufferedCopy(int maxBufferSize)
            {
                BodyWriter writer2;
                using (BufferedOutputStream stream = new BufferedOutputStream(SR2.MaxReceivedMessageSizeExceeded, maxBufferSize))
                {
                    using (HttpStreamXmlDictionaryWriter writer = new HttpStreamXmlDictionaryWriter(stream))
                    {
                        int num;
                        this.OnWriteBodyContents(writer);
                        writer.Flush();
                        writer2 = new BufferedBytesBodyWriter(stream.ToArray(out num), num);
                    }
                }
                return writer2;
            }

            protected override void OnWriteBodyContents(XmlDictionaryWriter writer)
            {
                lock (this.ThisLock)
                {
                    writer.WriteStartElement("Binary", string.Empty);
                    writer.WriteValue(new HttpStreamProvider(this.stream));
                    writer.WriteEndElement();
                }
            }

            private object ThisLock =>
                this.thisLock;

            private class BufferedBytesBodyWriter : BodyWriter
            {
                private byte[] array;
                private int size;

                public BufferedBytesBodyWriter(byte[] array, int size) : base(true)
                {
                    this.array = array;
                    this.size = size;
                }

                protected override void OnWriteBodyContents(XmlDictionaryWriter writer)
                {
                    writer.WriteStartElement("Binary", string.Empty);
                    writer.WriteBase64(this.array, 0, this.size);
                    writer.WriteEndElement();
                }
            }

            private class HttpStreamProvider : IStreamProvider
            {
                private Stream stream;

                internal HttpStreamProvider(Stream stream)
                {
                    this.stream = stream;
                }

                public Stream GetStream() => 
                    this.stream;

                public void ReleaseStream(Stream stream)
                {
                }
            }
        }

        private class HttpStreamMessageBuffer : MessageBuffer
        {
            private BodyWriter bodyWriter;
            private bool closed;
            private MessageHeaders headers;
            private MessageProperties properties;
            private object thisLock = new object();

            public HttpStreamMessageBuffer(MessageHeaders headers, MessageProperties properties, BodyWriter bodyWriter)
            {
                this.bodyWriter = bodyWriter;
                this.headers = headers;
                this.properties = properties;
            }

            public override void Close()
            {
                lock (this.ThisLock)
                {
                    if (!this.closed)
                    {
                        this.closed = true;
                        this.bodyWriter = null;
                        this.headers = null;
                        this.properties = null;
                    }
                }
            }

            private Exception CreateDisposedException() => 
                new ObjectDisposedException("", SR2.GetString(SR2.MessageBufferIsClosed, new object[0]));

            public override Message CreateMessage()
            {
                lock (this.ThisLock)
                {
                    if (this.closed)
                    {
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(this.CreateDisposedException());
                    }
                    return new HttpStreamMessage(this.headers, this.properties, this.bodyWriter);
                }
            }

            public override int BufferSize =>
                0;

            private object ThisLock =>
                this.thisLock;
        }
    }
}

