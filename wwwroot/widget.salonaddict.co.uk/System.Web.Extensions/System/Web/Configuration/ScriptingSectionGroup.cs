namespace System.Web.Configuration
{
    using System.Configuration;
    using System.Security.Permissions;
    using System.Web;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public sealed class ScriptingSectionGroup : ConfigurationSectionGroup
    {
        [ConfigurationProperty("scriptResourceHandler")]
        public ScriptingScriptResourceHandlerSection ScriptResourceHandler =>
            ((ScriptingScriptResourceHandlerSection) base.Sections["scriptResourceHandler"]);

        [ConfigurationProperty("webServices")]
        public ScriptingWebServicesSectionGroup WebServices =>
            ((ScriptingWebServicesSectionGroup) base.SectionGroups["webServices"]);
    }
}

