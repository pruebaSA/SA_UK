namespace Microsoft.Win32.SafeHandles
{
    using Microsoft.Win32;
    using System;

    internal sealed class SafeProcessHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        private SafeProcessHandle() : base(true)
        {
        }

        internal SafeProcessHandle(IntPtr handle) : base(true)
        {
            base.SetHandle(handle);
        }

        protected override bool ReleaseHandle() => 
            Win32Native.CloseHandle(base.handle);

        internal static SafeProcessHandle InvalidHandle =>
            new SafeProcessHandle(IntPtr.Zero);
    }
}

