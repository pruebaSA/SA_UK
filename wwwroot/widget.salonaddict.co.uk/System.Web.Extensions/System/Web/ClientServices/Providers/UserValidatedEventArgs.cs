namespace System.Web.ClientServices.Providers
{
    using System;

    public class UserValidatedEventArgs : EventArgs
    {
        private string _UserName;

        public UserValidatedEventArgs(string username)
        {
            this._UserName = username;
        }

        public string UserName =>
            this._UserName;
    }
}

