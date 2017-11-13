namespace System.ServiceModel.Configuration
{
    using System;
    using System.Configuration;
    using System.ServiceModel;

    public sealed class WebHttpSecurityElement : ConfigurationElement
    {
        private ConfigurationPropertyCollection properties;

        private void ApplyConfiguration(HttpTransportSecurity security)
        {
            if (security == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("security");
            }
            security.ClientCredentialType = this.Transport.ClientCredentialType;
            security.ProxyCredentialType = this.Transport.ProxyCredentialType;
            security.Realm = this.Transport.Realm;
        }

        internal void ApplyConfiguration(WebHttpSecurity security)
        {
            if (security == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("security");
            }
            security.Mode = this.Mode;
            this.Transport.ApplyConfiguration(security.Transport);
        }

        internal void InitializeFrom(WebHttpSecurity security)
        {
            if (security == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("security");
            }
            this.Mode = security.Mode;
            this.InitializeTransportSecurity(security.Transport);
        }

        private void InitializeTransportSecurity(HttpTransportSecurity security)
        {
            if (security == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("security");
            }
            this.Transport.ClientCredentialType = security.ClientCredentialType;
            this.Transport.ProxyCredentialType = security.ProxyCredentialType;
            this.Transport.Realm = security.Realm;
        }

        [InternalEnumValidator(typeof(WebHttpSecurityModeHelper)), ConfigurationProperty("mode", DefaultValue=0)]
        public WebHttpSecurityMode Mode
        {
            get => 
                ((WebHttpSecurityMode) base["mode"]);
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
                        new ConfigurationProperty("mode", typeof(WebHttpSecurityMode), WebHttpSecurityMode.None, null, new InternalEnumValidator(typeof(WebHttpSecurityModeHelper)), ConfigurationPropertyOptions.None),
                        new ConfigurationProperty("transport", typeof(HttpTransportSecurityElement), null, null, null, ConfigurationPropertyOptions.None)
                    };
                    this.properties = propertys;
                }
                return this.properties;
            }
        }

        [ConfigurationProperty("transport")]
        public HttpTransportSecurityElement Transport =>
            ((HttpTransportSecurityElement) base["transport"]);
    }
}

