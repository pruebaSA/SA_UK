﻿namespace System.IdentityModel
{
    using Microsoft.Win32.SafeHandles;
    using System;
    using System.Runtime.ConstrainedExecution;
    using System.Runtime.InteropServices;
    using System.Security;

    internal sealed class SafeCloseHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        private const string KERNEL32 = "kernel32.dll";

        private SafeCloseHandle() : base(true)
        {
        }

        internal SafeCloseHandle(IntPtr handle, bool ownsHandle) : base(ownsHandle)
        {
            base.SetHandle(handle);
        }

        [SuppressUnmanagedCodeSecurity, ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success), DllImport("kernel32.dll", SetLastError=true, ExactSpelling=true)]
        private static extern bool CloseHandle(IntPtr handle);
        protected override bool ReleaseHandle() => 
            CloseHandle(base.handle);
    }
}

