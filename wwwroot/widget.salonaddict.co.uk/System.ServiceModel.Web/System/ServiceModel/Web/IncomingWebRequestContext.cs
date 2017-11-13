namespace System.ServiceModel.Web
{
    using System;
    using System.Globalization;
    using System.Net;
    using System.ServiceModel;
    using System.ServiceModel.Channels;

    public class IncomingWebRequestContext
    {
        private OperationContext operationContext;
        internal const string UriTemplateMatchResultsPropertyName = "UriTemplateMatchResults";

        internal IncomingWebRequestContext(OperationContext operationContext)
        {
            this.operationContext = operationContext;
        }

        private HttpRequestMessageProperty EnsureMessageProperty()
        {
            if (this.MessageProperty == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SR2.GetString(SR2.HttpContextNoIncomingMessageProperty, new object[] { typeof(HttpRequestMessageProperty).Name })));
            }
            return this.MessageProperty;
        }

        public string Accept =>
            this.EnsureMessageProperty().Headers[HttpRequestHeader.Accept];

        public long ContentLength =>
            long.Parse(this.EnsureMessageProperty().Headers[HttpRequestHeader.ContentLength], CultureInfo.InvariantCulture);

        public string ContentType =>
            this.EnsureMessageProperty().Headers[HttpRequestHeader.ContentType];

        public WebHeaderCollection Headers =>
            this.EnsureMessageProperty().Headers;

        private HttpRequestMessageProperty MessageProperty
        {
            get
            {
                if (this.operationContext.IncomingMessageProperties == null)
                {
                    return null;
                }
                if (!this.operationContext.IncomingMessageProperties.ContainsKey(HttpRequestMessageProperty.Name))
                {
                    return null;
                }
                return (this.operationContext.IncomingMessageProperties[HttpRequestMessageProperty.Name] as HttpRequestMessageProperty);
            }
        }

        public string Method =>
            this.EnsureMessageProperty().Method;

        public System.UriTemplateMatch UriTemplateMatch
        {
            get
            {
                if (this.operationContext.IncomingMessageProperties.ContainsKey("UriTemplateMatchResults"))
                {
                    return (this.operationContext.IncomingMessageProperties["UriTemplateMatchResults"] as System.UriTemplateMatch);
                }
                return null;
            }
            set
            {
                this.operationContext.IncomingMessageProperties["UriTemplateMatchResults"] = value;
            }
        }

        public string UserAgent =>
            this.EnsureMessageProperty().Headers[HttpRequestHeader.UserAgent];
    }
}

