namespace System.Web.Hosting
{
    using System;
    using System.IO;
    using System.Security.Permissions;
    using System.Web;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public abstract class VirtualFile : VirtualFileBase
    {
        protected VirtualFile(string virtualPath)
        {
            base._virtualPath = VirtualPath.Create(virtualPath);
        }

        public abstract Stream Open();

        public override bool IsDirectory =>
            false;
    }
}

