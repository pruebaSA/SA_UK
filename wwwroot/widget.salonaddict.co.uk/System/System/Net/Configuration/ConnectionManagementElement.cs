namespace System.Net.Configuration
{
    using System;
    using System.Configuration;

    public sealed class ConnectionManagementElement : ConfigurationElement
    {
        private readonly ConfigurationProperty address;
        private readonly ConfigurationProperty maxconnection;
        private ConfigurationPropertyCollection properties;

        public ConnectionManagementElement()
        {
            this.properties = new ConfigurationPropertyCollection();
            this.address = new ConfigurationProperty("address", typeof(string), null, ConfigurationPropertyOptions.IsKey);
            this.maxconnection = new ConfigurationProperty("maxconnection", typeof(int), 1, ConfigurationPropertyOptions.None);
            this.properties.Add(this.address);
            this.properties.Add(this.maxconnection);
        }

        public ConnectionManagementElement(string address, int maxConnection) : this()
        {
            this.Address = address;
            this.MaxConnection = maxConnection;
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

        [ConfigurationProperty("maxconnection", IsRequired=true, DefaultValue=1)]
        public int MaxConnection
        {
            get => 
                ((int) base[this.maxconnection]);
            set
            {
                base[this.maxconnection] = value;
            }
        }

        protected override ConfigurationPropertyCollection Properties =>
            this.properties;
    }
}

