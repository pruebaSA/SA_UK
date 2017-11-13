namespace System.ServiceModel.Configuration
{
    using System;
    using System.Configuration;
    using System.ServiceModel;

    public sealed class AuthorizationPolicyTypeElement : ConfigurationElement
    {
        private ConfigurationPropertyCollection properties;

        public AuthorizationPolicyTypeElement()
        {
        }

        public AuthorizationPolicyTypeElement(string policyType)
        {
            if (string.IsNullOrEmpty(policyType))
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("policyType");
            }
            this.PolicyType = policyType;
        }

        [StringValidator(MinLength=1), ConfigurationProperty("policyType", Options=ConfigurationPropertyOptions.IsKey | ConfigurationPropertyOptions.IsRequired)]
        public string PolicyType
        {
            get => 
                ((string) base["policyType"]);
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    value = string.Empty;
                }
                base["policyType"] = value;
            }
        }

        protected override ConfigurationPropertyCollection Properties
        {
            get
            {
                if (this.properties == null)
                {
                    ConfigurationPropertyCollection propertys = new ConfigurationPropertyCollection {
                        new ConfigurationProperty("policyType", typeof(string), null, null, new StringValidator(1, 0x7fffffff, null), ConfigurationPropertyOptions.IsKey | ConfigurationPropertyOptions.IsRequired)
                    };
                    this.properties = propertys;
                }
                return this.properties;
            }
        }
    }
}

