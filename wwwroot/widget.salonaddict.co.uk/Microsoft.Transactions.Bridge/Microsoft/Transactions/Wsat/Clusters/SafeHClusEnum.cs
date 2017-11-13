﻿namespace Microsoft.Transactions.Wsat.Clusters
{
    using System;
    using System.Runtime.ConstrainedExecution;
    using System.Runtime.InteropServices;

    internal class SafeHClusEnum : SafeClusterHandle
    {
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail), DllImport("clusapi.dll")]
        private static extern uint ClusterCloseEnum([In] IntPtr hEnum);
        protected override bool ReleaseHandle() => 
            (ClusterCloseEnum(base.handle) == 0);
    }
}

