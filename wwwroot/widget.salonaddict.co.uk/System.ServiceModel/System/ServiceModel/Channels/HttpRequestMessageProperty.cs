namespace System.ServiceModel.Channels
{
    using System;
    using System.Net;
    using System.ServiceModel;
    using System.ServiceModel.Activation;

    public sealed class HttpRequestMessageProperty
    {
        private WebHeaderCollection headers;
        private HostedRequestContainer hostedRequestContainer;
        private HttpListenerRequest listenerRequest;
        private string method;
        private string queryString;
        private bool suppressEntityBody;

        public HttpRequestMessageProperty()
        {
            this.method = "POST";
            this.queryString = string.Empty;
            this.suppressEntityBody = false;
        }

        internal HttpRequestMessageProperty(HttpListenerRequest listenerRequest) : this()
        {
            this.listenerRequest = listenerRequest;
        }

        internal HttpRequestMessageProperty(HostedRequestContainer hostedRequest) : this()
        {
            this.hostedRequestContainer = hostedRequest;
        }

        public WebHeaderCollection Headers
        {
            get
            {
                if (this.headers == null)
                {
                    this.headers = new WebHeaderCollection();
                    if (this.listenerRequest != null)
                    {
                        this.headers.Add(this.listenerRequest.Headers);
                        if ((this.listenerRequest.UserAgent != null) && (this.headers[HttpRequestHeader.UserAgent] == null))
                        {
                            this.headers.Add(HttpRequestHeader.UserAgent, this.listenerRequest.UserAgent);
                        }
                        this.listenerRequest = null;
                    }
                    else if (this.hostedRequestContainer != null)
                    {
                        this.hostedRequestContainer.CopyHeaders(this.headers);
                        this.hostedRequestContainer = null;
                    }
                }
                return this.headers;
            }
        }

        public string Method
        {
            get => 
                this.method;
            set
            {
                if (value == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("value");
                }
                this.method = value;
            }
        }

        public static string Name =>
            "httpRequest";

        public string QueryString
        {
            get => 
                this.queryString;
            set
            {
                if (value == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("value");
                }
                this.queryString = value;
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
    }
}

