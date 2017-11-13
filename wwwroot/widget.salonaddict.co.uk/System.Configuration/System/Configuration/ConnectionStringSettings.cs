namespace System.Configuration
{
    using System;

    public sealed class ConnectionStringSettings : ConfigurationElement
    {
        private static readonly ConfigurationProperty _propConnectionString = new ConfigurationProperty("connectionString", typeof(string), "", ConfigurationPropertyOptions.IsRequired);
        private static ConfigurationPropertyCollection _properties = new ConfigurationPropertyCollection();
        private static readonly ConfigurationProperty _propName = new ConfigurationProperty("name", typeof(string), null, null, ConfigurationProperty.NonEmptyStringValidator, ConfigurationPropertyOptions.IsKey | ConfigurationPropertyOptions.IsRequired);
        private static readonly ConfigurationProperty _propProviderName = new ConfigurationProperty("providerName", typeof(string), string.Empty, ConfigurationPropertyOptions.None);

        static ConnectionStringSettings()
        {
            _properties.Add(_propName);
            _properties.Add(_propConnectionString);
            _properties.Add(_propProviderName);
        }

        public ConnectionStringSettings()
        {
        }

        public ConnectionStringSettings(string name, string connectionString) : this()
        {
            this.Name = name;
            this.ConnectionString = connectionString;
        }

        public ConnectionStringSettings(string name, string connectionString, string providerName) : this()
        {
            this.Name = name;
            this.ConnectionString = connectionString;
            this.ProviderName = providerName;
        }

        public override string ToString() => 
            this.ConnectionString;

        [ConfigurationProperty("connectionString", Options=ConfigurationPropertyOptions.IsRequired, DefaultValue="")]
        public string ConnectionString
        {
            get => 
                ((string) base[_propConnectionString]);
            set
            {
                base[_propConnectionString] = value;
            }
        }

        internal string Key =>
            this.Name;

        [ConfigurationProperty("name", Options=ConfigurationPropertyOptions.IsKey | ConfigurationPropertyOptions.IsRequired, DefaultValue="")]
        public string Name
        {
            get => 
                ((string) base[_propName]);
            set
            {
                base[_propName] = value;
            }
        }

        protected internal override ConfigurationPropertyCollection Properties =>
            _properties;

        [ConfigurationProperty("providerName", DefaultValue="System.Data.SqlClient")]
        public string ProviderName
        {
            get => 
                ((string) base[_propProviderName]);
            set
            {
                base[_propProviderName] = value;
            }
        }
    }
}

