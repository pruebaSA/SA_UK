namespace System.ServiceModel.Configuration
{
    using System;
    using System.Configuration;
    using System.Globalization;
    using System.ServiceModel;

    [ConfigurationCollection(typeof(ServiceEndpointElement), AddItemName="endpoint")]
    public sealed class ServiceEndpointElementCollection : ServiceModelEnhancedConfigurationElementCollection<ServiceEndpointElement>
    {
        public ServiceEndpointElementCollection() : base("endpoint")
        {
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            if (element == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("element");
            }
            ServiceEndpointElement element2 = (ServiceEndpointElement) element;
            return string.Format(CultureInfo.InvariantCulture, "address:{0};bindingConfiguration{1};bindingName:{2};bindingNamespace:{3};bindingSectionName:{4};contractType:{5};", new object[] { element2.Address.ToString().ToUpperInvariant(), element2.BindingConfiguration, element2.BindingName, element2.BindingNamespace, element2.Binding, element2.Contract });
        }

        protected override bool ThrowOnDuplicate =>
            false;
    }
}

