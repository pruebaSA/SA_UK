namespace System.Configuration
{
    using System;

    public class ProtectedProviderSettings : ConfigurationElement
    {
        private ConfigurationPropertyCollection _properties = new ConfigurationPropertyCollection();
        private readonly ConfigurationProperty _propProviders = new ConfigurationProperty(null, typeof(ProviderSettingsCollection), null, ConfigurationPropertyOptions.IsDefaultCollection);

        public ProtectedProviderSettings()
        {
            this._properties.Add(this._propProviders);
        }

        protected internal override ConfigurationPropertyCollection Properties =>
            this._properties;

        [ConfigurationProperty("", IsDefaultCollection=true, Options=ConfigurationPropertyOptions.IsDefaultCollection)]
        public ProviderSettingsCollection Providers =>
            ((ProviderSettingsCollection) base[this._propProviders]);
    }
}

