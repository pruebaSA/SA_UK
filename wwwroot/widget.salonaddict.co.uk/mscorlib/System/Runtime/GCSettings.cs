namespace System.Runtime
{
    using System;
    using System.Runtime.ConstrainedExecution;
    using System.Security.Permissions;

    public static class GCSettings
    {
        public static bool IsServerGC =>
            GC.nativeIsServerGC();

        public static GCLatencyMode LatencyMode
        {
            [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
            get => 
                ((GCLatencyMode) GC.nativeGetGCLatencyMode());
            [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success), PermissionSet(SecurityAction.LinkDemand, Name="FullTrust"), HostProtection(SecurityAction.LinkDemand, MayLeakOnAbort=true)]
            set
            {
                if ((value < GCLatencyMode.Batch) || (value > GCLatencyMode.LowLatency))
                {
                    throw new ArgumentOutOfRangeException(Environment.GetResourceString("ArgumentOutOfRange_Enum"));
                }
                GC.nativeSetGCLatencyMode((int) value);
            }
        }
    }
}

