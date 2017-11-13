namespace System.Web.UI.WebControls.WebParts
{
    using System;
    using System.Security.Permissions;
    using System.Web;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public interface IWebPart
    {
        string CatalogIconImageUrl { get; set; }

        string Description { get; set; }

        string Subtitle { get; }

        string Title { get; set; }

        string TitleIconImageUrl { get; set; }

        string TitleUrl { get; set; }
    }
}

