namespace System.Net.Configuration
{
    using System;
    using System.Configuration;

    public sealed class NetSectionGroup : ConfigurationSectionGroup
    {
        public static NetSectionGroup GetSectionGroup(System.Configuration.Configuration config)
        {
            if (config == null)
            {
                throw new ArgumentNullException("config");
            }
            return (config.GetSectionGroup("system.net") as NetSectionGroup);
        }

        [ConfigurationProperty("authenticationModules")]
        public AuthenticationModulesSection AuthenticationModules =>
            ((AuthenticationModulesSection) base.Sections["authenticationModules"]);

        [ConfigurationProperty("connectionManagement")]
        public ConnectionManagementSection ConnectionManagement =>
            ((ConnectionManagementSection) base.Sections["connectionManagement"]);

        [ConfigurationProperty("defaultProxy")]
        public DefaultProxySection DefaultProxy =>
            ((DefaultProxySection) base.Sections["defaultProxy"]);

        public MailSettingsSectionGroup MailSettings =>
            ((MailSettingsSectionGroup) base.SectionGroups["mailSettings"]);

        [ConfigurationProperty("requestCaching")]
        public RequestCachingSection RequestCaching =>
            ((RequestCachingSection) base.Sections["requestCaching"]);

        [ConfigurationProperty("settings")]
        public SettingsSection Settings =>
            ((SettingsSection) base.Sections["settings"]);

        [ConfigurationProperty("webRequestModules")]
        public WebRequestModulesSection WebRequestModules =>
            ((WebRequestModulesSection) base.Sections["webRequestModules"]);
    }
}

