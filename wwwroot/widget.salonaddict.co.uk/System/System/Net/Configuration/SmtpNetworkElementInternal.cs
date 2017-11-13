namespace System.Net.Configuration
{
    using System;
    using System.Net;

    internal sealed class SmtpNetworkElementInternal
    {
        private string clientDomain;
        private NetworkCredential credential;
        private string host;
        private int port;
        private string targetname;

        internal SmtpNetworkElementInternal(SmtpNetworkElement element)
        {
            this.host = element.Host;
            this.port = element.Port;
            this.clientDomain = element.ClientDomain;
            this.targetname = element.TargetName;
            if (element.DefaultCredentials)
            {
                this.credential = (NetworkCredential) CredentialCache.DefaultCredentials;
            }
            else if ((element.UserName != null) && (element.UserName.Length > 0))
            {
                this.credential = new NetworkCredential(element.UserName, element.Password);
            }
        }

        internal string ClientDomain =>
            this.clientDomain;

        internal NetworkCredential Credential =>
            this.credential;

        internal string Host =>
            this.host;

        internal int Port =>
            this.port;

        internal string TargetName =>
            this.targetname;
    }
}

