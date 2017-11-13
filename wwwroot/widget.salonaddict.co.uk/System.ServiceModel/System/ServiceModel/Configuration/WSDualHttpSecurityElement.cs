﻿namespace System.ServiceModel.Configuration
{
    using System;
    using System.Configuration;
    using System.ServiceModel;

    public sealed class WSDualHttpSecurityElement : ConfigurationElement
    {
        private ConfigurationPropertyCollection properties;

        internal void ApplyConfiguration(WSDualHttpSecurity security)
        {
            if (security == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("security");
            }
            security.Mode = this.Mode;
            this.Message.ApplyConfiguration(security.Message);
        }

        internal void InitializeFrom(WSDualHttpSecurity security)
        {
            if (security == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("security");
            }
            this.Mode = security.Mode;
            this.Message.InitializeFrom(security.Message);
        }

        [ConfigurationProperty("message")]
        public MessageSecurityOverHttpElement Message =>
            ((MessageSecurityOverHttpElement) base["message"]);

        [ConfigurationProperty("mode", DefaultValue=1), ServiceModelEnumValidator(typeof(WSDualHttpSecurityModeHelper))]
        public WSDualHttpSecurityMode Mode
        {
            get => 
                ((WSDualHttpSecurityMode) base["mode"]);
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
                        new ConfigurationProperty("mode", typeof(WSDualHttpSecurityMode), WSDualHttpSecurityMode.Message, null, new ServiceModelEnumValidator(typeof(WSDualHttpSecurityModeHelper)), ConfigurationPropertyOptions.None),
                        new ConfigurationProperty("message", typeof(MessageSecurityOverHttpElement), null, null, null, ConfigurationPropertyOptions.None)
                    };
                    this.properties = propertys;
                }
                return this.properties;
            }
        }
    }
}

