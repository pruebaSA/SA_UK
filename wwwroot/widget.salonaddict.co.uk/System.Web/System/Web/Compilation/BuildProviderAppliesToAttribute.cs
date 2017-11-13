namespace System.Web.Compilation
{
    using System;
    using System.Security.Permissions;
    using System.Web;

    [AttributeUsage(AttributeTargets.Class, AllowMultiple=false), AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public sealed class BuildProviderAppliesToAttribute : Attribute
    {
        private BuildProviderAppliesTo _appliesTo;

        public BuildProviderAppliesToAttribute(BuildProviderAppliesTo appliesTo)
        {
            this._appliesTo = appliesTo;
        }

        public BuildProviderAppliesTo AppliesTo =>
            this._appliesTo;
    }
}

