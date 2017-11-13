namespace System.Web.UI
{
    using System;
    using System.Security.Permissions;
    using System.Web;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public sealed class DataSourceControlBuilder : ControlBuilder
    {
        public override bool AllowWhitespaceLiterals() => 
            false;
    }
}

