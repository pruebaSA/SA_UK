namespace System.Web.Hosting
{
    using System;
    using System.Runtime.InteropServices;
    using System.Security.Permissions;
    using System.Web;

    [ComImport, Guid("0ccd465e-3114-4ca3-ad50-cea561307e93"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public interface IProcessHost
    {
        void StartApplication([In, MarshalAs(UnmanagedType.LPWStr)] string appId, [In, MarshalAs(UnmanagedType.LPWStr)] string appPath, [MarshalAs(UnmanagedType.Interface)] out object runtimeInterface);
        void ShutdownApplication([In, MarshalAs(UnmanagedType.LPWStr)] string appId);
        void Shutdown();
        void EnumerateAppDomains([MarshalAs(UnmanagedType.Interface)] out IAppDomainInfoEnum appDomainInfoEnum);
    }
}

