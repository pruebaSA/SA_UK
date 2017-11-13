namespace System.Web.UI.WebControls
{
    using System;
    using System.Security.Permissions;
    using System.Web;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class GridViewRowEventArgs : EventArgs
    {
        private GridViewRow _row;

        public GridViewRowEventArgs(GridViewRow row)
        {
            this._row = row;
        }

        public GridViewRow Row =>
            this._row;
    }
}

