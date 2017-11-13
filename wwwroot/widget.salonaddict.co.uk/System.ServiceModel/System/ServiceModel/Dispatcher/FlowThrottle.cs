namespace System.ServiceModel.Dispatcher
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.ServiceModel;
    using System.ServiceModel.Diagnostics;
    using System.Threading;

    internal sealed class FlowThrottle
    {
        private int capacity;
        private string configName;
        private int count;
        private object mutex;
        private string propertyName;
        private WaitCallback release;
        private Queue<object> waiters;

        internal FlowThrottle(WaitCallback release, int capacity, string propertyName, string configName)
        {
            if (capacity <= 0)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(System.ServiceModel.SR.GetString("SFxThrottleLimitMustBeGreaterThanZero0")));
            }
            this.count = 0;
            this.capacity = capacity;
            this.mutex = new object();
            this.release = release;
            this.waiters = new Queue<object>();
            this.propertyName = propertyName;
            this.configName = configName;
        }

        internal bool Acquire(object o)
        {
            lock (this.mutex)
            {
                if (this.count < this.capacity)
                {
                    this.count++;
                    return true;
                }
                if ((this.waiters.Count == 0) && DiagnosticUtility.ShouldTraceInformation)
                {
                    DiagnosticUtility.DiagnosticTrace.TraceEvent(TraceEventType.Information, TraceCode.ServiceThrottleLimitReached, System.ServiceModel.SR.GetString("TraceCodeServiceThrottleLimitReached", new object[] { this.propertyName, this.capacity, this.configName }));
                }
                this.waiters.Enqueue(o);
                return false;
            }
        }

        internal void Release()
        {
            object state = null;
            lock (this.mutex)
            {
                if (this.waiters.Count > 0)
                {
                    state = this.waiters.Dequeue();
                    if (this.waiters.Count == 0)
                    {
                        this.waiters.TrimExcess();
                    }
                }
                else
                {
                    this.count--;
                }
            }
            if (state != null)
            {
                this.release(state);
            }
        }

        internal int Capacity
        {
            get => 
                this.capacity;
            set
            {
                if (value <= 0)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(System.ServiceModel.SR.GetString("SFxThrottleLimitMustBeGreaterThanZero0")));
                }
                this.capacity = value;
            }
        }
    }
}

