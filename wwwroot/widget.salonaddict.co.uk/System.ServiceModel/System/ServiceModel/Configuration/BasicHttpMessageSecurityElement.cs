namespace System.ServiceModel.Configuration
{
    using System;
    using System.ComponentModel;
    using System.Configuration;
    using System.ServiceModel;
    using System.ServiceModel.Security;

    public sealed class BasicHttpMessageSecurityElement : ConfigurationElement
    {
        private ConfigurationPropertyCollection properties;

        internal void ApplyConfiguration(BasicHttpMessageSecurity security)
        {
            if (security == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("security");
            }
            security.ClientCredentialType = this.ClientCredentialType;
            if (base.ElementInformation.Properties["algorithmSuite"].ValueOrigin != PropertyValueOrigin.Default)
            {
                security.AlgorithmSuite = this.AlgorithmSuite;
            }
        }

        internal void InitializeFrom(BasicHttpMessageSecurity security)
        {
            if (security == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("security");
            }
            this.ClientCredentialType = security.ClientCredentialType;
            this.AlgorithmSuite = security.AlgorithmSuite;
        }

        [ConfigurationProperty("algorithmSuite", DefaultValue="Default"), TypeConverter(typeof(SecurityAlgorithmSuiteConverter))]
        public SecurityAlgorithmSuite AlgorithmSuite
        {
            get => 
                ((SecurityAlgorithmSuite) base["algorithmSuite"]);
            set
            {
                base["algorithmSuite"] = value;
            }
        }

        [ConfigurationProperty("clientCredentialType", DefaultValue=0), ServiceModelEnumValidator(typeof(BasicHttpMessageCredentialTypeHelper))]
        public BasicHttpMessageCredentialType ClientCredentialType
        {
            get => 
                ((BasicHttpMessageCredentialType) base["clientCredentialType"]);
            set
            {
                base["clientCredentialType"] = value;
            }
        }

        protected override ConfigurationPropertyCollection Properties
        {
            get
            {
                if (this.properties == null)
                {
                    ConfigurationPropertyCollection propertys = new ConfigurationPropertyCollection {
                        new ConfigurationProperty("clientCredentialType", typeof(BasicHttpMessageCredentialType), BasicHttpMessageCredentialType.UserName, null, new ServiceModelEnumValidator(typeof(BasicHttpMessageCredentialTypeHelper)), ConfigurationPropertyOptions.None),
                        new ConfigurationProperty("algorithmSuite", typeof(SecurityAlgorithmSuite), "Default", new SecurityAlgorithmSuiteConverter(), null, ConfigurationPropertyOptions.None)
                    };
                    this.properties = propertys;
                }
                return this.properties;
            }
        }
    }
}

