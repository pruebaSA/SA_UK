namespace System.Web.Hosting
{
    using System;
    using System.Collections;
    using System.Security.Permissions;
    using System.Web;

    [AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public abstract class VirtualDirectory : VirtualFileBase
    {
        protected VirtualDirectory(string virtualPath)
        {
            base._virtualPath = VirtualPath.CreateTrailingSlash(virtualPath);
        }

        public abstract IEnumerable Children { get; }

        public abstract IEnumerable Directories { get; }

        public abstract IEnumerable Files { get; }

        public override bool IsDirectory =>
            true;
    }
}

