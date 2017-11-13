namespace System.Web.UI.WebControls
{
    using System;
    using System.Security.Permissions;
    using System.Web;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class DetailsViewCommandEventArgs : CommandEventArgs
    {
        private object _commandSource;

        public DetailsViewCommandEventArgs(object commandSource, CommandEventArgs originalArgs) : base(originalArgs)
        {
            this._commandSource = commandSource;
        }

        public object CommandSource =>
            this._commandSource;
    }
}

