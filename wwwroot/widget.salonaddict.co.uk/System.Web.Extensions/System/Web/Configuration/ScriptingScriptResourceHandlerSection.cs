namespace System.Web.Configuration
{
    using System;
    using System.Configuration;
    using System.Runtime.CompilerServices;
    using System.Security.Permissions;
    using System.Web;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public sealed class ScriptingScriptResourceHandlerSection : ConfigurationSection
    {
        private static readonly ConfigurationProperty _propEnableCaching = new ConfigurationProperty("enableCaching", typeof(bool), true, ConfigurationPropertyOptions.None);
        private static readonly ConfigurationProperty _propEnableCompression = new ConfigurationProperty("enableCompression", typeof(bool), true, ConfigurationPropertyOptions.None);
        private static ConfigurationPropertyCollection _properties = BuildProperties();

        private static ConfigurationPropertyCollection BuildProperties() => 
            new ConfigurationPropertyCollection { 
                _propEnableCaching,
                _propEnableCompression
            };

        [ConfigurationProperty("enableCaching", DefaultValue=true)]
        public bool EnableCaching
        {
            get => 
                ((bool) base[_propEnableCaching]);
            set
            {
                base[_propEnableCaching] = value;
            }
        }

        [ConfigurationProperty("enableCompression", DefaultValue=true)]
        public bool EnableCompression
        {
            get => 
                ((bool) base[_propEnableCompression]);
            set
            {
                base[_propEnableCompression] = value;
            }
        }

        protected override ConfigurationPropertyCollection Properties =>
            _properties;

        internal static class ApplicationSettings
        {
            private static bool s_enableCaching;
            private static bool s_enableCompression;
            private static volatile bool s_sectionLoaded;

            private static void EnsureSectionLoaded()
            {
                if (!s_sectionLoaded)
                {
                    ScriptingScriptResourceHandlerSection webApplicationSection = (ScriptingScriptResourceHandlerSection) WebConfigurationManager.GetWebApplicationSection("system.web.extensions/scripting/scriptResourceHandler");
                    if (webApplicationSection != null)
                    {
                        s_enableCaching = webApplicationSection.EnableCaching;
                        s_enableCompression = webApplicationSection.EnableCompression;
                    }
                    else
                    {
                        s_enableCaching = (bool) ScriptingScriptResourceHandlerSection._propEnableCaching.DefaultValue;
                        s_enableCompression = (bool) ScriptingScriptResourceHandlerSection._propEnableCompression.DefaultValue;
                    }
                    s_sectionLoaded = true;
                }
            }

            internal static bool EnableCaching
            {
                get
                {
                    EnsureSectionLoaded();
                    return s_enableCaching;
                }
            }

            internal static bool EnableCompression
            {
                get
                {
                    EnsureSectionLoaded();
                    return s_enableCompression;
                }
            }
        }
    }
}

