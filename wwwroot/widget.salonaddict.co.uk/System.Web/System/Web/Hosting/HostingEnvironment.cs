namespace System.Web.Hosting
{
    using System;
    using System.Collections;
    using System.Globalization;
    using System.Runtime.Remoting;
    using System.Runtime.Remoting.Messaging;
    using System.Security;
    using System.Security.Permissions;
    using System.Threading;
    using System.Web;
    using System.Web.Caching;
    using System.Web.Compilation;
    using System.Web.Configuration;
    using System.Web.Util;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public sealed class HostingEnvironment : MarshalByRefObject
    {
        private string _appConfigPath;
        private bool _appDomainShutdownStarted;
        private IApplicationHost _appHost;
        private string _appId;
        private IdentitySection _appIdentity;
        private IntPtr _appIdentityToken;
        private bool _appIdentityTokenSet;
        private ApplicationManager _appManager;
        private string _appPhysicalPath;
        private VirtualPath _appVirtualPath;
        private int _busyCount;
        private IConfigMapPath _configMapPath;
        private IConfigMapPath2 _configMapPath2;
        private IntPtr _configToken;
        private bool _externalAppHost;
        private static IProcessHostSupportFunctions _functions;
        private static bool _hasBeenRemovedFromAppManangerTable;
        private HostingEnvironmentParameters _hostingParameters;
        private IdleTimeoutMonitor _idleTimeoutMonitor;
        private WaitCallback _initiateShutdownWorkItemCallback;
        private bool _isBusy;
        private System.Web.Hosting.VirtualPathProvider _mapPathBasedVirtualPathProvider;
        private EventHandler _onAppDomainUnload;
        private Hashtable _registeredObjects = new Hashtable();
        private bool _removedFromAppManager;
        private bool _shutdownInitated;
        private bool _shutdownInProgress;
        private string _shutDownStack;
        private string _siteID;
        private string _siteName;
        private static HostingEnvironment _theHostingEnvironment;
        private System.Web.Hosting.VirtualPathProvider _virtualPathProvider;
        private Hashtable _wellKnownObjects = new Hashtable();
        private static int s_appDomainUniqueInteger;

        [SecurityPermission(SecurityAction.Demand, Unrestricted=true)]
        public HostingEnvironment()
        {
            if (_theHostingEnvironment != null)
            {
                throw new InvalidOperationException(System.Web.SR.GetString("Only_1_HostEnv"));
            }
            _theHostingEnvironment = this;
            this._onAppDomainUnload = new EventHandler(this.OnAppDomainUnload);
            Thread.GetDomain().DomainUnload += this._onAppDomainUnload;
        }

        internal static object AddVirtualPathToFileMapping(VirtualPath virtualPath, string physicalPath)
        {
            CallContext.SetData(GetFixedMappingSlotName(virtualPath), physicalPath);
            VirtualPathToFileMappingState state = new VirtualPathToFileMappingState {
                VirtualPath = virtualPath,
                VirtualPathProvider = _theHostingEnvironment._virtualPathProvider
            };
            _theHostingEnvironment._virtualPathProvider = _theHostingEnvironment._mapPathBasedVirtualPathProvider;
            return state;
        }

        internal static void ClearVirtualPathToFileMapping(object state)
        {
            VirtualPathToFileMappingState state2 = (VirtualPathToFileMappingState) state;
            CallContext.SetData(GetFixedMappingSlotName(state2.VirtualPath), null);
            _theHostingEnvironment._virtualPathProvider = state2.VirtualPathProvider;
        }

        internal ObjectHandle CreateInstance(Type type) => 
            new ObjectHandle(Activator.CreateInstance(type));

        internal ObjectHandle CreateWellKnownObjectInstance(Type type, bool failIfExists)
        {
            IRegisteredObject o = null;
            string fullName = type.FullName;
            bool flag = false;
            lock (this)
            {
                o = this._wellKnownObjects[fullName] as IRegisteredObject;
                if (o == null)
                {
                    o = (IRegisteredObject) Activator.CreateInstance(type);
                    this._wellKnownObjects[fullName] = o;
                }
                else
                {
                    flag = true;
                }
            }
            if (flag && failIfExists)
            {
                throw new InvalidOperationException(System.Web.SR.GetString("Wellknown_object_already_exists", new object[] { fullName }));
            }
            return new ObjectHandle(o);
        }

        public static void DecrementBusyCount()
        {
            if (_theHostingEnvironment != null)
            {
                _theHostingEnvironment.DecrementBusyCountInternal();
            }
        }

        internal void DecrementBusyCountInternal()
        {
            this._isBusy = true;
            Interlocked.Decrement(ref this._busyCount);
            IdleTimeoutMonitor monitor = this._idleTimeoutMonitor;
            if (monitor != null)
            {
                monitor.LastEvent = DateTime.UtcNow;
            }
        }

        private void EnforceAppDomainLimit()
        {
            if (this._appManager != null)
            {
                int limit = 0;
                try
                {
                    limit = RuntimeConfig.GetMachineConfig().ProcessModel.MaxAppDomains;
                }
                catch
                {
                }
                if ((limit > 0) && (this._appManager.AppDomainsCount >= limit))
                {
                    this._appManager.ReduceAppDomainsCount(limit);
                }
            }
        }

        internal ObjectHandle FindWellKnownObject(Type type)
        {
            IRegisteredObject o = null;
            string fullName = type.FullName;
            lock (this)
            {
                o = this._wellKnownObjects[fullName] as IRegisteredObject;
            }
            if (o == null)
            {
                return null;
            }
            return new ObjectHandle(o);
        }

        private string GetAppConfigPath()
        {
            if (this._appConfigPath == null)
            {
                this._appConfigPath = WebConfigurationHost.GetConfigPathFromSiteIDAndVPath(SiteID, ApplicationVirtualPathObject);
            }
            return this._appConfigPath;
        }

        private void GetApplicationIdentity()
        {
            try
            {
                IdentitySection identity = RuntimeConfig.GetAppConfig().Identity;
                if (identity.Impersonate && (identity.ImpersonateToken != IntPtr.Zero))
                {
                    this._appIdentity = identity;
                    this._appIdentityToken = identity.ImpersonateToken;
                }
                else
                {
                    this._appIdentityToken = this._configToken;
                }
                this._appIdentityTokenSet = true;
            }
            catch
            {
            }
        }

        internal ApplicationInfo GetApplicationInfo() => 
            new ApplicationInfo(this._appId, this._appVirtualPath, this._appPhysicalPath);

        internal static ApplicationManager GetApplicationManager()
        {
            if (_theHostingEnvironment == null)
            {
                return null;
            }
            return _theHostingEnvironment._appManager;
        }

        private static string GetFixedMappingSlotName(VirtualPath virtualPath) => 
            ("MapPath_" + virtualPath.VirtualPathString.ToLowerInvariant().GetHashCode().ToString(CultureInfo.InvariantCulture));

        internal bool GetIdleValue() => 
            (!this._isBusy && (this._busyCount == 0));

        internal static WebApplicationLevel GetPathLevel(string path)
        {
            WebApplicationLevel aboveApplication = WebApplicationLevel.AboveApplication;
            if ((_theHostingEnvironment != null) && !string.IsNullOrEmpty(path))
            {
                string applicationVirtualPath = ApplicationVirtualPath;
                if (applicationVirtualPath == "/")
                {
                    if (path == "/")
                    {
                        return WebApplicationLevel.AtApplication;
                    }
                    if (path[0] == '/')
                    {
                        aboveApplication = WebApplicationLevel.BelowApplication;
                    }
                    return aboveApplication;
                }
                if (StringUtil.EqualsIgnoreCase(applicationVirtualPath, path))
                {
                    return WebApplicationLevel.AtApplication;
                }
                if (((path.Length > applicationVirtualPath.Length) && (path[applicationVirtualPath.Length] == '/')) && StringUtil.StringStartsWithIgnoreCase(path, applicationVirtualPath))
                {
                    aboveApplication = WebApplicationLevel.BelowApplication;
                }
            }
            return aboveApplication;
        }

        private string GetSiteID()
        {
            if (this._siteID == null)
            {
                lock (this)
                {
                    if (this._siteID == null)
                    {
                        string siteID = null;
                        if (this._appHost != null)
                        {
                            InternalSecurityPermissions.Unrestricted.Assert();
                            try
                            {
                                siteID = this._appHost.GetSiteID();
                            }
                            finally
                            {
                                CodeAccessPermission.RevertAssert();
                            }
                        }
                        if (siteID == null)
                        {
                            siteID = "1";
                        }
                        this._siteID = siteID.ToLower(CultureInfo.InvariantCulture);
                    }
                }
            }
            return this._siteID;
        }

        private string GetSiteName()
        {
            if (this._siteName == null)
            {
                lock (this)
                {
                    if (this._siteName == null)
                    {
                        string siteName = null;
                        if (this._appHost != null)
                        {
                            InternalSecurityPermissions.Unrestricted.Assert();
                            try
                            {
                                siteName = this._appHost.GetSiteName();
                            }
                            finally
                            {
                                CodeAccessPermission.RevertAssert();
                            }
                        }
                        if (siteName == null)
                        {
                            siteName = WebConfigurationHost.DefaultSiteName;
                        }
                        this._siteName = siteName;
                    }
                }
            }
            return this._siteName;
        }

        private static string GetVirtualPathToFileMapping(VirtualPath virtualPath) => 
            (CallContext.GetData(GetFixedMappingSlotName(virtualPath)) as string);

        [SecurityPermission(SecurityAction.Demand, ControlPrincipal=true)]
        public static IDisposable Impersonate() => 
            new ApplicationImpersonationContext();

        [SecurityPermission(SecurityAction.Demand, Unrestricted=true)]
        public static IDisposable Impersonate(IntPtr token)
        {
            if (token == IntPtr.Zero)
            {
                return new ProcessImpersonationContext();
            }
            return new ImpersonationContext(token);
        }

        [SecurityPermission(SecurityAction.Demand, Unrestricted=true)]
        public static IDisposable Impersonate(IntPtr userToken, string virtualPath)
        {
            virtualPath = UrlPath.MakeVirtualPathAppAbsoluteReduceAndCheck(virtualPath);
            if (_theHostingEnvironment == null)
            {
                return Impersonate(userToken);
            }
            IdentitySection identity = RuntimeConfig.GetConfig(virtualPath).Identity;
            if (!identity.Impersonate)
            {
                return new ApplicationImpersonationContext();
            }
            if (identity.ImpersonateToken != IntPtr.Zero)
            {
                return new ImpersonationContext(identity.ImpersonateToken);
            }
            return new ImpersonationContext(userToken);
        }

        public static void IncrementBusyCount()
        {
            if (_theHostingEnvironment != null)
            {
                _theHostingEnvironment.IncrementBusyCountInternal();
            }
        }

        internal void IncrementBusyCountInternal()
        {
            this._isBusy = true;
            Interlocked.Increment(ref this._busyCount);
        }

        internal void Initialize(ApplicationManager appManager, IApplicationHost appHost, IConfigMapPathFactory configMapPathFactory, HostingEnvironmentParameters hostingParameters)
        {
            this._hostingParameters = hostingParameters;
            HostingEnvironmentFlags hostingFlags = HostingEnvironmentFlags.Default;
            if (this._hostingParameters != null)
            {
                hostingFlags = this._hostingParameters.HostingFlags;
            }
            if ((hostingFlags & HostingEnvironmentFlags.HideFromAppManager) == HostingEnvironmentFlags.Default)
            {
                this._appManager = appManager;
            }
            if ((hostingFlags & HostingEnvironmentFlags.ClientBuildManager) != HostingEnvironmentFlags.Default)
            {
                BuildManagerHost.InClientBuildManager = true;
            }
            if ((appHost is ISAPIApplicationHost) && !ServerConfig.UseMetabase)
            {
                string str = ((ISAPIApplicationHost) appHost).ResolveRootWebConfigPath();
                if (!string.IsNullOrEmpty(str))
                {
                    HttpConfigurationSystem.RootWebConfigurationFilePath = str;
                }
                IProcessHostSupportFunctions supportFunctions = ((ISAPIApplicationHost) appHost).SupportFunctions;
                if (supportFunctions != null)
                {
                    _functions = Misc.CreateLocalSupportFunctions(supportFunctions);
                }
            }
            this._appId = HttpRuntime.AppDomainAppIdInternal;
            this._appVirtualPath = HttpRuntime.AppDomainAppVirtualPathObject;
            this._appPhysicalPath = HttpRuntime.AppDomainAppPathInternal;
            this._appHost = appHost;
            this._configMapPath = configMapPathFactory.Create(this._appVirtualPath.VirtualPathString, this._appPhysicalPath);
            HttpConfigurationSystem.EnsureInit(this._configMapPath, true, false);
            this._configMapPath2 = this._configMapPath as IConfigMapPath2;
            this._initiateShutdownWorkItemCallback = new WaitCallback(this.InitiateShutdownWorkItemCallback);
            if (this._appManager != null)
            {
                this._appManager.HostingEnvironmentActivated(this._appId);
            }
            if (this._appHost == null)
            {
                this._appHost = new SimpleApplicationHost(this._appVirtualPath, this._appPhysicalPath);
            }
            else
            {
                this._externalAppHost = true;
            }
            this._configToken = this._appHost.GetConfigToken();
            this._mapPathBasedVirtualPathProvider = new MapPathBasedVirtualPathProvider();
            this._virtualPathProvider = this._mapPathBasedVirtualPathProvider;
            HttpRuntime.InitializeHostingFeatures(hostingFlags);
            if (!BuildManagerHost.InClientBuildManager)
            {
                this.StartMonitoringForIdleTimeout();
            }
            this.EnforceAppDomainLimit();
            this.GetApplicationIdentity();
            if (((hostingFlags & HostingEnvironmentFlags.DontCallAppInitialize) == HostingEnvironmentFlags.Default) && !HttpRuntime.HostingInitFailed)
            {
                try
                {
                    BuildManager.CallAppInitializeMethod();
                }
                catch (Exception exception)
                {
                    HttpRuntime.InitializationException = exception;
                    if ((hostingFlags & HostingEnvironmentFlags.ThrowHostingInitErrors) != HostingEnvironmentFlags.Default)
                    {
                        throw;
                    }
                }
            }
        }

        public override object InitializeLifetimeService() => 
            null;

        [SecurityPermission(SecurityAction.Demand, Unrestricted=true)]
        public static void InitiateShutdown()
        {
            if (_theHostingEnvironment != null)
            {
                _theHostingEnvironment.InitiateShutdownInternal();
            }
        }

        internal void InitiateShutdownInternal()
        {
            bool flag = false;
            if (!this._shutdownInitated)
            {
                lock (this)
                {
                    if (!this._shutdownInitated)
                    {
                        this._shutdownInProgress = true;
                        flag = true;
                        this._shutdownInitated = true;
                    }
                }
            }
            if (flag)
            {
                HttpRuntime.SetShutdownReason(ApplicationShutdownReason.HostingEnvironment, "HostingEnvironment initiated shutdown");
                new EnvironmentPermission(PermissionState.Unrestricted).Assert();
                try
                {
                    this._shutDownStack = Environment.StackTrace;
                }
                finally
                {
                    CodeAccessPermission.RevertAssert();
                }
                this.RemoveThisAppDomainFromAppManagerTableOnce();
                ThreadPool.QueueUserWorkItem(this._initiateShutdownWorkItemCallback);
            }
        }

        private void InitiateShutdownWorkItemCallback(object state)
        {
            if (this._registeredObjects.Count == 0)
            {
                this.ShutdownThisAppDomainOnce();
            }
            else
            {
                this.StopRegisteredObjects(false);
                if (this._registeredObjects.Count == 0)
                {
                    this.ShutdownThisAppDomainOnce();
                }
                else
                {
                    int totalSeconds = 30;
                    HostingEnvironmentSection hostingEnvironment = RuntimeConfig.GetAppLKGConfig().HostingEnvironment;
                    if (hostingEnvironment != null)
                    {
                        totalSeconds = (int) hostingEnvironment.ShutdownTimeout.TotalSeconds;
                    }
                    DateTime time = DateTime.UtcNow.AddSeconds((double) totalSeconds);
                    while ((this._registeredObjects.Count > 0) && (DateTime.UtcNow < time))
                    {
                        Thread.Sleep(100);
                    }
                    this.StopRegisteredObjects(true);
                    if (this._registeredObjects.Count == 0)
                    {
                        this.ShutdownThisAppDomainOnce();
                    }
                    else
                    {
                        this._registeredObjects = new Hashtable();
                        this.ShutdownThisAppDomainOnce();
                    }
                }
            }
        }

        internal bool IsIdle()
        {
            bool flag = this._isBusy;
            this._isBusy = false;
            return (!flag && (this._busyCount == 0));
        }

        internal void IsUnloaded()
        {
        }

        private bool IsWellKnownObject(object obj)
        {
            bool flag = false;
            string fullName = obj.GetType().FullName;
            lock (this)
            {
                if (this._wellKnownObjects[fullName] == obj)
                {
                    flag = true;
                }
            }
            return flag;
        }

        public static string MapPath(string virtualPath) => 
            MapPath(VirtualPath.Create(virtualPath));

        internal static string MapPath(VirtualPath virtualPath)
        {
            if (_theHostingEnvironment == null)
            {
                return null;
            }
            string path = MapPathInternal(virtualPath);
            if (path != null)
            {
                InternalSecurityPermissions.PathDiscovery(path).Demand();
            }
            return path;
        }

        private string MapPathActual(VirtualPath virtualPath, bool permitNull)
        {
            string virtualPathToFileMapping = null;
            virtualPath.FailIfRelativePath();
            VirtualPath path = virtualPath;
            if (string.CompareOrdinal(path.VirtualPathString, this._appVirtualPath.VirtualPathString) == 0)
            {
                virtualPathToFileMapping = this._appPhysicalPath;
            }
            else
            {
                using (new ProcessImpersonationContext())
                {
                    virtualPathToFileMapping = GetVirtualPathToFileMapping(path);
                    if (virtualPathToFileMapping == null)
                    {
                        if (this._configMapPath == null)
                        {
                            throw new InvalidOperationException(System.Web.SR.GetString("Cannot_map_path", new object[] { path }));
                        }
                        if (this._configMapPath2 != null)
                        {
                            virtualPathToFileMapping = this._configMapPath2.MapPath(this.GetSiteID(), path);
                        }
                        else
                        {
                            virtualPathToFileMapping = this._configMapPath.MapPath(this.GetSiteID(), path.VirtualPathString);
                        }
                    }
                }
            }
            if (string.IsNullOrEmpty(virtualPathToFileMapping))
            {
                if (!permitNull)
                {
                    throw new InvalidOperationException(System.Web.SR.GetString("Cannot_map_path", new object[] { path }));
                }
                return virtualPathToFileMapping;
            }
            if (virtualPath.HasTrailingSlash)
            {
                if (!UrlPath.PathEndsWithExtraSlash(virtualPathToFileMapping) && !UrlPath.PathIsDriveRoot(virtualPathToFileMapping))
                {
                    virtualPathToFileMapping = virtualPathToFileMapping + @"\";
                }
                return virtualPathToFileMapping;
            }
            if (UrlPath.PathEndsWithExtraSlash(virtualPathToFileMapping) && !UrlPath.PathIsDriveRoot(virtualPathToFileMapping))
            {
                virtualPathToFileMapping = virtualPathToFileMapping.Substring(0, virtualPathToFileMapping.Length - 1);
            }
            return virtualPathToFileMapping;
        }

        internal static string MapPathInternal(string virtualPath) => 
            MapPathInternal(VirtualPath.Create(virtualPath));

        internal static string MapPathInternal(VirtualPath virtualPath) => 
            _theHostingEnvironment?.MapPathActual(virtualPath, false);

        internal static string MapPathInternal(string virtualPath, bool permitNull) => 
            MapPathInternal(VirtualPath.Create(virtualPath), permitNull);

        internal static string MapPathInternal(VirtualPath virtualPath, bool permitNull) => 
            _theHostingEnvironment?.MapPathActual(virtualPath, permitNull);

        internal static string MapPathInternal(string virtualPath, string baseVirtualDir, bool allowCrossAppMapping) => 
            MapPathInternal(VirtualPath.Create(virtualPath), VirtualPath.CreateNonRelative(baseVirtualDir), allowCrossAppMapping);

        internal static string MapPathInternal(VirtualPath virtualPath, VirtualPath baseVirtualDir, bool allowCrossAppMapping)
        {
            virtualPath = baseVirtualDir.Combine(virtualPath);
            if (!allowCrossAppMapping && !virtualPath.IsWithinAppRoot)
            {
                throw new ArgumentException(System.Web.SR.GetString("Cross_app_not_allowed", new object[] { virtualPath }));
            }
            return MapPathInternal(virtualPath);
        }

        public static void MessageReceived()
        {
            if (_theHostingEnvironment != null)
            {
                _theHostingEnvironment.MessageReceivedInternal();
            }
        }

        private void MessageReceivedInternal()
        {
            this._isBusy = true;
            IdleTimeoutMonitor monitor = this._idleTimeoutMonitor;
            if (monitor != null)
            {
                monitor.LastEvent = DateTime.UtcNow;
            }
        }

        private void OnAppDomainUnload(object unusedObject, EventArgs unusedEventArgs)
        {
            Thread.GetDomain().DomainUnload -= this._onAppDomainUnload;
            if (!this._removedFromAppManager)
            {
                this.RemoveThisAppDomainFromAppManagerTableOnce();
            }
            HttpRuntime.RecoverFromUnexceptedAppDomainUnload();
            this.StopRegisteredObjects(true);
            if (this._appManager != null)
            {
                IApplicationHost appHost = null;
                if (this._externalAppHost)
                {
                    appHost = this._appHost;
                    this._appHost = new SimpleApplicationHost(this._appVirtualPath, this._appPhysicalPath);
                    this._externalAppHost = false;
                }
                this._appManager.HostingEnvironmentShutdownComplete(this._appId, appHost);
            }
            if (this._configToken != IntPtr.Zero)
            {
                System.Web.UnsafeNativeMethods.CloseHandle(this._configToken);
                this._configToken = IntPtr.Zero;
            }
        }

        [SecurityPermission(SecurityAction.Demand, Unrestricted=true)]
        public static void RegisterObject(IRegisteredObject obj)
        {
            if (_theHostingEnvironment != null)
            {
                _theHostingEnvironment.RegisterRunningObjectInternal(obj);
            }
        }

        private void RegisterRunningObjectInternal(IRegisteredObject obj)
        {
            lock (this)
            {
                this._registeredObjects[obj] = obj;
            }
        }

        [AspNetHostingPermission(SecurityAction.Demand, Level=AspNetHostingPermissionLevel.High)]
        public static void RegisterVirtualPathProvider(System.Web.Hosting.VirtualPathProvider virtualPathProvider)
        {
            if (_theHostingEnvironment == null)
            {
                throw new InvalidOperationException();
            }
            if (!BuildManager.IsPrecompiledApp)
            {
                RegisterVirtualPathProviderInternal(virtualPathProvider);
            }
        }

        internal static void RegisterVirtualPathProviderInternal(System.Web.Hosting.VirtualPathProvider virtualPathProvider)
        {
            System.Web.Hosting.VirtualPathProvider previous = _theHostingEnvironment._virtualPathProvider;
            _theHostingEnvironment._virtualPathProvider = virtualPathProvider;
            virtualPathProvider.Initialize(previous);
        }

        private void RemoveThisAppDomainFromAppManagerTableOnce()
        {
            bool flag = false;
            if (!this._removedFromAppManager)
            {
                lock (this)
                {
                    if (!this._removedFromAppManager)
                    {
                        flag = true;
                        this._removedFromAppManager = true;
                    }
                }
            }
            if (flag && (this._appManager != null))
            {
                this._appManager.HostingEnvironmentShutdownInitiated(this._appId, this);
            }
        }

        public static IDisposable SetCultures() => 
            SetCultures(RuntimeConfig.GetAppLKGConfig().Globalization);

        public static IDisposable SetCultures(string virtualPath)
        {
            virtualPath = UrlPath.MakeVirtualPathAppAbsoluteReduceAndCheck(virtualPath);
            return SetCultures(RuntimeConfig.GetConfig(virtualPath).Globalization);
        }

        private static IDisposable SetCultures(GlobalizationSection gs)
        {
            CultureContext context = new CultureContext();
            if (gs != null)
            {
                CultureInfo culture = null;
                CultureInfo uiCulture = null;
                if ((gs.Culture != null) && (gs.Culture.Length > 0))
                {
                    try
                    {
                        culture = HttpServerUtility.CreateReadOnlyCultureInfo(gs.Culture);
                    }
                    catch
                    {
                    }
                }
                if ((gs.UICulture != null) && (gs.UICulture.Length > 0))
                {
                    try
                    {
                        uiCulture = HttpServerUtility.CreateReadOnlyCultureInfo(gs.UICulture);
                    }
                    catch
                    {
                    }
                }
                context.SetCultures(culture, uiCulture);
            }
            return context;
        }

        private void ShutdownThisAppDomainOnce()
        {
            bool flag = false;
            if (!this._appDomainShutdownStarted)
            {
                lock (this)
                {
                    if (!this._appDomainShutdownStarted)
                    {
                        flag = true;
                        this._appDomainShutdownStarted = true;
                    }
                }
            }
            if (flag)
            {
                if (this._idleTimeoutMonitor != null)
                {
                    this._idleTimeoutMonitor.Stop();
                    this._idleTimeoutMonitor = null;
                }
                HttpRuntime.SetUserForcedShutdown();
                this._shutdownInProgress = false;
                HttpRuntime.ShutdownAppDomainWithStackTrace(ApplicationShutdownReason.HostingEnvironment, System.Web.SR.GetString("Hosting_Env_Restart"), this._shutDownStack);
            }
        }

        private void StartMonitoringForIdleTimeout()
        {
            HostingEnvironmentSection hostingEnvironment = RuntimeConfig.GetAppLKGConfig().HostingEnvironment;
            TimeSpan timeout = (hostingEnvironment != null) ? hostingEnvironment.IdleTimeout : HostingEnvironmentSection.DefaultIdleTimeout;
            this._idleTimeoutMonitor = new IdleTimeoutMonitor(timeout);
        }

        private void StopRegisteredObjects(bool immediate)
        {
            if (this._registeredObjects.Count > 0)
            {
                ArrayList list = new ArrayList();
                lock (this)
                {
                    foreach (DictionaryEntry entry in this._registeredObjects)
                    {
                        object key = entry.Key;
                        if (this.IsWellKnownObject(key))
                        {
                            list.Insert(0, key);
                        }
                        else
                        {
                            list.Add(key);
                        }
                    }
                }
                foreach (IRegisteredObject obj3 in list)
                {
                    try
                    {
                        obj3.Stop(immediate);
                    }
                    catch
                    {
                    }
                }
            }
        }

        internal void StopWellKnownObject(Type type)
        {
            IRegisteredObject obj2 = null;
            string fullName = type.FullName;
            lock (this)
            {
                obj2 = this._wellKnownObjects[fullName] as IRegisteredObject;
                if (obj2 != null)
                {
                    this._wellKnownObjects.Remove(fullName);
                    obj2.Stop(false);
                }
            }
        }

        [SecurityPermission(SecurityAction.Demand, Unrestricted=true)]
        public static void UnregisterObject(IRegisteredObject obj)
        {
            if (_theHostingEnvironment != null)
            {
                _theHostingEnvironment.UnregisterRunningObjectInternal(obj);
            }
        }

        private void UnregisterRunningObjectInternal(IRegisteredObject obj)
        {
            bool flag = false;
            lock (this)
            {
                string fullName = obj.GetType().FullName;
                if (this._wellKnownObjects[fullName] == obj)
                {
                    this._wellKnownObjects.Remove(fullName);
                }
                this._registeredObjects.Remove(obj);
                if (this._registeredObjects.Count == 0)
                {
                    flag = true;
                }
            }
            if (flag)
            {
                this.InitiateShutdownInternal();
            }
        }

        internal static string AppConfigPath =>
            _theHostingEnvironment?.GetAppConfigPath();

        internal static int AppDomainsCount
        {
            get
            {
                ApplicationManager applicationManager = GetApplicationManager();
                return applicationManager?.AppDomainsCount;
            }
        }

        internal static int AppDomainUniqueInteger
        {
            get
            {
                if (s_appDomainUniqueInteger == 0)
                {
                    s_appDomainUniqueInteger = Guid.NewGuid().GetHashCode();
                }
                return s_appDomainUniqueInteger;
            }
        }

        public static IApplicationHost ApplicationHost
        {
            [SecurityPermission(SecurityAction.Demand, Unrestricted=true)]
            get
            {
                if (_theHostingEnvironment == null)
                {
                    return null;
                }
                return _theHostingEnvironment._appHost;
            }
        }

        public static string ApplicationID
        {
            get
            {
                if (_theHostingEnvironment == null)
                {
                    return null;
                }
                InternalSecurityPermissions.AspNetHostingPermissionLevelHigh.Demand();
                return _theHostingEnvironment._appId;
            }
        }

        internal static IntPtr ApplicationIdentityToken
        {
            get
            {
                if (_theHostingEnvironment == null)
                {
                    return IntPtr.Zero;
                }
                if (_theHostingEnvironment._appIdentityTokenSet)
                {
                    return _theHostingEnvironment._appIdentityToken;
                }
                return _theHostingEnvironment._configToken;
            }
        }

        internal static string ApplicationIDNoDemand
        {
            get
            {
                if (_theHostingEnvironment == null)
                {
                    return null;
                }
                return _theHostingEnvironment._appId;
            }
        }

        public static string ApplicationPhysicalPath
        {
            get
            {
                if (_theHostingEnvironment == null)
                {
                    return null;
                }
                InternalSecurityPermissions.AppPathDiscovery.Demand();
                return _theHostingEnvironment._appPhysicalPath;
            }
        }

        public static string ApplicationVirtualPath =>
            VirtualPath.GetVirtualPathStringNoTrailingSlash(ApplicationVirtualPathObject);

        internal static VirtualPath ApplicationVirtualPathObject
        {
            get
            {
                if (_theHostingEnvironment == null)
                {
                    return null;
                }
                return _theHostingEnvironment._appVirtualPath;
            }
        }

        internal static int BusyCount
        {
            get
            {
                if (_theHostingEnvironment == null)
                {
                    return 0;
                }
                return _theHostingEnvironment._busyCount;
            }
        }

        public static System.Web.Caching.Cache Cache =>
            HttpRuntime.Cache;

        internal static IConfigMapPath ConfigMapPath
        {
            get
            {
                if (_theHostingEnvironment == null)
                {
                    return null;
                }
                return _theHostingEnvironment._configMapPath;
            }
        }

        internal bool HasBeenRemovedFromAppManagerTable
        {
            get => 
                _hasBeenRemovedFromAppManangerTable;
            set
            {
                _hasBeenRemovedFromAppManangerTable = value;
            }
        }

        internal static bool HasHostingIdentity =>
            (ApplicationIdentityToken != IntPtr.Zero);

        internal static HostingEnvironmentParameters HostingParameters
        {
            get
            {
                if (_theHostingEnvironment == null)
                {
                    return null;
                }
                return _theHostingEnvironment._hostingParameters;
            }
        }

        public static Exception InitializationException =>
            HttpRuntime.InitializationException;

        internal IApplicationHost InternalApplicationHost =>
            this._appHost;

        public static bool IsHosted =>
            (_theHostingEnvironment != null);

        internal static bool IsUnderIIS6Process =>
            (VersionInfo.ExeName == "w3wp");

        internal static bool IsUnderIISProcess
        {
            get
            {
                string exeName = VersionInfo.ExeName;
                if ((exeName != "aspnet_wp") && (exeName != "w3wp"))
                {
                    return (exeName == "inetinfo");
                }
                return true;
            }
        }

        internal int LruScore
        {
            get
            {
                if (this._busyCount > 0)
                {
                    return this._busyCount;
                }
                IdleTimeoutMonitor monitor = this._idleTimeoutMonitor;
                if (monitor == null)
                {
                    return 0;
                }
                TimeSpan span = (TimeSpan) (DateTime.UtcNow - monitor.LastEvent);
                return -((int) span.TotalSeconds);
            }
        }

        internal static bool ShutdownInitiated
        {
            get
            {
                if (_theHostingEnvironment == null)
                {
                    return false;
                }
                return _theHostingEnvironment._shutdownInitated;
            }
        }

        internal static bool ShutdownInProgress
        {
            get
            {
                if (_theHostingEnvironment == null)
                {
                    return false;
                }
                return _theHostingEnvironment._shutdownInProgress;
            }
        }

        public static ApplicationShutdownReason ShutdownReason =>
            HttpRuntime.ShutdownReason;

        internal static string SiteID =>
            _theHostingEnvironment?.GetSiteID();

        public static string SiteName
        {
            get
            {
                if (_theHostingEnvironment == null)
                {
                    return null;
                }
                InternalSecurityPermissions.AspNetHostingPermissionLevelMedium.Demand();
                return _theHostingEnvironment.GetSiteName();
            }
        }

        internal static string SiteNameNoDemand =>
            _theHostingEnvironment?.GetSiteName();

        internal static IProcessHostSupportFunctions SupportFunctions
        {
            get => 
                _functions;
            set
            {
                _functions = value;
            }
        }

        internal static bool UsingMapPathBasedVirtualPathProvider =>
            ((_theHostingEnvironment == null) || (_theHostingEnvironment._virtualPathProvider == _theHostingEnvironment._mapPathBasedVirtualPathProvider));

        public static System.Web.Hosting.VirtualPathProvider VirtualPathProvider
        {
            get
            {
                if (_theHostingEnvironment == null)
                {
                    return null;
                }
                return _theHostingEnvironment._virtualPathProvider;
            }
        }

        private class CultureContext : IDisposable
        {
            private CultureInfo _savedCulture;
            private CultureInfo _savedUICulture;

            internal CultureContext()
            {
            }

            internal void RestoreCultures()
            {
                if ((this._savedCulture != null) && (this._savedCulture != Thread.CurrentThread.CurrentCulture))
                {
                    Thread.CurrentThread.CurrentCulture = this._savedCulture;
                    this._savedCulture = null;
                }
                if ((this._savedUICulture != null) && (this._savedUICulture != Thread.CurrentThread.CurrentUICulture))
                {
                    Thread.CurrentThread.CurrentUICulture = this._savedUICulture;
                    this._savedUICulture = null;
                }
            }

            internal void SetCultures(CultureInfo culture, CultureInfo uiCulture)
            {
                CultureInfo currentCulture = Thread.CurrentThread.CurrentCulture;
                CultureInfo currentUICulture = Thread.CurrentThread.CurrentUICulture;
                if ((culture != null) && (culture != currentCulture))
                {
                    Thread.CurrentThread.CurrentCulture = culture;
                    this._savedCulture = currentCulture;
                }
                if ((uiCulture != null) && (uiCulture != currentCulture))
                {
                    Thread.CurrentThread.CurrentUICulture = uiCulture;
                    this._savedUICulture = currentUICulture;
                }
            }

            void IDisposable.Dispose()
            {
                this.RestoreCultures();
            }
        }

        internal class VirtualPathToFileMappingState
        {
            internal System.Web.VirtualPath VirtualPath;
            internal System.Web.Hosting.VirtualPathProvider VirtualPathProvider;
        }
    }
}

