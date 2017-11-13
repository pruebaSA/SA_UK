namespace System.Web.UI.WebControls
{
    using System;
    using System.Security.Permissions;
    using System.Web;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class BulletedListEventArgs : EventArgs
    {
        private int _index;

        public BulletedListEventArgs(int index)
        {
            this._index = index;
        }

        public int Index =>
            this._index;
    }
}

