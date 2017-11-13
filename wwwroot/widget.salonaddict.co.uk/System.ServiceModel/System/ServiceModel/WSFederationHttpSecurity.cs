namespace System.ServiceModel
{
    using System;
    using System.Runtime.InteropServices;
    using System.ServiceModel.Channels;

    public sealed class WSFederationHttpSecurity
    {
        internal const WSFederationHttpSecurityMode DefaultMode = WSFederationHttpSecurityMode.Message;
        private FederatedMessageSecurityOverHttp messageSecurity;
        private WSFederationHttpSecurityMode mode;

        internal WSFederationHttpSecurity() : this(WSFederationHttpSecurityMode.Message, new FederatedMessageSecurityOverHttp())
        {
        }

        private WSFederationHttpSecurity(WSFederationHttpSecurityMode mode, FederatedMessageSecurityOverHttp messageSecurity)
        {
            this.mode = mode;
            this.messageSecurity = (messageSecurity == null) ? new FederatedMessageSecurityOverHttp() : messageSecurity;
        }

        internal SecurityBindingElement CreateMessageSecurity(bool isReliableSessionEnabled, MessageSecurityVersion version)
        {
            if ((this.mode != WSFederationHttpSecurityMode.Message) && (this.mode != WSFederationHttpSecurityMode.TransportWithMessageCredential))
            {
                return null;
            }
            return this.messageSecurity.CreateSecurityBindingElement(this.Mode == WSFederationHttpSecurityMode.TransportWithMessageCredential, isReliableSessionEnabled, version);
        }

        internal static bool TryCreate(SecurityBindingElement sbe, WSFederationHttpSecurityMode mode, HttpTransportSecurity transportSecurity, bool isReliableSessionEnabled, MessageSecurityVersion version, out WSFederationHttpSecurity security)
        {
            security = null;
            FederatedMessageSecurityOverHttp messageSecurity = null;
            if (sbe == null)
            {
                mode = WSFederationHttpSecurityMode.None;
            }
            else
            {
                mode &= WSFederationHttpSecurityMode.TransportWithMessageCredential | WSFederationHttpSecurityMode.Message;
                if (!FederatedMessageSecurityOverHttp.TryCreate(sbe, mode == WSFederationHttpSecurityMode.TransportWithMessageCredential, isReliableSessionEnabled, version, out messageSecurity))
                {
                    return false;
                }
            }
            security = new WSFederationHttpSecurity(mode, messageSecurity);
            return true;
        }

        public FederatedMessageSecurityOverHttp Message =>
            this.messageSecurity;

        public WSFederationHttpSecurityMode Mode
        {
            get => 
                this.mode;
            set
            {
                if (!WSFederationHttpSecurityModeHelper.IsDefined(value))
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("value"));
                }
                this.mode = value;
            }
        }
    }
}

