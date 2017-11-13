namespace System.Web.UI
{
    using System.Collections;
    using System.Security.Permissions;
    using System.Web;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public interface IAutoFieldGenerator
    {
        ICollection GenerateFields(Control control);
    }
}

