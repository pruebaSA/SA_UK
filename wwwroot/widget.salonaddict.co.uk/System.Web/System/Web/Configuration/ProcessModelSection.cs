namespace System.Web.Configuration
{
    using System;
    using System.ComponentModel;
    using System.Configuration;
    using System.Security.Permissions;
    using System.Web;
    using System.Web.Util;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public sealed class ProcessModelSection : ConfigurationSection
    {
        private static readonly ConfigurationProperty _propAutoConfig = new ConfigurationProperty("autoConfig", typeof(bool), false, ConfigurationPropertyOptions.None);
        private static readonly ConfigurationProperty _propClientConnectedCheck = new ConfigurationProperty("clientConnectedCheck", typeof(TimeSpan), DefaultClientConnectedCheck, StdValidatorsAndConverters.InfiniteTimeSpanConverter, null, ConfigurationPropertyOptions.None);
        private static readonly ConfigurationProperty _propComAuthenticationLevel = new ConfigurationProperty("comAuthenticationLevel", typeof(ProcessModelComAuthenticationLevel), ProcessModelComAuthenticationLevel.Connect, ConfigurationPropertyOptions.None);
        private static readonly ConfigurationProperty _propComImpersonationLevel = new ConfigurationProperty("comImpersonationLevel", typeof(ProcessModelComImpersonationLevel), ProcessModelComImpersonationLevel.Impersonate, ConfigurationPropertyOptions.None);
        private static readonly ConfigurationProperty _propCpuMask = new ConfigurationProperty("cpuMask", typeof(string), "0xffffffff", ConfigurationPropertyOptions.None);
        private static readonly ConfigurationProperty _propEnable = new ConfigurationProperty("enable", typeof(bool), true, ConfigurationPropertyOptions.None);
        private static ConfigurationPropertyCollection _properties = new ConfigurationPropertyCollection();
        private static readonly ConfigurationProperty _propIdleTimeout = new ConfigurationProperty("idleTimeout", typeof(TimeSpan), TimeSpan.MaxValue, StdValidatorsAndConverters.InfiniteTimeSpanConverter, null, ConfigurationPropertyOptions.None);
        private static readonly ConfigurationProperty _propLogLevel = new ConfigurationProperty("logLevel", typeof(ProcessModelLogLevel), ProcessModelLogLevel.Errors, ConfigurationPropertyOptions.None);
        private static readonly ConfigurationProperty _propMaxAppDomains = new ConfigurationProperty("maxAppDomains", typeof(int), 0x7d0, null, new IntegerValidator(1, 0x7ffffffe), ConfigurationPropertyOptions.None);
        private static readonly ConfigurationProperty _propMaxIOThreads = new ConfigurationProperty("maxIoThreads", typeof(int), 20, null, new IntegerValidator(1, 0x7ffffffe), ConfigurationPropertyOptions.None);
        private static readonly ConfigurationProperty _propMaxWorkerThreads = new ConfigurationProperty("maxWorkerThreads", typeof(int), 20, null, new IntegerValidator(1, 0x7ffffffe), ConfigurationPropertyOptions.None);
        private static readonly ConfigurationProperty _propMemoryLimit = new ConfigurationProperty("memoryLimit", typeof(int), 60, ConfigurationPropertyOptions.None);
        private static readonly ConfigurationProperty _propMinIOThreads = new ConfigurationProperty("minIoThreads", typeof(int), 1, null, new IntegerValidator(1, 0x7ffffffe), ConfigurationPropertyOptions.None);
        private static readonly ConfigurationProperty _propMinWorkerThreads = new ConfigurationProperty("minWorkerThreads", typeof(int), 1, null, new IntegerValidator(1, 0x7ffffffe), ConfigurationPropertyOptions.None);
        private static readonly ConfigurationProperty _propPassword = new ConfigurationProperty("password", typeof(string), "AutoGenerate", ConfigurationPropertyOptions.None);
        private static readonly ConfigurationProperty _propPingFrequency = new ConfigurationProperty("pingFrequency", typeof(TimeSpan), TimeSpan.MaxValue, StdValidatorsAndConverters.InfiniteTimeSpanConverter, null, ConfigurationPropertyOptions.None);
        private static readonly ConfigurationProperty _propPingTimeout = new ConfigurationProperty("pingTimeout", typeof(TimeSpan), TimeSpan.MaxValue, StdValidatorsAndConverters.InfiniteTimeSpanConverter, null, ConfigurationPropertyOptions.None);
        private static readonly ConfigurationProperty _propRequestLimit = new ConfigurationProperty("requestLimit", typeof(int), 0x7fffffff, new InfiniteIntConverter(), StdValidatorsAndConverters.PositiveIntegerValidator, ConfigurationPropertyOptions.None);
        private static readonly ConfigurationProperty _propRequestQueueLimit = new ConfigurationProperty("requestQueueLimit", typeof(int), 0x1388, new InfiniteIntConverter(), StdValidatorsAndConverters.PositiveIntegerValidator, ConfigurationPropertyOptions.None);
        private static readonly ConfigurationProperty _propResponseDeadlockInterval = new ConfigurationProperty("responseDeadlockInterval", typeof(TimeSpan), TimeSpan.FromMinutes(3.0), StdValidatorsAndConverters.InfiniteTimeSpanConverter, StdValidatorsAndConverters.PositiveTimeSpanValidator, ConfigurationPropertyOptions.None);
        private static readonly ConfigurationProperty _propResponseRestartDeadlockInterval = new ConfigurationProperty("responseRestartDeadlockInterval", typeof(TimeSpan), TimeSpan.FromMinutes(3.0), StdValidatorsAndConverters.InfiniteTimeSpanConverter, null, ConfigurationPropertyOptions.None);
        private static readonly ConfigurationProperty _propRestartQueueLimit = new ConfigurationProperty("restartQueueLimit", typeof(int), 10, new InfiniteIntConverter(), StdValidatorsAndConverters.PositiveIntegerValidator, ConfigurationPropertyOptions.None);
        private static readonly ConfigurationProperty _propServerErrorMessageFile = new ConfigurationProperty("serverErrorMessageFile", typeof(string), string.Empty, ConfigurationPropertyOptions.None);
        private static readonly ConfigurationProperty _propShutdownTimeout = new ConfigurationProperty("shutdownTimeout", typeof(TimeSpan), TimeSpan.FromSeconds(5.0), StdValidatorsAndConverters.InfiniteTimeSpanConverter, StdValidatorsAndConverters.PositiveTimeSpanValidator, ConfigurationPropertyOptions.None);
        private static readonly ConfigurationProperty _propTimeout = new ConfigurationProperty("timeout", typeof(TimeSpan), TimeSpan.MaxValue, StdValidatorsAndConverters.InfiniteTimeSpanConverter, null, ConfigurationPropertyOptions.None);
        private static readonly ConfigurationProperty _propUserName = new ConfigurationProperty("userName", typeof(string), "machine", ConfigurationPropertyOptions.None);
        private static readonly ConfigurationProperty _propWebGarden = new ConfigurationProperty("webGarden", typeof(bool), false, ConfigurationPropertyOptions.None);
        private static int cpuCount;
        internal static TimeSpan DefaultClientConnectedCheck = new TimeSpan(0, 0, 5);
        private static readonly ConfigurationElementProperty s_elemProperty = new ConfigurationElementProperty(new CallbackValidator(typeof(ProcessModelSection), new ValidatorCallback(ProcessModelSection.Validate)));
        internal const string sectionName = "system.web/processModel";

        static ProcessModelSection()
        {
            _properties.Add(_propEnable);
            _properties.Add(_propTimeout);
            _properties.Add(_propIdleTimeout);
            _properties.Add(_propShutdownTimeout);
            _properties.Add(_propRequestLimit);
            _properties.Add(_propRequestQueueLimit);
            _properties.Add(_propRestartQueueLimit);
            _properties.Add(_propMemoryLimit);
            _properties.Add(_propWebGarden);
            _properties.Add(_propCpuMask);
            _properties.Add(_propUserName);
            _properties.Add(_propPassword);
            _properties.Add(_propLogLevel);
            _properties.Add(_propClientConnectedCheck);
            _properties.Add(_propComAuthenticationLevel);
            _properties.Add(_propComImpersonationLevel);
            _properties.Add(_propResponseDeadlockInterval);
            _properties.Add(_propResponseRestartDeadlockInterval);
            _properties.Add(_propAutoConfig);
            _properties.Add(_propMaxWorkerThreads);
            _properties.Add(_propMaxIOThreads);
            _properties.Add(_propMinWorkerThreads);
            _properties.Add(_propMinIOThreads);
            _properties.Add(_propServerErrorMessageFile);
            _properties.Add(_propPingFrequency);
            _properties.Add(_propPingTimeout);
            _properties.Add(_propMaxAppDomains);
            cpuCount = SystemInfo.GetNumProcessCPUs();
        }

        private static void Validate(object value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            ProcessModelSection section = (ProcessModelSection) value;
            int cpuMask = -1;
            try
            {
                cpuMask = section.CpuMask;
            }
            catch
            {
                throw new ConfigurationErrorsException(System.Web.SR.GetString("Invalid_non_zero_hexadecimal_attribute", new object[] { "cpuMask" }), section.ElementInformation.Properties["cpuMask"].Source, section.ElementInformation.Properties["cpuMask"].LineNumber);
            }
            if (cpuMask == 0)
            {
                throw new ConfigurationErrorsException(System.Web.SR.GetString("Invalid_non_zero_hexadecimal_attribute", new object[] { "cpuMask" }), section.ElementInformation.Properties["cpuMask"].Source, section.ElementInformation.Properties["cpuMask"].LineNumber);
            }
        }

        [ConfigurationProperty("autoConfig", DefaultValue=false)]
        public bool AutoConfig
        {
            get => 
                ((bool) base[_propAutoConfig]);
            set
            {
                base[_propAutoConfig] = value;
            }
        }

        [ConfigurationProperty("clientConnectedCheck", DefaultValue="00:00:05"), TypeConverter(typeof(InfiniteTimeSpanConverter))]
        public TimeSpan ClientConnectedCheck
        {
            get => 
                ((TimeSpan) base[_propClientConnectedCheck]);
            set
            {
                base[_propClientConnectedCheck] = value;
            }
        }

        [ConfigurationProperty("comAuthenticationLevel", DefaultValue=2)]
        public ProcessModelComAuthenticationLevel ComAuthenticationLevel
        {
            get => 
                ((ProcessModelComAuthenticationLevel) base[_propComAuthenticationLevel]);
            set
            {
                base[_propComAuthenticationLevel] = value;
            }
        }

        [ConfigurationProperty("comImpersonationLevel", DefaultValue=4)]
        public ProcessModelComImpersonationLevel ComImpersonationLevel
        {
            get => 
                ((ProcessModelComImpersonationLevel) base[_propComImpersonationLevel]);
            set
            {
                base[_propComImpersonationLevel] = value;
            }
        }

        internal int CpuCount =>
            cpuCount;

        [ConfigurationProperty("cpuMask", DefaultValue="0xffffffff")]
        public int CpuMask
        {
            get => 
                Convert.ToInt32((string) base[_propCpuMask], 0x10);
            set
            {
                base[_propCpuMask] = "0x" + Convert.ToString(value, 0x10);
            }
        }

        internal int DefaultMaxIoThreadsForAutoConfig =>
            (100 * cpuCount);

        internal int DefaultMaxWorkerThreadsForAutoConfig =>
            (100 * cpuCount);

        protected override ConfigurationElementProperty ElementProperty =>
            s_elemProperty;

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

        [ConfigurationProperty("idleTimeout", DefaultValue="10675199.02:48:05.4775807"), TypeConverter(typeof(InfiniteTimeSpanConverter))]
        public TimeSpan IdleTimeout
        {
            get => 
                ((TimeSpan) base[_propIdleTimeout]);
            set
            {
                base[_propIdleTimeout] = value;
            }
        }

        [ConfigurationProperty("logLevel", DefaultValue=2)]
        public ProcessModelLogLevel LogLevel
        {
            get => 
                ((ProcessModelLogLevel) base[_propLogLevel]);
            set
            {
                base[_propLogLevel] = value;
            }
        }

        [ConfigurationProperty("maxAppDomains", DefaultValue=0x7d0), IntegerValidator(MinValue=1, MaxValue=0x7ffffffe)]
        public int MaxAppDomains
        {
            get => 
                ((int) base[_propMaxAppDomains]);
            set
            {
                base[_propMaxAppDomains] = value;
            }
        }

        [ConfigurationProperty("maxIoThreads", DefaultValue=20), IntegerValidator(MinValue=1, MaxValue=0x7ffffffe)]
        public int MaxIOThreads
        {
            get => 
                ((int) base[_propMaxIOThreads]);
            set
            {
                base[_propMaxIOThreads] = value;
            }
        }

        internal int MaxIoThreadsTimesCpuCount =>
            (this.MaxIOThreads * cpuCount);

        [ConfigurationProperty("maxWorkerThreads", DefaultValue=20), IntegerValidator(MinValue=1, MaxValue=0x7ffffffe)]
        public int MaxWorkerThreads
        {
            get => 
                ((int) base[_propMaxWorkerThreads]);
            set
            {
                base[_propMaxWorkerThreads] = value;
            }
        }

        internal int MaxWorkerThreadsTimesCpuCount =>
            (this.MaxWorkerThreads * cpuCount);

        [ConfigurationProperty("memoryLimit", DefaultValue=60)]
        public int MemoryLimit
        {
            get => 
                ((int) base[_propMemoryLimit]);
            set
            {
                base[_propMemoryLimit] = value;
            }
        }

        [IntegerValidator(MinValue=1, MaxValue=0x7ffffffe), ConfigurationProperty("minIoThreads", DefaultValue=1)]
        public int MinIOThreads
        {
            get => 
                ((int) base[_propMinIOThreads]);
            set
            {
                base[_propMinIOThreads] = value;
            }
        }

        internal int MinIoThreadsTimesCpuCount =>
            (this.MinIOThreads * cpuCount);

        [IntegerValidator(MinValue=1, MaxValue=0x7ffffffe), ConfigurationProperty("minWorkerThreads", DefaultValue=1)]
        public int MinWorkerThreads
        {
            get => 
                ((int) base[_propMinWorkerThreads]);
            set
            {
                base[_propMinWorkerThreads] = value;
            }
        }

        internal int MinWorkerThreadsTimesCpuCount =>
            (this.MinWorkerThreads * cpuCount);

        [ConfigurationProperty("password", DefaultValue="AutoGenerate")]
        public string Password
        {
            get => 
                ((string) base[_propPassword]);
            set
            {
                base[_propPassword] = value;
            }
        }

        [TypeConverter(typeof(InfiniteTimeSpanConverter)), ConfigurationProperty("pingFrequency", DefaultValue="10675199.02:48:05.4775807")]
        public TimeSpan PingFrequency
        {
            get => 
                ((TimeSpan) base[_propPingFrequency]);
            set
            {
                base[_propPingFrequency] = value;
            }
        }

        [ConfigurationProperty("pingTimeout", DefaultValue="10675199.02:48:05.4775807"), TypeConverter(typeof(InfiniteTimeSpanConverter))]
        public TimeSpan PingTimeout
        {
            get => 
                ((TimeSpan) base[_propPingTimeout]);
            set
            {
                base[_propPingTimeout] = value;
            }
        }

        protected override ConfigurationPropertyCollection Properties =>
            _properties;

        [ConfigurationProperty("requestLimit", DefaultValue=0x7fffffff), TypeConverter(typeof(InfiniteIntConverter)), IntegerValidator(MinValue=0)]
        public int RequestLimit
        {
            get => 
                ((int) base[_propRequestLimit]);
            set
            {
                base[_propRequestLimit] = value;
            }
        }

        [TypeConverter(typeof(InfiniteIntConverter)), ConfigurationProperty("requestQueueLimit", DefaultValue=0x1388), IntegerValidator(MinValue=0)]
        public int RequestQueueLimit
        {
            get => 
                ((int) base[_propRequestQueueLimit]);
            set
            {
                base[_propRequestQueueLimit] = value;
            }
        }

        [TimeSpanValidator(MinValueString="00:00:00", MaxValueString="10675199.02:48:05.4775807"), TypeConverter(typeof(InfiniteTimeSpanConverter)), ConfigurationProperty("responseDeadlockInterval", DefaultValue="00:03:00")]
        public TimeSpan ResponseDeadlockInterval
        {
            get => 
                ((TimeSpan) base[_propResponseDeadlockInterval]);
            set
            {
                base[_propResponseDeadlockInterval] = value;
            }
        }

        [TypeConverter(typeof(InfiniteTimeSpanConverter)), ConfigurationProperty("responseRestartDeadlockInterval", DefaultValue="00:03:00")]
        public TimeSpan ResponseRestartDeadlockInterval
        {
            get => 
                ((TimeSpan) base[_propResponseRestartDeadlockInterval]);
            set
            {
                base[_propResponseRestartDeadlockInterval] = value;
            }
        }

        [IntegerValidator(MinValue=0), ConfigurationProperty("restartQueueLimit", DefaultValue=10), TypeConverter(typeof(InfiniteIntConverter))]
        public int RestartQueueLimit
        {
            get => 
                ((int) base[_propRestartQueueLimit]);
            set
            {
                base[_propRestartQueueLimit] = value;
            }
        }

        [ConfigurationProperty("serverErrorMessageFile", DefaultValue="")]
        public string ServerErrorMessageFile
        {
            get => 
                ((string) base[_propServerErrorMessageFile]);
            set
            {
                base[_propServerErrorMessageFile] = value;
            }
        }

        [TypeConverter(typeof(InfiniteTimeSpanConverter)), TimeSpanValidator(MinValueString="00:00:00", MaxValueString="10675199.02:48:05.4775807"), ConfigurationProperty("shutdownTimeout", DefaultValue="00:00:05")]
        public TimeSpan ShutdownTimeout
        {
            get => 
                ((TimeSpan) base[_propShutdownTimeout]);
            set
            {
                base[_propShutdownTimeout] = value;
            }
        }

        [ConfigurationProperty("timeout", DefaultValue="10675199.02:48:05.4775807"), TypeConverter(typeof(InfiniteTimeSpanConverter))]
        public TimeSpan Timeout
        {
            get => 
                ((TimeSpan) base[_propTimeout]);
            set
            {
                base[_propTimeout] = value;
            }
        }

        [ConfigurationProperty("userName", DefaultValue="machine")]
        public string UserName
        {
            get => 
                ((string) base[_propUserName]);
            set
            {
                base[_propUserName] = value;
            }
        }

        [ConfigurationProperty("webGarden", DefaultValue=false)]
        public bool WebGarden
        {
            get => 
                ((bool) base[_propWebGarden]);
            set
            {
                base[_propWebGarden] = value;
            }
        }
    }
}

