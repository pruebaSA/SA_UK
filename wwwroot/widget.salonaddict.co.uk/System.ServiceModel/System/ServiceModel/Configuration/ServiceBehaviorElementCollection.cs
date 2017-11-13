namespace System.ServiceModel.Configuration
{
    using System;
    using System.Configuration;
    using System.ServiceModel;
    using System.Xml;

    [ConfigurationCollection(typeof(ServiceBehaviorElement), AddItemName="behavior")]
    public sealed class ServiceBehaviorElementCollection : ServiceModelEnhancedConfigurationElementCollection<ServiceBehaviorElement>
    {
        public ServiceBehaviorElementCollection() : base("behavior")
        {
        }

        protected override void DeserializeElement(XmlReader reader, bool serializeCollectionKey)
        {
            base.DeserializeElement(reader, serializeCollectionKey);
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            if (element == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("element");
            }
            ServiceBehaviorElement element2 = (ServiceBehaviorElement) element;
            return element2.Name;
        }

        protected override bool ThrowOnDuplicate =>
            false;
    }
}

