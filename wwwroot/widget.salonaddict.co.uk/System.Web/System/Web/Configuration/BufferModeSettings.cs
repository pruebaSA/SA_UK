namespace System.Web.Configuration
{
    using System;
    using System.ComponentModel;
    using System.Configuration;
    using System.Globalization;
    using System.Security.Permissions;
    using System.Web;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public sealed class BufferModeSettings : ConfigurationElement
    {
        private static ConfigurationPropertyCollection _properties = new ConfigurationPropertyCollection();
        private static readonly ConfigurationProperty _propMaxBufferSize = new ConfigurationProperty("maxBufferSize", typeof(int), 0x7fffffff, new InfiniteIntConverter(), StdValidatorsAndConverters.NonZeroPositiveIntegerValidator, ConfigurationPropertyOptions.IsRequired);
        private static readonly ConfigurationProperty _propMaxBufferThreads = new ConfigurationProperty("maxBufferThreads", typeof(int), 1, new InfiniteIntConverter(), StdValidatorsAndConverters.NonZeroPositiveIntegerValidator, ConfigurationPropertyOptions.None);
        private static readonly ConfigurationProperty _propMaxFlushSize = new ConfigurationProperty("maxFlushSize", typeof(int), 0x7fffffff, new InfiniteIntConverter(), StdValidatorsAndConverters.NonZeroPositiveIntegerValidator, ConfigurationPropertyOptions.IsRequired);
        private static readonly ConfigurationProperty _propName = new ConfigurationProperty("name", typeof(string), null, null, StdValidatorsAndConverters.NonEmptyStringValidator, ConfigurationPropertyOptions.IsKey | ConfigurationPropertyOptions.IsRequired);
        private static readonly ConfigurationProperty _propRegularFlushInterval = new ConfigurationProperty("regularFlushInterval", typeof(TimeSpan), TimeSpan.FromSeconds(1.0), StdValidatorsAndConverters.InfiniteTimeSpanConverter, StdValidatorsAndConverters.PositiveTimeSpanValidator, ConfigurationPropertyOptions.IsRequired);
        private static readonly ConfigurationProperty _propUrgentFlushInterval = new ConfigurationProperty("urgentFlushInterval", typeof(TimeSpan), TimeSpan.Zero, StdValidatorsAndConverters.InfiniteTimeSpanConverter, null, ConfigurationPropertyOptions.IsRequired);
        private static readonly ConfigurationProperty _propUrgentFlushThreshold = new ConfigurationProperty("urgentFlushThreshold", typeof(int), 0x7fffffff, new InfiniteIntConverter(), StdValidatorsAndConverters.NonZeroPositiveIntegerValidator, ConfigurationPropertyOptions.IsRequired);
        private const int DefaultMaxBufferThreads = 1;
        private static readonly ConfigurationElementProperty s_elemProperty = new ConfigurationElementProperty(new CallbackValidator(typeof(BufferModeSettings), new ValidatorCallback(BufferModeSettings.Validate)));

        static BufferModeSettings()
        {
            _properties.Add(_propName);
            _properties.Add(_propMaxBufferSize);
            _properties.Add(_propMaxFlushSize);
            _properties.Add(_propUrgentFlushThreshold);
            _properties.Add(_propRegularFlushInterval);
            _properties.Add(_propUrgentFlushInterval);
            _properties.Add(_propMaxBufferThreads);
        }

        internal BufferModeSettings()
        {
        }

        public BufferModeSettings(string name, int maxBufferSize, int maxFlushSize, int urgentFlushThreshold, TimeSpan regularFlushInterval, TimeSpan urgentFlushInterval, int maxBufferThreads) : this()
        {
            this.Name = name;
            this.MaxBufferSize = maxBufferSize;
            this.MaxFlushSize = maxFlushSize;
            this.UrgentFlushThreshold = urgentFlushThreshold;
            this.RegularFlushInterval = regularFlushInterval;
            this.UrgentFlushInterval = urgentFlushInterval;
            this.MaxBufferThreads = maxBufferThreads;
        }

        private static void Validate(object value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("bufferMode");
            }
            BufferModeSettings settings = (BufferModeSettings) value;
            if (settings.UrgentFlushThreshold > settings.MaxBufferSize)
            {
                throw new ConfigurationErrorsException(System.Web.SR.GetString("Invalid_attribute1_must_less_than_or_equal_attribute2", new object[] { settings.UrgentFlushThreshold.ToString(CultureInfo.InvariantCulture), "urgentFlushThreshold", settings.MaxBufferSize.ToString(CultureInfo.InvariantCulture), "maxBufferSize" }), settings.ElementInformation.Properties["urgentFlushThreshold"].Source, settings.ElementInformation.Properties["urgentFlushThreshold"].LineNumber);
            }
            if (settings.MaxFlushSize > settings.MaxBufferSize)
            {
                throw new ConfigurationErrorsException(System.Web.SR.GetString("Invalid_attribute1_must_less_than_or_equal_attribute2", new object[] { settings.MaxFlushSize.ToString(CultureInfo.InvariantCulture), "maxFlushSize", settings.MaxBufferSize.ToString(CultureInfo.InvariantCulture), "maxBufferSize" }), settings.ElementInformation.Properties["maxFlushSize"].Source, settings.ElementInformation.Properties["maxFlushSize"].LineNumber);
            }
            if (settings.UrgentFlushInterval >= settings.RegularFlushInterval)
            {
                throw new ConfigurationErrorsException(System.Web.SR.GetString("Invalid_attribute1_must_less_than_attribute2", new object[] { settings.UrgentFlushInterval.ToString(), "urgentFlushInterval", settings.RegularFlushInterval.ToString(), "regularFlushInterval" }), settings.ElementInformation.Properties["urgentFlushInterval"].Source, settings.ElementInformation.Properties["urgentFlushInterval"].LineNumber);
            }
        }

        protected override ConfigurationElementProperty ElementProperty =>
            s_elemProperty;

        [ConfigurationProperty("maxBufferSize", IsRequired=true, DefaultValue=0x7fffffff), IntegerValidator(MinValue=1), TypeConverter(typeof(InfiniteIntConverter))]
        public int MaxBufferSize
        {
            get => 
                ((int) base[_propMaxBufferSize]);
            set
            {
                base[_propMaxBufferSize] = value;
            }
        }

        [ConfigurationProperty("maxBufferThreads", DefaultValue=1), TypeConverter(typeof(InfiniteIntConverter)), IntegerValidator(MinValue=1)]
        public int MaxBufferThreads
        {
            get => 
                ((int) base[_propMaxBufferThreads]);
            set
            {
                base[_propMaxBufferThreads] = value;
            }
        }

        [IntegerValidator(MinValue=1), TypeConverter(typeof(InfiniteIntConverter)), ConfigurationProperty("maxFlushSize", IsRequired=true, DefaultValue=0x7fffffff)]
        public int MaxFlushSize
        {
            get => 
                ((int) base[_propMaxFlushSize]);
            set
            {
                base[_propMaxFlushSize] = value;
            }
        }

        [StringValidator(MinLength=1), ConfigurationProperty("name", IsRequired=true, IsKey=true, DefaultValue="")]
        public string Name
        {
            get => 
                ((string) base[_propName]);
            set
            {
                base[_propName] = value;
            }
        }

        protected override ConfigurationPropertyCollection Properties =>
            _properties;

        [TimeSpanValidator(MinValueString="00:00:00", MaxValueString="10675199.02:48:05.4775807"), TypeConverter(typeof(InfiniteTimeSpanConverter)), ConfigurationProperty("regularFlushInterval", IsRequired=true, DefaultValue="00:00:01")]
        public TimeSpan RegularFlushInterval
        {
            get => 
                ((TimeSpan) base[_propRegularFlushInterval]);
            set
            {
                base[_propRegularFlushInterval] = value;
            }
        }

        [TypeConverter(typeof(InfiniteTimeSpanConverter)), ConfigurationProperty("urgentFlushInterval", IsRequired=true, DefaultValue="00:00:00")]
        public TimeSpan UrgentFlushInterval
        {
            get => 
                ((TimeSpan) base[_propUrgentFlushInterval]);
            set
            {
                base[_propUrgentFlushInterval] = value;
            }
        }

        [ConfigurationProperty("urgentFlushThreshold", IsRequired=true, DefaultValue=0x7fffffff), IntegerValidator(MinValue=1), TypeConverter(typeof(InfiniteIntConverter))]
        public int UrgentFlushThreshold
        {
            get => 
                ((int) base[_propUrgentFlushThreshold]);
            set
            {
                base[_propUrgentFlushThreshold] = value;
            }
        }
    }
}

