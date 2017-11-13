namespace System.ServiceModel.Configuration
{
    using System;
    using System.Configuration;
    using System.Security;
    using System.ServiceModel.Diagnostics;

    public sealed class DiagnosticSection : ConfigurationSection
    {
        private ConfigurationPropertyCollection properties;

        internal static DiagnosticSection GetSection() => 
            ((DiagnosticSection) ConfigurationHelpers.GetSection(ConfigurationStrings.DiagnosticSectionPath));

        [SecurityCritical]
        internal static DiagnosticSection UnsafeGetSection() => 
            ((DiagnosticSection) ConfigurationHelpers.UnsafeGetSection(ConfigurationStrings.DiagnosticSectionPath));

        [ConfigurationProperty("messageLogging", Options=ConfigurationPropertyOptions.None)]
        public MessageLoggingElement MessageLogging =>
            ((MessageLoggingElement) base["messageLogging"]);

        [ConfigurationProperty("performanceCounters", DefaultValue=3), ServiceModelEnumValidator(typeof(PerformanceCounterScopeHelper))]
        public PerformanceCounterScope PerformanceCounters
        {
            get => 
                ((PerformanceCounterScope) base["performanceCounters"]);
            set
            {
                base["performanceCounters"] = value;
            }
        }

        protected override ConfigurationPropertyCollection Properties
        {
            get
            {
                if (this.properties == null)
                {
                    ConfigurationPropertyCollection propertys = new ConfigurationPropertyCollection {
                        new ConfigurationProperty("wmiProviderEnabled", typeof(bool), false, null, null, ConfigurationPropertyOptions.None),
                        new ConfigurationProperty("messageLogging", typeof(MessageLoggingElement), null, null, null, ConfigurationPropertyOptions.None),
                        new ConfigurationProperty("performanceCounters", typeof(PerformanceCounterScope), PerformanceCounterScope.Default, null, new ServiceModelEnumValidator(typeof(PerformanceCounterScopeHelper)), ConfigurationPropertyOptions.None)
                    };
                    this.properties = propertys;
                }
                return this.properties;
            }
        }

        [ConfigurationProperty("wmiProviderEnabled", DefaultValue=false)]
        public bool WmiProviderEnabled
        {
            get => 
                ((bool) base["wmiProviderEnabled"]);
            set
            {
                base["wmiProviderEnabled"] = value;
            }
        }
    }
}

