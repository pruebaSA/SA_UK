namespace System.ServiceModel
{
    using System;
    using System.ServiceModel.Channels;

    public sealed class WebHttpSecurity
    {
        internal const WebHttpSecurityMode DefaultMode = WebHttpSecurityMode.None;
        private WebHttpSecurityMode mode;
        private HttpTransportSecurity transportSecurity;

        internal WebHttpSecurity() : this(WebHttpSecurityMode.None, new HttpTransportSecurity())
        {
        }

        private WebHttpSecurity(WebHttpSecurityMode mode, HttpTransportSecurity transportSecurity)
        {
            this.Mode = mode;
            this.transportSecurity = (transportSecurity == null) ? new HttpTransportSecurity() : transportSecurity;
        }

        internal void DisableTransportAuthentication(HttpTransportBindingElement http)
        {
            this.transportSecurity.DisableTransportAuthentication(http);
        }

        internal void EnableTransportAuthentication(HttpTransportBindingElement http)
        {
            this.transportSecurity.ConfigureTransportAuthentication(http);
        }

        internal void EnableTransportSecurity(HttpsTransportBindingElement https)
        {
            this.transportSecurity.ConfigureTransportProtectionAndAuthentication(https);
        }

        public WebHttpSecurityMode Mode
        {
            get => 
                this.mode;
            set
            {
                if (!WebHttpSecurityModeHelper.IsDefined(value))
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("value"));
                }
                this.mode = value;
            }
        }

        public HttpTransportSecurity Transport =>
            this.transportSecurity;
    }
}

