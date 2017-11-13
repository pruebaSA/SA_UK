namespace System.Runtime.Remoting.Contexts
{
    using System.Runtime.InteropServices;
    using System.Security.Permissions;

    [ComVisible(true)]
    public interface IContributeDynamicSink
    {
        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.Infrastructure)]
        IDynamicMessageSink GetDynamicSink();
    }
}

