﻿namespace System.ServiceModel.Configuration
{
    using System;
    using System.Configuration;
    using System.ServiceModel.Description;

    public sealed class ClientViaElement : BehaviorExtensionElement
    {
        private ConfigurationPropertyCollection properties;

        public override void CopyFrom(ServiceModelExtensionElement from)
        {
            base.CopyFrom(from);
            ClientViaElement element = (ClientViaElement) from;
            this.ViaUri = element.ViaUri;
        }

        protected internal override object CreateBehavior() => 
            new ClientViaBehavior(this.ViaUri);

        public override Type BehaviorType =>
            typeof(ClientViaBehavior);

        protected override ConfigurationPropertyCollection Properties
        {
            get
            {
                if (this.properties == null)
                {
                    ConfigurationPropertyCollection propertys = new ConfigurationPropertyCollection {
                        new ConfigurationProperty("viaUri", typeof(Uri), null, null, null, ConfigurationPropertyOptions.None)
                    };
                    this.properties = propertys;
                }
                return this.properties;
            }
        }

        [ConfigurationProperty("viaUri")]
        public Uri ViaUri
        {
            get => 
                ((Uri) base["viaUri"]);
            set
            {
                base["viaUri"] = value;
            }
        }
    }
}

