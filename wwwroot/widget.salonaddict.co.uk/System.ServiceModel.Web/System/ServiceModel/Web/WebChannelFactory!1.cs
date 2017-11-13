namespace System.ServiceModel.Web
{
    using System;
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Description;

    public class WebChannelFactory<TChannel> : ChannelFactory<TChannel> where TChannel: class
    {
        public WebChannelFactory()
        {
        }

        public WebChannelFactory(Binding binding) : base(binding)
        {
        }

        public WebChannelFactory(ServiceEndpoint endpoint) : base(endpoint)
        {
        }

        public WebChannelFactory(string endpointConfigurationName) : base(endpointConfigurationName)
        {
        }

        public WebChannelFactory(Type channelType) : base(channelType)
        {
        }

        public WebChannelFactory(Uri remoteAddress) : this(WebChannelFactory<TChannel>.GetDefaultBinding(remoteAddress), remoteAddress)
        {
        }

        public WebChannelFactory(Binding binding, Uri remoteAddress) : base(binding, (remoteAddress != null) ? new EndpointAddress(remoteAddress, new AddressHeader[0]) : null)
        {
        }

        public WebChannelFactory(string endpointConfigurationName, Uri remoteAddress) : base(endpointConfigurationName, (remoteAddress != null) ? new EndpointAddress(remoteAddress, new AddressHeader[0]) : null)
        {
        }

        private static Binding GetDefaultBinding(Uri remoteAddress)
        {
            if ((remoteAddress == null) || ((remoteAddress.Scheme != Uri.UriSchemeHttp) && (remoteAddress.Scheme != Uri.UriSchemeHttps)))
            {
                return null;
            }
            if (remoteAddress.Scheme == Uri.UriSchemeHttp)
            {
                return new WebHttpBinding();
            }
            return new WebHttpBinding { Security = { 
                Mode = WebHttpSecurityMode.Transport,
                Transport = { ClientCredentialType = HttpClientCredentialType.None }
            } };
        }

        protected override void OnOpening()
        {
            if (base.Endpoint != null)
            {
                if ((base.Endpoint.Binding == null) && (base.Endpoint.Address != null))
                {
                    base.Endpoint.Binding = WebChannelFactory<TChannel>.GetDefaultBinding(base.Endpoint.Address.Uri);
                }
                WebServiceHost.SetRawContentTypeMapperIfNecessary(base.Endpoint, false);
                if (base.Endpoint.Behaviors.Find<WebHttpBehavior>() == null)
                {
                    base.Endpoint.Behaviors.Add(new WebHttpBehavior());
                }
                base.OnOpening();
            }
        }
    }
}

