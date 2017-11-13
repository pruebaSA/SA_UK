namespace System.Web.UI.WebControls.WebParts
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Security.Permissions;
    using System.Web;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public interface ITransformerConfigurationControl
    {
        event EventHandler Cancelled;

        event EventHandler Succeeded;
    }
}

