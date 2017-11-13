namespace System.IdentityModel.Selectors
{
    using Microsoft.InfoCards.Diagnostics;
    using System;
    using System.Collections.ObjectModel;
    using System.Xml;

    public class CardSpacePolicyElement
    {
        private bool m_isManagedIssuer;
        private XmlElement m_issuer;
        private Collection<XmlElement> m_parameters;
        private Uri m_policyNoticeLink;
        private int m_policyNoticeVersion;
        private XmlElement m_target;

        public CardSpacePolicyElement(XmlElement target, XmlElement issuer, Collection<XmlElement> parameters, Uri privacyNoticeLink, int privacyNoticeVersion, bool isManagedIssuer)
        {
            InfoCardTrace.ThrowInvalidArgumentConditional((privacyNoticeVersion == 0) && (null != privacyNoticeLink), "privacyNoticeVersion");
            InfoCardTrace.ThrowInvalidArgumentConditional((privacyNoticeVersion != 0) && (null == privacyNoticeLink), "privacyNoticeLink");
            this.m_target = target;
            this.m_issuer = issuer;
            this.m_parameters = parameters;
            this.m_policyNoticeLink = privacyNoticeLink;
            this.m_policyNoticeVersion = privacyNoticeVersion;
            this.m_isManagedIssuer = isManagedIssuer;
        }

        public bool IsManagedIssuer
        {
            get => 
                this.m_isManagedIssuer;
            set
            {
                this.m_isManagedIssuer = value;
            }
        }

        public XmlElement Issuer
        {
            get => 
                this.m_issuer;
            set
            {
                this.m_issuer = value;
            }
        }

        public Collection<XmlElement> Parameters =>
            this.m_parameters;

        public Uri PolicyNoticeLink
        {
            get => 
                this.m_policyNoticeLink;
            set
            {
                this.m_policyNoticeLink = value;
            }
        }

        public int PolicyNoticeVersion
        {
            get => 
                this.m_policyNoticeVersion;
            set
            {
                this.m_policyNoticeVersion = value;
            }
        }

        public XmlElement Target
        {
            get => 
                this.m_target;
            set
            {
                this.m_target = value;
            }
        }
    }
}

