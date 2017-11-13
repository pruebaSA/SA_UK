namespace System.Web.UI.WebControls
{
    using System;
    using System.ComponentModel;
    using System.Security.Permissions;
    using System.Web;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class ListViewCancelEventArgs : CancelEventArgs
    {
        private ListViewCancelMode _cancelMode;
        private int _itemIndex;

        public ListViewCancelEventArgs(int itemIndex, ListViewCancelMode cancelMode) : base(false)
        {
            this._itemIndex = itemIndex;
            this._cancelMode = cancelMode;
        }

        public ListViewCancelMode CancelMode =>
            this._cancelMode;

        public int ItemIndex =>
            this._itemIndex;
    }
}

