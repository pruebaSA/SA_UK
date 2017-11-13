namespace MS.Internal
{
    using Microsoft.Win32;
    using MS.Internal.WindowsBase;
    using System;
    using System.Security;
    using System.Security.Permissions;

    [FriendAccessAllowed]
    internal static class RegistryKeys
    {
        internal const string HKCU_XpsViewer = @"HKEY_CURRENT_USER\Software\Microsoft\XPSViewer";
        internal const string HKLM_IetfLanguage = @"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\Nls\IetfLanguage";
        internal const string HKLM_XpsViewerLocalServer32 = @"HKEY_LOCAL_MACHINE\SOFTWARE\Classes\CLSID\{7DDA204B-2097-47C9-8323-C40BB840AE44}\LocalServer32";
        internal const string value_DisableXbapErrorPage = "DisableXbapErrorPage";
        internal const string value_IsolatedStorageUserQuota = "IsolatedStorageUserQuota";
        internal const string value_MediaAudioDisallow = "MediaAudioDisallow";
        internal const string value_MediaImageDisallow = "MediaImageDisallow";
        internal const string value_MediaVideoDisallow = "MediaVideoDisallow";
        internal const string value_UnblockWebBrowserControl = "UnblockWebBrowserControl";
        internal const string value_WebBrowserDisallow = "WebBrowserDisallow";
        internal const string WPF = @"Software\Microsoft\.NETFramework\Windows Presentation Foundation";
        internal const string WPF_Features = @"Software\Microsoft\.NETFramework\Windows Presentation Foundation\Features";
        internal const string WPF_Hosting = @"Software\Microsoft\.NETFramework\Windows Presentation Foundation\Hosting";

        [SecurityCritical]
        internal static bool ReadLocalMachineBool(string key, string valueName)
        {
            string pathList = @"HKEY_LOCAL_MACHINE\" + key;
            new RegistryPermission(RegistryPermissionAccess.Read, pathList).Assert();
            object obj2 = Registry.GetValue(pathList, valueName, null);
            return ((obj2 is int) && (((int) obj2) != 0));
        }
    }
}

