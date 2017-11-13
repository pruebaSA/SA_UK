namespace Microsoft.Win32.SafeHandles
{
    using Microsoft.Win32;
    using System;

    internal sealed class SafeLsaPolicyHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        private SafeLsaPolicyHandle() : base(true)
        {
        }

        internal SafeLsaPolicyHandle(IntPtr handle) : base(true)
        {
            base.SetHandle(handle);
        }

        protected override bool ReleaseHandle() => 
            (Win32Native.LsaClose(base.handle) == 0);

        internal static SafeLsaPolicyHandle InvalidHandle =>
            new SafeLsaPolicyHandle(IntPtr.Zero);
    }
}

