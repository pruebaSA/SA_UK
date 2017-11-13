namespace System.Web.Extensions.Util
{
    using System;
    using System.Collections.Specialized;
    using System.Configuration;
    using System.Runtime.CompilerServices;

    internal static class AppSettings
    {
        private static object _appSettingsLock = new object();
        private static int _maxJsonDeserializerMembers = 0x3e8;
        private static bool _scriptResourceAllowNonJsFiles;
        private static volatile bool _settingsInitialized = false;
        private const int DefaultMaxJsonDeserializerMembers = 0x3e8;

        private static void EnsureSettingsLoaded()
        {
            if (!_settingsInitialized)
            {
                lock (_appSettingsLock)
                {
                    if (!_settingsInitialized)
                    {
                        NameValueCollection appSettings = null;
                        try
                        {
                            appSettings = ConfigurationManager.AppSettings;
                        }
                        finally
                        {
                            if ((appSettings == null) || !bool.TryParse(appSettings["aspnet:ScriptResourceAllowNonJsFiles"], out _scriptResourceAllowNonJsFiles))
                            {
                                _scriptResourceAllowNonJsFiles = false;
                            }
                            if (((appSettings == null) || !int.TryParse(appSettings["aspnet:MaxJsonDeserializerMembers"], out _maxJsonDeserializerMembers)) || (_maxJsonDeserializerMembers < 0))
                            {
                                _maxJsonDeserializerMembers = 0x3e8;
                            }
                            _settingsInitialized = true;
                        }
                    }
                }
            }
        }

        internal static int MaxJsonDeserializerMembers
        {
            get
            {
                EnsureSettingsLoaded();
                return _maxJsonDeserializerMembers;
            }
        }

        internal static bool ScriptResourceAllowNonJsFiles
        {
            get
            {
                EnsureSettingsLoaded();
                return _scriptResourceAllowNonJsFiles;
            }
        }
    }
}

