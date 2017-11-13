namespace System.Runtime.Remoting.Services
{
    using System;
    using System.Runtime.InteropServices;
    using System.Runtime.Remoting;
    using System.Runtime.Remoting.Activation;
    using System.Runtime.Remoting.Messaging;
    using System.Runtime.Remoting.Proxies;
    using System.Security.Permissions;

    [ComVisible(true), SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.Infrastructure)]
    public sealed class EnterpriseServicesHelper
    {
        [ComVisible(true)]
        public static IConstructionReturnMessage CreateConstructionReturnMessage(IConstructionCallMessage ctorMsg, MarshalByRefObject retObj) => 
            new ConstructorReturnMessage(retObj, null, 0, null, ctorMsg);

        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
        public static void SwitchWrappers(RealProxy oldcp, RealProxy newcp)
        {
            object transparentProxy = oldcp.GetTransparentProxy();
            object tp = newcp.GetTransparentProxy();
            RemotingServices.GetServerContextForProxy(transparentProxy);
            RemotingServices.GetServerContextForProxy(tp);
            Marshal.InternalSwitchCCW(transparentProxy, tp);
        }

        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
        public static object WrapIUnknownWithComObject(IntPtr punk) => 
            Marshal.InternalWrapIUnknownWithComObject(punk);
    }
}

