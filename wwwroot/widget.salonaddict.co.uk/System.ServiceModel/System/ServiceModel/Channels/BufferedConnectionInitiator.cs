namespace System.ServiceModel.Channels
{
    using System;

    internal class BufferedConnectionInitiator : IConnectionInitiator
    {
        private IConnectionInitiator connectionInitiator;
        private TimeSpan flushTimeout;
        private int writeBufferSize;

        public BufferedConnectionInitiator(IConnectionInitiator connectionInitiator, TimeSpan flushTimeout, int writeBufferSize)
        {
            this.connectionInitiator = connectionInitiator;
            this.flushTimeout = flushTimeout;
            this.writeBufferSize = writeBufferSize;
        }

        public IAsyncResult BeginConnect(Uri uri, TimeSpan timeout, AsyncCallback callback, object state) => 
            this.connectionInitiator.BeginConnect(uri, timeout, callback, state);

        public IConnection Connect(Uri uri, TimeSpan timeout) => 
            new BufferedConnection(this.connectionInitiator.Connect(uri, timeout), this.flushTimeout, this.writeBufferSize);

        public IConnection EndConnect(IAsyncResult result) => 
            new BufferedConnection(this.connectionInitiator.EndConnect(result), this.flushTimeout, this.writeBufferSize);

        protected TimeSpan FlushTimeout =>
            this.flushTimeout;

        protected int WriteBufferSize =>
            this.writeBufferSize;
    }
}

