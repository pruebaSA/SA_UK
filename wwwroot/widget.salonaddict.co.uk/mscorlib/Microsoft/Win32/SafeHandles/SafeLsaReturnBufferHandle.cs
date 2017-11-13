namespace Microsoft.Win32.SafeHandles
{
    using Microsoft.Win32;
    using System;

    internal sealed class SafeLsaReturnBufferHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        private SafeLsaReturnBufferHandle() : base(true)
        {
        }

        internal SafeLsaReturnBufferHandle(IntPtr handle) : base(true)
        {
            base.SetHandle(handle);
        }

        protected override bool ReleaseHandle() => 
            (Win32Native.LsaFreeReturnBuffer(base.handle) >= 0);

        internal static SafeLsaReturnBufferHandle InvalidHandle =>
            new SafeLsaReturnBufferHandle(IntPtr.Zero);
    }
}

