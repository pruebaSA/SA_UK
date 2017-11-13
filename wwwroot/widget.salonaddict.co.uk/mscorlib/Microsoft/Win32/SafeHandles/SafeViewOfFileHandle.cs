namespace Microsoft.Win32.SafeHandles
{
    using Microsoft.Win32;
    using System;
    using System.Security.Permissions;

    internal sealed class SafeViewOfFileHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        [SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode=true)]
        internal SafeViewOfFileHandle() : base(true)
        {
        }

        [SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode=true)]
        internal SafeViewOfFileHandle(IntPtr handle, bool ownsHandle) : base(ownsHandle)
        {
            base.SetHandle(handle);
        }

        protected override bool ReleaseHandle()
        {
            if (Win32Native.UnmapViewOfFile(base.handle))
            {
                base.handle = IntPtr.Zero;
                return true;
            }
            return false;
        }
    }
}

