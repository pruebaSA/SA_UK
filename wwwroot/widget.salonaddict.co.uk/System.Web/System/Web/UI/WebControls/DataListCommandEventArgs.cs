namespace System.Web.UI.WebControls
{
    using System;
    using System.Security.Permissions;
    using System.Web;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class DataListCommandEventArgs : CommandEventArgs
    {
        private object commandSource;
        private DataListItem item;

        public DataListCommandEventArgs(DataListItem item, object commandSource, CommandEventArgs originalArgs) : base(originalArgs)
        {
            this.item = item;
            this.commandSource = commandSource;
        }

        public object CommandSource =>
            this.commandSource;

        public DataListItem Item =>
            this.item;
    }
}

