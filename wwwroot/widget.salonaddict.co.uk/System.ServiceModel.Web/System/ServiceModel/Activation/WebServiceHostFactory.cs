namespace System.ServiceModel.Activation
{
    using System;
    using System.ServiceModel;
    using System.ServiceModel.Web;

    public class WebServiceHostFactory : ServiceHostFactory
    {
        protected override ServiceHost CreateServiceHost(Type serviceType, Uri[] baseAddresses) => 
            new WebServiceHost(serviceType, baseAddresses);
    }
}

