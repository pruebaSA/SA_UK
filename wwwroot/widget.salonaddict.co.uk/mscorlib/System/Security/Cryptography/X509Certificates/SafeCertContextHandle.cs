namespace System.Security.Cryptography.X509Certificates
{
    using Microsoft.Win32.SafeHandles;
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.ConstrainedExecution;
    using System.Runtime.InteropServices;

    internal sealed class SafeCertContextHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        private SafeCertContextHandle() : base(true)
        {
        }

        internal SafeCertContextHandle(IntPtr handle) : base(true)
        {
            base.SetHandle(handle);
        }

        [MethodImpl(MethodImplOptions.InternalCall), ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        private static extern void _FreePCertContext(IntPtr pCert);
        protected override bool ReleaseHandle()
        {
            _FreePCertContext(base.handle);
            return true;
        }

        internal static SafeCertContextHandle InvalidHandle =>
            new SafeCertContextHandle(IntPtr.Zero);

        internal IntPtr pCertContext
        {
            get
            {
                if (base.handle == IntPtr.Zero)
                {
                    return IntPtr.Zero;
                }
                return Marshal.ReadIntPtr(base.handle);
            }
        }
    }
}

