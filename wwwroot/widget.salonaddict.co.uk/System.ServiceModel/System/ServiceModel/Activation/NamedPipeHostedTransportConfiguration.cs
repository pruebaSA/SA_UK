namespace System.ServiceModel.Activation
{
    using System;
    using System.ServiceModel.Channels;

    internal sealed class NamedPipeHostedTransportConfiguration : HostedTransportConfigurationBase
    {
        private HostedNamedPipeTransportManager uniqueManager;

        public NamedPipeHostedTransportConfiguration() : base(Uri.UriSchemeNetPipe)
        {
            string[] bindings = HostedTransportConfigurationManager.MetabaseSettings.GetBindings(Uri.UriSchemeNetPipe);
            for (int i = 0; i < bindings.Length; i++)
            {
                BaseUriWithWildcard baseAddress = BaseUriWithWildcard.CreatePipeUri(bindings[i]);
                if (i == 0)
                {
                    this.uniqueManager = new HostedNamedPipeTransportManager(baseAddress);
                }
                base.ListenAddresses.Add(baseAddress);
                NamedPipeChannelListener.StaticTransportManagerTable.RegisterUri(baseAddress.BaseAddress, baseAddress.HostNameComparisonMode, this.uniqueManager);
            }
        }

        internal NamedPipeTransportManager TransportManager =>
            this.uniqueManager;
    }
}

