namespace System.Web.UI.WebControls
{
    using System;
    using System.ComponentModel;
    using System.Security.Permissions;
    using System.Web;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class ListViewSelectEventArgs : CancelEventArgs
    {
        private int _newSelectedIndex;

        public ListViewSelectEventArgs(int newSelectedIndex) : base(false)
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

