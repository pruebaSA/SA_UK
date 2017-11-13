namespace System.Web.Security
{
    using System;
    using System.Security.Permissions;
    using System.Web;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public sealed class ValidatePasswordEventArgs : EventArgs
    {
        private bool _cancel;
        private Exception _failureInformation;
        private bool _isNewUser;
        private string _password;
        private string _userName;

        public ValidatePasswordEventArgs(string userName, string password, bool isNewUser)
        {
            this._userName = userName;
            this._password = password;
            this._isNewUser = isNewUser;
            this._cancel = false;
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

        public Exception FailureInformation
        {
            get => 
                this._failureInformation;
            set
            {
                this._failureInformation = value;
            }
        }

        public bool IsNewUser =>
            this._isNewUser;

        public string Password =>
            this._password;

        public string UserName =>
            this._userName;
    }
}

