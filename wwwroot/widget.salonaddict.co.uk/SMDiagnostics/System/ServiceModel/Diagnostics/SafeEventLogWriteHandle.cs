namespace System.ServiceModel.Diagnostics
{
    using Microsoft.Win32.SafeHandles;
    using System;
    using System.Runtime.InteropServices;

    internal sealed class SafeEventLogWriteHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        private SafeEventLogWriteHandle() : base(true)
        {
        }

        [DllImport("advapi32", SetLastError=true)]
        private static extern bool DeregisterEventSource(IntPtr hEventLog);
        internal static System.ServiceModel.Diagnostics.SafeEventLogWriteHandle RegisterEventSource(string uncServerName, string sourceName)
        {
            System.ServiceModel.Diagnostics.SafeEventLogWriteHandle handle = NativeMethods.RegisterEventSource(uncServerName, sourceName);
            Marshal.GetLastWin32Error();
            bool isInvalid = handle.IsInvalid;
            return handle;
        }

        protected override bool ReleaseHandle() => 
            DeregisterEventSource(base.handle);
    }
}

