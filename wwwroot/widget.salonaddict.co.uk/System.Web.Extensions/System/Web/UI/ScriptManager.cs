namespace System.Web.UI
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Configuration;
    using System.Drawing;
    using System.Drawing.Design;
    using System.Globalization;
    using System.Reflection;
    using System.Security;
    using System.Security.Permissions;
    using System.Text;
    using System.Web;
    using System.Web.Configuration;
    using System.Web.Globalization;
    using System.Web.Handlers;
    using System.Web.Resources;
    using System.Web.Script.Serialization;
    using System.Web.Script.Services;
    using System.Web.UI.Design;
    using System.Web.Util;

    [NonVisualControl, DefaultProperty("Scripts"), Designer("System.Web.UI.Design.ScriptManagerDesigner, System.Web.Extensions.Design, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"), ParseChildren(true), PersistChildren(false), ToolboxBitmap(typeof(EmbeddedResourceFinder), "System.Web.Resources.ScriptManager.bmp"), ToolboxItemFilter("System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35", ToolboxItemFilterType.Require), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class ScriptManager : System.Web.UI.Control, IPostBackDataHandler, IPostBackEventHandler, IControl, IClientUrlResolver, IScriptManager, IScriptManagerInternal
    {
        private bool _allowCustomErrorsRedirect;
        private readonly ICompilationSection _appLevelCompilationSection;
        private string _asyncPostBackErrorMessage;
        private int _asyncPostBackTimeout;
        private AuthenticationServiceManager _authenticationServiceManager;
        private string _clientNavigateHandler;
        private CompositeScriptReference _compositeScript;
        private readonly IControl _control;
        private readonly ICustomErrorsSection _customErrorsSection;
        private readonly IDeploymentSection _deploymentSection;
        private bool _enableHistory;
        private bool _enablePageMethods;
        private bool _enablePartialRendering;
        private bool _enableScriptGlobalization;
        private bool _enableScriptLocalization;
        private bool _enableSecureHistoryState;
        private bool _initCompleted;
        private Hashtable _initialState;
        private bool _isInAsyncPostBack;
        private bool _isNavigating;
        private bool _loadScriptsBeforeUI;
        private bool _newPointCreated;
        private readonly System.Web.UI.IPage _page;
        private System.Web.UI.PageRequestManager _pageRequestManager;
        private ProfileServiceManager _profileServiceManager;
        private List<ScriptManagerProxy> _proxies;
        private RoleServiceManager _roleServiceManager;
        private System.Web.UI.ScriptControlManager _scriptControlManager;
        private System.Web.UI.ScriptMode _scriptMode;
        private string _scriptPath;
        private ScriptRegistrationManager _scriptRegistration;
        private ScriptReferenceCollection _scripts;
        private ServiceReferenceCollection _services;
        private bool _supportsPartialRendering;
        internal bool _supportsPartialRenderingSetByUser;
        private int _uniqueScriptCounter;
        private bool _zip;
        private bool _zipSet;
        private static readonly object AsyncPostBackErrorEvent = new object();
        private const int AsyncPostBackTimeoutDefault = 90;
        private static readonly object NavigateEvent = new object();
        private static readonly object ResolveCompositeScriptReferenceEvent = new object();
        private static readonly object ResolveScriptReferenceEvent = new object();

        [Category("Action"), ResourceDescription("ScriptManager_AsyncPostBackError")]
        public event EventHandler<AsyncPostBackErrorEventArgs> AsyncPostBackError
        {
            add
            {
                base.Events.AddHandler(AsyncPostBackErrorEvent, value);
            }
            remove
            {
                base.Events.RemoveHandler(AsyncPostBackErrorEvent, value);
            }
        }

        [Category("Action"), ResourceDescription("ScriptManager_Navigate")]
        public event EventHandler<HistoryEventArgs> Navigate
        {
            add
            {
                base.Events.AddHandler(NavigateEvent, value);
            }
            remove
            {
                base.Events.RemoveHandler(NavigateEvent, value);
            }
        }

        [Category("Action"), ResourceDescription("ScriptManager_ResolveCompositeScriptReference")]
        public event EventHandler<CompositeScriptReferenceEventArgs> ResolveCompositeScriptReference
        {
            add
            {
                base.Events.AddHandler(ResolveCompositeScriptReferenceEvent, value);
            }
            remove
            {
                base.Events.RemoveHandler(ResolveCompositeScriptReferenceEvent, value);
            }
        }

        [ResourceDescription("ScriptManager_ResolveScriptReference"), Category("Action")]
        public event EventHandler<ScriptReferenceEventArgs> ResolveScriptReference
        {
            add
            {
                base.Events.AddHandler(ResolveScriptReferenceEvent, value);
            }
            remove
            {
                base.Events.RemoveHandler(ResolveScriptReferenceEvent, value);
            }
        }

        public ScriptManager()
        {
            this._enablePartialRendering = true;
            this._supportsPartialRendering = true;
            this._enableScriptLocalization = true;
            this._loadScriptsBeforeUI = true;
            this._asyncPostBackTimeout = 90;
            this._allowCustomErrorsRedirect = true;
            this._enableSecureHistoryState = true;
        }

        internal ScriptManager(IControl control, System.Web.UI.IPage page, ICompilationSection appLevelCompilationSection, IDeploymentSection deploymentSection, ICustomErrorsSection customErrorsSection)
        {
            this._enablePartialRendering = true;
            this._supportsPartialRendering = true;
            this._enableScriptLocalization = true;
            this._loadScriptsBeforeUI = true;
            this._asyncPostBackTimeout = 90;
            this._allowCustomErrorsRedirect = true;
            this._enableSecureHistoryState = true;
            this._control = control;
            this._page = page;
            this._appLevelCompilationSection = appLevelCompilationSection;
            this._deploymentSection = deploymentSection;
            this._customErrorsSection = customErrorsSection;
        }

        private void AddFrameworkLoadedCheck()
        {
            this.IPage.ClientScript.RegisterClientScriptBlock(typeof(ScriptManager), "FrameworkLoadedCheck", "if (typeof(Sys) === 'undefined') throw new Error('" + JavaScriptString.QuoteString(AtlasWeb.ScriptManager_FrameworkFailedToLoad) + "');\r\n", true);
        }

        private static void AddFrameworkScript(ScriptReference frameworkScript, List<ScriptReferenceBase> scripts, int scriptIndex)
        {
            ScriptReferenceBase item = frameworkScript;
            if (scripts.Count != 0)
            {
                string name = frameworkScript.Name;
                Assembly assembly = frameworkScript.GetAssembly();
                foreach (ScriptReferenceBase base3 in scripts)
                {
                    ScriptReference reference = base3 as ScriptReference;
                    if (((reference != null) && (reference.Name == name)) && (reference.GetAssembly() == assembly))
                    {
                        item = base3;
                        scripts.Remove(base3);
                        break;
                    }
                    CompositeScriptReference reference2 = base3 as CompositeScriptReference;
                    if (reference2 != null)
                    {
                        bool flag = false;
                        foreach (ScriptReference reference3 in reference2.Scripts)
                        {
                            if ((reference3.Name == name) && (reference3.GetAssembly() == assembly))
                            {
                                item = base3;
                                flag = true;
                                break;
                            }
                        }
                        if (flag)
                        {
                            break;
                        }
                    }
                }
            }
            item.AlwaysLoadBeforeUI = true;
            if (!(item is CompositeScriptReference))
            {
                scripts.Insert(scriptIndex, item);
            }
        }

        internal void AddFrameworkScripts(List<ScriptReferenceBase> scripts)
        {
            ScriptReference frameworkScript = new ScriptReference("MicrosoftAjax.js", this, this);
            AddFrameworkScript(frameworkScript, scripts, 0);
            if (this.SupportsPartialRendering)
            {
                ScriptReference reference2 = new ScriptReference("MicrosoftAjaxWebForms.js", this, this);
                AddFrameworkScript(reference2, scripts, 1);
            }
        }

        public void AddHistoryPoint(NameValueCollection state, string title)
        {
            this.PrepareNewHistoryPoint();
            foreach (string str in state)
            {
                this.SetStateValue(str, state[str]);
            }
            this.SetPageTitle(title);
        }

        public void AddHistoryPoint(string key, string value)
        {
            this.AddHistoryPoint(key, value, null);
        }

        public void AddHistoryPoint(string key, string value, string title)
        {
            this.PrepareNewHistoryPoint();
            this.SetStateValue(key, value);
            this.SetPageTitle(title);
        }

        internal void AddScriptCollections(List<ScriptReferenceBase> scripts, IEnumerable<ScriptManagerProxy> proxies)
        {
            if ((this._compositeScript != null) && (this._compositeScript.Scripts.Count != 0))
            {
                this._compositeScript.ClientUrlResolver = this.Control;
                this._compositeScript.ContainingControl = this;
                this._compositeScript.IsStaticReference = true;
                scripts.Add(this._compositeScript);
            }
            if (this._scripts != null)
            {
                foreach (ScriptReference reference in this._scripts)
                {
                    reference.ClientUrlResolver = this.Control;
                    reference.ContainingControl = this;
                    reference.IsStaticReference = true;
                    scripts.Add(reference);
                }
            }
            if (proxies != null)
            {
                foreach (ScriptManagerProxy proxy in proxies)
                {
                    proxy.CollectScripts(scripts);
                }
            }
        }

        private void ConfigureApplicationServices()
        {
            StringBuilder sb = null;
            ProfileServiceManager.ConfigureProfileService(ref sb, this.Context, this, this._proxies);
            AuthenticationServiceManager.ConfigureAuthenticationService(ref sb, this.Context, this, this._proxies);
            RoleServiceManager.ConfigureRoleService(ref sb, this.Context, this, this._proxies);
            if ((sb != null) && (sb.Length > 0))
            {
                this.IPage.ClientScript.RegisterClientScriptBlock(typeof(ScriptManager), "AppServicesConfig", sb.ToString(), true);
            }
        }

        internal string CreateUniqueScriptKey()
        {
            this._uniqueScriptCounter++;
            return ("UniqueScript_" + this._uniqueScriptCounter.ToString(CultureInfo.InvariantCulture));
        }

        public static ScriptManager GetCurrent(Page page)
        {
            if (page == null)
            {
                throw new ArgumentNullException("page");
            }
            return (page.Items[typeof(ScriptManager)] as ScriptManager);
        }

        [SecurityCritical, ConfigurationPermission(SecurityAction.Assert, Unrestricted=true), AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal)]
        private static ICustomErrorsSection GetCustomErrorsSectionWithAssert() => 
            new CustomErrorsSectionWrapper((System.Web.Configuration.CustomErrorsSection) WebConfigurationManager.GetSection("system.web/customErrors"));

        public ReadOnlyCollection<RegisteredArrayDeclaration> GetRegisteredArrayDeclarations() => 
            new ReadOnlyCollection<RegisteredArrayDeclaration>(this.ScriptRegistration.ScriptArrays);

        public ReadOnlyCollection<RegisteredScript> GetRegisteredClientScriptBlocks() => 
            new ReadOnlyCollection<RegisteredScript>(this.ScriptRegistration.ScriptBlocks);

        public ReadOnlyCollection<RegisteredDisposeScript> GetRegisteredDisposeScripts() => 
            new ReadOnlyCollection<RegisteredDisposeScript>(this.ScriptRegistration.ScriptDisposes);

        public ReadOnlyCollection<RegisteredExpandoAttribute> GetRegisteredExpandoAttributes() => 
            new ReadOnlyCollection<RegisteredExpandoAttribute>(this.ScriptRegistration.ScriptExpandos);

        public ReadOnlyCollection<RegisteredHiddenField> GetRegisteredHiddenFields() => 
            new ReadOnlyCollection<RegisteredHiddenField>(this.ScriptRegistration.ScriptHiddenFields);

        public ReadOnlyCollection<RegisteredScript> GetRegisteredOnSubmitStatements() => 
            new ReadOnlyCollection<RegisteredScript>(this.ScriptRegistration.ScriptSubmitStatements);

        public ReadOnlyCollection<RegisteredScript> GetRegisteredStartupScripts() => 
            new ReadOnlyCollection<RegisteredScript>(this.ScriptRegistration.ScriptStartupBlocks);

        internal string GetScriptResourceUrl(string resourceName, Assembly assembly) => 
            ScriptResourceHandler.GetScriptResourceUrl(assembly, resourceName, this.EnableScriptLocalization ? CultureInfo.CurrentUICulture : CultureInfo.InvariantCulture, this.Zip, true);

        public string GetStateString()
        {
            if (this.EnableSecureHistoryState)
            {
                StatePersister persister = new StatePersister(this.Page);
                return persister.Serialize(this._initialState);
            }
            if (this._initialState == null)
            {
                return string.Empty;
            }
            StringBuilder builder = new StringBuilder();
            bool flag = true;
            foreach (DictionaryEntry entry in this._initialState)
            {
                if (!flag)
                {
                    builder.Append('&');
                }
                else
                {
                    flag = false;
                }
                builder.Append(HttpUtility.UrlEncode((string) entry.Key));
                builder.Append('=');
                builder.Append(HttpUtility.UrlEncode((string) entry.Value));
            }
            return builder.ToString();
        }

        private void LoadHistoryState(string serverState)
        {
            NameValueCollection values;
            if (string.IsNullOrEmpty(serverState))
            {
                this._initialState = new Hashtable();
                values = new NameValueCollection();
            }
            else if (this.EnableSecureHistoryState)
            {
                StatePersister persister = new StatePersister(this.Page);
                this._initialState = (Hashtable) persister.Deserialize(serverState);
                values = new NameValueCollection();
                foreach (DictionaryEntry entry in this._initialState)
                {
                    values.Add((string) entry.Key, (string) entry.Value);
                }
            }
            else
            {
                values = HttpUtility.ParseQueryString(serverState);
                this._initialState = new Hashtable(values.Count);
                foreach (string str in values)
                {
                    this._initialState.Add(str, values[str]);
                }
            }
            HistoryEventArgs e = new HistoryEventArgs(values);
            this.RaiseNavigate(e);
        }

        protected virtual bool LoadPostData(string postDataKey, NameValueCollection postCollection)
        {
            if (this.IsInAsyncPostBack)
            {
                this.PageRequestManager.LoadPostData(postDataKey, postCollection);
            }
            else if (this.EnableHistory)
            {
                string serverState = postCollection[postDataKey];
                this.LoadHistoryState(serverState);
            }
            return false;
        }

        protected internal virtual void OnAsyncPostBackError(AsyncPostBackErrorEventArgs e)
        {
            EventHandler<AsyncPostBackErrorEventArgs> handler = (EventHandler<AsyncPostBackErrorEventArgs>) base.Events[AsyncPostBackErrorEvent];
            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected internal override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            if (!base.DesignMode)
            {
                System.Web.UI.IPage iPage = this.IPage;
                if (GetCurrent(this.Page) != null)
                {
                    throw new InvalidOperationException(AtlasWeb.ScriptManager_OnlyOneScriptManager);
                }
                iPage.Items[typeof(IScriptManager)] = this;
                iPage.Items[typeof(ScriptManager)] = this;
                iPage.InitComplete += new EventHandler(this.OnPageInitComplete);
                iPage.PreRenderComplete += new EventHandler(this.OnPagePreRenderComplete);
                if (iPage.IsPostBack)
                {
                    this._isInAsyncPostBack = System.Web.UI.PageRequestManager.IsAsyncPostBackRequest(iPage.Request);
                }
                this.PageRequestManager.OnInit();
                iPage.PreRender += new EventHandler(this.ScriptControlManager.OnPagePreRender);
            }
        }

        private void OnPageInitComplete(object sender, EventArgs e)
        {
            if ((this.IPage.IsPostBack && this.IsInAsyncPostBack) && !this.SupportsPartialRendering)
            {
                throw new InvalidOperationException(AtlasWeb.ScriptManager_AsyncPostBackNotInPartialRenderingMode);
            }
            this._initCompleted = true;
            if (this.EnableHistory)
            {
                this.RegisterAsyncPostBackControl(this);
                if (this.IPage.IsPostBack)
                {
                    this._isNavigating = this.IPage.Request["__EVENTTARGET"] == this.UniqueID;
                }
            }
        }

        private void OnPagePreRenderComplete(object sender, EventArgs e)
        {
            if (!this.IsInAsyncPostBack)
            {
                if (this.SupportsPartialRendering)
                {
                    this.IPage.ClientScript.GetPostBackEventReference(new PostBackOptions(this, null, null, false, false, false, false, true, null));
                }
                this.RegisterGlobalizationScriptBlock();
                this.RegisterScripts();
                this.RegisterServices();
                if (this.EnableHistory)
                {
                    JavaScriptSerializer serializer = new JavaScriptSerializer(new SimpleTypeResolver());
                    string script = "\r\nSys.Application.setServerId(" + serializer.Serialize(this.ClientID) + ", " + serializer.Serialize(this.UniqueID) + ");\r\n" + (((this._initialState != null) && (this._initialState.Count != 0)) ? ("  Sys.Application.setServerState('" + this.GetStateString() + "');\r\n") : "") + "\r\nSys.Application._enableHistoryInScriptManager();" + (!string.IsNullOrEmpty(this.ClientNavigateHandler) ? ("  Sys.Application.add_navigate(" + this.ClientNavigateHandler + ");\r\n") : "\r\n");
                    RegisterStartupScript(this, typeof(ScriptManager), "HistoryStartup", script, true);
                }
            }
            else
            {
                this.RegisterScripts();
                if (this.EnableHistory)
                {
                    if ((this._initialState != null) && (this._initialState.Count == 0))
                    {
                        this._initialState = null;
                    }
                    if (this._newPointCreated)
                    {
                        this.RegisterDataItem(this, "'" + this.GetStateString() + "'", true);
                    }
                }
            }
        }

        protected internal override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            if (this.IsInAsyncPostBack)
            {
                this.PageRequestManager.OnPreRender();
            }
        }

        protected virtual void OnResolveCompositeScriptReference(CompositeScriptReferenceEventArgs e)
        {
            EventHandler<CompositeScriptReferenceEventArgs> handler = (EventHandler<CompositeScriptReferenceEventArgs>) base.Events[ResolveCompositeScriptReferenceEvent];
            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected virtual void OnResolveScriptReference(ScriptReferenceEventArgs e)
        {
            EventHandler<ScriptReferenceEventArgs> handler = (EventHandler<ScriptReferenceEventArgs>) base.Events[ResolveScriptReferenceEvent];
            if (handler != null)
            {
                handler(this, e);
            }
        }

        private void PrepareNewHistoryPoint()
        {
            if (!this.EnableHistory)
            {
                throw new InvalidOperationException(AtlasWeb.ScriptManager_CannotAddHistoryPointWithHistoryDisabled);
            }
            if (!this.IsInAsyncPostBack)
            {
                throw new InvalidOperationException(AtlasWeb.ScriptManager_CannotAddHistoryPointOutsideOfAsyncPostBack);
            }
            this._newPointCreated = true;
            if (this._initialState == null)
            {
                this._initialState = new Hashtable();
            }
        }

        private void RaiseNavigate(HistoryEventArgs e)
        {
            EventHandler<HistoryEventArgs> navigateEvent = (EventHandler<HistoryEventArgs>) base.Events[NavigateEvent];
            if (navigateEvent != null)
            {
                navigateEvent(this, e);
            }
            foreach (ScriptManagerProxy proxy in this.Proxies)
            {
                navigateEvent = proxy.NavigateEvent;
                if (navigateEvent != null)
                {
                    navigateEvent(this, e);
                }
            }
        }

        protected virtual void RaisePostBackEvent(string eventArgument)
        {
            this.LoadHistoryState(eventArgument);
        }

        protected virtual void RaisePostDataChangedEvent()
        {
        }

        public static void RegisterArrayDeclaration(System.Web.UI.Control control, string arrayName, string arrayValue)
        {
            ScriptRegistrationManager.RegisterArrayDeclaration(control, arrayName, arrayValue);
        }

        public static void RegisterArrayDeclaration(Page page, string arrayName, string arrayValue)
        {
            ScriptRegistrationManager.RegisterArrayDeclaration(page, arrayName, arrayValue);
        }

        public void RegisterAsyncPostBackControl(System.Web.UI.Control control)
        {
            this.PageRequestManager.RegisterAsyncPostBackControl(control);
        }

        public static void RegisterClientScriptBlock(System.Web.UI.Control control, Type type, string key, string script, bool addScriptTags)
        {
            ScriptRegistrationManager.RegisterClientScriptBlock(control, type, key, script, addScriptTags);
        }

        public static void RegisterClientScriptBlock(Page page, Type type, string key, string script, bool addScriptTags)
        {
            ScriptRegistrationManager.RegisterClientScriptBlock(page, type, key, script, addScriptTags);
        }

        internal virtual void RegisterClientScriptBlockInternal(System.Web.UI.Control control, Type type, string key, string script, bool addScriptTags)
        {
            RegisterClientScriptBlock(control, type, key, script, addScriptTags);
        }

        public static void RegisterClientScriptInclude(System.Web.UI.Control control, Type type, string key, string url)
        {
            ScriptRegistrationManager.RegisterClientScriptInclude(control, type, key, url);
        }

        public static void RegisterClientScriptInclude(Page page, Type type, string key, string url)
        {
            ScriptRegistrationManager.RegisterClientScriptInclude(page, type, key, url);
        }

        internal virtual void RegisterClientScriptIncludeInternal(System.Web.UI.Control control, Type type, string key, string url)
        {
            RegisterClientScriptInclude(control, type, key, url);
        }

        public static void RegisterClientScriptResource(System.Web.UI.Control control, Type type, string resourceName)
        {
            ScriptRegistrationManager.RegisterClientScriptResource(control, type, resourceName);
        }

        public static void RegisterClientScriptResource(Page page, Type type, string resourceName)
        {
            ScriptRegistrationManager.RegisterClientScriptResource(page, type, resourceName);
        }

        public void RegisterDataItem(System.Web.UI.Control control, string dataItem)
        {
            this.RegisterDataItem(control, dataItem, false);
        }

        public void RegisterDataItem(System.Web.UI.Control control, string dataItem, bool isJsonSerialized)
        {
            this.PageRequestManager.RegisterDataItem(control, dataItem, isJsonSerialized);
        }

        public void RegisterDispose(System.Web.UI.Control control, string disposeScript)
        {
            if (this.SupportsPartialRendering)
            {
                this.ScriptRegistration.RegisterDispose(control, disposeScript);
            }
        }

        public static void RegisterExpandoAttribute(System.Web.UI.Control control, string controlId, string attributeName, string attributeValue, bool encode)
        {
            ScriptRegistrationManager.RegisterExpandoAttribute(control, controlId, attributeName, attributeValue, encode);
        }

        public void RegisterExtenderControl<TExtenderControl>(TExtenderControl extenderControl, System.Web.UI.Control targetControl) where TExtenderControl: System.Web.UI.Control, IExtenderControl
        {
            this.ScriptControlManager.RegisterExtenderControl<TExtenderControl>(extenderControl, targetControl);
        }

        private void RegisterGlobalizationScriptBlock()
        {
            if (this.EnableScriptGlobalization)
            {
                string clientCultureScriptBlock = ClientCultureInfo.GetClientCultureScriptBlock(CultureInfo.CurrentCulture);
                if (clientCultureScriptBlock != null)
                {
                    ScriptRegistrationManager.RegisterClientScriptBlock(this, typeof(ScriptManager), "CultureInfo", clientCultureScriptBlock, true);
                }
            }
        }

        public static void RegisterHiddenField(System.Web.UI.Control control, string hiddenFieldName, string hiddenFieldInitialValue)
        {
            ScriptRegistrationManager.RegisterHiddenField(control, hiddenFieldName, hiddenFieldInitialValue);
        }

        public static void RegisterHiddenField(Page page, string hiddenFieldName, string hiddenFieldInitialValue)
        {
            ScriptRegistrationManager.RegisterHiddenField(page, hiddenFieldName, hiddenFieldInitialValue);
        }

        public static void RegisterOnSubmitStatement(System.Web.UI.Control control, Type type, string key, string script)
        {
            ScriptRegistrationManager.RegisterOnSubmitStatement(control, type, key, script);
        }

        public static void RegisterOnSubmitStatement(Page page, Type type, string key, string script)
        {
            ScriptRegistrationManager.RegisterOnSubmitStatement(page, type, key, script);
        }

        public void RegisterPostBackControl(System.Web.UI.Control control)
        {
            this.PageRequestManager.RegisterPostBackControl(control);
        }

        public void RegisterScriptControl<TScriptControl>(TScriptControl scriptControl) where TScriptControl: System.Web.UI.Control, IScriptControl
        {
            this.ScriptControlManager.RegisterScriptControl<TScriptControl>(scriptControl);
        }

        public void RegisterScriptDescriptors(IExtenderControl extenderControl)
        {
            this.ScriptControlManager.RegisterScriptDescriptors(extenderControl);
        }

        public void RegisterScriptDescriptors(IScriptControl scriptControl)
        {
            this.ScriptControlManager.RegisterScriptDescriptors(scriptControl);
        }

        private void RegisterScripts()
        {
            List<ScriptReferenceBase> scripts = new List<ScriptReferenceBase>();
            this.AddScriptCollections(scripts, this._proxies);
            this.ScriptControlManager.AddScriptReferences(scripts);
            this.AddFrameworkScripts(scripts);
            foreach (ScriptReferenceBase base2 in scripts)
            {
                ScriptReference script = base2 as ScriptReference;
                if (script != null)
                {
                    this.OnResolveScriptReference(new ScriptReferenceEventArgs(script));
                }
                else
                {
                    CompositeScriptReference compositeScript = base2 as CompositeScriptReference;
                    if (compositeScript != null)
                    {
                        this.OnResolveCompositeScriptReference(new CompositeScriptReferenceEventArgs(compositeScript));
                    }
                }
            }
            List<ScriptReferenceBase> uniqueScripts = RemoveDuplicates(scripts);
            this.RegisterUniqueScripts(uniqueScripts);
        }

        private void RegisterServices()
        {
            if (this._services != null)
            {
                foreach (ServiceReference reference in this._services)
                {
                    reference.Register(this, this.Context, this, this.IsDebuggingEnabled);
                }
            }
            if (this._proxies != null)
            {
                foreach (ScriptManagerProxy proxy in this._proxies)
                {
                    proxy.RegisterServices(this);
                }
            }
            if (this.EnablePageMethods)
            {
                string str = PageClientProxyGenerator.GetClientProxyScript(this.Context, this.IPage, this.IsDebuggingEnabled);
                if (!string.IsNullOrEmpty(str))
                {
                    this.RegisterClientScriptBlockInternal(this, typeof(ScriptManager), str, str, true);
                }
            }
        }

        public static void RegisterStartupScript(System.Web.UI.Control control, Type type, string key, string script, bool addScriptTags)
        {
            ScriptRegistrationManager.RegisterStartupScript(control, type, key, script, addScriptTags);
        }

        public static void RegisterStartupScript(Page page, Type type, string key, string script, bool addScriptTags)
        {
            ScriptRegistrationManager.RegisterStartupScript(page, type, key, script, addScriptTags);
        }

        internal virtual void RegisterStartupScriptInternal(System.Web.UI.Control control, Type type, string key, string script, bool addScriptTags)
        {
            RegisterStartupScript(control, type, key, script, addScriptTags);
        }

        private void RegisterUniqueScripts(List<ScriptReferenceBase> uniqueScripts)
        {
            bool flag = false;
            bool loadScriptsBeforeUI = this.LoadScriptsBeforeUI;
            bool isDebuggingEnabled = this.IsDebuggingEnabled;
            foreach (ScriptReferenceBase base2 in uniqueScripts)
            {
                string url = base2.GetUrl(this, this.Zip);
                if (loadScriptsBeforeUI || base2.AlwaysLoadBeforeUI)
                {
                    this.RegisterClientScriptIncludeInternal(base2.ContainingControl, typeof(ScriptManager), url, url);
                }
                else
                {
                    string script = "\r\n<script src=\"" + HttpUtility.HtmlAttributeEncode(url) + "\" type=\"text/javascript\"></script>";
                    this.RegisterStartupScriptInternal(base2.ContainingControl, typeof(ScriptManager), url, script, false);
                }
                if (!flag && base2.IsFromSystemWebExtensions())
                {
                    if (isDebuggingEnabled && !this.IsInAsyncPostBack)
                    {
                        this.AddFrameworkLoadedCheck();
                    }
                    this.ConfigureApplicationServices();
                    flag = true;
                }
            }
        }

        internal static List<ScriptReferenceBase> RemoveDuplicates(List<ScriptReferenceBase> scripts)
        {
            int count = scripts.Count;
            switch (count)
            {
                case 1:
                    return scripts;

                case 2:
                {
                    ScriptReference reference = scripts[0] as ScriptReference;
                    ScriptReference reference2 = scripts[1] as ScriptReference;
                    if (((reference != null) && (reference2 != null)) && ((reference.Name != reference2.Name) || (reference.Assembly != reference2.Assembly)))
                    {
                        return scripts;
                    }
                    break;
                }
            }
            HybridDictionary dictionary = new HybridDictionary(count);
            List<ScriptReferenceBase> list = new List<ScriptReferenceBase>(count);
            foreach (ScriptReferenceBase base2 in scripts)
            {
                CompositeScriptReference reference3 = base2 as CompositeScriptReference;
                if (reference3 != null)
                {
                    foreach (ScriptReference reference4 in reference3.Scripts)
                    {
                        Pair<string, Assembly> key = string.IsNullOrEmpty(reference4.Name) ? new Pair<string, Assembly>(reference4.Path, null) : new Pair<string, Assembly>(reference4.Name, reference4.GetAssembly());
                        if (dictionary.Contains(key))
                        {
                            throw new InvalidOperationException(AtlasWeb.ScriptManager_CannotRegisterScriptInMultipleCompositeReferences);
                        }
                        dictionary.Add(key, reference4);
                    }
                }
            }
            foreach (ScriptReferenceBase base3 in scripts)
            {
                CompositeScriptReference item = base3 as CompositeScriptReference;
                if (item != null)
                {
                    list.Add(item);
                }
                else
                {
                    ScriptReference reference6 = base3 as ScriptReference;
                    if (reference6 != null)
                    {
                        Pair<string, Assembly> pair2 = string.IsNullOrEmpty(reference6.Name) ? new Pair<string, Assembly>(reference6.Path, null) : new Pair<string, Assembly>(reference6.Name, reference6.GetAssembly());
                        if (!dictionary.Contains(pair2))
                        {
                            if (reference6.IsStaticReference)
                            {
                                dictionary.Add(pair2, reference6);
                            }
                            list.Add(reference6);
                        }
                    }
                }
            }
            return list;
        }

        protected internal override void Render(HtmlTextWriter writer)
        {
            this.PageRequestManager.Render(writer);
            if (!this.IsInAsyncPostBack)
            {
                this.IPage.ClientScript.RegisterStartupScript(typeof(ScriptManager), "AppInitialize", "Sys.Application.initialize();\r\n", true);
                if ((this.EnableHistory && !base.DesignMode) && (this.Page != null))
                {
                    writer.AddAttribute(HtmlTextWriterAttribute.Type, "hidden");
                    writer.AddAttribute(HtmlTextWriterAttribute.Name, this.UniqueID);
                    writer.AddAttribute(HtmlTextWriterAttribute.Id, this.ClientID);
                    writer.RenderBeginTag(HtmlTextWriterTag.Input);
                    writer.RenderEndTag();
                    string browser = this.Page.Request.Browser.Browser;
                    if (browser.Equals("IE", StringComparison.OrdinalIgnoreCase))
                    {
                        if (string.IsNullOrEmpty(this.Page.Title))
                        {
                            this.Page.Title = AtlasWeb.ScriptManager_PageUntitled;
                        }
                        string str2 = (this.EmptyPageUrl.Length == 0) ? ScriptResourceHandler.GetEmptyPageUrl(this.Page.Title) : (this.EmptyPageUrl + ((this.EmptyPageUrl.IndexOf('?') != -1) ? "&title=" : "?title=") + this.Page.Title);
                        writer.AddAttribute(HtmlTextWriterAttribute.Id, "__historyFrame");
                        writer.AddAttribute(HtmlTextWriterAttribute.Src, str2);
                        writer.AddStyleAttribute(HtmlTextWriterStyle.Display, "none");
                        writer.RenderBeginTag(HtmlTextWriterTag.Iframe);
                        writer.RenderEndTag();
                    }
                    if (browser.Equals("AppleMAC-Safari", StringComparison.OrdinalIgnoreCase))
                    {
                        string userAgent = this.Page.Request.UserAgent;
                        int index = userAgent.IndexOf(" AppleWebKit/", StringComparison.Ordinal);
                        if (index != -1)
                        {
                            int num3;
                            int num2 = userAgent.IndexOf('.', index + 13);
                            if (((num2 != -1) && int.TryParse(userAgent.Substring(index + 13, (num2 - index) - 13), out num3)) && (num3 <= 0x1a3))
                            {
                                writer.AddAttribute(HtmlTextWriterAttribute.Id, "__history");
                                writer.AddAttribute(HtmlTextWriterAttribute.Type, "text");
                                writer.AddStyleAttribute(HtmlTextWriterStyle.Display, "none");
                                writer.RenderBeginTag(HtmlTextWriterTag.Input);
                                writer.RenderEndTag();
                            }
                        }
                    }
                }
            }
            base.Render(writer);
        }

        public void SetFocus(string clientID)
        {
            this.PageRequestManager.SetFocus(clientID);
        }

        public void SetFocus(System.Web.UI.Control control)
        {
            this.PageRequestManager.SetFocus(control);
        }

        private void SetPageTitle(string title)
        {
            if ((this.Page != null) && (this.Page.Header != null))
            {
                this.Page.Title = title;
            }
        }

        private void SetStateValue(string key, string value)
        {
            if (value == null)
            {
                if (this._initialState.ContainsKey(key))
                {
                    this._initialState.Remove(key);
                }
            }
            else if (this._initialState.ContainsKey(key))
            {
                this._initialState[key] = value;
            }
            else
            {
                this._initialState.Add(key, value);
            }
        }

        string IClientUrlResolver.get_AppRelativeTemplateSourceDirectory() => 
            base.AppRelativeTemplateSourceDirectory;

        bool IPostBackDataHandler.LoadPostData(string postDataKey, NameValueCollection postCollection) => 
            this.LoadPostData(postDataKey, postCollection);

        void IPostBackDataHandler.RaisePostDataChangedEvent()
        {
            this.RaisePostDataChangedEvent();
        }

        void IPostBackEventHandler.RaisePostBackEvent(string eventArgument)
        {
            this.RaisePostBackEvent(eventArgument);
        }

        void IScriptManager.RegisterArrayDeclaration(System.Web.UI.Control control, string arrayName, string arrayValue)
        {
            RegisterArrayDeclaration(control, arrayName, arrayValue);
        }

        void IScriptManager.RegisterClientScriptBlock(System.Web.UI.Control control, Type type, string key, string script, bool addScriptTags)
        {
            RegisterClientScriptBlock(control, type, key, script, addScriptTags);
        }

        void IScriptManager.RegisterClientScriptInclude(System.Web.UI.Control control, Type type, string key, string url)
        {
            RegisterClientScriptInclude(control, type, key, url);
        }

        void IScriptManager.RegisterClientScriptResource(System.Web.UI.Control control, Type type, string resourceName)
        {
            RegisterClientScriptResource(control, type, resourceName);
        }

        void IScriptManager.RegisterDispose(System.Web.UI.Control control, string disposeScript)
        {
            this.RegisterDispose(control, disposeScript);
        }

        void IScriptManager.RegisterExpandoAttribute(System.Web.UI.Control control, string controlId, string attributeName, string attributeValue, bool encode)
        {
            RegisterExpandoAttribute(control, controlId, attributeName, attributeValue, encode);
        }

        void IScriptManager.RegisterHiddenField(System.Web.UI.Control control, string hiddenFieldName, string hiddenFieldValue)
        {
            RegisterHiddenField(control, hiddenFieldName, hiddenFieldValue);
        }

        void IScriptManager.RegisterOnSubmitStatement(System.Web.UI.Control control, Type type, string key, string script)
        {
            RegisterOnSubmitStatement(control, type, key, script);
        }

        void IScriptManager.RegisterPostBackControl(System.Web.UI.Control control)
        {
            this.RegisterPostBackControl(control);
        }

        void IScriptManager.RegisterStartupScript(System.Web.UI.Control control, Type type, string key, string script, bool addScriptTags)
        {
            RegisterStartupScript(control, type, key, script, addScriptTags);
        }

        void IScriptManager.SetFocusInternal(string clientID)
        {
            this.PageRequestManager.SetFocusInternal(clientID);
        }

        void IScriptManagerInternal.RegisterProxy(ScriptManagerProxy proxy)
        {
            if (!this.Proxies.Contains(proxy))
            {
                this.Proxies.Add(proxy);
            }
        }

        void IScriptManagerInternal.RegisterUpdatePanel(UpdatePanel updatePanel)
        {
            this.PageRequestManager.RegisterUpdatePanel(updatePanel);
        }

        void IScriptManagerInternal.UnregisterUpdatePanel(UpdatePanel updatePanel)
        {
            this.PageRequestManager.UnregisterUpdatePanel(updatePanel);
        }

        [DefaultValue(true), Category("Behavior"), ResourceDescription("ScriptManager_AllowCustomErrorsRedirect")]
        public bool AllowCustomErrorsRedirect
        {
            get => 
                this._allowCustomErrorsRedirect;
            set
            {
                this._allowCustomErrorsRedirect = value;
            }
        }

        private ICompilationSection AppLevelCompilationSection
        {
            get
            {
                if (this._appLevelCompilationSection != null)
                {
                    return this._appLevelCompilationSection;
                }
                return AppLevelCompilationSectionCache.Instance;
            }
        }

        [DefaultValue(""), Category("Behavior"), ResourceDescription("ScriptManager_AsyncPostBackErrorMessage")]
        public string AsyncPostBackErrorMessage
        {
            get
            {
                if (this._asyncPostBackErrorMessage == null)
                {
                    return string.Empty;
                }
                return this._asyncPostBackErrorMessage;
            }
            set
            {
                this._asyncPostBackErrorMessage = value;
            }
        }

        [Browsable(false)]
        public string AsyncPostBackSourceElementID =>
            this.PageRequestManager.AsyncPostBackSourceElementID;

        [ResourceDescription("ScriptManager_AsyncPostBackTimeout"), Category("Behavior"), DefaultValue(90)]
        public int AsyncPostBackTimeout
        {
            get => 
                this._asyncPostBackTimeout;
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException("value");
                }
                this._asyncPostBackTimeout = value;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content), DefaultValue((string) null), MergableProperty(false), PersistenceMode(PersistenceMode.InnerProperty), ResourceDescription("ScriptManager_AuthenticationService"), Category("Behavior")]
        public AuthenticationServiceManager AuthenticationService
        {
            get
            {
                if (this._authenticationServiceManager == null)
                {
                    this._authenticationServiceManager = new AuthenticationServiceManager();
                }
                return this._authenticationServiceManager;
            }
        }

        [DefaultValue(""), Category("Behavior"), ResourceDescription("ScriptManager_ClientNavigateHandler")]
        public string ClientNavigateHandler
        {
            get => 
                (this._clientNavigateHandler ?? string.Empty);
            set
            {
                this._clientNavigateHandler = value;
            }
        }

        [MergableProperty(false), PersistenceMode(PersistenceMode.InnerProperty), Category("Behavior"), ResourceDescription("ScriptManager_CompositeScript"), DefaultValue((string) null), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public CompositeScriptReference CompositeScript
        {
            get
            {
                if (this._compositeScript == null)
                {
                    this._compositeScript = new CompositeScriptReference();
                }
                return this._compositeScript;
            }
        }

        internal IControl Control
        {
            get
            {
                if (this._control != null)
                {
                    return this._control;
                }
                return this;
            }
        }

        internal ICustomErrorsSection CustomErrorsSection
        {
            [SecurityCritical]
            get
            {
                if (this._customErrorsSection != null)
                {
                    return this._customErrorsSection;
                }
                return GetCustomErrorsSectionWithAssert();
            }
        }

        private IDeploymentSection DeploymentSection
        {
            get
            {
                if (this._deploymentSection != null)
                {
                    return this._deploymentSection;
                }
                return DeploymentSectionCache.Instance;
            }
        }

        internal bool DeploymentSectionRetail =>
            this.DeploymentSection.Retail;

        [Editor(typeof(UrlEditor), typeof(UITypeEditor)), Category("Appearance"), DefaultValue(""), UrlProperty, ResourceDescription("ScriptManager_EmptyPageUrl")]
        public virtual string EmptyPageUrl
        {
            get => 
                ((this.ViewState["EmptyPageUrl"] as string) ?? string.Empty);
            set
            {
                this.ViewState["EmptyPageUrl"] = value;
            }
        }

        [Category("Behavior"), DefaultValue(false), ResourceDescription("ScriptManager_EnableHistory")]
        public bool EnableHistory
        {
            get => 
                this._enableHistory;
            set
            {
                if (this._initCompleted)
                {
                    throw new InvalidOperationException(AtlasWeb.ScriptManager_CannotChangeEnableHistory);
                }
                this._enableHistory = value;
            }
        }

        [ResourceDescription("ScriptManager_EnablePageMethods"), DefaultValue(false), Category("Behavior")]
        public bool EnablePageMethods
        {
            get => 
                this._enablePageMethods;
            set
            {
                this._enablePageMethods = value;
            }
        }

        [DefaultValue(true), ResourceDescription("ScriptManager_EnablePartialRendering"), Category("Behavior")]
        public bool EnablePartialRendering
        {
            get => 
                this._enablePartialRendering;
            set
            {
                if (this._initCompleted)
                {
                    throw new InvalidOperationException(AtlasWeb.ScriptManager_CannotChangeEnablePartialRendering);
                }
                this._enablePartialRendering = value;
            }
        }

        [Category("Behavior"), DefaultValue(false), ResourceDescription("ScriptManager_EnableScriptGlobalization")]
        public bool EnableScriptGlobalization
        {
            get => 
                this._enableScriptGlobalization;
            set
            {
                if (this._initCompleted)
                {
                    throw new InvalidOperationException(AtlasWeb.ScriptManager_CannotChangeEnableScriptGlobalization);
                }
                this._enableScriptGlobalization = value;
            }
        }

        [DefaultValue(true), ResourceDescription("ScriptManager_EnableScriptLocalization"), Category("Behavior")]
        public bool EnableScriptLocalization
        {
            get => 
                this._enableScriptLocalization;
            set
            {
                this._enableScriptLocalization = value;
            }
        }

        [DefaultValue(true), ResourceDescription("ScriptManager_EnableSecureHistoryState"), Category("Behavior")]
        public bool EnableSecureHistoryState
        {
            get => 
                this._enableSecureHistoryState;
            set
            {
                this._enableSecureHistoryState = value;
            }
        }

        internal bool HasAuthenticationServiceManager =>
            (this._authenticationServiceManager != null);

        internal bool HasProfileServiceManager =>
            (this._profileServiceManager != null);

        internal bool HasRoleServiceManager =>
            (this._roleServiceManager != null);

        internal System.Web.UI.IPage IPage
        {
            get
            {
                if (this._page != null)
                {
                    return this._page;
                }
                Page page = this.Page;
                if (page == null)
                {
                    throw new InvalidOperationException(AtlasWeb.Common_PageCannotBeNull);
                }
                return new PageWrapper(page);
            }
        }

        [Browsable(false)]
        public bool IsDebuggingEnabled
        {
            get
            {
                if (this.DeploymentSectionRetail)
                {
                    return false;
                }
                if ((this.ScriptMode != System.Web.UI.ScriptMode.Auto) && (this.ScriptMode != System.Web.UI.ScriptMode.Inherit))
                {
                    return (this.ScriptMode == System.Web.UI.ScriptMode.Debug);
                }
                return this.AppLevelCompilationSection.Debug;
            }
        }

        [Browsable(false)]
        public bool IsInAsyncPostBack =>
            this._isInAsyncPostBack;

        [Browsable(false)]
        public bool IsNavigating =>
            this._isNavigating;

        [DefaultValue(true), Category("Behavior"), ResourceDescription("ScriptManager_LoadScriptsBeforeUI")]
        public bool LoadScriptsBeforeUI
        {
            get => 
                this._loadScriptsBeforeUI;
            set
            {
                this._loadScriptsBeforeUI = value;
            }
        }

        private System.Web.UI.PageRequestManager PageRequestManager
        {
            get
            {
                if (this._pageRequestManager == null)
                {
                    this._pageRequestManager = new System.Web.UI.PageRequestManager(this);
                }
                return this._pageRequestManager;
            }
        }

        [MergableProperty(false), ResourceDescription("ScriptManager_ProfileService"), Category("Behavior"), DefaultValue((string) null), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), PersistenceMode(PersistenceMode.InnerProperty)]
        public ProfileServiceManager ProfileService
        {
            get
            {
                if (this._profileServiceManager == null)
                {
                    this._profileServiceManager = new ProfileServiceManager();
                }
                return this._profileServiceManager;
            }
        }

        internal List<ScriptManagerProxy> Proxies
        {
            get
            {
                if (this._proxies == null)
                {
                    this._proxies = new List<ScriptManagerProxy>();
                }
                return this._proxies;
            }
        }

        [Category("Behavior"), DefaultValue((string) null), MergableProperty(false), PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), ResourceDescription("ScriptManager_RoleService")]
        public RoleServiceManager RoleService
        {
            get
            {
                if (this._roleServiceManager == null)
                {
                    this._roleServiceManager = new RoleServiceManager();
                }
                return this._roleServiceManager;
            }
        }

        internal System.Web.UI.ScriptControlManager ScriptControlManager
        {
            get
            {
                if (this._scriptControlManager == null)
                {
                    this._scriptControlManager = new System.Web.UI.ScriptControlManager(this);
                }
                return this._scriptControlManager;
            }
        }

        [DefaultValue(0), ResourceDescription("ScriptManager_ScriptMode"), Category("Behavior")]
        public System.Web.UI.ScriptMode ScriptMode
        {
            get => 
                this._scriptMode;
            set
            {
                if ((value < System.Web.UI.ScriptMode.Auto) || (value > System.Web.UI.ScriptMode.Release))
                {
                    throw new ArgumentOutOfRangeException("value");
                }
                this._scriptMode = value;
            }
        }

        [Category("Behavior"), DefaultValue(""), ResourceDescription("ScriptManager_ScriptPath")]
        public string ScriptPath
        {
            get
            {
                if (this._scriptPath != null)
                {
                    return this._scriptPath;
                }
                return string.Empty;
            }
            set
            {
                this._scriptPath = value;
            }
        }

        internal ScriptRegistrationManager ScriptRegistration
        {
            get
            {
                if (this._scriptRegistration == null)
                {
                    this._scriptRegistration = new ScriptRegistrationManager(this);
                }
                return this._scriptRegistration;
            }
        }

        [DefaultValue((string) null), PersistenceMode(PersistenceMode.InnerProperty), ResourceDescription("ScriptManager_Scripts"), Category("Behavior"), Editor("System.Web.UI.Design.CollectionEditorBase, System.Web.Extensions.Design, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35", typeof(UITypeEditor)), MergableProperty(false)]
        public ScriptReferenceCollection Scripts
        {
            get
            {
                if (this._scripts == null)
                {
                    this._scripts = new ScriptReferenceCollection();
                }
                return this._scripts;
            }
        }

        [Editor("System.Web.UI.Design.CollectionEditorBase, System.Web.Extensions.Design, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35", typeof(UITypeEditor)), ResourceDescription("ScriptManager_Services"), MergableProperty(false), Category("Behavior"), DefaultValue((string) null), PersistenceMode(PersistenceMode.InnerProperty)]
        public ServiceReferenceCollection Services
        {
            get
            {
                if (this._services == null)
                {
                    this._services = new ServiceReferenceCollection();
                }
                return this._services;
            }
        }

        [DefaultValue(true), Browsable(false)]
        public bool SupportsPartialRendering
        {
            get
            {
                if (!this.EnablePartialRendering)
                {
                    return false;
                }
                return this._supportsPartialRendering;
            }
            set
            {
                if (!this.EnablePartialRendering)
                {
                    throw new InvalidOperationException(AtlasWeb.ScriptManager_CannotSetSupportsPartialRenderingWhenDisabled);
                }
                if (this._initCompleted)
                {
                    throw new InvalidOperationException(AtlasWeb.ScriptManager_CannotChangeSupportsPartialRendering);
                }
                this._supportsPartialRendering = value;
                this._supportsPartialRenderingSetByUser = true;
            }
        }

        HttpContextBase IControl.Context =>
            new System.Web.HttpContextWrapper(this.Context);

        bool IControl.DesignMode =>
            base.DesignMode;

        [EditorBrowsable(EditorBrowsableState.Never), Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override bool Visible
        {
            get => 
                base.Visible;
            set
            {
                throw new NotImplementedException();
            }
        }

        internal bool Zip
        {
            get
            {
                if (!this._zipSet)
                {
                    this._zip = HeaderUtility.IsEncodingInAcceptList(this.IPage.Request.Headers["Accept-encoding"], "gzip");
                    this._zipSet = true;
                }
                return this._zip;
            }
        }

        private class StatePersister : PageStatePersister
        {
            public StatePersister(Page page) : base(page)
            {
            }

            public object Deserialize(string serialized) => 
                base.StateFormatter.Deserialize(serialized);

            public override void Load()
            {
                throw new NotImplementedException();
            }

            public override void Save()
            {
                throw new NotImplementedException();
            }

            public string Serialize(object state) => 
                base.StateFormatter.Serialize(state);
        }
    }
}

