namespace System.Web.Compilation
{
    using System;
    using System.Globalization;
    using System.Resources;
    using System.Security.Permissions;
    using System.Web;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public interface IResourceProvider
    {
        object GetObject(string resourceKey, CultureInfo culture);

        IResourceReader ResourceReader { get; }
    }
}

