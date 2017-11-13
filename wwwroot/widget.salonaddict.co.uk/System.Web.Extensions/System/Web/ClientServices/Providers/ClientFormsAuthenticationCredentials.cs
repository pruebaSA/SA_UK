namespace System.Web.ClientServices.Providers
{
    using System;

    public class ClientFormsAuthenticationCredentials
    {
        private string _Password;
        private bool _RememberMe;
        private string _UserName;

        public ClientFormsAuthenticationCredentials(string username, string password, bool rememberMe)
        {
            this._UserName = username;
            this._Password = password;
            this._RememberMe = rememberMe;
        }

        public string Password
        {
            get => 
                this._Password;
            set
            {
                this._Password = value;
            }
        }

        public bool RememberMe
        {
            get => 
                this._RememberMe;
            set
            {
                this._RememberMe = value;
            }
        }

        public string UserName
        {
            get => 
                this._UserName;
            set
            {
                this._UserName = value;
            }
        }
    }
}

