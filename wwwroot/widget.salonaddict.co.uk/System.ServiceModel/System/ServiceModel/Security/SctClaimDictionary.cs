namespace System.ServiceModel.Security
{
    using System;
    using System.Xml;

    internal sealed class SctClaimDictionary : XmlDictionary
    {
        private XmlDictionaryString anonymousClaimSet;
        private XmlDictionaryString authenticationType;
        private XmlDictionaryString binaryClaim;
        private XmlDictionaryString claim;
        private XmlDictionaryString claimSet;
        private XmlDictionaryString claimSets;
        private XmlDictionaryString contextId;
        private XmlDictionaryString denyOnlySidClaim;
        private XmlDictionaryString dnsClaim;
        private XmlDictionaryString effectiveTime;
        private XmlDictionaryString emptyString;
        private XmlDictionaryString expiryTime;
        private XmlDictionaryString externalTokenReference;
        private XmlDictionaryString genericIdentity;
        private XmlDictionaryString genericXmlToken;
        private XmlDictionaryString hashClaim;
        private XmlDictionaryString id;
        private XmlDictionaryString identities;
        private static readonly SctClaimDictionary instance = new SctClaimDictionary();
        private XmlDictionaryString internalTokenReference;
        private XmlDictionaryString isCookieMode;
        private XmlDictionaryString key;
        private XmlDictionaryString keyEffectiveTime;
        private XmlDictionaryString keyExpiryTime;
        private XmlDictionaryString keyGeneration;
        private XmlDictionaryString mailAddressClaim;
        private XmlDictionaryString name;
        private XmlDictionaryString nameClaim;
        private XmlDictionaryString nullValue;
        private XmlDictionaryString primaryIdentity;
        private XmlDictionaryString primaryIssuer;
        private XmlDictionaryString right;
        private XmlDictionaryString rsaClaim;
        private XmlDictionaryString securityContextToken;
        private XmlDictionaryString serviceContractId;
        private XmlDictionaryString sid;
        private XmlDictionaryString spnClaim;
        private XmlDictionaryString systemClaim;
        private XmlDictionaryString systemClaimSet;
        private XmlDictionaryString tokenType;
        private XmlDictionaryString tokenXml;
        private XmlDictionaryString upnClaim;
        private XmlDictionaryString urlClaim;
        private XmlDictionaryString value;
        private XmlDictionaryString version;
        private XmlDictionaryString windowsClaimSet;
        private XmlDictionaryString windowsSidClaim;
        private XmlDictionaryString windowsSidIdentity;
        private XmlDictionaryString x500DistinguishedNameClaim;
        private XmlDictionaryString x509CertificateClaimSet;
        private XmlDictionaryString x509ThumbprintClaim;

        private SctClaimDictionary()
        {
            this.securityContextToken = this.Add("SecurityContextSecurityToken");
            this.version = this.Add("Version");
            this.contextId = this.Add("ContextId");
            this.id = this.Add("Id");
            this.key = this.Add("Key");
            this.isCookieMode = this.Add("IsCookieMode");
            this.serviceContractId = this.Add("ServiceContractId");
            this.effectiveTime = this.Add("EffectiveTime");
            this.expiryTime = this.Add("ExpiryTime");
            this.keyGeneration = this.Add("KeyGeneration");
            this.keyEffectiveTime = this.Add("KeyEffectiveTime");
            this.keyExpiryTime = this.Add("KeyExpiryTime");
            this.claim = this.Add("Claim");
            this.claimSets = this.Add("ClaimSets");
            this.claimSet = this.Add("ClaimSet");
            this.identities = this.Add("Identities");
            this.primaryIdentity = this.Add("PrimaryIdentity");
            this.primaryIssuer = this.Add("PrimaryIssuer");
            this.x509CertificateClaimSet = this.Add("X509CertificateClaimSet");
            this.systemClaimSet = this.Add("SystemClaimSet");
            this.windowsClaimSet = this.Add("WindowsClaimSet");
            this.anonymousClaimSet = this.Add("AnonymousClaimSet");
            this.binaryClaim = this.Add("BinaryClaim");
            this.dnsClaim = this.Add("DnsClaim");
            this.genericIdentity = this.Add("GenericIdentity");
            this.authenticationType = this.Add("AuthenticationType");
            this.right = this.Add("Right");
            this.hashClaim = this.Add("HashClaim");
            this.mailAddressClaim = this.Add("MailAddressClaim");
            this.nameClaim = this.Add("NameClaim");
            this.rsaClaim = this.Add("RsaClaim");
            this.spnClaim = this.Add("SpnClaim");
            this.systemClaim = this.Add("SystemClaim");
            this.upnClaim = this.Add("UpnClaim");
            this.urlClaim = this.Add("UrlClaim");
            this.windowsSidClaim = this.Add("WindowsSidClaim");
            this.denyOnlySidClaim = this.Add("DenyOnlySidClaim");
            this.windowsSidIdentity = this.Add("WindowsSidIdentity");
            this.x500DistinguishedNameClaim = this.Add("X500DistinguishedClaim");
            this.x509ThumbprintClaim = this.Add("X509ThumbprintClaim");
            this.name = this.Add("Name");
            this.sid = this.Add("Sid");
            this.value = this.Add("Value");
            this.nullValue = this.Add("Null");
            this.genericXmlToken = this.Add("GenericXmlSecurityToken");
            this.tokenType = this.Add("TokenType");
            this.internalTokenReference = this.Add("InternalTokenReference");
            this.externalTokenReference = this.Add("ExternalTokenReference");
            this.tokenXml = this.Add("TokenXml");
            this.emptyString = this.Add(string.Empty);
        }

        public XmlDictionaryString AnonymousClaimSet =>
            this.anonymousClaimSet;

        public XmlDictionaryString AuthenticationType =>
            this.authenticationType;

        public XmlDictionaryString BinaryClaim =>
            this.binaryClaim;

        public XmlDictionaryString Claim =>
            this.claim;

        public XmlDictionaryString ClaimSet =>
            this.claimSet;

        public XmlDictionaryString ClaimSets =>
            this.claimSets;

        public XmlDictionaryString ContextId =>
            this.contextId;

        public XmlDictionaryString DenyOnlySidClaim =>
            this.denyOnlySidClaim;

        public XmlDictionaryString DnsClaim =>
            this.dnsClaim;

        public XmlDictionaryString EffectiveTime =>
            this.effectiveTime;

        public XmlDictionaryString EmptyString =>
            this.emptyString;

        public XmlDictionaryString ExpiryTime =>
            this.expiryTime;

        public XmlDictionaryString ExternalTokenReference =>
            this.externalTokenReference;

        public XmlDictionaryString GenericIdentity =>
            this.genericIdentity;

        public XmlDictionaryString GenericXmlSecurityToken =>
            this.genericXmlToken;

        public XmlDictionaryString HashClaim =>
            this.hashClaim;

        public XmlDictionaryString Id =>
            this.id;

        public XmlDictionaryString Identities =>
            this.identities;

        public static SctClaimDictionary Instance =>
            instance;

        public XmlDictionaryString InternalTokenReference =>
            this.internalTokenReference;

        public XmlDictionaryString IsCookieMode =>
            this.isCookieMode;

        public XmlDictionaryString Key =>
            this.key;

        public XmlDictionaryString KeyEffectiveTime =>
            this.keyEffectiveTime;

        public XmlDictionaryString KeyExpiryTime =>
            this.keyExpiryTime;

        public XmlDictionaryString KeyGeneration =>
            this.keyGeneration;

        public XmlDictionaryString MailAddressClaim =>
            this.mailAddressClaim;

        public XmlDictionaryString Name =>
            this.name;

        public XmlDictionaryString NameClaim =>
            this.nameClaim;

        public XmlDictionaryString NullValue =>
            this.nullValue;

        public XmlDictionaryString PrimaryIdentity =>
            this.primaryIdentity;

        public XmlDictionaryString PrimaryIssuer =>
            this.primaryIssuer;

        public XmlDictionaryString Right =>
            this.right;

        public XmlDictionaryString RsaClaim =>
            this.rsaClaim;

        public XmlDictionaryString SecurityContextSecurityToken =>
            this.securityContextToken;

        public XmlDictionaryString ServiceContractId =>
            this.serviceContractId;

        public XmlDictionaryString Sid =>
            this.sid;

        public XmlDictionaryString SpnClaim =>
            this.spnClaim;

        public XmlDictionaryString SystemClaim =>
            this.systemClaim;

        public XmlDictionaryString SystemClaimSet =>
            this.systemClaimSet;

        public XmlDictionaryString TokenType =>
            this.tokenType;

        public XmlDictionaryString TokenXml =>
            this.tokenXml;

        public XmlDictionaryString UpnClaim =>
            this.upnClaim;

        public XmlDictionaryString UrlClaim =>
            this.urlClaim;

        public XmlDictionaryString Value =>
            this.value;

        public XmlDictionaryString Version =>
            this.version;

        public XmlDictionaryString WindowsClaimSet =>
            this.windowsClaimSet;

        public XmlDictionaryString WindowsSidClaim =>
            this.windowsSidClaim;

        public XmlDictionaryString WindowsSidIdentity =>
            this.windowsSidIdentity;

        public XmlDictionaryString X500DistinguishedNameClaim =>
            this.x500DistinguishedNameClaim;

        public XmlDictionaryString X509CertificateClaimSet =>
            this.x509CertificateClaimSet;

        public XmlDictionaryString X509ThumbprintClaim =>
            this.x509ThumbprintClaim;
    }
}

