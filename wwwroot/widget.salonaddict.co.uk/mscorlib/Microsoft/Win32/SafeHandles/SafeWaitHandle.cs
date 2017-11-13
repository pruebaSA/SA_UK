namespace Microsoft.Win32.SafeHandles
{
    using Microsoft.Win32;
    using System;
    using System.Runtime.ConstrainedExecution;
    using System.Security.Permissions;

    [SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode=true)]
    public sealed class SafeWaitHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        private SafeWaitHandle() : base(true)
        {
        }

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
        public SafeWaitHandle(IntPtr existingHandle, bool ownsHandle) : base(ownsHandle)
        {
            base.SetHandle(existingHandle);
        }

        protected override bool ReleaseHandle() => 
            Win32Native.CloseHandle(base.handle);
    }
}

