namespace System.Web.Hosting
{
    using System;
    using System.Runtime.InteropServices;
    using System.Security.Permissions;
    using System.Web;

    [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("9d98b251-453e-44f6-9cec-8b5aed970129"), AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public interface IProcessHostIdleAndHealthCheck
    {
        [return: MarshalAs(UnmanagedType.Bool)]
        bool IsIdle();
        void Ping(IProcessPingCallback callback);
    }
}

