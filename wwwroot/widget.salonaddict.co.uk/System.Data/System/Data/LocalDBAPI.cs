namespace System.Data
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data.Common;
    using System.Data.SqlClient;
    using System.Globalization;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Security;
    using System.Text;
    using System.Threading;

    internal static class LocalDBAPI
    {
        private static PermissionSet _fullTrust = null;
        private static bool _partialTrustAllowed = false;
        private static bool _partialTrustFlagChecked = false;
        private const int const_ErrorMessageBufferSize = 0x400;
        private const uint const_LOCALDB_TRUNCATE_ERR_MESSAGE = 1;
        private const string const_localDbPrefix = @"(localdb)\";
        private const string const_partialTrustFlagKey = "ALLOW_LOCALDB_IN_PARTIAL_TRUST";
        private static object s_configLock = new object();
        private static Dictionary<string, InstanceInfo> s_configurableInstances = null;
        private static object s_dllLock = new object();
        private static LocalDBCreateInstanceDelegate s_localDBCreateInstance = null;
        private static LocalDBFormatMessageDelegate s_localDBFormatMessage = null;
        private static IntPtr s_userInstanceDLLHandle = IntPtr.Zero;

        internal static void AssertLocalDBPermissions()
        {
            _partialTrustAllowed = true;
        }

        private static SqlException CreateLocalDBException(string errorMessage) => 
            CreateLocalDBException(errorMessage, null, 0, 0);

        private static SqlException CreateLocalDBException(string errorMessage, string instance, int localDbError, int sniError)
        {
            SqlErrorCollection errorCollection = new SqlErrorCollection();
            int infoNumber = (localDbError == 0) ? sniError : localDbError;
            if (sniError != 0)
            {
                string name = string.Format(null, "SNI_ERROR_{0}", new object[] { sniError });
                errorMessage = string.Format(null, "{0} (error: {1} - {2})", new object[] { errorMessage, sniError, Res.GetString(name) });
            }
            errorCollection.Add(new SqlError(infoNumber, 0, 20, instance, errorMessage, null, 0));
            if (localDbError != 0)
            {
                errorCollection.Add(new SqlError(infoNumber, 0, 20, instance, GetLocalDBMessage(localDbError), null, 0));
            }
            SqlException exception = SqlException.CreateException(errorCollection, null);
            exception._doNotReconnect = true;
            return exception;
        }

        internal static void CreateLocalDBInstance(string instance)
        {
            DemandLocalDBPermissions();
            if (s_configurableInstances == null)
            {
                RuntimeHelpers.PrepareConstrainedRegions();
                try
                {
                    Monitor.Enter(s_configLock);
                    if (s_configurableInstances == null)
                    {
                        Dictionary<string, InstanceInfo> dictionary = new Dictionary<string, InstanceInfo>(StringComparer.OrdinalIgnoreCase);
                        object obj2 = System.Configuration.PrivilegedConfigurationManager.GetSection("system.data.localdb");
                        if (obj2 != null)
                        {
                            LocalDBConfigurationSection section = obj2 as LocalDBConfigurationSection;
                            if (section == null)
                            {
                                throw CreateLocalDBException(Res.GetString("LocalDB_BadConfigSectionType"));
                            }
                            foreach (LocalDBInstanceElement element in section.LocalDbInstances)
                            {
                                dictionary.Add(element.Name.Trim(), new InstanceInfo(element.Version.Trim()));
                            }
                        }
                        else
                        {
                            Bid.Trace("<sc.LocalDBAPI.CreateLocalDBInstance> No system.data.localdb section found in configuration");
                        }
                        s_configurableInstances = dictionary;
                    }
                }
                finally
                {
                    Monitor.Exit(s_configLock);
                }
            }
            InstanceInfo info = null;
            if (s_configurableInstances.TryGetValue(instance, out info) && !info.created)
            {
                if (info.version.Contains("\0"))
                {
                    throw CreateLocalDBException(Res.GetString("LocalDB_InvalidVersion"), instance, 0, 0);
                }
                int localDbError = LocalDBCreateInstance(info.version, instance, 0);
                Bid.Trace("<sc.LocalDBAPI.CreateLocalDBInstance> Starting creation of instance %ls version %ls", instance, info.version);
                if (localDbError < 0)
                {
                    throw CreateLocalDBException(Res.GetString("LocalDB_CreateFailed"), instance, localDbError, 0);
                }
                Bid.Trace("<sc.LocalDBAPI.CreateLocalDBInstance> Finished creation of instance %ls", instance);
                info.created = true;
            }
        }

        internal static void DemandLocalDBPermissions()
        {
            if (!_partialTrustAllowed)
            {
                if (!_partialTrustFlagChecked)
                {
                    object data = AppDomain.CurrentDomain.GetData("ALLOW_LOCALDB_IN_PARTIAL_TRUST");
                    if ((data != null) && (data is bool))
                    {
                        _partialTrustAllowed = (bool) data;
                    }
                    _partialTrustFlagChecked = true;
                    if (_partialTrustAllowed)
                    {
                        return;
                    }
                }
                if (_fullTrust == null)
                {
                    _fullTrust = new NamedPermissionSet("FullTrust");
                }
                _fullTrust.Demand();
            }
        }

        internal static string GetLocalDbInstanceNameFromServerName(string serverName)
        {
            if (serverName == null)
            {
                return null;
            }
            serverName = serverName.TrimStart(new char[0]);
            if (!serverName.StartsWith(@"(localdb)\", StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }
            string str = serverName.Substring(@"(localdb)\".Length).Trim();
            if (str.Length == 0)
            {
                return null;
            }
            return str;
        }

        internal static string GetLocalDBMessage(int hrCode)
        {
            try
            {
                StringBuilder buffer = new StringBuilder(0x400);
                uint capacity = (uint) buffer.Capacity;
                if (LocalDBFormatMessage(hrCode, 1, (uint) CultureInfo.CurrentCulture.LCID, buffer, ref capacity) >= 0)
                {
                    return buffer.ToString();
                }
                buffer = new StringBuilder(0x400);
                capacity = (uint) buffer.Capacity;
                int num = LocalDBFormatMessage(hrCode, 1, 0, buffer, ref capacity);
                if (num >= 0)
                {
                    return buffer.ToString();
                }
                return string.Format(CultureInfo.CurrentCulture, "{0} (0x{1:X}).", new object[] { Res.GetString("LocalDB_UnobtainableMessage"), num });
            }
            catch (SqlException exception)
            {
                return string.Format(CultureInfo.CurrentCulture, "{0} ({1}).", new object[] { Res.GetString("LocalDB_UnobtainableMessage"), exception.Message });
            }
        }

        internal static void ReleaseDLLHandles()
        {
            s_userInstanceDLLHandle = IntPtr.Zero;
            s_localDBFormatMessage = null;
            s_localDBCreateInstance = null;
        }

        private static LocalDBCreateInstanceDelegate LocalDBCreateInstance
        {
            get
            {
                if (s_localDBCreateInstance == null)
                {
                    RuntimeHelpers.PrepareConstrainedRegions();
                    try
                    {
                        Monitor.Enter(s_dllLock);
                        if (s_localDBCreateInstance == null)
                        {
                            IntPtr procAddress = SafeNativeMethods.GetProcAddress(UserInstanceDLLHandle, "LocalDBCreateInstance");
                            if (procAddress == IntPtr.Zero)
                            {
                                int num = Marshal.GetLastWin32Error();
                                Bid.Trace("<sc.LocalDBAPI.LocalDBCreateInstance> GetProcAddress for LocalDBCreateInstance error 0x{%X}", num);
                                throw CreateLocalDBException(Res.GetString("LocalDB_MethodNotFound"));
                            }
                            s_localDBCreateInstance = (LocalDBCreateInstanceDelegate) Marshal.GetDelegateForFunctionPointer(procAddress, typeof(LocalDBCreateInstanceDelegate));
                        }
                    }
                    finally
                    {
                        Monitor.Exit(s_dllLock);
                    }
                }
                return s_localDBCreateInstance;
            }
        }

        private static LocalDBFormatMessageDelegate LocalDBFormatMessage
        {
            get
            {
                if (s_localDBFormatMessage == null)
                {
                    RuntimeHelpers.PrepareConstrainedRegions();
                    try
                    {
                        Monitor.Enter(s_dllLock);
                        if (s_localDBFormatMessage == null)
                        {
                            IntPtr procAddress = SafeNativeMethods.GetProcAddress(UserInstanceDLLHandle, "LocalDBFormatMessage");
                            if (procAddress == IntPtr.Zero)
                            {
                                int num = Marshal.GetLastWin32Error();
                                Bid.Trace("<sc.LocalDBAPI.LocalDBFormatMessage> GetProcAddress for LocalDBFormatMessage error 0x{%X}", num);
                                throw CreateLocalDBException(Res.GetString("LocalDB_MethodNotFound"));
                            }
                            s_localDBFormatMessage = (LocalDBFormatMessageDelegate) Marshal.GetDelegateForFunctionPointer(procAddress, typeof(LocalDBFormatMessageDelegate));
                        }
                    }
                    finally
                    {
                        Monitor.Exit(s_dllLock);
                    }
                }
                return s_localDBFormatMessage;
            }
        }

        private static IntPtr UserInstanceDLLHandle
        {
            get
            {
                if (s_userInstanceDLLHandle == IntPtr.Zero)
                {
                    RuntimeHelpers.PrepareConstrainedRegions();
                    try
                    {
                        Monitor.Enter(s_dllLock);
                        if (s_userInstanceDLLHandle == IntPtr.Zero)
                        {
                            SNINativeMethodWrapper.SNIQueryInfo(SNINativeMethodWrapper.QTypes.SNI_QUERY_LOCALDB_HMODULE, ref s_userInstanceDLLHandle);
                            if (s_userInstanceDLLHandle == IntPtr.Zero)
                            {
                                SNINativeMethodWrapper.SNI_Error error = new SNINativeMethodWrapper.SNI_Error();
                                SNINativeMethodWrapper.SNIGetLastError(error);
                                throw CreateLocalDBException(Res.GetString("LocalDB_FailedGetDLLHandle"), null, 0, (int) error.sniError);
                            }
                            Bid.Trace("<sc.LocalDBAPI.UserInstanceDLLHandle> LocalDB - handle obtained");
                        }
                    }
                    finally
                    {
                        Monitor.Exit(s_dllLock);
                    }
                }
                return s_userInstanceDLLHandle;
            }
        }

        private class InstanceInfo
        {
            internal bool created;
            internal readonly string version;

            internal InstanceInfo(string version)
            {
                this.version = version;
                this.created = false;
            }
        }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl), SuppressUnmanagedCodeSecurity]
        private delegate int LocalDBCreateInstanceDelegate([MarshalAs(UnmanagedType.LPWStr)] string version, [MarshalAs(UnmanagedType.LPWStr)] string instance, uint flags);

        [SuppressUnmanagedCodeSecurity, UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet=CharSet.Unicode)]
        private delegate int LocalDBFormatMessageDelegate(int hrLocalDB, uint dwFlags, uint dwLanguageId, StringBuilder buffer, ref uint buflen);
    }
}

