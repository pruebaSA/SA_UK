namespace System.Web.Configuration
{
    using System;
    using System.Configuration;
    using System.Security.Permissions;
    using System.Web;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public sealed class TrustSection : ConfigurationSection
    {
        private static ConfigurationPropertyCollection _properties = new ConfigurationPropertyCollection();
        private static readonly ConfigurationProperty _propLevel = new ConfigurationProperty("level", typeof(string), "Full", null, StdValidatorsAndConverters.NonEmptyStringValidator, ConfigurationPropertyOptions.IsRequired);
        private static readonly ConfigurationProperty _propOriginUrl = new ConfigurationProperty("originUrl", typeof(string), string.Empty, ConfigurationPropertyOptions.None);
        private static readonly ConfigurationProperty _propProcessRequestInApplicationTrust = new ConfigurationProperty("processRequestInApplicationTrust", typeof(bool), true, ConfigurationPropertyOptions.None);

        static TrustSection()
        {
            _properties.Add(_propLevel);
            _properties.Add(_propOriginUrl);
            _properties.Add(_propProcessRequestInApplicationTrust);
        }

        [ConfigurationProperty("level", IsRequired=true, DefaultValue="Full"), StringValidator(MinLength=1)]
        public string Level
        {
            get => 
                ((string) base[_propLevel]);
            set
            {
                base[_propLevel] = value;
            }
        }

        [ConfigurationProperty("originUrl", DefaultValue="")]
        public string OriginUrl
        {
            get => 
                ((string) base[_propOriginUrl]);
            set
            {
                base[_propOriginUrl] = value;
            }
        }

        [ConfigurationProperty("processRequestInApplicationTrust", DefaultValue=true)]
        public bool ProcessRequestInApplicationTrust
        {
            get => 
                ((bool) base[_propProcessRequestInApplicationTrust]);
            set
            {
                base[_propProcessRequestInApplicationTrust] = value;
            }
        }

        protected override ConfigurationPropertyCollection Properties =>
            _properties;
    }
}

