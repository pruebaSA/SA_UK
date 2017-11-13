namespace System.IdentityModel
{
    using Microsoft.Win32.SafeHandles;
    using System;

    internal class SafeCertContextHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        private SafeCertContextHandle() : base(true)
        {
        }

        private SafeCertContextHandle(IntPtr handle) : base(true)
        {
            base.SetHandle(handle);
        }

        protected override bool ReleaseHandle() => 
            CAPI.CertFreeCertificateContext(base.handle);

        internal static SafeCertContextHandle InvalidHandle =>
            new SafeCertContextHandle(IntPtr.Zero);
    }
}

