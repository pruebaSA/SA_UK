namespace System.Web.Configuration
{
    using System;
    using System.ComponentModel;
    using System.Configuration;
    using System.Security.Permissions;
    using System.Web;
    using System.Web.Util;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public sealed class HttpRuntimeSection : ConfigurationSection
    {
        private int _MaxRequestLengthBytes = -1;
        private static readonly ConfigurationProperty _propApartmentThreading = new ConfigurationProperty("apartmentThreading", typeof(bool), false, ConfigurationPropertyOptions.None);
        private static readonly ConfigurationProperty _propAppRequestQueueLimit = new ConfigurationProperty("appRequestQueueLimit", typeof(int), 0x1388, null, StdValidatorsAndConverters.NonZeroPositiveIntegerValidator, ConfigurationPropertyOptions.None);
        private static readonly ConfigurationProperty _propDelayNotificationTimeout = new ConfigurationProperty("delayNotificationTimeout", typeof(TimeSpan), TimeSpan.FromSeconds(5.0), StdValidatorsAndConverters.TimeSpanSecondsConverter, null, ConfigurationPropertyOptions.None);
        private static readonly ConfigurationProperty _propEnable = new ConfigurationProperty("enable", typeof(bool), true, ConfigurationPropertyOptions.None);
        private static readonly ConfigurationProperty _propEnableHeaderChecking = new ConfigurationProperty("enableHeaderChecking", typeof(bool), true, ConfigurationPropertyOptions.None);
        private static readonly ConfigurationProperty _propEnableKernelOutputCache = new ConfigurationProperty("enableKernelOutputCache", typeof(bool), true, ConfigurationPropertyOptions.None);
        private static readonly ConfigurationProperty _propEnableVersionHeader = new ConfigurationProperty("enableVersionHeader", typeof(bool), true, ConfigurationPropertyOptions.None);
        private static ConfigurationPropertyCollection _properties = new ConfigurationPropertyCollection();
        private static readonly ConfigurationProperty _propExecutionTimeout = new ConfigurationProperty("executionTimeout", typeof(TimeSpan), TimeSpan.FromSeconds(110.0), StdValidatorsAndConverters.TimeSpanSecondsConverter, StdValidatorsAndConverters.PositiveTimeSpanValidator, ConfigurationPropertyOptions.None);
        private static readonly ConfigurationProperty _propMaxRequestLength = new ConfigurationProperty("maxRequestLength", typeof(int), 0x1000, null, new IntegerValidator(0, 0x1fffff), ConfigurationPropertyOptions.None);
        private static readonly ConfigurationProperty _propMaxWaitChangeNotification = new ConfigurationProperty("maxWaitChangeNotification", typeof(int), 0, null, StdValidatorsAndConverters.PositiveIntegerValidator, ConfigurationPropertyOptions.None);
        private static readonly ConfigurationProperty _propMinFreeThreads = new ConfigurationProperty("minFreeThreads", typeof(int), 8, null, StdValidatorsAndConverters.PositiveIntegerValidator, ConfigurationPropertyOptions.None);
        private static readonly ConfigurationProperty _propMinLocalRequestFreeThreads = new ConfigurationProperty("minLocalRequestFreeThreads", typeof(int), 4, null, StdValidatorsAndConverters.PositiveIntegerValidator, ConfigurationPropertyOptions.None);
        private static readonly ConfigurationProperty _propRequestLengthDiskThreshold = new ConfigurationProperty("requestLengthDiskThreshold", typeof(int), 80, null, StdValidatorsAndConverters.NonZeroPositiveIntegerValidator, ConfigurationPropertyOptions.None);
        private static readonly ConfigurationProperty _propRequireRootedSaveAsPath = new ConfigurationProperty("requireRootedSaveAsPath", typeof(bool), true, ConfigurationPropertyOptions.None);
        private static readonly ConfigurationProperty _propSendCacheControlHeader = new ConfigurationProperty("sendCacheControlHeader", typeof(bool), true, ConfigurationPropertyOptions.None);
        private static readonly ConfigurationProperty _propShutdownTimeout = new ConfigurationProperty("shutdownTimeout", typeof(TimeSpan), TimeSpan.FromSeconds(90.0), StdValidatorsAndConverters.TimeSpanSecondsConverter, null, ConfigurationPropertyOptions.None);
        private static readonly ConfigurationProperty _propUseFullyQualifiedRedirectUrl = new ConfigurationProperty("useFullyQualifiedRedirectUrl", typeof(bool), false, ConfigurationPropertyOptions.None);
        private static readonly ConfigurationProperty _propWaitChangeNotification = new ConfigurationProperty("waitChangeNotification", typeof(int), 0, null, StdValidatorsAndConverters.PositiveIntegerValidator, ConfigurationPropertyOptions.None);
        private int _RequestLengthDiskThresholdBytes = -1;
        internal const int DefaultAppRequestQueueLimit = 100;
        internal const int DefaultDelayNotificationTimeout = 5;
        internal const bool DefaultEnableKernelOutputCache = true;
        internal const int DefaultExecutionTimeout = 110;
        internal const int DefaultMaxRequestLength = 0x400000;
        internal const int DefaultMaxWaitChangeNotification = 0;
        internal const int DefaultMinFreeThreads = 8;
        internal const int DefaultMinLocalRequestFreeThreads = 4;
        internal const int DefaultRequestLengthDiskThreshold = 0x14000;
        internal const bool DefaultRequireRootedSaveAsPath = true;
        internal const bool DefaultSendCacheControlHeader = true;
        internal const int DefaultShutdownTimeout = 90;
        internal const int DefaultWaitChangeNotification = 0;
        private bool enableVersionHeaderCache = true;
        private bool enableVersionHeaderCached;
        private TimeSpan executionTimeoutCache;
        private bool executionTimeoutCached;
        private static string s_versionHeader = null;
        private bool sendCacheControlHeaderCache;
        private bool sendCacheControlHeaderCached;

        static HttpRuntimeSection()
        {
            _properties.Add(_propExecutionTimeout);
            _properties.Add(_propMaxRequestLength);
            _properties.Add(_propRequestLengthDiskThreshold);
            _properties.Add(_propUseFullyQualifiedRedirectUrl);
            _properties.Add(_propMinFreeThreads);
            _properties.Add(_propMinLocalRequestFreeThreads);
            _properties.Add(_propAppRequestQueueLimit);
            _properties.Add(_propEnableKernelOutputCache);
            _properties.Add(_propEnableVersionHeader);
            _properties.Add(_propRequireRootedSaveAsPath);
            _properties.Add(_propEnable);
            _properties.Add(_propShutdownTimeout);
            _properties.Add(_propDelayNotificationTimeout);
            _properties.Add(_propWaitChangeNotification);
            _properties.Add(_propMaxWaitChangeNotification);
            _properties.Add(_propEnableHeaderChecking);
            _properties.Add(_propSendCacheControlHeader);
            _properties.Add(_propApartmentThreading);
        }

        private int BytesFromKilobytes(int kilobytes)
        {
            long num = kilobytes * 0x400L;
            if (num >= 0x7fffffffL)
            {
                return 0x7fffffff;
            }
            return (int) num;
        }

        [ConfigurationProperty("apartmentThreading", DefaultValue=false)]
        public bool ApartmentThreading
        {
            get => 
                ((bool) base[_propApartmentThreading]);
            set
            {
                base[_propApartmentThreading] = value;
            }
        }

        [IntegerValidator(MinValue=1), ConfigurationProperty("appRequestQueueLimit", DefaultValue=0x1388)]
        public int AppRequestQueueLimit
        {
            get => 
                ((int) base[_propAppRequestQueueLimit]);
            set
            {
                base[_propAppRequestQueueLimit] = value;
            }
        }

        [ConfigurationProperty("delayNotificationTimeout", DefaultValue="00:00:05"), TypeConverter(typeof(TimeSpanSecondsConverter))]
        public TimeSpan DelayNotificationTimeout
        {
            get => 
                ((TimeSpan) base[_propDelayNotificationTimeout]);
            set
            {
                base[_propDelayNotificationTimeout] = value;
            }
        }

        [ConfigurationProperty("enable", DefaultValue=true)]
        public bool Enable
        {
            get => 
                ((bool) base[_propEnable]);
            set
            {
                base[_propEnable] = value;
            }
        }

        [ConfigurationProperty("enableHeaderChecking", DefaultValue=true)]
        public bool EnableHeaderChecking
        {
            get => 
                ((bool) base[_propEnableHeaderChecking]);
            set
            {
                base[_propEnableHeaderChecking] = value;
            }
        }

        [ConfigurationProperty("enableKernelOutputCache", DefaultValue=true)]
        public bool EnableKernelOutputCache
        {
            get => 
                ((bool) base[_propEnableKernelOutputCache]);
            set
            {
                base[_propEnableKernelOutputCache] = value;
            }
        }

        [ConfigurationProperty("enableVersionHeader", DefaultValue=true)]
        public bool EnableVersionHeader
        {
            get
            {
                if (!this.enableVersionHeaderCached)
                {
                    this.enableVersionHeaderCache = (bool) base[_propEnableVersionHeader];
                    this.enableVersionHeaderCached = true;
                }
                return this.enableVersionHeaderCache;
            }
            set
            {
                base[_propEnableVersionHeader] = value;
                this.enableVersionHeaderCache = value;
            }
        }

        [TimeSpanValidator(MinValueString="00:00:00", MaxValueString="10675199.02:48:05.4775807"), ConfigurationProperty("executionTimeout", DefaultValue="00:01:50"), TypeConverter(typeof(TimeSpanSecondsConverter))]
        public TimeSpan ExecutionTimeout
        {
            get
            {
                if (!this.executionTimeoutCached)
                {
                    this.executionTimeoutCache = (TimeSpan) base[_propExecutionTimeout];
                    this.executionTimeoutCached = true;
                }
                return this.executionTimeoutCache;
            }
            set
            {
                base[_propExecutionTimeout] = value;
                this.executionTimeoutCache = value;
            }
        }

        [ConfigurationProperty("maxRequestLength", DefaultValue=0x1000), IntegerValidator(MinValue=0)]
        public int MaxRequestLength
        {
            get => 
                ((int) base[_propMaxRequestLength]);
            set
            {
                if (value < this.RequestLengthDiskThreshold)
                {
                    throw new ConfigurationErrorsException(System.Web.SR.GetString("Config_max_request_length_smaller_than_max_request_length_disk_threshold"), base.ElementInformation.Properties[_propMaxRequestLength.Name].Source, base.ElementInformation.Properties[_propMaxRequestLength.Name].LineNumber);
                }
                base[_propMaxRequestLength] = value;
            }
        }

        internal int MaxRequestLengthBytes
        {
            get
            {
                if (this._MaxRequestLengthBytes < 0)
                {
                    this._MaxRequestLengthBytes = this.BytesFromKilobytes(this.MaxRequestLength);
                }
                return this._MaxRequestLengthBytes;
            }
        }

        [IntegerValidator(MinValue=0), ConfigurationProperty("maxWaitChangeNotification", DefaultValue=0)]
        public int MaxWaitChangeNotification
        {
            get => 
                ((int) base[_propMaxWaitChangeNotification]);
            set
            {
                base[_propMaxWaitChangeNotification] = value;
            }
        }

        [ConfigurationProperty("minFreeThreads", DefaultValue=8), IntegerValidator(MinValue=0)]
        public int MinFreeThreads
        {
            get => 
                ((int) base[_propMinFreeThreads]);
            set
            {
                base[_propMinFreeThreads] = value;
            }
        }

        [ConfigurationProperty("minLocalRequestFreeThreads", DefaultValue=4), IntegerValidator(MinValue=0)]
        public int MinLocalRequestFreeThreads
        {
            get => 
                ((int) base[_propMinLocalRequestFreeThreads]);
            set
            {
                base[_propMinLocalRequestFreeThreads] = value;
            }
        }

        protected override ConfigurationPropertyCollection Properties =>
            _properties;

        [IntegerValidator(MinValue=1), ConfigurationProperty("requestLengthDiskThreshold", DefaultValue=80)]
        public int RequestLengthDiskThreshold
        {
            get => 
                ((int) base[_propRequestLengthDiskThreshold]);
            set
            {
                if (value > this.MaxRequestLength)
                {
                    throw new ConfigurationErrorsException(System.Web.SR.GetString("Config_max_request_length_disk_threshold_exceeds_max_request_length"), base.ElementInformation.Properties[_propRequestLengthDiskThreshold.Name].Source, base.ElementInformation.Properties[_propRequestLengthDiskThreshold.Name].LineNumber);
                }
                base[_propRequestLengthDiskThreshold] = value;
            }
        }

        internal int RequestLengthDiskThresholdBytes
        {
            get
            {
                if (this._RequestLengthDiskThresholdBytes < 0)
                {
                    this._RequestLengthDiskThresholdBytes = this.BytesFromKilobytes(this.RequestLengthDiskThreshold);
                }
                return this._RequestLengthDiskThresholdBytes;
            }
        }

        [ConfigurationProperty("requireRootedSaveAsPath", DefaultValue=true)]
        public bool RequireRootedSaveAsPath
        {
            get => 
                ((bool) base[_propRequireRootedSaveAsPath]);
            set
            {
                base[_propRequireRootedSaveAsPath] = value;
            }
        }

        [ConfigurationProperty("sendCacheControlHeader", DefaultValue=true)]
        public bool SendCacheControlHeader
        {
            get
            {
                if (!this.sendCacheControlHeaderCached)
                {
                    this.sendCacheControlHeaderCache = (bool) base[_propSendCacheControlHeader];
                    this.sendCacheControlHeaderCached = true;
                }
                return this.sendCacheControlHeaderCache;
            }
            set
            {
                base[_propSendCacheControlHeader] = value;
                this.sendCacheControlHeaderCache = value;
            }
        }

        [ConfigurationProperty("shutdownTimeout", DefaultValue="00:01:30"), TypeConverter(typeof(TimeSpanSecondsConverter))]
        public TimeSpan ShutdownTimeout
        {
            get => 
                ((TimeSpan) base[_propShutdownTimeout]);
            set
            {
                base[_propShutdownTimeout] = value;
            }
        }

        [ConfigurationProperty("useFullyQualifiedRedirectUrl", DefaultValue=false)]
        public bool UseFullyQualifiedRedirectUrl
        {
            get => 
                ((bool) base[_propUseFullyQualifiedRedirectUrl]);
            set
            {
                base[_propUseFullyQualifiedRedirectUrl] = value;
            }
        }

        internal string VersionHeader
        {
            get
            {
                if (!this.EnableVersionHeader)
                {
                    return null;
                }
                if (s_versionHeader == null)
                {
                    string str = null;
                    try
                    {
                        string systemWebVersion = VersionInfo.SystemWebVersion;
                        int length = systemWebVersion.LastIndexOf('.');
                        if (length > 0)
                        {
                            str = systemWebVersion.Substring(0, length);
                        }
                    }
                    catch
                    {
                    }
                    if (str == null)
                    {
                        str = string.Empty;
                    }
                    s_versionHeader = str;
                }
                return s_versionHeader;
            }
        }

        [ConfigurationProperty("waitChangeNotification", DefaultValue=0), IntegerValidator(MinValue=0)]
        public int WaitChangeNotification
        {
            get => 
                ((int) base[_propWaitChangeNotification]);
            set
            {
                base[_propWaitChangeNotification] = value;
            }
        }
    }
}

