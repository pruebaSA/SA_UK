namespace System.ServiceModel.Configuration
{
    using System;
    using System.ComponentModel;
    using System.Configuration;
    using System.ServiceModel;
    using System.ServiceModel.Channels;

    public sealed class LocalClientSecuritySettingsElement : ConfigurationElement
    {
        private ConfigurationPropertyCollection properties;

        internal void ApplyConfiguration(LocalClientSecuritySettings settings)
        {
            if (settings == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("settings");
            }
            settings.CacheCookies = this.CacheCookies;
            if (base.ElementInformation.Properties["detectReplays"].ValueOrigin != PropertyValueOrigin.Default)
            {
                settings.DetectReplays = this.DetectReplays;
            }
            settings.MaxClockSkew = this.MaxClockSkew;
            settings.MaxCookieCachingTime = this.MaxCookieCachingTime;
            settings.ReconnectTransportOnFailure = this.ReconnectTransportOnFailure;
            settings.ReplayCacheSize = this.ReplayCacheSize;
            settings.ReplayWindow = this.ReplayWindow;
            settings.SessionKeyRenewalInterval = this.SessionKeyRenewalInterval;
            settings.SessionKeyRolloverInterval = this.SessionKeyRolloverInterval;
            settings.TimestampValidityDuration = this.TimestampValidityDuration;
            settings.CookieRenewalThresholdPercentage = this.CookieRenewalThresholdPercentage;
        }

        internal void CopyFrom(LocalClientSecuritySettingsElement source)
        {
            if (source == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("source");
            }
            this.CacheCookies = source.CacheCookies;
            if (source.ElementInformation.Properties["detectReplays"].ValueOrigin != PropertyValueOrigin.Default)
            {
                this.DetectReplays = source.DetectReplays;
            }
            this.MaxClockSkew = source.MaxClockSkew;
            this.MaxCookieCachingTime = source.MaxCookieCachingTime;
            this.ReconnectTransportOnFailure = source.ReconnectTransportOnFailure;
            this.ReplayCacheSize = source.ReplayCacheSize;
            this.ReplayWindow = source.ReplayWindow;
            this.SessionKeyRenewalInterval = source.SessionKeyRenewalInterval;
            this.SessionKeyRolloverInterval = source.SessionKeyRolloverInterval;
            this.TimestampValidityDuration = source.TimestampValidityDuration;
            this.CookieRenewalThresholdPercentage = source.CookieRenewalThresholdPercentage;
        }

        internal void InitializeFrom(LocalClientSecuritySettings settings)
        {
            if (settings == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("settings");
            }
            this.CacheCookies = settings.CacheCookies;
            this.DetectReplays = settings.DetectReplays;
            this.MaxClockSkew = settings.MaxClockSkew;
            this.MaxCookieCachingTime = settings.MaxCookieCachingTime;
            this.ReconnectTransportOnFailure = settings.ReconnectTransportOnFailure;
            this.ReplayCacheSize = settings.ReplayCacheSize;
            this.ReplayWindow = settings.ReplayWindow;
            this.SessionKeyRenewalInterval = settings.SessionKeyRenewalInterval;
            this.SessionKeyRolloverInterval = settings.SessionKeyRolloverInterval;
            this.TimestampValidityDuration = settings.TimestampValidityDuration;
            this.CookieRenewalThresholdPercentage = settings.CookieRenewalThresholdPercentage;
        }

        [ConfigurationProperty("cacheCookies", DefaultValue=true)]
        public bool CacheCookies
        {
            get => 
                ((bool) base["cacheCookies"]);
            set
            {
                base["cacheCookies"] = value;
            }
        }

        [ConfigurationProperty("cookieRenewalThresholdPercentage", DefaultValue=60), IntegerValidator(MinValue=0, MaxValue=100)]
        public int CookieRenewalThresholdPercentage
        {
            get => 
                ((int) base["cookieRenewalThresholdPercentage"]);
            set
            {
                base["cookieRenewalThresholdPercentage"] = value;
            }
        }

        [ConfigurationProperty("detectReplays", DefaultValue=true)]
        public bool DetectReplays
        {
            get => 
                ((bool) base["detectReplays"]);
            set
            {
                base["detectReplays"] = value;
            }
        }

        [ServiceModelTimeSpanValidator(MinValueString="00:00:00"), ConfigurationProperty("maxClockSkew", DefaultValue="00:05:00"), TypeConverter(typeof(TimeSpanOrInfiniteConverter))]
        public TimeSpan MaxClockSkew
        {
            get => 
                ((TimeSpan) base["maxClockSkew"]);
            set
            {
                base["maxClockSkew"] = value;
            }
        }

        [TypeConverter(typeof(TimeSpanOrInfiniteConverter)), ConfigurationProperty("maxCookieCachingTime", DefaultValue="10675199.02:48:05.4775807"), ServiceModelTimeSpanValidator(MinValueString="00:00:00")]
        public TimeSpan MaxCookieCachingTime
        {
            get => 
                ((TimeSpan) base["maxCookieCachingTime"]);
            set
            {
                base["maxCookieCachingTime"] = value;
            }
        }

        protected override ConfigurationPropertyCollection Properties
        {
            get
            {
                if (this.properties == null)
                {
                    ConfigurationPropertyCollection propertys = new ConfigurationPropertyCollection {
                        new ConfigurationProperty("cacheCookies", typeof(bool), true, null, null, ConfigurationPropertyOptions.None),
                        new ConfigurationProperty("detectReplays", typeof(bool), true, null, null, ConfigurationPropertyOptions.None),
                        new ConfigurationProperty("replayCacheSize", typeof(int), 0xdbba0, null, new IntegerValidator(1, 0x7fffffff, false), ConfigurationPropertyOptions.None),
                        new ConfigurationProperty("maxClockSkew", typeof(TimeSpan), TimeSpan.Parse("00:05:00"), new TimeSpanOrInfiniteConverter(), new TimeSpanOrInfiniteValidator(TimeSpan.Parse("00:00:00"), TimeSpan.Parse("24.20:31:23.6470000")), ConfigurationPropertyOptions.None),
                        new ConfigurationProperty("maxCookieCachingTime", typeof(TimeSpan), TimeSpan.Parse("10675199.02:48:05.4775807"), new TimeSpanOrInfiniteConverter(), new TimeSpanOrInfiniteValidator(TimeSpan.Parse("00:00:00"), TimeSpan.Parse("24.20:31:23.6470000")), ConfigurationPropertyOptions.None),
                        new ConfigurationProperty("replayWindow", typeof(TimeSpan), TimeSpan.Parse("00:05:00"), new TimeSpanOrInfiniteConverter(), new TimeSpanOrInfiniteValidator(TimeSpan.Parse("00:00:00"), TimeSpan.Parse("24.20:31:23.6470000")), ConfigurationPropertyOptions.None),
                        new ConfigurationProperty("sessionKeyRenewalInterval", typeof(TimeSpan), TimeSpan.Parse("10:00:00"), new TimeSpanOrInfiniteConverter(), new TimeSpanOrInfiniteValidator(TimeSpan.Parse("00:00:00"), TimeSpan.Parse("24.20:31:23.6470000")), ConfigurationPropertyOptions.None),
                        new ConfigurationProperty("sessionKeyRolloverInterval", typeof(TimeSpan), TimeSpan.Parse("00:05:00"), new TimeSpanOrInfiniteConverter(), new TimeSpanOrInfiniteValidator(TimeSpan.Parse("00:00:00"), TimeSpan.Parse("24.20:31:23.6470000")), ConfigurationPropertyOptions.None),
                        new ConfigurationProperty("reconnectTransportOnFailure", typeof(bool), true, null, null, ConfigurationPropertyOptions.None),
                        new ConfigurationProperty("timestampValidityDuration", typeof(TimeSpan), TimeSpan.Parse("00:05:00"), new TimeSpanOrInfiniteConverter(), new TimeSpanOrInfiniteValidator(TimeSpan.Parse("00:00:00"), TimeSpan.Parse("24.20:31:23.6470000")), ConfigurationPropertyOptions.None),
                        new ConfigurationProperty("cookieRenewalThresholdPercentage", typeof(int), 60, null, new IntegerValidator(0, 100, false), ConfigurationPropertyOptions.None)
                    };
                    this.properties = propertys;
                }
                return this.properties;
            }
        }

        [ConfigurationProperty("reconnectTransportOnFailure", DefaultValue=true)]
        public bool ReconnectTransportOnFailure
        {
            get => 
                ((bool) base["reconnectTransportOnFailure"]);
            set
            {
                base["reconnectTransportOnFailure"] = value;
            }
        }

        [IntegerValidator(MinValue=1), ConfigurationProperty("replayCacheSize", DefaultValue=0xdbba0)]
        public int ReplayCacheSize
        {
            get => 
                ((int) base["replayCacheSize"]);
            set
            {
                base["replayCacheSize"] = value;
            }
        }

        [ConfigurationProperty("replayWindow", DefaultValue="00:05:00"), TypeConverter(typeof(TimeSpanOrInfiniteConverter)), ServiceModelTimeSpanValidator(MinValueString="00:00:00")]
        public TimeSpan ReplayWindow
        {
            get => 
                ((TimeSpan) base["replayWindow"]);
            set
            {
                base["replayWindow"] = value;
            }
        }

        [ServiceModelTimeSpanValidator(MinValueString="00:00:00"), ConfigurationProperty("sessionKeyRenewalInterval", DefaultValue="10:00:00"), TypeConverter(typeof(TimeSpanOrInfiniteConverter))]
        public TimeSpan SessionKeyRenewalInterval
        {
            get => 
                ((TimeSpan) base["sessionKeyRenewalInterval"]);
            set
            {
                base["sessionKeyRenewalInterval"] = value;
            }
        }

        [ConfigurationProperty("sessionKeyRolloverInterval", DefaultValue="00:05:00"), TypeConverter(typeof(TimeSpanOrInfiniteConverter)), ServiceModelTimeSpanValidator(MinValueString="00:00:00")]
        public TimeSpan SessionKeyRolloverInterval
        {
            get => 
                ((TimeSpan) base["sessionKeyRolloverInterval"]);
            set
            {
                base["sessionKeyRolloverInterval"] = value;
            }
        }

        [ConfigurationProperty("timestampValidityDuration", DefaultValue="00:05:00"), ServiceModelTimeSpanValidator(MinValueString="00:00:00"), TypeConverter(typeof(TimeSpanOrInfiniteConverter))]
        public TimeSpan TimestampValidityDuration
        {
            get => 
                ((TimeSpan) base["timestampValidityDuration"]);
            set
            {
                base["timestampValidityDuration"] = value;
            }
        }
    }
}

