namespace System.ServiceModel.Activation
{
    using System;
    using System.ServiceModel;

    public class WebScriptServiceHostFactory : ServiceHostFactory
    {
        protected override ServiceHost CreateServiceHost(Type serviceType, Uri[] baseAddresses) => 
            new WebScriptServiceHost(serviceType, baseAddresses);
    }
}

