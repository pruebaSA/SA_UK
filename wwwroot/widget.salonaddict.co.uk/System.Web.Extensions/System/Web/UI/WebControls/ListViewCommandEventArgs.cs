namespace System.Web.UI.WebControls
{
    using System;
    using System.Security.Permissions;
    using System.Web;

    [AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class ListViewCommandEventArgs : CommandEventArgs
    {
        private object _commandSource;
        private ListViewItem _item;

        public ListViewCommandEventArgs(ListViewItem item, object commandSource, CommandEventArgs originalArgs) : base(originalArgs)
        {
            this._item = item;
            this._commandSource = commandSource;
        }

        public object CommandSource =>
            this._commandSource;

        public ListViewItem Item =>
            this._item;
    }
}

