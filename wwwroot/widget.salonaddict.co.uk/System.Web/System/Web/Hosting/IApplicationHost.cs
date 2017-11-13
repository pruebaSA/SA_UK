namespace System.Web.Hosting
{
    using System;
    using System.Security.Permissions;
    using System.Web;
    using System.Web.Configuration;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public interface IApplicationHost
    {
        IConfigMapPathFactory GetConfigMapPathFactory();
        IntPtr GetConfigToken();
        string GetPhysicalPath();
        string GetSiteID();
        string GetSiteName();
        string GetVirtualPath();
        void MessageReceived();
    }
}

