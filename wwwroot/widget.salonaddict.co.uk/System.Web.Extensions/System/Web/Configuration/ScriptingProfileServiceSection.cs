namespace System.Web.Configuration
{
    using System;
    using System.Configuration;
    using System.Security.Permissions;
    using System.Web;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public sealed class ScriptingProfileServiceSection : ConfigurationSection
    {
        private static readonly ConfigurationProperty _propEnabled = new ConfigurationProperty("enabled", typeof(bool), false);
        private static readonly ConfigurationProperty _propEnableForReading = new ConfigurationProperty("readAccessProperties", typeof(string[]), new string[0], new StringArrayConverter(), null, ConfigurationPropertyOptions.None);
        private static readonly ConfigurationProperty _propEnableForWriting = new ConfigurationProperty("writeAccessProperties", typeof(string[]), new string[0], new StringArrayConverter(), null, ConfigurationPropertyOptions.None);
        private static ConfigurationPropertyCollection _properties = BuildProperties();

        private static ConfigurationPropertyCollection BuildProperties() => 
            new ConfigurationPropertyCollection { 
                _propEnabled,
                _propEnableForReading,
                _propEnableForWriting
            };

        internal static ScriptingProfileServiceSection GetConfigurationSection() => 
            ((ScriptingProfileServiceSection) WebConfigurationManager.GetWebApplicationSection("system.web.extensions/scripting/webServices/profileService"));

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

        [ConfigurationProperty("readAccessProperties", DefaultValue=null)]
        public string[] ReadAccessProperties
        {
            get
            {
                string[] strArray = (string[]) base[_propEnableForReading];
                if (strArray != null)
                {
                    return (string[]) strArray.Clone();
                }
                return null;
            }
            set
            {
                if (value != null)
                {
                    value = (string[]) value.Clone();
                }
                base[_propEnableForReading] = value;
            }
        }

        [ConfigurationProperty("writeAccessProperties", DefaultValue=null)]
        public string[] WriteAccessProperties
        {
            get
            {
                string[] strArray = (string[]) base[_propEnableForWriting];
                if (strArray != null)
                {
                    return (string[]) strArray.Clone();
                }
                return null;
            }
            set
            {
                if (value != null)
                {
                    value = (string[]) value.Clone();
                }
                base[_propEnableForWriting] = value;
            }
        }
    }
}

