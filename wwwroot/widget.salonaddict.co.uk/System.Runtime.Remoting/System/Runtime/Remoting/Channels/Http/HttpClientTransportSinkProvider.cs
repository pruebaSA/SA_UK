namespace System.Runtime.Remoting.Channels.Http
{
    using System;
    using System.Runtime.Remoting.Channels;

    internal class HttpClientTransportSinkProvider : IClientChannelSinkProvider
    {
        private int _timeout;

        internal HttpClientTransportSinkProvider(int timeout)
        {
            this._timeout = timeout;
        }

        public IClientChannelSink CreateSink(IChannelSender channel, string url, object remoteChannelData) => 
            new HttpClientTransportSink((HttpClientChannel) channel, url) { ["timeout"] = this._timeout };

        public IClientChannelSinkProvider Next
        {
            get => 
                null;
            set
            {
                throw new NotSupportedException();
            }
        }
    }
}

