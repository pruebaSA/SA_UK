namespace System.Web.Configuration
{
    using System;
    using System.Collections;
    using System.Configuration;
    using System.Security.Permissions;
    using System.Web;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public sealed class WebControlsSection : ConfigurationSection
    {
        private static readonly ConfigurationProperty _propClientScriptsLocation = new ConfigurationProperty("clientScriptsLocation", typeof(string), "/aspnet_client/{0}/{1}/", null, StdValidatorsAndConverters.NonEmptyStringValidator, ConfigurationPropertyOptions.IsRequired);
        private static ConfigurationPropertyCollection _properties = new ConfigurationPropertyCollection();

        static WebControlsSection()
        {
            _properties.Add(_propClientScriptsLocation);
        }

        protected override object GetRuntimeObject()
        {
            Hashtable hashtable = new Hashtable();
            foreach (ConfigurationProperty property in this.Properties)
            {
                hashtable[property.Name] = base[property];
            }
            return hashtable;
        }

        [StringValidator(MinLength=1), ConfigurationProperty("clientScriptsLocation", IsRequired=true, DefaultValue="/aspnet_client/{0}/{1}/")]
        public string ClientScriptsLocation =>
            ((string) base[_propClientScriptsLocation]);

        protected override ConfigurationPropertyCollection Properties =>
            _properties;
    }
}

