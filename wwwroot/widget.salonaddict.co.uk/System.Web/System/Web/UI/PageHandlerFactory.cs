namespace System.Web.UI
{
    using System;
    using System.Security.Permissions;
    using System.Web;
    using System.Web.Compilation;

    [PermissionSet(SecurityAction.LinkDemand, Unrestricted=true), PermissionSet(SecurityAction.InheritanceDemand, Unrestricted=true)]
    public class PageHandlerFactory : IHttpHandlerFactory2, IHttpHandlerFactory
    {
        private bool _isInheritedInstance;

        protected internal PageHandlerFactory()
        {
            this._isInheritedInstance = base.GetType() != typeof(PageHandlerFactory);
        }

        public virtual IHttpHandler GetHandler(HttpContext context, string requestType, string virtualPath, string path) => 
            this.GetHandlerHelper(context, requestType, VirtualPath.CreateNonRelative(virtualPath), path);

        private IHttpHandler GetHandlerHelper(HttpContext context, string requestType, VirtualPath virtualPath, string physicalPath)
        {
            Page page = BuildManager.CreateInstanceFromVirtualPath(virtualPath, typeof(Page), context, true, true) as Page;
            if (page == null)
            {
                return null;
            }
            page.TemplateControlVirtualPath = virtualPath;
            return page;
        }

        public virtual void ReleaseHandler(IHttpHandler handler)
        {
        }

        IHttpHandler IHttpHandlerFactory2.GetHandler(HttpContext context, string requestType, VirtualPath virtualPath, string physicalPath)
        {
            if (this._isInheritedInstance)
            {
                return this.GetHandler(context, requestType, virtualPath.VirtualPathString, physicalPath);
            }
            return this.GetHandlerHelper(context, requestType, virtualPath, physicalPath);
        }
    }
}

