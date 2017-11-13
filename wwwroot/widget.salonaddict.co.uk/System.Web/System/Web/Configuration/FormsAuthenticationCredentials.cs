namespace System.Web.Configuration
{
    using System;
    using System.Configuration;
    using System.Security.Permissions;
    using System.Web;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public sealed class FormsAuthenticationCredentials : ConfigurationElement
    {
        private static ConfigurationPropertyCollection _properties = new ConfigurationPropertyCollection();
        private static readonly ConfigurationProperty _propPasswordFormat = new ConfigurationProperty("passwordFormat", typeof(FormsAuthPasswordFormat), FormsAuthPasswordFormat.SHA1, ConfigurationPropertyOptions.None);
        private static readonly ConfigurationProperty _propUsers = new ConfigurationProperty(null, typeof(FormsAuthenticationUserCollection), null, ConfigurationPropertyOptions.IsDefaultCollection);

        static FormsAuthenticationCredentials()
        {
            _properties.Add(_propUsers);
            _properties.Add(_propPasswordFormat);
        }

        [ConfigurationProperty("passwordFormat", DefaultValue=1)]
        public FormsAuthPasswordFormat PasswordFormat
        {
            get => 
                ((FormsAuthPasswordFormat) base[_propPasswordFormat]);
            set
            {
                base[_propPasswordFormat] = value;
            }
        }

        protected override ConfigurationPropertyCollection Properties =>
            _properties;

        [ConfigurationProperty("", IsDefaultCollection=true, Options=ConfigurationPropertyOptions.IsDefaultCollection)]
        public FormsAuthenticationUserCollection Users =>
            ((FormsAuthenticationUserCollection) base[_propUsers]);
    }
}

