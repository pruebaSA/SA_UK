namespace System.Web.Configuration
{
    using System;
    using System.Configuration;
    using System.Security.Permissions;
    using System.Web;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public sealed class SecurityPolicySection : ConfigurationSection
    {
        private static ConfigurationPropertyCollection _properties = new ConfigurationPropertyCollection();
        private static readonly ConfigurationProperty _propTrustLevels = new ConfigurationProperty(null, typeof(TrustLevelCollection), null, ConfigurationPropertyOptions.IsDefaultCollection);

        static SecurityPolicySection()
        {
            _properties.Add(_propTrustLevels);
        }

        protected override ConfigurationPropertyCollection Properties =>
            _properties;

        [ConfigurationProperty("", IsDefaultCollection=true)]
        public TrustLevelCollection TrustLevels =>
            ((TrustLevelCollection) base[_propTrustLevels]);
    }
}

