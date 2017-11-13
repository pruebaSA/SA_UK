namespace System.Web.Hosting
{
    using System;
    using System.Runtime.InteropServices;
    using System.Security.Permissions;
    using System.Web;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public interface IAdphManager
    {
        void StartAppDomainProtocolListenerChannel([In, MarshalAs(UnmanagedType.LPWStr)] string appId, [In, MarshalAs(UnmanagedType.LPWStr)] string protocolId, IListenerChannelCallback listenerChannelCallback);
        void StopAppDomainProtocol([In, MarshalAs(UnmanagedType.LPWStr)] string appId, [In, MarshalAs(UnmanagedType.LPWStr)] string protocolId, bool immediate);
        void StopAppDomainProtocolListenerChannel([In, MarshalAs(UnmanagedType.LPWStr)] string appId, [In, MarshalAs(UnmanagedType.LPWStr)] string protocolId, int listenerChannelId, bool immediate);
    }
}

