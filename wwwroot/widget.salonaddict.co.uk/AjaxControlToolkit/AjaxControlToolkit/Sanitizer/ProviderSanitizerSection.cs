namespace AjaxControlToolkit.Sanitizer
{
    using System;
    using System.Configuration;

    public class ProviderSanitizerSection : ConfigurationSection
    {
        private readonly ConfigurationProperty defaultProvider = new ConfigurationProperty("defaultProvider", typeof(string), null);
        private ConfigurationPropertyCollection properties = new ConfigurationPropertyCollection();
        private readonly ConfigurationProperty providers = new ConfigurationProperty("providers", typeof(ProviderSettingsCollection), null);

        public ProviderSanitizerSection()
        {
            this.properties.Add(this.providers);
            this.properties.Add(this.defaultProvider);
        }

        [ConfigurationProperty("defaultProvider")]
        public string DefaultProvider
        {
            get => 
                ((string) base[this.defaultProvider]);
            set
            {
                base[this.defaultProvider] = value;
            }
        }

        protected override ConfigurationPropertyCollection Properties =>
            this.properties;

        [ConfigurationProperty("providers")]
        public ProviderSettingsCollection Providers =>
            ((ProviderSettingsCollection) base[this.providers]);
    }
}

