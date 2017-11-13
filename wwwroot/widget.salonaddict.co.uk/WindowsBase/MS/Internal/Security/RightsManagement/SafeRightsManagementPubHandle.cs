namespace MS.Internal.Security.RightsManagement
{
    using System;
    using System.Runtime.InteropServices;
    using System.Security;

    [SecurityCritical(SecurityCriticalScope.Everything)]
    internal sealed class SafeRightsManagementPubHandle : SafeHandle
    {
        private static readonly SafeRightsManagementPubHandle _invalidHandle = new SafeRightsManagementPubHandle(0);

        private SafeRightsManagementPubHandle() : base(IntPtr.Zero, true)
        {
        }

        internal SafeRightsManagementPubHandle(uint handle) : base((IntPtr) handle, true)
        {
        }

        protected override bool ReleaseHandle()
        {
            int num = 0;
            if (!this.IsInvalid)
            {
                num = SafeNativeMethods.DRMClosePubHandle((uint) ((int) base.handle));
                base.SetHandle(IntPtr.Zero);
            }
            return (num >= 0);
        }

        internal static SafeRightsManagementPubHandle InvalidHandle =>
            _invalidHandle;

        public override bool IsInvalid =>
            this.handle.Equals(IntPtr.Zero);
    }
}

