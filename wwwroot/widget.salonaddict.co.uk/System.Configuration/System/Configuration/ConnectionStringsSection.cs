namespace System.Configuration
{
    using System;

    public sealed class ConnectionStringsSection : ConfigurationSection
    {
        private static readonly ConfigurationProperty _propConnectionStrings = new ConfigurationProperty(null, typeof(ConnectionStringSettingsCollection), null, ConfigurationPropertyOptions.IsDefaultCollection);
        private static ConfigurationPropertyCollection _properties = new ConfigurationPropertyCollection();

        static ConnectionStringsSection()
        {
            _properties.Add(_propConnectionStrings);
        }

        protected internal override object GetRuntimeObject()
        {
            this.SetReadOnly();
            return this;
        }

        [ConfigurationProperty("", Options=ConfigurationPropertyOptions.IsDefaultCollection)]
        public ConnectionStringSettingsCollection ConnectionStrings =>
            ((ConnectionStringSettingsCollection) base[_propConnectionStrings]);

        protected internal override ConfigurationPropertyCollection Properties =>
            _properties;
    }
}

