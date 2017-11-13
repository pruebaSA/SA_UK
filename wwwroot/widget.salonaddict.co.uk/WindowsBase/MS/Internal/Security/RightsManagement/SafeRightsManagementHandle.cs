namespace MS.Internal.Security.RightsManagement
{
    using System;
    using System.Runtime.InteropServices;
    using System.Security;

    [SecurityCritical(SecurityCriticalScope.Everything)]
    internal sealed class SafeRightsManagementHandle : SafeHandle
    {
        private static readonly SafeRightsManagementHandle _invalidHandle = new SafeRightsManagementHandle(0);

        private SafeRightsManagementHandle() : base(IntPtr.Zero, true)
        {
        }

        internal SafeRightsManagementHandle(uint handle) : base((IntPtr) handle, true)
        {
        }

        protected override bool ReleaseHandle()
        {
            int num = 0;
            if (!this.IsInvalid)
            {
                num = SafeNativeMethods.DRMCloseHandle((uint) ((int) base.handle));
                base.SetHandle(IntPtr.Zero);
            }
            return (num >= 0);
        }

        internal static SafeRightsManagementHandle InvalidHandle =>
            _invalidHandle;

        public override bool IsInvalid =>
            this.handle.Equals(IntPtr.Zero);
    }
}

