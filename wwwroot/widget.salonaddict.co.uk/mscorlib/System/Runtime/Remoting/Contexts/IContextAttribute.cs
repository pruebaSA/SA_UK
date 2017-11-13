namespace System.Runtime.Remoting.Contexts
{
    using System;
    using System.Runtime.InteropServices;
    using System.Runtime.Remoting.Activation;
    using System.Security.Permissions;

    [ComVisible(true)]
    public interface IContextAttribute
    {
        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.Infrastructure)]
        void GetPropertiesForNewContext(IConstructionCallMessage msg);
        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.Infrastructure)]
        bool IsContextOK(Context ctx, IConstructionCallMessage msg);
    }
}

