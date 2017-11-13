﻿namespace System.ServiceModel.Configuration
{
    using System;
    using System.Configuration;
    using System.Security.Authentication.ExtendedProtection.Configuration;
    using System.ServiceModel;
    using System.ServiceModel.Channels;

    public sealed class HttpTransportSecurityElement : ConfigurationElement
    {
        private ConfigurationPropertyCollection properties;

        internal void ApplyConfiguration(HttpTransportSecurity security)
        {
            if (security == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("security");
            }
            security.ClientCredentialType = this.ClientCredentialType;
            security.ProxyCredentialType = this.ProxyCredentialType;
            security.Realm = this.Realm;
            security.ExtendedProtectionPolicy = ChannelBindingUtility.BuildPolicy(this.ExtendedProtectionPolicy);
        }

        internal void InitializeFrom(HttpTransportSecurity security)
        {
            if (security == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("security");
            }
            this.ClientCredentialType = security.ClientCredentialType;
            this.ProxyCredentialType = security.ProxyCredentialType;
            this.Realm = security.Realm;
            ChannelBindingUtility.InitializeFrom(security.ExtendedProtectionPolicy, this.ExtendedProtectionPolicy);
        }

        [ServiceModelEnumValidator(typeof(HttpClientCredentialTypeHelper)), ConfigurationProperty("clientCredentialType", DefaultValue=0)]
        public HttpClientCredentialType ClientCredentialType
        {
            get => 
                ((HttpClientCredentialType) base["clientCredentialType"]);
            set
            {
                base["clientCredentialType"] = value;
            }
        }

        [ConfigurationProperty("extendedProtectionPolicy")]
        public ExtendedProtectionPolicyElement ExtendedProtectionPolicy
        {
            get => 
                ((ExtendedProtectionPolicyElement) base["extendedProtectionPolicy"]);
            private set
            {
                base["extendedProtectionPolicy"] = value;
            }
        }

        protected override ConfigurationPropertyCollection Properties
        {
            get
            {
                if (this.properties == null)
                {
                    ConfigurationPropertyCollection propertys = new ConfigurationPropertyCollection {
                        new ConfigurationProperty("clientCredentialType", typeof(HttpClientCredentialType), HttpClientCredentialType.None, null, new ServiceModelEnumValidator(typeof(HttpClientCredentialTypeHelper)), ConfigurationPropertyOptions.None),
                        new ConfigurationProperty("proxyCredentialType", typeof(HttpProxyCredentialType), HttpProxyCredentialType.None, null, new ServiceModelEnumValidator(typeof(HttpProxyCredentialTypeHelper)), ConfigurationPropertyOptions.None),
                        new ConfigurationProperty("extendedProtectionPolicy", typeof(ExtendedProtectionPolicyElement), null, null, null, ConfigurationPropertyOptions.None),
                        new ConfigurationProperty("realm", typeof(string), string.Empty, null, new StringValidator(0, 0x7fffffff, null), ConfigurationPropertyOptions.None)
                    };
                    this.properties = propertys;
                }
                return this.properties;
            }
        }

        [ConfigurationProperty("proxyCredentialType", DefaultValue=0), ServiceModelEnumValidator(typeof(HttpProxyCredentialTypeHelper))]
        public HttpProxyCredentialType ProxyCredentialType
        {
            get => 
                ((HttpProxyCredentialType) base["proxyCredentialType"]);
            set
            {
                base["proxyCredentialType"] = value;
            }
        }

        [ConfigurationProperty("realm", DefaultValue=""), StringValidator(MinLength=0)]
        public string Realm
        {
            get => 
                ((string) base["realm"]);
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    value = string.Empty;
                }
                base["realm"] = value;
            }
        }
    }
}

