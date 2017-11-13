namespace System.ServiceModel.Configuration
{
    using System;
    using System.ComponentModel;
    using System.Configuration;
    using System.ServiceModel;
    using System.ServiceModel.Security;

    public sealed class MessageSecurityOverTcpElement : ConfigurationElement
    {
        private ConfigurationPropertyCollection properties;

        internal void ApplyConfiguration(MessageSecurityOverTcp security)
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

        internal void InitializeFrom(MessageSecurityOverTcp security)
        {
            if (security == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("security");
            }
            this.ClientCredentialType = security.ClientCredentialType;
            if (security.WasAlgorithmSuiteSet)
            {
                this.AlgorithmSuite = security.AlgorithmSuite;
            }
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

        [ConfigurationProperty("clientCredentialType", DefaultValue=1), ServiceModelEnumValidator(typeof(MessageCredentialTypeHelper))]
        public MessageCredentialType ClientCredentialType
        {
            get => 
                ((MessageCredentialType) base["clientCredentialType"]);
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
                        new ConfigurationProperty("clientCredentialType", typeof(MessageCredentialType), MessageCredentialType.Windows, null, new ServiceModelEnumValidator(typeof(MessageCredentialTypeHelper)), ConfigurationPropertyOptions.None),
                        new ConfigurationProperty("algorithmSuite", typeof(SecurityAlgorithmSuite), "Default", new SecurityAlgorithmSuiteConverter(), null, ConfigurationPropertyOptions.None)
                    };
                    this.properties = propertys;
                }
                return this.properties;
            }
        }
    }
}

