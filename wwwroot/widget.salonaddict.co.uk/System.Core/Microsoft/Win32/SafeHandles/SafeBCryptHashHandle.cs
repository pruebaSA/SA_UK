namespace Microsoft.Win32.SafeHandles
{
    using Microsoft.Contracts;
    using System;
    using System.Runtime.ConstrainedExecution;
    using System.Runtime.InteropServices;
    using System.Security;
    using System.Security.Cryptography;

    [SecurityCritical(SecurityCriticalScope.Everything)]
    internal sealed class SafeBCryptHashHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        private IntPtr m_hashObject;

        private SafeBCryptHashHandle() : base(true)
        {
        }

        [SuppressUnmanagedCodeSecurity, ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success), DllImport("bcrypt")]
        private static extern BCryptNative.ErrorCode BCryptDestroyHash(IntPtr hHash);
        protected override bool ReleaseHandle()
        {
            bool flag = BCryptDestroyHash(base.handle) == BCryptNative.ErrorCode.Success;
            if (this.m_hashObject != IntPtr.Zero)
            {
                Marshal.FreeCoTaskMem(this.m_hashObject);
            }
            return flag;
        }

        internal IntPtr HashObject
        {
            get => 
                this.m_hashObject;
            set
            {
                Contract.Requires(value != IntPtr.Zero);
                this.m_hashObject = value;
            }
        }
    }
}

