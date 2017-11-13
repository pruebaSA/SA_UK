namespace System.Web.Configuration
{
    using System;
    using System.ComponentModel;
    using System.Configuration;
    using System.Security.Permissions;
    using System.Web;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class Converter : ConfigurationElement
    {
        private static ConfigurationValidatorBase _nonEmptyStringValidator = new StringValidator(1);
        private static ConfigurationPropertyCollection _properties = BuildProperties();
        private static readonly ConfigurationProperty _propName = new ConfigurationProperty("name", typeof(string), null, _whiteSpaceTrimStringConverter, _nonEmptyStringValidator, ConfigurationPropertyOptions.IsKey | ConfigurationPropertyOptions.IsRequired);
        private static readonly ConfigurationProperty _propType = new ConfigurationProperty("type", typeof(string), null, _whiteSpaceTrimStringConverter, _nonEmptyStringValidator, ConfigurationPropertyOptions.IsRequired);
        private static TypeConverter _whiteSpaceTrimStringConverter = new WhiteSpaceTrimStringConverter();

        private static ConfigurationPropertyCollection BuildProperties() => 
            new ConfigurationPropertyCollection { 
                _propType,
                _propName
            };

        [ConfigurationProperty("name", IsRequired=true, IsKey=true, DefaultValue=""), StringValidator(MinLength=1)]
        public string Name
        {
            get => 
                ((string) base[_propName]);
            set
            {
                base[_propName] = value;
            }
        }

        protected override ConfigurationPropertyCollection Properties =>
            _properties;

        [ConfigurationProperty("type", IsRequired=true, DefaultValue=""), StringValidator(MinLength=1)]
        public string Type
        {
            get => 
                ((string) base[_propType]);
            set
            {
                base[_propType] = value;
            }
        }
    }
}

