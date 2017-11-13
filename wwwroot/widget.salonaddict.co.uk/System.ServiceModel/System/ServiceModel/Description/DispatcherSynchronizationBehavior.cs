namespace System.ServiceModel.Description
{
    using System;
    using System.Runtime.CompilerServices;
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Dispatcher;

    public class DispatcherSynchronizationBehavior : IEndpointBehavior
    {
        public DispatcherSynchronizationBehavior() : this(1)
        {
        }

        internal DispatcherSynchronizationBehavior(int maxPendingReceives)
        {
            this.MaxPendingReceives = maxPendingReceives;
        }

        void IEndpointBehavior.AddBindingParameters(ServiceEndpoint serviceEndpoint, BindingParameterCollection parameters)
        {
        }

        void IEndpointBehavior.ApplyClientBehavior(ServiceEndpoint serviceEndpoint, ClientRuntime behavior)
        {
        }

        void IEndpointBehavior.ApplyDispatchBehavior(ServiceEndpoint serviceEndpoint, EndpointDispatcher endpointDispatcher)
        {
            if (endpointDispatcher == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("endpointDispatcher");
            }
            endpointDispatcher.ChannelDispatcher.MaxPendingReceives = this.MaxPendingReceives;
        }

        void IEndpointBehavior.Validate(ServiceEndpoint serviceEndpoint)
        {
        }

        public int MaxPendingReceives { get; set; }
    }
}

