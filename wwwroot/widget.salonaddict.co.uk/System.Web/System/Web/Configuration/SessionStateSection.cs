namespace System.Web.Configuration
{
    using System;
    using System.ComponentModel;
    using System.Configuration;
    using System.Security.Permissions;
    using System.Web;
    using System.Web.SessionState;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public sealed class SessionStateSection : ConfigurationSection
    {
        private static readonly ConfigurationProperty _propAllowCustomSqlDatabase = new ConfigurationProperty("allowCustomSqlDatabase", typeof(bool), false, ConfigurationPropertyOptions.None);
        private static readonly ConfigurationProperty _propCookieless = new ConfigurationProperty("cookieless", typeof(string), HttpCookieMode.UseCookies.ToString(), ConfigurationPropertyOptions.None);
        private static readonly ConfigurationProperty _propCookieName = new ConfigurationProperty("cookieName", typeof(string), "ASP.NET_SessionId", ConfigurationPropertyOptions.None);
        private static readonly ConfigurationProperty _propCustomProvider = new ConfigurationProperty("customProvider", typeof(string), string.Empty, ConfigurationPropertyOptions.None);
        private static ConfigurationPropertyCollection _properties = new ConfigurationPropertyCollection();
        private static readonly ConfigurationProperty _propMode = new ConfigurationProperty("mode", typeof(SessionStateMode), SessionStateMode.InProc, ConfigurationPropertyOptions.None);
        private static readonly ConfigurationProperty _propPartitionResolverType = new ConfigurationProperty("partitionResolverType", typeof(string), string.Empty, ConfigurationPropertyOptions.None);
        private static readonly ConfigurationProperty _propProviders = new ConfigurationProperty("providers", typeof(ProviderSettingsCollection), null, ConfigurationPropertyOptions.None);
        private static readonly ConfigurationProperty _propRegenerateExpiredSessionId = new ConfigurationProperty("regenerateExpiredSessionId", typeof(bool), true, ConfigurationPropertyOptions.None);
        private static readonly ConfigurationProperty _propSessionIDManagerType = new ConfigurationProperty("sessionIDManagerType", typeof(string), string.Empty, ConfigurationPropertyOptions.None);
        private static readonly ConfigurationProperty _propSqlCommandTimeout = new ConfigurationProperty("sqlCommandTimeout", typeof(TimeSpan), TimeSpan.FromSeconds(30.0), StdValidatorsAndConverters.TimeSpanSecondsOrInfiniteConverter, null, ConfigurationPropertyOptions.None);
        private static readonly ConfigurationProperty _propSqlConnectionString = new ConfigurationProperty("sqlConnectionString", typeof(string), "data source=localhost;Integrated Security=SSPI", ConfigurationPropertyOptions.None);
        private static readonly ConfigurationProperty _propStateConnectionString = new ConfigurationProperty("stateConnectionString", typeof(string), "tcpip=loopback:42424", ConfigurationPropertyOptions.None);
        private static readonly ConfigurationProperty _propStateNetworkTimeout = new ConfigurationProperty("stateNetworkTimeout", typeof(TimeSpan), TimeSpan.FromSeconds(10.0), StdValidatorsAndConverters.TimeSpanSecondsOrInfiniteConverter, StdValidatorsAndConverters.PositiveTimeSpanValidator, ConfigurationPropertyOptions.None);
        private static readonly ConfigurationProperty _propTimeout = new ConfigurationProperty("timeout", typeof(TimeSpan), TimeSpan.FromMinutes(20.0), StdValidatorsAndConverters.TimeSpanMinutesOrInfiniteConverter, new TimeSpanValidator(TimeSpan.FromMinutes(1.0), TimeSpan.MaxValue), ConfigurationPropertyOptions.None);
        private static readonly ConfigurationProperty _propUseHostingIdentity = new ConfigurationProperty("useHostingIdentity", typeof(bool), true, ConfigurationPropertyOptions.None);
        private HttpCookieMode cookielessCache = HttpCookieMode.UseCookies;
        private bool cookielessCached;
        private bool regenerateExpiredSessionIdCache;
        private bool regenerateExpiredSessionIdCached;
        private static readonly ConfigurationElementProperty s_elemProperty = new ConfigurationElementProperty(new CallbackValidator(typeof(SessionStateSection), new ValidatorCallback(SessionStateSection.Validate)));

        static SessionStateSection()
        {
            _properties.Add(_propMode);
            _properties.Add(_propStateConnectionString);
            _properties.Add(_propStateNetworkTimeout);
            _properties.Add(_propSqlConnectionString);
            _properties.Add(_propSqlCommandTimeout);
            _properties.Add(_propCustomProvider);
            _properties.Add(_propCookieless);
            _properties.Add(_propCookieName);
            _properties.Add(_propTimeout);
            _properties.Add(_propAllowCustomSqlDatabase);
            _properties.Add(_propProviders);
            _properties.Add(_propRegenerateExpiredSessionId);
            _properties.Add(_propPartitionResolverType);
            _properties.Add(_propUseHostingIdentity);
            _properties.Add(_propSessionIDManagerType);
        }

        private HttpCookieMode ConvertToCookieMode(string s)
        {
            if (s == "true")
            {
                return HttpCookieMode.UseUri;
            }
            if (s == "false")
            {
                return HttpCookieMode.UseCookies;
            }
            int num = 0;
            Type enumType = typeof(HttpCookieMode);
            if (Enum.IsDefined(enumType, s))
            {
                num = (int) Enum.Parse(enumType, s);
                return (HttpCookieMode) num;
            }
            string str = "true, false";
            foreach (string str2 in Enum.GetNames(enumType))
            {
                if (str == null)
                {
                    str = str2;
                }
                else
                {
                    str = str + ", " + str2;
                }
            }
            throw new ConfigurationErrorsException(System.Web.SR.GetString("Invalid_enum_attribute", new object[] { "cookieless", str }), base.ElementInformation.Properties["cookieless"].Source, base.ElementInformation.Properties["cookieless"].LineNumber);
        }

        protected override void PostDeserialize()
        {
            this.ConvertToCookieMode((string) base[_propCookieless]);
        }

        private static void Validate(object value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("sessionState");
            }
            SessionStateSection section = (SessionStateSection) value;
            if ((section.Timeout.TotalMinutes > 525600.0) && ((section.Mode == SessionStateMode.InProc) || (section.Mode == SessionStateMode.StateServer)))
            {
                throw new ConfigurationErrorsException(System.Web.SR.GetString("Invalid_cache_based_session_timeout"), section.ElementInformation.Properties["timeout"].Source, section.ElementInformation.Properties["timeout"].LineNumber);
            }
        }

        [ConfigurationProperty("allowCustomSqlDatabase", DefaultValue=false)]
        public bool AllowCustomSqlDatabase
        {
            get => 
                ((bool) base[_propAllowCustomSqlDatabase]);
            set
            {
                base[_propAllowCustomSqlDatabase] = value;
            }
        }

        [ConfigurationProperty("cookieless")]
        public HttpCookieMode Cookieless
        {
            get
            {
                if (!this.cookielessCached)
                {
                    this.cookielessCache = this.ConvertToCookieMode((string) base[_propCookieless]);
                    this.cookielessCached = true;
                }
                return this.cookielessCache;
            }
            set
            {
                base[_propCookieless] = value.ToString();
                this.cookielessCache = value;
            }
        }

        [ConfigurationProperty("cookieName", DefaultValue="ASP.NET_SessionId")]
        public string CookieName
        {
            get => 
                ((string) base[_propCookieName]);
            set
            {
                base[_propCookieName] = value;
            }
        }

        [ConfigurationProperty("customProvider", DefaultValue="")]
        public string CustomProvider
        {
            get => 
                ((string) base[_propCustomProvider]);
            set
            {
                base[_propCustomProvider] = value;
            }
        }

        protected override ConfigurationElementProperty ElementProperty =>
            s_elemProperty;

        [ConfigurationProperty("mode", DefaultValue=1)]
        public SessionStateMode Mode
        {
            get => 
                ((SessionStateMode) base[_propMode]);
            set
            {
                base[_propMode] = value;
            }
        }

        [ConfigurationProperty("partitionResolverType", DefaultValue="")]
        public string PartitionResolverType
        {
            get => 
                ((string) base[_propPartitionResolverType]);
            set
            {
                base[_propPartitionResolverType] = value;
            }
        }

        protected override ConfigurationPropertyCollection Properties =>
            _properties;

        [ConfigurationProperty("providers")]
        public ProviderSettingsCollection Providers =>
            ((ProviderSettingsCollection) base[_propProviders]);

        [ConfigurationProperty("regenerateExpiredSessionId", DefaultValue=true)]
        public bool RegenerateExpiredSessionId
        {
            get
            {
                if (!this.regenerateExpiredSessionIdCached)
                {
                    this.regenerateExpiredSessionIdCache = (bool) base[_propRegenerateExpiredSessionId];
                    this.regenerateExpiredSessionIdCached = true;
                }
                return this.regenerateExpiredSessionIdCache;
            }
            set
            {
                base[_propRegenerateExpiredSessionId] = value;
                this.regenerateExpiredSessionIdCache = value;
            }
        }

        [ConfigurationProperty("sessionIDManagerType", DefaultValue="")]
        public string SessionIDManagerType
        {
            get => 
                ((string) base[_propSessionIDManagerType]);
            set
            {
                base[_propSessionIDManagerType] = value;
            }
        }

        [ConfigurationProperty("sqlCommandTimeout", DefaultValue="00:00:30"), TypeConverter(typeof(TimeSpanSecondsOrInfiniteConverter))]
        public TimeSpan SqlCommandTimeout
        {
            get => 
                ((TimeSpan) base[_propSqlCommandTimeout]);
            set
            {
                base[_propSqlCommandTimeout] = value;
            }
        }

        [ConfigurationProperty("sqlConnectionString", DefaultValue="data source=localhost;Integrated Security=SSPI")]
        public string SqlConnectionString
        {
            get => 
                ((string) base[_propSqlConnectionString]);
            set
            {
                base[_propSqlConnectionString] = value;
            }
        }

        [ConfigurationProperty("stateConnectionString", DefaultValue="tcpip=loopback:42424")]
        public string StateConnectionString
        {
            get => 
                ((string) base[_propStateConnectionString]);
            set
            {
                base[_propStateConnectionString] = value;
            }
        }

        [TypeConverter(typeof(TimeSpanSecondsOrInfiniteConverter)), ConfigurationProperty("stateNetworkTimeout", DefaultValue="00:00:10")]
        public TimeSpan StateNetworkTimeout
        {
            get => 
                ((TimeSpan) base[_propStateNetworkTimeout]);
            set
            {
                base[_propStateNetworkTimeout] = value;
            }
        }

        [TimeSpanValidator(MinValueString="00:01:00", MaxValueString="10675199.02:48:05.4775807"), ConfigurationProperty("timeout", DefaultValue="00:20:00"), TypeConverter(typeof(TimeSpanMinutesOrInfiniteConverter))]
        public TimeSpan Timeout
        {
            get => 
                ((TimeSpan) base[_propTimeout]);
            set
            {
                base[_propTimeout] = value;
            }
        }

        [ConfigurationProperty("useHostingIdentity", DefaultValue=true)]
        public bool UseHostingIdentity
        {
            get => 
                ((bool) base[_propUseHostingIdentity]);
            set
            {
                base[_propUseHostingIdentity] = value;
            }
        }
    }
}

