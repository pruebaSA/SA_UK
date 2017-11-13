namespace System.Net
{
    using Microsoft.Win32.SafeHandles;
    using System;
    using System.Net.NetworkInformation;
    using System.Runtime.ConstrainedExecution;
    using System.Security;

    [SuppressUnmanagedCodeSecurity]
    internal sealed class SafeCloseIcmpHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        private bool IsPostWin2K;

        private SafeCloseIcmpHandle() : base(true)
        {
            this.IsPostWin2K = ComNetOS.IsPostWin2K;
        }

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
        protected override bool ReleaseHandle()
        {
            if (this.IsPostWin2K)
            {
                return UnsafeNetInfoNativeMethods.IcmpCloseHandle(base.handle);
            }
            return UnsafeIcmpNativeMethods.IcmpCloseHandle(base.handle);
        }
    }
}

