namespace System.Web
{
    using System;
    using System.Security.Permissions;

    [AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class SiteMapResolveEventArgs : EventArgs
    {
        private HttpContext _context;
        private SiteMapProvider _provider;

        public SiteMapResolveEventArgs(HttpContext context, SiteMapProvider provider)
        {
            this._context = context;
            this._provider = provider;
        }

        public HttpContext Context =>
            this._context;

        public SiteMapProvider Provider =>
            this._provider;
    }
}

