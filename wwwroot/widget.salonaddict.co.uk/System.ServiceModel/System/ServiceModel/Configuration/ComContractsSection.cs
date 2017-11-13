namespace System.ServiceModel.Configuration
{
    using System.Configuration;

    public sealed class ComContractsSection : ConfigurationSection
    {
        private ConfigurationPropertyCollection properties;

        internal static ComContractsSection GetSection() => 
            ((ComContractsSection) ConfigurationHelpers.GetSection(ConfigurationStrings.ComContractsSectionPath));

        [ConfigurationProperty("", Options=ConfigurationPropertyOptions.IsDefaultCollection)]
        public ComContractElementCollection ComContracts =>
            ((ComContractElementCollection) base[""]);

        protected override ConfigurationPropertyCollection Properties
        {
            get
            {
                if (this.properties == null)
                {
                    ConfigurationPropertyCollection propertys = new ConfigurationPropertyCollection {
                        new ConfigurationProperty("", typeof(ComContractElementCollection), null, null, null, ConfigurationPropertyOptions.IsDefaultCollection)
                    };
                    this.properties = propertys;
                }
                return this.properties;
            }
        }
    }
}

