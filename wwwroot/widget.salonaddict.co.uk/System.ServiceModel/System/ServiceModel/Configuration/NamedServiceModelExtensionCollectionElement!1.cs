namespace System.ServiceModel.Configuration
{
    using System;
    using System.Configuration;

    public abstract class NamedServiceModelExtensionCollectionElement<TServiceModelExtensionElement> : ServiceModelExtensionCollectionElement<TServiceModelExtensionElement> where TServiceModelExtensionElement: ServiceModelExtensionElement
    {
        private ConfigurationPropertyCollection properties;

        internal NamedServiceModelExtensionCollectionElement(string extensionCollectionName, string name) : base(extensionCollectionName)
        {
            if (!string.IsNullOrEmpty(name))
            {
                this.Name = name;
            }
        }

        [ConfigurationProperty("name", Options=ConfigurationPropertyOptions.IsKey | ConfigurationPropertyOptions.IsRequired), StringValidator(MinLength=1)]
        public string Name
        {
            get => 
                ((string) base["name"]);
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    value = string.Empty;
                }
                base["name"] = value;
                base.SetIsModified();
            }
        }

        protected override ConfigurationPropertyCollection Properties
        {
            get
            {
                if (this.properties == null)
                {
                    this.properties = base.Properties;
                    this.properties.Add(new ConfigurationProperty("name", typeof(string), null, null, new StringValidator(1), ConfigurationPropertyOptions.IsKey));
                }
                return this.properties;
            }
        }
    }
}

