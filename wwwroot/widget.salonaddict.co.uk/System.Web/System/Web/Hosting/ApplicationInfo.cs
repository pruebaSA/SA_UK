namespace System.Web.Hosting
{
    using System;
    using System.Security.Permissions;
    using System.Web;

    [Serializable, AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public sealed class ApplicationInfo
    {
        private string _id;
        private string _physicalPath;
        private System.Web.VirtualPath _virtualPath;

        internal ApplicationInfo(string id, System.Web.VirtualPath virtualPath, string physicalPath)
        {
            this._id = id;
            this._virtualPath = virtualPath;
            this._physicalPath = physicalPath;
        }

        public string ID =>
            this._id;

        public string PhysicalPath =>
            this._physicalPath;

        public string VirtualPath =>
            this._virtualPath.VirtualPathString;
    }
}

