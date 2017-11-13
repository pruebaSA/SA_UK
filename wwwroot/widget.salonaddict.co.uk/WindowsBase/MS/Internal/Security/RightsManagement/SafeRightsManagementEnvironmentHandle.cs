namespace MS.Internal.Security.RightsManagement
{
    using System;
    using System.Runtime.InteropServices;
    using System.Security;

    [SecurityCritical(SecurityCriticalScope.Everything)]
    internal sealed class SafeRightsManagementEnvironmentHandle : SafeHandle
    {
        private SafeRightsManagementEnvironmentHandle() : base(IntPtr.Zero, true)
        {
        }

        internal SafeRightsManagementEnvironmentHandle(uint handle) : base((IntPtr) handle, true)
        {
        }

        protected override bool ReleaseHandle()
        {
            int num = 0;
            if (!this.IsInvalid)
            {
                num = SafeNativeMethods.DRMCloseEnvironmentHandle((uint) ((int) base.handle));
                base.SetHandle(IntPtr.Zero);
            }
            return (num >= 0);
        }

        public override bool IsInvalid =>
            this.handle.Equals(IntPtr.Zero);
    }
}

