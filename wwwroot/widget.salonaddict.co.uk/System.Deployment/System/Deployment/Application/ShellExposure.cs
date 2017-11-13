namespace System.Deployment.Application
{
    using Microsoft.Win32;
    using System;
    using System.Deployment.Application.Manifest;
    using System.Globalization;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Text;

    internal static class ShellExposure
    {
        private static void AddFileAssociation(FileAssociation fileAssociation, DefinitionIdentity subId, Uri deploymentProviderUri)
        {
            RegistryKey key = Registry.ClassesRoot.OpenSubKey(fileAssociation.Extension);
            RegistryKey key2 = Registry.ClassesRoot.OpenSubKey(fileAssociation.ProgID);
            if ((key != null) || (key2 != null))
            {
                Logger.AddWarningInformation(string.Format(CultureInfo.CurrentUICulture, Resources.GetString("SkippedFileAssoc"), new object[] { fileAssociation.Extension }));
            }
            else
            {
                string str = Guid.NewGuid().ToString("B");
                string str2 = subId.ToString();
                using (RegistryKey key3 = Registry.CurrentUser.CreateSubKey(@"Software\Classes"))
                {
                    using (RegistryKey key4 = key3.CreateSubKey(fileAssociation.Extension))
                    {
                        key4.SetValue(null, fileAssociation.ProgID);
                        key4.SetValue("AppId", str2);
                        key4.SetValue("Guid", str);
                        key4.SetValue("DeploymentProviderUrl", deploymentProviderUri.AbsoluteUri);
                    }
                    using (RegistryKey key5 = key3.CreateSubKey(fileAssociation.ProgID))
                    {
                        key5.SetValue(null, fileAssociation.Description);
                        key5.SetValue("AppId", str2);
                        key5.SetValue("Guid", str);
                        key5.SetValue("DeploymentProviderUrl", deploymentProviderUri.AbsoluteUri);
                        using (RegistryKey key6 = key5.CreateSubKey("shell"))
                        {
                            key6.SetValue(null, "open");
                            using (RegistryKey key7 = key6.CreateSubKey(@"open\command"))
                            {
                                key7.SetValue(null, "rundll32.exe dfshim.dll, ShOpenVerbExtension " + str + " %1");
                            }
                            using (RegistryKey key8 = key5.CreateSubKey(@"shellex\IconHandler"))
                            {
                                key8.SetValue(null, str);
                            }
                        }
                    }
                    using (RegistryKey key9 = key3.CreateSubKey("CLSID"))
                    {
                        using (RegistryKey key10 = key9.CreateSubKey(str))
                        {
                            key10.SetValue(null, "Shell Icon Handler For " + fileAssociation.Description);
                            key10.SetValue("AppId", str2);
                            key10.SetValue("DeploymentProviderUrl", deploymentProviderUri.AbsoluteUri);
                            key10.SetValue("IconFile", fileAssociation.DefaultIcon);
                            using (RegistryKey key11 = key10.CreateSubKey("InProcServer32"))
                            {
                                key11.SetValue(null, "dfshim.dll");
                                key11.SetValue("ThreadingModel", "Apartment");
                            }
                        }
                    }
                }
            }
        }

        private static void AddShellExtensions(DefinitionIdentity subId, Uri deploymentProviderUri, AssemblyManifest appManifest)
        {
            foreach (FileAssociation association in appManifest.FileAssociations)
            {
                AddFileAssociation(association, subId, deploymentProviderUri);
            }
        }

        private static void GenerateAppShortcut(SubscriptionState subState, ShellExposureInformation shellExposureInformation)
        {
            using (StreamWriter writer = new StreamWriter(shellExposureInformation.ApplicationShortcutPath, false, Encoding.Unicode))
            {
                writer.Write("{0}#{1}", subState.DeploymentProviderUri.AbsoluteUri, subState.SubscriptionId.ToString());
            }
            if (subState.CurrentDeploymentManifest.Deployment.CreateDesktopShortcut)
            {
                using (StreamWriter writer2 = new StreamWriter(shellExposureInformation.DesktopShortcutPath, false, Encoding.Unicode))
                {
                    writer2.Write("{0}#{1}", subState.DeploymentProviderUri.AbsoluteUri, subState.SubscriptionId.ToString());
                }
            }
        }

        private static string GenerateArpKeyName(DefinitionIdentity subId) => 
            string.Format(CultureInfo.InvariantCulture, "{0:x16}", new object[] { subId.Hash });

        private static void GenerateSupportShortcut(SubscriptionState subState, ShellExposureInformation shellExposureInformation)
        {
            Description effectiveDescription = subState.EffectiveDescription;
            if (effectiveDescription.SupportUri != null)
            {
                using (StreamWriter writer = new StreamWriter(shellExposureInformation.SupportShortcutPath, false, Encoding.ASCII))
                {
                    writer.WriteLine("[Default]");
                    writer.WriteLine("BASEURL=" + effectiveDescription.SupportUri.AbsoluteUri);
                    writer.WriteLine("[InternetShortcut]");
                    writer.WriteLine("URL=" + effectiveDescription.SupportUri.AbsoluteUri);
                    writer.WriteLine();
                    writer.WriteLine("IconFile=" + PathHelper.ShortShimDllPath);
                    writer.WriteLine("IconIndex=" + 0.ToString(CultureInfo.InvariantCulture));
                    writer.WriteLine();
                }
            }
        }

        private static void MoveDeleteEmptyFolder(string folderPath)
        {
            if (Directory.Exists(folderPath) && (Directory.GetFiles(folderPath).Length <= 0))
            {
                string path = folderPath;
                string destDirName = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
                try
                {
                    Directory.Move(folderPath, destDirName);
                    path = destDirName;
                }
                catch (IOException)
                {
                }
                catch (UnauthorizedAccessException)
                {
                }
                try
                {
                    Directory.Delete(path);
                }
                catch (IOException)
                {
                }
                catch (UnauthorizedAccessException)
                {
                }
            }
        }

        private static void MoveDeleteFile(string filePath)
        {
            if (System.IO.File.Exists(filePath))
            {
                string path = filePath;
                string destFileName = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
                try
                {
                    System.IO.File.Move(filePath, destFileName);
                    path = destFileName;
                }
                catch (IOException)
                {
                }
                catch (UnauthorizedAccessException)
                {
                }
                try
                {
                    System.IO.File.Delete(path);
                }
                catch (IOException)
                {
                }
                catch (UnauthorizedAccessException)
                {
                }
            }
        }

        public static void ParseAppShortcut(string shortcutFile, out DefinitionIdentity subId, out Uri providerUri)
        {
            FileInfo info = new FileInfo(shortcutFile);
            if (info.Length > 0x10000L)
            {
                throw new DeploymentException(ExceptionTypes.InvalidShortcut, Resources.GetString("Ex_ShortcutTooLarge"));
            }
            using (StreamReader reader = new StreamReader(shortcutFile, Encoding.Unicode))
            {
                string str;
                try
                {
                    str = reader.ReadToEnd();
                }
                catch (IOException exception)
                {
                    throw new DeploymentException(ExceptionTypes.InvalidShortcut, Resources.GetString("Ex_InvalidShortcutFormat"), exception);
                }
                if (str == null)
                {
                    throw new DeploymentException(ExceptionTypes.InvalidShortcut, Resources.GetString("Ex_InvalidShortcutFormat"));
                }
                int index = str.IndexOf('#');
                if (index < 0)
                {
                    throw new DeploymentException(ExceptionTypes.InvalidShortcut, Resources.GetString("Ex_InvalidShortcutFormat"));
                }
                try
                {
                    subId = new DefinitionIdentity(str.Substring(index + 1));
                }
                catch (COMException exception2)
                {
                    throw new DeploymentException(ExceptionTypes.InvalidShortcut, Resources.GetString("Ex_InvalidShortcutFormat"), exception2);
                }
                catch (SEHException exception3)
                {
                    throw new DeploymentException(ExceptionTypes.InvalidShortcut, Resources.GetString("Ex_InvalidShortcutFormat"), exception3);
                }
                try
                {
                    providerUri = new Uri(str.Substring(0, index));
                }
                catch (UriFormatException exception4)
                {
                    throw new DeploymentException(ExceptionTypes.InvalidShortcut, Resources.GetString("Ex_InvalidShortcutFormat"), exception4);
                }
            }
        }

        private static void RemoveArpEntry(DefinitionIdentity subId)
        {
            using (RegistryKey key = UninstallRoot.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall", true))
            {
                string subkey = null;
                try
                {
                    if (key != null)
                    {
                        subkey = GenerateArpKeyName(subId);
                        key.DeleteSubKeyTree(subkey);
                    }
                }
                catch (ArgumentException exception)
                {
                    throw new DeploymentException(ExceptionTypes.InvalidARPEntry, string.Format(CultureInfo.CurrentUICulture, Resources.GetString("Ex_ArpEntryRemovalFailure"), new object[] { subkey }), exception);
                }
            }
        }

        private static void RemoveFileAssociation(FileAssociation fileAssociation, DefinitionIdentity subId, string productName)
        {
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\Classes", true))
            {
                if (key != null)
                {
                    RemoveFileAssociationExtentionInfo(fileAssociation, subId, key, productName);
                    string clsIdString = RemoveFileAssociationProgIDInfo(fileAssociation, subId, key, productName);
                    if (clsIdString != null)
                    {
                        RemoveFileAssociationCLSIDInfo(fileAssociation, subId, key, clsIdString, productName);
                    }
                }
            }
        }

        private static void RemoveFileAssociationCLSIDInfo(FileAssociation fileAssociation, DefinitionIdentity subId, RegistryKey classesKey, string clsIdString, string productName)
        {
            using (RegistryKey key = classesKey.OpenSubKey("CLSID", true))
            {
                if (key != null)
                {
                    using (RegistryKey key2 = key.OpenSubKey(clsIdString))
                    {
                        object obj2 = key2.GetValue("AppId");
                        if (obj2 is string)
                        {
                            string a = (string) obj2;
                            if (string.Equals(a, subId.ToString(), StringComparison.Ordinal))
                            {
                                try
                                {
                                    key.DeleteSubKeyTree(clsIdString);
                                }
                                catch (ArgumentException exception)
                                {
                                    throw new DeploymentException(ExceptionTypes.InvalidARPEntry, string.Format(CultureInfo.CurrentUICulture, Resources.GetString("Ex_FileAssocCLSIDDeleteFailed"), new object[] { clsIdString, productName }), exception);
                                }
                            }
                        }
                    }
                }
            }
        }

        private static void RemoveFileAssociationExtentionInfo(FileAssociation fileAssociation, DefinitionIdentity subId, RegistryKey classesKey, string productName)
        {
            using (RegistryKey key = classesKey.OpenSubKey(fileAssociation.Extension, true))
            {
                if (key != null)
                {
                    object obj2 = key.GetValue("AppId");
                    if (obj2 is string)
                    {
                        string a = (string) obj2;
                        if (string.Equals(a, subId.ToString(), StringComparison.Ordinal))
                        {
                            try
                            {
                                classesKey.DeleteSubKeyTree(fileAssociation.Extension);
                            }
                            catch (ArgumentException exception)
                            {
                                throw new DeploymentException(ExceptionTypes.InvalidARPEntry, string.Format(CultureInfo.CurrentUICulture, Resources.GetString("Ex_FileAssocExtDeleteFailed"), new object[] { fileAssociation.Extension, productName }), exception);
                            }
                        }
                    }
                }
            }
        }

        private static string RemoveFileAssociationProgIDInfo(FileAssociation fileAssociation, DefinitionIdentity subId, RegistryKey classesKey, string productName)
        {
            string str = null;
            using (RegistryKey key = classesKey.OpenSubKey(fileAssociation.ProgID, true))
            {
                if (key == null)
                {
                    return null;
                }
                object obj2 = key.GetValue("AppId");
                if (!(obj2 is string))
                {
                    return null;
                }
                string a = (string) obj2;
                if (!string.Equals(a, subId.ToString(), StringComparison.Ordinal))
                {
                    return null;
                }
                str = (string) key.GetValue("Guid");
                try
                {
                    classesKey.DeleteSubKeyTree(fileAssociation.ProgID);
                }
                catch (ArgumentException exception)
                {
                    throw new DeploymentException(ExceptionTypes.InvalidARPEntry, string.Format(CultureInfo.CurrentUICulture, Resources.GetString("Ex_FileAssocProgIdDeleteFailed"), new object[] { fileAssociation.ProgID, productName }), exception);
                }
            }
            return str;
        }

        internal static void RemovePins(SubscriptionState subState)
        {
            ShellExposureInformation information = ShellExposureInformation.CreateShellExposureInformation(subState.SubscriptionId);
            if ((information != null) && System.IO.File.Exists(information.ApplicationShortcutPath))
            {
                UnpinShortcut(information.ApplicationShortcutPath);
            }
        }

        public static void RemoveShellExtensions(DefinitionIdentity subId, AssemblyManifest appManifest, string productName)
        {
            foreach (FileAssociation association in appManifest.FileAssociations)
            {
                RemoveFileAssociation(association, subId, productName);
            }
            System.Deployment.Application.NativeMethods.SHChangeNotify(0x8000000, 0, IntPtr.Zero, IntPtr.Zero);
        }

        private static void RemoveShortcuts(ShellExposureInformation shellExposureInformation)
        {
            try
            {
                if (System.IO.File.Exists(shellExposureInformation.ApplicationShortcutPath))
                {
                    System.IO.File.Delete(shellExposureInformation.ApplicationShortcutPath);
                }
                if (System.IO.File.Exists(shellExposureInformation.SupportShortcutPath))
                {
                    System.IO.File.Delete(shellExposureInformation.SupportShortcutPath);
                }
                if (System.IO.File.Exists(shellExposureInformation.DesktopShortcutPath))
                {
                    System.IO.File.Delete(shellExposureInformation.DesktopShortcutPath);
                }
                if (Directory.Exists(shellExposureInformation.ApplicationFolderPath))
                {
                    string[] files = Directory.GetFiles(shellExposureInformation.ApplicationFolderPath);
                    string[] directories = Directory.GetDirectories(shellExposureInformation.ApplicationFolderPath);
                    if ((files.Length == 0) && (directories.Length == 0))
                    {
                        Directory.Delete(shellExposureInformation.ApplicationFolderPath);
                    }
                }
                if (Directory.Exists(shellExposureInformation.ApplicationRootFolderPath))
                {
                    string[] strArray3 = Directory.GetFiles(shellExposureInformation.ApplicationRootFolderPath);
                    string[] strArray4 = Directory.GetDirectories(shellExposureInformation.ApplicationRootFolderPath);
                    if ((strArray3.Length == 0) && (strArray4.Length == 0))
                    {
                        Directory.Delete(shellExposureInformation.ApplicationRootFolderPath);
                    }
                }
            }
            catch (IOException exception)
            {
                throw new DeploymentException(ExceptionTypes.InvalidShortcut, Resources.GetString("Ex_ShortcutRemovalFailure"), exception);
            }
            catch (UnauthorizedAccessException exception2)
            {
                throw new DeploymentException(ExceptionTypes.InvalidShortcut, Resources.GetString("Ex_ShortcutRemovalFailure"), exception2);
            }
        }

        public static void RemoveSubscriptionShellExposure(SubscriptionState subState)
        {
            using (subState.SubscriptionStore.AcquireStoreWriterLock())
            {
                DefinitionIdentity subscriptionId = subState.SubscriptionId;
                bool flag = false;
                ShellExposureInformation shellExposureInformation = ShellExposureInformation.CreateShellExposureInformation(subscriptionId);
                if (shellExposureInformation == null)
                {
                    flag = true;
                }
                else
                {
                    RemoveShortcuts(shellExposureInformation);
                }
                RemoveArpEntry(subscriptionId);
                if (flag)
                {
                    throw new DeploymentException(ExceptionTypes.Subscription, Resources.GetString("Ex_ShortcutRemovalFailureDueToInvalidPublisherProduct"));
                }
            }
        }

        private static void UnpinShortcut(string shortcutPath)
        {
            System.Deployment.Application.NativeMethods.IShellItem psi = null;
            System.Deployment.Application.NativeMethods.IStartMenuPinnedList o = null;
            try
            {
                object ppv = null;
                object obj3 = null;
                if (System.Deployment.Application.NativeMethods.SHCreateItemFromParsingName(shortcutPath, IntPtr.Zero, Constants.uuid, out ppv) == 0)
                {
                    psi = ppv as System.Deployment.Application.NativeMethods.IShellItem;
                    if (System.Deployment.Application.NativeMethods.CoCreateInstance(ref Constants.CLSID_StartMenuPin, null, 1, ref Constants.IID_IUnknown, out obj3) == 0)
                    {
                        o = obj3 as System.Deployment.Application.NativeMethods.IStartMenuPinnedList;
                        o.RemoveFromList(psi);
                    }
                }
            }
            catch (EntryPointNotFoundException)
            {
            }
            finally
            {
                if (psi != null)
                {
                    Marshal.ReleaseComObject(psi);
                }
                if (o != null)
                {
                    Marshal.ReleaseComObject(o);
                }
            }
        }

        private static void UpdateArpEntry(SubscriptionState subState, ShellExposureInformation shellExposureInformation)
        {
            DefinitionIdentity subscriptionId = subState.SubscriptionId;
            string str = string.Format(CultureInfo.InvariantCulture, "rundll32.exe dfshim.dll,ShArpMaintain {0}", new object[] { subscriptionId.ToString() });
            string str2 = string.Format(CultureInfo.InvariantCulture, "dfshim.dll,2", new object[0]);
            AssemblyManifest currentDeploymentManifest = subState.CurrentDeploymentManifest;
            Description effectiveDescription = subState.EffectiveDescription;
            using (RegistryKey key = UninstallRoot.CreateSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall"))
            {
                using (RegistryKey key2 = key.CreateSubKey(GenerateArpKeyName(subscriptionId)))
                {
                    string[] strArray = new string[] { 
                        "DisplayName", shellExposureInformation.ARPDisplayName, "DisplayIcon", str2, "DisplayVersion", currentDeploymentManifest.Identity.Version.ToString(), "Publisher", effectiveDescription.FilteredPublisher, "UninstallString", str, "HelpLink", effectiveDescription.SupportUrl, "UrlUpdateInfo", subState.DeploymentProviderUri.AbsoluteUri, "ShortcutFolderName", shellExposureInformation.AppVendor,
                        "ShortcutFileName", shellExposureInformation.AppProduct, "ShortcutSuiteName", shellExposureInformation.AppSuiteName, "SupportShortcutFileName", shellExposureInformation.AppSupportShortcut, "ShortcutAppId", shellExposureInformation.ShortcutAppId
                    };
                    for (int i = strArray.Length - 2; i >= 0; i -= 2)
                    {
                        string name = strArray[i];
                        string str4 = strArray[i + 1];
                        if (str4 != null)
                        {
                            key2.SetValue(name, str4);
                        }
                        else
                        {
                            key2.DeleteValue(name, false);
                        }
                    }
                }
            }
        }

        public static void UpdateShellExtensions(SubscriptionState subState, ref ShellExposureInformation shellExposureInformation)
        {
            string productName = null;
            if (shellExposureInformation != null)
            {
                productName = shellExposureInformation.AppProduct;
            }
            if (productName == null)
            {
                productName = subState.SubscriptionId.Name;
            }
            if (subState.PreviousBind != null)
            {
                RemoveShellExtensions(subState.SubscriptionId, subState.PreviousApplicationManifest, productName);
            }
            AddShellExtensions(subState.SubscriptionId, subState.DeploymentProviderUri, subState.CurrentApplicationManifest);
            System.Deployment.Application.NativeMethods.SHChangeNotify(0x8000000, 0, IntPtr.Zero, IntPtr.Zero);
        }

        private static void UpdateShortcuts(SubscriptionState subState, ref ShellExposureInformation shellExposureInformation)
        {
            string strA = $"{subState.DeploymentProviderUri.AbsoluteUri}#{subState.SubscriptionId.ToString()}";
            Description effectiveDescription = subState.EffectiveDescription;
            if (shellExposureInformation != null)
            {
                bool flag = true;
                bool flag2 = true;
                bool flag3 = true;
                bool flag4 = true;
                if (string.Compare(effectiveDescription.FilteredPublisher, shellExposureInformation.AppVendor, StringComparison.Ordinal) == 0)
                {
                    flag = false;
                    if (Utilities.CompareWithNullEqEmpty(effectiveDescription.FilteredSuiteName, shellExposureInformation.AppSuiteName, StringComparison.Ordinal) == 0)
                    {
                        flag2 = false;
                        if (string.Compare(effectiveDescription.FilteredProduct, shellExposureInformation.AppProduct, StringComparison.Ordinal) == 0)
                        {
                            flag3 = false;
                            if (string.Compare(strA, shellExposureInformation.ShortcutAppId, StringComparison.Ordinal) == 0)
                            {
                                flag4 = false;
                            }
                        }
                    }
                }
                if (((!flag && !flag2) && (!flag3 && !flag4)) && System.IO.File.Exists(shellExposureInformation.ApplicationShortcutPath))
                {
                    return;
                }
                if (flag3)
                {
                    UnpinShortcut(shellExposureInformation.ApplicationShortcutPath);
                    MoveDeleteFile(shellExposureInformation.ApplicationShortcutPath);
                    MoveDeleteFile(shellExposureInformation.SupportShortcutPath);
                    MoveDeleteFile(shellExposureInformation.DesktopShortcutPath);
                }
                if (flag2)
                {
                    MoveDeleteEmptyFolder(shellExposureInformation.ApplicationFolderPath);
                }
                if (flag)
                {
                    MoveDeleteEmptyFolder(shellExposureInformation.ApplicationRootFolderPath);
                }
                if ((flag || flag2) || flag3)
                {
                    shellExposureInformation = ShellExposureInformation.CreateShellExposureInformation(effectiveDescription.FilteredPublisher, effectiveDescription.FilteredSuiteName, effectiveDescription.FilteredProduct, strA);
                }
                else
                {
                    shellExposureInformation.ShortcutAppId = strA;
                }
            }
            else
            {
                shellExposureInformation = ShellExposureInformation.CreateShellExposureInformation(effectiveDescription.FilteredPublisher, effectiveDescription.FilteredSuiteName, effectiveDescription.FilteredProduct, strA);
            }
            try
            {
                Directory.CreateDirectory(shellExposureInformation.ApplicationFolderPath);
                GenerateAppShortcut(subState, shellExposureInformation);
                GenerateSupportShortcut(subState, shellExposureInformation);
            }
            catch (Exception)
            {
                RemoveShortcuts(shellExposureInformation);
                throw;
            }
        }

        public static void UpdateSubscriptionShellExposure(SubscriptionState subState)
        {
            using (subState.SubscriptionStore.AcquireStoreWriterLock())
            {
                ShellExposureInformation shellExposureInformation = ShellExposureInformation.CreateShellExposureInformation(subState.SubscriptionId);
                UpdateShortcuts(subState, ref shellExposureInformation);
                UpdateShellExtensions(subState, ref shellExposureInformation);
                UpdateArpEntry(subState, shellExposureInformation);
            }
        }

        private static RegistryKey UninstallRoot
        {
            get
            {
                if (!PlatformSpecific.OnWin9x)
                {
                    return Registry.CurrentUser;
                }
                return Registry.LocalMachine;
            }
        }

        public class ShellExposureInformation
        {
            private string _applicationFolderPath;
            private string _applicationRootFolderPath;
            private string _applicationShortcutPath;
            private string _appProduct;
            private string _appSuiteName;
            private string _appSupportShortcut;
            private string _appVendor;
            private string _desktopShortcutPath;
            private string _shortcutAppId;
            private string _supportShortcutPath;

            protected ShellExposureInformation()
            {
            }

            public static ShellExposure.ShellExposureInformation CreateShellExposureInformation(DefinitionIdentity subscriptionIdentity)
            {
                ShellExposure.ShellExposureInformation information = null;
                string str = null;
                string str2 = null;
                string str3 = null;
                string str4 = null;
                string str5 = "";
                using (RegistryKey key = ShellExposure.UninstallRoot.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall"))
                {
                    if (key != null)
                    {
                        using (RegistryKey key2 = key.OpenSubKey(ShellExposure.GenerateArpKeyName(subscriptionIdentity)))
                        {
                            if (key2 != null)
                            {
                                str = key2.GetValue("ShortcutFolderName") as string;
                                str2 = key2.GetValue("ShortcutFileName") as string;
                                if (key2.GetValue("ShortcutSuiteName") != null)
                                {
                                    str3 = key2.GetValue("ShortcutSuiteName") as string;
                                }
                                else
                                {
                                    str3 = "";
                                }
                                str4 = key2.GetValue("SupportShortcutFileName") as string;
                                if (key2.GetValue("ShortcutAppId") != null)
                                {
                                    str5 = key2.GetValue("ShortcutAppId") as string;
                                }
                                else
                                {
                                    str5 = "";
                                }
                            }
                        }
                    }
                }
                if (((str != null) && (str2 != null)) && (str4 != null))
                {
                    information = new ShellExposure.ShellExposureInformation {
                        _applicationRootFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Programs), str)
                    };
                    if (string.IsNullOrEmpty(str3))
                    {
                        information._applicationFolderPath = information._applicationRootFolderPath;
                    }
                    else
                    {
                        information._applicationFolderPath = Path.Combine(information._applicationRootFolderPath, str3);
                    }
                    information._applicationShortcutPath = Path.Combine(information._applicationFolderPath, str2 + ".appref-ms");
                    information._supportShortcutPath = Path.Combine(information._applicationFolderPath, str4 + ".url");
                    information._desktopShortcutPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), str2 + ".appref-ms");
                    information._appVendor = str;
                    information._appProduct = str2;
                    information._appSupportShortcut = str4;
                    information._shortcutAppId = str5;
                    information._appSuiteName = str3;
                }
                return information;
            }

            public static ShellExposure.ShellExposureInformation CreateShellExposureInformation(string publisher, string suiteName, string product, string shortcutAppId)
            {
                ShellExposure.ShellExposureInformation information = new ShellExposure.ShellExposureInformation();
                string str = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Programs), publisher);
                string str2 = str;
                if (!string.IsNullOrEmpty(suiteName))
                {
                    str2 = Path.Combine(str, suiteName);
                }
                string folderPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                string str4 = null;
                string path = null;
                string str6 = null;
                int num = 0;
                num = 0;
            Label_003B:
                switch (num)
                {
                    case 0x7fffffff:
                        throw new OverflowException();

                    case 0:
                        str4 = string.Format(CultureInfo.CurrentUICulture, Resources.GetString("ShellExposure_DisplayStringNoIndex"), new object[] { product });
                        break;

                    default:
                        str4 = string.Format(CultureInfo.CurrentUICulture, Resources.GetString("ShellExposure_DisplayStringWithIndex"), new object[] { product, num });
                        break;
                }
                path = Path.Combine(str2, str4 + ".appref-ms");
                str6 = Path.Combine(folderPath, str4 + ".appref-ms");
                if (System.IO.File.Exists(path) || System.IO.File.Exists(str6))
                {
                    num++;
                    goto Label_003B;
                }
                information._appVendor = publisher;
                information._appProduct = str4;
                information._appSuiteName = suiteName;
                information._applicationFolderPath = str2;
                information._applicationRootFolderPath = str;
                information._applicationShortcutPath = path;
                information._desktopShortcutPath = str6;
                information._appSupportShortcut = string.Format(CultureInfo.CurrentUICulture, Resources.GetString("SupportUrlFormatter"), new object[] { str4 });
                information._supportShortcutPath = Path.Combine(str2, information._appSupportShortcut + ".url");
                information._shortcutAppId = shortcutAppId;
                return information;
            }

            public string ApplicationFolderPath =>
                this._applicationFolderPath;

            public string ApplicationRootFolderPath =>
                this._applicationRootFolderPath;

            public string ApplicationShortcutPath =>
                this._applicationShortcutPath;

            public string AppProduct =>
                this._appProduct;

            public string AppSuiteName =>
                this._appSuiteName;

            public string AppSupportShortcut =>
                this._appSupportShortcut;

            public string AppVendor =>
                this._appVendor;

            public string ARPDisplayName
            {
                get
                {
                    StringBuilder builder = new StringBuilder();
                    builder.Append(this._appProduct);
                    if (PlatformSpecific.OnWin9x && (builder.Length > 0x3f))
                    {
                        builder.Length = 60;
                        builder.Append("...");
                    }
                    return builder.ToString();
                }
            }

            public string DesktopShortcutPath =>
                this._desktopShortcutPath;

            public string ShortcutAppId
            {
                get => 
                    this._shortcutAppId;
                set
                {
                    this._shortcutAppId = value;
                }
            }

            public string SupportShortcutPath =>
                this._supportShortcutPath;
        }
    }
}

