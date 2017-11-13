namespace System.Web.UI.WebControls.Adapters
{
    using System;
    using System.Security.Permissions;
    using System.Web;
    using System.Web.UI;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class HideDisabledControlAdapter : WebControlAdapter
    {
        protected internal override void Render(HtmlTextWriter writer)
        {
            if (base.Control.Enabled)
            {
                base.Control.Render(writer);
            }
        }
    }
}

