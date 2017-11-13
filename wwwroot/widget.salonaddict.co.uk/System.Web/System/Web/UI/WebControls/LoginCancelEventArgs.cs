namespace System.Web.UI.WebControls
{
    using System;
    using System.Security.Permissions;
    using System.Web;

    [AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class LoginCancelEventArgs : EventArgs
    {
        private bool _cancel;

        public LoginCancelEventArgs() : this(false)
        {
        }

        public LoginCancelEventArgs(bool cancel)
        {
            this._cancel = cancel;
        }

        public bool Cancel
        {
            get => 
                this._cancel;
            set
            {
                this._cancel = value;
            }
        }
    }
}

