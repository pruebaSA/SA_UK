namespace System.ServiceModel.Configuration
{
    using System;
    using System.ComponentModel;
    using System.Configuration;
    using System.ServiceModel;
    using System.ServiceModel.Channels;

    public sealed class LocalServiceSecuritySettingsElement : ConfigurationElement
    {
        private ConfigurationPropertyCollection properties;

        internal void ApplyConfiguration(LocalServiceSecuritySettings settings)
        {
            if (settings == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("settings");
            }
            if (base.ElementInformation.Properties["detectReplays"].ValueOrigin != PropertyValueOrigin.Default)
            {
                settings.DetectReplays = this.DetectReplays;
            }
            settings.IssuedCookieLifetime = this.IssuedCookieLifetime;
            settings.MaxClockSkew = this.MaxClockSkew;
            settings.MaxPendingSessions = this.MaxPendingSessions;
            settings.MaxStatefulNegotiations = this.MaxStatefulNegotiations;
            settings.NegotiationTimeout = this.NegotiationTimeout;
            settings.ReconnectTransportOnFailure = this.ReconnectTransportOnFailure;
            settings.ReplayCacheSize = this.ReplayCacheSize;
            settings.ReplayWindow = this.ReplayWindow;
            settings.SessionKeyRenewalInterval = this.SessionKeyRenewalInterval;
            settings.SessionKeyRolloverInterval = this.SessionKeyRolloverInterval;
            settings.InactivityTimeout = this.InactivityTimeout;
            settings.TimestampValidityDuration = this.TimestampValidityDuration;
            settings.MaxCachedCookies = this.MaxCachedCookies;
        }

        internal void CopyFrom(LocalServiceSecuritySettingsElement source)
        {
            if (source == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("source");
            }
            if (source.ElementInformation.Properties["detectReplays"].ValueOrigin != PropertyValueOrigin.Default)
            {
                this.DetectReplays = source.DetectReplays;
            }
            this.IssuedCookieLifetime = source.IssuedCookieLifetime;
            this.MaxClockSkew = source.MaxClockSkew;
            this.MaxPendingSessions = source.MaxPendingSessions;
            this.MaxStatefulNegotiations = source.MaxStatefulNegotiations;
            this.NegotiationTimeout = source.NegotiationTimeout;
            this.ReconnectTransportOnFailure = source.ReconnectTransportOnFailure;
            this.ReplayCacheSize = source.ReplayCacheSize;
            this.ReplayWindow = source.ReplayWindow;
            this.SessionKeyRenewalInterval = source.SessionKeyRenewalInterval;
            this.SessionKeyRolloverInterval = source.SessionKeyRolloverInterval;
            this.InactivityTimeout = source.InactivityTimeout;
            this.TimestampValidityDuration = source.TimestampValidityDuration;
            this.MaxCachedCookies = source.MaxCachedCookies;
        }

        internal void InitializeFrom(LocalServiceSecuritySettings settings)
        {
            if (settings == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("settings");
            }
            this.DetectReplays = settings.DetectReplays;
            this.IssuedCookieLifetime = settings.IssuedCookieLifetime;
            this.MaxClockSkew = settings.MaxClockSkew;
            this.MaxPendingSessions = settings.MaxPendingSessions;
            this.MaxStatefulNegotiations = settings.MaxStatefulNegotiations;
            this.NegotiationTimeout = settings.NegotiationTimeout;
            this.ReconnectTransportOnFailure = settings.ReconnectTransportOnFailure;
            this.ReplayCacheSize = settings.ReplayCacheSize;
            this.ReplayWindow = settings.ReplayWindow;
            this.SessionKeyRenewalInterval = settings.SessionKeyRenewalInterval;
            this.SessionKeyRolloverInterval = settings.SessionKeyRolloverInterval;
            this.InactivityTimeout = settings.InactivityTimeout;
            this.TimestampValidityDuration = settings.TimestampValidityDuration;
            this.MaxCachedCookies = settings.MaxCachedCookies;
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

        [ConfigurationProperty("inactivityTimeout", DefaultValue="00:02:00"), TypeConverter(typeof(TimeSpanOrInfiniteConverter)), ServiceModelTimeSpanValidator(MinValueString="00:00:00")]
        public TimeSpan InactivityTimeout
        {
            get => 
                ((TimeSpan) base["inactivityTimeout"]);
            set
            {
                base["inactivityTimeout"] = value;
            }
        }

        [ServiceModelTimeSpanValidator(MinValueString="00:00:00"), ConfigurationProperty("issuedCookieLifetime", DefaultValue="10:00:00"), TypeConverter(typeof(TimeSpanOrInfiniteConverter))]
        public TimeSpan IssuedCookieLifetime
        {
            get => 
                ((TimeSpan) base["issuedCookieLifetime"]);
            set
            {
                base["issuedCookieLifetime"] = value;
            }
        }

        [IntegerValidator(MinValue=0), ConfigurationProperty("maxCachedCookies", DefaultValue=0x3e8)]
        public int MaxCachedCookies
        {
            get => 
                ((int) base["maxCachedCookies"]);
            set
            {
                base["maxCachedCookies"] = value;
            }
        }

        [ConfigurationProperty("maxClockSkew", DefaultValue="00:05:00"), TypeConverter(typeof(TimeSpanOrInfiniteConverter)), ServiceModelTimeSpanValidator(MinValueString="00:00:00")]
        public TimeSpan MaxClockSkew
        {
            get => 
                ((TimeSpan) base["maxClockSkew"]);
            set
            {
                base["maxClockSkew"] = value;
            }
        }

        [IntegerValidator(MinValue=1), ConfigurationProperty("maxPendingSessions", DefaultValue=0x80)]
        public int MaxPendingSessions
        {
            get => 
                ((int) base["maxPendingSessions"]);
            set
            {
                base["maxPendingSessions"] = value;
            }
        }

        [IntegerValidator(MinValue=0), ConfigurationProperty("maxStatefulNegotiations", DefaultValue=0x80)]
        public int MaxStatefulNegotiations
        {
            get => 
                ((int) base["maxStatefulNegotiations"]);
            set
            {
                base["maxStatefulNegotiations"] = value;
            }
        }

        [ConfigurationProperty("negotiationTimeout", DefaultValue="00:01:00"), ServiceModelTimeSpanValidator(MinValueString="00:00:00"), TypeConverter(typeof(TimeSpanOrInfiniteConverter))]
        public TimeSpan NegotiationTimeout
        {
            get => 
                ((TimeSpan) base["negotiationTimeout"]);
            set
            {
                base["negotiationTimeout"] = value;
            }
        }

        protected override ConfigurationPropertyCollection Properties
        {
            get
            {
                if (this.properties == null)
                {
                    ConfigurationPropertyCollection propertys = new ConfigurationPropertyCollection {
                        new ConfigurationProperty("detectReplays", typeof(bool), true, null, null, ConfigurationPropertyOptions.None),
                        new ConfigurationProperty("issuedCookieLifetime", typeof(TimeSpan), TimeSpan.Parse("10:00:00"), new TimeSpanOrInfiniteConverter(), new TimeSpanOrInfiniteValidator(TimeSpan.Parse("00:00:00"), TimeSpan.Parse("24.20:31:23.6470000")), ConfigurationPropertyOptions.None),
                        new ConfigurationProperty("maxStatefulNegotiations", typeof(int), 0x80, null, new IntegerValidator(0, 0x7fffffff, false), ConfigurationPropertyOptions.None),
                        new ConfigurationProperty("replayCacheSize", typeof(int), 0xdbba0, null, new IntegerValidator(1, 0x7fffffff, false), ConfigurationPropertyOptions.None),
                        new ConfigurationProperty("maxClockSkew", typeof(TimeSpan), TimeSpan.Parse("00:05:00"), new TimeSpanOrInfiniteConverter(), new TimeSpanOrInfiniteValidator(TimeSpan.Parse("00:00:00"), TimeSpan.Parse("24.20:31:23.6470000")), ConfigurationPropertyOptions.None),
                        new ConfigurationProperty("negotiationTimeout", typeof(TimeSpan), TimeSpan.Parse("00:01:00"), new TimeSpanOrInfiniteConverter(), new TimeSpanOrInfiniteValidator(TimeSpan.Parse("00:00:00"), TimeSpan.Parse("24.20:31:23.6470000")), ConfigurationPropertyOptions.None),
                        new ConfigurationProperty("replayWindow", typeof(TimeSpan), TimeSpan.Parse("00:05:00"), new TimeSpanOrInfiniteConverter(), new TimeSpanOrInfiniteValidator(TimeSpan.Parse("00:00:00"), TimeSpan.Parse("24.20:31:23.6470000")), ConfigurationPropertyOptions.None),
                        new ConfigurationProperty("inactivityTimeout", typeof(TimeSpan), TimeSpan.Parse("00:02:00"), new TimeSpanOrInfiniteConverter(), new TimeSpanOrInfiniteValidator(TimeSpan.Parse("00:00:00"), TimeSpan.Parse("24.20:31:23.6470000")), ConfigurationPropertyOptions.None),
                        new ConfigurationProperty("sessionKeyRenewalInterval", typeof(TimeSpan), TimeSpan.Parse("15:00:00"), new TimeSpanOrInfiniteConverter(), new TimeSpanOrInfiniteValidator(TimeSpan.Parse("00:00:00"), TimeSpan.Parse("24.20:31:23.6470000")), ConfigurationPropertyOptions.None),
                        new ConfigurationProperty("sessionKeyRolloverInterval", typeof(TimeSpan), TimeSpan.Parse("00:05:00"), new TimeSpanOrInfiniteConverter(), new TimeSpanOrInfiniteValidator(TimeSpan.Parse("00:00:00"), TimeSpan.Parse("24.20:31:23.6470000")), ConfigurationPropertyOptions.None),
                        new ConfigurationProperty("reconnectTransportOnFailure", typeof(bool), true, null, null, ConfigurationPropertyOptions.None),
                        new ConfigurationProperty("maxPendingSessions", typeof(int), 0x80, null, new IntegerValidator(1, 0x7fffffff, false), ConfigurationPropertyOptions.None),
                        new ConfigurationProperty("maxCachedCookies", typeof(int), 0x3e8, null, new IntegerValidator(0, 0x7fffffff, false), ConfigurationPropertyOptions.None),
                        new ConfigurationProperty("timestampValidityDuration", typeof(TimeSpan), TimeSpan.Parse("00:05:00"), new TimeSpanOrInfiniteConverter(), new TimeSpanOrInfiniteValidator(TimeSpan.Parse("00:00:00"), TimeSpan.Parse("24.20:31:23.6470000")), ConfigurationPropertyOptions.None)
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

        [ConfigurationProperty("replayCacheSize", DefaultValue=0xdbba0), IntegerValidator(MinValue=1)]
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

        [ServiceModelTimeSpanValidator(MinValueString="00:00:00"), TypeConverter(typeof(TimeSpanOrInfiniteConverter)), ConfigurationProperty("sessionKeyRenewalInterval", DefaultValue="15:00:00")]
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

        [ServiceModelTimeSpanValidator(MinValueString="00:00:00"), ConfigurationProperty("timestampValidityDuration", DefaultValue="00:05:00"), TypeConverter(typeof(TimeSpanOrInfiniteConverter))]
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

