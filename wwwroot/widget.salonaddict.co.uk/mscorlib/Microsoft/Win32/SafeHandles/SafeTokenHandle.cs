namespace Microsoft.Win32.SafeHandles
{
    using Microsoft.Win32;
    using System;

    internal sealed class SafeTokenHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        private SafeTokenHandle() : base(true)
        {
        }

        internal SafeTokenHandle(IntPtr handle) : base(true)
        {
            base.SetHandle(handle);
        }

        protected override bool ReleaseHandle() => 
            Win32Native.CloseHandle(base.handle);

        internal static SafeTokenHandle InvalidHandle =>
            new SafeTokenHandle(IntPtr.Zero);
    }
}

