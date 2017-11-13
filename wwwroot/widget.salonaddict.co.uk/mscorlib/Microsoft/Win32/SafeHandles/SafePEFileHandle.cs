namespace Microsoft.Win32.SafeHandles
{
    using System;
    using System.Security.Policy;

    internal sealed class SafePEFileHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        private SafePEFileHandle(IntPtr handle) : base(true)
        {
            base.SetHandle(handle);
        }

        protected override bool ReleaseHandle()
        {
            Hash._ReleasePEFile(base.handle);
            return true;
        }

        internal static SafePEFileHandle InvalidHandle =>
            new SafePEFileHandle(IntPtr.Zero);
    }
}

