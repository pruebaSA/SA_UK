namespace System.Web.Hosting
{
    using System;
    using System.Collections;
    using System.Globalization;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Runtime.Remoting;
    using System.Security;
    using System.Security.Permissions;
    using System.Security.Policy;
    using System.Text;
    using System.Threading;
    using System.Web;
    using System.Web.Compilation;
    using System.Web.Configuration;
    using System.Web.Util;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public sealed class ApplicationManager : MarshalByRefObject
    {
        private int _activeHostingEnvCount;
        private Hashtable _appDomains = new Hashtable(StringComparer.OrdinalIgnoreCase);
        private StringBuilder _appDomainsShutdowdIds = new StringBuilder();
        private static object _applicationManagerStaticLock = new object();
        private static Exception _fatalException = null;
        private WaitCallback _onRespondToPingWaitCallback;
        private int _openCount;
        private object _pendingPingCallback;
        private bool _shutdownInProgress;
        private static ApplicationManager _theAppManager;
        private static int s_domainCount = 0;
        private static object s_domainCountLock = new object();

        internal ApplicationManager()
        {
            this._onRespondToPingWaitCallback = new WaitCallback(this.OnRespondToPingWaitCallback);
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(ApplicationManager.OnUnhandledException);
        }

        [SecurityPermission(SecurityAction.Demand, Unrestricted=true)]
        public void Close()
        {
            if (Interlocked.Decrement(ref this._openCount) <= 0)
            {
                this.ShutdownAll();
            }
        }

        private static string ConstructAppDomainId(string id)
        {
            int num = 0;
            lock (s_domainCountLock)
            {
                num = ++s_domainCount;
            }
            return (id + "-" + num.ToString(NumberFormatInfo.InvariantInfo) + "-" + DateTime.UtcNow.ToFileTime().ToString());
        }

        private HostingEnvironment CreateAppDomainWithHostingEnvironment(string appId, IApplicationHost appHost, HostingEnvironmentParameters hostingParameters)
        {
            string physicalPath = appHost.GetPhysicalPath();
            if (!StringUtil.StringEndsWith(physicalPath, Path.DirectorySeparatorChar))
            {
                physicalPath = physicalPath + Path.DirectorySeparatorChar;
            }
            string domainId = ConstructAppDomainId(appId);
            string appName = StringUtil.GetStringHashCode(appId.ToLower(CultureInfo.InvariantCulture) + physicalPath.ToLower(CultureInfo.InvariantCulture)).ToString("x", CultureInfo.InvariantCulture);
            VirtualPath appVPath = VirtualPath.Create(appHost.GetVirtualPath());
            IDictionary dict = new Hashtable(20);
            AppDomainSetup setup = new AppDomainSetup();
            PopulateDomainBindings(domainId, appId, appName, physicalPath, appVPath, setup, dict);
            AppDomain domain = null;
            Exception innerException = null;
            try
            {
                domain = AppDomain.CreateDomain(domainId, GetDefaultDomainIdentity(), setup);
                foreach (DictionaryEntry entry in dict)
                {
                    domain.SetData((string) entry.Key, (string) entry.Value);
                }
            }
            catch (Exception exception2)
            {
                innerException = exception2;
            }
            if (domain == null)
            {
                throw new SystemException(System.Web.SR.GetString("Cannot_create_AppDomain"), innerException);
            }
            Type type = typeof(HostingEnvironment);
            string fullName = type.Module.Assembly.FullName;
            string typeName = type.FullName;
            ObjectHandle handle = null;
            ImpersonationContext context = null;
            IntPtr zero = IntPtr.Zero;
            int num = 10;
            int num2 = 0;
            while (num2 < num)
            {
                try
                {
                    zero = appHost.GetConfigToken();
                    break;
                }
                catch (InvalidOperationException)
                {
                    num2++;
                    Thread.Sleep(250);
                    continue;
                }
            }
            if (zero != IntPtr.Zero)
            {
                try
                {
                    context = new ImpersonationContext(zero);
                }
                catch
                {
                }
                finally
                {
                    System.Web.UnsafeNativeMethods.CloseHandle(zero);
                }
            }
            try
            {
                handle = domain.CreateInstance(fullName, typeName);
            }
            finally
            {
                if (context != null)
                {
                    context.Undo();
                }
                if (handle == null)
                {
                    AppDomain.Unload(domain);
                }
            }
            HostingEnvironment environment = (handle != null) ? (handle.Unwrap() as HostingEnvironment) : null;
            if (environment == null)
            {
                throw new SystemException(System.Web.SR.GetString("Cannot_create_HostEnv"));
            }
            bool wasLaunchedFromDevelopmentEnvironment = EnvironmentInfo.WasLaunchedFromDevelopmentEnvironment;
            IConfigMapPathFactory configMapPathFactory = appHost.GetConfigMapPathFactory();
            environment.Initialize(this, appHost, configMapPathFactory, hostingParameters);
            if (wasLaunchedFromDevelopmentEnvironment)
            {
                domain.DoCallBack(new CrossAppDomainDelegate(ApplicationManager.SetAppDomainAdditionalData));
            }
            return environment;
        }

        private HostingEnvironment CreateAppDomainWithHostingEnvironmentAndReportErrors(string appId, IApplicationHost appHost, HostingEnvironmentParameters hostingParameters)
        {
            HostingEnvironment environment;
            try
            {
                environment = this.CreateAppDomainWithHostingEnvironment(appId, appHost, hostingParameters);
            }
            catch (Exception exception)
            {
                Misc.ReportUnhandledException(exception, new string[] { System.Web.SR.GetString("Failed_to_initialize_AppDomain"), appId });
                throw;
            }
            return environment;
        }

        internal ObjectHandle CreateInstanceInNewWorkerAppDomain(Type type, string appId, VirtualPath virtualPath, string physicalPath)
        {
            IApplicationHost appHost = new SimpleApplicationHost(virtualPath, physicalPath);
            HostingEnvironmentParameters hostingParameters = new HostingEnvironmentParameters {
                HostingFlags = HostingEnvironmentFlags.HideFromAppManager
            };
            return this.CreateAppDomainWithHostingEnvironmentAndReportErrors(appId, appHost, hostingParameters).CreateInstance(type);
        }

        [SecurityPermission(SecurityAction.Demand, Unrestricted=true)]
        public IRegisteredObject CreateObject(IApplicationHost appHost, Type type)
        {
            if (appHost == null)
            {
                throw new ArgumentNullException("appHost");
            }
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }
            string appId = this.CreateSimpleAppID(VirtualPath.Create(appHost.GetVirtualPath()), appHost.GetPhysicalPath(), appHost.GetSiteName());
            return this.CreateObjectInternal(appId, type, appHost, false);
        }

        [SecurityPermission(SecurityAction.Demand, UnmanagedCode=true)]
        public IRegisteredObject CreateObject(string appId, Type type, string virtualPath, string physicalPath, bool failIfExists) => 
            this.CreateObject(appId, type, virtualPath, physicalPath, failIfExists, false);

        [SecurityPermission(SecurityAction.Demand, Unrestricted=true)]
        public IRegisteredObject CreateObject(string appId, Type type, string virtualPath, string physicalPath, bool failIfExists, bool throwOnError)
        {
            if (appId == null)
            {
                throw new ArgumentNullException("appId");
            }
            SimpleApplicationHost appHost = new SimpleApplicationHost(VirtualPath.CreateAbsolute(virtualPath), physicalPath);
            HostingEnvironmentParameters hostingParameters = null;
            if (throwOnError)
            {
                hostingParameters = new HostingEnvironmentParameters {
                    HostingFlags = HostingEnvironmentFlags.ThrowHostingInitErrors
                };
            }
            return this.CreateObjectInternal(appId, type, appHost, failIfExists, hostingParameters);
        }

        [SecurityPermission(SecurityAction.Demand, Unrestricted=true)]
        internal IRegisteredObject CreateObjectInternal(string appId, Type type, IApplicationHost appHost, bool failIfExists)
        {
            if (appId == null)
            {
                throw new ArgumentNullException("appId");
            }
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }
            if (appHost == null)
            {
                throw new ArgumentNullException("appHost");
            }
            return this.CreateObjectInternal(appId, type, appHost, failIfExists, null);
        }

        internal IRegisteredObject CreateObjectInternal(string appId, Type type, IApplicationHost appHost, bool failIfExists, HostingEnvironmentParameters hostingParameters)
        {
            if (!typeof(IRegisteredObject).IsAssignableFrom(type))
            {
                throw new ArgumentException(System.Web.SR.GetString("Not_IRegisteredObject", new object[] { type.FullName }), "type");
            }
            ObjectHandle handle = this.GetAppDomainWithHostingEnvironment(appId, appHost, hostingParameters).CreateWellKnownObjectInstance(type, failIfExists);
            if (handle == null)
            {
                return null;
            }
            return (handle.Unwrap() as IRegisteredObject);
        }

        internal IRegisteredObject CreateObjectWithDefaultAppHostAndAppId(string physicalPath, string virtualPath, Type type, out string appId) => 
            this.CreateObjectWithDefaultAppHostAndAppId(physicalPath, VirtualPath.CreateNonRelative(virtualPath), type, out appId);

        internal IRegisteredObject CreateObjectWithDefaultAppHostAndAppId(string physicalPath, VirtualPath virtualPath, Type type, out string appId)
        {
            HostingEnvironmentParameters hostingParameters = new HostingEnvironmentParameters {
                HostingFlags = HostingEnvironmentFlags.DontCallAppInitialize
            };
            return this.CreateObjectWithDefaultAppHostAndAppId(physicalPath, virtualPath, type, false, hostingParameters, out appId);
        }

        internal IRegisteredObject CreateObjectWithDefaultAppHostAndAppId(string physicalPath, VirtualPath virtualPath, Type type, bool failIfExists, HostingEnvironmentParameters hostingParameters, out string appId)
        {
            IApplicationHost host;
            if (physicalPath == null)
            {
                HttpRuntime.ForceStaticInit();
                ISAPIApplicationHost host2 = new ISAPIApplicationHost(virtualPath.VirtualPathString, null, true);
                host = host2;
                appId = host2.AppId;
                virtualPath = VirtualPath.Create(host.GetVirtualPath());
                physicalPath = FileUtil.FixUpPhysicalDirectory(host.GetPhysicalPath());
            }
            else
            {
                appId = this.CreateSimpleAppID(virtualPath, physicalPath, null);
                host = new SimpleApplicationHost(virtualPath, physicalPath);
            }
            string precompilationTargetPhysicalDirectory = hostingParameters.PrecompilationTargetPhysicalDirectory;
            if (precompilationTargetPhysicalDirectory != null)
            {
                BuildManager.VerifyUnrelatedSourceAndDest(physicalPath, precompilationTargetPhysicalDirectory);
                if ((hostingParameters.ClientBuildManagerParameter != null) && ((hostingParameters.ClientBuildManagerParameter.PrecompilationFlags & PrecompilationFlags.Updatable) == PrecompilationFlags.Default))
                {
                    appId = appId + "_precompile";
                }
                else
                {
                    appId = appId + "_precompile_u";
                }
            }
            return this.CreateObjectInternal(appId, type, host, failIfExists, hostingParameters);
        }

        private string CreateSimpleAppID(VirtualPath virtualPath, string physicalPath, string siteName)
        {
            string str = virtualPath.VirtualPathString + physicalPath;
            if (!string.IsNullOrEmpty(siteName))
            {
                str = str + siteName;
            }
            return str.GetHashCode().ToString("x", CultureInfo.InvariantCulture);
        }

        private HostingEnvironment FindAppDomainWithHostingEnvironment(string appId)
        {
            lock (this)
            {
                return (this._appDomains[appId] as HostingEnvironment);
            }
        }

        internal AppDomainInfo[] GetAppDomainInfos()
        {
            ArrayList list = new ArrayList();
            lock (this)
            {
                foreach (DictionaryEntry entry in this._appDomains)
                {
                    int num;
                    HostingEnvironment environment = (HostingEnvironment) entry.Value;
                    IApplicationHost internalApplicationHost = environment.InternalApplicationHost;
                    ApplicationInfo applicationInfo = environment.GetApplicationInfo();
                    if (internalApplicationHost != null)
                    {
                        try
                        {
                            num = int.Parse(internalApplicationHost.GetSiteID(), CultureInfo.InvariantCulture);
                        }
                        catch
                        {
                            num = 0;
                        }
                    }
                    else
                    {
                        num = 0;
                    }
                    AppDomainInfo info = new AppDomainInfo(applicationInfo.ID, applicationInfo.VirtualPath, applicationInfo.PhysicalPath, num, environment.GetIdleValue());
                    list.Add(info);
                }
            }
            return (AppDomainInfo[]) list.ToArray(typeof(AppDomainInfo));
        }

        private HostingEnvironment GetAppDomainWithHostingEnvironment(string appId, IApplicationHost appHost, HostingEnvironmentParameters hostingParameters)
        {
            HostingEnvironment environment = null;
            lock (this)
            {
                environment = this._appDomains[appId] as HostingEnvironment;
                if (environment != null)
                {
                    try
                    {
                        environment.IsUnloaded();
                    }
                    catch (AppDomainUnloadedException)
                    {
                        environment = null;
                        this._appDomainsShutdowdIds.Append("Un:" + appId + ":" + DateTime.UtcNow.ToShortTimeString() + ";");
                    }
                }
                if (environment == null)
                {
                    environment = this.CreateAppDomainWithHostingEnvironmentAndReportErrors(appId, appHost, hostingParameters);
                    this._appDomains[appId] = environment;
                }
            }
            return environment;
        }

        public static ApplicationManager GetApplicationManager()
        {
            if (_theAppManager == null)
            {
                lock (_applicationManagerStaticLock)
                {
                    if (_theAppManager == null)
                    {
                        if (HostingEnvironment.IsHosted)
                        {
                            _theAppManager = HostingEnvironment.GetApplicationManager();
                        }
                        if (_theAppManager == null)
                        {
                            _theAppManager = new ApplicationManager();
                        }
                    }
                }
            }
            return _theAppManager;
        }

        private static Evidence GetDefaultDomainIdentity()
        {
            Evidence evidence = new Evidence();
            bool flag = false;
            bool flag2 = false;
            IEnumerator hostEnumerator = AppDomain.CurrentDomain.Evidence.GetHostEnumerator();
            while (hostEnumerator.MoveNext())
            {
                if (hostEnumerator.Current is Zone)
                {
                    flag = true;
                }
                if (hostEnumerator.Current is Url)
                {
                    flag2 = true;
                }
                evidence.AddHost(hostEnumerator.Current);
            }
            hostEnumerator = AppDomain.CurrentDomain.Evidence.GetAssemblyEnumerator();
            while (hostEnumerator.MoveNext())
            {
                evidence.AddAssembly(hostEnumerator.Current);
            }
            if (!flag)
            {
                evidence.AddHost(new Zone(SecurityZone.MyComputer));
            }
            if (!flag2)
            {
                evidence.AddHost(new Url("ms-internal-microsoft-asp-net-webhost-20"));
            }
            return evidence;
        }

        [SecurityPermission(SecurityAction.Demand, Unrestricted=true)]
        public IRegisteredObject GetObject(string appId, Type type)
        {
            if (appId == null)
            {
                throw new ArgumentNullException("appId");
            }
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }
            HostingEnvironment environment = this.FindAppDomainWithHostingEnvironment(appId);
            if (environment == null)
            {
                return null;
            }
            ObjectHandle handle = environment.FindWellKnownObject(type);
            if (handle == null)
            {
                return null;
            }
            return (handle.Unwrap() as IRegisteredObject);
        }

        [SecurityPermission(SecurityAction.Demand, Unrestricted=true)]
        public ApplicationInfo[] GetRunningApplications()
        {
            ArrayList list = new ArrayList();
            lock (this)
            {
                foreach (DictionaryEntry entry in this._appDomains)
                {
                    list.Add(((HostingEnvironment) entry.Value).GetApplicationInfo());
                }
            }
            int count = list.Count;
            ApplicationInfo[] array = new ApplicationInfo[count];
            if (count > 0)
            {
                list.CopyTo(array);
            }
            return array;
        }

        internal void HostingEnvironmentActivated(string appId)
        {
            Interlocked.Increment(ref this._activeHostingEnvCount);
        }

        internal void HostingEnvironmentShutdownComplete(string appId, IApplicationHost appHost)
        {
            try
            {
                if (appHost != null)
                {
                    MarshalByRefObject obj2 = appHost as MarshalByRefObject;
                    if (obj2 != null)
                    {
                        RemotingServices.Disconnect(obj2);
                    }
                }
            }
            finally
            {
                Interlocked.Decrement(ref this._activeHostingEnvCount);
            }
        }

        internal void HostingEnvironmentShutdownInitiated(string appId, HostingEnvironment env)
        {
            if (!this._shutdownInProgress)
            {
                lock (this)
                {
                    if (!env.HasBeenRemovedFromAppManagerTable)
                    {
                        env.HasBeenRemovedFromAppManagerTable = true;
                        this._appDomainsShutdowdIds.Append("SI:" + appId + ":" + DateTime.UtcNow.ToShortTimeString() + ";");
                        this._appDomains.Remove(appId);
                    }
                }
            }
        }

        public override object InitializeLifetimeService() => 
            null;

        public bool IsIdle()
        {
            lock (this)
            {
                foreach (DictionaryEntry entry in this._appDomains)
                {
                    if (!((HostingEnvironment) entry.Value).IsIdle())
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        internal void OnRespondToPingWaitCallback(object state)
        {
            this.RespondToPingIfNeeded();
        }

        private static void OnUnhandledException(object sender, UnhandledExceptionEventArgs eventArgs)
        {
            if (eventArgs.IsTerminating)
            {
                Exception exceptionObject = eventArgs.ExceptionObject as Exception;
                if (exceptionObject != null)
                {
                    AppDomain appDomain = sender as AppDomain;
                    if (appDomain != null)
                    {
                        RecordFatalException(appDomain, exceptionObject);
                    }
                }
            }
        }

        [SecurityPermission(SecurityAction.Demand, Unrestricted=true)]
        public void Open()
        {
            Interlocked.Increment(ref this._openCount);
        }

        internal void Ping(IProcessPingCallback callback)
        {
            if (((callback != null) && (this._pendingPingCallback == null)) && (Interlocked.CompareExchange(ref this._pendingPingCallback, callback, null) == null))
            {
                ThreadPool.QueueUserWorkItem(this._onRespondToPingWaitCallback);
            }
        }

        private static void PopulateDomainBindings(string domainId, string appId, string appName, string appPath, VirtualPath appVPath, AppDomainSetup setup, IDictionary dict)
        {
            setup.PrivateBinPathProbe = "*";
            setup.ShadowCopyFiles = "true";
            setup.ApplicationBase = appPath;
            setup.ApplicationName = appName;
            setup.ConfigurationFile = "web.config";
            setup.DisallowCodeDownload = true;
            dict.Add(".appDomain", "*");
            dict.Add(".appId", appId);
            dict.Add(".appPath", appPath);
            dict.Add(".appVPath", appVPath.VirtualPathString);
            dict.Add(".domainId", domainId);
        }

        internal static void RecordFatalException(Exception e)
        {
            RecordFatalException(AppDomain.CurrentDomain, e);
        }

        internal static void RecordFatalException(AppDomain appDomain, Exception e)
        {
            if (Interlocked.CompareExchange<Exception>(ref _fatalException, e, null) == null)
            {
                Misc.WriteUnhandledExceptionToEventLog(appDomain, e);
            }
        }

        internal void ReduceAppDomainsCount(int limit)
        {
            while ((this._appDomains.Count >= limit) && !this._shutdownInProgress)
            {
                HostingEnvironment environment = null;
                lock (this)
                {
                    foreach (DictionaryEntry entry in this._appDomains)
                    {
                        HostingEnvironment environment2 = (HostingEnvironment) entry.Value;
                        if ((environment == null) || (environment2.LruScore < environment.LruScore))
                        {
                            environment = environment2;
                        }
                    }
                }
                if (environment == null)
                {
                    return;
                }
                environment.InitiateShutdownInternal();
            }
        }

        internal void RemoveFromTableIfRuntimeExists(string appId, Type runtimeType)
        {
            if (appId == null)
            {
                throw new ArgumentNullException("appId");
            }
            if (runtimeType == null)
            {
                throw new ArgumentNullException("runtimeType");
            }
            HostingEnvironment env = this.FindAppDomainWithHostingEnvironment(appId);
            if ((env != null) && (env.FindWellKnownObject(runtimeType) != null))
            {
                this.HostingEnvironmentShutdownInitiated(appId, env);
            }
        }

        internal void RespondToPingIfNeeded()
        {
            IProcessPingCallback comparand = this._pendingPingCallback as IProcessPingCallback;
            if ((comparand != null) && (Interlocked.CompareExchange(ref this._pendingPingCallback, null, comparand) == comparand))
            {
                comparand.Respond();
            }
        }

        private static void SetAppDomainAdditionalData()
        {
            try
            {
                CachedPathData machinePathData = CachedPathData.GetMachinePathData();
                if ((machinePathData != null) && (machinePathData.ConfigRecord != null))
                {
                    DeploymentSection section = machinePathData.ConfigRecord.GetSection("system.web/deployment") as DeploymentSection;
                    if ((section != null) && !section.Retail)
                    {
                        AppDomain.CurrentDomain.SetData(".devEnvironment", true);
                        AppDomain.CurrentDomain.SetData("ALLOW_LOCALDB_IN_PARTIAL_TRUST", true);
                    }
                }
            }
            catch
            {
            }
        }

        [SecurityPermission(SecurityAction.Demand, Unrestricted=true)]
        public void ShutdownAll()
        {
            this._shutdownInProgress = true;
            lock (this)
            {
                foreach (DictionaryEntry entry in this._appDomains)
                {
                    this._appDomainsShutdowdIds.Append(string.Concat(new object[] { "SA:", entry.Key, ":", DateTime.UtcNow.ToShortTimeString(), ";" }));
                    ((HostingEnvironment) entry.Value).InitiateShutdownInternal();
                }
                this._appDomains = new Hashtable();
            }
            for (int i = 0; (this._activeHostingEnvCount > 0) && (i < 0xbb8); i++)
            {
                Thread.Sleep(100);
            }
        }

        [SecurityPermission(SecurityAction.Demand, Unrestricted=true)]
        public void ShutdownApplication(string appId)
        {
            if (appId == null)
            {
                throw new ArgumentNullException("appId");
            }
            HostingEnvironment environment = this.FindAppDomainWithHostingEnvironment(appId);
            if (environment != null)
            {
                environment.InitiateShutdownInternal();
            }
        }

        [SecurityPermission(SecurityAction.Demand, Unrestricted=true)]
        public void StopObject(string appId, Type type)
        {
            if (appId == null)
            {
                throw new ArgumentNullException("appId");
            }
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }
            HostingEnvironment environment = this.FindAppDomainWithHostingEnvironment(appId);
            if (environment != null)
            {
                environment.StopWellKnownObject(type);
            }
        }

        internal int AppDomainsCount
        {
            get
            {
                lock (this)
                {
                    return this._appDomains.Count;
                }
            }
        }

        private static class EnvironmentInfo
        {
            public static readonly bool WasLaunchedFromDevelopmentEnvironment = GetWasLaunchedFromDevelopmentEnvironmentValue();

            private static bool GetWasLaunchedFromDevelopmentEnvironmentValue()
            {
                try
                {
                    return string.Equals(Environment.GetEnvironmentVariable("DEV_ENVIRONMENT", EnvironmentVariableTarget.Process), "1", StringComparison.Ordinal);
                }
                catch
                {
                    return false;
                }
            }
        }
    }
}

