namespace System.ServiceModel.Diagnostics
{
    using System;
    using System.Collections.Generic;

    internal class ServiceModelPerformanceCountersEntry
    {
        private System.ServiceModel.Diagnostics.DefaultPerformanceCounters defaultPerformanceCounters;
        private List<ServiceModelPerformanceCounters> performanceCounters;
        private System.ServiceModel.Diagnostics.ServicePerformanceCounters servicePerformanceCounters;

        public ServiceModelPerformanceCountersEntry(System.ServiceModel.Diagnostics.DefaultPerformanceCounters defaultServiceCounters)
        {
            this.defaultPerformanceCounters = defaultServiceCounters;
            this.performanceCounters = new List<ServiceModelPerformanceCounters>();
        }

        public ServiceModelPerformanceCountersEntry(System.ServiceModel.Diagnostics.ServicePerformanceCounters serviceCounters)
        {
            this.servicePerformanceCounters = serviceCounters;
            this.performanceCounters = new List<ServiceModelPerformanceCounters>();
        }

        public void Add(ServiceModelPerformanceCounters counters)
        {
            this.performanceCounters.Add(counters);
        }

        public void Clear()
        {
            this.performanceCounters.Clear();
        }

        public void Remove(string id)
        {
            for (int i = 0; i < this.performanceCounters.Count; i++)
            {
                if (this.performanceCounters[i].PerfCounterId.Equals(id))
                {
                    this.performanceCounters.RemoveAt(i);
                    return;
                }
            }
        }

        public List<ServiceModelPerformanceCounters> CounterList =>
            this.performanceCounters;

        public System.ServiceModel.Diagnostics.DefaultPerformanceCounters DefaultPerformanceCounters
        {
            get => 
                this.defaultPerformanceCounters;
            set
            {
                this.defaultPerformanceCounters = value;
            }
        }

        public System.ServiceModel.Diagnostics.ServicePerformanceCounters ServicePerformanceCounters
        {
            get => 
                this.servicePerformanceCounters;
            set
            {
                this.servicePerformanceCounters = value;
            }
        }
    }
}

