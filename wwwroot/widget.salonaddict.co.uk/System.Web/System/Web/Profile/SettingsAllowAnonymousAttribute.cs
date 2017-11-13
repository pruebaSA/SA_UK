namespace System.Web.Profile
{
    using System;
    using System.Security.Permissions;
    using System.Web;

    [AttributeUsage(AttributeTargets.Property), AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public sealed class SettingsAllowAnonymousAttribute : Attribute
    {
        private bool _Allow;

        public SettingsAllowAnonymousAttribute(bool allow)
        {
            this._Allow = allow;
        }

        public override bool IsDefaultAttribute() => 
            !this._Allow;

        public bool Allow =>
            this._Allow;
    }
}

