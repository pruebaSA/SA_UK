namespace System.Web.Configuration
{
    using System.Configuration;
    using System.Security.Permissions;
    using System.Web;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public sealed class SystemWebExtensionsSectionGroup : ConfigurationSectionGroup
    {
        [ConfigurationProperty("scripting")]
        public ScriptingSectionGroup Scripting =>
            ((ScriptingSectionGroup) base.SectionGroups["scripting"]);
    }
}

