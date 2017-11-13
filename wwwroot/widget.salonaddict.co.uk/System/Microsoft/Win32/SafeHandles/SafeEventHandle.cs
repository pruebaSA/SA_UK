namespace Microsoft.Win32.SafeHandles
{
    using System;
    using System.Runtime.ConstrainedExecution;
    using System.Runtime.InteropServices;
    using System.Security;
    using System.Security.Permissions;

    [SuppressUnmanagedCodeSecurity, HostProtection(SecurityAction.LinkDemand, MayLeakOnAbort=true)]
    internal sealed class SafeEventHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        internal SafeEventHandle() : base(true)
        {
        }

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success), DllImport("kernel32.dll", SetLastError=true, ExactSpelling=true)]
        private static extern bool CloseHandle(IntPtr handle);
        [DllImport("kernel32.dll", CharSet=CharSet.Unicode)]
        internal static extern SafeEventHandle CreateEvent(HandleRef lpEventAttributes, bool bManualReset, bool bInitialState, string name);
        protected override bool ReleaseHandle() => 
            CloseHandle(base.handle);
    }
}

