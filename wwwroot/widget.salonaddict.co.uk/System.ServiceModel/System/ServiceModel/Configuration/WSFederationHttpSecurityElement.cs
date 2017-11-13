namespace System.ServiceModel.Configuration
{
    using System;
    using System.Configuration;
    using System.ServiceModel;

    public sealed class WSFederationHttpSecurityElement : ConfigurationElement
    {
        private ConfigurationPropertyCollection properties;

        internal void ApplyConfiguration(WSFederationHttpSecurity security)
        {
            if (security == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("security");
            }
            security.Mode = this.Mode;
            this.Message.ApplyConfiguration(security.Message);
        }

        internal void InitializeFrom(WSFederationHttpSecurity security)
        {
            if (security == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("security");
            }
            this.Mode = security.Mode;
            this.Message.InitializeFrom(security.Message);
        }

        [ConfigurationProperty("message")]
        public FederatedMessageSecurityOverHttpElement Message =>
            ((FederatedMessageSecurityOverHttpElement) base["message"]);

        [ConfigurationProperty("mode", DefaultValue=1), ServiceModelEnumValidator(typeof(WSFederationHttpSecurityModeHelper))]
        public WSFederationHttpSecurityMode Mode
        {
            get => 
                ((WSFederationHttpSecurityMode) base["mode"]);
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
                        new ConfigurationProperty("mode", typeof(WSFederationHttpSecurityMode), WSFederationHttpSecurityMode.Message, null, new ServiceModelEnumValidator(typeof(WSFederationHttpSecurityModeHelper)), ConfigurationPropertyOptions.None),
                        new ConfigurationProperty("message", typeof(FederatedMessageSecurityOverHttpElement), null, null, null, ConfigurationPropertyOptions.None)
                    };
                    this.properties = propertys;
                }
                return this.properties;
            }
        }
    }
}

