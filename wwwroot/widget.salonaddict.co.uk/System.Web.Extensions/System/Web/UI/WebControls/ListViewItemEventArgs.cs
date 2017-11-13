namespace System.Web.UI.WebControls
{
    using System;
    using System.Security.Permissions;
    using System.Web;

    [AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class ListViewItemEventArgs : EventArgs
    {
        private ListViewItem _item;

        public ListViewItemEventArgs(ListViewItem item)
        {
            this._item = item;
        }

        public ListViewItem Item =>
            this._item;
    }
}

