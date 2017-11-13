namespace System.ServiceModel.Configuration
{
    using System.Configuration;

    public sealed class HostElement : ConfigurationElement
    {
        private ConfigurationPropertyCollection properties;

        [ConfigurationProperty("baseAddresses", Options=ConfigurationPropertyOptions.None)]
        public BaseAddressElementCollection BaseAddresses =>
            ((BaseAddressElementCollection) base["baseAddresses"]);

        protected override ConfigurationPropertyCollection Properties
        {
            get
            {
                if (this.properties == null)
                {
                    ConfigurationPropertyCollection propertys = new ConfigurationPropertyCollection {
                        new ConfigurationProperty("baseAddresses", typeof(BaseAddressElementCollection), null, null, null, ConfigurationPropertyOptions.None),
                        new ConfigurationProperty("timeouts", typeof(HostTimeoutsElement), null, null, null, ConfigurationPropertyOptions.None)
                    };
                    this.properties = propertys;
                }
                return this.properties;
            }
        }

        [ConfigurationProperty("timeouts", Options=ConfigurationPropertyOptions.None)]
        public HostTimeoutsElement Timeouts =>
            ((HostTimeoutsElement) base["timeouts"]);
    }
}

