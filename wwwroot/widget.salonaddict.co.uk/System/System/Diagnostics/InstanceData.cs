namespace System.Diagnostics
{
    using System;

    public class InstanceData
    {
        private string instanceName;
        private CounterSample sample;

        public InstanceData(string instanceName, CounterSample sample)
        {
            this.instanceName = instanceName;
            this.sample = sample;
        }

        public string InstanceName =>
            this.instanceName;

        public long RawValue =>
            this.sample.RawValue;

        public CounterSample Sample =>
            this.sample;
    }
}

