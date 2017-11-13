﻿namespace System.ServiceModel.Administration
{
    using System;
    using System.Collections.Generic;
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Description;

    internal sealed class EndpointInfo
    {
        private Uri address;
        private KeyedByTypeCollection<IEndpointBehavior> behaviors;
        private CustomBinding binding;
        private ContractDescription contract;
        private ServiceEndpoint endpoint;
        private AddressHeaderCollection headers;
        private EndpointIdentity identity;
        private string serviceName;

        internal EndpointInfo(ServiceEndpoint endpoint, string serviceName)
        {
            this.endpoint = endpoint;
            this.address = endpoint.Address.Uri;
            this.headers = endpoint.Address.Headers;
            this.identity = endpoint.Address.Identity;
            this.behaviors = endpoint.Behaviors;
            this.serviceName = serviceName;
            this.binding = (endpoint.Binding == null) ? new CustomBinding() : new CustomBinding(endpoint.Binding);
            this.contract = endpoint.Contract;
        }

        public Uri Address =>
            this.address;

        public KeyedByTypeCollection<IEndpointBehavior> Behaviors =>
            this.behaviors;

        public CustomBinding Binding =>
            this.binding;

        public ContractDescription Contract =>
            this.contract;

        public ServiceEndpoint Endpoint =>
            this.endpoint;

        public AddressHeaderCollection Headers =>
            this.headers;

        public EndpointIdentity Identity =>
            this.identity;

        public Uri ListenUri
        {
            get
            {
                if (null == this.Endpoint.ListenUri)
                {
                    return this.Address;
                }
                return this.Endpoint.ListenUri;
            }
        }

        public string Name =>
            (this.ServiceName + "." + this.Contract.Name + "@" + this.Address.AbsoluteUri);

        public string ServiceName =>
            this.serviceName;
    }
}

