namespace System.ServiceModel
{
    using System;
    using System.ServiceModel.Description;
    using System.ServiceModel.Web;

    internal class WebScriptServiceHost : ServiceHost
    {
        public WebScriptServiceHost(Type serviceType, params Uri[] baseAddresses) : base(serviceType, baseAddresses)
        {
        }

        protected override void OnOpening()
        {
            base.OnOpening();
            WebServiceHost.AddAutomaticWebHttpBindingEndpoints(this, base.ImplementedContracts, SR2.GetString(SR2.JsonWebScriptServiceHostOneServiceContract, new object[] { base.ImplementedContracts.Count }));
            foreach (ServiceEndpoint endpoint in base.Description.Endpoints)
            {
                if (((endpoint.Binding != null) && (endpoint.Binding.CreateBindingElements().Find<WebMessageEncodingBindingElement>() != null)) && (endpoint.Behaviors.Find<WebHttpBehavior>() == null))
                {
                    endpoint.Behaviors.Add(new WebScriptEnablingBehavior());
                }
            }
        }
    }
}

