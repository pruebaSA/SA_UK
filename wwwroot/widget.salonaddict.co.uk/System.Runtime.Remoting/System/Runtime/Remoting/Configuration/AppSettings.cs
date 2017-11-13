namespace System.Runtime.Remoting.Configuration
{
    using System;
    using System.Configuration;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    internal static class AppSettings
    {
        internal static readonly bool AllowDtdProcessingInWSDLDefaultValue = false;
        internal static readonly string AllowDtdProcessingInWSDLKeyName = "microsoft:Remoting:AllowDtdProcessingInWSDL";
        private static bool allowDtdProcessingInWSDLValue = AllowDtdProcessingInWSDLDefaultValue;
        internal static readonly bool AllowTransparentProxyMessageDefaultValue = false;
        internal static readonly string AllowTransparentProxyMessageFwLink = "http://go.microsoft.com/fwlink/?LinkId=390633";
        internal static readonly string AllowTransparentProxyMessageKeyName = "microsoft:Remoting:AllowTransparentProxyMessage";
        private static bool allowTransparentProxyMessageValue = AllowTransparentProxyMessageDefaultValue;
        internal static readonly bool AllowUnsanitizedWSDLUrlsDefaultValue = false;
        internal static readonly string AllowUnsanitizedWSDLUrlsKeyName = "microsoft:Remoting:AllowUnsanitizedWSDLUrls";
        private static bool allowUnsanitizedWSDLUrlsValue = AllowUnsanitizedWSDLUrlsDefaultValue;
        private static object appSettingsLock = new object();
        private static volatile bool settingsInitialized = false;

        private static void EnsureSettingsLoaded()
        {
            if (!settingsInitialized)
            {
                lock (appSettingsLock)
                {
                    if (!settingsInitialized)
                    {
                        try
                        {
                            AppSettingsReader appSettingsReader = new AppSettingsReader();
                            object obj2 = null;
                            if (TryGetValue(appSettingsReader, AllowTransparentProxyMessageKeyName, typeof(bool), out obj2))
                            {
                                allowTransparentProxyMessageValue = (bool) obj2;
                            }
                            else
                            {
                                allowTransparentProxyMessageValue = AllowTransparentProxyMessageDefaultValue;
                            }
                            if (TryGetValue(appSettingsReader, AllowDtdProcessingInWSDLKeyName, typeof(bool), out obj2))
                            {
                                allowDtdProcessingInWSDLValue = (bool) obj2;
                            }
                            else
                            {
                                allowDtdProcessingInWSDLValue = AllowDtdProcessingInWSDLDefaultValue;
                            }
                            if (TryGetValue(appSettingsReader, AllowUnsanitizedWSDLUrlsKeyName, typeof(bool), out obj2))
                            {
                                allowUnsanitizedWSDLUrlsValue = (bool) obj2;
                            }
                            else
                            {
                                allowUnsanitizedWSDLUrlsValue = AllowUnsanitizedWSDLUrlsDefaultValue;
                            }
                        }
                        catch
                        {
                        }
                        finally
                        {
                            settingsInitialized = true;
                        }
                    }
                }
            }
        }

        private static bool TryGetValue(AppSettingsReader appSettingsReader, string key, Type type, out object value)
        {
            try
            {
                value = appSettingsReader.GetValue(key, type);
                return true;
            }
            catch
            {
                value = null;
                return false;
            }
        }

        internal static bool AllowDtdProcessingInWSDL
        {
            get
            {
                EnsureSettingsLoaded();
                return allowDtdProcessingInWSDLValue;
            }
        }

        internal static bool AllowTransparentProxyMessage
        {
            get
            {
                EnsureSettingsLoaded();
                return allowTransparentProxyMessageValue;
            }
        }

        internal static bool AllowUnsanitizedWSDLUrls
        {
            get
            {
                EnsureSettingsLoaded();
                return allowUnsanitizedWSDLUrlsValue;
            }
        }
    }
}

