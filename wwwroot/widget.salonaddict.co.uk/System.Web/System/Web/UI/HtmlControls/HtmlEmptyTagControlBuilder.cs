namespace System.Web.UI.HtmlControls
{
    using System;
    using System.Security.Permissions;
    using System.Web;
    using System.Web.UI;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public sealed class HtmlEmptyTagControlBuilder : ControlBuilder
    {
        public override bool HasBody() => 
            false;
    }
}

