namespace System.Web.ApplicationServices
{
    using System;
    using System.Security.Permissions;
    using System.Web;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class CreatingCookieEventArgs : EventArgs
    {
        private bool _cookieIsSet;
        private string _customCredential;
        private bool _isPersistent;
        private string _password;
        private string _userName;

        private CreatingCookieEventArgs()
        {
        }

        internal CreatingCookieEventArgs(string username, string password, bool isPersistent, string customCredential)
        {
            this._cookieIsSet = false;
            this._userName = username;
            this._password = password;
            this._password = password;
            this._isPersistent = isPersistent;
            this._customCredential = customCredential;
        }

        public bool CookieIsSet
        {
            get => 
                this._cookieIsSet;
            set
            {
                this._cookieIsSet = value;
            }
        }

        public string CustomCredential =>
            this._customCredential;

        public bool IsPersistent =>
            this._isPersistent;

        public string Password =>
            this._password;

        public string UserName =>
            this._userName;
    }
}

