namespace MS.Internal
{
    using Microsoft.Win32;
    using MS.Internal.WindowsBase;
    using MS.Win32;
    using System;
    using System.Runtime.CompilerServices;
    using System.Security;
    using System.Security.Permissions;
    using System.Threading;

    [FriendAccessAllowed]
    internal class TextServicesLoader
    {
        private const int CLSIDLength = 0x26;
        private const int LANGIDLength = 10;
        private static InstallState s_servicesInstalled = InstallState.Unknown;
        private static object s_servicesInstalledLock = new object();

        private TextServicesLoader()
        {
        }

        private static EnableState IsAssemblyEnabled(RegistryKey key, string subKeyName, bool localMachine)
        {
            if (subKeyName.Length != 0x26)
            {
                return EnableState.Error;
            }
            RegistryKey key2 = key.OpenSubKey(subKeyName);
            if (key2 == null)
            {
                return EnableState.Error;
            }
            object obj2 = key2.GetValue("Enable");
            if (!(obj2 is int))
            {
                return EnableState.None;
            }
            if (((int) obj2) != 0)
            {
                return EnableState.Enabled;
            }
            return EnableState.Disabled;
        }

        private static EnableState IsLangidEnabled(RegistryKey key, string subKeyName, bool localMachine)
        {
            if (subKeyName.Length != 10)
            {
                return EnableState.Error;
            }
            return IterateSubKeys(key, subKeyName, new IterateHandler(TextServicesLoader.IsAssemblyEnabled), localMachine);
        }

        private static EnableState IterateSubKeys(RegistryKey keyBase, string subKey, IterateHandler handler, bool localMachine)
        {
            RegistryKey key = keyBase.OpenSubKey(subKey, false);
            if (key == null)
            {
                return EnableState.Error;
            }
            string[] subKeyNames = key.GetSubKeyNames();
            EnableState error = EnableState.Error;
            foreach (string str in subKeyNames)
            {
                switch (handler(key, str, localMachine))
                {
                    case EnableState.Error:
                    {
                        continue;
                    }
                    case EnableState.None:
                        if (!localMachine)
                        {
                            break;
                        }
                        return EnableState.None;

                    case EnableState.Enabled:
                        return EnableState.Enabled;

                    case EnableState.Disabled:
                    {
                        error = EnableState.Disabled;
                        continue;
                    }
                    default:
                    {
                        continue;
                    }
                }
                if (error == EnableState.Error)
                {
                    error = EnableState.None;
                }
            }
            return error;
        }

        [SecurityCritical]
        internal static MS.Win32.UnsafeNativeMethods.ITfThreadMgr Load()
        {
            MS.Win32.UnsafeNativeMethods.ITfThreadMgr mgr;
            Invariant.Assert(Thread.CurrentThread.GetApartmentState() == ApartmentState.STA, "Load called on MTA thread!");
            if (ServicesInstalled && (MS.Win32.UnsafeNativeMethods.TF_CreateThreadMgr(out mgr) == 0))
            {
                return mgr;
            }
            return null;
        }

        private static EnableState SingleTIPWantsToRun(RegistryKey keyLocalMachine, string subKeyName, bool localMachine)
        {
            if (subKeyName.Length != 0x26)
            {
                return EnableState.Disabled;
            }
            EnableState enabled = IterateSubKeys(Registry.CurrentUser, @"SOFTWARE\Microsoft\CTF\TIP\" + subKeyName + @"\LanguageProfile", new IterateHandler(TextServicesLoader.IsLangidEnabled), false);
            switch (enabled)
            {
                case EnableState.None:
                case EnableState.Error:
                    enabled = IterateSubKeys(keyLocalMachine, subKeyName + @"\LanguageProfile", new IterateHandler(TextServicesLoader.IsLangidEnabled), true);
                    if (enabled == EnableState.None)
                    {
                        enabled = EnableState.Enabled;
                    }
                    break;
            }
            return enabled;
        }

        [SecurityCritical, SecurityTreatAsSafe]
        private static bool TIPsWantToRun()
        {
            bool flag = false;
            PermissionSet set = new PermissionSet(PermissionState.None);
            set.AddPermission(new RegistryPermission(RegistryPermissionAccess.Read, @"HKEY_LOCAL_MACHINE\Software\Microsoft\CTF"));
            set.AddPermission(new RegistryPermission(RegistryPermissionAccess.Read, @"HKEY_CURRENT_USER\Software\Microsoft\CTF"));
            set.Assert();
            try
            {
                RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\CTF", false);
                if (key != null)
                {
                    object obj2 = key.GetValue("Disable Thread Input Manager");
                    if ((obj2 is int) && (((int) obj2) != 0))
                    {
                        return false;
                    }
                }
                flag = IterateSubKeys(Registry.LocalMachine, @"SOFTWARE\Microsoft\CTF\TIP", new IterateHandler(TextServicesLoader.SingleTIPWantsToRun), true) == EnableState.Enabled;
            }
            finally
            {
                CodeAccessPermission.RevertAssert();
            }
            return flag;
        }

        internal static bool ServicesInstalled
        {
            get
            {
                lock (s_servicesInstalledLock)
                {
                    if (s_servicesInstalled == InstallState.Unknown)
                    {
                        s_servicesInstalled = TIPsWantToRun() ? InstallState.Installed : InstallState.NotInstalled;
                    }
                }
                return (s_servicesInstalled == InstallState.Installed);
            }
        }

        private enum EnableState
        {
            Error,
            None,
            Enabled,
            Disabled
        }

        private enum InstallState
        {
            Unknown,
            Installed,
            NotInstalled
        }

        private delegate TextServicesLoader.EnableState IterateHandler(RegistryKey key, string subKeyName, bool localMachine);
    }
}

