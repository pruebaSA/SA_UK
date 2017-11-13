﻿namespace System.ServiceModel.Configuration
{
    using System;
    using System.Configuration;
    using System.ServiceModel.Channels;

    public sealed class OneWayElement : BindingElementExtensionElement
    {
        private ConfigurationPropertyCollection properties;

        public override void ApplyConfiguration(BindingElement bindingElement)
        {
            base.ApplyConfiguration(bindingElement);
            OneWayBindingElement element = (OneWayBindingElement) bindingElement;
            if (base.ElementInformation.Properties["channelPoolSettings"].ValueOrigin != PropertyValueOrigin.Default)
            {
                this.ChannelPoolSettings.ApplyConfiguration(element.ChannelPoolSettings);
            }
            element.MaxAcceptedChannels = this.MaxAcceptedChannels;
            element.PacketRoutable = this.PacketRoutable;
        }

        public override void CopyFrom(ServiceModelExtensionElement from)
        {
            base.CopyFrom(from);
            OneWayElement element = (OneWayElement) from;
            if (element.ElementInformation.Properties["channelPoolSettings"].ValueOrigin != PropertyValueOrigin.Default)
            {
                this.ChannelPoolSettings.CopyFrom(element.ChannelPoolSettings);
            }
            this.MaxAcceptedChannels = element.MaxAcceptedChannels;
            this.PacketRoutable = element.PacketRoutable;
        }

        protected internal override BindingElement CreateBindingElement()
        {
            OneWayBindingElement bindingElement = new OneWayBindingElement();
            this.ApplyConfiguration(bindingElement);
            return bindingElement;
        }

        protected internal override void InitializeFrom(BindingElement bindingElement)
        {
            base.InitializeFrom(bindingElement);
            OneWayBindingElement element = (OneWayBindingElement) bindingElement;
            this.ChannelPoolSettings.InitializeFrom(element.ChannelPoolSettings);
            this.MaxAcceptedChannels = element.MaxAcceptedChannels;
            this.PacketRoutable = element.PacketRoutable;
        }

        public override Type BindingElementType =>
            typeof(OneWayBindingElement);

        [ConfigurationProperty("channelPoolSettings")]
        public ChannelPoolSettingsElement ChannelPoolSettings =>
            ((ChannelPoolSettingsElement) base["channelPoolSettings"]);

        [IntegerValidator(MinValue=1), ConfigurationProperty("maxAcceptedChannels", DefaultValue=10)]
        public int MaxAcceptedChannels
        {
            get => 
                ((int) base["maxAcceptedChannels"]);
            set
            {
                base["maxAcceptedChannels"] = value;
            }
        }

        [ConfigurationProperty("packetRoutable", DefaultValue=false)]
        public bool PacketRoutable
        {
            get => 
                ((bool) base["packetRoutable"]);
            set
            {
                base["packetRoutable"] = value;
            }
        }

        protected override ConfigurationPropertyCollection Properties
        {
            get
            {
                if (this.properties == null)
                {
                    ConfigurationPropertyCollection propertys = new ConfigurationPropertyCollection {
                        new ConfigurationProperty("channelPoolSettings", typeof(ChannelPoolSettingsElement), null, null, null, ConfigurationPropertyOptions.None),
                        new ConfigurationProperty("maxAcceptedChannels", typeof(int), 10, null, new IntegerValidator(1, 0x7fffffff, false), ConfigurationPropertyOptions.None),
                        new ConfigurationProperty("packetRoutable", typeof(bool), false, null, null, ConfigurationPropertyOptions.None)
                    };
                    this.properties = propertys;
                }
                return this.properties;
            }
        }
    }
}

