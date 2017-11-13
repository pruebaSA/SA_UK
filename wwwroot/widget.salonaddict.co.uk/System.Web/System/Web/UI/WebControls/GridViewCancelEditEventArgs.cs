namespace System.Web.UI.WebControls
{
    using System;
    using System.ComponentModel;
    using System.Security.Permissions;
    using System.Web;

    [AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class GridViewCancelEditEventArgs : CancelEventArgs
    {
        private int _rowIndex;

        public GridViewCancelEditEventArgs(int rowIndex)
        {
            this._rowIndex = rowIndex;
        }

        public int RowIndex =>
            this._rowIndex;
    }
}

