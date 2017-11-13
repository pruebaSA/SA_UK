namespace MS.Internal
{
    using Microsoft.Win32;
    using MS.Internal.WindowsBase;
    using System;
    using System.Diagnostics;
    using System.Security;
    using System.Security.Permissions;
    using System.Windows;

    [FriendAccessAllowed]
    internal static class Invariant
    {
        [SecurityTreatAsSafe, SecurityCritical]
        private static bool _strict = false;
        private const bool _strictDefaultValue = false;

        internal static void Assert(bool condition)
        {
            if (!condition)
            {
                FailFast(null, null);
            }
        }

        internal static void Assert(bool condition, string invariantMessage)
        {
            if (!condition)
            {
                FailFast(invariantMessage, null);
            }
        }

        internal static void Assert(bool condition, string invariantMessage, string detailMessage)
        {
            if (!condition)
            {
                FailFast(invariantMessage, detailMessage);
            }
        }

        [SecurityTreatAsSafe, SecurityCritical]
        private static void FailFast(string message, string detailMessage)
        {
            if (IsDialogOverrideEnabled)
            {
                Debugger.Break();
            }
            Environment.FailFast(System.Windows.SR.Get("InvariantFailure"));
        }

        private static bool IsDialogOverrideEnabled
        {
            [SecurityTreatAsSafe, SecurityCritical]
            get
            {
                RegistryKey key;
                object obj2;
                string str;
                bool flag = false;
                PermissionSet set = new PermissionSet(PermissionState.None);
                RegistryPermission perm = new RegistryPermission(RegistryPermissionAccess.Read, @"HKEY_LOCAL_MACHINE\Software\Microsoft\.NetFramework");
                set.AddPermission(perm);
                set.Assert();
                try
                {
                    key = Registry.LocalMachine.OpenSubKey(@"Software\Microsoft\.NETFramework");
                    obj2 = key.GetValue("DbgJITDebugLaunchSetting");
                    str = key.GetValue("DbgManagedDebugger") as string;
                }
                finally
                {
                    PermissionSet.RevertAssert();
                }
                if (key != null)
                {
                    flag = (obj2 is int) && ((((int) obj2) & 2) != 0);
                    if (flag)
                    {
                        flag = (str != null) && (str.Length > 0);
                    }
                }
                return flag;
            }
        }

        internal static bool Strict
        {
            get => 
                _strict;
            set
            {
                _strict = value;
            }
        }
    }
}

