namespace System.Configuration
{
    using System;
    using System.Security;
    using System.Security.Permissions;

    [Serializable, AttributeUsage(AttributeTargets.All, AllowMultiple=true, Inherited=false)]
    public sealed class ConfigurationPermissionAttribute : CodeAccessSecurityAttribute
    {
        public ConfigurationPermissionAttribute(SecurityAction action) : base(action)
        {
        }

        public override IPermission CreatePermission() => 
            new ConfigurationPermission(base.Unrestricted ? PermissionState.Unrestricted : PermissionState.None);
    }
}

