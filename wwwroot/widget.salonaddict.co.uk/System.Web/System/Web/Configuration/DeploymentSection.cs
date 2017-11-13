namespace System.Web.Configuration
{
    using System;
    using System.Configuration;
    using System.Security.Permissions;
    using System.Web;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public sealed class DeploymentSection : ConfigurationSection
    {
        private static ConfigurationPropertyCollection _properties = new ConfigurationPropertyCollection();
        private static readonly ConfigurationProperty _propRetail = new ConfigurationProperty("retail", typeof(bool), false, ConfigurationPropertyOptions.None);
        private static bool s_hasCachedData;
        private static bool s_retail;

        static DeploymentSection()
        {
            _properties.Add(_propRetail);
        }

        protected override ConfigurationPropertyCollection Properties =>
            _properties;

        [ConfigurationProperty("retail", DefaultValue=false)]
        public bool Retail
        {
            get => 
                ((bool) base[_propRetail]);
            set
            {
                base[_propRetail] = value;
            }
        }

        internal static bool RetailInternal
        {
            get
            {
                if (!s_hasCachedData)
                {
                    s_retail = RuntimeConfig.GetAppConfig().Deployment.Retail;
                    s_hasCachedData = true;
                }
                return s_retail;
            }
        }
    }
}

