namespace System.Web.UI
{
    using System;
    using System.Security.Permissions;
    using System.Web;
    using System.Web.UI.WebControls;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public interface IStyleSheet
    {
        void CreateStyleRule(Style style, IUrlResolutionService urlResolver, string selector);
        void RegisterStyle(Style style, IUrlResolutionService urlResolver);
    }
}

