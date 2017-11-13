namespace System.Web.UI.WebControls
{
    using System;
    using System.Security.Permissions;
    using System.Web;

    [AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class AuthenticateEventArgs : EventArgs
    {
        private bool _authenticated;

        public AuthenticateEventArgs() : this(false)
        {
        }

        public AuthenticateEventArgs(bool authenticated)
        {
            this._authenticated = authenticated;
        }

        public bool Authenticated
        {
            get => 
                this._authenticated;
            set
            {
                this._authenticated = value;
            }
        }
    }
}

