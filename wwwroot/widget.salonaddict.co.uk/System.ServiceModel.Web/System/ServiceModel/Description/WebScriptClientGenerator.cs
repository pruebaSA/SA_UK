namespace System.ServiceModel.Description
{
    using System;
    using System.Globalization;
    using System.Net;
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using System.Web;
    using System.Web.Script.Services;

    internal class WebScriptClientGenerator : ServiceMetadataExtension.IHttpGetMetadata
    {
        internal const string DebugMetadataEndpointSuffix = "jsdebug";
        private bool debugMode;
        private ServiceEndpoint endpoint;
        internal const string MetadataEndpointSuffix = "js";
        private string proxyContent;
        private DateTime serviceLastModified;
        private string serviceLastModifiedRfc1123String;

        public WebScriptClientGenerator(ServiceEndpoint endpoint, bool debugMode)
        {
            this.endpoint = endpoint;
            this.debugMode = debugMode;
            this.serviceLastModified = DateTime.UtcNow;
            this.serviceLastModified = new DateTime(this.serviceLastModified.Year, this.serviceLastModified.Month, this.serviceLastModified.Day, this.serviceLastModified.Hour, this.serviceLastModified.Minute, this.serviceLastModified.Second, DateTimeKind.Utc);
        }

        public Message Get(Message message)
        {
            HttpRequestMessageProperty property = (HttpRequestMessageProperty) message.Properties[HttpRequestMessageProperty.Name];
            HttpResponseMessageProperty property2 = new HttpResponseMessageProperty();
            if ((property != null) && this.IsServiceUnchanged(property.Headers["If-Modified-Since"]))
            {
                Message message2 = Message.CreateMessage(MessageVersion.None, string.Empty);
                property2.StatusCode = HttpStatusCode.NotModified;
                message2.Properties.Add(HttpResponseMessageProperty.Name, property2);
                return message2;
            }
            Message message3 = new WebScriptMetadataMessage(string.Empty, this.ProxyContent);
            property2.Headers.Add("Last-Modified", this.ServiceLastModifiedRfc1123String);
            property2.Headers.Add("Expires", this.ServiceLastModifiedRfc1123String);
            if (ServiceHostingEnvironment.AspNetCompatibilityEnabled)
            {
                HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.Public);
            }
            else
            {
                property2.Headers.Add("Cache-Control", "public");
            }
            message3.Properties.Add(HttpResponseMessageProperty.Name, property2);
            return message3;
        }

        internal static string GetMetadataEndpointSuffix(bool debugMode)
        {
            if (debugMode)
            {
                return "jsdebug";
            }
            return "js";
        }

        private bool IsServiceUnchanged(string ifModifiedSinceHeaderValue)
        {
            DateTime time;
            if (string.IsNullOrEmpty(ifModifiedSinceHeaderValue))
            {
                return false;
            }
            return (DateTime.TryParse(ifModifiedSinceHeaderValue, DateTimeFormatInfo.InvariantInfo, DateTimeStyles.AdjustToUniversal, out time) && (time >= this.serviceLastModified));
        }

        private string ProxyContent
        {
            get
            {
                if (this.proxyContent == null)
                {
                    this.proxyContent = ProxyGenerator.GetClientProxyScript(this.endpoint.Contract.ContractType, this.endpoint.Address.Uri.PathAndQuery, this.debugMode);
                }
                return this.proxyContent;
            }
        }

        private string ServiceLastModifiedRfc1123String
        {
            get
            {
                if (this.serviceLastModifiedRfc1123String == null)
                {
                    this.serviceLastModifiedRfc1123String = this.serviceLastModified.ToString("R", DateTimeFormatInfo.InvariantInfo);
                }
                return this.serviceLastModifiedRfc1123String;
            }
        }
    }
}

