namespace System.ServiceModel.Activation
{
    using System;
    using System.Security;
    using System.ServiceModel;
    using System.Web;
    using System.Web.SessionState;

    internal class HttpHandler : IHttpHandler, IRequiresSessionState
    {
        [SecurityCritical]
        public void ProcessRequest(HttpContext context)
        {
            ServiceHostingEnvironment.SafeEnsureInitialized();
            HostedHttpRequestAsyncResult.ExecuteSynchronous(context.ApplicationInstance, true);
        }

        public bool IsReusable =>
            true;
    }
}

