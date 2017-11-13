namespace System.ServiceModel.Activation
{
    using System;

    public abstract class HostedTransportConfiguration
    {
        protected HostedTransportConfiguration()
        {
        }

        public abstract Uri[] GetBaseAddresses(string virtualPath);
    }
}

