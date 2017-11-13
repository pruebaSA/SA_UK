namespace System.Runtime.Remoting.Channels
{
    using System;
    using System.Security.Permissions;

    public interface ISecurableChannel
    {
        bool IsSecured { [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.Infrastructure)] get; [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.Infrastructure)] set; }
    }
}

