namespace System.ServiceModel.Configuration
{
    using System;
    using System.Configuration;
    using System.ServiceModel;
    using System.ServiceModel.PeerResolvers;

    public sealed class PeerResolverElement : ConfigurationElement
    {
        private ConfigurationPropertyCollection properties;

        internal void ApplyConfiguration(PeerResolverSettings settings)
        {
            if (settings == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("settings");
            }
            settings.Mode = this.Mode;
            settings.ReferralPolicy = this.ReferralPolicy;
            this.Custom.ApplyConfiguration(settings.Custom);
        }

        internal void InitializeFrom(PeerResolverSettings settings)
        {
            if (settings == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("settings");
            }
            this.Mode = settings.Mode;
            this.ReferralPolicy = settings.ReferralPolicy;
            this.Custom.InitializeFrom(settings.Custom);
        }

        [ConfigurationProperty("custom")]
        public PeerCustomResolverElement Custom =>
            ((PeerCustomResolverElement) base["custom"]);

        [ServiceModelEnumValidator(typeof(PeerResolverModeHelper)), ConfigurationProperty("mode", DefaultValue=0)]
        public PeerResolverMode Mode
        {
            get => 
                ((PeerResolverMode) base["mode"]);
            set
            {
                base["mode"] = value;
            }
        }

        protected override ConfigurationPropertyCollection Properties
        {
            get
            {
                if (this.properties == null)
                {
                    ConfigurationPropertyCollection propertys = new ConfigurationPropertyCollection {
                        new ConfigurationProperty("mode", typeof(PeerResolverMode), PeerResolverMode.Auto, null, new ServiceModelEnumValidator(typeof(PeerResolverModeHelper)), ConfigurationPropertyOptions.None),
                        new ConfigurationProperty("referralPolicy", typeof(PeerReferralPolicy), PeerReferralPolicy.Service, null, new ServiceModelEnumValidator(typeof(PeerReferralPolicyHelper)), ConfigurationPropertyOptions.None),
                        new ConfigurationProperty("custom", typeof(PeerCustomResolverElement), null, null, null, ConfigurationPropertyOptions.None)
                    };
                    this.properties = propertys;
                }
                return this.properties;
            }
        }

        [ConfigurationProperty("referralPolicy", DefaultValue=0), ServiceModelEnumValidator(typeof(PeerReferralPolicyHelper))]
        public PeerReferralPolicy ReferralPolicy
        {
            get => 
                ((PeerReferralPolicy) base["referralPolicy"]);
            set
            {
                base["referralPolicy"] = value;
            }
        }
    }
}

