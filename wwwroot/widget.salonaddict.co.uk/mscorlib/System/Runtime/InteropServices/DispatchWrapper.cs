namespace System.Runtime.InteropServices
{
    using System;
    using System.Security.Permissions;

    [Serializable, ComVisible(true)]
    public sealed class DispatchWrapper
    {
        private object m_WrappedObject;

        [SecurityPermission(SecurityAction.Demand, Flags=SecurityPermissionFlag.UnmanagedCode)]
        public DispatchWrapper(object obj)
        {
            if (obj != null)
            {
                Marshal.Release(Marshal.GetIDispatchForObject(obj));
            }
            this.m_WrappedObject = obj;
        }

        public object WrappedObject =>
            this.m_WrappedObject;
    }
}

