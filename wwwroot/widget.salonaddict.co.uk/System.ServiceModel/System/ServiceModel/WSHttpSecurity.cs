namespace System.ServiceModel
{
    using System;
    using System.Runtime.InteropServices;
    using System.ServiceModel.Channels;

    public sealed class WSHttpSecurity
    {
        internal const SecurityMode DefaultMode = SecurityMode.Message;
        private NonDualMessageSecurityOverHttp messageSecurity;
        private SecurityMode mode;
        private HttpTransportSecurity transportSecurity;

        internal WSHttpSecurity() : this(SecurityMode.Message, GetDefaultHttpTransportSecurity(), new NonDualMessageSecurityOverHttp())
        {
        }

        internal WSHttpSecurity(SecurityMode mode, HttpTransportSecurity transportSecurity, NonDualMessageSecurityOverHttp messageSecurity)
        {
            this.mode = mode;
            this.transportSecurity = (transportSecurity == null) ? GetDefaultHttpTransportSecurity() : transportSecurity;
            this.messageSecurity = (messageSecurity == null) ? new NonDualMessageSecurityOverHttp() : messageSecurity;
        }

        internal void ApplyTransportSecurity(HttpsTransportBindingElement https)
        {
            if (this.mode == SecurityMode.TransportWithMessageCredential)
            {
                this.transportSecurity.ConfigureTransportProtectionOnly(https);
            }
            else
            {
                this.transportSecurity.ConfigureTransportProtectionAndAuthentication(https);
            }
        }

        internal static void ApplyTransportSecurity(HttpsTransportBindingElement transport, HttpTransportSecurity transportSecurity)
        {
            HttpTransportSecurity.ConfigureTransportProtectionAndAuthentication(transport, transportSecurity);
        }

        internal SecurityBindingElement CreateMessageSecurity(bool isReliableSessionEnabled, MessageSecurityVersion version)
        {
            if ((this.mode != SecurityMode.Message) && (this.mode != SecurityMode.TransportWithMessageCredential))
            {
                return null;
            }
            return this.messageSecurity.CreateSecurityBindingElement(this.Mode == SecurityMode.TransportWithMessageCredential, isReliableSessionEnabled, version);
        }

        internal static HttpTransportSecurity GetDefaultHttpTransportSecurity() => 
            new HttpTransportSecurity { ClientCredentialType = HttpClientCredentialType.Windows };

        internal static bool TryCreate(SecurityBindingElement sbe, UnifiedSecurityMode mode, HttpTransportSecurity transportSecurity, bool isReliableSessionEnabled, out WSHttpSecurity security)
        {
            security = null;
            NonDualMessageSecurityOverHttp messageSecurity = null;
            SecurityMode none = SecurityMode.None;
            if (sbe != null)
            {
                mode &= UnifiedSecurityMode.TransportWithMessageCredential | UnifiedSecurityMode.Message;
                none = SecurityModeHelper.ToSecurityMode(mode);
                if (!MessageSecurityOverHttp.TryCreate<NonDualMessageSecurityOverHttp>(sbe, none == SecurityMode.TransportWithMessageCredential, isReliableSessionEnabled, out messageSecurity))
                {
                    return false;
                }
            }
            else
            {
                mode &= ~(UnifiedSecurityMode.TransportWithMessageCredential | UnifiedSecurityMode.Message);
                none = SecurityModeHelper.ToSecurityMode(mode);
            }
            security = new WSHttpSecurity(none, transportSecurity, messageSecurity);
            return true;
        }

        public NonDualMessageSecurityOverHttp Message =>
            this.messageSecurity;

        public SecurityMode Mode
        {
            get => 
                this.mode;
            set
            {
                if (!SecurityModeHelper.IsDefined(value))
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

