namespace System.Security.Policy
{
    using System;
    using System.Security.Cryptography.X509Certificates;

    internal class ParsedData
    {
        private string appName;
        private string appPublisher;
        private string authenticodedPublisher;
        private X509Certificate2 certificate;
        private bool disallowTrustOverride;
        private bool requestsShellIntegration;
        private string supportUrl;

        public string AppName
        {
            get => 
                this.appName;
            set
            {
                this.appName = value;
            }
        }

        public string AppPublisher
        {
            get => 
                this.appPublisher;
            set
            {
                this.appPublisher = value;
            }
        }

        public string AuthenticodedPublisher
        {
            get => 
                this.authenticodedPublisher;
            set
            {
                this.authenticodedPublisher = value;
            }
        }

        public X509Certificate2 Certificate
        {
            get => 
                this.certificate;
            set
            {
                this.certificate = value;
            }
        }

        public bool RequestsShellIntegration
        {
            get => 
                this.requestsShellIntegration;
            set
            {
                this.requestsShellIntegration = value;
            }
        }

        public string SupportUrl
        {
            get => 
                this.supportUrl;
            set
            {
                this.supportUrl = value;
            }
        }

        public bool UseManifestForTrust
        {
            get => 
                this.disallowTrustOverride;
            set
            {
                this.disallowTrustOverride = value;
            }
        }
    }
}

