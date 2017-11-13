﻿namespace System.Security.Cryptography
{
    using Microsoft.Win32.SafeHandles;
    using System;
    using System.Runtime.ConstrainedExecution;
    using System.Runtime.InteropServices;
    using System.Security;

    internal sealed class SafeLocalAllocHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        private SafeLocalAllocHandle() : base(true)
        {
        }

        internal SafeLocalAllocHandle(IntPtr handle) : base(true)
        {
            base.SetHandle(handle);
        }

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success), SuppressUnmanagedCodeSecurity, DllImport("kernel32.dll", SetLastError=true)]
        private static extern IntPtr LocalFree(IntPtr handle);
        protected override bool ReleaseHandle() => 
            (LocalFree(base.handle) == IntPtr.Zero);

        internal static System.Security.Cryptography.SafeLocalAllocHandle InvalidHandle =>
            new System.Security.Cryptography.SafeLocalAllocHandle(IntPtr.Zero);
    }
}

