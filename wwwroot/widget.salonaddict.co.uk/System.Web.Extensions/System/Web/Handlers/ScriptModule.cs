namespace System.Web.Handlers
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Security.Permissions;
    using System.Web;
    using System.Web.ApplicationServices;
    using System.Web.Resources;
    using System.Web.Script.Services;
    using System.Web.UI;

    [AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class ScriptModule : IHttpModule
    {
        private static Type _authenticationServiceType = typeof(AuthenticationService);

        private void AuthenticateRequestHandler(object sender, EventArgs e)
        {
            HttpApplication application = (HttpApplication) sender;
            if ((application != null) && ShouldSkipAuthorization(application.Context))
            {
                application.Context.SkipAuthorization = true;
            }
        }

        protected virtual void Dispose()
        {
        }

        private void EndRequestHandler(object sender, EventArgs e)
        {
            HttpApplication application = (HttpApplication) sender;
            HttpContext context = application.Context;
            object obj2 = context.Items["System.Web.UI.PageRequestManager:AsyncPostBackError"];
            if ((obj2 != null) && ((bool) obj2))
            {
                context.ClearError();
                context.Response.ClearHeaders();
                context.Response.Clear();
                context.Response.Cache.SetCacheability(HttpCacheability.NoCache);
                context.Response.ContentType = "text/plain";
                string content = (string) context.Items["System.Web.UI.PageRequestManager:AsyncPostBackErrorMessage"];
                obj2 = context.Items["System.Web.UI.PageRequestManager:AsyncPostBackErrorHttpCode"];
                PageRequestManager.EncodeString(context.Response.Output, "error", ((obj2 is int) ? ((int) obj2) : 500).ToString(CultureInfo.InvariantCulture), content);
            }
        }

        protected virtual void Init(HttpApplication context)
        {
            context.PreSendRequestHeaders += new EventHandler(this.PreSendRequestHeadersHandler);
            context.PostAcquireRequestState += new EventHandler(this.OnPostAcquireRequestState);
            context.AuthenticateRequest += new EventHandler(this.AuthenticateRequestHandler);
            context.EndRequest += new EventHandler(this.EndRequestHandler);
        }

        private void OnPostAcquireRequestState(object sender, EventArgs eventArgs)
        {
            HttpApplication application = (HttpApplication) sender;
            HttpRequest request = application.Context.Request;
            if ((application.Context.Handler is Page) && RestHandlerFactory.IsRestMethodCall(request))
            {
                WebServiceData data = WebServiceData.GetWebServiceData(HttpContext.Current, request.FilePath, false, true);
                string methodName = request.PathInfo.Substring(1);
                WebServiceMethodData methodData = data.GetMethodData(methodName);
                RestHandler.ExecuteWebServiceCall(HttpContext.Current, methodData);
                application.CompleteRequest();
            }
        }

        private void PreSendRequestHeadersHandler(object sender, EventArgs args)
        {
            HttpApplication application = (HttpApplication) sender;
            HttpResponse response = application.Response;
            if (response.StatusCode == 0x12e)
            {
                if (PageRequestManager.IsAsyncPostBackRequest(new HttpRequestWrapper(application.Request)))
                {
                    string redirectLocation = response.RedirectLocation;
                    List<HttpCookie> list = new List<HttpCookie>(response.Cookies.Count);
                    for (int i = 0; i < response.Cookies.Count; i++)
                    {
                        list.Add(response.Cookies[i]);
                    }
                    response.ClearContent();
                    response.ClearHeaders();
                    for (int j = 0; j < list.Count; j++)
                    {
                        response.AppendCookie(list[j]);
                    }
                    response.Cache.SetCacheability(HttpCacheability.NoCache);
                    response.ContentType = "text/plain";
                    response.AddHeader("X-Content-Type-Options", "nosniff");
                    PageRequestManager.EncodeString(response.Output, "dataItem", string.Empty, "<script type=\"text/javascript\">window.location=\"about:blank\"</script>");
                    PageRequestManager.EncodeString(response.Output, "pageRedirect", string.Empty, redirectLocation);
                }
                else if (RestHandlerFactory.IsRestRequest(application.Context))
                {
                    RestHandler.WriteExceptionJsonString(application.Context, new InvalidOperationException(AtlasWeb.WebService_RedirectError), 0x191);
                }
            }
        }

        private static bool ShouldSkipAuthorization(HttpContext context)
        {
            if ((context == null) || (context.Request == null))
            {
                return false;
            }
            string filePath = context.Request.FilePath;
            if (ScriptResourceHandler.IsScriptResourceRequest(filePath))
            {
                return true;
            }
            if (!ApplicationServiceHelper.AuthenticationServiceEnabled || !RestHandlerFactory.IsRestRequest(context))
            {
                return false;
            }
            if (context.SkipAuthorization)
            {
                return true;
            }
            if ((filePath == null) || !filePath.EndsWith(".axd", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }
            WebServiceData data = WebServiceData.GetWebServiceData(context, filePath, false, false);
            return ((data != null) && (_authenticationServiceType == data.TypeData.Type));
        }

        void IHttpModule.Dispose()
        {
            this.Dispose();
        }

        void IHttpModule.Init(HttpApplication context)
        {
            this.Init(context);
        }
    }
}

