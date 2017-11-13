namespace System.Diagnostics
{
    using System;
    using System.ComponentModel;
    using System.Globalization;
    using System.Runtime.CompilerServices;
    using System.Runtime.ConstrainedExecution;
    using System.Security.Permissions;
    using System.Threading;

    [SRDescription("PerformanceCounterDesc"), InstallerType("System.Diagnostics.PerformanceCounterInstaller,System.Configuration.Install, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"), HostProtection(SecurityAction.LinkDemand, Synchronization=true, SharedState=true)]
    public sealed class PerformanceCounter : Component, ISupportInitialize
    {
        private string categoryName;
        private string counterName;
        private int counterType;
        [Obsolete("This field has been deprecated and is not used.  Use machine.config or an application configuration file to set the size of the PerformanceCounter file mapping.")]
        public static int DefaultFileMappingSize = 0x80000;
        private string helpMsg;
        private bool initialized;
        private PerformanceCounterInstanceLifetime instanceLifetime;
        private string instanceName;
        private bool isReadOnly;
        private string machineName;
        private CounterSample oldSample;
        private SharedPerformanceCounter sharedCounter;

        public PerformanceCounter()
        {
            this.counterType = -1;
            this.oldSample = CounterSample.Empty;
            this.machineName = ".";
            this.categoryName = string.Empty;
            this.counterName = string.Empty;
            this.instanceName = string.Empty;
            this.isReadOnly = true;
            GC.SuppressFinalize(this);
        }

        public PerformanceCounter(string categoryName, string counterName) : this(categoryName, counterName, true)
        {
        }

        public PerformanceCounter(string categoryName, string counterName, bool readOnly) : this(categoryName, counterName, "", readOnly)
        {
        }

        public PerformanceCounter(string categoryName, string counterName, string instanceName) : this(categoryName, counterName, instanceName, true)
        {
        }

        public PerformanceCounter(string categoryName, string counterName, string instanceName, bool readOnly)
        {
            this.counterType = -1;
            this.oldSample = CounterSample.Empty;
            this.MachineName = ".";
            this.CategoryName = categoryName;
            this.CounterName = counterName;
            this.InstanceName = instanceName;
            this.isReadOnly = readOnly;
            this.Initialize();
            GC.SuppressFinalize(this);
        }

        public PerformanceCounter(string categoryName, string counterName, string instanceName, string machineName)
        {
            this.counterType = -1;
            this.oldSample = CounterSample.Empty;
            this.MachineName = machineName;
            this.CategoryName = categoryName;
            this.CounterName = counterName;
            this.InstanceName = instanceName;
            this.isReadOnly = true;
            this.Initialize();
            GC.SuppressFinalize(this);
        }

        internal PerformanceCounter(string categoryName, string counterName, string instanceName, string machineName, bool skipInit)
        {
            this.counterType = -1;
            this.oldSample = CounterSample.Empty;
            this.MachineName = machineName;
            this.CategoryName = categoryName;
            this.CounterName = counterName;
            this.InstanceName = instanceName;
            this.isReadOnly = true;
            this.initialized = true;
            GC.SuppressFinalize(this);
        }

        public void BeginInit()
        {
            this.Close();
        }

        public void Close()
        {
            this.helpMsg = null;
            this.oldSample = CounterSample.Empty;
            this.sharedCounter = null;
            this.initialized = false;
            this.counterType = -1;
        }

        public static void CloseSharedResources()
        {
            new PerformanceCounterPermission(PerformanceCounterPermissionAccess.Browse, ".", "*").Demand();
            PerformanceCounterLib.CloseAllLibraries();
        }

        public long Decrement()
        {
            if (this.ReadOnly)
            {
                this.ThrowReadOnly();
            }
            this.Initialize();
            return this.sharedCounter.Decrement();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.Close();
            }
            base.Dispose(disposing);
        }

        public void EndInit()
        {
            this.Initialize();
        }

        public long Increment()
        {
            if (this.isReadOnly)
            {
                this.ThrowReadOnly();
            }
            this.Initialize();
            return this.sharedCounter.Increment();
        }

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
        public long IncrementBy(long value)
        {
            if (this.isReadOnly)
            {
                this.ThrowReadOnly();
            }
            this.Initialize();
            return this.sharedCounter.IncrementBy(value);
        }

        private void Initialize()
        {
            if (!this.initialized && !base.DesignMode)
            {
                bool flag = false;
                RuntimeHelpers.PrepareConstrainedRegions();
                try
                {
                    RuntimeHelpers.PrepareConstrainedRegions();
                    try
                    {
                    }
                    finally
                    {
                        Monitor.Enter(this);
                        flag = true;
                    }
                    if (!this.initialized)
                    {
                        string categoryName = this.categoryName;
                        string machineName = this.machineName;
                        if (categoryName == string.Empty)
                        {
                            throw new InvalidOperationException(SR.GetString("CategoryNameMissing"));
                        }
                        if (this.counterName == string.Empty)
                        {
                            throw new InvalidOperationException(SR.GetString("CounterNameMissing"));
                        }
                        if (this.ReadOnly)
                        {
                            new PerformanceCounterPermission(PerformanceCounterPermissionAccess.Browse, machineName, categoryName).Demand();
                            if (!PerformanceCounterLib.CounterExists(machineName, categoryName, this.counterName))
                            {
                                throw new InvalidOperationException(SR.GetString("CounterExists", new object[] { categoryName, this.counterName }));
                            }
                            PerformanceCounterCategoryType categoryType = PerformanceCounterLib.GetCategoryType(machineName, categoryName);
                            if (categoryType == PerformanceCounterCategoryType.MultiInstance)
                            {
                                if (string.IsNullOrEmpty(this.instanceName))
                                {
                                    throw new InvalidOperationException(SR.GetString("MultiInstanceOnly", new object[] { categoryName }));
                                }
                            }
                            else if ((categoryType == PerformanceCounterCategoryType.SingleInstance) && !string.IsNullOrEmpty(this.instanceName))
                            {
                                throw new InvalidOperationException(SR.GetString("SingleInstanceOnly", new object[] { categoryName }));
                            }
                            if (this.instanceLifetime != PerformanceCounterInstanceLifetime.Global)
                            {
                                throw new InvalidOperationException(SR.GetString("InstanceLifetimeProcessonReadOnly"));
                            }
                            this.initialized = true;
                        }
                        else
                        {
                            new PerformanceCounterPermission(PerformanceCounterPermissionAccess.Write, machineName, categoryName).Demand();
                            if ((machineName != ".") && (string.Compare(machineName, PerformanceCounterLib.ComputerName, StringComparison.OrdinalIgnoreCase) != 0))
                            {
                                throw new InvalidOperationException(SR.GetString("RemoteWriting"));
                            }
                            SharedUtils.CheckNtEnvironment();
                            if (!PerformanceCounterLib.IsCustomCategory(machineName, categoryName))
                            {
                                throw new InvalidOperationException(SR.GetString("NotCustomCounter"));
                            }
                            PerformanceCounterCategoryType type2 = PerformanceCounterLib.GetCategoryType(machineName, categoryName);
                            if (type2 == PerformanceCounterCategoryType.MultiInstance)
                            {
                                if (string.IsNullOrEmpty(this.instanceName))
                                {
                                    throw new InvalidOperationException(SR.GetString("MultiInstanceOnly", new object[] { categoryName }));
                                }
                            }
                            else if ((type2 == PerformanceCounterCategoryType.SingleInstance) && !string.IsNullOrEmpty(this.instanceName))
                            {
                                throw new InvalidOperationException(SR.GetString("SingleInstanceOnly", new object[] { categoryName }));
                            }
                            if (string.IsNullOrEmpty(this.instanceName) && (this.InstanceLifetime == PerformanceCounterInstanceLifetime.Process))
                            {
                                throw new InvalidOperationException(SR.GetString("InstanceLifetimeProcessforSingleInstance"));
                            }
                            this.sharedCounter = new SharedPerformanceCounter(categoryName.ToLower(CultureInfo.InvariantCulture), this.counterName.ToLower(CultureInfo.InvariantCulture), this.instanceName.ToLower(CultureInfo.InvariantCulture), this.instanceLifetime);
                            this.initialized = true;
                        }
                    }
                }
                finally
                {
                    if (flag)
                    {
                        Monitor.Exit(this);
                    }
                }
            }
        }

        public CounterSample NextSample()
        {
            string categoryName = this.categoryName;
            string machineName = this.machineName;
            new PerformanceCounterPermission(PerformanceCounterPermissionAccess.Browse, machineName, categoryName).Demand();
            this.Initialize();
            CategorySample categorySample = PerformanceCounterLib.GetCategorySample(machineName, categoryName);
            CounterDefinitionSample counterDefinitionSample = categorySample.GetCounterDefinitionSample(this.counterName);
            this.counterType = counterDefinitionSample.CounterType;
            if (!categorySample.IsMultiInstance)
            {
                if ((this.instanceName != null) && (this.instanceName.Length != 0))
                {
                    throw new InvalidOperationException(SR.GetString("InstanceNameProhibited", new object[] { this.instanceName }));
                }
                return counterDefinitionSample.GetSingleValue();
            }
            if ((this.instanceName == null) || (this.instanceName.Length == 0))
            {
                throw new InvalidOperationException(SR.GetString("InstanceNameRequired"));
            }
            return counterDefinitionSample.GetInstanceValue(this.instanceName);
        }

        public float NextValue()
        {
            CounterSample nextCounterSample = this.NextSample();
            float num = 0f;
            num = CounterSample.Calculate(this.oldSample, nextCounterSample);
            this.oldSample = nextCounterSample;
            return num;
        }

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
        public void RemoveInstance()
        {
            if (this.isReadOnly)
            {
                throw new InvalidOperationException(SR.GetString("ReadOnlyRemoveInstance"));
            }
            this.Initialize();
            this.sharedCounter.RemoveInstance(this.instanceName.ToLower(CultureInfo.InvariantCulture), this.instanceLifetime);
        }

        private void ThrowReadOnly()
        {
            throw new InvalidOperationException(SR.GetString("ReadOnlyCounter"));
        }

        [ReadOnly(true), SRDescription("PCCategoryName"), RecommendedAsConfigurable(true), TypeConverter("System.Diagnostics.Design.CategoryValueConverter, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"), DefaultValue("")]
        public string CategoryName
        {
            get => 
                this.categoryName;
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }
                if ((this.categoryName == null) || (string.Compare(this.categoryName, value, StringComparison.OrdinalIgnoreCase) != 0))
                {
                    this.categoryName = value;
                    this.Close();
                }
            }
        }

        [MonitoringDescription("PC_CounterHelp"), ReadOnly(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string CounterHelp
        {
            get
            {
                string categoryName = this.categoryName;
                string machineName = this.machineName;
                new PerformanceCounterPermission(PerformanceCounterPermissionAccess.Browse, machineName, categoryName).Demand();
                this.Initialize();
                if (this.helpMsg == null)
                {
                    this.helpMsg = PerformanceCounterLib.GetCounterHelp(machineName, categoryName, this.counterName);
                }
                return this.helpMsg;
            }
        }

        [SRDescription("PCCounterName"), TypeConverter("System.Diagnostics.Design.CounterNameConverter, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"), ReadOnly(true), DefaultValue(""), RecommendedAsConfigurable(true)]
        public string CounterName
        {
            get => 
                this.counterName;
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }
                if ((this.counterName == null) || (string.Compare(this.counterName, value, StringComparison.OrdinalIgnoreCase) != 0))
                {
                    this.counterName = value;
                    this.Close();
                }
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), MonitoringDescription("PC_CounterType")]
        public PerformanceCounterType CounterType
        {
            get
            {
                if (this.counterType == -1)
                {
                    string categoryName = this.categoryName;
                    string machineName = this.machineName;
                    new PerformanceCounterPermission(PerformanceCounterPermissionAccess.Browse, machineName, categoryName).Demand();
                    this.Initialize();
                    CounterDefinitionSample counterDefinitionSample = PerformanceCounterLib.GetCategorySample(machineName, categoryName).GetCounterDefinitionSample(this.counterName);
                    this.counterType = counterDefinitionSample.CounterType;
                }
                return (PerformanceCounterType) this.counterType;
            }
        }

        [SRDescription("PCInstanceLifetime"), DefaultValue(0)]
        public PerformanceCounterInstanceLifetime InstanceLifetime
        {
            get => 
                this.instanceLifetime;
            set
            {
                if ((value > PerformanceCounterInstanceLifetime.Process) || (value < PerformanceCounterInstanceLifetime.Global))
                {
                    throw new ArgumentOutOfRangeException("value");
                }
                if (this.initialized)
                {
                    throw new InvalidOperationException(SR.GetString("CantSetLifetimeAfterInitialized"));
                }
                this.instanceLifetime = value;
            }
        }

        [SRDescription("PCInstanceName"), RecommendedAsConfigurable(true), ReadOnly(true), DefaultValue(""), TypeConverter("System.Diagnostics.Design.InstanceNameConverter, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
        public string InstanceName
        {
            get => 
                this.instanceName;
            set
            {
                if (((value != null) || (this.instanceName != null)) && ((((value == null) && (this.instanceName != null)) || ((value != null) && (this.instanceName == null))) || (string.Compare(this.instanceName, value, StringComparison.OrdinalIgnoreCase) != 0)))
                {
                    this.instanceName = value;
                    this.Close();
                }
            }
        }

        [SRDescription("PCMachineName"), RecommendedAsConfigurable(true), Browsable(false), DefaultValue(".")]
        public string MachineName
        {
            get => 
                this.machineName;
            set
            {
                if (!SyntaxCheck.CheckMachineName(value))
                {
                    throw new ArgumentException(SR.GetString("InvalidParameter", new object[] { "machineName", value }));
                }
                if (this.machineName != value)
                {
                    this.machineName = value;
                    this.Close();
                }
            }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), MonitoringDescription("PC_RawValue")]
        public long RawValue
        {
            get
            {
                if (this.ReadOnly)
                {
                    return this.NextSample().RawValue;
                }
                this.Initialize();
                return this.sharedCounter.Value;
            }
            set
            {
                if (this.ReadOnly)
                {
                    this.ThrowReadOnly();
                }
                this.Initialize();
                this.sharedCounter.Value = value;
            }
        }

        [MonitoringDescription("PC_ReadOnly"), DefaultValue(true), Browsable(false)]
        public bool ReadOnly
        {
            get => 
                this.isReadOnly;
            set
            {
                if (value != this.isReadOnly)
                {
                    this.isReadOnly = value;
                    this.Close();
                }
            }
        }
    }
}

