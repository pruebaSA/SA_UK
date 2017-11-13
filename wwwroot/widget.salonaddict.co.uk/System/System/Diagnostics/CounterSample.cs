namespace System.Diagnostics
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct CounterSample
    {
        private long rawValue;
        private long baseValue;
        private long timeStamp;
        private long counterFrequency;
        private PerformanceCounterType counterType;
        private long timeStamp100nSec;
        private long systemFrequency;
        private long counterTimeStamp;
        public static CounterSample Empty;
        public CounterSample(long rawValue, long baseValue, long counterFrequency, long systemFrequency, long timeStamp, long timeStamp100nSec, PerformanceCounterType counterType)
        {
            this.rawValue = rawValue;
            this.baseValue = baseValue;
            this.timeStamp = timeStamp;
            this.counterFrequency = counterFrequency;
            this.counterType = counterType;
            this.timeStamp100nSec = timeStamp100nSec;
            this.systemFrequency = systemFrequency;
            this.counterTimeStamp = 0L;
        }

        public CounterSample(long rawValue, long baseValue, long counterFrequency, long systemFrequency, long timeStamp, long timeStamp100nSec, PerformanceCounterType counterType, long counterTimeStamp)
        {
            this.rawValue = rawValue;
            this.baseValue = baseValue;
            this.timeStamp = timeStamp;
            this.counterFrequency = counterFrequency;
            this.counterType = counterType;
            this.timeStamp100nSec = timeStamp100nSec;
            this.systemFrequency = systemFrequency;
            this.counterTimeStamp = counterTimeStamp;
        }

        public long RawValue =>
            this.rawValue;
        internal ulong UnsignedRawValue =>
            ((ulong) this.rawValue);
        public long BaseValue =>
            this.baseValue;
        public long SystemFrequency =>
            this.systemFrequency;
        public long CounterFrequency =>
            this.counterFrequency;
        public long CounterTimeStamp =>
            this.counterTimeStamp;
        public long TimeStamp =>
            this.timeStamp;
        public long TimeStamp100nSec =>
            this.timeStamp100nSec;
        public PerformanceCounterType CounterType =>
            this.counterType;
        public static float Calculate(CounterSample counterSample) => 
            CounterSampleCalculator.ComputeCounterValue(counterSample);

        public static float Calculate(CounterSample counterSample, CounterSample nextCounterSample) => 
            CounterSampleCalculator.ComputeCounterValue(counterSample, nextCounterSample);

        public override bool Equals(object o) => 
            ((o is CounterSample) && this.Equals((CounterSample) o));

        public bool Equals(CounterSample sample) => 
            (((((this.rawValue == sample.rawValue) && (this.baseValue == sample.baseValue)) && ((this.timeStamp == sample.timeStamp) && (this.counterFrequency == sample.counterFrequency))) && (((this.counterType == sample.counterType) && (this.timeStamp100nSec == sample.timeStamp100nSec)) && (this.systemFrequency == sample.systemFrequency))) && (this.counterTimeStamp == sample.counterTimeStamp));

        public override int GetHashCode() => 
            this.rawValue.GetHashCode();

        public static bool operator ==(CounterSample a, CounterSample b) => 
            a.Equals(b);

        public static bool operator !=(CounterSample a, CounterSample b) => 
            !a.Equals(b);

        static CounterSample()
        {
            Empty = new CounterSample(0L, 0L, 0L, 0L, 0L, 0L, PerformanceCounterType.NumberOfItems32);
        }
    }
}

