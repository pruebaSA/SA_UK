namespace System.Runtime.Remoting.Messaging
{
    using System.Collections;
    using System.Runtime.InteropServices;
    using System.Security.Permissions;

    [ComVisible(true)]
    public interface IMessage
    {
        IDictionary Properties { [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.Infrastructure)] get; }
    }
}

