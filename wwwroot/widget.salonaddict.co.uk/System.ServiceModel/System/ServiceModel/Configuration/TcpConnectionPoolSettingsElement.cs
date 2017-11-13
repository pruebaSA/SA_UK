namespace System.ServiceModel.Configuration
{
    using System;
    using System.ComponentModel;
    using System.Configuration;
    using System.ServiceModel;
    using System.ServiceModel.Channels;

    public sealed class TcpConnectionPoolSettingsElement : ConfigurationElement
    {
        private ConfigurationPropertyCollection properties;

        internal void ApplyConfiguration(TcpConnectionPoolSettings settings)
        {
            if (settings == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("settings");
            }
            settings.GroupName = this.GroupName;
            settings.IdleTimeout = this.IdleTimeout;
            settings.LeaseTimeout = this.LeaseTimeout;
            settings.MaxOutboundConnectionsPerEndpoint = this.MaxOutboundConnectionsPerEndpoint;
        }

        internal void CopyFrom(TcpConnectionPoolSettingsElement source)
        {
            if (source == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("source");
            }
            this.GroupName = source.GroupName;
            this.IdleTimeout = source.IdleTimeout;
            this.LeaseTimeout = source.LeaseTimeout;
            this.MaxOutboundConnectionsPerEndpoint = source.MaxOutboundConnectionsPerEndpoint;
        }

        internal void InitializeFrom(TcpConnectionPoolSettings settings)
        {
            if (settings == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("settings");
            }
            this.GroupName = settings.GroupName;
            this.IdleTimeout = settings.IdleTimeout;
            this.LeaseTimeout = settings.LeaseTimeout;
            this.MaxOutboundConnectionsPerEndpoint = settings.MaxOutboundConnectionsPerEndpoint;
        }

        [ConfigurationProperty("groupName", DefaultValue="default"), StringValidator(MinLength=0)]
        public string GroupName
        {
            get => 
                ((string) base["groupName"]);
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    value = string.Empty;
                }
                base["groupName"] = value;
            }
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

        [ConfigurationProperty("leaseTimeout", DefaultValue="00:05:00"), TypeConverter(typeof(TimeSpanOrInfiniteConverter)), ServiceModelTimeSpanValidator(MinValueString="00:00:00")]
        public TimeSpan LeaseTimeout
        {
            get => 
                ((TimeSpan) base["leaseTimeout"]);
            set
            {
                base["leaseTimeout"] = value;
            }
        }

        [ConfigurationProperty("maxOutboundConnectionsPerEndpoint", DefaultValue=10), IntegerValidator(MinValue=0)]
        public int MaxOutboundConnectionsPerEndpoint
        {
            get => 
                ((int) base["maxOutboundConnectionsPerEndpoint"]);
            set
            {
                base["maxOutboundConnectionsPerEndpoint"] = value;
            }
        }

        protected override ConfigurationPropertyCollection Properties
        {
            get
            {
                if (this.properties == null)
                {
                    ConfigurationPropertyCollection propertys = new ConfigurationPropertyCollection {
                        new ConfigurationProperty("groupName", typeof(string), "default", null, new StringValidator(0, 0x7fffffff, null), ConfigurationPropertyOptions.None),
                        new ConfigurationProperty("leaseTimeout", typeof(TimeSpan), TimeSpan.Parse("00:05:00"), new TimeSpanOrInfiniteConverter(), new TimeSpanOrInfiniteValidator(TimeSpan.Parse("00:00:00"), TimeSpan.Parse("24.20:31:23.6470000")), ConfigurationPropertyOptions.None),
                        new ConfigurationProperty("idleTimeout", typeof(TimeSpan), TimeSpan.Parse("00:02:00"), new TimeSpanOrInfiniteConverter(), new TimeSpanOrInfiniteValidator(TimeSpan.Parse("00:00:00"), TimeSpan.Parse("24.20:31:23.6470000")), ConfigurationPropertyOptions.None),
                        new ConfigurationProperty("maxOutboundConnectionsPerEndpoint", typeof(int), 10, null, new IntegerValidator(0, 0x7fffffff, false), ConfigurationPropertyOptions.None)
                    };
                    this.properties = propertys;
                }
                return this.properties;
            }
        }
    }
}

