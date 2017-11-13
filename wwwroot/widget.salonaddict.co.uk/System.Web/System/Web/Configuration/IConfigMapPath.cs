﻿namespace System.Web.Configuration
{
    using System;
    using System.Runtime.InteropServices;
    using System.Security.Permissions;
    using System.Web;

    [AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public interface IConfigMapPath
    {
        string GetAppPathForPath(string siteID, string path);
        void GetDefaultSiteNameAndID(out string siteName, out string siteID);
        string GetMachineConfigFilename();
        void GetPathConfigFilename(string siteID, string path, out string directory, out string baseName);
        string GetRootWebConfigFilename();
        string MapPath(string siteID, string path);
        void ResolveSiteArgument(string siteArgument, out string siteName, out string siteID);
    }
}

