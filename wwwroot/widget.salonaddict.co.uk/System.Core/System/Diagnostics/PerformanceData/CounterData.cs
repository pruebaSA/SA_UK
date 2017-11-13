namespace System.Diagnostics.PerformanceData
{
    using System;
    using System.Security;
    using System.Security.Permissions;

    [HostProtection(SecurityAction.LinkDemand, MayLeakOnAbort=true)]
    public sealed class CounterData
    {
        private unsafe long* m_offset;

        [SecurityCritical]
        internal unsafe CounterData(long* pCounterData)
        {
            this.m_offset = pCounterData;
            this.m_offset[0] = 0L;
        }

        public long Value
        {
            [SecurityCritical]
            get => 
                this.m_offset[0];
            [SecurityCritical]
            set
            {
                this.m_offset[0] = value;
            }
        }
    }
}

