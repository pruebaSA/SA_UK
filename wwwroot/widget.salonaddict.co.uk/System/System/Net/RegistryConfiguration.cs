namespace System.Net
{
    using Microsoft.Win32;
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Security;
    using System.Security.Permissions;

    internal static class RegistryConfiguration
    {
        private const string netFrameworkFullPath = @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\.NETFramework";
        private const string netFrameworkPath = @"SOFTWARE\Microsoft\.NETFramework";
        private const string netFrameworkVersionedPath = @"SOFTWARE\Microsoft\.NETFramework\v{0}";

        [RegistryPermission(SecurityAction.Assert, Read=@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\.NETFramework")]
        public static int AppConfigReadInt(string configVariable, int defaultValue)
        {
            object obj2 = ReadConfig(GetAppConfigPath(configVariable), GetAppConfigValueName(), RegistryValueKind.DWord);
            if (obj2 != null)
            {
                return (int) obj2;
            }
            return defaultValue;
        }

        [RegistryPermission(SecurityAction.Assert, Read=@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\.NETFramework")]
        public static string AppConfigReadString(string configVariable, string defaultValue)
        {
            object obj2 = ReadConfig(GetAppConfigPath(configVariable), GetAppConfigValueName(), RegistryValueKind.String);
            if (obj2 != null)
            {
                return (string) obj2;
            }
            return defaultValue;
        }

        private static string GetAppConfigPath(string valueName) => 
            string.Format(CultureInfo.InvariantCulture, @"{0}\{1}", new object[] { GetNetFrameworkVersionedPath(), valueName });

        [SecurityPermission(SecurityAction.Assert, UnmanagedCode=true)]
        private static string GetAppConfigValueName()
        {
            string path = "Unknown";
            Process currentProcess = Process.GetCurrentProcess();
            try
            {
                path = currentProcess.MainModule.FileName;
            }
            catch (NotSupportedException)
            {
            }
            catch (Win32Exception)
            {
            }
            catch (InvalidOperationException)
            {
            }
            try
            {
                path = Path.GetFullPath(path);
            }
            catch (ArgumentException)
            {
            }
            catch (SecurityException)
            {
            }
            catch (NotSupportedException)
            {
            }
            catch (PathTooLongException)
            {
            }
            return path;
        }

        private static string GetNetFrameworkVersionedPath() => 
            string.Format(CultureInfo.InvariantCulture, @"SOFTWARE\Microsoft\.NETFramework\v{0}", new object[] { Environment.Version.ToString(3) });

        [RegistryPermission(SecurityAction.Assert, Read=@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\.NETFramework")]
        public static int GlobalConfigReadInt(string configVariable, int defaultValue)
        {
            object obj2 = ReadConfig(GetNetFrameworkVersionedPath(), configVariable, RegistryValueKind.DWord);
            if (obj2 != null)
            {
                return (int) obj2;
            }
            return defaultValue;
        }

        [RegistryPermission(SecurityAction.Assert, Read=@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\.NETFramework")]
        public static string GlobalConfigReadString(string configVariable, string defaultValue)
        {
            object obj2 = ReadConfig(GetNetFrameworkVersionedPath(), configVariable, RegistryValueKind.String);
            if (obj2 != null)
            {
                return (string) obj2;
            }
            return defaultValue;
        }

        private static object ReadConfig(string path, string valueName, RegistryValueKind kind)
        {
            object obj2 = null;
            try
            {
                using (RegistryKey key = Registry.LocalMachine.OpenSubKey(path))
                {
                    if (key != null)
                    {
                        try
                        {
                            object obj3 = key.GetValue(valueName, null);
                            if ((obj3 != null) && (key.GetValueKind(valueName) == kind))
                            {
                                obj2 = obj3;
                            }
                        }
                        catch (UnauthorizedAccessException)
                        {
                        }
                        catch (IOException)
                        {
                        }
                    }
                    return obj2;
                }
            }
            catch (SecurityException)
            {
            }
            catch (ObjectDisposedException)
            {
            }
            return obj2;
        }
    }
}

