namespace System.Net.Configuration
{
    using System;
    using System.Configuration;

    public sealed class BypassElement : ConfigurationElement
    {
        private readonly ConfigurationProperty address;
        private ConfigurationPropertyCollection properties;

        public BypassElement()
        {
            this.properties = new ConfigurationPropertyCollection();
            this.address = new ConfigurationProperty("address", typeof(string), null, ConfigurationPropertyOptions.IsKey);
            this.properties.Add(this.address);
        }

        public BypassElement(string address) : this()
        {
            this.Address = address;
        }

        [ConfigurationProperty("address", IsRequired=true, IsKey=true)]
        public string Address
        {
            get => 
                ((string) base[this.address]);
            set
            {
                base[this.address] = value;
            }
        }

        internal string Key =>
            this.Address;

        protected override ConfigurationPropertyCollection Properties =>
            this.properties;
    }
}

