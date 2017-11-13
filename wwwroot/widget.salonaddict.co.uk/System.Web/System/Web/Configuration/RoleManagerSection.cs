﻿namespace System.Web.Configuration
{
    using System;
    using System.ComponentModel;
    using System.Configuration;
    using System.Security.Permissions;
    using System.Web;
    using System.Web.Security;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public sealed class RoleManagerSection : ConfigurationSection
    {
        private static readonly ConfigurationProperty _propCookieName = new ConfigurationProperty("cookieName", typeof(string), ".ASPXROLES", StdValidatorsAndConverters.WhiteSpaceTrimStringConverter, StdValidatorsAndConverters.NonEmptyStringValidator, ConfigurationPropertyOptions.None);
        private static readonly ConfigurationProperty _propCookiePath = new ConfigurationProperty("cookiePath", typeof(string), "/", StdValidatorsAndConverters.WhiteSpaceTrimStringConverter, StdValidatorsAndConverters.NonEmptyStringValidator, ConfigurationPropertyOptions.None);
        private static readonly ConfigurationProperty _propCookieProtection = new ConfigurationProperty("cookieProtection", typeof(System.Web.Security.CookieProtection), System.Web.Security.CookieProtection.All, ConfigurationPropertyOptions.None);
        private static readonly ConfigurationProperty _propCookieRequireSSL = new ConfigurationProperty("cookieRequireSSL", typeof(bool), false, ConfigurationPropertyOptions.None);
        private static readonly ConfigurationProperty _propCookieSlidingExpiration = new ConfigurationProperty("cookieSlidingExpiration", typeof(bool), true, ConfigurationPropertyOptions.None);
        private static readonly ConfigurationProperty _propCookieTimeout = new ConfigurationProperty("cookieTimeout", typeof(TimeSpan), TimeSpan.FromMinutes(30.0), StdValidatorsAndConverters.TimeSpanMinutesOrInfiniteConverter, StdValidatorsAndConverters.PositiveTimeSpanValidator, ConfigurationPropertyOptions.None);
        private static readonly ConfigurationProperty _propCreatePersistentCookie = new ConfigurationProperty("createPersistentCookie", typeof(bool), false, ConfigurationPropertyOptions.None);
        private static readonly ConfigurationProperty _propDefaultProvider = new ConfigurationProperty("defaultProvider", typeof(string), "AspNetSqlRoleProvider", null, StdValidatorsAndConverters.NonEmptyStringValidator, ConfigurationPropertyOptions.None);
        private static readonly ConfigurationProperty _propDomain = new ConfigurationProperty("domain", typeof(string), null, ConfigurationPropertyOptions.None);
        private static readonly ConfigurationProperty _propEnabled = new ConfigurationProperty("enabled", typeof(bool), false, ConfigurationPropertyOptions.None);
        private static ConfigurationPropertyCollection _properties = new ConfigurationPropertyCollection();
        private static readonly ConfigurationProperty _propMaxCachedResults = new ConfigurationProperty("maxCachedResults", typeof(int), 0x19, ConfigurationPropertyOptions.None);
        private static readonly ConfigurationProperty _propProviders = new ConfigurationProperty("providers", typeof(ProviderSettingsCollection), null, ConfigurationPropertyOptions.None);
        private static readonly ConfigurationProperty _propUseCookies = new ConfigurationProperty("cacheRolesInCookie", typeof(bool), false, ConfigurationPropertyOptions.None);

        static RoleManagerSection()
        {
            _properties.Add(_propEnabled);
            _properties.Add(_propUseCookies);
            _properties.Add(_propCookieName);
            _properties.Add(_propCookieTimeout);
            _properties.Add(_propCookiePath);
            _properties.Add(_propCookieRequireSSL);
            _properties.Add(_propCookieSlidingExpiration);
            _properties.Add(_propCookieProtection);
            _properties.Add(_propDefaultProvider);
            _properties.Add(_propProviders);
            _properties.Add(_propCreatePersistentCookie);
            _properties.Add(_propDomain);
            _properties.Add(_propMaxCachedResults);
        }

        [ConfigurationProperty("cacheRolesInCookie", DefaultValue=false)]
        public bool CacheRolesInCookie
        {
            get => 
                ((bool) base[_propUseCookies]);
            set
            {
                base[_propUseCookies] = value;
            }
        }

        [StringValidator(MinLength=1), ConfigurationProperty("cookieName", DefaultValue=".ASPXROLES"), TypeConverter(typeof(WhiteSpaceTrimStringConverter))]
        public string CookieName
        {
            get => 
                ((string) base[_propCookieName]);
            set
            {
                base[_propCookieName] = value;
            }
        }

        [ConfigurationProperty("cookiePath", DefaultValue="/"), StringValidator(MinLength=1), TypeConverter(typeof(WhiteSpaceTrimStringConverter))]
        public string CookiePath
        {
            get => 
                ((string) base[_propCookiePath]);
            set
            {
                base[_propCookiePath] = value;
            }
        }

        [ConfigurationProperty("cookieProtection", DefaultValue=3)]
        public System.Web.Security.CookieProtection CookieProtection
        {
            get => 
                ((System.Web.Security.CookieProtection) base[_propCookieProtection]);
            set
            {
                base[_propCookieProtection] = value;
            }
        }

        [ConfigurationProperty("cookieRequireSSL", DefaultValue=false)]
        public bool CookieRequireSSL
        {
            get => 
                ((bool) base[_propCookieRequireSSL]);
            set
            {
                base[_propCookieRequireSSL] = value;
            }
        }

        [ConfigurationProperty("cookieSlidingExpiration", DefaultValue=true)]
        public bool CookieSlidingExpiration
        {
            get => 
                ((bool) base[_propCookieSlidingExpiration]);
            set
            {
                base[_propCookieSlidingExpiration] = value;
            }
        }

        [TypeConverter(typeof(TimeSpanMinutesOrInfiniteConverter)), TimeSpanValidator(MinValueString="00:00:00", MaxValueString="10675199.02:48:05.4775807"), ConfigurationProperty("cookieTimeout", DefaultValue="00:30:00")]
        public TimeSpan CookieTimeout
        {
            get => 
                ((TimeSpan) base[_propCookieTimeout]);
            set
            {
                base[_propCookieTimeout] = value;
            }
        }

        [ConfigurationProperty("createPersistentCookie", DefaultValue=false)]
        public bool CreatePersistentCookie
        {
            get => 
                ((bool) base[_propCreatePersistentCookie]);
            set
            {
                base[_propCreatePersistentCookie] = value;
            }
        }

        [StringValidator(MinLength=1), TypeConverter(typeof(WhiteSpaceTrimStringConverter)), ConfigurationProperty("defaultProvider", DefaultValue="AspNetSqlRoleProvider")]
        public string DefaultProvider
        {
            get => 
                ((string) base[_propDefaultProvider]);
            set
            {
                base[_propDefaultProvider] = value;
            }
        }

        [ConfigurationProperty("domain")]
        public string Domain
        {
            get => 
                ((string) base[_propDomain]);
            set
            {
                base[_propDomain] = value;
            }
        }

        [ConfigurationProperty("enabled", DefaultValue=false)]
        public bool Enabled
        {
            get => 
                ((bool) base[_propEnabled]);
            set
            {
                base[_propEnabled] = value;
            }
        }

        [ConfigurationProperty("maxCachedResults", DefaultValue=0x19)]
        public int MaxCachedResults
        {
            get => 
                ((int) base[_propMaxCachedResults]);
            set
            {
                base[_propMaxCachedResults] = value;
            }
        }

        protected override ConfigurationPropertyCollection Properties =>
            _properties;

        [ConfigurationProperty("providers")]
        public ProviderSettingsCollection Providers =>
            ((ProviderSettingsCollection) base[_propProviders]);

        private enum InheritedType
        {
            inNeither,
            inParent,
            inSelf,
            inBothSame,
            inBothDiff
        }
    }
}

