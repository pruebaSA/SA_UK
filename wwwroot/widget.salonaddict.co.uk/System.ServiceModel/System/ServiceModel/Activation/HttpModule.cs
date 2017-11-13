namespace System.ServiceModel.Activation
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Security;
    using System.ServiceModel;
    using System.Web;

    internal class HttpModule : IHttpModule
    {
        private static bool disabled;

        public void Dispose()
        {
        }

        [SecurityCritical]
        public void Init(HttpApplication context)
        {
            context.PostAuthenticateRequest += new EventHandler(HttpModule.ProcessRequest);
        }

        [SecurityCritical]
        private static void ProcessRequest(object sender, EventArgs e)
        {
            if (!disabled)
            {
                try
                {
                    ServiceHostingEnvironment.SafeEnsureInitialized();
                }
                catch (SecurityException exception)
                {
                    disabled = true;
                    if (DiagnosticUtility.ShouldTraceWarning)
                    {
                        DiagnosticUtility.ExceptionUtility.TraceHandledException(exception, TraceEventType.Warning);
                    }
                    return;
                }
                if (!ServiceHostingEnvironment.AspNetCompatibilityEnabled)
                {
                    HttpApplication context = (HttpApplication) sender;
                    string extension = Path.GetExtension(context.Request.FilePath);
                    if ((extension != null) && ServiceHostingEnvironment.GetExtensionSupported(extension))
                    {
                        HostedHttpRequestAsyncResult.ExecuteSynchronous(context, false);
                    }
                }
            }
        }
    }
}

