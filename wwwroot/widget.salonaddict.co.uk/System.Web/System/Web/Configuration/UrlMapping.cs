namespace System.Web.Configuration
{
    using System;
    using System.Configuration;
    using System.Security.Permissions;
    using System.Web;
    using System.Web.Util;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public sealed class UrlMapping : ConfigurationElement
    {
        private static ConfigurationPropertyCollection _properties = new ConfigurationPropertyCollection();
        private static readonly ConfigurationProperty _propMappedUrl = new ConfigurationProperty("mappedUrl", typeof(string), null, StdValidatorsAndConverters.WhiteSpaceTrimStringConverter, StdValidatorsAndConverters.NonEmptyStringValidator, ConfigurationPropertyOptions.IsRequired);
        private static readonly ConfigurationProperty _propUrl = new ConfigurationProperty("url", typeof(string), null, StdValidatorsAndConverters.WhiteSpaceTrimStringConverter, new CallbackValidator(typeof(string), new ValidatorCallback(UrlMapping.ValidateUrl)), ConfigurationPropertyOptions.IsKey | ConfigurationPropertyOptions.IsRequired);

        static UrlMapping()
        {
            _properties.Add(_propUrl);
            _properties.Add(_propMappedUrl);
        }

        internal UrlMapping()
        {
        }

        public UrlMapping(string url, string mappedUrl)
        {
            base[_propUrl] = url;
            base[_propMappedUrl] = mappedUrl;
        }

        private static void ValidateUrl(object value)
        {
            StdValidatorsAndConverters.NonEmptyStringValidator.Validate(value);
            string path = (string) value;
            if (!System.Web.Util.UrlPath.IsAppRelativePath(path))
            {
                throw new ConfigurationErrorsException(System.Web.SR.GetString("UrlMappings_only_app_relative_url_allowed", new object[] { path }));
            }
        }

        [ConfigurationProperty("mappedUrl", IsRequired=true)]
        public string MappedUrl =>
            ((string) base[_propMappedUrl]);

        protected override ConfigurationPropertyCollection Properties =>
            _properties;

        [ConfigurationProperty("url", IsRequired=true, IsKey=true)]
        public string Url =>
            ((string) base[_propUrl]);
    }
}

