namespace System.Web.Profile
{
    using System;
    using System.Security.Permissions;
    using System.Web;

    [AttributeUsage(AttributeTargets.Property), AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public sealed class CustomProviderDataAttribute : Attribute
    {
        private string _CustomProviderData;

        public CustomProviderDataAttribute(string customProviderData)
        {
            this._CustomProviderData = customProviderData;
        }

        public override bool IsDefaultAttribute() => 
            string.IsNullOrEmpty(this._CustomProviderData);

        public string CustomProviderData =>
            this._CustomProviderData;
    }
}

