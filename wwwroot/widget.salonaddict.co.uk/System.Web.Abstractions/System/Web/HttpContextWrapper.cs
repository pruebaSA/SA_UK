namespace System.Web
{
    using System;
    using System.Collections;
    using System.Globalization;
    using System.Security.Permissions;
    using System.Security.Principal;
    using System.Web.Caching;
    using System.Web.Profile;
    using System.Web.SessionState;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class HttpContextWrapper : HttpContextBase
    {
        private readonly HttpContext _context;

        public HttpContextWrapper(HttpContext httpContext)
        {
            if (httpContext == null)
            {
                throw new ArgumentNullException("httpContext");
            }
            this._context = httpContext;
        }

        public override void AddError(Exception errorInfo)
        {
            this._context.AddError(errorInfo);
        }

        public override void ClearError()
        {
            this._context.ClearError();
        }

        public override object GetGlobalResourceObject(string classKey, string resourceKey) => 
            HttpContext.GetGlobalResourceObject(classKey, resourceKey);

        public override object GetGlobalResourceObject(string classKey, string resourceKey, CultureInfo culture) => 
            HttpContext.GetGlobalResourceObject(classKey, resourceKey, culture);

        public override object GetLocalResourceObject(string virtualPath, string resourceKey) => 
            HttpContext.GetLocalResourceObject(virtualPath, resourceKey);

        public override object GetLocalResourceObject(string virtualPath, string resourceKey, CultureInfo culture) => 
            HttpContext.GetLocalResourceObject(virtualPath, resourceKey, culture);

        public override object GetSection(string sectionName) => 
            this._context.GetSection(sectionName);

        public override object GetService(Type serviceType) => 
            ((IServiceProvider) this._context).GetService(serviceType);

        public override void RewritePath(string path)
        {
            this._context.RewritePath(path);
        }

        public override void RewritePath(string path, bool rebaseClientPath)
        {
            this._context.RewritePath(path, rebaseClientPath);
        }

        public override void RewritePath(string filePath, string pathInfo, string queryString)
        {
            this._context.RewritePath(filePath, pathInfo, queryString);
        }

        public override void RewritePath(string filePath, string pathInfo, string queryString, bool setClientFilePath)
        {
            this._context.RewritePath(filePath, pathInfo, queryString, setClientFilePath);
        }

        public override Exception[] AllErrors =>
            this._context.AllErrors;

        public override HttpApplicationStateBase Application =>
            new HttpApplicationStateWrapper(this._context.Application);

        public override HttpApplication ApplicationInstance
        {
            get => 
                this._context.ApplicationInstance;
            set
            {
                this._context.ApplicationInstance = value;
            }
        }

        public override System.Web.Caching.Cache Cache =>
            this._context.Cache;

        public override IHttpHandler CurrentHandler =>
            this._context.CurrentHandler;

        public override RequestNotification CurrentNotification =>
            this._context.CurrentNotification;

        public override Exception Error =>
            this._context.Error;

        public override IHttpHandler Handler
        {
            get => 
                this._context.Handler;
            set
            {
                this._context.Handler = value;
            }
        }

        public override bool IsCustomErrorEnabled =>
            this._context.IsCustomErrorEnabled;

        public override bool IsDebuggingEnabled =>
            this._context.IsDebuggingEnabled;

        public override bool IsPostNotification =>
            this._context.IsDebuggingEnabled;

        public override IDictionary Items =>
            this._context.Items;

        public override IHttpHandler PreviousHandler =>
            this._context.PreviousHandler;

        public override ProfileBase Profile =>
            this._context.Profile;

        public override HttpRequestBase Request =>
            new HttpRequestWrapper(this._context.Request);

        public override HttpResponseBase Response =>
            new HttpResponseWrapper(this._context.Response);

        public override HttpServerUtilityBase Server =>
            new HttpServerUtilityWrapper(this._context.Server);

        public override HttpSessionStateBase Session
        {
            get
            {
                HttpSessionState session = this._context.Session;
                if (session == null)
                {
                    return null;
                }
                return new HttpSessionStateWrapper(session);
            }
        }

        public override bool SkipAuthorization
        {
            get => 
                this._context.SkipAuthorization;
            set
            {
                this._context.SkipAuthorization = value;
            }
        }

        public override DateTime Timestamp =>
            this._context.Timestamp;

        public override TraceContext Trace =>
            this._context.Trace;

        public override IPrincipal User
        {
            get => 
                this._context.User;
            set
            {
                this._context.User = value;
            }
        }
    }
}

