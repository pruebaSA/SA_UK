namespace System.Web.UI.WebControls
{
    using System;
    using System.Security.Permissions;
    using System.Web;
    using System.Web.UI;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class ListItemControlBuilder : ControlBuilder
    {
        public override bool AllowWhitespaceLiterals() => 
            false;

        public override bool HtmlDecodeLiterals() => 
            true;
    }
}

