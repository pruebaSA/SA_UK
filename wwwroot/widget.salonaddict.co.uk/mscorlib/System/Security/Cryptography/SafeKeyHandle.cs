namespace System.Security.Cryptography
{
    using Microsoft.Win32.SafeHandles;
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.ConstrainedExecution;

    internal sealed class SafeKeyHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        private SafeKeyHandle(IntPtr handle) : base(true)
        {
            base.SetHandle(handle);
        }

        [MethodImpl(MethodImplOptions.InternalCall), ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        private static extern void _FreeHKey(IntPtr pKeyCtx);
        protected override bool ReleaseHandle()
        {
            _FreeHKey(base.handle);
            return true;
        }

        internal static SafeKeyHandle InvalidHandle =>
            new SafeKeyHandle(IntPtr.Zero);
    }
}

