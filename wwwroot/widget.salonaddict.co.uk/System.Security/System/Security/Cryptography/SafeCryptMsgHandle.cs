namespace System.Security.Cryptography
{
    using Microsoft.Win32.SafeHandles;
    using System;
    using System.Runtime.ConstrainedExecution;
    using System.Runtime.InteropServices;
    using System.Security;

    internal sealed class SafeCryptMsgHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        private SafeCryptMsgHandle() : base(true)
        {
        }

        internal SafeCryptMsgHandle(IntPtr handle) : base(true)
        {
            base.SetHandle(handle);
        }

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success), SuppressUnmanagedCodeSecurity, DllImport("crypt32.dll", SetLastError=true)]
        private static extern bool CryptMsgClose(IntPtr handle);
        protected override bool ReleaseHandle() => 
            CryptMsgClose(base.handle);

        internal static System.Security.Cryptography.SafeCryptMsgHandle InvalidHandle =>
            new System.Security.Cryptography.SafeCryptMsgHandle(IntPtr.Zero);
    }
}

