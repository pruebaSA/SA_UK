namespace System.Web.Compilation
{
    using System;
    using System.Security.Permissions;
    using System.Web;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public abstract class ResourceProviderFactory
    {
        protected ResourceProviderFactory()
        {
        }

        public abstract IResourceProvider CreateGlobalResourceProvider(string classKey);
        public abstract IResourceProvider CreateLocalResourceProvider(string virtualPath);
    }
}

