namespace System.Diagnostics
{
    using System;
    using System.ComponentModel;

    [Serializable, TypeConverter("System.Diagnostics.Design.CounterCreationDataConverter, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
    public class CounterCreationData
    {
        private string counterHelp;
        private string counterName;
        private PerformanceCounterType counterType;

        public CounterCreationData()
        {
            this.counterType = PerformanceCounterType.NumberOfItems32;
            this.counterName = string.Empty;
            this.counterHelp = string.Empty;
        }

        public CounterCreationData(string counterName, string counterHelp, PerformanceCounterType counterType)
        {
            this.counterType = PerformanceCounterType.NumberOfItems32;
            this.counterName = string.Empty;
            this.counterHelp = string.Empty;
            this.CounterType = counterType;
            this.CounterName = counterName;
            this.CounterHelp = counterHelp;
        }

        [DefaultValue(""), MonitoringDescription("CounterHelp")]
        public string CounterHelp
        {
            get => 
                this.counterHelp;
            set
            {
                PerformanceCounterCategory.CheckValidHelp(value);
                this.counterHelp = value;
            }
        }

        [MonitoringDescription("CounterName"), DefaultValue(""), TypeConverter("System.Diagnostics.Design.StringValueConverter, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
        public string CounterName
        {
            get => 
                this.counterName;
            set
            {
                PerformanceCounterCategory.CheckValidCounter(value);
                this.counterName = value;
            }
        }

        [DefaultValue(0x10000), MonitoringDescription("CounterType")]
        public PerformanceCounterType CounterType
        {
            get => 
                this.counterType;
            set
            {
                if (!Enum.IsDefined(typeof(PerformanceCounterType), value))
                {
                    throw new InvalidEnumArgumentException("value", (int) value, typeof(PerformanceCounterType));
                }
                this.counterType = value;
            }
        }
    }
}

