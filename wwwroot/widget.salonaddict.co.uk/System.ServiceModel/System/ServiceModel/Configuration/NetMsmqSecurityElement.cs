namespace System.ServiceModel.Configuration
{
    using System;
    using System.Configuration;
    using System.ServiceModel;

    public sealed class NetMsmqSecurityElement : ConfigurationElement
    {
        private ConfigurationPropertyCollection properties;

        internal void ApplyConfiguration(NetMsmqSecurity security)
        {
            if (security == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("security");
            }
            security.Mode = this.Mode;
            this.Transport.ApplyConfiguration(security.Transport);
            this.Message.ApplyConfiguration(security.Message);
        }

        internal void InitializeFrom(NetMsmqSecurity security)
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
        public MessageSecurityOverMsmqElement Message =>
            ((MessageSecurityOverMsmqElement) base["message"]);

        [ServiceModelEnumValidator(typeof(SecurityModeHelper)), ConfigurationProperty("mode", DefaultValue=1)]
        public NetMsmqSecurityMode Mode
        {
            get => 
                ((NetMsmqSecurityMode) base["mode"]);
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
                        new ConfigurationProperty("mode", typeof(NetMsmqSecurityMode), NetMsmqSecurityMode.Transport, null, new ServiceModelEnumValidator(typeof(SecurityModeHelper)), ConfigurationPropertyOptions.None),
                        new ConfigurationProperty("transport", typeof(MsmqTransportSecurityElement), null, null, null, ConfigurationPropertyOptions.None),
                        new ConfigurationProperty("message", typeof(MessageSecurityOverMsmqElement), null, null, null, ConfigurationPropertyOptions.None)
                    };
                    this.properties = propertys;
                }
                return this.properties;
            }
        }

        [ConfigurationProperty("transport")]
        public MsmqTransportSecurityElement Transport =>
            ((MsmqTransportSecurityElement) base["transport"]);
    }
}

