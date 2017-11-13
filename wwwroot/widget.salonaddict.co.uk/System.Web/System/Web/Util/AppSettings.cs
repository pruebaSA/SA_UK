namespace System.Web.Util
{
    using System;
    using System.Collections.Specialized;
    using System.Runtime.CompilerServices;
    using System.Web;

    internal static class AppSettings
    {
        private static bool _allowAnonymousImpersonation;
        private static bool? _allowInsecureDeserialization;
        private static bool _allowRelaxedHttpUserName;
        private static bool _allowRelaxedRelativeUrl;
        private static bool _alwaysIgnoreViewStateValidationErrors;
        private static object _appSettingsLock = new object();
        private static string _formsAuthReturnUrlVar;
        private static bool _ignoreFormActionAttribute;
        private static int _maxHttpCollectionKeys = 0x3e8;
        private static int _requestQueueLimitPerSession;
        private static bool _restrictXmlControls;
        private static volatile bool _settingsInitialized = false;
        private static TimeSpan _sqlSessionRetryInterval = TimeSpan.Zero;
        private static bool _useHostHeaderForRequestUrl;
        private static bool _useLegacyBrowserCaps;
        private static bool _useLegacyEncryption;
        private static bool _useLegacyFormsAuthenticationTicketCompatibility;
        private static bool _useStrictParserRegex;
        private const int DefaultMaxHttpCollectionKeys = 0x3e8;
        internal const int UnlimitedRequestsPerSession = 0x7fffffff;

        private static void EnsureSettingsLoaded()
        {
            if (!_settingsInitialized)
            {
                lock (_appSettingsLock)
                {
                    if (!_settingsInitialized)
                    {
                        NameValueCollection settings = null;
                        try
                        {
                            CachedPathData applicationPathData = CachedPathData.GetApplicationPathData();
                            if ((applicationPathData != null) && (applicationPathData.ConfigRecord != null))
                            {
                                settings = applicationPathData.ConfigRecord.GetSection("appSettings") as NameValueCollection;
                            }
                        }
                        finally
                        {
                            if ((settings == null) || !bool.TryParse(settings["aspnet:IgnoreFormActionAttribute"], out _ignoreFormActionAttribute))
                            {
                                _ignoreFormActionAttribute = false;
                            }
                            if ((settings == null) || !bool.TryParse(settings["aspnet:UseHostHeaderForRequestUrl"], out _useHostHeaderForRequestUrl))
                            {
                                _useHostHeaderForRequestUrl = false;
                            }
                            if ((settings == null) || !bool.TryParse(settings["aspnet:AllowAnonymousImpersonation"], out _allowAnonymousImpersonation))
                            {
                                _allowAnonymousImpersonation = false;
                            }
                            if ((settings == null) || !TimeSpan.TryParse(settings["aspnet:SqlSessionState:RetryInterval"], out _sqlSessionRetryInterval))
                            {
                                _sqlSessionRetryInterval = TimeSpan.Zero;
                            }
                            if ((settings == null) || !bool.TryParse(settings["aspnet:UseLegacyEncryption"], out _useLegacyEncryption))
                            {
                                _useLegacyEncryption = false;
                            }
                            if ((settings == null) || !bool.TryParse(settings["aspnet:UseStrictParserRegex"], out _useStrictParserRegex))
                            {
                                _useStrictParserRegex = false;
                            }
                            if (settings != null)
                            {
                                _formsAuthReturnUrlVar = settings["aspnet:FormsAuthReturnUrlVar"];
                            }
                            if ((settings == null) || !bool.TryParse(settings["aspnet:AllowRelaxedRelativeUrl"], out _allowRelaxedRelativeUrl))
                            {
                                _allowRelaxedRelativeUrl = false;
                            }
                            if ((settings == null) || !bool.TryParse(settings["aspnet:RestrictXmlControls"], out _restrictXmlControls))
                            {
                                _restrictXmlControls = false;
                            }
                            if ((settings == null) || !bool.TryParse(settings["aspnet:UseLegacyFormsAuthenticationTicketCompatibility"], out _useLegacyFormsAuthenticationTicketCompatibility))
                            {
                                _useLegacyFormsAuthenticationTicketCompatibility = false;
                            }
                            if ((settings == null) || !bool.TryParse(settings["aspnet:AllowRelaxedHttpUserName"], out _allowRelaxedHttpUserName))
                            {
                                _allowRelaxedHttpUserName = false;
                            }
                            if (((settings == null) || !int.TryParse(settings["aspnet:MaxHttpCollectionKeys"], out _maxHttpCollectionKeys)) || (_maxHttpCollectionKeys < 0))
                            {
                                _maxHttpCollectionKeys = 0x3e8;
                            }
                            if ((settings == null) || !bool.TryParse(settings["aspnet:UseLegacyBrowserCaps"], out _useLegacyBrowserCaps))
                            {
                                _useLegacyBrowserCaps = false;
                            }
                            _allowInsecureDeserialization = GetNullableBooleanValue(settings, "aspnet:AllowInsecureDeserialization");
                            if ((settings == null) || !bool.TryParse(settings["aspnet:AlwaysIgnoreViewStateValidationErrors"], out _alwaysIgnoreViewStateValidationErrors))
                            {
                                _alwaysIgnoreViewStateValidationErrors = false;
                            }
                            if (((settings == null) || !int.TryParse(settings["aspnet:RequestQueueLimitPerSession"], out _requestQueueLimitPerSession)) || (_requestQueueLimitPerSession < 0))
                            {
                                _requestQueueLimitPerSession = 0x7fffffff;
                            }
                            _settingsInitialized = true;
                        }
                    }
                }
            }
        }

        private static bool? GetNullableBooleanValue(NameValueCollection settings, string key)
        {
            bool flag;
            if ((settings != null) && bool.TryParse(settings[key], out flag))
            {
                return new bool?(flag);
            }
            return null;
        }

        internal static bool AllowAnonymousImpersonation
        {
            get
            {
                EnsureSettingsLoaded();
                return _allowAnonymousImpersonation;
            }
        }

        internal static bool? AllowInsecureDeserialization
        {
            get
            {
                EnsureSettingsLoaded();
                return _allowInsecureDeserialization;
            }
        }

        internal static bool AllowRelaxedHttpUserName
        {
            get
            {
                EnsureSettingsLoaded();
                return _allowRelaxedHttpUserName;
            }
        }

        internal static bool AllowRelaxedRelativeUrl
        {
            get
            {
                EnsureSettingsLoaded();
                return _allowRelaxedRelativeUrl;
            }
        }

        internal static bool AlwaysIgnoreViewStateValidationErrors
        {
            get
            {
                EnsureSettingsLoaded();
                return _alwaysIgnoreViewStateValidationErrors;
            }
        }

        internal static string FormsAuthReturnUrlVar
        {
            get
            {
                EnsureSettingsLoaded();
                return _formsAuthReturnUrlVar;
            }
        }

        internal static bool IgnoreFormActionAttribute
        {
            get
            {
                EnsureSettingsLoaded();
                return _ignoreFormActionAttribute;
            }
        }

        internal static int MaxHttpCollectionKeys
        {
            get
            {
                EnsureSettingsLoaded();
                return _maxHttpCollectionKeys;
            }
        }

        internal static int RequestQueueLimitPerSession
        {
            get
            {
                EnsureSettingsLoaded();
                return _requestQueueLimitPerSession;
            }
        }

        internal static bool RestrictXmlControls
        {
            get
            {
                EnsureSettingsLoaded();
                return _restrictXmlControls;
            }
        }

        internal static TimeSpan SqlSessionRetryInterval
        {
            get
            {
                EnsureSettingsLoaded();
                return _sqlSessionRetryInterval;
            }
        }

        internal static bool UseHostHeaderForRequestUrl
        {
            get
            {
                EnsureSettingsLoaded();
                return _useHostHeaderForRequestUrl;
            }
        }

        internal static bool UseLegacyBrowserCaps
        {
            get
            {
                EnsureSettingsLoaded();
                return _useLegacyBrowserCaps;
            }
        }

        internal static bool UseLegacyEncryption
        {
            get
            {
                EnsureSettingsLoaded();
                return _useLegacyEncryption;
            }
        }

        internal static bool UseLegacyFormsAuthenticationTicketCompatibility
        {
            get
            {
                EnsureSettingsLoaded();
                return _useLegacyFormsAuthenticationTicketCompatibility;
            }
        }

        internal static bool UseStrictParserRegex
        {
            get
            {
                EnsureSettingsLoaded();
                return _useStrictParserRegex;
            }
        }
    }
}

