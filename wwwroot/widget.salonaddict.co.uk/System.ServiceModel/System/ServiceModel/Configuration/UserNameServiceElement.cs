namespace System.ServiceModel.Configuration
{
    using System;
    using System.ComponentModel;
    using System.Configuration;
    using System.IdentityModel.Selectors;
    using System.ServiceModel;
    using System.ServiceModel.Security;
    using System.Web.Security;

    public sealed class UserNameServiceElement : ConfigurationElement
    {
        private ConfigurationPropertyCollection properties;

        internal void ApplyConfiguration(UserNamePasswordServiceCredential userName)
        {
            if (userName == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("userName");
            }
            userName.UserNamePasswordValidationMode = this.UserNamePasswordValidationMode;
            userName.IncludeWindowsGroups = this.IncludeWindowsGroups;
            userName.CacheLogonTokens = this.CacheLogonTokens;
            userName.MaxCachedLogonTokens = this.MaxCachedLogonTokens;
            userName.CachedLogonTokenLifetime = this.CachedLogonTokenLifetime;
            if (base.ElementInformation.Properties["membershipProviderName"].ValueOrigin != PropertyValueOrigin.Default)
            {
                userName.MembershipProvider = Membership.Providers[this.MembershipProviderName];
                if (userName.MembershipProvider == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ConfigurationErrorsException(System.ServiceModel.SR.GetString("InvalidMembershipProviderSpecifiedInConfig", new object[] { this.MembershipProviderName })));
                }
            }
            else if (userName.UserNamePasswordValidationMode == System.ServiceModel.Security.UserNamePasswordValidationMode.MembershipProvider)
            {
                userName.MembershipProvider = Membership.Provider;
            }
            if (!string.IsNullOrEmpty(this.CustomUserNamePasswordValidatorType))
            {
                Type c = Type.GetType(this.CustomUserNamePasswordValidatorType, true);
                if (!typeof(UserNamePasswordValidator).IsAssignableFrom(c))
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ConfigurationErrorsException(System.ServiceModel.SR.GetString("ConfigInvalidUserNamePasswordValidatorType", new object[] { this.CustomUserNamePasswordValidatorType, typeof(UserNamePasswordValidator).ToString() })));
                }
                userName.CustomUserNamePasswordValidator = (UserNamePasswordValidator) Activator.CreateInstance(c);
            }
        }

        public void Copy(UserNameServiceElement from)
        {
            if (this.IsReadOnly())
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ConfigurationErrorsException(System.ServiceModel.SR.GetString("ConfigReadOnly")));
            }
            if (from == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("from");
            }
            this.UserNamePasswordValidationMode = from.UserNamePasswordValidationMode;
            this.IncludeWindowsGroups = from.IncludeWindowsGroups;
            this.MembershipProviderName = from.MembershipProviderName;
            this.CustomUserNamePasswordValidatorType = from.CustomUserNamePasswordValidatorType;
            this.CacheLogonTokens = from.CacheLogonTokens;
            this.MaxCachedLogonTokens = from.MaxCachedLogonTokens;
            this.CachedLogonTokenLifetime = from.CachedLogonTokenLifetime;
        }

        [ServiceModelTimeSpanValidator(MinValueString="00:00:00.0000001"), ConfigurationProperty("cachedLogonTokenLifetime", DefaultValue="00:15:00"), TypeConverter(typeof(TimeSpanOrInfiniteConverter))]
        public TimeSpan CachedLogonTokenLifetime
        {
            get => 
                ((TimeSpan) base["cachedLogonTokenLifetime"]);
            set
            {
                base["cachedLogonTokenLifetime"] = value;
            }
        }

        [ConfigurationProperty("cacheLogonTokens", DefaultValue=false)]
        public bool CacheLogonTokens
        {
            get => 
                ((bool) base["cacheLogonTokens"]);
            set
            {
                base["cacheLogonTokens"] = value;
            }
        }

        [StringValidator(MinLength=0), ConfigurationProperty("customUserNamePasswordValidatorType", DefaultValue="")]
        public string CustomUserNamePasswordValidatorType
        {
            get => 
                ((string) base["customUserNamePasswordValidatorType"]);
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    value = string.Empty;
                }
                base["customUserNamePasswordValidatorType"] = value;
            }
        }

        [ConfigurationProperty("includeWindowsGroups", DefaultValue=true)]
        public bool IncludeWindowsGroups
        {
            get => 
                ((bool) base["includeWindowsGroups"]);
            set
            {
                base["includeWindowsGroups"] = value;
            }
        }

        [ConfigurationProperty("maxCachedLogonTokens", DefaultValue=0x80), IntegerValidator(MinValue=1)]
        public int MaxCachedLogonTokens
        {
            get => 
                ((int) base["maxCachedLogonTokens"]);
            set
            {
                base["maxCachedLogonTokens"] = value;
            }
        }

        [StringValidator(MinLength=0), ConfigurationProperty("membershipProviderName", DefaultValue="")]
        public string MembershipProviderName
        {
            get => 
                ((string) base["membershipProviderName"]);
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    value = string.Empty;
                }
                base["membershipProviderName"] = value;
            }
        }

        protected override ConfigurationPropertyCollection Properties
        {
            get
            {
                if (this.properties == null)
                {
                    ConfigurationPropertyCollection propertys = new ConfigurationPropertyCollection {
                        new ConfigurationProperty("userNamePasswordValidationMode", typeof(System.ServiceModel.Security.UserNamePasswordValidationMode), System.ServiceModel.Security.UserNamePasswordValidationMode.Windows, null, new ServiceModelEnumValidator(typeof(UserNamePasswordValidationModeHelper)), ConfigurationPropertyOptions.None),
                        new ConfigurationProperty("includeWindowsGroups", typeof(bool), true, null, null, ConfigurationPropertyOptions.None),
                        new ConfigurationProperty("membershipProviderName", typeof(string), string.Empty, null, new StringValidator(0, 0x7fffffff, null), ConfigurationPropertyOptions.None),
                        new ConfigurationProperty("customUserNamePasswordValidatorType", typeof(string), string.Empty, null, new StringValidator(0, 0x7fffffff, null), ConfigurationPropertyOptions.None),
                        new ConfigurationProperty("cacheLogonTokens", typeof(bool), false, null, null, ConfigurationPropertyOptions.None),
                        new ConfigurationProperty("maxCachedLogonTokens", typeof(int), 0x80, null, new IntegerValidator(1, 0x7fffffff, false), ConfigurationPropertyOptions.None),
                        new ConfigurationProperty("cachedLogonTokenLifetime", typeof(TimeSpan), TimeSpan.Parse("00:15:00"), new TimeSpanOrInfiniteConverter(), new TimeSpanOrInfiniteValidator(TimeSpan.Parse("00:00:00.0000001"), TimeSpan.Parse("24.20:31:23.6470000")), ConfigurationPropertyOptions.None)
                    };
                    this.properties = propertys;
                }
                return this.properties;
            }
        }

        [ServiceModelEnumValidator(typeof(UserNamePasswordValidationModeHelper)), ConfigurationProperty("userNamePasswordValidationMode", DefaultValue=0)]
        public System.ServiceModel.Security.UserNamePasswordValidationMode UserNamePasswordValidationMode
        {
            get => 
                ((System.ServiceModel.Security.UserNamePasswordValidationMode) base["userNamePasswordValidationMode"]);
            set
            {
                base["userNamePasswordValidationMode"] = value;
            }
        }
    }
}

