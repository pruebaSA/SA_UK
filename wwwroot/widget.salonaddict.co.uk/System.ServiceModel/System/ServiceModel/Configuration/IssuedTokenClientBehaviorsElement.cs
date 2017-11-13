namespace System.ServiceModel.Configuration
{
    using System;
    using System.Configuration;

    public sealed class IssuedTokenClientBehaviorsElement : ConfigurationElement
    {
        private ConfigurationPropertyCollection properties;

        [ConfigurationProperty("behaviorConfiguration", DefaultValue=""), StringValidator(MinLength=0)]
        public string BehaviorConfiguration
        {
            get => 
                ((string) base["behaviorConfiguration"]);
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    value = string.Empty;
                }
                base["behaviorConfiguration"] = value;
            }
        }

        [ConfigurationProperty("issuerAddress", DefaultValue="", Options=ConfigurationPropertyOptions.IsKey | ConfigurationPropertyOptions.IsRequired), StringValidator(MinLength=0)]
        public string IssuerAddress
        {
            get => 
                ((string) base["issuerAddress"]);
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    value = string.Empty;
                }
                base["issuerAddress"] = value;
            }
        }

        protected override ConfigurationPropertyCollection Properties
        {
            get
            {
                if (this.properties == null)
                {
                    ConfigurationPropertyCollection propertys = new ConfigurationPropertyCollection {
                        new ConfigurationProperty("issuerAddress", typeof(string), string.Empty, null, new StringValidator(0, 0x7fffffff, null), ConfigurationPropertyOptions.IsKey | ConfigurationPropertyOptions.IsRequired),
                        new ConfigurationProperty("behaviorConfiguration", typeof(string), string.Empty, null, new StringValidator(0, 0x7fffffff, null), ConfigurationPropertyOptions.None)
                    };
                    this.properties = propertys;
                }
                return this.properties;
            }
        }
    }
}

