namespace System.Web
{
    using System;
    using System.Collections.Specialized;
    using System.Security.Permissions;
    using System.Web.Util;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class DefaultHttpHandler : IHttpAsyncHandler, IHttpHandler
    {
        private HttpContext _context;
        private NameValueCollection _executeUrlHeaders;

        public virtual IAsyncResult BeginProcessRequest(HttpContext context, AsyncCallback callback, object state)
        {
            if (HttpRuntime.UseIntegratedPipeline)
            {
                throw new PlatformNotSupportedException(System.Web.SR.GetString("Method_Not_Supported_By_Iis_Integrated_Mode", new object[] { "DefaultHttpHandler.BeginProcessRequest" }));
            }
            this._context = context;
            HttpResponse response = this._context.Response;
            if (response.CanExecuteUrlForEntireResponse)
            {
                string virtualPath = this.OverrideExecuteUrlPath();
                if (((virtualPath != null) && !HttpRuntime.IsFullTrust) && !base.GetType().Assembly.GlobalAssemblyCache)
                {
                    HttpRuntime.CheckFilePermission(context.Request.MapPath(virtualPath));
                }
                return response.BeginExecuteUrlForEntireResponse(virtualPath, this._executeUrlHeaders, callback, state);
            }
            this.OnExecuteUrlPreconditionFailure();
            this._context = null;
            HttpRequest request = context.Request;
            if (request.HttpVerb == HttpVerb.POST)
            {
                throw new HttpException(0x195, System.Web.SR.GetString("Method_not_allowed", new object[] { request.HttpMethod, request.Path }));
            }
            if (IsClassicAspRequest(request.FilePath))
            {
                throw new HttpException(0x193, System.Web.SR.GetString("Path_forbidden", new object[] { request.Path }));
            }
            StaticFileHandler.ProcessRequestInternal(context);
            return new HttpAsyncResult(callback, state, true, null, null);
        }

        public virtual void EndProcessRequest(IAsyncResult result)
        {
            if (this._context != null)
            {
                HttpResponse response = this._context.Response;
                this._context = null;
                response.EndExecuteUrlForEntireResponse(result);
            }
        }

        internal static bool IsClassicAspRequest(string filePath) => 
            StringUtil.StringEndsWithIgnoreCase(filePath, ".asp");

        public virtual void OnExecuteUrlPreconditionFailure()
        {
        }

        public virtual string OverrideExecuteUrlPath() => 
            null;

        public virtual void ProcessRequest(HttpContext context)
        {
            throw new InvalidOperationException(System.Web.SR.GetString("Cannot_call_defaulthttphandler_sync"));
        }

        protected HttpContext Context =>
            this._context;

        protected NameValueCollection ExecuteUrlHeaders
        {
            get
            {
                if ((this._executeUrlHeaders == null) && (this._context != null))
                {
                    this._executeUrlHeaders = new NameValueCollection(this._context.Request.Headers);
                }
                return this._executeUrlHeaders;
            }
        }

        public virtual bool IsReusable =>
            false;
    }
}

