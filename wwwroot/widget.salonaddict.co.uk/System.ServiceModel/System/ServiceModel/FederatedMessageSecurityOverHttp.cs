namespace System.ServiceModel
{
    using System;
    using System.Collections.ObjectModel;
    using System.IdentityModel.Tokens;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Security;
    using System.ServiceModel.Security.Tokens;
    using System.Xml;

    public sealed class FederatedMessageSecurityOverHttp
    {
        private SecurityAlgorithmSuite algorithmSuite = SecurityAlgorithmSuite.Default;
        private Collection<ClaimTypeRequirement> claimTypeRequirements = new Collection<ClaimTypeRequirement>();
        internal const SecurityKeyType DefaultIssuedKeyType = SecurityKeyType.SymmetricKey;
        internal const bool DefaultNegotiateServiceCredential = true;
        private SecurityKeyType issuedKeyType = SecurityKeyType.SymmetricKey;
        private string issuedTokenType;
        private EndpointAddress issuerAddress;
        private Binding issuerBinding;
        private EndpointAddress issuerMetadataAddress;
        private bool negotiateServiceCredential = true;
        private Collection<XmlElement> tokenRequestParameters = new Collection<XmlElement>();

        internal FederatedMessageSecurityOverHttp()
        {
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        internal SecurityBindingElement CreateSecurityBindingElement(bool isSecureTransportMode, bool isReliableSession, MessageSecurityVersion version)
        {
            SecurityBindingElement element3;
            if ((this.IssuedKeyType == SecurityKeyType.BearerKey) && (version.TrustVersion == TrustVersion.WSTrustFeb2005))
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(System.ServiceModel.SR.GetString("BearerKeyIncompatibleWithWSFederationHttpBinding")));
            }
            bool emitBspRequiredAttributes = true;
            IssuedSecurityTokenParameters issuedTokenParameters = new IssuedSecurityTokenParameters(this.IssuedTokenType, this.IssuerAddress, this.IssuerBinding) {
                IssuerMetadataAddress = this.issuerMetadataAddress,
                KeyType = this.IssuedKeyType
            };
            if (this.IssuedKeyType == SecurityKeyType.SymmetricKey)
            {
                issuedTokenParameters.KeySize = this.AlgorithmSuite.DefaultSymmetricKeyLength;
            }
            else
            {
                issuedTokenParameters.KeySize = 0;
            }
            foreach (ClaimTypeRequirement requirement in this.claimTypeRequirements)
            {
                issuedTokenParameters.ClaimTypeRequirements.Add(requirement);
            }
            foreach (XmlElement element2 in this.TokenRequestParameters)
            {
                issuedTokenParameters.AdditionalRequestParameters.Add(element2);
            }
            WSSecurityTokenSerializer tokenSerializer = new WSSecurityTokenSerializer(version.SecurityVersion, version.TrustVersion, version.SecureConversationVersion, emitBspRequiredAttributes, null, null, null);
            SecurityStandardsManager standardsManager = new SecurityStandardsManager(version, tokenSerializer);
            issuedTokenParameters.AddAlgorithmParameters(this.AlgorithmSuite, standardsManager, this.issuedKeyType);
            if (isSecureTransportMode)
            {
                element3 = SecurityBindingElement.CreateIssuedTokenOverTransportBindingElement(issuedTokenParameters);
            }
            else if (this.negotiateServiceCredential)
            {
                element3 = SecurityBindingElement.CreateIssuedTokenForSslBindingElement(issuedTokenParameters, version.SecurityPolicyVersion != SecurityPolicyVersion.WSSecurityPolicy11);
            }
            else
            {
                element3 = SecurityBindingElement.CreateIssuedTokenForCertificateBindingElement(issuedTokenParameters);
            }
            element3.MessageSecurityVersion = version;
            element3.DefaultAlgorithmSuite = this.AlgorithmSuite;
            SecurityBindingElement element = SecurityBindingElement.CreateSecureConversationBindingElement(element3, true);
            element.MessageSecurityVersion = version;
            element.DefaultAlgorithmSuite = this.AlgorithmSuite;
            element.IncludeTimestamp = true;
            if (!isReliableSession)
            {
                element.LocalServiceSettings.ReconnectTransportOnFailure = false;
                element.LocalClientSettings.ReconnectTransportOnFailure = false;
                return element;
            }
            element.LocalServiceSettings.ReconnectTransportOnFailure = true;
            element.LocalClientSettings.ReconnectTransportOnFailure = true;
            return element;
        }

        internal static bool TryCreate(SecurityBindingElement sbe, bool isSecureTransportMode, bool isReliableSession, MessageSecurityVersion version, out FederatedMessageSecurityOverHttp messageSecurity)
        {
            bool flag;
            bool flag2;
            IssuedSecurityTokenParameters parameters;
            Collection<XmlElement> collection;
            messageSecurity = null;
            if (sbe.IncludeTimestamp)
            {
                SecurityBindingElement element;
                if (sbe.SecurityHeaderLayout != SecurityHeaderLayout.Strict)
                {
                    return false;
                }
                flag = true;
                if (!SecurityBindingElement.IsSecureConversationBinding(sbe, true, out element))
                {
                    return false;
                }
                if (isSecureTransportMode && !(element is TransportSecurityBindingElement))
                {
                    return false;
                }
                flag2 = true;
                if (isSecureTransportMode)
                {
                    if (!SecurityBindingElement.IsIssuedTokenOverTransportBinding(element, out parameters))
                    {
                        return false;
                    }
                    goto Label_0073;
                }
                if (SecurityBindingElement.IsIssuedTokenForSslBinding(element, version.SecurityPolicyVersion != SecurityPolicyVersion.WSSecurityPolicy11, out parameters))
                {
                    flag2 = true;
                    goto Label_0073;
                }
                if (SecurityBindingElement.IsIssuedTokenForCertificateBinding(element, out parameters))
                {
                    flag2 = false;
                    goto Label_0073;
                }
            }
            return false;
        Label_0073:
            if ((parameters.KeyType == SecurityKeyType.BearerKey) && (version.TrustVersion == TrustVersion.WSTrustFeb2005))
            {
                return false;
            }
            WSSecurityTokenSerializer tokenSerializer = new WSSecurityTokenSerializer(version.SecurityVersion, version.TrustVersion, version.SecureConversationVersion, flag, null, null, null);
            SecurityStandardsManager standardsManager = new SecurityStandardsManager(version, tokenSerializer);
            if (!parameters.DoAlgorithmsMatch(sbe.DefaultAlgorithmSuite, standardsManager, out collection))
            {
                return false;
            }
            messageSecurity = new FederatedMessageSecurityOverHttp();
            messageSecurity.AlgorithmSuite = sbe.DefaultAlgorithmSuite;
            messageSecurity.NegotiateServiceCredential = flag2;
            messageSecurity.IssuedTokenType = parameters.TokenType;
            messageSecurity.IssuerAddress = parameters.IssuerAddress;
            messageSecurity.IssuerBinding = parameters.IssuerBinding;
            messageSecurity.IssuerMetadataAddress = parameters.IssuerMetadataAddress;
            messageSecurity.IssuedKeyType = parameters.KeyType;
            foreach (ClaimTypeRequirement requirement in parameters.ClaimTypeRequirements)
            {
                messageSecurity.ClaimTypeRequirements.Add(requirement);
            }
            foreach (XmlElement element2 in collection)
            {
                messageSecurity.TokenRequestParameters.Add(element2);
            }
            if ((parameters.AlternativeIssuerEndpoints != null) && (parameters.AlternativeIssuerEndpoints.Count > 0))
            {
                return false;
            }
            return true;
        }

        public SecurityAlgorithmSuite AlgorithmSuite
        {
            get => 
                this.algorithmSuite;
            set
            {
                if (value == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("value");
                }
                this.algorithmSuite = value;
            }
        }

        public Collection<ClaimTypeRequirement> ClaimTypeRequirements =>
            this.claimTypeRequirements;

        public SecurityKeyType IssuedKeyType
        {
            get => 
                this.issuedKeyType;
            set
            {
                if (!SecurityKeyTypeHelper.IsDefined(value))
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("value"));
                }
                this.issuedKeyType = value;
            }
        }

        public string IssuedTokenType
        {
            get => 
                this.issuedTokenType;
            set
            {
                this.issuedTokenType = value;
            }
        }

        public EndpointAddress IssuerAddress
        {
            get => 
                this.issuerAddress;
            set
            {
                this.issuerAddress = value;
            }
        }

        public Binding IssuerBinding
        {
            get => 
                this.issuerBinding;
            set
            {
                this.issuerBinding = value;
            }
        }

        public EndpointAddress IssuerMetadataAddress
        {
            get => 
                this.issuerMetadataAddress;
            set
            {
                this.issuerMetadataAddress = value;
            }
        }

        public bool NegotiateServiceCredential
        {
            get => 
                this.negotiateServiceCredential;
            set
            {
                this.negotiateServiceCredential = value;
            }
        }

        public Collection<XmlElement> TokenRequestParameters =>
            this.tokenRequestParameters;
    }
}

