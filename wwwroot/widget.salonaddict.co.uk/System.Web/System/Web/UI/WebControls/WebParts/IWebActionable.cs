namespace System.Web.UI.WebControls.WebParts
{
    using System.Security.Permissions;
    using System.Web;

    [AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public interface IWebActionable
    {
        WebPartVerbCollection Verbs { get; }
    }
}

