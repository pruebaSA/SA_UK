namespace System.ServiceModel
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Security;
    using System.Security.Permissions;
    using System.ServiceModel.Activation;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Configuration;
    using System.ServiceModel.Description;
    using System.ServiceModel.Diagnostics;
    using System.Threading;
    using System.Web;
    using System.Web.Compilation;
    using System.Web.Configuration;
    using System.Web.Hosting;

    public static class ServiceHostingEnvironment
    {
        private static bool canGetHtmlErrorMessage = true;
        private static bool didAssemblyCheck;
        private const char FileExtensionSeparator = '.';
        private static HostingManager hostingManager;
        internal const string ISAPIApplicationIdPrefix = "/LM/W3SVC/";
        private static bool isApplicationDomainHosted;
        private static bool isHosted;
        private static bool isSimpleApplicationHost;
        private const char PathSeparator = '/';
        internal const string RelativeVirtualPathPrefix = "~";
        private static long requestCount;
        internal const string ServiceParserDelimiter = "|";
        private static object syncRoot = new object();
        private const string SystemWebComma = "System.Web,";
        private const char UriSchemeSeparator = ':';
        internal const string VerbPost = "POST";

        internal static void DecrementBusyCount()
        {
            if (IsHosted)
            {
                HostingEnvironmentWrapper.DecrementBusyCount();
            }
        }

        internal static void DecrementRequestCount()
        {
            Interlocked.Decrement(ref requestCount);
            if ((requestCount == 0L) && (hostingManager != null))
            {
                hostingManager.NotifyAllRequestDone();
            }
        }

        internal static void EnsureInitialized()
        {
            if (hostingManager == null)
            {
                lock (ThisLock)
                {
                    if (hostingManager == null)
                    {
                        if (!HostingEnvironmentWrapper.IsHosted)
                        {
                            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(System.ServiceModel.SR.GetString("Hosting_ProcessNotExecutingUnderHostedContext", new object[] { "ServiceHostingEnvironment.EnsureServiceAvailable" })));
                        }
                        HostingManager manager = new HostingManager();
                        HookADUnhandledExceptionEvent();
                        Thread.MemoryBarrier();
                        isSimpleApplicationHost = GetIsSimpleApplicationHost();
                        hostingManager = manager;
                        isHosted = true;
                    }
                }
            }
        }

        public static void EnsureServiceAvailable(string virtualPath)
        {
            if (string.IsNullOrEmpty(virtualPath))
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("virtualPath"));
            }
            if (virtualPath.IndexOf(':') > 0)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentException(System.ServiceModel.SR.GetString("Hosting_AddressIsAbsoluteUri", new object[] { virtualPath }), "virtualPath"));
            }
            EnsureInitialized();
            virtualPath = NormalizeVirtualPath(virtualPath);
            EnsureServiceAvailableFast(virtualPath);
        }

        internal static void EnsureServiceAvailableFast(string relativeVirtualPath)
        {
            try
            {
                hostingManager.EnsureServiceAvailable(relativeVirtualPath);
            }
            catch (ServiceActivationException exception)
            {
                LogServiceActivationException(exception);
                throw;
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static Uri[] GetBaseAddressPrefixFilters() => 
            hostingManager.BaseAddressPrefixFilters;

        internal static bool GetExtensionSupported(string extension) => 
            hostingManager.GetExtensionSupported(extension);

        [SecurityTreatAsSafe, SecurityCritical]
        private static bool GetIsSimpleApplicationHost() => 
            (string.Compare("/LM/W3SVC/", 0, HostingEnvironmentWrapper.UnsafeApplicationID, 0, "/LM/W3SVC/".Length, StringComparison.OrdinalIgnoreCase) != 0);

        [SecurityTreatAsSafe, SecurityCritical]
        private static void HookADUnhandledExceptionEvent()
        {
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(ServiceHostingEnvironment.OnUnhandledException);
        }

        internal static void IncrementBusyCount()
        {
            if (IsHosted)
            {
                HostingEnvironmentWrapper.IncrementBusyCount();
            }
        }

        internal static void IncrementRequestCount()
        {
            Interlocked.Increment(ref requestCount);
        }

        [MethodImpl(MethodImplOptions.NoInlining), SecurityTreatAsSafe, SecurityCritical, AspNetHostingPermission(SecurityAction.Assert, Level=AspNetHostingPermissionLevel.Minimal)]
        private static bool IsApplicationDomainHosted() => 
            HostingEnvironment.IsHosted;

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static bool IsAspNetCompatibilityEnabled() => 
            hostingManager.AspNetCompatibilityEnabled;

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static bool IsMultipleSiteBindingsEnabled() => 
            hostingManager.MultipleSiteBindingsEnabled;

        [SecurityTreatAsSafe, SecurityCritical]
        private static void LogServiceActivationException(ServiceActivationException exception)
        {
            if (exception.InnerException is HttpException)
            {
                string message = SafeTryGetHtmlErrorMessage((HttpException) exception.InnerException);
                if ((message == null) || (message.Length == 0))
                {
                    message = exception.Message;
                }
                DiagnosticUtility.UnsafeEventLog.UnsafeLogEvent(TraceEventType.Error, EventLogCategory.WebHost, (EventLogEventId) (-1073610750), true, new string[] { DiagnosticTrace.CreateSourceString(hostingManager), message, (exception == null) ? string.Empty : exception.ToString() });
            }
            else
            {
                DiagnosticUtility.UnsafeEventLog.UnsafeLogEvent(TraceEventType.Error, EventLogCategory.WebHost, (EventLogEventId) (-1073610749), true, new string[] { DiagnosticTrace.CreateSourceString(hostingManager), (exception == null) ? string.Empty : exception.ToString() });
            }
        }

        internal static string NormalizeVirtualPath(string virtualPath)
        {
            string str = null;
            try
            {
                str = VirtualPathUtility.ToAppRelative(virtualPath, HostingEnvironmentWrapper.ApplicationVirtualPath);
            }
            catch (HttpException exception)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentException(exception.Message, "virtualPath", exception));
            }
            if (string.IsNullOrEmpty(str) || !str.StartsWith("~", StringComparison.Ordinal))
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentException(System.ServiceModel.SR.GetString("Hosting_AddressPointsOutsideTheVirtualDirectory", new object[] { virtualPath, HostingEnvironmentWrapper.ApplicationVirtualPath })));
            }
            int index = str.IndexOf('.');
            while (index > 0)
            {
                index = str.IndexOf('/', index + 1);
                string str2 = (index == -1) ? str : str.Substring(0, index);
                string extension = VirtualPathUtility.GetExtension(str2);
                if (!string.IsNullOrEmpty(extension) && GetExtensionSupported(extension))
                {
                    return str2;
                }
            }
            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new EndpointNotFoundException(System.ServiceModel.SR.GetString("Hosting_ServiceNotExist", new object[] { virtualPath })));
        }

        private static void OnEnsureInitialized(object state)
        {
            EnsureInitialized();
        }

        [SecurityTreatAsSafe, SecurityCritical]
        private static void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (DiagnosticUtility.ShouldTraceError)
            {
                Exception exceptionObject = e.ExceptionObject as Exception;
                DiagnosticUtility.UnsafeEventLog.UnsafeLogEvent(TraceEventType.Error, EventLogCategory.WebHost, (EventLogEventId) (-1073610751), true, new string[] { DiagnosticTrace.CreateSourceString(sender), (exceptionObject == null) ? string.Empty : exceptionObject.ToString() });
            }
        }

        internal static void ProcessNotMatchedEndpointAddress(Uri uri, string endpointName)
        {
            if ((AspNetCompatibilityEnabled && !object.ReferenceEquals(uri.Scheme, Uri.UriSchemeHttp)) && !object.ReferenceEquals(uri.Scheme, Uri.UriSchemeHttps))
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(System.ServiceModel.SR.GetString("Hosting_NonHTTPInCompatibilityMode", new object[] { endpointName })));
            }
        }

        internal static void SafeEnsureInitialized()
        {
            if (hostingManager == null)
            {
                PartialTrustHelpers.PartialTrustInvoke(new ContextCallback(ServiceHostingEnvironment.OnEnsureInitialized), null);
            }
        }

        private static string SafeTryGetHtmlErrorMessage(HttpException exception)
        {
            if ((exception != null) && canGetHtmlErrorMessage)
            {
                try
                {
                    return exception.GetHtmlErrorMessage();
                }
                catch (SecurityException exception2)
                {
                    canGetHtmlErrorMessage = false;
                    if (DiagnosticUtility.ShouldTraceWarning)
                    {
                        DiagnosticUtility.ExceptionUtility.TraceHandledException(exception2, TraceEventType.Warning);
                    }
                }
            }
            return null;
        }

        internal static bool ApplicationDomainHosted
        {
            get
            {
                if (!didAssemblyCheck)
                {
                    Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
                    for (int i = 0; i < assemblies.Length; i++)
                    {
                        if (string.Compare(assemblies[i].FullName, 0, "System.Web,", 0, "System.Web,".Length, StringComparison.OrdinalIgnoreCase) == 0)
                        {
                            isApplicationDomainHosted = IsApplicationDomainHosted();
                            break;
                        }
                    }
                    didAssemblyCheck = true;
                }
                return isApplicationDomainHosted;
            }
        }

        public static bool AspNetCompatibilityEnabled
        {
            get
            {
                if (!IsHosted)
                {
                    return false;
                }
                return IsAspNetCompatibilityEnabled();
            }
        }

        internal static string CurrentVirtualPath =>
            hostingManager.CurrentVirtualPath;

        internal static bool IsHosted =>
            isHosted;

        internal static bool IsRecycling =>
            hostingManager.IsRecycling;

        internal static bool IsSimpleApplicationHost =>
            isSimpleApplicationHost;

        public static bool MultipleSiteBindingsEnabled
        {
            get
            {
                if (!IsHosted)
                {
                    return false;
                }
                return IsMultipleSiteBindingsEnabled();
            }
        }

        internal static Uri[] PrefixFilters
        {
            get
            {
                if (!IsHosted)
                {
                    return null;
                }
                return GetBaseAddressPrefixFilters();
            }
        }

        private static object ThisLock =>
            syncRoot;

        private class BuildProviderInfo
        {
            [SecurityCritical]
            private System.Web.Configuration.BuildProvider buildProvider;
            private bool initialized;
            private bool isSupported;
            private object thisLock = new object();

            [SecurityTreatAsSafe, SecurityCritical]
            public BuildProviderInfo(System.Web.Configuration.BuildProvider buildProvider)
            {
                this.buildProvider = buildProvider;
            }

            [SecurityTreatAsSafe, SecurityCritical]
            private void ClearBuildProvider()
            {
                this.buildProvider = null;
            }

            private void EnsureInitialized()
            {
                if (!this.initialized)
                {
                    lock (this.thisLock)
                    {
                        if (!this.initialized)
                        {
                            Type attrProvider = Type.GetType(this.BuildProviderType, false);
                            if (attrProvider == null)
                            {
                                Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
                                for (int i = 0; i < assemblies.Length; i++)
                                {
                                    attrProvider = assemblies[i].GetType(this.BuildProviderType, false);
                                    if (attrProvider != null)
                                    {
                                        break;
                                    }
                                }
                            }
                            if ((attrProvider != null) && (ServiceReflector.GetCustomAttributes(attrProvider, typeof(ServiceActivationBuildProviderAttribute), true).Length > 0))
                            {
                                this.isSupported = true;
                            }
                            this.ClearBuildProvider();
                            this.initialized = true;
                        }
                    }
                }
            }

            private string BuildProviderType =>
                this.buildProvider.Type;

            public bool IsSupported
            {
                get
                {
                    this.EnsureInitialized();
                    return this.isSupported;
                }
            }
        }

        private class HostingManager : IRegisteredObject
        {
            private ManualResetEvent allRequestDoneInStop = new ManualResetEvent(false);
            private bool aspNetCompatibilityEnabled;
            private Uri[] baseAddressPrefixFilters;
            private static bool canDebugPrint = true;
            [ThreadStatic]
            private string currentVirtualPath;
            private IDictionary<string, ServiceHostingEnvironment.ServiceActivationInfo> directory = new Dictionary<string, ServiceHostingEnvironment.ServiceActivationInfo>(0x10, StringComparer.OrdinalIgnoreCase);
            private readonly ExtensionHelper extensions;
            private bool isRecycling;
            private bool isStopStarted;
            private bool isUnregistered;
            [SecurityCritical]
            private int minFreeMemoryPercentageToActivateService;
            private bool multipleSiteBindingsEnabled;
            private static object syncRoot = new object();

            internal HostingManager()
            {
                try
                {
                    this.RegisterObject();
                    this.LoadConfigParameters();
                    this.extensions = new ExtensionHelper();
                }
                finally
                {
                    if (this.extensions == null)
                    {
                        this.UnregisterObject();
                    }
                }
            }

            private void Abort()
            {
                this.allRequestDoneInStop.Set();
                List<KeyValuePair<string, ServiceHostingEnvironment.ServiceActivationInfo>> list = null;
                lock (ThisLock)
                {
                    this.isRecycling = true;
                    if (this.UnregisterObject())
                    {
                        return;
                    }
                    list = new List<KeyValuePair<string, ServiceHostingEnvironment.ServiceActivationInfo>>(this.directory);
                }
                for (int i = 0; i < list.Count; i++)
                {
                    KeyValuePair<string, ServiceHostingEnvironment.ServiceActivationInfo> pair = list[i];
                    if (pair.Value.Service != null)
                    {
                        try
                        {
                            KeyValuePair<string, ServiceHostingEnvironment.ServiceActivationInfo> pair2 = list[i];
                            pair2.Value.Service.Abort();
                        }
                        catch (Exception exception)
                        {
                            if (!DiagnosticUtility.IsFatal(exception))
                            {
                                KeyValuePair<string, ServiceHostingEnvironment.ServiceActivationInfo> pair3 = list[i];
                                this.LogServiceCloseError(pair3.Key, exception);
                            }
                            throw;
                        }
                    }
                    KeyValuePair<string, ServiceHostingEnvironment.ServiceActivationInfo> pair4 = list[i];
                    this.RemoveCachedService(pair4.Key);
                }
            }

            private ServiceHostBase ActivateService(string normalizedVirtualPath)
            {
                ServiceHostBase base2 = this.CreateService(normalizedVirtualPath);
                base2.Closed += new EventHandler(this.OnServiceClosed);
                this.FailActivationIfRecyling(normalizedVirtualPath);
                try
                {
                    base2.Open();
                }
                finally
                {
                    if (base2.State != CommunicationState.Opened)
                    {
                        base2.Abort();
                    }
                }
                return base2;
            }

            [SecurityCritical, SecurityTreatAsSafe]
            private void CheckMemoryGates()
            {
                ServiceMemoryGates.Check(this.minFreeMemoryPercentageToActivateService);
            }

            private ServiceHostBase CreateService(string normalizedVirtualPath)
            {
                string compiledCustomString = this.GetCompiledCustomString(normalizedVirtualPath);
                if (string.IsNullOrEmpty(compiledCustomString))
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(System.ServiceModel.SR.GetString("Hosting_CompilationResultEmpty", new object[] { normalizedVirtualPath })));
                }
                string[] strArray = compiledCustomString.Split("|".ToCharArray());
                if (strArray.Length < 3)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(System.ServiceModel.SR.GetString("Hosting_CompilationResultInvalid", new object[] { normalizedVirtualPath })));
                }
                Uri[] baseAddresses = HostedTransportConfigurationManager.GetBaseAddresses(strArray[0]);
                Uri[] prefixFilters = ServiceHostingEnvironment.PrefixFilters;
                if ((!this.multipleSiteBindingsEnabled && (prefixFilters != null)) && (prefixFilters.Length > 0))
                {
                    baseAddresses = FilterBaseAddressList(baseAddresses, prefixFilters);
                }
                normalizedVirtualPath = VirtualPathUtility.ToAppRelative(strArray[0], HostingEnvironmentWrapper.ApplicationVirtualPath);
                this.currentVirtualPath = strArray[0].Substring(0, strArray[0].LastIndexOf('/'));
                if (this.currentVirtualPath.Length == 0)
                {
                    this.currentVirtualPath = "/";
                }
                ServiceHostBase base2 = null;
                ServiceHostFactoryBase base3 = null;
                if (string.IsNullOrEmpty(strArray[1]))
                {
                    base3 = new ServiceHostFactory();
                }
                else
                {
                    Type c = Type.GetType(strArray[1]);
                    if (!typeof(ServiceHostFactoryBase).IsAssignableFrom(c))
                    {
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(System.ServiceModel.SR.GetString("Hosting_IServiceHostNotImplemented", new object[] { strArray[1] })));
                    }
                    ConstructorInfo constructor = c.GetConstructor(new Type[0]);
                    if (constructor == null)
                    {
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(System.ServiceModel.SR.GetString("Hosting_NoDefaultCtor", new object[] { strArray[1] })));
                    }
                    base3 = (ServiceHostFactoryBase) constructor.Invoke(new object[0]);
                }
                if (base3 is ServiceHostFactory)
                {
                    for (int i = 3; i < strArray.Length; i++)
                    {
                        ((ServiceHostFactory) base3).AddAssemblyReference(strArray[i]);
                    }
                }
                base2 = base3.CreateServiceHost(strArray[2], baseAddresses);
                if (base2 == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(System.ServiceModel.SR.GetString("Hosting_ServiceHostBaseIsNull", new object[] { strArray[2] })));
                }
                VirtualPathExtension item = new VirtualPathExtension(normalizedVirtualPath);
                base2.Extensions.Add(item);
                if (base2.Description != null)
                {
                    base2.Description.Behaviors.Add(new ApplyHostConfigurationBehavior());
                    if (this.multipleSiteBindingsEnabled && (base2.Description.Behaviors.Find<UseRequestHeadersForMetadataAddressBehavior>() == null))
                    {
                        base2.Description.Behaviors.Add(new UseRequestHeadersForMetadataAddressBehavior());
                    }
                }
                return base2;
            }

            private void EndCloseService(IAsyncResult result)
            {
                KeyValuePair<string, ServiceHostingEnvironment.ServiceActivationInfo> asyncState = (KeyValuePair<string, ServiceHostingEnvironment.ServiceActivationInfo>) result.AsyncState;
                try
                {
                    asyncState.Value.Service.EndClose(result);
                }
                catch (Exception exception)
                {
                    if (DiagnosticUtility.IsFatal(exception))
                    {
                        throw;
                    }
                    this.LogServiceCloseError(asyncState.Key, exception);
                }
                this.RemoveCachedService(asyncState.Key);
            }

            internal void EnsureServiceAvailable(string normalizedVirtualPath)
            {
                ServiceHostingEnvironment.ServiceActivationInfo info = null;
                lock (ThisLock)
                {
                    if (this.directory.TryGetValue(normalizedVirtualPath, out info) && (info.Service != null))
                    {
                        return;
                    }
                    this.FailActivationIfRecyling(normalizedVirtualPath);
                    if (info == null)
                    {
                        if (!HostingEnvironment.VirtualPathProvider.FileExists(normalizedVirtualPath))
                        {
                            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new EndpointNotFoundException(System.ServiceModel.SR.GetString("Hosting_ServiceNotExist", new object[] { VirtualPathUtility.ToAbsolute(normalizedVirtualPath, HostingEnvironmentWrapper.ApplicationVirtualPath) })));
                        }
                        info = new ServiceHostingEnvironment.ServiceActivationInfo(normalizedVirtualPath);
                        this.directory.Add(normalizedVirtualPath, info);
                    }
                }
                ServiceHostBase base2 = null;
                lock (info)
                {
                    if (info.Service != null)
                    {
                        return;
                    }
                    this.FailActivationIfRecyling(normalizedVirtualPath);
                    try
                    {
                        this.CheckMemoryGates();
                        base2 = this.ActivateService(normalizedVirtualPath);
                        lock (ThisLock)
                        {
                            if (!this.IsRecycling)
                            {
                                info.Service = base2;
                            }
                        }
                        if (DiagnosticUtility.ShouldTraceInformation)
                        {
                            TraceUtility.TraceEvent(TraceEventType.Information, TraceCode.WebHostServiceActivated, new StringTraceRecord("VirtualPath", VirtualPathUtility.ToAbsolute(normalizedVirtualPath, HostingEnvironmentWrapper.ApplicationVirtualPath)), this, null);
                        }
                    }
                    catch (HttpCompileException exception)
                    {
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ServiceActivationException(System.ServiceModel.SR.GetString("Hosting_ServiceCannotBeActivated", new object[] { VirtualPathUtility.ToAbsolute(normalizedVirtualPath, HostingEnvironmentWrapper.ApplicationVirtualPath), exception.Message }), exception));
                    }
                    catch (ServiceActivationException)
                    {
                        throw;
                    }
                    catch (Exception exception2)
                    {
                        if (DiagnosticUtility.IsFatal(exception2))
                        {
                            throw;
                        }
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ServiceActivationException(System.ServiceModel.SR.GetString("Hosting_ServiceCannotBeActivated", new object[] { VirtualPathUtility.ToAbsolute(normalizedVirtualPath, HostingEnvironmentWrapper.ApplicationVirtualPath), exception2.Message }), exception2));
                    }
                    finally
                    {
                        this.currentVirtualPath = null;
                    }
                }
                if (info.Service == null)
                {
                    base2.Abort();
                }
                this.FailActivationIfRecyling(normalizedVirtualPath);
            }

            private void FailActivationIfRecyling(string normalizedVirtualPath)
            {
                if (this.IsRecycling)
                {
                    InvalidOperationException innerException = new InvalidOperationException(System.ServiceModel.SR.GetString("Hosting_EnvironmentShuttingDown", new object[] { normalizedVirtualPath, HostingEnvironmentWrapper.ApplicationVirtualPath }));
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ServiceActivationException(innerException.Message, innerException));
                }
            }

            private static Uri[] FilterBaseAddressList(Uri[] baseAddresses, Uri[] prefixFilters)
            {
                List<Uri> list = new List<Uri>();
                Dictionary<string, Uri> dictionary = new Dictionary<string, Uri>();
                foreach (Uri uri in prefixFilters)
                {
                    if (dictionary.ContainsKey(uri.Scheme))
                    {
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(System.ServiceModel.SR.GetString("BaseAddressDuplicateScheme", new object[] { uri.Scheme })));
                    }
                    dictionary.Add(uri.Scheme, uri);
                }
                foreach (Uri uri2 in baseAddresses)
                {
                    string scheme = uri2.Scheme;
                    if (dictionary.ContainsKey(scheme))
                    {
                        Uri uri3 = dictionary[scheme];
                        if ((uri2.Port == uri3.Port) && (string.Compare(uri2.Host, uri3.Host, StringComparison.OrdinalIgnoreCase) == 0))
                        {
                            list.Add(uri2);
                        }
                    }
                    else
                    {
                        list.Add(uri2);
                    }
                }
                return list.ToArray();
            }

            [SecurityTreatAsSafe, SecurityCritical]
            private string GetCompiledCustomString(string normalizedVirtualPath)
            {
                string compiledCustomString;
                try
                {
                    using (IDisposable disposable = null)
                    {
                        try
                        {
                        }
                        finally
                        {
                            disposable = HostingEnvironmentWrapper.UnsafeImpersonate();
                        }
                        compiledCustomString = BuildManager.GetCompiledCustomString(normalizedVirtualPath);
                    }
                }
                catch
                {
                    throw;
                }
                return compiledCustomString;
            }

            internal bool GetExtensionSupported(string extension) => 
                this.extensions.GetExtensionSupported(extension);

            [SecurityCritical, SecurityTreatAsSafe]
            private void LoadConfigParameters()
            {
                ServiceHostingEnvironmentSection section = ServiceHostingEnvironmentSection.UnsafeGetSection();
                this.aspNetCompatibilityEnabled = section.AspNetCompatibilityEnabled;
                this.multipleSiteBindingsEnabled = section.MultipleSiteBindingsEnabled;
                this.minFreeMemoryPercentageToActivateService = section.MinFreeMemoryPercentageToActivateService;
                List<Uri> list = new List<Uri>();
                foreach (BaseAddressPrefixFilterElement element in section.BaseAddressPrefixFilters)
                {
                    list.Add(element.Prefix);
                }
                this.baseAddressPrefixFilters = list.ToArray();
            }

            private void LogServiceCloseError(string virtualPath, Exception exception)
            {
                if (DiagnosticUtility.ShouldTraceError)
                {
                    TraceUtility.TraceEvent(TraceEventType.Error, TraceCode.WebHostServiceCloseFailed, new StringTraceRecord("VirtualPath", VirtualPathUtility.ToAbsolute(virtualPath, HostingEnvironmentWrapper.ApplicationVirtualPath)), this, exception);
                }
            }

            internal void NotifyAllRequestDone()
            {
                if (this.isStopStarted)
                {
                    this.allRequestDoneInStop.Set();
                }
            }

            private void OnCloseService(IAsyncResult result)
            {
                if (!result.CompletedSynchronously)
                {
                    this.EndCloseService(result);
                }
            }

            private void OnServiceClosed(object sender, EventArgs e)
            {
                lock (ThisLock)
                {
                    if (!this.isRecycling)
                    {
                        ServiceHostBase base2 = (ServiceHostBase) sender;
                        string key = null;
                        foreach (string str2 in this.directory.Keys)
                        {
                            if (this.directory[str2].Service == base2)
                            {
                                key = str2;
                                break;
                            }
                        }
                        if (key != null)
                        {
                            this.directory.Remove(key);
                        }
                    }
                }
            }

            [SecurityCritical, SecurityTreatAsSafe]
            private void RegisterObject()
            {
                HostingEnvironmentWrapper.UnsafeRegisterObject(this);
            }

            private void RemoveCachedService(string path)
            {
                lock (ThisLock)
                {
                    this.directory.Remove(path);
                    this.UnregisterObject();
                }
            }

            public void Stop(bool immediate)
            {
                if (!immediate)
                {
                    IOThreadScheduler.ScheduleCallback(new WaitCallback(this.WaitAndCloseCallback), this);
                }
                else
                {
                    this.Abort();
                }
            }

            [Conditional("DEBUG")]
            private static void TryDebugPrint(string message)
            {
                if (canDebugPrint)
                {
                }
            }

            [SecurityTreatAsSafe, SecurityCritical]
            private bool UnregisterObject()
            {
                if (this.directory.Count != 0)
                {
                    return false;
                }
                if (!this.isUnregistered)
                {
                    this.isUnregistered = true;
                    HostingEnvironmentWrapper.UnsafeUnregisterObject(this);
                }
                return true;
            }

            private void WaitAndCloseCallback(object obj)
            {
                this.isStopStarted = true;
                if (ServiceHostingEnvironment.requestCount != 0L)
                {
                    this.allRequestDoneInStop.WaitOne();
                }
                List<KeyValuePair<string, ServiceHostingEnvironment.ServiceActivationInfo>> list = null;
                lock (ThisLock)
                {
                    if (this.UnregisterObject())
                    {
                        return;
                    }
                    list = new List<KeyValuePair<string, ServiceHostingEnvironment.ServiceActivationInfo>>(this.directory);
                }
                AsyncCallback callback = null;
                for (int i = 0; i < list.Count; i++)
                {
                    KeyValuePair<string, ServiceHostingEnvironment.ServiceActivationInfo> pair = list[i];
                    if (pair.Value.Service != null)
                    {
                        if (callback == null)
                        {
                            callback = DiagnosticUtility.ThunkAsyncCallback(new AsyncCallback(this.OnCloseService));
                        }
                        IAsyncResult result = null;
                        try
                        {
                            KeyValuePair<string, ServiceHostingEnvironment.ServiceActivationInfo> pair2 = list[i];
                            result = pair2.Value.Service.BeginClose(TimeSpan.MaxValue, callback, list[i]);
                        }
                        catch (Exception exception)
                        {
                            if (!DiagnosticUtility.IsFatal(exception))
                            {
                                KeyValuePair<string, ServiceHostingEnvironment.ServiceActivationInfo> pair3 = list[i];
                                this.LogServiceCloseError(pair3.Key, exception);
                            }
                            if (!(exception is CommunicationException))
                            {
                                throw;
                            }
                            KeyValuePair<string, ServiceHostingEnvironment.ServiceActivationInfo> pair4 = list[i];
                            this.RemoveCachedService(pair4.Key);
                        }
                        if ((result != null) && result.CompletedSynchronously)
                        {
                            this.EndCloseService(result);
                        }
                    }
                    else
                    {
                        KeyValuePair<string, ServiceHostingEnvironment.ServiceActivationInfo> pair5 = list[i];
                        this.RemoveCachedService(pair5.Key);
                    }
                }
            }

            internal bool AspNetCompatibilityEnabled =>
                this.aspNetCompatibilityEnabled;

            internal Uri[] BaseAddressPrefixFilters =>
                this.baseAddressPrefixFilters;

            internal string CurrentVirtualPath =>
                this.currentVirtualPath;

            internal bool IsRecycling =>
                this.isRecycling;

            internal bool MultipleSiteBindingsEnabled =>
                this.multipleSiteBindingsEnabled;

            internal static object ThisLock =>
                syncRoot;

            private class ExtensionHelper
            {
                private readonly IDictionary<string, ServiceHostingEnvironment.BuildProviderInfo> buildProviders = new Dictionary<string, ServiceHostingEnvironment.BuildProviderInfo>(8, StringComparer.OrdinalIgnoreCase);

                [SecurityTreatAsSafe, SecurityCritical]
                public ExtensionHelper()
                {
                    CompilationSection section = (CompilationSection) ConfigurationHelpers.UnsafeGetSectionFromWebConfigurationManager("system.web/compilation");
                    foreach (System.Web.Configuration.BuildProvider provider in section.BuildProviders)
                    {
                        this.buildProviders.Add(provider.Extension, new ServiceHostingEnvironment.BuildProviderInfo(provider));
                    }
                }

                public bool GetExtensionSupported(string extension)
                {
                    ServiceHostingEnvironment.BuildProviderInfo info;
                    if (!this.buildProviders.TryGetValue(extension, out info))
                    {
                        return false;
                    }
                    return info.IsSupported;
                }
            }
        }

        private class ServiceActivationInfo
        {
            private ServiceHostBase service;
            private string virtualPath;

            public ServiceActivationInfo(string virtualPath)
            {
                this.virtualPath = virtualPath;
            }

            public ServiceHostBase Service
            {
                get => 
                    this.service;
                set
                {
                    this.service = value;
                }
            }
        }
    }
}

