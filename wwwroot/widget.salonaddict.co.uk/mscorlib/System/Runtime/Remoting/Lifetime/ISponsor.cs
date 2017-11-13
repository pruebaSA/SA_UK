namespace System.Runtime.Remoting.Lifetime
{
    using System;
    using System.Runtime.InteropServices;
    using System.Security.Permissions;

    [ComVisible(true)]
    public interface ISponsor
    {
        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.Infrastructure)]
        TimeSpan Renewal(ILease lease);
    }
}

