namespace MS.Internal.WindowsBase
{
    using Microsoft.Win32;
    using MS.Internal.Permissions;
    using System;
    using System.Runtime.InteropServices;
    using System.Security;
    using System.Security.Permissions;

    [FriendAccessAllowed]
    internal static class SecurityHelper
    {
        private static UIPermission _allWindowsUIPermission;
        private static CompoundFileIOPermission _compoundFileIOPermission;
        private static PermissionSet _envelopePermissionSet;
        private static RightsManagementPermission _rightsManagementPermission;
        private static SecurityPermission _unmanagedCodePermission;
        private static UIPermission _unrestrictedUIPermission;

        internal static bool AreStringTypesEqual(string m1, string m2) => 
            (string.Compare(m1, m2, StringComparison.OrdinalIgnoreCase) == 0);

        internal static bool CheckUnmanagedCodePermission()
        {
            try
            {
                DemandUnmanagedCode();
            }
            catch (SecurityException)
            {
                return false;
            }
            return true;
        }

        private static PermissionSet CreateEnvelopePermissionSet()
        {
            PermissionSet set = new PermissionSet(PermissionState.None);
            set.AddPermission(new RightsManagementPermission());
            set.AddPermission(new CompoundFileIOPermission());
            return set;
        }

        internal static void DemandCompoundFileIOPermission()
        {
            if (_compoundFileIOPermission == null)
            {
                _compoundFileIOPermission = new CompoundFileIOPermission();
            }
            _compoundFileIOPermission.Demand();
        }

        internal static void DemandPathDiscovery(string path)
        {
            FileIOPermission permission = new FileIOPermission(PermissionState.None);
            permission.AddPathList(FileIOPermissionAccess.PathDiscovery, path);
            permission.Demand();
        }

        internal static void DemandRightsManagementPermission()
        {
            if (_rightsManagementPermission == null)
            {
                _rightsManagementPermission = new RightsManagementPermission();
            }
            _rightsManagementPermission.Demand();
        }

        internal static void DemandUIWindowPermission()
        {
            if (_allWindowsUIPermission == null)
            {
                _allWindowsUIPermission = new UIPermission(UIPermissionWindow.AllWindows);
            }
            _allWindowsUIPermission.Demand();
        }

        internal static void DemandUnmanagedCode()
        {
            if (_unmanagedCodePermission == null)
            {
                _unmanagedCodePermission = new SecurityPermission(SecurityPermissionFlag.UnmanagedCode);
            }
            _unmanagedCodePermission.Demand();
        }

        internal static void DemandUnrestrictedUIPermission()
        {
            if (_unrestrictedUIPermission == null)
            {
                _unrestrictedUIPermission = new UIPermission(PermissionState.Unrestricted);
            }
            _unrestrictedUIPermission.Demand();
        }

        [SecurityCritical]
        internal static object ReadRegistryValue(RegistryKey baseRegistryKey, string keyName, string valueName)
        {
            object obj2 = null;
            new RegistryPermission(RegistryPermissionAccess.Read, baseRegistryKey.Name + @"\" + keyName).Assert();
            try
            {
                RegistryKey key = baseRegistryKey.OpenSubKey(keyName);
                if (key == null)
                {
                    return obj2;
                }
                using (key)
                {
                    obj2 = key.GetValue(valueName);
                }
            }
            finally
            {
                CodeAccessPermission.RevertAssert();
            }
            return obj2;
        }

        [SecurityTreatAsSafe, SecurityCritical]
        internal static int SizeOf(object o) => 
            Marshal.SizeOf(o);

        [SecurityTreatAsSafe, SecurityCritical]
        internal static int SizeOf(Type t) => 
            Marshal.SizeOf(t);

        [SecurityTreatAsSafe, SecurityCritical]
        internal static void ThrowExceptionForHR(int hr)
        {
            Marshal.ThrowExceptionForHR(hr);
        }

        internal static PermissionSet EnvelopePermissionSet
        {
            get
            {
                if (_envelopePermissionSet == null)
                {
                    _envelopePermissionSet = CreateEnvelopePermissionSet();
                }
                return _envelopePermissionSet;
            }
        }
    }
}

