namespace System.Web.Configuration
{
    using System.Configuration;
    using System.Security.Permissions;
    using System.Web;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public sealed class ScriptingWebServicesSectionGroup : ConfigurationSectionGroup
    {
        [ConfigurationProperty("authenticationService")]
        public ScriptingAuthenticationServiceSection AuthenticationService =>
            ((ScriptingAuthenticationServiceSection) base.Sections["authenticationService"]);

        [ConfigurationProperty("jsonSerialization")]
        public ScriptingJsonSerializationSection JsonSerialization =>
            ((ScriptingJsonSerializationSection) base.Sections["jsonSerialization"]);

        [ConfigurationProperty("profileService")]
        public ScriptingProfileServiceSection ProfileService =>
            ((ScriptingProfileServiceSection) base.Sections["profileService"]);

        [ConfigurationProperty("roleService")]
        public ScriptingRoleServiceSection RoleService =>
            ((ScriptingRoleServiceSection) base.Sections["roleService"]);
    }
}

