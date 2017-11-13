﻿namespace Microsoft.Win32.SafeHandles
{
    using System;
    using System.Runtime.ConstrainedExecution;
    using System.Runtime.InteropServices;
    using System.Security;

    [SecurityCritical(SecurityCriticalScope.Everything)]
    internal sealed class SafeAxlBufferHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        private SafeAxlBufferHandle() : base(true)
        {
        }

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success), SuppressUnmanagedCodeSecurity, DllImport("kernel32")]
        private static extern IntPtr GetProcessHeap();
        [return: MarshalAs(UnmanagedType.Bool)]
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success), SuppressUnmanagedCodeSecurity, DllImport("kernel32")]
        private static extern bool HeapFree(IntPtr hHeap, int dwFlags, IntPtr lpMem);
        protected override bool ReleaseHandle()
        {
            HeapFree(GetProcessHeap(), 0, base.handle);
            return true;
        }
    }
}

