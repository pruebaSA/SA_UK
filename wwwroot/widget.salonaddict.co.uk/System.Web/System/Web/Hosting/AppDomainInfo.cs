namespace System.Web.Hosting
{
    using System;
    using System.Security.Permissions;
    using System.Web;

    [AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class AppDomainInfo : IAppDomainInfo
    {
        private string _id;
        private bool _isIdle;
        private string _physicalPath;
        private int _siteId;
        private string _virtualPath;

        internal AppDomainInfo(string id, string vpath, string physPath, int siteId, bool isIdle)
        {
            this._id = id;
            this._virtualPath = vpath;
            this._physicalPath = physPath;
            this._siteId = siteId;
            this._isIdle = isIdle;
        }

        public string GetId() => 
            this._id;

        public string GetPhysicalPath() => 
            this._physicalPath;

        public int GetSiteId() => 
            this._siteId;

        public string GetVirtualPath() => 
            this._virtualPath;

        public bool IsIdle() => 
            this._isIdle;
    }
}

