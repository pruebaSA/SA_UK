namespace System.Web.UI.WebControls
{
    using System;
    using System.Security.Permissions;
    using System.Web;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class RepeaterCommandEventArgs : CommandEventArgs
    {
        private object commandSource;
        private RepeaterItem item;

        public RepeaterCommandEventArgs(RepeaterItem item, object commandSource, CommandEventArgs originalArgs) : base(originalArgs)
        {
            this.item = item;
            this.commandSource = commandSource;
        }

        public object CommandSource =>
            this.commandSource;

        public RepeaterItem Item =>
            this.item;
    }
}

