namespace System.ServiceModel.Web
{
    using System;
    using System.Globalization;
    using System.Net;
    using System.ServiceModel;
    using System.ServiceModel.Channels;

    public class IncomingWebResponseContext
    {
        private OperationContext operationContext;

        internal IncomingWebResponseContext(OperationContext operationContext)
        {
            this.operationContext = operationContext;
        }

        private HttpResponseMessageProperty EnsureMessageProperty()
        {
            if (this.MessageProperty == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(SR2.GetString(SR2.HttpContextNoIncomingMessageProperty, new object[] { typeof(HttpResponseMessageProperty).Name })));
            }
            return this.MessageProperty;
        }

        public long ContentLength =>
            long.Parse(this.EnsureMessageProperty().Headers[HttpResponseHeader.ContentLength], CultureInfo.InvariantCulture);

        public string ContentType =>
            this.EnsureMessageProperty().Headers[HttpResponseHeader.ContentType];

        public string ETag =>
            this.EnsureMessageProperty().Headers[HttpResponseHeader.ETag];

        public WebHeaderCollection Headers =>
            this.EnsureMessageProperty().Headers;

        public string Location =>
            this.EnsureMessageProperty().Headers[HttpResponseHeader.Location];

        private HttpResponseMessageProperty MessageProperty
        {
            get
            {
                if (this.operationContext.IncomingMessageProperties == null)
                {
                    return null;
                }
                if (!this.operationContext.IncomingMessageProperties.ContainsKey(HttpResponseMessageProperty.Name))
                {
                    return null;
                }
                return (this.operationContext.IncomingMessageProperties[HttpResponseMessageProperty.Name] as HttpResponseMessageProperty);
            }
        }

        public HttpStatusCode StatusCode =>
            this.EnsureMessageProperty().StatusCode;

        public string StatusDescription =>
            this.EnsureMessageProperty().StatusDescription;
    }
}

