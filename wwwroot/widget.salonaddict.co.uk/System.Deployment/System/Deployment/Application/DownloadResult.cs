namespace System.Deployment.Application
{
    using System;

    internal class DownloadResult
    {
        private Uri _responseUri;
        private System.Deployment.Application.ServerInformation _serverInformation = new System.Deployment.Application.ServerInformation();

        public Uri ResponseUri
        {
            get => 
                this._responseUri;
            set
            {
                this._responseUri = value;
            }
        }

        public System.Deployment.Application.ServerInformation ServerInformation =>
            this._serverInformation;
    }
}

