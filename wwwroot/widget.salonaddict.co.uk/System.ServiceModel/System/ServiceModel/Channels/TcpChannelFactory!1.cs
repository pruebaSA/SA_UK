namespace System.ServiceModel.Channels
{
    using System;
    using System.ServiceModel;

    internal class TcpChannelFactory<TChannel> : ConnectionOrientedTransportChannelFactory<TChannel>, ITcpChannelFactorySettings, IConnectionOrientedTransportChannelFactorySettings, IConnectionOrientedTransportFactorySettings, ITransportFactorySettings, IDefaultCommunicationTimeouts, IConnectionOrientedConnectionSettings
    {
        private static TcpConnectionPoolRegistry connectionPoolRegistry;
        private TimeSpan leaseTimeout;

        static TcpChannelFactory()
        {
            TcpChannelFactory<TChannel>.connectionPoolRegistry = new TcpConnectionPoolRegistry();
        }

        public TcpChannelFactory(TcpTransportBindingElement bindingElement, BindingContext context) : base(bindingElement, context, bindingElement.ConnectionPoolSettings.GroupName, bindingElement.ConnectionPoolSettings.IdleTimeout, bindingElement.ConnectionPoolSettings.MaxOutboundConnectionsPerEndpoint, true)
        {
            this.leaseTimeout = bindingElement.ConnectionPoolSettings.LeaseTimeout;
        }

        internal override IConnectionInitiator GetConnectionInitiator() => 
            new BufferedConnectionInitiator(new SocketConnectionInitiator(base.ConnectionBufferSize), base.MaxOutputDelay, base.ConnectionBufferSize);

        internal override ConnectionPool GetConnectionPool() => 
            TcpChannelFactory<TChannel>.connectionPoolRegistry.Lookup(this);

        internal override void ReleaseConnectionPool(ConnectionPool pool, TimeSpan timeout)
        {
            TcpChannelFactory<TChannel>.connectionPoolRegistry.Release(pool, timeout);
        }

        public TimeSpan LeaseTimeout =>
            this.leaseTimeout;

        public override string Scheme =>
            Uri.UriSchemeNetTcp;
    }
}

