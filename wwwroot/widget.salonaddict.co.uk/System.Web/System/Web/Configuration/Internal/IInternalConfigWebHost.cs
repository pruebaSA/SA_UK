namespace System.Web.Configuration.Internal
{
    using System;
    using System.Runtime.InteropServices;
    using System.Security.Permissions;
    using System.Web;

    [ComVisible(false), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public interface IInternalConfigWebHost
    {
        string GetConfigPathFromSiteIDAndVPath(string siteID, string vpath);
        void GetSiteIDAndVPathFromConfigPath(string configPath, out string siteID, out string vpath);
    }
}

