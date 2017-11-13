namespace System.ServiceModel.Configuration
{
    using System;
    using System.Configuration;
    using System.ServiceModel;
    using System.ServiceModel.Channels;

    public class WSFederationHttpBindingElement : WSHttpBindingBaseElement
    {
        private ConfigurationPropertyCollection properties;

        public WSFederationHttpBindingElement() : this(null)
        {
        }

        public WSFederationHttpBindingElement(string name) : base(name)
        {
        }

        protected internal override void InitializeFrom(Binding binding)
        {
            base.InitializeFrom(binding);
            WSFederationHttpBinding binding2 = (WSFederationHttpBinding) binding;
            if (binding2.PrivacyNoticeAt != null)
            {
                this.PrivacyNoticeAt = binding2.PrivacyNoticeAt;
                this.PrivacyNoticeVersion = binding2.PrivacyNoticeVersion;
            }
            this.Security.InitializeFrom(binding2.Security);
        }

        protected override void OnApplyConfiguration(Binding binding)
        {
            base.OnApplyConfiguration(binding);
            WSFederationHttpBinding binding2 = (WSFederationHttpBinding) binding;
            if (this.PrivacyNoticeAt != null)
            {
                binding2.PrivacyNoticeAt = this.PrivacyNoticeAt;
                binding2.PrivacyNoticeVersion = this.PrivacyNoticeVersion;
            }
            this.Security.ApplyConfiguration(binding2.Security);
        }

        protected override Type BindingElementType =>
            typeof(WSFederationHttpBinding);

        [ConfigurationProperty("privacyNoticeAt", DefaultValue=null)]
        public Uri PrivacyNoticeAt
        {
            get => 
                ((Uri) base["privacyNoticeAt"]);
            set
            {
                base["privacyNoticeAt"] = value;
            }
        }

        [ConfigurationProperty("privacyNoticeVersion", DefaultValue=0), IntegerValidator(MinValue=0)]
        public int PrivacyNoticeVersion
        {
            get => 
                ((int) base["privacyNoticeVersion"]);
            set
            {
                base["privacyNoticeVersion"] = value;
            }
        }

        protected override ConfigurationPropertyCollection Properties
        {
            get
            {
                if (this.properties == null)
                {
                    ConfigurationPropertyCollection properties = base.Properties;
                    properties.Add(new ConfigurationProperty("privacyNoticeAt", typeof(Uri), null, null, null, ConfigurationPropertyOptions.None));
                    properties.Add(new ConfigurationProperty("privacyNoticeVersion", typeof(int), 0, null, new IntegerValidator(0, 0x7fffffff, false), ConfigurationPropertyOptions.None));
                    properties.Add(new ConfigurationProperty("security", typeof(WSFederationHttpSecurityElement), null, null, null, ConfigurationPropertyOptions.None));
                    this.properties = properties;
                }
                return this.properties;
            }
        }

        [ConfigurationProperty("security")]
        public WSFederationHttpSecurityElement Security =>
            ((WSFederationHttpSecurityElement) base["security"]);
    }
}

