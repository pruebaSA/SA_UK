namespace Microsoft.Win32.SafeHandles
{
    using Microsoft.Win32;
    using System;

    internal sealed class SafeLocalAllocHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        private SafeLocalAllocHandle() : base(true)
        {
        }

        internal SafeLocalAllocHandle(IntPtr handle) : base(true)
        {
            base.SetHandle(handle);
        }

        protected override bool ReleaseHandle() => 
            (Win32Native.LocalFree(base.handle) == IntPtr.Zero);

        internal static SafeLocalAllocHandle InvalidHandle =>
            new SafeLocalAllocHandle(IntPtr.Zero);
    }
}

