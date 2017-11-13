namespace System.Runtime.Remoting.Contexts
{
    using System;
    using System.Runtime.InteropServices;
    using System.Runtime.Remoting.Messaging;
    using System.Security.Permissions;

    [ComVisible(true)]
    public interface IContributeObjectSink
    {
        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.Infrastructure)]
        IMessageSink GetObjectSink(MarshalByRefObject obj, IMessageSink nextSink);
    }
}

