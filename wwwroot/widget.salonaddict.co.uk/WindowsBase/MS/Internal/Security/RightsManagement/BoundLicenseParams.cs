namespace MS.Internal.Security.RightsManagement
{
    using System;
    using System.Runtime.InteropServices;
    using System.Security;

    [StructLayout(LayoutKind.Sequential), SecurityCritical(SecurityCriticalScope.Everything)]
    internal class BoundLicenseParams
    {
        internal uint uVersion;
        internal uint hEnablingPrincipal;
        internal uint hSecureStore;
        [MarshalAs(UnmanagedType.LPWStr)]
        public string wszRightsRequested;
        [MarshalAs(UnmanagedType.LPWStr)]
        public string wszRightsGroup;
        internal uint DRMIDuVersion;
        [MarshalAs(UnmanagedType.LPWStr)]
        public string DRMIDIdType;
        [MarshalAs(UnmanagedType.LPWStr)]
        public string DRMIDId;
        internal uint cAuthenticatorCount;
        internal IntPtr rghAuthenticators = IntPtr.Zero;
        [MarshalAs(UnmanagedType.LPWStr)]
        public string wszDefaultEnablingPrincipalCredentials;
        internal uint dwFlags;
    }
}

