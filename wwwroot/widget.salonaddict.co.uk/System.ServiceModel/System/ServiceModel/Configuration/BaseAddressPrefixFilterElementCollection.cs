namespace System.ServiceModel.Configuration
{
    using System;
    using System.Configuration;
    using System.ServiceModel;

    [ConfigurationCollection(typeof(BaseAddressPrefixFilterElement))]
    public sealed class BaseAddressPrefixFilterElementCollection : ServiceModelConfigurationElementCollection<BaseAddressPrefixFilterElement>
    {
        public BaseAddressPrefixFilterElementCollection() : base(ConfigurationElementCollectionType.AddRemoveClearMap, "add")
        {
        }

        protected override ConfigurationElement CreateNewElement() => 
            new BaseAddressPrefixFilterElement();

        protected override object GetElementKey(ConfigurationElement element)
        {
            if (element == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("element");
            }
            BaseAddressPrefixFilterElement element2 = (BaseAddressPrefixFilterElement) element;
            return element2.Prefix;
        }

        protected override bool ThrowOnDuplicate =>
            true;
    }
}

