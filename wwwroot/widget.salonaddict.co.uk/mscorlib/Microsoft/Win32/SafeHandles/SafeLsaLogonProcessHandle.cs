namespace Microsoft.Win32.SafeHandles
{
    using Microsoft.Win32;
    using System;

    internal sealed class SafeLsaLogonProcessHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        private SafeLsaLogonProcessHandle() : base(true)
        {
        }

        internal SafeLsaLogonProcessHandle(IntPtr handle) : base(true)
        {
            base.SetHandle(handle);
        }

        protected override bool ReleaseHandle() => 
            (Win32Native.LsaDeregisterLogonProcess(base.handle) >= 0);

        internal static SafeLsaLogonProcessHandle InvalidHandle =>
            new SafeLsaLogonProcessHandle(IntPtr.Zero);
    }
}

