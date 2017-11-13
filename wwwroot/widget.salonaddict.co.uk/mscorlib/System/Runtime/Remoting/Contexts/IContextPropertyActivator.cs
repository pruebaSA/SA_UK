namespace System.Runtime.Remoting.Contexts
{
    using System;
    using System.Runtime.InteropServices;
    using System.Runtime.Remoting.Activation;
    using System.Security.Permissions;

    [ComVisible(true)]
    public interface IContextPropertyActivator
    {
        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.Infrastructure)]
        void CollectFromClientContext(IConstructionCallMessage msg);
        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.Infrastructure)]
        void CollectFromServerContext(IConstructionReturnMessage msg);
        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.Infrastructure)]
        bool DeliverClientContextToServerContext(IConstructionCallMessage msg);
        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.Infrastructure)]
        bool DeliverServerContextToClientContext(IConstructionReturnMessage msg);
        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.Infrastructure)]
        bool IsOKToActivate(IConstructionCallMessage msg);
    }
}

