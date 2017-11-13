namespace System.Configuration.Internal
{
    using System;
    using System.Configuration;

    internal sealed class ConfigurationManagerInternal : IConfigurationManagerInternal
    {
        private ConfigurationManagerInternal()
        {
        }

        string IConfigurationManagerInternal.ApplicationConfigUri =>
            ClientConfigPaths.Current.ApplicationConfigUri;

        string IConfigurationManagerInternal.ExeLocalConfigDirectory =>
            ClientConfigPaths.Current.LocalConfigDirectory;

        string IConfigurationManagerInternal.ExeLocalConfigPath =>
            ClientConfigPaths.Current.LocalConfigFilename;

        string IConfigurationManagerInternal.ExeProductName =>
            ClientConfigPaths.Current.ProductName;

        string IConfigurationManagerInternal.ExeProductVersion =>
            ClientConfigPaths.Current.ProductVersion;

        string IConfigurationManagerInternal.ExeRoamingConfigDirectory =>
            ClientConfigPaths.Current.RoamingConfigDirectory;

        string IConfigurationManagerInternal.ExeRoamingConfigPath =>
            ClientConfigPaths.Current.RoamingConfigFilename;

        string IConfigurationManagerInternal.MachineConfigPath =>
            ClientConfigurationHost.MachineConfigFilePath;

        bool IConfigurationManagerInternal.SetConfigurationSystemInProgress =>
            ConfigurationManager.SetConfigurationSystemInProgress;

        bool IConfigurationManagerInternal.SupportsUserConfig =>
            ConfigurationManager.SupportsUserConfig;

        string IConfigurationManagerInternal.UserConfigFilename =>
            "user.config";
    }
}

