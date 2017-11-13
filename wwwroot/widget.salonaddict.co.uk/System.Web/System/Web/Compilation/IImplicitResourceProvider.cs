namespace System.Web.Compilation
{
    using System;
    using System.Collections;
    using System.Globalization;
    using System.Security.Permissions;
    using System.Web;

    [AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public interface IImplicitResourceProvider
    {
        ICollection GetImplicitResourceKeys(string keyPrefix);
        object GetObject(ImplicitResourceKey key, CultureInfo culture);
    }
}

