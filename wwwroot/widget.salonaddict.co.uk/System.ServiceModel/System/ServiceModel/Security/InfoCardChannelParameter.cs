namespace System.ServiceModel.Security
{
    using System;
    using System.IdentityModel.Tokens;

    internal class InfoCardChannelParameter
    {
        private Uri m_relyingPartyIssuer;
        private bool m_requiresInfocard;
        private SecurityToken m_token;

        public InfoCardChannelParameter(SecurityToken token, Uri relyingIssuer, bool requiresInfoCard)
        {
            this.m_token = token;
            this.m_relyingPartyIssuer = relyingIssuer;
            this.m_requiresInfocard = requiresInfoCard;
        }

        public Uri RelyingPartyIssuer =>
            this.m_relyingPartyIssuer;

        public bool RequiresInfoCard =>
            this.m_requiresInfocard;

        public SecurityToken Token =>
            this.m_token;
    }
}

