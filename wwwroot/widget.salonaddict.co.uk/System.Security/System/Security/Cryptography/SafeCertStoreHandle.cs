namespace System.Security.Cryptography
{
    using Microsoft.Win32.SafeHandles;
    using System;
    using System.Runtime.ConstrainedExecution;
    using System.Runtime.InteropServices;
    using System.Security;

    internal sealed class SafeCertStoreHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        private SafeCertStoreHandle() : base(true)
        {
        }

        internal SafeCertStoreHandle(IntPtr handle) : base(true)
        {
            base.SetHandle(handle);
        }

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success), SuppressUnmanagedCodeSecurity, DllImport("crypt32.dll", SetLastError=true)]
        private static extern bool CertCloseStore(IntPtr hCertStore, uint dwFlags);
        protected override bool ReleaseHandle() => 
            CertCloseStore(base.handle, 0);

        internal static System.Security.Cryptography.SafeCertStoreHandle InvalidHandle =>
            new System.Security.Cryptography.SafeCertStoreHandle(IntPtr.Zero);
    }
}

