namespace System.ServiceModel.PeerResolvers
{
    using System;
    using System.ComponentModel;
    using System.ServiceModel;
    using System.ServiceModel.Channels;

    public class PeerResolverSettings
    {
        private PeerCustomResolverSettings customSettings = new PeerCustomResolverSettings();
        private PeerResolverMode mode;
        private PeerReferralPolicy referralPolicy;

        public PeerCustomResolverSettings Custom =>
            this.customSettings;

        public PeerResolverMode Mode
        {
            get => 
                this.mode;
            set
            {
                if (!PeerResolverModeHelper.IsDefined(value))
                {
                    PeerExceptionHelper.ThrowArgument_InvalidResolverMode(value);
                }
                this.mode = value;
            }
        }

        public PeerReferralPolicy ReferralPolicy
        {
            get => 
                this.referralPolicy;
            set
            {
                if (!PeerReferralPolicyHelper.IsDefined(value))
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidEnumArgumentException("value", (int) value, typeof(PeerReferralPolicy)));
                }
                this.referralPolicy = value;
            }
        }
    }
}

