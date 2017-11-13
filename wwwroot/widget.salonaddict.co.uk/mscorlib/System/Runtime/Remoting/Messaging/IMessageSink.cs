namespace System.Runtime.Remoting.Messaging
{
    using System.Runtime.InteropServices;
    using System.Security.Permissions;

    [ComVisible(true)]
    public interface IMessageSink
    {
        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.Infrastructure)]
        IMessageCtrl AsyncProcessMessage(IMessage msg, IMessageSink replySink);
        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.Infrastructure)]
        IMessage SyncProcessMessage(IMessage msg);

        IMessageSink NextSink { [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.Infrastructure)] get; }
    }
}

