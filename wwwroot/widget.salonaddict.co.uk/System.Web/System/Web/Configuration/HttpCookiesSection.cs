namespace System.Web.Configuration
{
    using System;
    using System.Configuration;
    using System.Security.Permissions;
    using System.Web;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public sealed class HttpCookiesSection : ConfigurationSection
    {
        private static readonly ConfigurationProperty _propDomain = new ConfigurationProperty("domain", typeof(string), string.Empty, ConfigurationPropertyOptions.None);
        private static ConfigurationPropertyCollection _properties = new ConfigurationPropertyCollection();
        private static readonly ConfigurationProperty _propHttpOnlyCookies = new ConfigurationProperty("httpOnlyCookies", typeof(bool), false, ConfigurationPropertyOptions.None);
        private static readonly ConfigurationProperty _propRequireSSL = new ConfigurationProperty("requireSSL", typeof(bool), false, ConfigurationPropertyOptions.None);

        static HttpCookiesSection()
        {
            _properties.Add(_propHttpOnlyCookies);
            _properties.Add(_propRequireSSL);
            _properties.Add(_propDomain);
        }

        [ConfigurationProperty("domain", DefaultValue="")]
        public string Domain
        {
            get => 
                ((string) base[_propDomain]);
            set
            {
                base[_propDomain] = value;
            }
        }

        [ConfigurationProperty("httpOnlyCookies", DefaultValue=false)]
        public bool HttpOnlyCookies
        {
            get => 
                ((bool) base[_propHttpOnlyCookies]);
            set
            {
                base[_propHttpOnlyCookies] = value;
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

