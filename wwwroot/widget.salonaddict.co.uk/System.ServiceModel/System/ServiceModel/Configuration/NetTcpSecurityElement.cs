namespace System.ServiceModel.Configuration
{
    using System;
    using System.Configuration;
    using System.ServiceModel;

    public sealed class NetTcpSecurityElement : ConfigurationElement
    {
        private ConfigurationPropertyCollection properties;

        internal void ApplyConfiguration(NetTcpSecurity security)
        {
            if (security == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("security");
            }
            security.Mode = this.Mode;
            this.Transport.ApplyConfiguration(security.Transport);
            this.Message.ApplyConfiguration(security.Message);
        }

        internal void InitializeFrom(NetTcpSecurity security)
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
        public MessageSecurityOverTcpElement Message =>
            ((MessageSecurityOverTcpElement) base["message"]);

        [ServiceModelEnumValidator(typeof(SecurityModeHelper)), ConfigurationProperty("mode", DefaultValue=1)]
        public SecurityMode Mode
        {
            get => 
                ((SecurityMode) base["mode"]);
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
                        new ConfigurationProperty("mode", typeof(SecurityMode), SecurityMode.Transport, null, new ServiceModelEnumValidator(typeof(SecurityModeHelper)), ConfigurationPropertyOptions.None),
                        new ConfigurationProperty("transport", typeof(TcpTransportSecurityElement), null, null, null, ConfigurationPropertyOptions.None),
                        new ConfigurationProperty("message", typeof(MessageSecurityOverTcpElement), null, null, null, ConfigurationPropertyOptions.None)
                    };
                    this.properties = propertys;
                }
                return this.properties;
            }
        }

        [ConfigurationProperty("transport")]
        public TcpTransportSecurityElement Transport =>
            ((TcpTransportSecurityElement) base["transport"]);
    }
}

