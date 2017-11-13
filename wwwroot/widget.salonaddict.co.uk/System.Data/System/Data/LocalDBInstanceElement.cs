namespace System.Data
{
    using System;
    using System.Configuration;

    internal sealed class LocalDBInstanceElement : ConfigurationElement
    {
        [ConfigurationProperty("name", IsRequired=true)]
        public string Name =>
            (base["name"] as string);

        [ConfigurationProperty("version", IsRequired=true)]
        public string Version =>
            (base["version"] as string);
    }
}

