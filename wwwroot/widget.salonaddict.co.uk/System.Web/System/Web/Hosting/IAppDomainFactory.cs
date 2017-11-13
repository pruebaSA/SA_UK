namespace System.Web.Hosting
{
    using System;
    using System.Runtime.InteropServices;
    using System.Security.Permissions;
    using System.Web;

    [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("e6e21054-a7dc-4378-877d-b7f4a2d7e8ba"), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public interface IAppDomainFactory
    {
        [return: MarshalAs(UnmanagedType.Interface)]
        object Create([In, MarshalAs(UnmanagedType.BStr)] string module, [In, MarshalAs(UnmanagedType.BStr)] string typeName, [In, MarshalAs(UnmanagedType.BStr)] string appId, [In, MarshalAs(UnmanagedType.BStr)] string appPath, [In, MarshalAs(UnmanagedType.BStr)] string strUrlOfAppOrigin, [In, MarshalAs(UnmanagedType.I4)] int iZone);
    }
}

