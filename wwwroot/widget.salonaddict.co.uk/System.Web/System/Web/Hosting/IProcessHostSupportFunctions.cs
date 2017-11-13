namespace System.Web.Hosting
{
    using System;
    using System.Runtime.InteropServices;
    using System.Security;
    using System.Security.Permissions;
    using System.Web;

    [ComImport, Guid("35f9c4c1-3800-4d17-99bc-018a62243687"), SuppressUnmanagedCodeSecurity, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public interface IProcessHostSupportFunctions
    {
        void GetApplicationProperties([In, MarshalAs(UnmanagedType.LPWStr)] string appId, out string virtualPath, out string physicalPath, out string siteName, out string siteId);
        void MapPath([In, MarshalAs(UnmanagedType.LPWStr)] string appId, [In, MarshalAs(UnmanagedType.LPWStr)] string virtualPath, out string physicalPath);
        [return: MarshalAs(UnmanagedType.SysInt)]
        IntPtr GetConfigToken([In, MarshalAs(UnmanagedType.LPWStr)] string appId);
        [return: MarshalAs(UnmanagedType.BStr)]
        string GetAppHostConfigFilename();
        [return: MarshalAs(UnmanagedType.BStr)]
        string GetRootWebConfigFilename();
        [return: MarshalAs(UnmanagedType.SysInt)]
        IntPtr GetNativeConfigurationSystem();
    }
}

