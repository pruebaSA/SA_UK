namespace System.Web.Configuration
{
    using System;
    using System.Configuration;
    using System.Security.Permissions;
    using System.Web;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public sealed class AuthenticationSection : ConfigurationSection
    {
        private static ConfigurationPropertyCollection _properties = new ConfigurationPropertyCollection();
        private static readonly ConfigurationProperty _propForms = new ConfigurationProperty("forms", typeof(FormsAuthenticationConfiguration), null, ConfigurationPropertyOptions.None);
        private static readonly ConfigurationProperty _propMode = new ConfigurationProperty("mode", typeof(AuthenticationMode), AuthenticationMode.Windows, ConfigurationPropertyOptions.None);
        private static readonly ConfigurationProperty _propPassport = new ConfigurationProperty("passport", typeof(PassportAuthentication), null, ConfigurationPropertyOptions.None);
        private AuthenticationMode authenticationModeCache;
        private bool authenticationModeCached;

        static AuthenticationSection()
        {
            _properties.Add(_propForms);
            _properties.Add(_propPassport);
            _properties.Add(_propMode);
        }

        protected override void Reset(ConfigurationElement parentElement)
        {
            base.Reset(parentElement);
            this.authenticationModeCached = false;
        }

        internal void ValidateAuthenticationMode()
        {
            if ((this.Mode == AuthenticationMode.Passport) && (System.Web.UnsafeNativeMethods.PassportVersion() < 0))
            {
                throw new ConfigurationErrorsException(System.Web.SR.GetString("Passport_not_installed"));
            }
        }

        [ConfigurationProperty("forms")]
        public FormsAuthenticationConfiguration Forms =>
            ((FormsAuthenticationConfiguration) base[_propForms]);

        [ConfigurationProperty("mode", DefaultValue=1)]
        public AuthenticationMode Mode
        {
            get
            {
                if (!this.authenticationModeCached)
                {
                    this.authenticationModeCache = (AuthenticationMode) base[_propMode];
                    this.authenticationModeCached = true;
                }
                return this.authenticationModeCache;
            }
            set
            {
                base[_propMode] = value;
                this.authenticationModeCache = value;
            }
        }

        [ConfigurationProperty("passport")]
        public PassportAuthentication Passport =>
            ((PassportAuthentication) base[_propPassport]);

        protected override ConfigurationPropertyCollection Properties =>
            _properties;
    }
}

