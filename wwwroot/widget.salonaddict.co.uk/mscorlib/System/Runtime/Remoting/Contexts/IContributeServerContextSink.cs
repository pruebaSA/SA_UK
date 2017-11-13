namespace System.Runtime.Remoting.Contexts
{
    using System.Runtime.InteropServices;
    using System.Runtime.Remoting.Messaging;
    using System.Security.Permissions;

    [ComVisible(true)]
    public interface IContributeServerContextSink
    {
        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.Infrastructure)]
        IMessageSink GetServerContextSink(IMessageSink nextSink);
    }
}

