namespace System.Web.Configuration
{
    using System.Configuration;
    using System.Security.Permissions;
    using System.Web;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public sealed class SystemWebCachingSectionGroup : ConfigurationSectionGroup
    {
        [ConfigurationProperty("cache")]
        public CacheSection Cache =>
            ((CacheSection) base.Sections["cache"]);

        [ConfigurationProperty("outputCache")]
        public OutputCacheSection OutputCache =>
            ((OutputCacheSection) base.Sections["outputCache"]);

        [ConfigurationProperty("outputCacheSettings")]
        public OutputCacheSettingsSection OutputCacheSettings =>
            ((OutputCacheSettingsSection) base.Sections["outputCacheSettings"]);

        [ConfigurationProperty("sqlCacheDependency")]
        public SqlCacheDependencySection SqlCacheDependency =>
            ((SqlCacheDependencySection) base.Sections["sqlCacheDependency"]);
    }
}

