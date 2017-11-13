namespace System.Web.UI.WebControls
{
    using System;
    using System.Security.Permissions;
    using System.Web;

    [AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class DataListItemEventArgs : EventArgs
    {
        private DataListItem item;

        public DataListItemEventArgs(DataListItem item)
        {
            this.item = item;
        }

        public DataListItem Item =>
            this.item;
    }
}

