namespace System.Web.UI.WebControls
{
    using System;
    using System.Security.Permissions;
    using System.Web;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class SendMailErrorEventArgs : EventArgs
    {
        private System.Exception _exception;
        private bool _handled;

        public SendMailErrorEventArgs(System.Exception e)
        {
            this._exception = e;
        }

        public System.Exception Exception
        {
            get => 
                this._exception;
            set
            {
                this._exception = value;
            }
        }

        public bool Handled
        {
            get => 
                this._handled;
            set
            {
                this._handled = value;
            }
        }
    }
}

