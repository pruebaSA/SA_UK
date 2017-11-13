namespace System.Web.Hosting
{
    using System;
    using System.Runtime.InteropServices;
    using System.Security.Permissions;
    using System.Web;

    [ComImport, Guid("02998279-7175-4d59-aa5a-fb8e44d4ca9d"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown), AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public interface IAppManagerAppDomainFactory
    {
        [return: MarshalAs(UnmanagedType.Interface)]
        object Create([In, MarshalAs(UnmanagedType.BStr)] string appId, [In, MarshalAs(UnmanagedType.BStr)] string appPath);
        void Stop();
    }
}

