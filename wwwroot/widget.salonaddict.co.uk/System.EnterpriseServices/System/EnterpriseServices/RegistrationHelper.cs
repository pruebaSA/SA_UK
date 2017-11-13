namespace System.EnterpriseServices
{
    using System;
    using System.Diagnostics;
    using System.EnterpriseServices.Thunk;
    using System.Globalization;
    using System.Runtime.InteropServices;
    using System.Security.Permissions;
    using System.Threading;
    using System.Transactions;

    [Guid("89a86e7b-c229-4008-9baa-2f5c8411d7e0")]
    public sealed class RegistrationHelper : MarshalByRefObject, IRegistrationHelper, IThunkInstallation
    {
        public void InstallAssembly(string assembly, ref string application, ref string tlb, InstallationFlags installFlags)
        {
            this.InstallAssembly(assembly, ref application, null, ref tlb, installFlags);
        }

        public void InstallAssembly(string assembly, ref string application, string partition, ref string tlb, InstallationFlags installFlags)
        {
            RegistrationConfig regConfig = new RegistrationConfig {
                AssemblyFile = assembly,
                Application = application,
                Partition = partition,
                TypeLibrary = tlb,
                InstallationFlags = installFlags
            };
            this.InstallAssemblyFromConfig(ref regConfig);
            application = regConfig.Application;
            tlb = regConfig.TypeLibrary;
        }

        public void InstallAssemblyFromConfig([MarshalAs(UnmanagedType.IUnknown)] ref RegistrationConfig regConfig)
        {
            SecurityPermission permission = new SecurityPermission(SecurityPermissionFlag.UnmanagedCode);
            permission.Demand();
            permission.Assert();
            Platform.Assert(Platform.W2K, "RegistrationHelper.InstallAssemblyFromConfig");
            if (Thread.CurrentThread.GetApartmentState() == ApartmentState.STA)
            {
                RegistrationThreadWrapper wrapper = new RegistrationThreadWrapper(this, regConfig);
                Thread thread = new Thread(new ThreadStart(wrapper.InstallThread));
                thread.Start();
                thread.Join();
                wrapper.PropInstallResult();
            }
            else if (!Platform.Supports(PlatformFeature.SWC))
            {
                if (Platform.IsLessThan(Platform.W2K) || !this.TryTransactedInstall(regConfig))
                {
                    new RegistrationDriver().InstallAssembly(regConfig, null);
                }
            }
            else
            {
                TransactionOptions transactionOptions = new TransactionOptions {
                    Timeout = TimeSpan.FromSeconds(0.0),
                    IsolationLevel = IsolationLevel.Serializable
                };
                CatalogSync obSync = new CatalogSync();
                using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, transactionOptions, EnterpriseServicesInteropOption.Full))
                {
                    new RegistrationDriver().InstallAssembly(regConfig, obSync);
                    scope.Complete();
                }
                obSync.Wait();
            }
        }

        void IThunkInstallation.DefaultInstall(string asm)
        {
            string application = null;
            string tlb = null;
            this.InstallAssembly(asm, ref application, ref tlb, InstallationFlags.ReconfigureExistingApplication | InstallationFlags.FindOrCreateTargetApplication);
        }

        private bool TryTransactedInstall(RegistrationConfig regConfig)
        {
            RegistrationHelperTx tx = null;
            try
            {
                tx = new RegistrationHelperTx();
                if (!tx.IsInTransaction())
                {
                    tx = null;
                }
            }
            catch (Exception exception)
            {
                try
                {
                    EventLog log = new EventLog {
                        Source = "System.EnterpriseServices"
                    };
                    string message = string.Format(CultureInfo.CurrentCulture, Resource.FormatString("Reg_ErrTxInst"), new object[] { exception });
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
                    string str2 = string.Format(CultureInfo.CurrentCulture, Resource.FormatString("Reg_ErrTxInst"), new object[] { Resource.FormatString("Err_NonClsException", "RegistrationHelper.TryTransactedInstall") });
                    log2.WriteEntry(str2, EventLogEntryType.Error);
                }
                catch
                {
                }
            }
            if (tx == null)
            {
                return false;
            }
            CatalogSync sync = new CatalogSync();
            tx.InstallAssemblyFromConfig(ref regConfig, sync);
            sync.Wait();
            return true;
        }

        private bool TryTransactedUninstall(RegistrationConfig regConfig)
        {
            RegistrationHelperTx tx = null;
            try
            {
                tx = new RegistrationHelperTx();
                if (!tx.IsInTransaction())
                {
                    tx = null;
                }
            }
            catch (Exception exception)
            {
                try
                {
                    EventLog log = new EventLog {
                        Source = "System.EnterpriseServices"
                    };
                    string message = string.Format(CultureInfo.CurrentCulture, Resource.FormatString("Reg_ErrTxUninst"), new object[] { exception });
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
                    string str2 = string.Format(CultureInfo.CurrentCulture, Resource.FormatString("Reg_ErrTxInst"), new object[] { Resource.FormatString("Err_NonClsException", "RegistrationHelper.TryTransactedUninstall") });
                    log2.WriteEntry(str2, EventLogEntryType.Error);
                }
                catch
                {
                }
            }
            if (tx == null)
            {
                return false;
            }
            CatalogSync sync = new CatalogSync();
            tx.UninstallAssemblyFromConfig(ref regConfig, sync);
            sync.Wait();
            return true;
        }

        public void UninstallAssembly(string assembly, string application)
        {
            this.UninstallAssembly(assembly, application, null);
        }

        public void UninstallAssembly(string assembly, string application, string partition)
        {
            RegistrationConfig regConfig = new RegistrationConfig {
                AssemblyFile = assembly,
                Application = application,
                Partition = partition
            };
            this.UninstallAssemblyFromConfig(ref regConfig);
        }

        public void UninstallAssemblyFromConfig([MarshalAs(UnmanagedType.IUnknown)] ref RegistrationConfig regConfig)
        {
            SecurityPermission permission = new SecurityPermission(SecurityPermissionFlag.UnmanagedCode);
            permission.Demand();
            permission.Assert();
            Platform.Assert(Platform.W2K, "RegistrationHelper.UninstallAssemblyFromConfig");
            if (Thread.CurrentThread.GetApartmentState() == ApartmentState.STA)
            {
                RegistrationThreadWrapper wrapper = new RegistrationThreadWrapper(this, regConfig);
                Thread thread = new Thread(new ThreadStart(wrapper.UninstallThread));
                thread.Start();
                thread.Join();
                wrapper.PropUninstallResult();
            }
            else if (!Platform.Supports(PlatformFeature.SWC))
            {
                if (Platform.IsLessThan(Platform.W2K) || !this.TryTransactedUninstall(regConfig))
                {
                    new RegistrationDriver().UninstallAssembly(regConfig, null);
                }
            }
            else
            {
                TransactionOptions transactionOptions = new TransactionOptions {
                    Timeout = TimeSpan.FromMinutes(0.0),
                    IsolationLevel = IsolationLevel.Serializable
                };
                CatalogSync obSync = new CatalogSync();
                using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, transactionOptions, EnterpriseServicesInteropOption.Full))
                {
                    new RegistrationDriver().UninstallAssembly(regConfig, obSync);
                    scope.Complete();
                }
                obSync.Wait();
            }
        }
    }
}

