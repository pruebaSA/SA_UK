﻿namespace System.ServiceModel.Configuration
{
    using System;
    using System.ComponentModel;
    using System.Configuration;
    using System.ServiceModel;
    using System.ServiceModel.Security;

    public class MessageSecurityOverHttpElement : ConfigurationElement
    {
        private ConfigurationPropertyCollection properties;

        internal MessageSecurityOverHttpElement()
        {
        }

        internal void ApplyConfiguration(MessageSecurityOverHttp security)
        {
            if (security == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("security");
            }
            security.ClientCredentialType = this.ClientCredentialType;
            security.NegotiateServiceCredential = this.NegotiateServiceCredential;
            if (base.ElementInformation.Properties["algorithmSuite"].ValueOrigin != PropertyValueOrigin.Default)
            {
                security.AlgorithmSuite = this.AlgorithmSuite;
            }
        }

        internal void InitializeFrom(MessageSecurityOverHttp security)
        {
            if (security == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("security");
            }
            this.ClientCredentialType = security.ClientCredentialType;
            this.NegotiateServiceCredential = security.NegotiateServiceCredential;
            if (security.WasAlgorithmSuiteSet)
            {
                this.AlgorithmSuite = security.AlgorithmSuite;
            }
        }

        [TypeConverter(typeof(SecurityAlgorithmSuiteConverter)), ConfigurationProperty("algorithmSuite", DefaultValue="Default")]
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

        [ConfigurationProperty("negotiateServiceCredential", DefaultValue=true)]
        public bool NegotiateServiceCredential
        {
            get => 
                ((bool) base["negotiateServiceCredential"]);
            set
            {
                base["negotiateServiceCredential"] = value;
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
                        new ConfigurationProperty("negotiateServiceCredential", typeof(bool), true, null, null, ConfigurationPropertyOptions.None),
                        new ConfigurationProperty("algorithmSuite", typeof(SecurityAlgorithmSuite), "Default", new SecurityAlgorithmSuiteConverter(), null, ConfigurationPropertyOptions.None)
                    };
                    this.properties = propertys;
                }
                return this.properties;
            }
        }
    }
}

