namespace System.Runtime.Remoting.Contexts
{
    using System;
    using System.Runtime.InteropServices;
    using System.Runtime.Remoting.Messaging;
    using System.Security.Permissions;

    [ComVisible(true)]
    public interface IDynamicMessageSink
    {
        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.Infrastructure)]
        void ProcessMessageFinish(IMessage replyMsg, bool bCliSide, bool bAsync);
        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.Infrastructure)]
        void ProcessMessageStart(IMessage reqMsg, bool bCliSide, bool bAsync);
    }
}

