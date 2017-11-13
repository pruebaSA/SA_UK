namespace System.ServiceModel.Activation
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Security;
    using System.ServiceModel;
    using System.Web;

    internal class ServiceHttpModule : IHttpModule
    {
        [SecurityCritical]
        private static BeginEventHandler beginEventHandler;
        private static bool disabled;
        [SecurityCritical]
        private static EndEventHandler endEventHandler;

        [SecurityCritical]
        public static IAsyncResult BeginProcessRequest(object sender, EventArgs e, AsyncCallback cb, object extraData)
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
                    return new CompletedAsyncResult(cb, extraData);
                }
                if (ServiceHostingEnvironment.AspNetCompatibilityEnabled)
                {
                    return new CompletedAsyncResult(cb, extraData);
                }
                HttpApplication context = (HttpApplication) sender;
                string extension = Path.GetExtension(context.Request.FilePath);
                if ((extension != null) && ServiceHostingEnvironment.GetExtensionSupported(extension))
                {
                    return new HostedHttpRequestAsyncResult(context, false, cb, extraData);
                }
            }
            return new CompletedAsyncResult(cb, extraData);
        }

        public void Dispose()
        {
        }

        public static void EndProcessRequest(IAsyncResult ar)
        {
            if (ar is CompletedAsyncResult)
            {
                CompletedAsyncResult.End(ar);
            }
            else
            {
                HostedHttpRequestAsyncResult.End(ar);
            }
        }

        [SecurityCritical]
        public void Init(HttpApplication context)
        {
            if (beginEventHandler == null)
            {
                beginEventHandler = new BeginEventHandler(ServiceHttpModule.BeginProcessRequest);
            }
            if (endEventHandler == null)
            {
                endEventHandler = new EndEventHandler(ServiceHttpModule.EndProcessRequest);
            }
            context.AddOnPostAuthenticateRequestAsync(beginEventHandler, endEventHandler);
        }
    }
}

