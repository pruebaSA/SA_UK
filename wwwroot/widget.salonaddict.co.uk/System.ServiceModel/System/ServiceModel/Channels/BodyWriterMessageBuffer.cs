namespace System.ServiceModel.Channels
{
    using System;
    using System.Collections.Generic;
    using System.ServiceModel;

    internal class BodyWriterMessageBuffer : MessageBuffer
    {
        private System.ServiceModel.Channels.BodyWriter bodyWriter;
        private bool closed;
        private MessageHeaders headers;
        private KeyValuePair<string, object>[] properties;
        private object thisLock = new object();

        public BodyWriterMessageBuffer(MessageHeaders headers, KeyValuePair<string, object>[] properties, System.ServiceModel.Channels.BodyWriter bodyWriter)
        {
            this.bodyWriter = bodyWriter;
            this.headers = new MessageHeaders(headers);
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

        public override Message CreateMessage()
        {
            lock (this.ThisLock)
            {
                if (this.closed)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(base.CreateBufferDisposedException());
                }
                return new BodyWriterMessage(this.headers, this.properties, this.bodyWriter);
            }
        }

        protected System.ServiceModel.Channels.BodyWriter BodyWriter =>
            this.bodyWriter;

        public override int BufferSize =>
            0;

        protected bool Closed =>
            this.closed;

        protected MessageHeaders Headers =>
            this.headers;

        protected KeyValuePair<string, object>[] Properties =>
            this.properties;

        protected object ThisLock =>
            this.thisLock;
    }
}

