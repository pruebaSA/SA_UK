namespace System.Windows
{
    using Microsoft.Win32;
    using System;
    using System.Collections.Specialized;
    using System.Configuration;
    using System.Security;
    using System.Security.Permissions;
    using System.Threading;

    internal static class BaseCompatibilityPreferences
    {
        private static object _lockObject = new object();
        private static bool _matchPackageSignatureMethodToPackagePartDigestMethod = true;
        private const string MatchPackageSignatureMethodToPackagePartDigestMethodValue = "MatchPackageSignatureMethodToPackagePartDigestMethod";
        private const string WpfPackagingKey = @"HKEY_CURRENT_USER\Software\Microsoft\Avalon.Packaging\";
        private const string WpfPackagingSubKeyPath = @"Software\Microsoft\Avalon.Packaging\";

        static BaseCompatibilityPreferences()
        {
            NameValueCollection appSettings = null;
            try
            {
                appSettings = ConfigurationManager.AppSettings;
            }
            catch (ConfigurationErrorsException)
            {
            }
            SetMatchPackageSignatureMethodToPackagePartDigestMethod(appSettings);
        }

        private static void SetMatchPackageSignatureMethodToPackagePartDigestMethod(NameValueCollection appSettings)
        {
            if ((appSettings == null) || !SetMatchPackageSignatureMethodToPackagePartDigestMethodFromAppSettings(appSettings))
            {
                SetMatchPackageSignatureMethodToPackagePartDigestMethodFromRegistry();
            }
        }

        private static bool SetMatchPackageSignatureMethodToPackagePartDigestMethodFromAppSettings(NameValueCollection appSettings)
        {
            bool flag;
            string str = appSettings["MatchPackageSignatureMethodToPackagePartDigestMethod"];
            if (bool.TryParse(str, out flag))
            {
                MatchPackageSignatureMethodToPackagePartDigestMethod = flag;
                return true;
            }
            return false;
        }

        [SecurityTreatAsSafe, SecurityCritical, RegistryPermission(SecurityAction.Assert, Read=@"HKEY_CURRENT_USER\Software\Microsoft\Avalon.Packaging\", Unrestricted=true)]
        private static void SetMatchPackageSignatureMethodToPackagePartDigestMethodFromRegistry()
        {
            try
            {
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Avalon.Packaging\", RegistryKeyPermissionCheck.ReadSubTree))
                {
                    if ((key != null) && (key.GetValueKind("MatchPackageSignatureMethodToPackagePartDigestMethod") == RegistryValueKind.DWord))
                    {
                        object obj2 = key.GetValue("MatchPackageSignatureMethodToPackagePartDigestMethod");
                        if (obj2 != null)
                        {
                            MatchPackageSignatureMethodToPackagePartDigestMethod = ((int) obj2) == 1;
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                if (((exception is StackOverflowException) || (exception is OutOfMemoryException)) || ((exception is ThreadAbortException) || (exception is AccessViolationException)))
                {
                    throw;
                }
            }
        }

        internal static bool MatchPackageSignatureMethodToPackagePartDigestMethod
        {
            get => 
                _matchPackageSignatureMethodToPackagePartDigestMethod;
            set
            {
                _matchPackageSignatureMethodToPackagePartDigestMethod = value;
            }
        }
    }
}

