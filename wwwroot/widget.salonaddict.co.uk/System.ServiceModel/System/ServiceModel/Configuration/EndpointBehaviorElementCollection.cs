namespace System.ServiceModel.Configuration
{
    using System;
    using System.Configuration;
    using System.ServiceModel;

    [ConfigurationCollection(typeof(EndpointBehaviorElement), AddItemName="behavior")]
    public sealed class EndpointBehaviorElementCollection : ServiceModelEnhancedConfigurationElementCollection<EndpointBehaviorElement>
    {
        public EndpointBehaviorElementCollection() : base("behavior")
        {
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            if (element == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("element");
            }
            EndpointBehaviorElement element2 = (EndpointBehaviorElement) element;
            return element2.Name;
        }

        protected override bool ThrowOnDuplicate =>
            false;
    }
}

