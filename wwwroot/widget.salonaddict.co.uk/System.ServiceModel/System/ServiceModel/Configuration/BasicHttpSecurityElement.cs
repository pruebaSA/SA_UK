namespace System.ServiceModel.Configuration
{
    using System;
    using System.Configuration;
    using System.ServiceModel;

    public sealed class BasicHttpSecurityElement : ConfigurationElement
    {
        private ConfigurationPropertyCollection properties;

        internal void ApplyConfiguration(BasicHttpSecurity security)
        {
            if (security == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("security");
            }
            security.Mode = this.Mode;
            this.Transport.ApplyConfiguration(security.Transport);
            this.Message.ApplyConfiguration(security.Message);
        }

        internal void InitializeFrom(BasicHttpSecurity security)
        {
            if (security == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("security");
            }
            this.Mode = security.Mode;
            this.Transport.InitializeFrom(security.Transport);
            this.Message.InitializeFrom(security.Message);
        }

        [ConfigurationProperty("message")]
        public BasicHttpMessageSecurityElement Message =>
            ((BasicHttpMessageSecurityElement) base["message"]);

        [ConfigurationProperty("mode", DefaultValue=0), ServiceModelEnumValidator(typeof(BasicHttpSecurityModeHelper))]
        public BasicHttpSecurityMode Mode
        {
            get => 
                ((BasicHttpSecurityMode) base["mode"]);
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
                        new ConfigurationProperty("mode", typeof(BasicHttpSecurityMode), BasicHttpSecurityMode.None, null, new ServiceModelEnumValidator(typeof(BasicHttpSecurityModeHelper)), ConfigurationPropertyOptions.None),
                        new ConfigurationProperty("transport", typeof(HttpTransportSecurityElement), null, null, null, ConfigurationPropertyOptions.None),
                        new ConfigurationProperty("message", typeof(BasicHttpMessageSecurityElement), null, null, null, ConfigurationPropertyOptions.None)
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

