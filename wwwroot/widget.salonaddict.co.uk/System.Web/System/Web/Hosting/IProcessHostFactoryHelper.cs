namespace System.Web.Hosting
{
    using System;
    using System.Runtime.InteropServices;
    using System.Security.Permissions;
    using System.Web;

    [ComImport, Guid("02fd465d-5c5d-4b7e-95b6-82faa031b74a"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public interface IProcessHostFactoryHelper
    {
        [return: MarshalAs(UnmanagedType.Interface)]
        object GetProcessHost(IProcessHostSupportFunctions functions);
    }
}

