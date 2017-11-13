namespace System.ServiceModel.Channels
{
    using System;

    internal abstract class TcpTransportManager : ConnectionOrientedTransportManager<TcpChannelListener>
    {
        internal TcpTransportManager()
        {
        }

        protected virtual bool IsCompatible(TcpChannelListener channelListener) => 
            base.IsCompatible(channelListener);

        internal override string Scheme =>
            Uri.UriSchemeNetTcp;
    }
}

