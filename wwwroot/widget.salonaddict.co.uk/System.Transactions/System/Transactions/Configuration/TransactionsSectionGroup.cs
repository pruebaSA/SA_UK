namespace System.Transactions.Configuration
{
    using System;
    using System.Configuration;

    public sealed class TransactionsSectionGroup : ConfigurationSectionGroup
    {
        public static TransactionsSectionGroup GetSectionGroup(System.Configuration.Configuration config)
        {
            if (config == null)
            {
                throw new ArgumentNullException("config");
            }
            return (TransactionsSectionGroup) config.GetSectionGroup("system.transactions");
        }

        [ConfigurationProperty("defaultSettings")]
        public DefaultSettingsSection DefaultSettings =>
            ((DefaultSettingsSection) base.Sections["defaultSettings"]);

        [ConfigurationProperty("machineSettings")]
        public MachineSettingsSection MachineSettings =>
            ((MachineSettingsSection) base.Sections["machineSettings"]);
    }
}

