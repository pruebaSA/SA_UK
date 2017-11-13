namespace Microsoft.Win32.SafeHandles
{
    using Microsoft.Win32;
    using System;
    using System.Security.Permissions;

    internal sealed class SafeFindHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        [SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode=true)]
        internal SafeFindHandle() : base(true)
        {
        }

        protected override bool ReleaseHandle() => 
            Win32Native.FindClose(base.handle);
    }
}

