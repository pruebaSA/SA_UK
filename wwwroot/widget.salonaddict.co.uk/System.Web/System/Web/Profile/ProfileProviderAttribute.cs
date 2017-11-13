namespace System.Web.Profile
{
    using System;
    using System.Security.Permissions;
    using System.Web;

    [AttributeUsage(AttributeTargets.Property), AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public sealed class ProfileProviderAttribute : Attribute
    {
        private string _ProviderName;

        public ProfileProviderAttribute(string providerName)
        {
            this._ProviderName = providerName;
        }

        public string ProviderName =>
            this._ProviderName;
    }
}

