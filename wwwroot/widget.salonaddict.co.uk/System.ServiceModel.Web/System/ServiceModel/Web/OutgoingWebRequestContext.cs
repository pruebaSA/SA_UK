namespace System.ServiceModel.Web
{
    using System;
    using System.Globalization;
    using System.Net;
    using System.ServiceModel;
    using System.ServiceModel.Channels;

    public class OutgoingWebRequestContext
    {
        private OperationContext operationContext;

        internal OutgoingWebRequestContext(OperationContext operationContext)
        {
            this.operationContext = operationContext;
        }

        public string Accept
        {
            get => 
                this.MessageProperty.Headers[HttpRequestHeader.Accept];
            set
            {
                this.MessageProperty.Headers[HttpRequestHeader.Accept] = value;
            }
        }

        public long ContentLength
        {
            get => 
                long.Parse(this.MessageProperty.Headers[HttpRequestHeader.ContentLength], CultureInfo.InvariantCulture);
            set
            {
                this.MessageProperty.Headers[HttpRequestHeader.ContentLength] = value.ToString(CultureInfo.InvariantCulture);
            }
        }

        public string ContentType
        {
            get => 
                this.MessageProperty.Headers[HttpRequestHeader.ContentType];
            set
            {
                this.MessageProperty.Headers[HttpRequestHeader.ContentType] = value;
            }
        }

        public WebHeaderCollection Headers =>
            this.MessageProperty.Headers;

        public string IfMatch
        {
            get => 
                this.MessageProperty.Headers[HttpRequestHeader.IfMatch];
            set
            {
                this.MessageProperty.Headers[HttpRequestHeader.IfMatch] = value;
            }
        }

        public string IfModifiedSince
        {
            get => 
                this.MessageProperty.Headers[HttpRequestHeader.IfModifiedSince];
            set
            {
                this.MessageProperty.Headers[HttpRequestHeader.IfModifiedSince] = value;
            }
        }

        public string IfNoneMatch
        {
            get => 
                this.MessageProperty.Headers[HttpRequestHeader.IfNoneMatch];
            set
            {
                this.MessageProperty.Headers[HttpRequestHeader.IfNoneMatch] = value;
            }
        }

        public string IfUnmodifiedSince
        {
            get => 
                this.MessageProperty.Headers[HttpRequestHeader.IfUnmodifiedSince];
            set
            {
                this.MessageProperty.Headers[HttpRequestHeader.IfUnmodifiedSince] = value;
            }
        }

        private HttpRequestMessageProperty MessageProperty
        {
            get
            {
                if (!this.operationContext.OutgoingMessageProperties.ContainsKey(HttpRequestMessageProperty.Name))
                {
                    this.operationContext.OutgoingMessageProperties.Add(HttpRequestMessageProperty.Name, new HttpRequestMessageProperty());
                }
                return (this.operationContext.OutgoingMessageProperties[HttpRequestMessageProperty.Name] as HttpRequestMessageProperty);
            }
        }

        public string Method
        {
            get => 
                this.MessageProperty.Method;
            set
            {
                this.MessageProperty.Method = value;
            }
        }

        public bool SuppressEntityBody
        {
            get => 
                this.MessageProperty.SuppressEntityBody;
            set
            {
                this.MessageProperty.SuppressEntityBody = value;
            }
        }

        public string UserAgent
        {
            get => 
                this.MessageProperty.Headers[HttpRequestHeader.UserAgent];
            set
            {
                this.MessageProperty.Headers[HttpRequestHeader.UserAgent] = value;
            }
        }
    }
}

