namespace System.Web.Hosting
{
    using System;
    using System.Collections;
    using System.Configuration;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Runtime.Remoting;
    using System.Security.Permissions;
    using System.Web;
    using System.Web.Configuration;
    using System.Web.Util;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public sealed class ProcessHost : MarshalByRefObject, IProcessHost, IAdphManager, IPphManager, IProcessHostIdleAndHealthCheck
    {
        private ApplicationManager _appManager;
        private IProcessHostSupportFunctions _functions;
        private static object _processHostStaticLock = new object();
        private Hashtable _protocolHandlers = new Hashtable();
        private ProtocolsSection _protocolsConfig;
        private static ProcessHost _theProcessHost;

        [AspNetHostingPermission(SecurityAction.Demand, Level=AspNetHostingPermissionLevel.Minimal)]
        private ProcessHost(IProcessHostSupportFunctions functions)
        {
            try
            {
                this._functions = functions;
                HostingEnvironment.SupportFunctions = functions;
                this._appManager = ApplicationManager.GetApplicationManager();
            }
            catch (Exception exception)
            {
                using (new ProcessImpersonationContext())
                {
                    Misc.ReportUnhandledException(exception, new string[] { System.Web.SR.GetString("Cant_Create_Process_Host") });
                }
                throw;
            }
        }

        private ISAPIApplicationHost CreateAppHost(string appId, string appPath)
        {
            if (string.IsNullOrEmpty(appPath))
            {
                string str;
                string str2;
                string str3;
                string str4;
                this._functions.GetApplicationProperties(appId, out str, out str2, out str3, out str4);
                if (!System.Web.Util.StringUtil.StringEndsWith(str2, '\\'))
                {
                    str2 = str2 + @"\";
                }
                appPath = str2;
            }
            return new ISAPIApplicationHost(appId, appPath, false, this._functions);
        }

        public void EnumerateAppDomains(out IAppDomainInfoEnum appDomainInfoEnum)
        {
            try
            {
                AppDomainInfo[] appDomainInfos = ApplicationManager.GetApplicationManager().GetAppDomainInfos();
                appDomainInfoEnum = new AppDomainInfoEnum(appDomainInfos);
            }
            catch (Exception exception)
            {
                using (new ProcessImpersonationContext())
                {
                    Misc.ReportUnhandledException(exception, new string[] { System.Web.SR.GetString("Failure_AppDomain_Enum") });
                }
                throw;
            }
        }

        private Type GetAppDomainProtocolHandlerType(string protocolId)
        {
            Type type = null;
            try
            {
                ProtocolElement element = this.ProtocolsConfig.Protocols[protocolId];
                if (element == null)
                {
                    throw new ArgumentException(System.Web.SR.GetString("Unknown_protocol_id", new object[] { protocolId }));
                }
                type = this.ValidateAndGetType(element, element.AppDomainHandlerType, typeof(AppDomainProtocolHandler), "AppDomainHandlerType");
            }
            catch (Exception exception)
            {
                using (new ProcessImpersonationContext())
                {
                    Misc.ReportUnhandledException(exception, new string[] { System.Web.SR.GetString("Invalid_AppDomain_Prot_Type") });
                }
            }
            return type;
        }

        internal static ProcessHost GetProcessHost(IProcessHostSupportFunctions functions)
        {
            if (_theProcessHost == null)
            {
                lock (_processHostStaticLock)
                {
                    if (_theProcessHost == null)
                    {
                        _theProcessHost = new ProcessHost(functions);
                    }
                }
            }
            return _theProcessHost;
        }

        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.Infrastructure)]
        public override object InitializeLifetimeService() => 
            null;

        public bool IsIdle()
        {
            bool flag = false;
            try
            {
                flag = this._appManager.IsIdle();
            }
            catch (Exception exception)
            {
                using (new ProcessImpersonationContext())
                {
                    Misc.ReportUnhandledException(exception, new string[] { System.Web.SR.GetString("Failure_PMH_Idle") });
                }
                throw;
            }
            return flag;
        }

        public void Ping(IProcessPingCallback callback)
        {
            try
            {
                if (callback != null)
                {
                    this._appManager.Ping(callback);
                }
            }
            catch (Exception exception)
            {
                using (new ProcessImpersonationContext())
                {
                    Misc.ReportUnhandledException(exception, new string[] { System.Web.SR.GetString("Failure_PMH_Ping") });
                }
                throw;
            }
        }

        public void Shutdown()
        {
            try
            {
                ArrayList list = new ArrayList();
                lock (this)
                {
                    foreach (DictionaryEntry entry in this._protocolHandlers)
                    {
                        list.Add(entry.Value);
                    }
                    this._protocolHandlers = new Hashtable();
                }
                foreach (ProcessProtocolHandler handler in list)
                {
                    handler.StopProtocol(true);
                }
                this._appManager.ShutdownAll();
                while (Marshal.ReleaseComObject(this._functions) != 0)
                {
                }
            }
            catch (Exception exception)
            {
                using (new ProcessImpersonationContext())
                {
                    Misc.ReportUnhandledException(exception, new string[] { System.Web.SR.GetString("Failure_Shutdown_ProcessHost"), exception.ToString() });
                }
                throw;
            }
        }

        public void ShutdownApplication(string appId)
        {
            try
            {
                this._appManager.ShutdownApplication(appId);
            }
            catch (Exception exception)
            {
                using (new ProcessImpersonationContext())
                {
                    Misc.ReportUnhandledException(exception, new string[] { System.Web.SR.GetString("Failure_Stop_Integrated_App") });
                }
                throw;
            }
        }

        public void StartAppDomainProtocolListenerChannel(string appId, string protocolId, IListenerChannelCallback listenerChannelCallback)
        {
            try
            {
                if (appId == null)
                {
                    throw new ArgumentNullException("appId");
                }
                if (protocolId == null)
                {
                    throw new ArgumentNullException("protocolId");
                }
                ISAPIApplicationHost appHost = this.CreateAppHost(appId, null);
                Type appDomainProtocolHandlerType = this.GetAppDomainProtocolHandlerType(protocolId);
                AppDomainProtocolHandler handler = null;
                lock (this._appManager)
                {
                    HostingEnvironmentParameters hostingParameters = new HostingEnvironmentParameters {
                        HostingFlags = HostingEnvironmentFlags.ThrowHostingInitErrors
                    };
                    handler = (AppDomainProtocolHandler) this._appManager.CreateObjectInternal(appId, appDomainProtocolHandlerType, appHost, false, hostingParameters);
                    ListenerAdapterDispatchShim shim = (ListenerAdapterDispatchShim) this._appManager.CreateObjectInternal(appId, typeof(ListenerAdapterDispatchShim), appHost, false, hostingParameters);
                    if (shim == null)
                    {
                        throw new HttpException(System.Web.SR.GetString("Failure_Create_Listener_Shim"));
                    }
                    shim.StartListenerChannel(handler, listenerChannelCallback);
                    ((IRegisteredObject) shim).Stop(true);
                }
            }
            catch (Exception exception)
            {
                using (new ProcessImpersonationContext())
                {
                    Misc.ReportUnhandledException(exception, new string[] { System.Web.SR.GetString("Failure_Start_AppDomain_Listener") });
                }
                throw;
            }
        }

        public void StartApplication(string appId, string appPath, out object runtimeInterface)
        {
            try
            {
                if (appId == null)
                {
                    throw new ArgumentNullException("appId");
                }
                if (appPath == null)
                {
                    throw new ArgumentNullException("appPath");
                }
                runtimeInterface = null;
                PipelineRuntime o = null;
                if (appPath[0] == '.')
                {
                    FileInfo info = new FileInfo(appPath);
                    appPath = info.FullName;
                }
                if (!System.Web.Util.StringUtil.StringEndsWith(appPath, '\\'))
                {
                    appPath = appPath + @"\";
                }
                IApplicationHost appHost = this.CreateAppHost(appId, appPath);
                lock (this._appManager)
                {
                    this._appManager.RemoveFromTableIfRuntimeExists(appId, typeof(PipelineRuntime));
                    try
                    {
                        o = (PipelineRuntime) this._appManager.CreateObjectInternal(appId, typeof(PipelineRuntime), appHost, true, null);
                    }
                    catch (AppDomainUnloadedException)
                    {
                    }
                    if (o != null)
                    {
                        o.SetThisAppDomainsIsapiAppId(appId);
                        o.StartProcessing();
                        runtimeInterface = new ObjectHandle(o);
                    }
                }
            }
            catch (Exception exception)
            {
                using (new ProcessImpersonationContext())
                {
                    Misc.ReportUnhandledException(exception, new string[] { System.Web.SR.GetString("Failure_Start_Integrated_App") });
                }
                throw;
            }
        }

        public void StartProcessProtocolListenerChannel(string protocolId, IListenerChannelCallback listenerChannelCallback)
        {
            try
            {
                if (protocolId == null)
                {
                    throw new ArgumentNullException("protocolId");
                }
                ProtocolElement element = this.ProtocolsConfig.Protocols[protocolId];
                if (element == null)
                {
                    throw new ArgumentException(System.Web.SR.GetString("Unknown_protocol_id", new object[] { protocolId }));
                }
                ProcessProtocolHandler handler = null;
                Type type = null;
                type = this.ValidateAndGetType(element, element.ProcessHandlerType, typeof(ProcessProtocolHandler), "ProcessHandlerType");
                lock (this)
                {
                    handler = this._protocolHandlers[protocolId] as ProcessProtocolHandler;
                    if (handler == null)
                    {
                        handler = (ProcessProtocolHandler) Activator.CreateInstance(type);
                        this._protocolHandlers[protocolId] = handler;
                    }
                }
                if (handler != null)
                {
                    handler.StartListenerChannel(listenerChannelCallback, this);
                }
            }
            catch (Exception exception)
            {
                using (new ProcessImpersonationContext())
                {
                    Misc.ReportUnhandledException(exception, new string[] { System.Web.SR.GetString("Invalid_Process_Prot_Type") });
                }
                throw;
            }
        }

        public void StopAppDomainProtocol(string appId, string protocolId, bool immediate)
        {
            try
            {
                if (appId == null)
                {
                    throw new ArgumentNullException("appId");
                }
                if (protocolId == null)
                {
                    throw new ArgumentNullException("protocolId");
                }
                Type appDomainProtocolHandlerType = this.GetAppDomainProtocolHandlerType(protocolId);
                AppDomainProtocolHandler handler = null;
                lock (this._appManager)
                {
                    handler = (AppDomainProtocolHandler) this._appManager.GetObject(appId, appDomainProtocolHandlerType);
                }
                if (handler != null)
                {
                    handler.StopProtocol(immediate);
                }
            }
            catch (Exception exception)
            {
                using (new ProcessImpersonationContext())
                {
                    Misc.ReportUnhandledException(exception, new string[] { System.Web.SR.GetString("Failure_Stop_AppDomain_Protocol") });
                }
                throw;
            }
        }

        public void StopAppDomainProtocolListenerChannel(string appId, string protocolId, int listenerChannelId, bool immediate)
        {
            try
            {
                if (appId == null)
                {
                    throw new ArgumentNullException("appId");
                }
                if (protocolId == null)
                {
                    throw new ArgumentNullException("protocolId");
                }
                Type appDomainProtocolHandlerType = this.GetAppDomainProtocolHandlerType(protocolId);
                AppDomainProtocolHandler handler = null;
                lock (this._appManager)
                {
                    handler = (AppDomainProtocolHandler) this._appManager.GetObject(appId, appDomainProtocolHandlerType);
                }
                if (handler != null)
                {
                    handler.StopListenerChannel(listenerChannelId, immediate);
                }
            }
            catch (Exception exception)
            {
                using (new ProcessImpersonationContext())
                {
                    Misc.ReportUnhandledException(exception, new string[] { System.Web.SR.GetString("Failure_Stop_AppDomain_Listener") });
                }
                throw;
            }
        }

        public void StopProcessProtocol(string protocolId, bool immediate)
        {
            try
            {
                if (protocolId == null)
                {
                    throw new ArgumentNullException("protocolId");
                }
                ProcessProtocolHandler handler = null;
                lock (this)
                {
                    handler = this._protocolHandlers[protocolId] as ProcessProtocolHandler;
                    if (handler != null)
                    {
                        this._protocolHandlers.Remove(protocolId);
                    }
                }
                if (handler != null)
                {
                    handler.StopProtocol(immediate);
                }
            }
            catch (Exception exception)
            {
                using (new ProcessImpersonationContext())
                {
                    Misc.ReportUnhandledException(exception, new string[] { System.Web.SR.GetString("Failure_Stop_Process_Prot") });
                }
                throw;
            }
        }

        public void StopProcessProtocolListenerChannel(string protocolId, int listenerChannelId, bool immediate)
        {
            try
            {
                if (protocolId == null)
                {
                    throw new ArgumentNullException("protocolId");
                }
                ProcessProtocolHandler handler = null;
                lock (this)
                {
                    handler = this._protocolHandlers[protocolId] as ProcessProtocolHandler;
                }
                if (handler != null)
                {
                    handler.StopListenerChannel(listenerChannelId, immediate);
                }
            }
            catch (Exception exception)
            {
                using (new ProcessImpersonationContext())
                {
                    Misc.ReportUnhandledException(exception, new string[] { System.Web.SR.GetString("Failure_Stop_Listener_Channel") });
                }
                throw;
            }
        }

        private Type ValidateAndGetType(ProtocolElement element, string typeName, Type assignableType, string elementPropertyName)
        {
            Type type;
            try
            {
                type = Type.GetType(typeName, true);
            }
            catch (Exception exception)
            {
                PropertyInformation information = null;
                string filename = string.Empty;
                int line = 0;
                if ((element != null) && (element.ElementInformation != null))
                {
                    information = element.ElementInformation.Properties[elementPropertyName];
                    if (information != null)
                    {
                        filename = information.Source;
                        line = information.LineNumber;
                    }
                }
                throw new ConfigurationErrorsException(exception.Message, exception, filename, line);
            }
            ConfigUtil.CheckAssignableType(assignableType, type, element, elementPropertyName);
            return type;
        }

        internal static ProcessHost DefaultHost =>
            _theProcessHost;

        private ProtocolsSection ProtocolsConfig
        {
            get
            {
                if (this._protocolsConfig == null)
                {
                    lock (this)
                    {
                        if (this._protocolsConfig == null)
                        {
                            if (HttpConfigurationSystem.IsSet)
                            {
                                this._protocolsConfig = RuntimeConfig.GetRootWebConfig().Protocols;
                            }
                            else
                            {
                                System.Configuration.Configuration configuration = WebConfigurationManager.OpenWebConfiguration(null);
                                this._protocolsConfig = (ProtocolsSection) configuration.GetSection("system.web/protocols");
                            }
                        }
                    }
                }
                return this._protocolsConfig;
            }
        }

        internal IProcessHostSupportFunctions SupportFunctions =>
            this._functions;
    }
}

