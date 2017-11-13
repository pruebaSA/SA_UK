﻿namespace System.IdentityModel
{
    using Microsoft.Win32.SafeHandles;
    using System;

    internal sealed class SafeLsaLogonProcessHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        private SafeLsaLogonProcessHandle() : base(true)
        {
        }

        internal SafeLsaLogonProcessHandle(IntPtr handle) : base(true)
        {
            base.SetHandle(handle);
        }

        protected override bool ReleaseHandle() => 
            (NativeMethods.LsaDeregisterLogonProcess(base.handle) >= 0);

        internal static System.IdentityModel.SafeLsaLogonProcessHandle InvalidHandle =>
            new System.IdentityModel.SafeLsaLogonProcessHandle(IntPtr.Zero);
    }
}

