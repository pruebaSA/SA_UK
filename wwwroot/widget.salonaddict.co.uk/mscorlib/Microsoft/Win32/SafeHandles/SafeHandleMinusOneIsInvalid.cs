namespace Microsoft.Win32.SafeHandles
{
    using System;
    using System.Runtime.ConstrainedExecution;
    using System.Runtime.InteropServices;
    using System.Security.Permissions;

    [SecurityPermission(SecurityAction.InheritanceDemand, UnmanagedCode=true), SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode=true)]
    public abstract class SafeHandleMinusOneIsInvalid : SafeHandle
    {
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
        protected SafeHandleMinusOneIsInvalid(bool ownsHandle) : base(new IntPtr(-1), ownsHandle)
        {
        }

        public override bool IsInvalid =>
            (base.handle == new IntPtr(-1));
    }
}

