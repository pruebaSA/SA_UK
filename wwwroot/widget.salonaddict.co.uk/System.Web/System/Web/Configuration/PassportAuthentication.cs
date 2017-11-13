namespace System.Web.Configuration
{
    using System;
    using System.Configuration;
    using System.Security.Permissions;
    using System.Web;
    using System.Web.Util;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public sealed class PassportAuthentication : ConfigurationElement
    {
        private static ConfigurationPropertyCollection _properties = new ConfigurationPropertyCollection();
        private static readonly ConfigurationProperty _propRedirectUrl = new ConfigurationProperty("redirectUrl", typeof(string), "internal", ConfigurationPropertyOptions.None);
        private static readonly ConfigurationElementProperty s_elemProperty = new ConfigurationElementProperty(new CallbackValidator(typeof(PassportAuthentication), new ValidatorCallback(PassportAuthentication.Validate)));

        static PassportAuthentication()
        {
            _properties.Add(_propRedirectUrl);
        }

        private static void Validate(object value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("passport");
            }
            PassportAuthentication authentication = (PassportAuthentication) value;
            if (System.Web.Util.StringUtil.StringStartsWith(authentication.RedirectUrl, @"\\") || ((authentication.RedirectUrl.Length > 1) && (authentication.RedirectUrl[1] == ':')))
            {
                throw new ConfigurationErrorsException(System.Web.SR.GetString("Auth_bad_url"));
            }
        }

        protected override ConfigurationElementProperty ElementProperty =>
            s_elemProperty;

        protected override ConfigurationPropertyCollection Properties =>
            _properties;

        [ConfigurationProperty("redirectUrl", DefaultValue="internal"), StringValidator]
        public string RedirectUrl
        {
            get => 
                ((string) base[_propRedirectUrl]);
            set
            {
                base[_propRedirectUrl] = value;
            }
        }
    }
}

