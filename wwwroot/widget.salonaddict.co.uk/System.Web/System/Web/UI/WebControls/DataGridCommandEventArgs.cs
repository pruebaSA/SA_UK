namespace System.Web.UI.WebControls
{
    using System;
    using System.Security.Permissions;
    using System.Web;

    [AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class DataGridCommandEventArgs : CommandEventArgs
    {
        private object commandSource;
        private DataGridItem item;

        public DataGridCommandEventArgs(DataGridItem item, object commandSource, CommandEventArgs originalArgs) : base(originalArgs)
        {
            this.item = item;
            this.commandSource = commandSource;
        }

        public object CommandSource =>
            this.commandSource;

        public DataGridItem Item =>
            this.item;
    }
}

