namespace System.Web.Script.Services
{
    using System;
    using System.Web;

    internal class RestClientProxyHandler : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            string clientProxyScript = WebServiceClientProxyGenerator.GetClientProxyScript(context);
            if (clientProxyScript != null)
            {
                context.Response.ContentType = "application/x-javascript";
                context.Response.Write(clientProxyScript);
            }
        }

        public bool IsReusable =>
            false;
    }
}

