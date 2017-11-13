namespace System.Web.Configuration
{
    using System;
    using System.ComponentModel;
    using System.Configuration;
    using System.Security.Permissions;
    using System.Web;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public sealed class CacheSection : ConfigurationSection
    {
        private static readonly ConfigurationProperty _propDisableExpiration = new ConfigurationProperty("disableExpiration", typeof(bool), false, ConfigurationPropertyOptions.None);
        private static readonly ConfigurationProperty _propDisableMemoryCollection = new ConfigurationProperty("disableMemoryCollection", typeof(bool), false, ConfigurationPropertyOptions.None);
        private static ConfigurationPropertyCollection _properties = new ConfigurationPropertyCollection();
        private static readonly ConfigurationProperty _propPercentagePhysicalMemoryUsedLimit = new ConfigurationProperty("percentagePhysicalMemoryUsedLimit", typeof(int), 0, null, new IntegerValidator(0, 100), ConfigurationPropertyOptions.None);
        private static readonly ConfigurationProperty _propPrivateBytesLimit = new ConfigurationProperty("privateBytesLimit", typeof(long), 0L, null, new LongValidator(0L, 0x7fffffffffffffffL), ConfigurationPropertyOptions.None);
        private static readonly ConfigurationProperty _propPrivateBytesPollTime = new ConfigurationProperty("privateBytesPollTime", typeof(TimeSpan), DefaultPrivateBytesPollTime, StdValidatorsAndConverters.InfiniteTimeSpanConverter, StdValidatorsAndConverters.PositiveTimeSpanValidator, ConfigurationPropertyOptions.None);
        internal static TimeSpan DefaultPrivateBytesPollTime = new TimeSpan(0, 2, 0);

        static CacheSection()
        {
            _properties.Add(_propDisableMemoryCollection);
            _properties.Add(_propDisableExpiration);
            _properties.Add(_propPrivateBytesLimit);
            _properties.Add(_propPercentagePhysicalMemoryUsedLimit);
            _properties.Add(_propPrivateBytesPollTime);
        }

        [ConfigurationProperty("disableExpiration", DefaultValue=false)]
        public bool DisableExpiration
        {
            get => 
                ((bool) base[_propDisableExpiration]);
            set
            {
                base[_propDisableExpiration] = value;
            }
        }

        [ConfigurationProperty("disableMemoryCollection", DefaultValue=false)]
        public bool DisableMemoryCollection
        {
            get => 
                ((bool) base[_propDisableMemoryCollection]);
            set
            {
                base[_propDisableMemoryCollection] = value;
            }
        }

        [IntegerValidator(MinValue=0, MaxValue=100), ConfigurationProperty("percentagePhysicalMemoryUsedLimit", DefaultValue=0)]
        public int PercentagePhysicalMemoryUsedLimit
        {
            get => 
                ((int) base[_propPercentagePhysicalMemoryUsedLimit]);
            set
            {
                base[_propPercentagePhysicalMemoryUsedLimit] = value;
            }
        }

        [ConfigurationProperty("privateBytesLimit", DefaultValue=0L), LongValidator(MinValue=0L)]
        public long PrivateBytesLimit
        {
            get => 
                ((long) base[_propPrivateBytesLimit]);
            set
            {
                base[_propPrivateBytesLimit] = value;
            }
        }

        [TypeConverter(typeof(InfiniteTimeSpanConverter)), ConfigurationProperty("privateBytesPollTime", DefaultValue="00:02:00")]
        public TimeSpan PrivateBytesPollTime
        {
            get => 
                ((TimeSpan) base[_propPrivateBytesPollTime]);
            set
            {
                base[_propPrivateBytesPollTime] = value;
            }
        }

        protected override ConfigurationPropertyCollection Properties =>
            _properties;
    }
}

