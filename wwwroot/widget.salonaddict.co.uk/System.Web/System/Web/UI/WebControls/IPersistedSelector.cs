namespace System.Web.UI.WebControls
{
    using System;
    using System.Security.Permissions;
    using System.Web;

    [AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public interface IPersistedSelector
    {
        System.Web.UI.WebControls.DataKey DataKey { get; set; }
    }
}

