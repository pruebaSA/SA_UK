namespace System.ServiceModel
{
    using System;
    using System.Net;
    using System.Security.Authentication.ExtendedProtection;
    using System.ServiceModel.Channels;

    public sealed class HttpTransportSecurity
    {
        private HttpClientCredentialType clientCredentialType = HttpClientCredentialType.None;
        internal const HttpClientCredentialType DefaultClientCredentialType = HttpClientCredentialType.None;
        internal const HttpProxyCredentialType DefaultProxyCredentialType = HttpProxyCredentialType.None;
        internal const string DefaultRealm = "";
        private System.Security.Authentication.ExtendedProtection.ExtendedProtectionPolicy extendedProtectionPolicy = ChannelBindingUtility.DefaultPolicy;
        private HttpProxyCredentialType proxyCredentialType = HttpProxyCredentialType.None;
        private string realm = "";

        internal HttpTransportSecurity()
        {
        }

        private void ConfigureAuthentication(HttpTransportBindingElement http)
        {
            http.AuthenticationScheme = HttpClientCredentialTypeHelper.MapToAuthenticationScheme(this.clientCredentialType);
            http.ProxyAuthenticationScheme = HttpProxyCredentialTypeHelper.MapToAuthenticationScheme(this.proxyCredentialType);
            http.Realm = this.Realm;
            http.ExtendedProtectionPolicy = this.extendedProtectionPolicy;
        }

        private static void ConfigureAuthentication(HttpTransportBindingElement http, HttpTransportSecurity transportSecurity)
        {
            transportSecurity.clientCredentialType = HttpClientCredentialTypeHelper.MapToClientCredentialType(http.AuthenticationScheme);
            transportSecurity.proxyCredentialType = HttpProxyCredentialTypeHelper.MapToProxyCredentialType(http.ProxyAuthenticationScheme);
            transportSecurity.Realm = http.Realm;
            transportSecurity.extendedProtectionPolicy = http.ExtendedProtectionPolicy;
        }

        internal void ConfigureTransportAuthentication(HttpTransportBindingElement http)
        {
            if (this.clientCredentialType == HttpClientCredentialType.Certificate)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(System.ServiceModel.SR.GetString("CertificateUnsupportedForHttpTransportCredentialOnly")));
            }
            this.ConfigureAuthentication(http);
        }

        internal void ConfigureTransportProtectionAndAuthentication(HttpsTransportBindingElement https)
        {
            this.ConfigureAuthentication(https);
            https.RequireClientCertificate = this.clientCredentialType == HttpClientCredentialType.Certificate;
        }

        internal static void ConfigureTransportProtectionAndAuthentication(HttpsTransportBindingElement https, HttpTransportSecurity transportSecurity)
        {
            ConfigureAuthentication(https, transportSecurity);
            if (https.RequireClientCertificate)
            {
                transportSecurity.ClientCredentialType = HttpClientCredentialType.Certificate;
            }
        }

        internal void ConfigureTransportProtectionOnly(HttpsTransportBindingElement https)
        {
            this.DisableAuthentication(https);
            https.RequireClientCertificate = false;
        }

        private void DisableAuthentication(HttpTransportBindingElement http)
        {
            http.AuthenticationScheme = AuthenticationSchemes.Anonymous;
            http.ProxyAuthenticationScheme = AuthenticationSchemes.Anonymous;
            http.Realm = "";
            http.ExtendedProtectionPolicy = this.extendedProtectionPolicy;
        }

        internal void DisableTransportAuthentication(HttpTransportBindingElement http)
        {
            this.DisableAuthentication(http);
        }

        internal static bool IsConfiguredTransportAuthentication(HttpTransportBindingElement http, HttpTransportSecurity transportSecurity)
        {
            if (HttpClientCredentialTypeHelper.MapToClientCredentialType(http.AuthenticationScheme) == HttpClientCredentialType.Certificate)
            {
                return false;
            }
            ConfigureAuthentication(http, transportSecurity);
            return true;
        }

        private static bool IsDisabledAuthentication(HttpTransportBindingElement http) => 
            (((http.AuthenticationScheme == AuthenticationSchemes.Anonymous) && (http.ProxyAuthenticationScheme == AuthenticationSchemes.Anonymous)) && (http.Realm == ""));

        internal static bool IsDisabledTransportAuthentication(HttpTransportBindingElement http) => 
            IsDisabledAuthentication(http);

        public HttpClientCredentialType ClientCredentialType
        {
            get => 
                this.clientCredentialType;
            set
            {
                if (!HttpClientCredentialTypeHelper.IsDefined(value))
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("value"));
                }
                this.clientCredentialType = value;
            }
        }

        public System.Security.Authentication.ExtendedProtection.ExtendedProtectionPolicy ExtendedProtectionPolicy
        {
            get => 
                this.extendedProtectionPolicy;
            set
            {
                if (value == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("value");
                }
                if ((value.PolicyEnforcement == PolicyEnforcement.Always) && !ChannelBindingUtility.OSSupportsExtendedProtection)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new PlatformNotSupportedException(System.ServiceModel.SR.GetString("ExtendedProtectionNotSupported")));
                }
                this.extendedProtectionPolicy = value;
            }
        }

        public HttpProxyCredentialType ProxyCredentialType
        {
            get => 
                this.proxyCredentialType;
            set
            {
                if (!HttpProxyCredentialTypeHelper.IsDefined(value))
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("value"));
                }
                this.proxyCredentialType = value;
            }
        }

        public string Realm
        {
            get => 
                this.realm;
            set
            {
                this.realm = value;
            }
        }
    }
}

