namespace System.ServiceModel.Channels
{
    using Microsoft.Win32.SafeHandles;
    using System;
    using System.Security;

    [SuppressUnmanagedCodeSecurity]
    internal sealed class SafeLibraryHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        internal SafeLibraryHandle() : base(true)
        {
        }

        protected override bool ReleaseHandle()
        {
            base.handle = IntPtr.Zero;
            return true;
        }
    }
}

