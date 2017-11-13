namespace System.Web.UI.WebControls
{
    using System;
    using System.ComponentModel;
    using System.Security.Permissions;
    using System.Web;

    [AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class ListViewEditEventArgs : CancelEventArgs
    {
        private int _newEditIndex;

        public ListViewEditEventArgs(int newEditIndex) : base(false)
        {
            this._newEditIndex = newEditIndex;
        }

        public int NewEditIndex =>
            this._newEditIndex;
    }
}

