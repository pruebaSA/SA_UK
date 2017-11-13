namespace System.Web.UI.WebControls
{
    using System;
    using System.ComponentModel;
    using System.Security.Permissions;
    using System.Web;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class ListViewSortEventArgs : CancelEventArgs
    {
        private System.Web.UI.WebControls.SortDirection _sortDirection;
        private string _sortExpression;

        public ListViewSortEventArgs(string sortExpression, System.Web.UI.WebControls.SortDirection sortDirection) : base(false)
        {
            this._sortExpression = sortExpression;
            this._sortDirection = sortDirection;
        }

        public System.Web.UI.WebControls.SortDirection SortDirection
        {
            get => 
                this._sortDirection;
            set
            {
                this._sortDirection = value;
            }
        }

        public string SortExpression
        {
            get => 
                this._sortExpression;
            set
            {
                this._sortExpression = value;
            }
        }
    }
}

