namespace Microsoft.Win32.SafeHandles
{
    using Microsoft.Contracts;
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.ConstrainedExecution;
    using System.Runtime.InteropServices;
    using System.Security;
    using System.Security.Cryptography;

    [SecurityCritical(SecurityCriticalScope.Everything)]
    internal sealed class SafeCapiKeyHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        private IntPtr m_csp;

        private SafeCapiKeyHandle() : base(true)
        {
        }

        [return: MarshalAs(UnmanagedType.Bool)]
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success), SuppressUnmanagedCodeSecurity, DllImport("advapi32", SetLastError=true)]
        private static extern bool CryptContextAddRef(IntPtr hProv, IntPtr pdwReserved, int dwFlags);
        [return: MarshalAs(UnmanagedType.Bool)]
        [SuppressUnmanagedCodeSecurity, ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success), DllImport("advapi32")]
        private static extern bool CryptDestroyKey(IntPtr hKey);
        [return: MarshalAs(UnmanagedType.Bool)]
        [SuppressUnmanagedCodeSecurity, ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success), DllImport("advapi32")]
        private static extern bool CryptReleaseContext(IntPtr hProv, int dwFlags);
        internal SafeCapiKeyHandle Duplicate()
        {
            Contract.Requires(!this.IsInvalid && !base.IsClosed);
            SafeCapiKeyHandle phKey = null;
            RuntimeHelpers.PrepareConstrainedRegions();
            try
            {
                if (!CapiNative.UnsafeNativeMethods.CryptDuplicateKey(this, IntPtr.Zero, 0, out phKey))
                {
                    throw new CryptographicException(Marshal.GetLastWin32Error());
                }
            }
            finally
            {
                if (((phKey != null) && !phKey.IsInvalid) && (this.m_csp != IntPtr.Zero))
                {
                    phKey.SetCsp(this.m_csp);
                }
            }
            return phKey;
        }

        protected override bool ReleaseHandle()
        {
            bool flag = CryptDestroyKey(base.handle);
            bool flag2 = true;
            if (this.m_csp != IntPtr.Zero)
            {
                flag2 = CryptReleaseContext(this.m_csp, 0);
            }
            return (flag && flag2);
        }

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
        internal void SetCsp(SafeCspHandle parentCsp)
        {
            bool success = false;
            RuntimeHelpers.PrepareConstrainedRegions();
            try
            {
                parentCsp.DangerousAddRef(ref success);
                this.SetCsp(parentCsp.DangerousGetHandle());
            }
            finally
            {
                if (success)
                {
                    parentCsp.DangerousRelease();
                }
            }
        }

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
        internal void SetCsp(IntPtr parentCsp)
        {
            Contract.Requires(this.m_csp == IntPtr.Zero);
            int hr = 0;
            RuntimeHelpers.PrepareConstrainedRegions();
            try
            {
            }
            finally
            {
                if (!CryptContextAddRef(parentCsp, IntPtr.Zero, 0))
                {
                    hr = Marshal.GetLastWin32Error();
                }
                else
                {
                    this.m_csp = parentCsp;
                }
            }
            if (hr != 0)
            {
                throw new CryptographicException(hr);
            }
        }

        internal static SafeCapiKeyHandle InvalidHandle
        {
            get
            {
                SafeCapiKeyHandle handle = new SafeCapiKeyHandle();
                handle.SetHandle(IntPtr.Zero);
                return handle;
            }
        }
    }
}

