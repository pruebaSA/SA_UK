namespace System.Web.UI.WebControls
{
    using System;
    using System.ComponentModel;
    using System.Security.Permissions;
    using System.Web;

    [AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class GridViewSelectEventArgs : CancelEventArgs
    {
        private int _newSelectedIndex;

        public GridViewSelectEventArgs(int newSelectedIndex)
        {
            this._newSelectedIndex = newSelectedIndex;
        }

        public int NewSelectedIndex
        {
            get => 
                this._newSelectedIndex;
            set
            {
                this._newSelectedIndex = value;
            }
        }
    }
}

