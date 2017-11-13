namespace System.Runtime.Remoting.Channels
{
    using System.Collections;
    using System.Runtime.InteropServices;
    using System.Security.Permissions;

    [ComVisible(true)]
    public interface IChannelSinkBase
    {
        IDictionary Properties { [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.Infrastructure)] get; }
    }
}

