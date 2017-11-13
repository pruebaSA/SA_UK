namespace System.Deployment.Application
{
    using System;

    internal class ServerInformation
    {
        private string _aspNetVersion;
        private string _poweredBy;
        private string _server;

        public string AspNetVersion
        {
            get => 
                this._aspNetVersion;
            set
            {
                this._aspNetVersion = value;
            }
        }

        public string PoweredBy
        {
            get => 
                this._poweredBy;
            set
            {
                this._poweredBy = value;
            }
        }

        public string Server
        {
            get => 
                this._server;
            set
            {
                this._server = value;
            }
        }
    }
}

