namespace System.Web.UI.WebControls
{
    using System.Security.Permissions;
    using System.Web;
    using System.Web.UI;

    [AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public interface IPostBackContainer
    {
        PostBackOptions GetPostBackOptions(IButtonControl buttonControl);
    }
}

