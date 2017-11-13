namespace System.ServiceModel.Diagnostics
{
    using System;
    using System.Diagnostics;
    using System.ServiceModel;

    internal class DefaultPerformanceCounters : PerformanceCountersBase
    {
        private PerformanceCounter[] counters;
        private const int hashLength = 2;
        private string instanceName;
        private const int maxCounterLength = 0x40;
        private string[] perfCounterNames = new string[] { "Instances" };

        internal DefaultPerformanceCounters(ServiceHostBase serviceHost)
        {
            this.instanceName = CreateFriendlyInstanceName(serviceHost);
            this.counters = new PerformanceCounter[1];
            for (int i = 0; i < 1; i++)
            {
                PerformanceCounter defaultPerformanceCounter = PerformanceCounters.GetDefaultPerformanceCounter(this.perfCounterNames[i], this.instanceName);
                if (defaultPerformanceCounter == null)
                {
                    break;
                }
                try
                {
                    defaultPerformanceCounter.RawValue = 0L;
                    this.counters[i] = defaultPerformanceCounter;
                }
                catch (Exception exception)
                {
                    if (ExceptionUtility.IsFatal(exception))
                    {
                        throw;
                    }
                    if (DiagnosticUtility.ShouldTraceError)
                    {
                        TraceUtility.TraceEvent(TraceEventType.Error, TraceCode.PerformanceCountersFailedForService, null, exception);
                    }
                    break;
                }
            }
        }

        internal static string CreateFriendlyInstanceName(ServiceHostBase serviceHost) => 
            "_WCF_Admin";

        internal override string[] CounterNames =>
            this.perfCounterNames;

        internal override PerformanceCounter[] Counters
        {
            get => 
                this.counters;
            set
            {
                this.counters = value;
            }
        }

        internal bool Initialized =>
            (this.counters != null);

        internal override string InstanceName =>
            this.instanceName;

        internal override int PerfCounterEnd =>
            1;

        internal override int PerfCounterStart =>
            0;

        private enum PerfCounters
        {
            Instances,
            TotalCounters
        }

        [Flags]
        private enum truncOptions : uint
        {
            NoBits = 0,
            service32 = 1,
            uri31 = 4
        }
    }
}

