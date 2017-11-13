namespace System.Web.Configuration
{
    using System;
    using System.Configuration;
    using System.Security.Permissions;
    using System.Web;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public sealed class ScriptingAuthenticationServiceSection : ConfigurationSection
    {
        private static readonly ConfigurationProperty _propEnabled = new ConfigurationProperty("enabled", typeof(bool), false);
        private static ConfigurationPropertyCollection _properties = BuildProperties();
        private static readonly ConfigurationProperty _propRequireSSL = new ConfigurationProperty("requireSSL", typeof(bool), false);

        private static ConfigurationPropertyCollection BuildProperties() => 
            new ConfigurationPropertyCollection { 
                _propEnabled,
                _propRequireSSL
            };

        internal static ScriptingAuthenticationServiceSection GetConfigurationSection() => 
            ((ScriptingAuthenticationServiceSection) WebConfigurationManager.GetWebApplicationSection("system.web.extensions/scripting/webServices/authenticationService"));

        [ConfigurationProperty("enabled", DefaultValue=false)]
        public bool Enabled
        {
            get => 
                ((bool) base[_propEnabled]);
            set
            {
                base[_propEnabled] = value;
            }
        }

        protected override ConfigurationPropertyCollection Properties =>
            _properties;

        [ConfigurationProperty("requireSSL", DefaultValue=false)]
        public bool RequireSSL
        {
            get => 
                ((bool) base[_propRequireSSL]);
            set
            {
                base[_propRequireSSL] = value;
            }
        }
    }
}

