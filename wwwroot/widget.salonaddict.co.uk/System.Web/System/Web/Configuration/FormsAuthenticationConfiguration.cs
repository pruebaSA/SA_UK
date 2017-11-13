﻿namespace System.Web.Configuration
{
    using System;
    using System.ComponentModel;
    using System.Configuration;
    using System.Security.Permissions;
    using System.Web;
    using System.Web.Util;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public sealed class FormsAuthenticationConfiguration : ConfigurationElement
    {
        private static readonly ConfigurationProperty _propCookieless = new ConfigurationProperty("cookieless", typeof(HttpCookieMode), HttpCookieMode.UseDeviceProfile, ConfigurationPropertyOptions.None);
        private static readonly ConfigurationProperty _propCredentials = new ConfigurationProperty("credentials", typeof(FormsAuthenticationCredentials), null, ConfigurationPropertyOptions.None);
        private static readonly ConfigurationProperty _propDefaultUrl = new ConfigurationProperty("defaultUrl", typeof(string), "default.aspx", null, StdValidatorsAndConverters.NonEmptyStringValidator, ConfigurationPropertyOptions.None);
        private static readonly ConfigurationProperty _propDomain = new ConfigurationProperty("domain", typeof(string), null, ConfigurationPropertyOptions.None);
        private static readonly ConfigurationProperty _propEnableCrossAppRedirects = new ConfigurationProperty("enableCrossAppRedirects", typeof(bool), false, ConfigurationPropertyOptions.None);
        private static ConfigurationPropertyCollection _properties = new ConfigurationPropertyCollection();
        private static readonly ConfigurationProperty _propLoginUrl = new ConfigurationProperty("loginUrl", typeof(string), "login.aspx", null, StdValidatorsAndConverters.NonEmptyStringValidator, ConfigurationPropertyOptions.None);
        private static readonly ConfigurationProperty _propName = new ConfigurationProperty("name", typeof(string), ".ASPXAUTH", null, StdValidatorsAndConverters.NonEmptyStringValidator, ConfigurationPropertyOptions.None);
        private static readonly ConfigurationProperty _propPath = new ConfigurationProperty("path", typeof(string), "/", null, StdValidatorsAndConverters.NonEmptyStringValidator, ConfigurationPropertyOptions.None);
        private static readonly ConfigurationProperty _propProtection = new ConfigurationProperty("protection", typeof(FormsProtectionEnum), FormsProtectionEnum.All, ConfigurationPropertyOptions.None);
        private static readonly ConfigurationProperty _propRequireSSL = new ConfigurationProperty("requireSSL", typeof(bool), false, ConfigurationPropertyOptions.None);
        private static readonly ConfigurationProperty _propSlidingExpiration = new ConfigurationProperty("slidingExpiration", typeof(bool), true, ConfigurationPropertyOptions.None);
        private static readonly ConfigurationProperty _propTimeout = new ConfigurationProperty("timeout", typeof(TimeSpan), TimeSpan.FromMinutes(30.0), StdValidatorsAndConverters.TimeSpanMinutesConverter, new TimeSpanValidator(TimeSpan.FromMinutes(1.0), TimeSpan.MaxValue), ConfigurationPropertyOptions.None);
        private static readonly ConfigurationElementProperty s_elemProperty = new ConfigurationElementProperty(new CallbackValidator(typeof(FormsAuthenticationConfiguration), new ValidatorCallback(FormsAuthenticationConfiguration.Validate)));

        static FormsAuthenticationConfiguration()
        {
            _properties.Add(_propCredentials);
            _properties.Add(_propName);
            _properties.Add(_propLoginUrl);
            _properties.Add(_propDefaultUrl);
            _properties.Add(_propProtection);
            _properties.Add(_propTimeout);
            _properties.Add(_propPath);
            _properties.Add(_propRequireSSL);
            _properties.Add(_propSlidingExpiration);
            _properties.Add(_propCookieless);
            _properties.Add(_propDomain);
            _properties.Add(_propEnableCrossAppRedirects);
        }

        private static void Validate(object value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("forms");
            }
            FormsAuthenticationConfiguration configuration = (FormsAuthenticationConfiguration) value;
            if (System.Web.Util.StringUtil.StringStartsWith(configuration.LoginUrl, @"\\") || ((configuration.LoginUrl.Length > 1) && (configuration.LoginUrl[1] == ':')))
            {
                throw new ConfigurationErrorsException(System.Web.SR.GetString("Auth_bad_url"), configuration.ElementInformation.Properties["loginUrl"].Source, configuration.ElementInformation.Properties["loginUrl"].LineNumber);
            }
            if (System.Web.Util.StringUtil.StringStartsWith(configuration.DefaultUrl, @"\\") || ((configuration.DefaultUrl.Length > 1) && (configuration.DefaultUrl[1] == ':')))
            {
                throw new ConfigurationErrorsException(System.Web.SR.GetString("Auth_bad_url"), configuration.ElementInformation.Properties["defaultUrl"].Source, configuration.ElementInformation.Properties["defaultUrl"].LineNumber);
            }
        }

        [ConfigurationProperty("cookieless", DefaultValue=3)]
        public HttpCookieMode Cookieless
        {
            get => 
                ((HttpCookieMode) base[_propCookieless]);
            set
            {
                base[_propCookieless] = value;
            }
        }

        [ConfigurationProperty("credentials")]
        public FormsAuthenticationCredentials Credentials =>
            ((FormsAuthenticationCredentials) base[_propCredentials]);

        [ConfigurationProperty("defaultUrl", DefaultValue="default.aspx"), StringValidator(MinLength=1)]
        public string DefaultUrl
        {
            get => 
                ((string) base[_propDefaultUrl]);
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    base[_propDefaultUrl] = _propDefaultUrl.DefaultValue;
                }
                else
                {
                    base[_propDefaultUrl] = value;
                }
            }
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

        protected override ConfigurationElementProperty ElementProperty =>
            s_elemProperty;

        [ConfigurationProperty("enableCrossAppRedirects", DefaultValue=false)]
        public bool EnableCrossAppRedirects
        {
            get => 
                ((bool) base[_propEnableCrossAppRedirects]);
            set
            {
                base[_propEnableCrossAppRedirects] = value;
            }
        }

        [StringValidator(MinLength=1), ConfigurationProperty("loginUrl", DefaultValue="login.aspx")]
        public string LoginUrl
        {
            get => 
                ((string) base[_propLoginUrl]);
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    base[_propLoginUrl] = _propLoginUrl.DefaultValue;
                }
                else
                {
                    base[_propLoginUrl] = value;
                }
            }
        }

        [ConfigurationProperty("name", DefaultValue=".ASPXAUTH"), StringValidator(MinLength=1)]
        public string Name
        {
            get => 
                ((string) base[_propName]);
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    base[_propName] = _propName.DefaultValue;
                }
                else
                {
                    base[_propName] = value;
                }
            }
        }

        [StringValidator(MinLength=1), ConfigurationProperty("path", DefaultValue="/")]
        public string Path
        {
            get => 
                ((string) base[_propPath]);
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    base[_propPath] = _propPath.DefaultValue;
                }
                else
                {
                    base[_propPath] = value;
                }
            }
        }

        protected override ConfigurationPropertyCollection Properties =>
            _properties;

        [ConfigurationProperty("protection", DefaultValue=0)]
        public FormsProtectionEnum Protection
        {
            get => 
                ((FormsProtectionEnum) base[_propProtection]);
            set
            {
                base[_propProtection] = value;
            }
        }

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

        [ConfigurationProperty("slidingExpiration", DefaultValue=true)]
        public bool SlidingExpiration
        {
            get => 
                ((bool) base[_propSlidingExpiration]);
            set
            {
                base[_propSlidingExpiration] = value;
            }
        }

        [TimeSpanValidator(MinValueString="00:01:00", MaxValueString="10675199.02:48:05.4775807"), TypeConverter(typeof(TimeSpanMinutesConverter)), ConfigurationProperty("timeout", DefaultValue="00:30:00")]
        public TimeSpan Timeout
        {
            get => 
                ((TimeSpan) base[_propTimeout]);
            set
            {
                base[_propTimeout] = value;
            }
        }
    }
}

