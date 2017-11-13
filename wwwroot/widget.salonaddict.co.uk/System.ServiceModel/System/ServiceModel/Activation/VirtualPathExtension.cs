namespace System.ServiceModel.Activation
{
    using System;
    using System.ServiceModel;
    using System.ServiceModel.Channels;

    public sealed class VirtualPathExtension : IExtension<ServiceHostBase>
    {
        private string virtualPath;

        internal VirtualPathExtension(string virtualPath)
        {
            this.virtualPath = virtualPath;
        }

        internal static void ApplyHostedContext(TransportChannelListener listener, BindingContext context)
        {
            VirtualPathExtension virtualPathExtension = context.BindingParameters.Find<VirtualPathExtension>();
            if (virtualPathExtension != null)
            {
                HostedMetadataProperty property = context.BindingParameters.Find<HostedMetadataProperty>();
                listener.ApplyHostedContext(virtualPathExtension, property != null);
            }
        }

        public void Attach(ServiceHostBase owner)
        {
        }

        public void Detach(ServiceHostBase owner)
        {
            throw new InvalidOperationException(System.ServiceModel.SR.GetString("Hosting_VirtualPathExtenstionCanNotBeDetached"));
        }

        public string VirtualPath =>
            this.virtualPath;
    }
}

