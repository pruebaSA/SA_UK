namespace System.Web.Hosting
{
    using System;
    using System.Runtime.InteropServices;
    using System.Security.Permissions;
    using System.Web;

    [ComImport, Guid("f11dc4c9-ddd1-4566-ad53-cf6f3a28fefe"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown), AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public interface IProcessPingCallback
    {
        void Respond();
    }
}

