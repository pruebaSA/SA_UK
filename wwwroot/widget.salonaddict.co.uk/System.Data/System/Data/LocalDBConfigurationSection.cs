namespace System.Data
{
    using System.Configuration;

    internal sealed class LocalDBConfigurationSection : ConfigurationSection
    {
        [ConfigurationProperty("localdbinstances", IsRequired=true)]
        public LocalDBInstancesCollection LocalDbInstances =>
            (((LocalDBInstancesCollection) base["localdbinstances"]) ?? new LocalDBInstancesCollection());
    }
}

