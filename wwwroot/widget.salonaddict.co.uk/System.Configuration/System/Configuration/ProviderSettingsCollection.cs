namespace System.Configuration
{
    using System;
    using System.Reflection;

    [ConfigurationCollection(typeof(ProviderSettings))]
    public sealed class ProviderSettingsCollection : ConfigurationElementCollection
    {
        private static ConfigurationPropertyCollection _properties = new ConfigurationPropertyCollection();

        public ProviderSettingsCollection() : base(StringComparer.OrdinalIgnoreCase)
        {
        }

        public void Add(ProviderSettings provider)
        {
            if (provider != null)
            {
                provider.UpdatePropertyCollection();
                this.BaseAdd(provider);
            }
        }

        public void Clear()
        {
            base.BaseClear();
        }

        protected override ConfigurationElement CreateNewElement() => 
            new ProviderSettings();

        protected override object GetElementKey(ConfigurationElement element) => 
            ((ProviderSettings) element).Name;

        public void Remove(string name)
        {
            base.BaseRemove(name);
        }

        public ProviderSettings this[string key] =>
            ((ProviderSettings) base.BaseGet(key));

        public ProviderSettings this[int index]
        {
            get => 
                ((ProviderSettings) base.BaseGet(index));
            set
            {
                if (base.BaseGet(index) != null)
                {
                    base.BaseRemoveAt(index);
                }
                this.BaseAdd(index, value);
            }
        }

        protected internal override ConfigurationPropertyCollection Properties =>
            _properties;
    }
}

