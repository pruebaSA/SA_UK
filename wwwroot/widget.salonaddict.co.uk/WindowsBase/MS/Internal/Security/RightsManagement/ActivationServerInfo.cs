namespace MS.Internal.Security.RightsManagement
{
    using System;
    using System.Runtime.InteropServices;
    using System.Security;

    [StructLayout(LayoutKind.Sequential), SecurityCritical(SecurityCriticalScope.Everything)]
    internal class ActivationServerInfo
    {
        public uint Version;
        [MarshalAs(UnmanagedType.LPWStr)]
        internal string PubKey = "";
        [MarshalAs(UnmanagedType.LPWStr)]
        internal string Url = "";
    }
}

