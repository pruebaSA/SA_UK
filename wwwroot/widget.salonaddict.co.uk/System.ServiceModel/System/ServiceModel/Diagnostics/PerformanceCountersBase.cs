namespace System.ServiceModel.Diagnostics
{
    using System;
    using System.Diagnostics;
    using System.Globalization;
    using System.ServiceModel;

    internal abstract class PerformanceCountersBase
    {
        protected PerformanceCountersBase()
        {
        }

        internal void Decrement(int counterIndex)
        {
            PerformanceCounter[] counters = this.Counters;
            PerformanceCounter counter = null;
            try
            {
                if (counters != null)
                {
                    counter = counters[counterIndex];
                    if (counter != null)
                    {
                        counter.Decrement();
                    }
                }
            }
            catch (Exception exception)
            {
                if (DiagnosticUtility.IsFatal(exception))
                {
                    throw;
                }
                PerformanceCounters.TracePerformanceCounterUpdateFailure(this.InstanceName, this.CounterNames[counterIndex]);
                if (counters != null)
                {
                    counters[counterIndex] = null;
                    PerformanceCounters.ReleasePerformanceCounter(ref counter);
                }
            }
        }

        protected static string GetHashedString(string str, int startIndex, int count, bool hashAtEnd)
        {
            string str2 = str.Remove(startIndex, count);
            string str3 = ((uint) (str.GetHashCode() % 0x63)).ToString("00", CultureInfo.InvariantCulture);
            if (!hashAtEnd)
            {
                return (str3 + str2);
            }
            return (str2 + str3);
        }

        internal void Increment(int counterIndex)
        {
            PerformanceCounter[] counters = this.Counters;
            PerformanceCounter counter = null;
            try
            {
                if (counters != null)
                {
                    counter = counters[counterIndex];
                    if (counter != null)
                    {
                        counter.Increment();
                    }
                }
            }
            catch (Exception exception)
            {
                if (DiagnosticUtility.IsFatal(exception))
                {
                    throw;
                }
                PerformanceCounters.TracePerformanceCounterUpdateFailure(this.InstanceName, this.CounterNames[counterIndex]);
                if (counters != null)
                {
                    counters[counterIndex] = null;
                    PerformanceCounters.ReleasePerformanceCounter(ref counter);
                }
            }
        }

        internal void IncrementBy(int counterIndex, long time)
        {
            PerformanceCounter[] counters = this.Counters;
            PerformanceCounter counter = null;
            try
            {
                if (counters != null)
                {
                    counter = counters[counterIndex];
                    if (counter != null)
                    {
                        counter.IncrementBy(time);
                    }
                }
            }
            catch (Exception exception)
            {
                if (DiagnosticUtility.IsFatal(exception))
                {
                    throw;
                }
                PerformanceCounters.TracePerformanceCounterUpdateFailure(this.InstanceName, this.CounterNames[counterIndex]);
                if (counters != null)
                {
                    counters[counterIndex] = null;
                    PerformanceCounters.ReleasePerformanceCounter(ref counter);
                }
            }
        }

        internal void ReleasePerformanceCounters()
        {
            if (PerformanceCounters.PerformanceCountersEnabled)
            {
                PerformanceCounter[] counters = this.Counters;
                this.Counters = null;
                if (counters != null)
                {
                    for (int i = this.PerfCounterStart; i < this.PerfCounterEnd; i++)
                    {
                        PerformanceCounter counter = counters[i];
                        if (counter != null)
                        {
                            PerformanceCounters.ReleasePerformanceCounter(ref counter);
                        }
                        counters[i] = null;
                    }
                }
            }
        }

        internal abstract string[] CounterNames { get; }

        internal abstract PerformanceCounter[] Counters { get; set; }

        internal abstract string InstanceName { get; }

        internal abstract int PerfCounterEnd { get; }

        internal abstract int PerfCounterStart { get; }
    }
}

