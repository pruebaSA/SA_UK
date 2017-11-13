namespace System.Runtime.Remoting.Services
{
    using System;
    using System.Runtime.InteropServices;
    using System.Runtime.Remoting;
    using System.Security.Permissions;

    [ComVisible(true)]
    public interface ITrackingHandler
    {
        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.Infrastructure)]
        void DisconnectedObject(object obj);
        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.Infrastructure)]
        void MarshaledObject(object obj, ObjRef or);
        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.Infrastructure)]
        void UnmarshaledObject(object obj, ObjRef or);
    }
}

