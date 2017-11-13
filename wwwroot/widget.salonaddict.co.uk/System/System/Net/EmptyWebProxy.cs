namespace System.Net
{
    using System;

    [Serializable]
    internal sealed class EmptyWebProxy : IAutoWebProxy, IWebProxy
    {
        [NonSerialized]
        private ICredentials m_credentials;

        public Uri GetProxy(Uri uri) => 
            uri;

        public bool IsBypassed(Uri uri) => 
            true;

        ProxyChain IAutoWebProxy.GetProxies(Uri destination) => 
            new DirectProxy(destination);

        public ICredentials Credentials
        {
            get => 
                this.m_credentials;
            set
            {
                this.m_credentials = value;
            }
        }
    }
}

