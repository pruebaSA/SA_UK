namespace System.Security.Cryptography
{
    using Microsoft.Win32.SafeHandles;
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.ConstrainedExecution;

    internal sealed class SafeHashHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        private SafeHashHandle(IntPtr handle) : base(true)
        {
            base.SetHandle(handle);
        }

        [MethodImpl(MethodImplOptions.InternalCall), ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        private static extern void _FreeHash(IntPtr pHashCtx);
        protected override bool ReleaseHandle()
        {
            _FreeHash(base.handle);
            return true;
        }

        internal static SafeHashHandle InvalidHandle =>
            new SafeHashHandle(IntPtr.Zero);
    }
}

