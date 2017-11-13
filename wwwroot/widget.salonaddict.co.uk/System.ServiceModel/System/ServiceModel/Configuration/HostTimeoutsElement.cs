namespace System.ServiceModel.Configuration
{
    using System;
    using System.ComponentModel;
    using System.Configuration;
    using System.ServiceModel;

    public sealed class HostTimeoutsElement : ConfigurationElement
    {
        private ConfigurationPropertyCollection properties;

        [ServiceModelTimeSpanValidator(MinValueString="00:00:00"), ConfigurationProperty("closeTimeout", DefaultValue="00:00:10"), TypeConverter(typeof(TimeSpanOrInfiniteConverter))]
        public TimeSpan CloseTimeout
        {
            get => 
                ((TimeSpan) base["closeTimeout"]);
            set
            {
                base["closeTimeout"] = value;
            }
        }

        [ServiceModelTimeSpanValidator(MinValueString="00:00:00"), ConfigurationProperty("openTimeout", DefaultValue="00:01:00"), TypeConverter(typeof(TimeSpanOrInfiniteConverter))]
        public TimeSpan OpenTimeout
        {
            get => 
                ((TimeSpan) base["openTimeout"]);
            set
            {
                base["openTimeout"] = value;
            }
        }

        protected override ConfigurationPropertyCollection Properties
        {
            get
            {
                if (this.properties == null)
                {
                    ConfigurationPropertyCollection propertys = new ConfigurationPropertyCollection {
                        new ConfigurationProperty("closeTimeout", typeof(TimeSpan), TimeSpan.Parse("00:00:10"), new TimeSpanOrInfiniteConverter(), new TimeSpanOrInfiniteValidator(TimeSpan.Parse("00:00:00"), TimeSpan.Parse("24.20:31:23.6470000")), ConfigurationPropertyOptions.None),
                        new ConfigurationProperty("openTimeout", typeof(TimeSpan), TimeSpan.Parse("00:01:00"), new TimeSpanOrInfiniteConverter(), new TimeSpanOrInfiniteValidator(TimeSpan.Parse("00:00:00"), TimeSpan.Parse("24.20:31:23.6470000")), ConfigurationPropertyOptions.None)
                    };
                    this.properties = propertys;
                }
                return this.properties;
            }
        }
    }
}

