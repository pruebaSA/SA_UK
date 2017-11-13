namespace System.Web.ApplicationServices
{
    using System;
    using System.Security.Permissions;
    using System.Web;

    [AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class AuthenticatingEventArgs : EventArgs
    {
        private bool _authenticated;
        private bool _authenticationIsComplete;
        private string _customCredential;
        private string _password;
        private string _userName;

        private AuthenticatingEventArgs()
        {
        }

        internal AuthenticatingEventArgs(string username, string password, string customCredential)
        {
            this._authenticated = false;
            this._authenticationIsComplete = false;
            this._userName = username;
            this._password = password;
            this._customCredential = customCredential;
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

        public bool AuthenticationIsComplete
        {
            get => 
                this._authenticationIsComplete;
            set
            {
                this._authenticationIsComplete = value;
            }
        }

        public string CustomCredential =>
            this._customCredential;

        public string Password =>
            this._password;

        public string UserName =>
            this._userName;
    }
}

