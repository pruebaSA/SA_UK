namespace System.Web.UI.WebControls
{
    using System;
    using System.Security.Permissions;
    using System.Web;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class DataGridItemEventArgs : EventArgs
    {
        private DataGridItem item;

        public DataGridItemEventArgs(DataGridItem item)
        {
            this.item = item;
        }

        public DataGridItem Item =>
            this.item;
    }
}

