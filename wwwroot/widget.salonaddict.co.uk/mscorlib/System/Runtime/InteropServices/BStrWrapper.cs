namespace System.Runtime.InteropServices
{
    using System;
    using System.Security.Permissions;

    [Serializable, ComVisible(true)]
    public sealed class BStrWrapper
    {
        private string m_WrappedObject;

        [SecurityPermission(SecurityAction.Demand, Flags=SecurityPermissionFlag.UnmanagedCode)]
        public BStrWrapper(string value)
        {
            this.m_WrappedObject = value;
        }

        public string WrappedObject =>
            this.m_WrappedObject;
    }
}

