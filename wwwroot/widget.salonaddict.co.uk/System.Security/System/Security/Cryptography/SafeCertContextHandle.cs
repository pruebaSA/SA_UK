﻿namespace System.Security.Cryptography
{
    using Microsoft.Win32.SafeHandles;
    using System;
    using System.Runtime.ConstrainedExecution;
    using System.Runtime.InteropServices;
    using System.Security;

    internal sealed class SafeCertContextHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        private SafeCertContextHandle() : base(true)
        {
        }

        internal SafeCertContextHandle(IntPtr handle) : base(true)
        {
            base.SetHandle(handle);
        }

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success), SuppressUnmanagedCodeSecurity, DllImport("crypt32.dll", SetLastError=true)]
        private static extern bool CertFreeCertificateContext(IntPtr pCertContext);
        protected override bool ReleaseHandle() => 
            CertFreeCertificateContext(base.handle);

        internal static System.Security.Cryptography.SafeCertContextHandle InvalidHandle =>
            new System.Security.Cryptography.SafeCertContextHandle(IntPtr.Zero);
    }
}

