namespace System.Web.UI.WebControls
{
    using System;
    using System.ComponentModel;
    using System.Security.Permissions;
    using System.Web;

    [AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class GridViewEditEventArgs : CancelEventArgs
    {
        private int _newEditIndex;

        public GridViewEditEventArgs(int newEditIndex)
        {
            this._newEditIndex = newEditIndex;
        }

        public int NewEditIndex
        {
            get => 
                this._newEditIndex;
            set
            {
                this._newEditIndex = value;
            }
        }
    }
}

