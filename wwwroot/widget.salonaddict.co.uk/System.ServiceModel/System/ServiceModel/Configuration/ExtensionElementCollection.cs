namespace System.ServiceModel.Configuration
{
    using System;
    using System.Configuration;
    using System.ServiceModel;

    [ConfigurationCollection(typeof(ExtensionElement), CollectionType=ConfigurationElementCollectionType.BasicMap)]
    public class ExtensionElementCollection : ServiceModelConfigurationElementCollection<ExtensionElement>
    {
        public ExtensionElementCollection() : base(ConfigurationElementCollectionType.BasicMap, "add")
        {
        }

        protected override void BaseAdd(ConfigurationElement element)
        {
            this.EnforceUniqueElement((ExtensionElement) element);
            base.BaseAdd(element);
        }

        protected override void BaseAdd(int index, ConfigurationElement element)
        {
            this.EnforceUniqueElement((ExtensionElement) element);
            base.BaseAdd(index, element);
        }

        private void EnforceUniqueElement(ExtensionElement element)
        {
            Type type = Type.GetType(element.Type, false);
            foreach (ExtensionElement element2 in this)
            {
                if (element.Name.Equals(element2.Name, StringComparison.Ordinal))
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ConfigurationErrorsException(System.ServiceModel.SR.GetString("ConfigDuplicateExtensionName", new object[] { element.Name })));
                }
                if (type != null)
                {
                    if (type.Equals(Type.GetType(element2.Type, false)))
                    {
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ConfigurationErrorsException(System.ServiceModel.SR.GetString("ConfigDuplicateExtensionType", new object[] { element.Type })));
                    }
                }
                else if (element.Type.Equals(element2.Type, StringComparison.OrdinalIgnoreCase))
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ConfigurationErrorsException(System.ServiceModel.SR.GetString("ConfigDuplicateExtensionType", new object[] { element.Type })));
                }
            }
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            if (element == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("element");
            }
            ExtensionElement element2 = (ExtensionElement) element;
            return element2.Name;
        }

        protected override bool ThrowOnDuplicate =>
            true;
    }
}

