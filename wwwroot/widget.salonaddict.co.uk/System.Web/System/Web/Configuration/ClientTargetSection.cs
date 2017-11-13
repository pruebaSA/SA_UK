namespace System.Web.Configuration
{
    using System;
    using System.Configuration;
    using System.Security.Permissions;
    using System.Web;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public sealed class ClientTargetSection : ConfigurationSection
    {
        private static readonly ConfigurationProperty _propClientTargets = new ConfigurationProperty(null, typeof(ClientTargetCollection), null, ConfigurationPropertyOptions.IsRequired | ConfigurationPropertyOptions.IsDefaultCollection);
        private static ConfigurationPropertyCollection _properties = new ConfigurationPropertyCollection();

        static ClientTargetSection()
        {
            _properties.Add(_propClientTargets);
        }

        [ConfigurationProperty("", IsRequired=true, IsDefaultCollection=true)]
        public ClientTargetCollection ClientTargets =>
            ((ClientTargetCollection) base[_propClientTargets]);

        protected override ConfigurationPropertyCollection Properties =>
            _properties;
    }
}

