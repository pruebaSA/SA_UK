namespace System.ServiceModel.Configuration
{
    using System;
    using System.ComponentModel;
    using System.Configuration;
    using System.ServiceModel;
    using System.ServiceModel.Channels;

    public abstract class ConnectionOrientedTransportElement : TransportElement
    {
        private ConfigurationPropertyCollection properties;

        internal ConnectionOrientedTransportElement()
        {
        }

        public override void ApplyConfiguration(BindingElement bindingElement)
        {
            base.ApplyConfiguration(bindingElement);
            ConnectionOrientedTransportBindingElement element = (ConnectionOrientedTransportBindingElement) bindingElement;
            element.ConnectionBufferSize = this.ConnectionBufferSize;
            element.HostNameComparisonMode = this.HostNameComparisonMode;
            element.ChannelInitializationTimeout = this.ChannelInitializationTimeout;
            if (base.ElementInformation.Properties["maxBufferSize"].ValueOrigin != PropertyValueOrigin.Default)
            {
                element.MaxBufferSize = this.MaxBufferSize;
            }
            element.MaxPendingConnections = this.MaxPendingConnections;
            element.MaxOutputDelay = this.MaxOutputDelay;
            element.MaxPendingAccepts = this.MaxPendingAccepts;
            element.TransferMode = this.TransferMode;
        }

        public override void CopyFrom(ServiceModelExtensionElement from)
        {
            base.CopyFrom(from);
            ConnectionOrientedTransportElement element = (ConnectionOrientedTransportElement) from;
            this.ConnectionBufferSize = element.ConnectionBufferSize;
            this.HostNameComparisonMode = element.HostNameComparisonMode;
            this.ChannelInitializationTimeout = element.ChannelInitializationTimeout;
            this.MaxBufferSize = element.MaxBufferSize;
            this.MaxPendingConnections = element.MaxPendingConnections;
            this.MaxOutputDelay = element.MaxOutputDelay;
            this.MaxPendingAccepts = element.MaxPendingAccepts;
            this.TransferMode = element.TransferMode;
        }

        protected internal override void InitializeFrom(BindingElement bindingElement)
        {
            base.InitializeFrom(bindingElement);
            ConnectionOrientedTransportBindingElement element = (ConnectionOrientedTransportBindingElement) bindingElement;
            this.ConnectionBufferSize = element.ConnectionBufferSize;
            this.HostNameComparisonMode = element.HostNameComparisonMode;
            this.ChannelInitializationTimeout = element.ChannelInitializationTimeout;
            this.MaxBufferSize = element.MaxBufferSize;
            this.MaxPendingConnections = element.MaxPendingConnections;
            this.MaxOutputDelay = element.MaxOutputDelay;
            this.MaxPendingAccepts = element.MaxPendingAccepts;
            this.TransferMode = element.TransferMode;
        }

        [ConfigurationProperty("channelInitializationTimeout", DefaultValue="00:00:05"), ServiceModelTimeSpanValidator(MinValueString="00:00:00.0000001"), TypeConverter(typeof(TimeSpanOrInfiniteConverter))]
        public TimeSpan ChannelInitializationTimeout
        {
            get => 
                ((TimeSpan) base["channelInitializationTimeout"]);
            set
            {
                base["channelInitializationTimeout"] = value;
            }
        }

        [ConfigurationProperty("connectionBufferSize", DefaultValue=0x2000), IntegerValidator(MinValue=1)]
        public int ConnectionBufferSize
        {
            get => 
                ((int) base["connectionBufferSize"]);
            set
            {
                base["connectionBufferSize"] = value;
            }
        }

        [ConfigurationProperty("hostNameComparisonMode", DefaultValue=0), ServiceModelEnumValidator(typeof(HostNameComparisonModeHelper))]
        public System.ServiceModel.HostNameComparisonMode HostNameComparisonMode
        {
            get => 
                ((System.ServiceModel.HostNameComparisonMode) base["hostNameComparisonMode"]);
            set
            {
                base["hostNameComparisonMode"] = value;
            }
        }

        [IntegerValidator(MinValue=1), ConfigurationProperty("maxBufferSize", DefaultValue=0x10000)]
        public int MaxBufferSize
        {
            get => 
                ((int) base["maxBufferSize"]);
            set
            {
                base["maxBufferSize"] = value;
            }
        }

        [ServiceModelTimeSpanValidator(MinValueString="00:00:00"), TypeConverter(typeof(TimeSpanOrInfiniteConverter)), ConfigurationProperty("maxOutputDelay", DefaultValue="00:00:00.2")]
        public TimeSpan MaxOutputDelay
        {
            get => 
                ((TimeSpan) base["maxOutputDelay"]);
            set
            {
                base["maxOutputDelay"] = value;
            }
        }

        [ConfigurationProperty("maxPendingAccepts", DefaultValue=1), IntegerValidator(MinValue=1)]
        public int MaxPendingAccepts
        {
            get => 
                ((int) base["maxPendingAccepts"]);
            set
            {
                base["maxPendingAccepts"] = value;
            }
        }

        [ConfigurationProperty("maxPendingConnections", DefaultValue=10), IntegerValidator(MinValue=1)]
        public int MaxPendingConnections
        {
            get => 
                ((int) base["maxPendingConnections"]);
            set
            {
                base["maxPendingConnections"] = value;
            }
        }

        protected override ConfigurationPropertyCollection Properties
        {
            get
            {
                if (this.properties == null)
                {
                    ConfigurationPropertyCollection properties = base.Properties;
                    properties.Add(new ConfigurationProperty("connectionBufferSize", typeof(int), 0x2000, null, new IntegerValidator(1, 0x7fffffff, false), ConfigurationPropertyOptions.None));
                    properties.Add(new ConfigurationProperty("hostNameComparisonMode", typeof(System.ServiceModel.HostNameComparisonMode), System.ServiceModel.HostNameComparisonMode.StrongWildcard, null, new ServiceModelEnumValidator(typeof(HostNameComparisonModeHelper)), ConfigurationPropertyOptions.None));
                    properties.Add(new ConfigurationProperty("channelInitializationTimeout", typeof(TimeSpan), TimeSpan.Parse("00:00:05"), new TimeSpanOrInfiniteConverter(), new TimeSpanOrInfiniteValidator(TimeSpan.Parse("00:00:00.0000001"), TimeSpan.Parse("24.20:31:23.6470000")), ConfigurationPropertyOptions.None));
                    properties.Add(new ConfigurationProperty("maxBufferSize", typeof(int), 0x10000, null, new IntegerValidator(1, 0x7fffffff, false), ConfigurationPropertyOptions.None));
                    properties.Add(new ConfigurationProperty("maxPendingConnections", typeof(int), 10, null, new IntegerValidator(1, 0x7fffffff, false), ConfigurationPropertyOptions.None));
                    properties.Add(new ConfigurationProperty("maxOutputDelay", typeof(TimeSpan), TimeSpan.Parse("00:00:00.2"), new TimeSpanOrInfiniteConverter(), new TimeSpanOrInfiniteValidator(TimeSpan.Parse("00:00:00"), TimeSpan.Parse("24.20:31:23.6470000")), ConfigurationPropertyOptions.None));
                    properties.Add(new ConfigurationProperty("maxPendingAccepts", typeof(int), 1, null, new IntegerValidator(1, 0x7fffffff, false), ConfigurationPropertyOptions.None));
                    properties.Add(new ConfigurationProperty("transferMode", typeof(System.ServiceModel.TransferMode), System.ServiceModel.TransferMode.Buffered, null, new ServiceModelEnumValidator(typeof(TransferModeHelper)), ConfigurationPropertyOptions.None));
                    this.properties = properties;
                }
                return this.properties;
            }
        }

        [ServiceModelEnumValidator(typeof(TransferModeHelper)), ConfigurationProperty("transferMode", DefaultValue=0)]
        public System.ServiceModel.TransferMode TransferMode
        {
            get => 
                ((System.ServiceModel.TransferMode) base["transferMode"]);
            set
            {
                base["transferMode"] = value;
            }
        }
    }
}

