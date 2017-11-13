﻿namespace System.ServiceModel
{
    using System;
    using System.Configuration;
    using System.Runtime.InteropServices;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Configuration;

    public class WS2007FederationHttpBinding : WSFederationHttpBinding
    {
        private static readonly MessageSecurityVersion WS2007MessageSecurityVersion = MessageSecurityVersion.WSSecurity11WSTrust13WSSecureConversation13WSSecurityPolicy12BasicSecurityProfile10;
        private static readonly ReliableMessagingVersion WS2007ReliableMessagingVersion = ReliableMessagingVersion.WSReliableMessaging11;
        private static readonly TransactionProtocol WS2007TransactionProtocol = TransactionProtocol.WSAtomicTransaction11;

        public WS2007FederationHttpBinding()
        {
            base.ReliableSessionBindingElement.ReliableMessagingVersion = WS2007ReliableMessagingVersion;
            base.TransactionFlowBindingElement.TransactionProtocol = WS2007TransactionProtocol;
            base.HttpsTransport.MessageSecurityVersion = WS2007MessageSecurityVersion;
        }

        public WS2007FederationHttpBinding(WSFederationHttpSecurityMode securityMode) : this(securityMode, false)
        {
        }

        public WS2007FederationHttpBinding(string configName) : this()
        {
            this.ApplyConfiguration(configName);
        }

        public WS2007FederationHttpBinding(WSFederationHttpSecurityMode securityMode, bool reliableSessionEnabled) : base(securityMode, reliableSessionEnabled)
        {
            base.ReliableSessionBindingElement.ReliableMessagingVersion = WS2007ReliableMessagingVersion;
            base.TransactionFlowBindingElement.TransactionProtocol = WS2007TransactionProtocol;
            base.HttpsTransport.MessageSecurityVersion = WS2007MessageSecurityVersion;
        }

        private WS2007FederationHttpBinding(WSFederationHttpSecurity security, PrivacyNoticeBindingElement privacy, bool reliableSessionEnabled) : base(security, privacy, reliableSessionEnabled)
        {
            base.ReliableSessionBindingElement.ReliableMessagingVersion = WS2007ReliableMessagingVersion;
            base.TransactionFlowBindingElement.TransactionProtocol = WS2007TransactionProtocol;
            base.HttpsTransport.MessageSecurityVersion = WS2007MessageSecurityVersion;
        }

        private void ApplyConfiguration(string configurationName)
        {
            WS2007FederationHttpBindingElement element2 = WS2007FederationHttpBindingCollectionElement.GetBindingCollectionElement().Bindings[configurationName];
            if (element2 == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ConfigurationErrorsException(System.ServiceModel.SR.GetString("ConfigInvalidBindingConfigurationName", new object[] { configurationName, "ws2007FederationHttpBinding" })));
            }
            element2.ApplyConfiguration(this);
        }

        protected override SecurityBindingElement CreateMessageSecurity() => 
            base.Security.CreateMessageSecurity(base.ReliableSession.Enabled, WS2007MessageSecurityVersion);

        internal static bool TryCreate(SecurityBindingElement sbe, TransportBindingElement transport, PrivacyNoticeBindingElement privacy, ReliableSessionBindingElement rsbe, TransactionFlowBindingElement tfbe, out Binding binding)
        {
            WSFederationHttpSecurityMode mode;
            WSFederationHttpSecurity security2;
            bool isReliableSession = rsbe != null;
            binding = null;
            HttpTransportSecurity transportSecurity = new HttpTransportSecurity();
            if (!WSFederationHttpBinding.GetSecurityModeFromTransport(transport, transportSecurity, out mode))
            {
                return false;
            }
            HttpsTransportBindingElement element = transport as HttpsTransportBindingElement;
            if (((element != null) && (element.MessageSecurityVersion != null)) && (element.MessageSecurityVersion.SecurityPolicyVersion != WS2007MessageSecurityVersion.SecurityPolicyVersion))
            {
                return false;
            }
            if (TryCreateSecurity(sbe, mode, transportSecurity, isReliableSession, out security2))
            {
                binding = new WS2007FederationHttpBinding(security2, privacy, isReliableSession);
            }
            if ((rsbe != null) && (rsbe.ReliableMessagingVersion != ReliableMessagingVersion.WSReliableMessaging11))
            {
                return false;
            }
            if ((tfbe != null) && (tfbe.TransactionProtocol != TransactionProtocol.WSAtomicTransaction11))
            {
                return false;
            }
            return (binding != null);
        }

        private static bool TryCreateSecurity(SecurityBindingElement sbe, WSFederationHttpSecurityMode mode, HttpTransportSecurity transportSecurity, bool isReliableSession, out WSFederationHttpSecurity security)
        {
            if (!WSFederationHttpSecurity.TryCreate(sbe, mode, transportSecurity, isReliableSession, WS2007MessageSecurityVersion, out security))
            {
                return false;
            }
            return SecurityElementBase.AreBindingsMatching(security.CreateMessageSecurity(isReliableSession, WS2007MessageSecurityVersion), sbe);
        }
    }
}

