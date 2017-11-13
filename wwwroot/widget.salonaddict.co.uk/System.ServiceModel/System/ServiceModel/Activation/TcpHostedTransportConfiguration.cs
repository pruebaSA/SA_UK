﻿namespace System.ServiceModel.Activation
{
    using System;
    using System.ServiceModel.Channels;

    internal sealed class TcpHostedTransportConfiguration : HostedTransportConfigurationBase
    {
        private HostedTcpTransportManager uniqueManager;

        public TcpHostedTransportConfiguration() : base(Uri.UriSchemeNetTcp)
        {
            string[] bindings = HostedTransportConfigurationManager.MetabaseSettings.GetBindings(Uri.UriSchemeNetTcp);
            for (int i = 0; i < bindings.Length; i++)
            {
                BaseUriWithWildcard baseAddress = BaseUriWithWildcard.CreateTcpUri(bindings[i]);
                if (i == 0)
                {
                    this.uniqueManager = new HostedTcpTransportManager(baseAddress);
                }
                base.ListenAddresses.Add(baseAddress);
                TcpChannelListener.StaticTransportManagerTable.RegisterUri(baseAddress.BaseAddress, baseAddress.HostNameComparisonMode, this.uniqueManager);
            }
        }

        internal TcpTransportManager TransportManager =>
            this.uniqueManager;
    }
}

