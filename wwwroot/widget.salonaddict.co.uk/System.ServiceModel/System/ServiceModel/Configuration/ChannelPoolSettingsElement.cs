namespace System.ServiceModel.Configuration
{
    using System;
    using System.ComponentModel;
    using System.Configuration;
    using System.ServiceModel;
    using System.ServiceModel.Channels;

    public sealed class ChannelPoolSettingsElement : ConfigurationElement
    {
        private ConfigurationPropertyCollection properties;

        internal void ApplyConfiguration(ChannelPoolSettings settings)
        {
            if (settings == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("settings");
            }
            settings.IdleTimeout = this.IdleTimeout;
            settings.LeaseTimeout = this.LeaseTimeout;
            settings.MaxOutboundChannelsPerEndpoint = this.MaxOutboundChannelsPerEndpoint;
        }

        internal void CopyFrom(ChannelPoolSettingsElement source)
        {
            if (source == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("source");
            }
            this.IdleTimeout = source.IdleTimeout;
            this.LeaseTimeout = source.LeaseTimeout;
            this.MaxOutboundChannelsPerEndpoint = source.MaxOutboundChannelsPerEndpoint;
        }

        internal void InitializeFrom(ChannelPoolSettings settings)
        {
            if (settings == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("settings");
            }
            this.IdleTimeout = settings.IdleTimeout;
            this.LeaseTimeout = settings.LeaseTimeout;
            this.MaxOutboundChannelsPerEndpoint = settings.MaxOutboundChannelsPerEndpoint;
        }

        [ConfigurationProperty("idleTimeout", DefaultValue="00:02:00"), ServiceModelTimeSpanValidator(MinValueString="00:00:00"), TypeConverter(typeof(TimeSpanOrInfiniteConverter))]
        public TimeSpan IdleTimeout
        {
            get => 
                ((TimeSpan) base["idleTimeout"]);
            set
            {
                base["idleTimeout"] = value;
            }
        }

        [ServiceModelTimeSpanValidator(MinValueString="00:00:00"), ConfigurationProperty("leaseTimeout", DefaultValue="00:10:00"), TypeConverter(typeof(TimeSpanOrInfiniteConverter))]
        public TimeSpan LeaseTimeout
        {
            get => 
                ((TimeSpan) base["leaseTimeout"]);
            set
            {
                base["leaseTimeout"] = value;
            }
        }

        [ConfigurationProperty("maxOutboundChannelsPerEndpoint", DefaultValue=10), IntegerValidator(MinValue=1)]
        public int MaxOutboundChannelsPerEndpoint
        {
            get => 
                ((int) base["maxOutboundChannelsPerEndpoint"]);
            set
            {
                base["maxOutboundChannelsPerEndpoint"] = value;
            }
        }

        protected override ConfigurationPropertyCollection Properties
        {
            get
            {
                if (this.properties == null)
                {
                    ConfigurationPropertyCollection propertys = new ConfigurationPropertyCollection {
                        new ConfigurationProperty("idleTimeout", typeof(TimeSpan), TimeSpan.Parse("00:02:00"), new TimeSpanOrInfiniteConverter(), new TimeSpanOrInfiniteValidator(TimeSpan.Parse("00:00:00"), TimeSpan.Parse("24.20:31:23.6470000")), ConfigurationPropertyOptions.None),
                        new ConfigurationProperty("leaseTimeout", typeof(TimeSpan), TimeSpan.Parse("00:10:00"), new TimeSpanOrInfiniteConverter(), new TimeSpanOrInfiniteValidator(TimeSpan.Parse("00:00:00"), TimeSpan.Parse("24.20:31:23.6470000")), ConfigurationPropertyOptions.None),
                        new ConfigurationProperty("maxOutboundChannelsPerEndpoint", typeof(int), 10, null, new IntegerValidator(1, 0x7fffffff, false), ConfigurationPropertyOptions.None)
                    };
                    this.properties = propertys;
                }
                return this.properties;
            }
        }
    }
}

