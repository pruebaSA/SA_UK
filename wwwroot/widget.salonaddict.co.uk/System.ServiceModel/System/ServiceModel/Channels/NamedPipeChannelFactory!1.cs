namespace System.ServiceModel.Channels
{
    using System;

    internal class NamedPipeChannelFactory<TChannel> : ConnectionOrientedTransportChannelFactory<TChannel>
    {
        private static NamedPipeConnectionPoolRegistry connectionPoolRegistry;

        static NamedPipeChannelFactory()
        {
            NamedPipeChannelFactory<TChannel>.connectionPoolRegistry = new NamedPipeConnectionPoolRegistry();
        }

        public NamedPipeChannelFactory(NamedPipeTransportBindingElement bindingElement, BindingContext context) : base(bindingElement, context, bindingElement.ConnectionPoolSettings.GroupName, bindingElement.ConnectionPoolSettings.IdleTimeout, bindingElement.ConnectionPoolSettings.MaxOutboundConnectionsPerEndpoint, false)
        {
        }

        internal override IConnectionInitiator GetConnectionInitiator() => 
            new BufferedConnectionInitiator(new PipeConnectionInitiator(false, base.ConnectionBufferSize), base.MaxOutputDelay, base.ConnectionBufferSize);

        internal override ConnectionPool GetConnectionPool() => 
            NamedPipeChannelFactory<TChannel>.connectionPoolRegistry.Lookup(this);

        internal override void ReleaseConnectionPool(ConnectionPool pool, TimeSpan timeout)
        {
            NamedPipeChannelFactory<TChannel>.connectionPoolRegistry.Release(pool, timeout);
        }

        protected override bool SupportsUpgrade(StreamUpgradeBindingElement upgradeBindingElement) => 
            !(upgradeBindingElement is SslStreamSecurityBindingElement);

        public override string Scheme =>
            Uri.UriSchemeNetPipe;
    }
}

