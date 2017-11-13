namespace System.Web
{
    using System.Security.Permissions;
    using System.Web.Configuration;

    [AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class HttpBrowserCapabilities : HttpCapabilitiesBase
    {
    }
}

