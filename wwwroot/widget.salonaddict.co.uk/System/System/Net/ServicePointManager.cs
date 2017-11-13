namespace System.Net
{
    using System;
    using System.Collections;
    using System.Diagnostics;
    using System.Globalization;
    using System.Net.Configuration;
    using System.Net.Security;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Security.Authentication;
    using System.Threading;

    public class ServicePointManager
    {
        private static volatile bool configurationLoaded = false;
        private static object configurationLoadedLock = new object();
        private const int DefaultAspPersistentConnectionLimit = 10;
        public const int DefaultNonPersistentConnectionLimit = 4;
        public const int DefaultPersistentConnectionLimit = 2;
        private const string RegistryGlobalRequireCertificateEKUs = "RequireCertificateEKUs";
        private const string RegistryGlobalSendAuxRecordName = "SchSendAuxRecord";
        private const string RegistryGlobalStrongCryptoName = "SchUseStrongCrypto";
        private const string RegistryGlobalSystemDefaultTlsVersionsName = "SystemDefaultTlsVersions";
        private const string RegistryLocalRequireCertificateEKUs = "System.Net.ServicePointManager.RequireCertificateEKUs";
        private const string RegistryLocalSecureProtocolName = "System.Net.ServicePointManager.SecurityProtocol";
        private const string RegistryLocalSendAuxRecordName = "System.Net.ServicePointManager.SchSendAuxRecord";
        private const string RegistryLocalSystemDefaultTlsVersionsName = "System.Net.ServicePointManager.SystemDefaultTlsVersions";
        private static System.Net.CertPolicyValidationCallback s_CertPolicyValidationCallback = new System.Net.CertPolicyValidationCallback();
        private static Hashtable s_ConfigTable = null;
        private static int s_ConnectionLimit = PersistentConnectionLimit;
        private static SslProtocols s_defaultSslProtocols;
        private static bool s_disableSendAuxRecord;
        private static bool s_disableStrongCrypto;
        private static bool s_disableSystemDefaultTlsVersions;
        private static bool s_dontCheckCertificateEKUs;
        internal static readonly TimerThread.Callback s_IdleServicePointTimeoutDelegate = new TimerThread.Callback(ServicePointManager.IdleServicePointTimeoutCallback);
        private static int s_MaxServicePoints = 0;
        private static SecurityProtocolType s_SecurityProtocolType;
        private static System.Net.ServerCertValidationCallback s_ServerCertValidationCallback = null;
        private static TimerThread.Queue s_ServicePointIdlingQueue = TimerThread.GetOrCreateQueue(0x186a0);
        private static Hashtable s_ServicePointTable = new Hashtable(10);
        internal static int s_TcpKeepAliveInterval;
        internal static int s_TcpKeepAliveTime;
        private static bool s_UserChangedLimit;
        internal static bool s_UseTcpKeepAlive = false;
        internal static readonly string SpecialConnectGroupName = "/.NET/NetClasses/HttpWebRequest/CONNECT__Group$$/";

        private ServicePointManager()
        {
        }

        [Conditional("DEBUG")]
        internal static void Debug(int requestHash)
        {
            try
            {
                foreach (WeakReference reference in s_ServicePointTable)
                {
                    ServicePoint target;
                    if ((reference != null) && reference.IsAlive)
                    {
                        target = (ServicePoint) reference.Target;
                    }
                    else
                    {
                        target = null;
                    }
                }
            }
            catch (Exception exception)
            {
                if (((exception is ThreadAbortException) || (exception is StackOverflowException)) || (exception is OutOfMemoryException))
                {
                    throw;
                }
            }
            catch
            {
            }
        }

        private static void EnsureConfigurationLoaded()
        {
            if (!configurationLoaded)
            {
                lock (configurationLoadedLock)
                {
                    if (!configurationLoaded)
                    {
                        s_disableStrongCrypto = TryInitialize<bool>(new ConfigurationLoaderDelegate<bool>(ServicePointManager.LoadDisableStrongCryptoConfiguration), true);
                        s_disableSendAuxRecord = TryInitialize<bool>(new ConfigurationLoaderDelegate<bool>(ServicePointManager.LoadDisableSendAuxRecordConfiguration), false);
                        s_disableSystemDefaultTlsVersions = TryInitialize<bool>(new ConfigurationLoaderDelegate<bool>(ServicePointManager.LoadDisableSystemDefaultTlsVersionsConfiguration), true);
                        s_dontCheckCertificateEKUs = TryInitialize<bool>(new ConfigurationLoaderDelegate<bool>(ServicePointManager.LoadDisableCertificateEKUsConfiguration), false);
                        s_defaultSslProtocols = TryInitialize<SslProtocols>(new ConfigurationLoaderDelegate<SslProtocols>(ServicePointManager.LoadSecureProtocolConfiguration), SslProtocols.Default);
                        s_SecurityProtocolType = (SecurityProtocolType) s_defaultSslProtocols;
                        configurationLoaded = true;
                    }
                }
            }
        }

        internal static ServicePoint FindServicePoint(ProxyChain chain)
        {
            if (!chain.Enumerator.MoveNext())
            {
                return null;
            }
            Uri current = chain.Enumerator.Current;
            return FindServicePointHelper((current == null) ? chain.Destination : current, current != null);
        }

        public static ServicePoint FindServicePoint(Uri address) => 
            FindServicePoint(address, null);

        internal static ServicePoint FindServicePoint(string host, int port)
        {
            if (host == null)
            {
                throw new ArgumentNullException("address");
            }
            string lookupString = null;
            bool proxyServicePoint = false;
            lookupString = "ByHost:" + host + ":" + port.ToString(CultureInfo.InvariantCulture);
            ServicePoint target = null;
            lock (s_ServicePointTable)
            {
                WeakReference reference = s_ServicePointTable[lookupString] as WeakReference;
                if (reference != null)
                {
                    target = (ServicePoint) reference.Target;
                }
                if (target != null)
                {
                    return target;
                }
                if ((s_MaxServicePoints <= 0) || (s_ServicePointTable.Count < s_MaxServicePoints))
                {
                    int internalConnectionLimit = InternalConnectionLimit;
                    bool userChangedLimit = s_UserChangedLimit;
                    string key = host + ":" + port.ToString(CultureInfo.InvariantCulture);
                    if (ConfigTable.ContainsKey(key))
                    {
                        internalConnectionLimit = (int) ConfigTable[key];
                        userChangedLimit = true;
                    }
                    target = new ServicePoint(host, port, s_ServicePointIdlingQueue, internalConnectionLimit, lookupString, userChangedLimit, proxyServicePoint);
                    reference = new WeakReference(target);
                    s_ServicePointTable[lookupString] = reference;
                    return target;
                }
                Exception exception = new InvalidOperationException(SR.GetString("net_maxsrvpoints"));
                throw exception;
            }
            return target;
        }

        public static ServicePoint FindServicePoint(string uriString, IWebProxy proxy)
        {
            Uri address = new Uri(uriString);
            return FindServicePoint(address, proxy);
        }

        public static ServicePoint FindServicePoint(Uri address, IWebProxy proxy)
        {
            ProxyChain chain;
            HttpAbortDelegate abortDelegate = null;
            int abortState = 0;
            return FindServicePoint(address, proxy, out chain, ref abortDelegate, ref abortState);
        }

        internal static ServicePoint FindServicePoint(Uri address, IWebProxy proxy, out ProxyChain chain, ref HttpAbortDelegate abortDelegate, ref int abortState)
        {
            if (address == null)
            {
                throw new ArgumentNullException("address");
            }
            bool isProxyServicePoint = false;
            chain = null;
            Uri current = null;
            if ((proxy != null) && !address.IsLoopback)
            {
                IAutoWebProxy proxy2 = proxy as IAutoWebProxy;
                if (proxy2 != null)
                {
                    chain = proxy2.GetProxies(address);
                    abortDelegate = chain.HttpAbortDelegate;
                    try
                    {
                        Thread.MemoryBarrier();
                        if (abortState != 0)
                        {
                            Exception exception = new WebException(NetRes.GetWebStatusString(WebExceptionStatus.RequestCanceled), WebExceptionStatus.RequestCanceled);
                            throw exception;
                        }
                        chain.Enumerator.MoveNext();
                        current = chain.Enumerator.Current;
                    }
                    finally
                    {
                        abortDelegate = null;
                    }
                }
                else if (!proxy.IsBypassed(address))
                {
                    current = proxy.GetProxy(address);
                }
                if (current != null)
                {
                    address = current;
                    isProxyServicePoint = true;
                }
            }
            return FindServicePointHelper(address, isProxyServicePoint);
        }

        private static ServicePoint FindServicePointHelper(Uri address, bool isProxyServicePoint)
        {
            if (isProxyServicePoint && (address.Scheme != Uri.UriSchemeHttp))
            {
                Exception exception = new NotSupportedException(SR.GetString("net_proxyschemenotsupported", new object[] { address.Scheme }));
                throw exception;
            }
            string lookupString = MakeQueryString(address, isProxyServicePoint);
            ServicePoint target = null;
            lock (s_ServicePointTable)
            {
                WeakReference reference = s_ServicePointTable[lookupString] as WeakReference;
                if (reference != null)
                {
                    target = (ServicePoint) reference.Target;
                }
                if (target != null)
                {
                    return target;
                }
                if ((s_MaxServicePoints <= 0) || (s_ServicePointTable.Count < s_MaxServicePoints))
                {
                    int internalConnectionLimit = InternalConnectionLimit;
                    string key = MakeQueryString(address);
                    bool userChangedLimit = s_UserChangedLimit;
                    if (ConfigTable.ContainsKey(key))
                    {
                        internalConnectionLimit = (int) ConfigTable[key];
                        userChangedLimit = true;
                    }
                    target = new ServicePoint(address, s_ServicePointIdlingQueue, internalConnectionLimit, lookupString, userChangedLimit, isProxyServicePoint);
                    reference = new WeakReference(target);
                    s_ServicePointTable[lookupString] = reference;
                    return target;
                }
                Exception exception2 = new InvalidOperationException(SR.GetString("net_maxsrvpoints"));
                throw exception2;
            }
            return target;
        }

        internal static ICertificatePolicy GetLegacyCertificatePolicy() => 
            s_CertPolicyValidationCallback?.CertificatePolicy;

        private static void IdleServicePointTimeoutCallback(TimerThread.Timer timer, int timeNoticed, object context)
        {
            ServicePoint point = (ServicePoint) context;
            lock (s_ServicePointTable)
            {
                s_ServicePointTable.Remove(point.LookupString);
            }
            point.ReleaseAllConnectionGroups();
        }

        private static bool LoadDisableCertificateEKUsConfiguration(bool disable) => 
            ((RegistryConfiguration.AppConfigReadInt("System.Net.ServicePointManager.RequireCertificateEKUs", 1) == 0) || ((RegistryConfiguration.GlobalConfigReadInt("RequireCertificateEKUs", 1) == 0) || disable));

        private static bool LoadDisableSendAuxRecordConfiguration(bool disable) => 
            ((RegistryConfiguration.AppConfigReadInt("System.Net.ServicePointManager.SchSendAuxRecord", 1) == 0) || ((RegistryConfiguration.GlobalConfigReadInt("SchSendAuxRecord", 1) == 0) || disable));

        private static bool LoadDisableStrongCryptoConfiguration(bool disable)
        {
            disable = RegistryConfiguration.GlobalConfigReadInt("SchUseStrongCrypto", 0) != 1;
            return disable;
        }

        private static bool LoadDisableSystemDefaultTlsVersionsConfiguration(bool disable)
        {
            disable = RegistryConfiguration.GlobalConfigReadInt("SystemDefaultTlsVersions", 0) != 1;
            if (!disable)
            {
                disable = RegistryConfiguration.AppConfigReadInt("System.Net.ServicePointManager.SystemDefaultTlsVersions", 1) != 1;
            }
            return disable;
        }

        private static SslProtocols LoadSecureProtocolConfiguration(SslProtocols defaultValue)
        {
            if (!s_disableSystemDefaultTlsVersions)
            {
                defaultValue = SslProtocols.None;
            }
            if (!s_disableStrongCrypto || !s_disableSystemDefaultTlsVersions)
            {
                string str = RegistryConfiguration.AppConfigReadString("System.Net.ServicePointManager.SecurityProtocol", null);
                try
                {
                    SecurityProtocolType type = (SecurityProtocolType) System.Enum.Parse(typeof(SecurityProtocolType), str);
                    ValidateSecurityProtocol(type);
                    defaultValue = (SslProtocols) type;
                }
                catch (ArgumentNullException)
                {
                }
                catch (ArgumentException)
                {
                }
                catch (NotSupportedException)
                {
                }
                catch (OverflowException)
                {
                }
            }
            return defaultValue;
        }

        internal static string MakeQueryString(Uri address)
        {
            if (address.IsDefaultPort)
            {
                return (address.Scheme + "://" + address.DnsSafeHost);
            }
            return (address.Scheme + "://" + address.DnsSafeHost + ":" + address.Port.ToString());
        }

        internal static string MakeQueryString(Uri address1, bool isProxy)
        {
            if (isProxy)
            {
                return (MakeQueryString(address1) + "://proxy");
            }
            return MakeQueryString(address1);
        }

        public static void SetTcpKeepAlive(bool enabled, int keepAliveTime, int keepAliveInterval)
        {
            if (enabled)
            {
                s_UseTcpKeepAlive = true;
                if (keepAliveTime <= 0)
                {
                    throw new ArgumentOutOfRangeException("keepAliveTime");
                }
                if (keepAliveInterval <= 0)
                {
                    throw new ArgumentOutOfRangeException("keepAliveInterval");
                }
                s_TcpKeepAliveTime = keepAliveTime;
                s_TcpKeepAliveInterval = keepAliveInterval;
            }
            else
            {
                s_UseTcpKeepAlive = false;
                s_TcpKeepAliveTime = 0;
                s_TcpKeepAliveInterval = 0;
            }
        }

        private static T TryInitialize<T>(ConfigurationLoaderDelegate<T> loadConfiguration, T fallbackDefault)
        {
            try
            {
                return loadConfiguration(fallbackDefault);
            }
            catch (Exception exception)
            {
                if (NclUtilities.IsFatal(exception))
                {
                    throw;
                }
                return fallbackDefault;
            }
        }

        private static void ValidateSecurityProtocol(SecurityProtocolType value)
        {
            SecurityProtocolType type = 0xff0;
            if ((value & ~type) != 0)
            {
                throw new NotSupportedException(SR.GetString("net_securityprotocolnotsupported"));
            }
        }

        [Obsolete("CertificatePolicy is obsoleted for this type, please use ServerCertificateValidationCallback instead. http://go.microsoft.com/fwlink/?linkid=14202")]
        public static ICertificatePolicy CertificatePolicy
        {
            get => 
                GetLegacyCertificatePolicy();
            set
            {
                ExceptionHelper.UnmanagedPermission.Demand();
                s_CertPolicyValidationCallback = new System.Net.CertPolicyValidationCallback(value);
            }
        }

        internal static System.Net.CertPolicyValidationCallback CertPolicyValidationCallback =>
            s_CertPolicyValidationCallback;

        internal static bool CheckCertificateName =>
            SettingsSectionInternal.Section.CheckCertificateName;

        public static bool CheckCertificateRevocationList
        {
            get => 
                SettingsSectionInternal.Section.CheckCertificateRevocationList;
            set
            {
                ExceptionHelper.UnmanagedPermission.Demand();
                SettingsSectionInternal.Section.CheckCertificateRevocationList = value;
            }
        }

        private static Hashtable ConfigTable
        {
            get
            {
                if (s_ConfigTable == null)
                {
                    lock (s_ServicePointTable)
                    {
                        if (s_ConfigTable == null)
                        {
                            Hashtable connectionManagement = ConnectionManagementSectionInternal.GetSection().ConnectionManagement;
                            if (connectionManagement == null)
                            {
                                connectionManagement = new Hashtable();
                            }
                            if (connectionManagement.ContainsKey("*"))
                            {
                                int persistentConnectionLimit = (int) connectionManagement["*"];
                                if (persistentConnectionLimit < 1)
                                {
                                    persistentConnectionLimit = PersistentConnectionLimit;
                                }
                                s_ConnectionLimit = persistentConnectionLimit;
                            }
                            s_ConfigTable = connectionManagement;
                        }
                    }
                }
                return s_ConfigTable;
            }
        }

        public static int DefaultConnectionLimit
        {
            get => 
                InternalConnectionLimit;
            set
            {
                ExceptionHelper.WebPermissionUnrestricted.Demand();
                if (value <= 0)
                {
                    throw new ArgumentOutOfRangeException(SR.GetString("net_toosmall"));
                }
                InternalConnectionLimit = value;
            }
        }

        internal static SslProtocols DefaultSslProtocols
        {
            get
            {
                EnsureConfigurationLoaded();
                return s_defaultSslProtocols;
            }
        }

        internal static bool DisableCertificateEKUs
        {
            get
            {
                EnsureConfigurationLoaded();
                return s_dontCheckCertificateEKUs;
            }
        }

        internal static bool DisableSendAuxRecord
        {
            get
            {
                EnsureConfigurationLoaded();
                return s_disableSendAuxRecord;
            }
        }

        internal static bool DisableStrongCrypto
        {
            get
            {
                EnsureConfigurationLoaded();
                return s_disableStrongCrypto;
            }
        }

        internal static bool DisableSystemDefaultTlsVersions
        {
            get
            {
                EnsureConfigurationLoaded();
                return s_disableSystemDefaultTlsVersions;
            }
        }

        public static int DnsRefreshTimeout
        {
            get => 
                SettingsSectionInternal.Section.DnsRefreshTimeout;
            set
            {
                if (value < -1)
                {
                    SettingsSectionInternal.Section.DnsRefreshTimeout = -1;
                }
                else
                {
                    SettingsSectionInternal.Section.DnsRefreshTimeout = value;
                }
            }
        }

        public static bool EnableDnsRoundRobin
        {
            get => 
                SettingsSectionInternal.Section.EnableDnsRoundRobin;
            set
            {
                SettingsSectionInternal.Section.EnableDnsRoundRobin = value;
            }
        }

        public static bool Expect100Continue
        {
            get => 
                SettingsSectionInternal.Section.Expect100Continue;
            set
            {
                SettingsSectionInternal.Section.Expect100Continue = value;
            }
        }

        internal static TimerThread.Callback IdleServicePointTimeoutDelegate =>
            s_IdleServicePointTimeoutDelegate;

        private static int InternalConnectionLimit
        {
            get
            {
                if (s_ConfigTable == null)
                {
                    s_ConfigTable = ConfigTable;
                }
                return s_ConnectionLimit;
            }
            set
            {
                if (s_ConfigTable == null)
                {
                    s_ConfigTable = ConfigTable;
                }
                s_UserChangedLimit = true;
                s_ConnectionLimit = value;
            }
        }

        public static int MaxServicePointIdleTime
        {
            get => 
                s_ServicePointIdlingQueue.Duration;
            set
            {
                ExceptionHelper.WebPermissionUnrestricted.Demand();
                if (!ValidationHelper.ValidateRange(value, -1, 0x7fffffff))
                {
                    throw new ArgumentOutOfRangeException("value");
                }
                if (s_ServicePointIdlingQueue.Duration != value)
                {
                    s_ServicePointIdlingQueue = TimerThread.GetOrCreateQueue(value);
                }
            }
        }

        public static int MaxServicePoints
        {
            get => 
                s_MaxServicePoints;
            set
            {
                ExceptionHelper.WebPermissionUnrestricted.Demand();
                if (!ValidationHelper.ValidateRange(value, 0, 0x7fffffff))
                {
                    throw new ArgumentOutOfRangeException("value");
                }
                s_MaxServicePoints = value;
            }
        }

        private static int PersistentConnectionLimit
        {
            get
            {
                if (ComNetOS.IsAspNetServer)
                {
                    return 10;
                }
                return 2;
            }
        }

        public static SecurityProtocolType SecurityProtocol
        {
            get
            {
                EnsureConfigurationLoaded();
                return s_SecurityProtocolType;
            }
            set
            {
                EnsureConfigurationLoaded();
                ValidateSecurityProtocol(value);
                s_SecurityProtocolType = value;
            }
        }

        public static RemoteCertificateValidationCallback ServerCertificateValidationCallback
        {
            get => 
                s_ServerCertValidationCallback?.ValidationCallback;
            set
            {
                ExceptionHelper.InfrastructurePermission.Demand();
                s_ServerCertValidationCallback = new System.Net.ServerCertValidationCallback(value);
            }
        }

        internal static System.Net.ServerCertValidationCallback ServerCertValidationCallback =>
            s_ServerCertValidationCallback;

        public static bool UseNagleAlgorithm
        {
            get => 
                SettingsSectionInternal.Section.UseNagleAlgorithm;
            set
            {
                SettingsSectionInternal.Section.UseNagleAlgorithm = value;
            }
        }

        private delegate T ConfigurationLoaderDelegate<T>(T initialValue);
    }
}

