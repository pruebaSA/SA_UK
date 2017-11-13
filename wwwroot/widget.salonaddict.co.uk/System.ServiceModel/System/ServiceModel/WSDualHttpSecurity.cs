namespace System.ServiceModel
{
    using System;
    using System.Runtime.InteropServices;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Configuration;

    public sealed class WSDualHttpSecurity
    {
        internal const WSDualHttpSecurityMode DefaultMode = WSDualHttpSecurityMode.Message;
        private MessageSecurityOverHttp messageSecurity;
        private WSDualHttpSecurityMode mode;
        private static readonly MessageSecurityVersion WSDualMessageSecurityVersion = MessageSecurityVersion.WSSecurity11WSTrustFebruary2005WSSecureConversationFebruary2005WSSecurityPolicy11BasicSecurityProfile10;

        internal WSDualHttpSecurity() : this(WSDualHttpSecurityMode.Message, new MessageSecurityOverHttp())
        {
        }

        private WSDualHttpSecurity(WSDualHttpSecurityMode mode, MessageSecurityOverHttp messageSecurity)
        {
            this.mode = mode;
            this.messageSecurity = (messageSecurity == null) ? new MessageSecurityOverHttp() : messageSecurity;
        }

        internal SecurityBindingElement CreateMessageSecurity()
        {
            if (this.mode == WSDualHttpSecurityMode.Message)
            {
                return this.messageSecurity.CreateSecurityBindingElement(false, true, WSDualMessageSecurityVersion);
            }
            return null;
        }

        internal static bool TryCreate(SecurityBindingElement sbe, out WSDualHttpSecurity security)
        {
            security = null;
            if (sbe == null)
            {
                security = new WSDualHttpSecurity(WSDualHttpSecurityMode.None, null);
            }
            else
            {
                MessageSecurityOverHttp http;
                if (!MessageSecurityOverHttp.TryCreate<MessageSecurityOverHttp>(sbe, false, true, out http))
                {
                    return false;
                }
                security = new WSDualHttpSecurity(WSDualHttpSecurityMode.Message, http);
            }
            return SecurityElementBase.AreBindingsMatching(security.CreateMessageSecurity(), sbe);
        }

        public MessageSecurityOverHttp Message =>
            this.messageSecurity;

        public WSDualHttpSecurityMode Mode
        {
            get => 
                this.mode;
            set
            {
                if (!WSDualHttpSecurityModeHelper.IsDefined(value))
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("value"));
                }
                this.mode = value;
            }
        }
    }
}

