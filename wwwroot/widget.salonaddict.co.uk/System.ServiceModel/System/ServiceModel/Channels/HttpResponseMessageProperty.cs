namespace System.ServiceModel.Channels
{
    using System;
    using System.Net;
    using System.ServiceModel;

    public sealed class HttpResponseMessageProperty
    {
        private WebHeaderCollection headers;
        private WebHeaderCollection originalHeaders;
        private HttpStatusCode statusCode;
        private string statusDescription;
        private bool suppressEntityBody;
        private bool suppressPreamble;

        public HttpResponseMessageProperty()
        {
            this.statusCode = HttpStatusCode.OK;
            this.statusDescription = null;
            this.suppressEntityBody = false;
        }

        internal HttpResponseMessageProperty(WebHeaderCollection originalHeaders) : this()
        {
            this.originalHeaders = originalHeaders;
        }

        public WebHeaderCollection Headers
        {
            get
            {
                if (this.headers == null)
                {
                    this.headers = new WebHeaderCollection();
                    if (this.originalHeaders != null)
                    {
                        this.headers.Add(this.originalHeaders);
                        this.originalHeaders = null;
                    }
                }
                return this.headers;
            }
        }

        public static string Name =>
            "httpResponse";

        public HttpStatusCode StatusCode
        {
            get => 
                this.statusCode;
            set
            {
                int num = (int) value;
                if ((num < 100) || (num > 0x257))
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("value", value, System.ServiceModel.SR.GetString("ValueMustBeInRange", new object[] { 100, 0x257 })));
                }
                this.statusCode = value;
            }
        }

        public string StatusDescription
        {
            get => 
                this.statusDescription;
            set
            {
                this.statusDescription = value;
            }
        }

        public bool SuppressEntityBody
        {
            get => 
                this.suppressEntityBody;
            set
            {
                this.suppressEntityBody = value;
            }
        }

        public bool SuppressPreamble
        {
            get => 
                this.suppressPreamble;
            set
            {
                this.suppressPreamble = value;
            }
        }
    }
}

