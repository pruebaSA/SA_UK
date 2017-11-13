namespace System.EnterpriseServices
{
    using Microsoft.Win32;
    using System;
    using System.Diagnostics;
    using System.EnterpriseServices.Admin;
    using System.EnterpriseServices.Thunk;
    using System.Globalization;
    using System.Runtime.InteropServices;
    using System.Security.Permissions;

    [Transaction(TransactionOption.RequiresNew), Guid("c89ac250-e18a-4fc7-abd5-b8897b6a78a5")]
    public sealed class RegistrationHelperTx : ServicedComponent
    {
        private static Guid _appid;
        private static Guid _appidNoWow64 = new Guid("1e246775-2281-484f-8ad4-044c15b86eb7");
        private static Guid _appidWow64 = new Guid("57926702-ab7c-402b-abce-e262da1dd7c9");
        private static string _appname;
        private static string _appnameNoWow64 = ".NET Utilities";
        private static string _appnameWow64 = ".NET Utilities (32 bit)";
        private static bool _isRunningInWow64;

        static RegistrationHelperTx()
        {
            if (Wow64Helper.IsWow64Process())
            {
                _appid = _appidWow64;
                _appname = _appnameWow64;
                _isRunningInWow64 = true;
            }
            else
            {
                _appid = _appidNoWow64;
                _appname = _appnameNoWow64;
                _isRunningInWow64 = false;
            }
        }

        protected internal override void Activate()
        {
        }

        private static void ConfigureComponent(ICatalogCollection coll, ICatalogObject obj)
        {
            obj.SetValue("Transaction", TransactionOption.RequiresNew);
            obj.SetValue("ComponentTransactionTimeoutEnabled", true);
            obj.SetValue("ComponentTransactionTimeout", 0);
            coll.SaveChanges();
        }

        protected internal override void Deactivate()
        {
        }

        private static ICatalogObject FindApplication(ICatalogCollection coll, Guid appid, ref int idx)
        {
            int num = coll.Count();
            for (int i = 0; i < num; i++)
            {
                ICatalogObject obj2 = (ICatalogObject) coll.Item(i);
                Guid guid = new Guid((string) obj2.GetValue("ID"));
                if (guid == appid)
                {
                    idx = i;
                    return obj2;
                }
            }
            return null;
        }

        private static ICatalogObject FindComponent(ICatalogCollection coll, Guid clsid, ref int idx)
        {
            RegistrationDriver.Populate(coll);
            int num = coll.Count();
            for (int i = 0; i < num; i++)
            {
                ICatalogObject obj2 = (ICatalogObject) coll.Item(i);
                Guid guid = new Guid((string) obj2.GetValue("CLSID"));
                if (guid == clsid)
                {
                    idx = i;
                    return obj2;
                }
            }
            return null;
        }

        public void InstallAssembly(string assembly, ref string application, ref string tlb, InstallationFlags installFlags, object sync)
        {
            this.InstallAssembly(assembly, ref application, null, ref tlb, installFlags, sync);
        }

        public void InstallAssembly(string assembly, ref string application, string partition, ref string tlb, InstallationFlags installFlags, object sync)
        {
            RegistrationConfig regConfig = new RegistrationConfig {
                AssemblyFile = assembly,
                Application = application,
                Partition = partition,
                TypeLibrary = tlb,
                InstallationFlags = installFlags
            };
            this.InstallAssemblyFromConfig(ref regConfig, sync);
            application = regConfig.AssemblyFile;
            tlb = regConfig.TypeLibrary;
        }

        public void InstallAssemblyFromConfig([MarshalAs(UnmanagedType.IUnknown)] ref RegistrationConfig regConfig, object sync)
        {
            bool flag = false;
            try
            {
                new RegistrationDriver().InstallAssembly(regConfig, sync);
                ContextUtil.SetComplete();
                flag = true;
            }
            finally
            {
                if (!flag)
                {
                    ContextUtil.SetAbort();
                }
            }
        }

        [ComRegisterFunction]
        internal static void InstallUtilityApplication(Type t)
        {
            if (!Platform.Supports(PlatformFeature.SWC))
            {
                try
                {
                    if (!Platform.IsLessThan(Platform.W2K))
                    {
                        ICatalog catalog = null;
                        ICatalogCollection coll = null;
                        ICatalogObject obj2 = null;
                        int idx = 0;
                        catalog = (ICatalog) new xCatalog();
                        if (!Platform.IsLessThan(Platform.Whistler))
                        {
                            ICatalog2 catalog2 = catalog as ICatalog2;
                            if (catalog2 != null)
                            {
                                catalog2.CurrentPartition(catalog2.GlobalPartitionID());
                            }
                        }
                        coll = (ICatalogCollection) catalog.GetCollection("Applications");
                        RegistrationDriver.Populate(coll);
                        obj2 = FindApplication(coll, _appid, ref idx);
                        if (obj2 == null)
                        {
                            obj2 = (ICatalogObject) coll.Add();
                            obj2.SetValue("Name", _appname);
                            obj2.SetValue("Activation", ActivationOption.Library);
                            obj2.SetValue("ID", "{" + _appid.ToString() + "}");
                            if (!Platform.IsLessThan(Platform.Whistler))
                            {
                                try
                                {
                                    obj2.SetValue("Replicable", 0);
                                }
                                catch
                                {
                                }
                            }
                            coll.SaveChanges();
                        }
                        else
                        {
                            obj2.SetValue("Changeable", true);
                            obj2.SetValue("Deleteable", true);
                            coll.SaveChanges();
                            obj2.SetValue("Name", _appname);
                            if (!Platform.IsLessThan(Platform.Whistler))
                            {
                                try
                                {
                                    obj2.SetValue("Replicable", 0);
                                }
                                catch
                                {
                                }
                            }
                            coll.SaveChanges();
                        }
                        Guid clsid = Marshal.GenerateGuidForType(typeof(RegistrationHelperTx));
                        ICatalogCollection collection = (ICatalogCollection) coll.GetCollection("Components", obj2.Key());
                        ICatalogObject obj3 = FindComponent(collection, clsid, ref idx);
                        if (obj3 == null)
                        {
                            if (_isRunningInWow64)
                            {
                                ICatalog2 catalog3 = catalog as ICatalog2;
                                string str = "{" + clsid + "}";
                                int num2 = 1;
                                object pVarCLSIDOrProgID = str;
                                object pVarComponentType = num2;
                                catalog3.ImportComponents("{" + _appid + "}", ref pVarCLSIDOrProgID, ref pVarComponentType);
                            }
                            else
                            {
                                catalog.ImportComponent("{" + _appid + "}", "{" + clsid + "}");
                            }
                            collection = (ICatalogCollection) coll.GetCollection("Components", obj2.Key());
                            obj3 = FindComponent(collection, clsid, ref idx);
                        }
                        ConfigureComponent(collection, obj3);
                        obj2.SetValue("Changeable", false);
                        obj2.SetValue("Deleteable", false);
                        coll.SaveChanges();
                        Proxy.RegisterProxyStub();
                        RegistryPermission permission = new RegistryPermission(PermissionState.Unrestricted);
                        permission.Demand();
                        permission.Assert();
                        RegistryKey key = Registry.LocalMachine.CreateSubKey(@"SOFTWARE\MICROSOFT\OLE\NONREDIST");
                        key.SetValue("System.EnterpriseServices.Thunk.dll", "");
                        key.Close();
                    }
                }
                catch (Exception exception)
                {
                    try
                    {
                        EventLog log = new EventLog {
                            Source = "System.EnterpriseServices"
                        };
                        string message = string.Format(CultureInfo.CurrentCulture, Resource.FormatString("Reg_ErrInstSysEnt"), new object[] { exception });
                        log.WriteEntry(message, EventLogEntryType.Error);
                    }
                    catch
                    {
                    }
                }
                catch
                {
                    try
                    {
                        EventLog log2 = new EventLog {
                            Source = "System.EnterpriseServices"
                        };
                        string str3 = string.Format(CultureInfo.CurrentCulture, Resource.FormatString("Reg_ErrTxInst"), new object[] { Resource.FormatString("Err_NonClsException", "RegistrationHelperTx.InstallUtilityApplication") });
                        log2.WriteEntry(str3, EventLogEntryType.Error);
                    }
                    catch
                    {
                    }
                }
            }
        }

        public bool IsInTransaction() => 
            ContextUtil.IsInTransaction;

        public void UninstallAssembly(string assembly, string application, object sync)
        {
            this.UninstallAssembly(assembly, application, null, sync);
        }

        public void UninstallAssembly(string assembly, string application, string partition, object sync)
        {
            RegistrationConfig regConfig = new RegistrationConfig {
                AssemblyFile = assembly,
                Application = application,
                Partition = partition
            };
            this.UninstallAssemblyFromConfig(ref regConfig, sync);
        }

        public void UninstallAssemblyFromConfig([MarshalAs(UnmanagedType.IUnknown)] ref RegistrationConfig regConfig, object sync)
        {
            bool flag = false;
            try
            {
                new RegistrationDriver().UninstallAssembly(regConfig, sync);
                ContextUtil.SetComplete();
                flag = true;
            }
            finally
            {
                if (!flag)
                {
                    ContextUtil.SetAbort();
                }
            }
        }

        [ComUnregisterFunction]
        internal static void UninstallUtilityApplication(Type t)
        {
            if (!Platform.Supports(PlatformFeature.SWC))
            {
                try
                {
                    if (!Platform.IsLessThan(Platform.W2K))
                    {
                        ICatalog catalog = null;
                        ICatalogCollection coll = null;
                        ICatalogObject obj2 = null;
                        int idx = 0;
                        catalog = (ICatalog) new xCatalog();
                        if (!Platform.IsLessThan(Platform.Whistler))
                        {
                            ICatalog2 catalog2 = catalog as ICatalog2;
                            if (catalog2 != null)
                            {
                                catalog2.CurrentPartition(catalog2.GlobalPartitionID());
                            }
                        }
                        coll = (ICatalogCollection) catalog.GetCollection("Applications");
                        RegistrationDriver.Populate(coll);
                        obj2 = FindApplication(coll, _appid, ref idx);
                        if (obj2 != null)
                        {
                            obj2.SetValue("Changeable", true);
                            obj2.SetValue("Deleteable", true);
                            coll.SaveChanges();
                            int num2 = 0;
                            int num3 = 0;
                            Guid clsid = Marshal.GenerateGuidForType(typeof(RegistrationHelperTx));
                            ICatalogCollection collection = (ICatalogCollection) coll.GetCollection("Components", obj2.Key());
                            ICatalogObject obj3 = FindComponent(collection, clsid, ref num2);
                            num3 = collection.Count();
                            if (obj3 != null)
                            {
                                collection.Remove(num2);
                                collection.SaveChanges();
                            }
                            if ((obj3 != null) && (num3 == 1))
                            {
                                coll.Remove(idx);
                                coll.SaveChanges();
                            }
                            else
                            {
                                obj2.SetValue("Changeable", false);
                                obj2.SetValue("Deleteable", false);
                                coll.SaveChanges();
                            }
                        }
                    }
                }
                catch (Exception exception)
                {
                    try
                    {
                        EventLog log = new EventLog {
                            Source = "System.EnterpriseServices"
                        };
                        string message = string.Format(CultureInfo.CurrentCulture, Resource.FormatString("Reg_ErrUninstSysEnt"), new object[] { exception });
                        log.WriteEntry(message, EventLogEntryType.Error);
                    }
                    catch
                    {
                    }
                }
                catch
                {
                    try
                    {
                        EventLog log2 = new EventLog {
                            Source = "System.EnterpriseServices"
                        };
                        string str2 = string.Format(CultureInfo.CurrentCulture, Resource.FormatString("Reg_ErrTxInst"), new object[] { Resource.FormatString("Err_NonClsException", "RegistrationHelperTx.UninstallUtilityApplication") });
                        log2.WriteEntry(str2, EventLogEntryType.Error);
                    }
                    catch
                    {
                    }
                }
            }
        }
    }
}

