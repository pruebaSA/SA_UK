namespace System.Web.Configuration
{
    using System;
    using System.ComponentModel;
    using System.Configuration;
    using System.Security.Permissions;
    using System.Web;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public sealed class HostingEnvironmentSection : ConfigurationSection
    {
        private static ConfigurationPropertyCollection _properties = new ConfigurationPropertyCollection();
        private static readonly ConfigurationProperty _propIdleTimeout = new ConfigurationProperty("idleTimeout", typeof(TimeSpan), DefaultIdleTimeout, StdValidatorsAndConverters.TimeSpanMinutesOrInfiniteConverter, StdValidatorsAndConverters.PositiveTimeSpanValidator, ConfigurationPropertyOptions.None);
        private static readonly ConfigurationProperty _propShadowCopyBinAssemblies = new ConfigurationProperty("shadowCopyBinAssemblies", typeof(bool), true, ConfigurationPropertyOptions.None);
        private static readonly ConfigurationProperty _propShutdownTimeout = new ConfigurationProperty("shutdownTimeout", typeof(TimeSpan), TimeSpan.FromSeconds(30.0), StdValidatorsAndConverters.TimeSpanSecondsConverter, StdValidatorsAndConverters.PositiveTimeSpanValidator, ConfigurationPropertyOptions.None);
        internal static readonly TimeSpan DefaultIdleTimeout = TimeSpan.MaxValue;
        internal const int DefaultShutdownTimeout = 30;
        internal const string sectionName = "system.web/hostingEnvironment";

        static HostingEnvironmentSection()
        {
            _properties.Add(_propIdleTimeout);
            _properties.Add(_propShutdownTimeout);
            _properties.Add(_propShadowCopyBinAssemblies);
        }

        [ConfigurationProperty("idleTimeout", DefaultValue="10675199.02:48:05.4775807"), TimeSpanValidator(MinValueString="00:00:00", MaxValueString="10675199.02:48:05.4775807"), TypeConverter(typeof(TimeSpanMinutesOrInfiniteConverter))]
        public TimeSpan IdleTimeout
        {
            get => 
                ((TimeSpan) base[_propIdleTimeout]);
            set
            {
                base[_propIdleTimeout] = value;
            }
        }

        protected override ConfigurationPropertyCollection Properties =>
            _properties;

        [ConfigurationProperty("shadowCopyBinAssemblies", DefaultValue=true)]
        public bool ShadowCopyBinAssemblies
        {
            get => 
                ((bool) base[_propShadowCopyBinAssemblies]);
            set
            {
                base[_propShadowCopyBinAssemblies] = value;
            }
        }

        [TypeConverter(typeof(TimeSpanSecondsConverter)), ConfigurationProperty("shutdownTimeout", DefaultValue="00:00:30"), TimeSpanValidator(MinValueString="00:00:00", MaxValueString="10675199.02:48:05.4775807")]
        public TimeSpan ShutdownTimeout
        {
            get => 
                ((TimeSpan) base[_propShutdownTimeout]);
            set
            {
                base[_propShutdownTimeout] = value;
            }
        }
    }
}

