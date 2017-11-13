namespace System.Security.Cryptography
{
    using Microsoft.Win32.SafeHandles;
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.ConstrainedExecution;

    internal sealed class SafeProvHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        private SafeProvHandle(IntPtr handle) : base(true)
        {
            base.SetHandle(handle);
        }

        [MethodImpl(MethodImplOptions.InternalCall), ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        private static extern void _FreeCSP(IntPtr pProvCtx);
        protected override bool ReleaseHandle()
        {
            _FreeCSP(base.handle);
            return true;
        }

        internal static SafeProvHandle InvalidHandle =>
            new SafeProvHandle(IntPtr.Zero);
    }
}

