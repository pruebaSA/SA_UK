namespace Microsoft.Win32.SafeHandles
{
    using Microsoft.Win32;
    using System;

    internal sealed class SafeLsaMemoryHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        private SafeLsaMemoryHandle() : base(true)
        {
        }

        internal SafeLsaMemoryHandle(IntPtr handle) : base(true)
        {
            base.SetHandle(handle);
        }

        protected override bool ReleaseHandle() => 
            (Win32Native.LsaFreeMemory(base.handle) == 0);

        internal static SafeLsaMemoryHandle InvalidHandle =>
            new SafeLsaMemoryHandle(IntPtr.Zero);
    }
}

