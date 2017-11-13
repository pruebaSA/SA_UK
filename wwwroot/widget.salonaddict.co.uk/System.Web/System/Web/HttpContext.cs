namespace System.Web
{
    using System;
    using System.Collections;
    using System.ComponentModel;
    using System.Configuration;
    using System.Diagnostics;
    using System.Globalization;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Runtime.Remoting.Messaging;
    using System.Security.Permissions;
    using System.Security.Principal;
    using System.Threading;
    using System.Web.Caching;
    using System.Web.Compilation;
    using System.Web.Configuration;
    using System.Web.Hosting;
    using System.Web.Profile;
    using System.Web.Security;
    using System.Web.SessionState;
    using System.Web.UI;
    using System.Web.Util;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public sealed class HttpContext : IServiceProvider
    {
        private HttpApplication _appInstance;
        private IHttpAsyncHandler _asyncAppHandler;
        private VirtualPath _configurationPath;
        private CachedPathData _configurationPathData;
        private CookielessHelperClass _CookielessHelper;
        private IntPtr _ctxPtr;
        private IHttpHandler _currentHandler;
        private CultureInfo _dynamicCulture;
        private CultureInfo _dynamicUICulture;
        private bool _errorCleared;
        private ArrayList _errors;
        private CachedPathData _filePathData;
        private bool _finishPipelineRequestCalled;
        private bool _firstNotificationInitCalled;
        private IHttpHandler _handler;
        private Stack _handlerStack;
        private bool _impersonationEnabled;
        private bool _isAppInitialized;
        private bool _isIntegratedPipeline;
        private Hashtable _items;
        private System.Web.NotificationContext _notificationContext;
        private IntPtr _pManagedPrincipal;
        private bool _preventPostback;
        internal ProfileBase _Profile;
        internal bool _ProfileDelayLoad;
        private IHttpHandler _remapHandler;
        private HttpRequest _request;
        private HttpResponse _response;
        private GCHandle _root;
        private bool _runtimeErrorReported;
        private HttpServerUtility _server;
        private int _serverExecuteDepth;
        private SessionStateModule _sessionStateModule;
        internal bool _skipAuthorization;
        private string _sqlDependencyCookie;
        private AspNetSynchronizationContext _syncContext;
        private Exception _tempError;
        private System.Web.UI.TemplateControl _templateControl;
        private Thread _thread;
        private TimeSpan _timeout;
        private DoubleLink _timeoutLink;
        private bool _timeoutSet;
        private DateTime _timeoutStartTime;
        private int _timeoutState;
        private TraceContext _topTraceContext;
        private Stack _traceContextStack;
        private IPrincipal _user;
        private DateTime _utcTimestamp;
        private HttpWorkerRequest _wr;
        private const int FLAG_CHANGE_IN_REQUEST_HEADERS = 2;
        private const int FLAG_CHANGE_IN_RESPONSE_HEADERS = 4;
        private const int FLAG_CHANGE_IN_RESPONSE_STATUS = 0x80;
        private const int FLAG_CHANGE_IN_SERVER_VARIABLES = 1;
        private const int FLAG_CHANGE_IN_USER_OBJECT = 8;
        internal const int FLAG_ETW_PROVIDER_ENABLED = 0x40;
        private const int FLAG_NONE = 0;
        private const int FLAG_RESPONSE_HEADERS_SENT = 0x20;
        private const int FLAG_SEND_RESPONSE_HEADERS = 0x10;
        internal bool HideRequestResponse;
        internal bool InAspCompatMode;
        internal volatile HttpApplication.ThreadContext IndicateCompletionContext;
        internal volatile bool InIndicateCompletion;
        internal bool ReadOnlySessionState;
        internal bool RequiresSessionState;
        internal static readonly Assembly SystemWebAssembly = typeof(HttpContext).Assembly;

        public HttpContext(HttpWorkerRequest wr)
        {
            this._timeoutStartTime = DateTime.MinValue;
            this._wr = wr;
            this.Init(new HttpRequest(wr, this), new HttpResponse(wr, this));
            this._response.InitResponseWriter();
        }

        public HttpContext(HttpRequest request, HttpResponse response)
        {
            this._timeoutStartTime = DateTime.MinValue;
            this.Init(request, response);
            request.Context = this;
            response.Context = this;
        }

        internal HttpContext(HttpWorkerRequest wr, bool initResponseWriter)
        {
            this._timeoutStartTime = DateTime.MinValue;
            this._wr = wr;
            this.Init(new HttpRequest(wr, this), new HttpResponse(wr, this));
            if (initResponseWriter)
            {
                this._response.InitResponseWriter();
            }
            PerfCounters.IncrementCounter(AppPerfCounter.REQUESTS_EXECUTING);
        }

        private void AbortConnection()
        {
            IIS7WorkerRequest request = this._wr as IIS7WorkerRequest;
            if (request != null)
            {
                request.AbortConnection();
            }
            else
            {
                this._wr.CloseConnection();
            }
        }

        internal void AddDelayedHttpSessionState(SessionStateModule module)
        {
            if (this._sessionStateModule != null)
            {
                throw new HttpException(System.Web.SR.GetString("Cant_have_multiple_session_module"));
            }
            this._sessionStateModule = module;
        }

        public void AddError(Exception errorInfo)
        {
            if (this._errors == null)
            {
                this._errors = new ArrayList();
            }
            this._errors.Add(errorInfo);
            if (this._isIntegratedPipeline && (this._notificationContext != null))
            {
                this._notificationContext.Error = errorInfo;
            }
        }

        internal void BeginCancellablePeriod()
        {
            if (this._timeoutStartTime == DateTime.MinValue)
            {
                this.SetStartTime();
            }
            this._timeoutState = 1;
        }

        internal int CallISAPI(System.Web.UnsafeNativeMethods.CallISAPIFunc iFunction, byte[] bufIn, byte[] bufOut)
        {
            if ((this._wr == null) || !(this._wr is ISAPIWorkerRequest))
            {
                throw new HttpException(System.Web.SR.GetString("Cannot_call_ISAPI_functions"));
            }
            return ((ISAPIWorkerRequest) this._wr).CallISAPI(iFunction, bufIn, bufOut);
        }

        public void ClearError()
        {
            if (this._tempError != null)
            {
                this._tempError = null;
            }
            else
            {
                this._errorCleared = true;
            }
            if (this._isIntegratedPipeline && (this._notificationContext != null))
            {
                this._notificationContext.Error = null;
            }
        }

        internal void ClearReferences()
        {
            this._appInstance = null;
            this._handler = null;
            this._handlerStack = null;
            this._currentHandler = null;
            if (this._isIntegratedPipeline)
            {
                this._items = null;
                this._syncContext = null;
            }
        }

        internal CultureInfo CultureFromConfig(string configString, bool requireSpecific)
        {
            if (System.Web.Util.StringUtil.EqualsIgnoreCase(configString, HttpApplication.AutoCulture))
            {
                string name = this.UserLanguageFromContext();
                if (name != null)
                {
                    try
                    {
                        if (requireSpecific)
                        {
                            return HttpServerUtility.CreateReadOnlySpecificCultureInfo(name);
                        }
                        return HttpServerUtility.CreateReadOnlyCultureInfo(name);
                    }
                    catch
                    {
                        return null;
                    }
                }
                return null;
            }
            if (System.Web.Util.StringUtil.StringStartsWithIgnoreCase(configString, "auto:"))
            {
                string str2 = this.UserLanguageFromContext();
                if (str2 != null)
                {
                    try
                    {
                        if (requireSpecific)
                        {
                            return HttpServerUtility.CreateReadOnlySpecificCultureInfo(str2);
                        }
                        return HttpServerUtility.CreateReadOnlyCultureInfo(str2);
                    }
                    catch
                    {
                        if (requireSpecific)
                        {
                            return HttpServerUtility.CreateReadOnlySpecificCultureInfo(HttpApplication.GetFallbackCulture(configString));
                        }
                        return HttpServerUtility.CreateReadOnlyCultureInfo(HttpApplication.GetFallbackCulture(configString));
                    }
                }
                if (requireSpecific)
                {
                    return HttpServerUtility.CreateReadOnlySpecificCultureInfo(configString.Substring(5));
                }
                return HttpServerUtility.CreateReadOnlyCultureInfo(configString.Substring(5));
            }
            if (requireSpecific)
            {
                return HttpServerUtility.CreateReadOnlySpecificCultureInfo(configString);
            }
            return HttpServerUtility.CreateReadOnlyCultureInfo(configString);
        }

        internal void DisableNotifications(RequestNotification notifications, RequestNotification postNotifications)
        {
            IIS7WorkerRequest request = this._wr as IIS7WorkerRequest;
            if (request != null)
            {
                request.DisableNotifications(notifications, postNotifications);
            }
        }

        internal void DisposePrincipal()
        {
            if (((this._pManagedPrincipal == IntPtr.Zero) && (this._user != null)) && (this._user != WindowsAuthenticationModule.AnonymousPrincipal))
            {
                WindowsIdentity identity = this._user.Identity as WindowsIdentity;
                if (identity != null)
                {
                    this._user = null;
                    identity.Dispose();
                }
            }
        }

        internal void EndCancellablePeriod()
        {
            Interlocked.CompareExchange(ref this._timeoutState, 0, 1);
        }

        internal void EnsureTimeout()
        {
            if (!this._timeoutSet)
            {
                int totalSeconds = (int) RuntimeConfig.GetConfig(this).HttpRuntime.ExecutionTimeout.TotalSeconds;
                this._timeout = new TimeSpan(0, 0, totalSeconds);
                this._timeoutSet = true;
            }
        }

        internal void FinishPipelineRequest()
        {
            if (!this._finishPipelineRequestCalled)
            {
                this._finishPipelineRequestCalled = true;
                HttpRuntime.FinishPipelineRequest(this);
            }
        }

        internal void FinishRequestForCachedPathData(int statusCode)
        {
            if ((this._filePathData != null) && !this._filePathData.CompletedFirstRequest)
            {
                if ((400 <= statusCode) && (statusCode < 500))
                {
                    CachedPathData.RemoveBadPathData(this._filePathData);
                }
                else
                {
                    CachedPathData.MarkCompleted(this._filePathData);
                }
            }
        }

        internal void FirstNotificationInit()
        {
            if (!this._firstNotificationInitCalled)
            {
                this._firstNotificationInitCalled = true;
                this.ValidatePath();
            }
        }

        [Obsolete("The recommended alternative is System.Web.Configuration.WebConfigurationManager.GetWebApplicationSection in System.Web.dll. http://go.microsoft.com/fwlink/?linkid=14202")]
        public static object GetAppConfig(string name) => 
            WebConfigurationManager.GetWebApplicationSection(name);

        [Obsolete("The recommended alternative is System.Web.HttpContext.GetSection in System.Web.dll. http://go.microsoft.com/fwlink/?linkid=14202")]
        public object GetConfig(string name) => 
            this.GetSection(name);

        internal CachedPathData GetConfigurationPathData()
        {
            if (this._configurationPath == null)
            {
                return this.GetFilePathData();
            }
            if (this._configurationPathData == null)
            {
                this._configurationPathData = CachedPathData.GetVirtualPathData(this._configurationPath, true);
            }
            return this._configurationPathData;
        }

        internal CachedPathData GetFilePathData()
        {
            if (this._filePathData == null)
            {
                this._filePathData = CachedPathData.GetVirtualPathData(this._request.FilePathObject, false);
            }
            return this._filePathData;
        }

        public static object GetGlobalResourceObject(string classKey, string resourceKey) => 
            GetGlobalResourceObject(classKey, resourceKey, null);

        public static object GetGlobalResourceObject(string classKey, string resourceKey, CultureInfo culture) => 
            ResourceExpressionBuilder.GetGlobalResourceObject(classKey, resourceKey, null, null, culture);

        public static object GetLocalResourceObject(string virtualPath, string resourceKey) => 
            GetLocalResourceObject(virtualPath, resourceKey, null);

        public static object GetLocalResourceObject(string virtualPath, string resourceKey, CultureInfo culture) => 
            ResourceExpressionBuilder.GetResourceObject(ResourceExpressionBuilder.GetLocalResourceProvider(VirtualPath.Create(virtualPath)), resourceKey, culture);

        internal CachedPathData GetPathData(VirtualPath path)
        {
            if (path != null)
            {
                if (path.Equals(this._request.FilePathObject))
                {
                    return this.GetFilePathData();
                }
                if ((this._configurationPath != null) && path.Equals(this._configurationPath))
                {
                    return this.GetConfigurationPathData();
                }
            }
            return CachedPathData.GetVirtualPathData(path, false);
        }

        internal RuntimeConfig GetRuntimeConfig() => 
            this.GetConfigurationPathData().RuntimeConfig;

        internal RuntimeConfig GetRuntimeConfig(VirtualPath path) => 
            this.GetPathData(path).RuntimeConfig;

        public object GetSection(string sectionName)
        {
            if (HttpConfigurationSystem.UseHttpConfigurationSystem)
            {
                return this.GetConfigurationPathData().ConfigRecord.GetSection(sectionName);
            }
            return ConfigurationManager.GetSection(sectionName);
        }

        private void Init(HttpRequest request, HttpResponse response)
        {
            this._request = request;
            this._response = response;
            this._utcTimestamp = DateTime.UtcNow;
            if (this._wr is IIS7WorkerRequest)
            {
                this._isIntegratedPipeline = true;
            }
            if (!(this._wr is StateHttpWorkerRequest))
            {
                this.CookielessHelper.RemoveCookielessValuesFromPath();
            }
            Profiler profile = HttpRuntime.Profile;
            if ((profile != null) && profile.IsEnabled)
            {
                this._topTraceContext = new TraceContext(this);
            }
        }

        internal AspNetSynchronizationContext InstallNewAspNetSynchronizationContext()
        {
            AspNetSynchronizationContext context = this._syncContext;
            if ((context != null) && (context == AsyncOperationManager.SynchronizationContext))
            {
                this._syncContext = new AspNetSynchronizationContext(this.ApplicationInstance);
                AsyncOperationManager.SynchronizationContext = this._syncContext;
                return context;
            }
            return null;
        }

        internal void InvokeCancellableCallback(WaitCallback callback, object state)
        {
            if (this.IsInCancellablePeriod)
            {
                callback(state);
            }
            else
            {
                try
                {
                    this.BeginCancellablePeriod();
                    try
                    {
                        callback(state);
                    }
                    finally
                    {
                        this.EndCancellablePeriod();
                    }
                    this.WaitForExceptionIfCancelled();
                }
                catch (ThreadAbortException exception)
                {
                    if (((exception.ExceptionState != null) && (exception.ExceptionState is HttpApplication.CancelModuleException)) && ((HttpApplication.CancelModuleException) exception.ExceptionState).Timeout)
                    {
                        Thread.ResetAbort();
                        PerfCounters.IncrementCounter(AppPerfCounter.REQUESTS_TIMED_OUT);
                        throw new HttpException(System.Web.SR.GetString("Request_timed_out"), null, 0xbb9);
                    }
                }
            }
        }

        internal Thread MustTimeout(DateTime utcNow)
        {
            if ((this._timeoutState == 1) && (TimeSpan.Compare(utcNow.Subtract(this._timeoutStartTime), this.Timeout) >= 0))
            {
                try
                {
                    if (CompilationUtil.IsDebuggingEnabled(this) || Debugger.IsAttached)
                    {
                        return null;
                    }
                }
                catch
                {
                    return null;
                }
                if (Interlocked.CompareExchange(ref this._timeoutState, -1, 1) == 1)
                {
                    if (this._wr.IsInReadEntitySync)
                    {
                        this.AbortConnection();
                    }
                    return this._thread;
                }
            }
            return null;
        }

        internal bool NeedToInitializeApp()
        {
            bool flag = !this._isAppInitialized;
            if (flag)
            {
                this._isAppInitialized = true;
            }
            return flag;
        }

        internal void PopTraceContext()
        {
            this._topTraceContext = (TraceContext) this._traceContextStack.Pop();
        }

        internal void PushTraceContext()
        {
            if (this._traceContextStack == null)
            {
                this._traceContextStack = new Stack();
            }
            this._traceContextStack.Push(this._topTraceContext);
            if (this._topTraceContext != null)
            {
                TraceContext tc = new TraceContext(this);
                this._topTraceContext.CopySettingsTo(tc);
                this._topTraceContext = tc;
            }
        }

        public void RemapHandler(IHttpHandler handler)
        {
            IIS7WorkerRequest request = this._wr as IIS7WorkerRequest;
            if (request != null)
            {
                if (this._notificationContext.CurrentNotification >= RequestNotification.MapRequestHandler)
                {
                    throw new InvalidOperationException(System.Web.SR.GetString("Invoke_before_pipeline_event", new object[] { "HttpContext.RemapHandler", "HttpApplication.MapRequestHandler" }));
                }
                string handlerType = null;
                string handlerName = null;
                if (handler != null)
                {
                    Type type = handler.GetType();
                    handlerType = type.AssemblyQualifiedName;
                    handlerName = type.FullName;
                }
                request.SetRemapHandler(handlerType, handlerName);
            }
            this._remapHandler = handler;
        }

        internal void RemoveDelayedHttpSessionState()
        {
            this._sessionStateModule = null;
        }

        internal void RemoveSqlDependencyCookie()
        {
            if (this._sqlDependencyCookie != null)
            {
                CallContext.LogicalSetData("MS.SqlDependencyCookie", null);
            }
        }

        internal void ReportRuntimeErrorIfExists(ref RequestNotificationStatus status)
        {
            Exception error = this.Error;
            if ((error == null) || this._runtimeErrorReported)
            {
                return;
            }
            if ((this._notificationContext != null) && (this.CurrentModuleIndex == -1))
            {
                try
                {
                    IIS7WorkerRequest request = this._wr as IIS7WorkerRequest;
                    if (((this.Request.QueryString["aspxerrorpath"] != null) && (request != null)) && (string.IsNullOrEmpty(request.GetManagedHandlerType()) && (request.GetCurrentModuleName() == "AspNetInitializationExceptionModule")))
                    {
                        status = RequestNotificationStatus.Continue;
                        return;
                    }
                }
                catch
                {
                }
            }
            this._runtimeErrorReported = true;
            if (HttpRuntime.AppOfflineMessage != null)
            {
                try
                {
                    this.Response.StatusCode = 0x194;
                    this.Response.TrySkipIisCustomErrors = true;
                    this.Response.OutputStream.Write(HttpRuntime.AppOfflineMessage, 0, HttpRuntime.AppOfflineMessage.Length);
                    goto Label_0110;
                }
                catch
                {
                    goto Label_0110;
                }
            }
            using (new HttpContextWrapper(this))
            {
                using (new ApplicationImpersonationContext())
                {
                    try
                    {
                        try
                        {
                            this.Response.ReportRuntimeError(error, true, false);
                        }
                        catch (Exception exception2)
                        {
                            this.Response.ReportRuntimeError(exception2, false, false);
                        }
                    }
                    catch (Exception)
                    {
                    }
                }
            }
        Label_0110:
            status = RequestNotificationStatus.FinishRequest;
        }

        internal bool RequestRequiresAuthorization()
        {
            if (!this.User.Identity.IsAuthenticated)
            {
                return false;
            }
            if (!FileAuthorizationModule.RequestRequiresAuthorization(this))
            {
                return UrlAuthorizationModule.RequestRequiresAuthorization(this);
            }
            return true;
        }

        internal void ResetSqlDependencyCookie()
        {
            if (this._sqlDependencyCookie != null)
            {
                CallContext.LogicalSetData("MS.SqlDependencyCookie", this._sqlDependencyCookie);
            }
        }

        internal void RestoreCurrentHandler()
        {
            this._currentHandler = (IHttpHandler) this._handlerStack.Pop();
        }

        internal void RestoreSavedAspNetSynchronizationContext(AspNetSynchronizationContext syncContext)
        {
            AsyncOperationManager.SynchronizationContext = syncContext;
            this._syncContext = syncContext;
        }

        public void RewritePath(string path)
        {
            this.RewritePath(path, true);
        }

        public void RewritePath(string path, bool rebaseClientPath)
        {
            if (path == null)
            {
                throw new ArgumentNullException("path");
            }
            string newQueryString = null;
            int index = path.IndexOf('?');
            if (index >= 0)
            {
                newQueryString = (index < (path.Length - 1)) ? path.Substring(index + 1) : string.Empty;
                path = path.Substring(0, index);
            }
            VirtualPath relativePath = VirtualPath.Create(path);
            relativePath = this.Request.FilePathObject.Combine(relativePath);
            relativePath.FailIfNotWithinAppRoot();
            this.ConfigurationPath = null;
            this.Request.InternalRewritePath(relativePath, newQueryString, rebaseClientPath);
        }

        public void RewritePath(string filePath, string pathInfo, string queryString)
        {
            this.RewritePath(VirtualPath.CreateAllowNull(filePath), VirtualPath.CreateAllowNull(pathInfo), queryString, false);
        }

        public void RewritePath(string filePath, string pathInfo, string queryString, bool setClientFilePath)
        {
            this.RewritePath(VirtualPath.CreateAllowNull(filePath), VirtualPath.CreateAllowNull(pathInfo), queryString, setClientFilePath);
        }

        internal void RewritePath(VirtualPath filePath, VirtualPath pathInfo, string queryString, bool setClientFilePath)
        {
            if (filePath == null)
            {
                throw new ArgumentNullException("filePath");
            }
            filePath = this.Request.FilePathObject.Combine(filePath);
            filePath.FailIfNotWithinAppRoot();
            this.ConfigurationPath = null;
            this.Request.InternalRewritePath(filePath, pathInfo, queryString, setClientFilePath);
        }

        internal void Root()
        {
            this._root = GCHandle.Alloc(this);
            this._ctxPtr = GCHandle.ToIntPtr(this._root);
        }

        internal void SendEmptyResponse()
        {
            if ((this._wr != null) && (this._wr is ISAPIWorkerRequest))
            {
                ((ISAPIWorkerRequest) this._wr).SendEmptyResponse();
            }
        }

        internal void SetCurrentHandler(IHttpHandler newtHandler)
        {
            if (this._handlerStack == null)
            {
                this._handlerStack = new Stack();
            }
            this._handlerStack.Push(this.CurrentHandler);
            this._currentHandler = newtHandler;
        }

        internal void SetImpersonationEnabled()
        {
            IdentitySection identity = RuntimeConfig.GetConfig(this).Identity;
            this._impersonationEnabled = (identity != null) && identity.Impersonate;
        }

        internal void SetPrincipalNoDemand(IPrincipal principal)
        {
            this.SetPrincipalNoDemand(principal, true);
        }

        internal void SetPrincipalNoDemand(IPrincipal principal, bool needToSetNativePrincipal)
        {
            this._user = principal;
            if ((needToSetNativePrincipal && this._isIntegratedPipeline) && (this._notificationContext.CurrentNotification == RequestNotification.AuthenticateRequest))
            {
                IntPtr zero = IntPtr.Zero;
                try
                {
                    IIS7WorkerRequest request = this._wr as IIS7WorkerRequest;
                    if (principal != null)
                    {
                        GCHandle handle = GCHandle.Alloc(principal);
                        try
                        {
                            zero = GCHandle.ToIntPtr(handle);
                            request.SetPrincipal(principal, zero);
                            return;
                        }
                        catch
                        {
                            zero = IntPtr.Zero;
                            if (handle.IsAllocated)
                            {
                                handle.Free();
                            }
                            throw;
                        }
                    }
                    request.SetPrincipal(null, IntPtr.Zero);
                }
                finally
                {
                    if (this._pManagedPrincipal != IntPtr.Zero)
                    {
                        GCHandle handle2 = GCHandle.FromIntPtr(this._pManagedPrincipal);
                        if (handle2.IsAllocated)
                        {
                            handle2.Free();
                        }
                    }
                    this._pManagedPrincipal = zero;
                }
            }
        }

        internal void SetSkipAuthorizationNoDemand(bool value, bool managedOnly)
        {
            if ((HttpRuntime.UseIntegratedPipeline && !managedOnly) && (value != this._skipAuthorization))
            {
                this._request.SetSkipAuthorization(value);
            }
            this._skipAuthorization = value;
        }

        internal void SetStartTime()
        {
            this._timeoutStartTime = DateTime.UtcNow;
        }

        object IServiceProvider.GetService(Type service)
        {
            if (service == typeof(HttpWorkerRequest))
            {
                InternalSecurityPermissions.UnmanagedCode.Demand();
                return this._wr;
            }
            if (service == typeof(HttpRequest))
            {
                return this.Request;
            }
            if (service == typeof(HttpResponse))
            {
                return this.Response;
            }
            if (service == typeof(HttpApplication))
            {
                return this.ApplicationInstance;
            }
            if (service == typeof(HttpApplicationState))
            {
                return this.Application;
            }
            if (service == typeof(HttpSessionState))
            {
                return this.Session;
            }
            if (service == typeof(HttpServerUtility))
            {
                return this.Server;
            }
            return null;
        }

        internal void Unroot()
        {
            if (this._root.IsAllocated)
            {
                this._root.Free();
                this._ctxPtr = IntPtr.Zero;
            }
        }

        internal string UserLanguageFromContext()
        {
            if ((this.Request != null) && (this.Request.UserLanguages != null))
            {
                string str = this.Request.UserLanguages[0];
                if (str != null)
                {
                    int index = str.IndexOf(';');
                    if (index != -1)
                    {
                        return str.Substring(0, index);
                    }
                    return str;
                }
            }
            return null;
        }

        internal void ValidatePath()
        {
            string physicalPathInternal = this._request.PhysicalPathInternal;
            if (!System.Web.Util.StringUtil.EqualsIgnoreCase(this.GetConfigurationPathData().PhysicalPath, physicalPathInternal))
            {
                System.Web.Util.FileUtil.CheckSuspiciousPhysicalPath(physicalPathInternal);
            }
        }

        internal void WaitForExceptionIfCancelled()
        {
            while (this._timeoutState == -1)
            {
                Thread.Sleep(100);
            }
        }

        public Exception[] AllErrors
        {
            get
            {
                int count = (this._errors != null) ? this._errors.Count : 0;
                if (count == 0)
                {
                    return null;
                }
                Exception[] array = new Exception[count];
                this._errors.CopyTo(0, array, 0, count);
                return array;
            }
        }

        public HttpApplicationState Application =>
            HttpApplicationFactory.ApplicationState;

        public HttpApplication ApplicationInstance
        {
            get => 
                this._appInstance;
            set
            {
                if ((this._isIntegratedPipeline && (this._appInstance != null)) && (value != null))
                {
                    throw new InvalidOperationException(System.Web.SR.GetString("Application_instance_cannot_be_changed"));
                }
                this._appInstance = value;
            }
        }

        internal bool AreResponseHeadersSent =>
            ((this._notificationContext.CurrentNotificationFlags & 0x20) == 0x20);

        internal IHttpAsyncHandler AsyncAppHandler
        {
            get => 
                this._asyncAppHandler;
            set
            {
                this._asyncAppHandler = value;
            }
        }

        public System.Web.Caching.Cache Cache =>
            HttpRuntime.Cache;

        internal IntPtr ClientIdentityToken
        {
            get
            {
                if (this._wr != null)
                {
                    return this._wr.GetUserToken();
                }
                return IntPtr.Zero;
            }
        }

        internal VirtualPath ConfigurationPath
        {
            get
            {
                if (this._configurationPath == null)
                {
                    this._configurationPath = this._request.FilePathObject;
                }
                return this._configurationPath;
            }
            set
            {
                this._configurationPath = value;
                if (this._configurationPathData != null)
                {
                    if (!this._configurationPathData.CompletedFirstRequest)
                    {
                        CachedPathData.RemoveBadPathData(this._configurationPathData);
                    }
                    this._configurationPathData = null;
                }
                if (this._filePathData != null)
                {
                    if (!this._filePathData.CompletedFirstRequest)
                    {
                        CachedPathData.RemoveBadPathData(this._filePathData);
                    }
                    this._filePathData = null;
                }
            }
        }

        internal IntPtr ContextPtr =>
            this._ctxPtr;

        internal CookielessHelperClass CookielessHelper
        {
            get
            {
                if (this._CookielessHelper == null)
                {
                    this._CookielessHelper = new CookielessHelperClass(this);
                }
                return this._CookielessHelper;
            }
        }

        public static HttpContext Current
        {
            get => 
                (ContextBase.Current as HttpContext);
            set
            {
                ContextBase.Current = value;
            }
        }

        public IHttpHandler CurrentHandler
        {
            get
            {
                if (this._currentHandler == null)
                {
                    this._currentHandler = this._handler;
                }
                return this._currentHandler;
            }
        }

        internal int CurrentModuleEventIndex
        {
            get => 
                this._notificationContext.CurrentModuleEventIndex;
            set
            {
                this._notificationContext.CurrentModuleEventIndex = value;
            }
        }

        internal int CurrentModuleIndex
        {
            get => 
                this._notificationContext.CurrentModuleIndex;
            set
            {
                this._notificationContext.CurrentModuleIndex = value;
            }
        }

        public RequestNotification CurrentNotification
        {
            get
            {
                if (!HttpRuntime.UseIntegratedPipeline)
                {
                    throw new PlatformNotSupportedException(System.Web.SR.GetString("Requires_Iis_Integrated_Mode"));
                }
                return this._notificationContext.CurrentNotification;
            }
            internal set
            {
                if (!HttpRuntime.UseIntegratedPipeline)
                {
                    throw new PlatformNotSupportedException(System.Web.SR.GetString("Requires_Iis_Integrated_Mode"));
                }
                this._notificationContext.CurrentNotification = value;
            }
        }

        internal int CurrentNotificationFlags
        {
            get => 
                this._notificationContext.CurrentNotificationFlags;
            set
            {
                this._notificationContext.CurrentNotificationFlags = value;
            }
        }

        internal Thread CurrentThread
        {
            get => 
                this._thread;
            set
            {
                this._thread = value;
            }
        }

        internal CultureInfo DynamicCulture
        {
            get => 
                this._dynamicCulture;
            set
            {
                this._dynamicCulture = value;
            }
        }

        internal CultureInfo DynamicUICulture
        {
            get => 
                this._dynamicUICulture;
            set
            {
                this._dynamicUICulture = value;
            }
        }

        public Exception Error
        {
            get
            {
                if (this._tempError != null)
                {
                    return this._tempError;
                }
                if (((this._errors != null) && (this._errors.Count != 0)) && !this._errorCleared)
                {
                    return (Exception) this._errors[0];
                }
                return null;
            }
        }

        public IHttpHandler Handler
        {
            get => 
                this._handler;
            set
            {
                this._handler = value;
                this.RequiresSessionState = false;
                this.ReadOnlySessionState = false;
                this.InAspCompatMode = false;
                if (this._handler != null)
                {
                    if (this._handler is IRequiresSessionState)
                    {
                        this.RequiresSessionState = true;
                    }
                    if (this._handler is IReadOnlySessionState)
                    {
                        this.ReadOnlySessionState = true;
                    }
                    Page page = this._handler as Page;
                    if ((page != null) && page.IsInAspCompatMode)
                    {
                        this.InAspCompatMode = true;
                    }
                }
            }
        }

        internal IntPtr ImpersonationToken
        {
            get
            {
                IntPtr applicationIdentityToken = HostingEnvironment.ApplicationIdentityToken;
                IdentitySection identity = RuntimeConfig.GetConfig(this).Identity;
                if (identity != null)
                {
                    if (identity.Impersonate)
                    {
                        return ((identity.ImpersonateToken != IntPtr.Zero) ? identity.ImpersonateToken : this.ClientIdentityToken);
                    }
                    if (!HttpRuntime.IsOnUNCShareInternal)
                    {
                        applicationIdentityToken = IntPtr.Zero;
                    }
                }
                return applicationIdentityToken;
            }
        }

        internal bool IsChangeInRequestHeaders =>
            ((this._notificationContext.CurrentNotificationFlags & 2) == 2);

        internal bool IsChangeInResponseHeaders =>
            ((this._notificationContext.CurrentNotificationFlags & 4) == 4);

        internal bool IsChangeInResponseStatus =>
            ((this._notificationContext.CurrentNotificationFlags & 0x80) == 0x80);

        internal bool IsChangeInServerVars =>
            ((this._notificationContext.CurrentNotificationFlags & 1) == 1);

        internal bool IsChangeInUserPrincipal =>
            ((this._notificationContext.CurrentNotificationFlags & 8) == 8);

        internal bool IsClientImpersonationConfigured
        {
            get
            {
                try
                {
                    IdentitySection identity = RuntimeConfig.GetConfig(this).Identity;
                    return (((identity != null) && identity.Impersonate) && (identity.ImpersonateToken == IntPtr.Zero));
                }
                catch
                {
                    return false;
                }
            }
        }

        public bool IsCustomErrorEnabled =>
            CustomErrorsSection.GetSettings(this).CustomErrorsEnabled(this._request);

        public bool IsDebuggingEnabled
        {
            get
            {
                try
                {
                    return CompilationUtil.IsDebuggingEnabled(this);
                }
                catch
                {
                    return false;
                }
            }
        }

        internal bool IsInCancellablePeriod =>
            (this._timeoutState == 1);

        public bool IsPostNotification
        {
            get
            {
                if (!HttpRuntime.UseIntegratedPipeline)
                {
                    throw new PlatformNotSupportedException(System.Web.SR.GetString("Requires_Iis_Integrated_Mode"));
                }
                return this._notificationContext.IsPostNotification;
            }
            internal set
            {
                if (!HttpRuntime.UseIntegratedPipeline)
                {
                    throw new PlatformNotSupportedException(System.Web.SR.GetString("Requires_Iis_Integrated_Mode"));
                }
                this._notificationContext.IsPostNotification = value;
            }
        }

        internal bool IsSendResponseHeaders =>
            ((this._notificationContext.CurrentNotificationFlags & 0x10) == 0x10);

        public IDictionary Items
        {
            get
            {
                if (this._items == null)
                {
                    this._items = new Hashtable();
                }
                return this._items;
            }
        }

        internal System.Web.NotificationContext NotificationContext
        {
            get => 
                this._notificationContext;
            set
            {
                this._notificationContext = value;
            }
        }

        internal bool PreventPostback
        {
            get => 
                this._preventPostback;
            set
            {
                this._preventPostback = value;
            }
        }

        public IHttpHandler PreviousHandler
        {
            get
            {
                if ((this._handlerStack != null) && (this._handlerStack.Count != 0))
                {
                    return (IHttpHandler) this._handlerStack.Peek();
                }
                return null;
            }
        }

        public ProfileBase Profile
        {
            get
            {
                if ((this._Profile == null) && this._ProfileDelayLoad)
                {
                    this._Profile = ProfileBase.Create(this.Request.IsAuthenticated ? this.User.Identity.Name : this.Request.AnonymousID, this.Request.IsAuthenticated);
                }
                return this._Profile;
            }
        }

        internal IHttpHandler RemapHandlerInstance =>
            this._remapHandler;

        public HttpRequest Request
        {
            get
            {
                if (this.HideRequestResponse)
                {
                    throw new HttpException(System.Web.SR.GetString("Request_not_available"));
                }
                return this._request;
            }
        }

        public HttpResponse Response
        {
            get
            {
                if (this.HideRequestResponse)
                {
                    throw new HttpException(System.Web.SR.GetString("Response_not_available"));
                }
                return this._response;
            }
        }

        public HttpServerUtility Server
        {
            get
            {
                if (this._server == null)
                {
                    this._server = new HttpServerUtility(this);
                }
                return this._server;
            }
        }

        internal int ServerExecuteDepth
        {
            get => 
                this._serverExecuteDepth;
            set
            {
                this._serverExecuteDepth = value;
            }
        }

        public HttpSessionState Session
        {
            get
            {
                if (this._sessionStateModule != null)
                {
                    lock (this)
                    {
                        if (this._sessionStateModule != null)
                        {
                            this._sessionStateModule.InitStateStoreItem(true);
                            this._sessionStateModule = null;
                        }
                    }
                }
                return (HttpSessionState) this.Items["AspSession"];
            }
        }

        public bool SkipAuthorization
        {
            get => 
                this._skipAuthorization;
            [SecurityPermission(SecurityAction.Demand, ControlPrincipal=true)]
            set
            {
                this.SetSkipAuthorizationNoDemand(value, false);
            }
        }

        internal string SqlDependencyCookie
        {
            get => 
                this._sqlDependencyCookie;
            set
            {
                this._sqlDependencyCookie = value;
                CallContext.LogicalSetData("MS.SqlDependencyCookie", value);
            }
        }

        internal AspNetSynchronizationContext SyncContext
        {
            get
            {
                if (this._syncContext == null)
                {
                    this._syncContext = new AspNetSynchronizationContext(this.ApplicationInstance);
                }
                return this._syncContext;
            }
        }

        internal Exception TempError
        {
            get => 
                this._tempError;
            set
            {
                this._tempError = value;
            }
        }

        internal System.Web.UI.TemplateControl TemplateControl
        {
            get => 
                this._templateControl;
            set
            {
                this._templateControl = value;
            }
        }

        internal long TimeLeft
        {
            get
            {
                long totalMilliseconds = 0x7fffffffffffffffL;
                if (this.IsInCancellablePeriod && (this.Timeout.TotalMilliseconds >= 0.0))
                {
                    try
                    {
                        if (!CompilationUtil.IsDebuggingEnabled(this) && !Debugger.IsAttached)
                        {
                            TimeSpan span = (TimeSpan) (this._timeoutStartTime.Add(this.Timeout) - DateTime.UtcNow);
                            totalMilliseconds = (long) span.TotalMilliseconds;
                        }
                    }
                    catch
                    {
                    }
                }
                return totalMilliseconds;
            }
        }

        internal TimeSpan Timeout
        {
            get
            {
                this.EnsureTimeout();
                return this._timeout;
            }
            set
            {
                this._timeout = value;
                this._timeoutSet = true;
            }
        }

        internal DoubleLink TimeoutLink
        {
            get => 
                this._timeoutLink;
            set
            {
                this._timeoutLink = value;
            }
        }

        public DateTime Timestamp =>
            this._utcTimestamp.ToLocalTime();

        internal IHttpHandler TopHandler
        {
            get
            {
                if (this._handlerStack != null)
                {
                    object[] objArray = this._handlerStack.ToArray();
                    if ((objArray != null) && (objArray.Length != 0))
                    {
                        return (IHttpHandler) objArray[objArray.Length - 1];
                    }
                }
                return this._handler;
            }
        }

        public TraceContext Trace
        {
            get
            {
                if (this._topTraceContext == null)
                {
                    this._topTraceContext = new TraceContext(this);
                }
                return this._topTraceContext;
            }
        }

        internal bool TraceIsEnabled
        {
            get => 
                this._topTraceContext?.IsEnabled;
            set
            {
                if (value)
                {
                    this._topTraceContext = new TraceContext(this);
                }
            }
        }

        public IPrincipal User
        {
            get => 
                this._user;
            [SecurityPermission(SecurityAction.Demand, ControlPrincipal=true)]
            set
            {
                this.SetPrincipalNoDemand(value);
            }
        }

        internal bool UsesImpersonation
        {
            get
            {
                if (HttpRuntime.IsOnUNCShareInternal && (HostingEnvironment.ApplicationIdentityToken != IntPtr.Zero))
                {
                    return true;
                }
                if (!this._impersonationEnabled)
                {
                    return false;
                }
                if (((this._notificationContext.CurrentNotification != RequestNotification.AuthenticateRequest) || !this._notificationContext.IsPostNotification) && (this._notificationContext.CurrentNotification <= RequestNotification.AuthenticateRequest))
                {
                    return false;
                }
                return (this._notificationContext.CurrentNotification != RequestNotification.SendResponse);
            }
        }

        internal DateTime UtcTimestamp =>
            this._utcTimestamp;

        internal HttpWorkerRequest WorkerRequest =>
            this._wr;
    }
}

