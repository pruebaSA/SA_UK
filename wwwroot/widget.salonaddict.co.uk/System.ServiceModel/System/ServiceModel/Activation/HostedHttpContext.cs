namespace System.ServiceModel.Activation
{
    using System;
    using System.IO;
    using System.Net;
    using System.Security;
    using System.Security.Authentication.ExtendedProtection;
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Security;

    internal class HostedHttpContext : HttpRequestContext
    {
        internal const string OriginalHttpRequestUriPropertyName = "OriginalHttpRequestUri";
        private HostedRequestContainer requestContainer;
        private HostedHttpRequestAsyncResult result;

        public HostedHttpContext(HttpChannelListener listener, HostedHttpRequestAsyncResult result) : base(listener, null)
        {
            this.result = result;
        }

        private void CloseHostedRequestContainer()
        {
            if (this.requestContainer != null)
            {
                this.requestContainer.Close();
                this.requestContainer = null;
            }
        }

        protected override HttpInput GetHttpInput() => 
            new HostedHttpInput(this);

        protected override HttpOutput GetHttpOutput(Message message)
        {
            if (((base.HttpInput.ContentLength == -1L) && !OSEnvironmentHelper.IsVistaOrGreater) || !base.KeepAliveEnabled)
            {
                this.result.SetConnectionClose();
            }
            return HttpOutput.CreateHttpOutput(this.result, base.Listener, message, this);
        }

        protected override void OnAbort()
        {
            base.OnAbort();
            this.result.Abort();
        }

        protected override IAsyncResult OnBeginReply(Message message, TimeSpan timeout, AsyncCallback callback, object state)
        {
            this.CloseHostedRequestContainer();
            return base.OnBeginReply(message, timeout, callback, state);
        }

        protected override void OnClose(TimeSpan timeout)
        {
            base.OnClose(timeout);
            this.result.OnReplySent();
        }

        protected override SecurityMessageProperty OnProcessAuthentication() => 
            base.Listener.ProcessAuthentication(this.result);

        protected override void OnReply(Message message, TimeSpan timeout)
        {
            this.CloseHostedRequestContainer();
            base.OnReply(message, timeout);
        }

        private void SetRequestContainer(HostedRequestContainer requestContainer)
        {
            this.requestContainer = requestContainer;
        }

        protected override HttpStatusCode ValidateAuthentication() => 
            base.Listener.ValidateAuthentication(this.result);

        public override string HttpMethod =>
            this.result.GetHttpMethod();

        private class HostedHttpInput : HttpInput
        {
            private int contentLength;
            private string contentType;
            private HostedHttpContext hostedHttpContext;
            private byte[] preReadBuffer;

            public HostedHttpInput(HostedHttpContext hostedHttpContext) : base(hostedHttpContext.Listener, true, hostedHttpContext.Listener.IsChannelBindingSupportEnabled)
            {
                this.hostedHttpContext = hostedHttpContext;
                if (hostedHttpContext.Listener.MessageEncoderFactory.Encoder.MessageVersion.Envelope == EnvelopeVersion.Soap11)
                {
                    this.contentType = hostedHttpContext.result.GetContentType();
                }
                else
                {
                    this.contentType = hostedHttpContext.result.GetContentTypeFast();
                }
                this.contentLength = hostedHttpContext.result.GetContentLength();
                if (this.contentLength == 0)
                {
                    this.preReadBuffer = hostedHttpContext.result.GetPrereadBuffer(ref this.contentLength);
                }
            }

            protected override void AddProperties(Message message)
            {
                HostedRequestContainer hostedRequest = new HostedRequestContainer(this.hostedHttpContext.result);
                HttpRequestMessageProperty property = new HttpRequestMessageProperty(hostedRequest) {
                    Method = this.hostedHttpContext.HttpMethod
                };
                if (this.hostedHttpContext.result.RequestUri.Query.Length > 1)
                {
                    property.QueryString = this.hostedHttpContext.result.RequestUri.Query.Substring(1);
                }
                message.Properties.Add(HttpRequestMessageProperty.Name, property);
                message.Properties.Add(HostingMessageProperty.Name, CreateMessagePropertyFromHostedResult(this.hostedHttpContext.result));
                message.Properties.Via = this.hostedHttpContext.result.RequestUri;
                RemoteEndpointMessageProperty property2 = new RemoteEndpointMessageProperty(hostedRequest);
                message.Properties.Add(RemoteEndpointMessageProperty.Name, property2);
                message.Properties.Add("OriginalHttpRequestUri", this.hostedHttpContext.result.OriginalRequestUri);
                this.hostedHttpContext.SetRequestContainer(hostedRequest);
            }

            [SecurityTreatAsSafe, SecurityCritical]
            private static HostingMessageProperty CreateMessagePropertyFromHostedResult(HostedHttpRequestAsyncResult result) => 
                new HostingMessageProperty(result);

            protected override Stream GetInputStream()
            {
                if (this.preReadBuffer != null)
                {
                    return new HostedInputStream(this.hostedHttpContext, this.preReadBuffer);
                }
                return new HostedInputStream(this.hostedHttpContext);
            }

            protected override System.Security.Authentication.ExtendedProtection.ChannelBinding ChannelBinding =>
                ChannelBindingUtility.DuplicateToken(this.hostedHttpContext.result.GetChannelBinding());

            public override long ContentLength =>
                ((long) this.contentLength);

            protected override string ContentType =>
                this.contentType;

            protected override bool HasContent
            {
                get
                {
                    if (this.preReadBuffer == null)
                    {
                        return (this.ContentLength > 0L);
                    }
                    return true;
                }
            }

            protected override string SoapActionHeader =>
                this.hostedHttpContext.result.GetSoapAction();

            private class HostedInputStream : HttpDelayedAcceptStream
            {
                public HostedInputStream(HostedHttpContext hostedContext) : base(hostedContext.result.GetInputStream())
                {
                }

                public HostedInputStream(HostedHttpContext hostedContext, byte[] preReadBuffer) : base(new PreReadStream(hostedContext.result.GetInputStream(), preReadBuffer))
                {
                }

                public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object state) => 
                    base.BeginRead(buffer, offset, count, callback, state);

                public override int EndRead(IAsyncResult result) => 
                    base.EndRead(result);

                public override int Read(byte[] buffer, int offset, int count) => 
                    base.Read(buffer, offset, count);
            }
        }
    }
}

