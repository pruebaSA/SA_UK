namespace System.Web.Configuration
{
    using System.Configuration;
    using System.Security.Permissions;
    using System.Web;
    using System.Web.Services.Configuration;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public sealed class SystemWebSectionGroup : ConfigurationSectionGroup
    {
        [ConfigurationProperty("anonymousIdentification")]
        public AnonymousIdentificationSection AnonymousIdentification =>
            ((AnonymousIdentificationSection) base.Sections["anonymousIdentification"]);

        [ConfigurationProperty("authentication")]
        public AuthenticationSection Authentication =>
            ((AuthenticationSection) base.Sections["authentication"]);

        [ConfigurationProperty("authorization")]
        public AuthorizationSection Authorization =>
            ((AuthorizationSection) base.Sections["authorization"]);

        [ConfigurationProperty("browserCaps")]
        public DefaultSection BrowserCaps =>
            ((DefaultSection) base.Sections["browserCaps"]);

        [ConfigurationProperty("clientTarget")]
        public ClientTargetSection ClientTarget =>
            ((ClientTargetSection) base.Sections["clientTarget"]);

        [ConfigurationProperty("compilation")]
        public CompilationSection Compilation =>
            ((CompilationSection) base.Sections["compilation"]);

        [ConfigurationProperty("customErrors")]
        public CustomErrorsSection CustomErrors =>
            ((CustomErrorsSection) base.Sections["customErrors"]);

        [ConfigurationProperty("deployment")]
        public DeploymentSection Deployment =>
            ((DeploymentSection) base.Sections["deployment"]);

        [ConfigurationProperty("deviceFilters")]
        public DefaultSection DeviceFilters =>
            ((DefaultSection) base.Sections["deviceFilters"]);

        [ConfigurationProperty("globalization")]
        public GlobalizationSection Globalization =>
            ((GlobalizationSection) base.Sections["globalization"]);

        [ConfigurationProperty("healthMonitoring")]
        public HealthMonitoringSection HealthMonitoring =>
            ((HealthMonitoringSection) base.Sections["healthMonitoring"]);

        [ConfigurationProperty("hostingEnvironment")]
        public HostingEnvironmentSection HostingEnvironment =>
            ((HostingEnvironmentSection) base.Sections["hostingEnvironment"]);

        [ConfigurationProperty("httpCookies")]
        public HttpCookiesSection HttpCookies =>
            ((HttpCookiesSection) base.Sections["httpCookies"]);

        [ConfigurationProperty("httpHandlers")]
        public HttpHandlersSection HttpHandlers =>
            ((HttpHandlersSection) base.Sections["httpHandlers"]);

        [ConfigurationProperty("httpModules")]
        public HttpModulesSection HttpModules =>
            ((HttpModulesSection) base.Sections["httpModules"]);

        [ConfigurationProperty("httpRuntime")]
        public HttpRuntimeSection HttpRuntime =>
            ((HttpRuntimeSection) base.Sections["httpRuntime"]);

        [ConfigurationProperty("identity")]
        public IdentitySection Identity =>
            ((IdentitySection) base.Sections["identity"]);

        [ConfigurationProperty("machineKey")]
        public MachineKeySection MachineKey =>
            ((MachineKeySection) base.Sections["machineKey"]);

        [ConfigurationProperty("membership")]
        public MembershipSection Membership =>
            ((MembershipSection) base.Sections["membership"]);

        [ConfigurationProperty("mobileControls")]
        public ConfigurationSection MobileControls =>
            base.Sections["mobileControls"];

        [ConfigurationProperty("pages")]
        public PagesSection Pages =>
            ((PagesSection) base.Sections["pages"]);

        [ConfigurationProperty("processModel")]
        public ProcessModelSection ProcessModel =>
            ((ProcessModelSection) base.Sections["processModel"]);

        [ConfigurationProperty("profile")]
        public ProfileSection Profile =>
            ((ProfileSection) base.Sections["profile"]);

        [ConfigurationProperty("protocols")]
        public DefaultSection Protocols =>
            ((DefaultSection) base.Sections["protocols"]);

        [ConfigurationProperty("roleManager")]
        public RoleManagerSection RoleManager =>
            ((RoleManagerSection) base.Sections["roleManager"]);

        [ConfigurationProperty("securityPolicy")]
        public SecurityPolicySection SecurityPolicy =>
            ((SecurityPolicySection) base.Sections["securityPolicy"]);

        [ConfigurationProperty("sessionState")]
        public SessionStateSection SessionState =>
            ((SessionStateSection) base.Sections["sessionState"]);

        [ConfigurationProperty("siteMap")]
        public SiteMapSection SiteMap =>
            ((SiteMapSection) base.Sections["siteMap"]);

        [ConfigurationProperty("trace")]
        public TraceSection Trace =>
            ((TraceSection) base.Sections["trace"]);

        [ConfigurationProperty("trust")]
        public TrustSection Trust =>
            ((TrustSection) base.Sections["trust"]);

        [ConfigurationProperty("urlMappings")]
        public UrlMappingsSection UrlMappings =>
            ((UrlMappingsSection) base.Sections["urlMappings"]);

        [ConfigurationProperty("webControls")]
        public WebControlsSection WebControls =>
            ((WebControlsSection) base.Sections["webControls"]);

        [ConfigurationProperty("webParts")]
        public WebPartsSection WebParts =>
            ((WebPartsSection) base.Sections["WebParts"]);

        [ConfigurationProperty("webServices")]
        public WebServicesSection WebServices =>
            ((WebServicesSection) base.Sections["webServices"]);

        [ConfigurationProperty("xhtmlConformance")]
        public XhtmlConformanceSection XhtmlConformance =>
            ((XhtmlConformanceSection) base.Sections["xhtmlConformance"]);
    }
}

