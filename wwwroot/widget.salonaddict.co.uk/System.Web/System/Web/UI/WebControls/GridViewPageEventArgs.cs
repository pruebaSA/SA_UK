namespace System.Web.UI.WebControls
{
    using System;
    using System.ComponentModel;
    using System.Security.Permissions;
    using System.Web;

    [AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class GridViewPageEventArgs : CancelEventArgs
    {
        private int _newPageIndex;

        public GridViewPageEventArgs(int newPageIndex)
        {
            this._newPageIndex = newPageIndex;
        }

        public int NewPageIndex
        {
            get => 
                this._newPageIndex;
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException("value");
                }
                this._newPageIndex = value;
            }
        }
    }
}

