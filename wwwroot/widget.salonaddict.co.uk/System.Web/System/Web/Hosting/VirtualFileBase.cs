namespace System.Web.Hosting
{
    using System;
    using System.Security.Permissions;
    using System.Web;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public abstract class VirtualFileBase : MarshalByRefObject
    {
        internal System.Web.VirtualPath _virtualPath;

        protected VirtualFileBase()
        {
        }

        public override object InitializeLifetimeService() => 
            null;

        public abstract bool IsDirectory { get; }

        public virtual string Name =>
            this._virtualPath.FileName;

        public string VirtualPath =>
            this._virtualPath.VirtualPathString;

        internal System.Web.VirtualPath VirtualPathObject =>
            this._virtualPath;
    }
}

