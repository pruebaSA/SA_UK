﻿namespace System.IdentityModel
{
    using Microsoft.Win32.SafeHandles;
    using System;

    internal class SafeCertStoreHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        private SafeCertStoreHandle() : base(true)
        {
        }

        private SafeCertStoreHandle(IntPtr handle) : base(true)
        {
            base.SetHandle(handle);
        }

        protected override bool ReleaseHandle() => 
            CAPI.CertCloseStore(base.handle, 0);

        public static SafeCertStoreHandle InvalidHandle =>
            new SafeCertStoreHandle(IntPtr.Zero);
    }
}

