namespace System.ServiceModel.Activation
{
    using System;
    using System.Diagnostics;
    using System.Globalization;
    using System.Security;
    using System.Security.Permissions;
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Diagnostics;
    using System.Web;

    internal class HostedHttpTransportManager : HttpTransportManager
    {
        private static bool canTraceConnectionInformation = true;

        internal HostedHttpTransportManager(BaseUriWithWildcard baseAddress)
        {
            base.ListenUri = baseAddress.BaseAddress;
            base.HostNameComparisonMode = baseAddress.HostNameComparisonMode;
        }

        [SecurityTreatAsSafe, SecurityCritical]
        public ServiceModelActivity CreateReceiveBytesActivity(HostedHttpRequestAsyncResult result)
        {
            ServiceModelActivity activity = null;
            if ((result != null) && DiagnosticUtility.ShouldUseActivity)
            {
                activity = ServiceModelActivity.CreateBoundedActivity(GetRequestTraceIdentifier(result.Application.Context));
                ServiceModelActivity.Start(activity, System.ServiceModel.SR.GetString("ActivityReceiveBytes", new object[] { result.RequestUri.ToString() }), ActivityType.ReceiveBytes);
            }
            return activity;
        }

        [SecurityTreatAsSafe, SecurityCritical, SecurityPermission(SecurityAction.Assert, UnmanagedCode=true)]
        private static Guid GetRequestTraceIdentifier(IServiceProvider provider) => 
            ((HttpWorkerRequest) provider.GetService(typeof(HttpWorkerRequest))).RequestTraceIdentifier;

        internal void HttpContextReceived(HostedHttpRequestAsyncResult result)
        {
            using (DiagnosticUtility.ShouldUseActivity ? ServiceModelActivity.BoundOperation(base.Activity) : null)
            {
                using (this.CreateReceiveBytesActivity(result))
                {
                    HttpChannelListener listener;
                    this.TraceConnectionInformation(result);
                    if (base.TryLookupUri(result.RequestUri, result.GetHttpMethod(), base.HostNameComparisonMode, out listener))
                    {
                        HostedHttpContext context = (HostedHttpContext) HttpRequestContext.CreateContext(listener, result);
                        listener.HttpContextReceived(context, null);
                    }
                    else
                    {
                        if (DiagnosticUtility.ShouldTraceError)
                        {
                            TraceUtility.TraceEvent(TraceEventType.Error, TraceCode.HttpChannelMessageReceiveFailed, new StringTraceRecord("IsRecycling", ServiceHostingEnvironment.IsRecycling.ToString(CultureInfo.CurrentCulture)), this, null);
                        }
                        if (ServiceHostingEnvironment.IsRecycling)
                        {
                            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new EndpointNotFoundException(System.ServiceModel.SR.GetString("Hosting_ListenerNotFoundForActivationInRecycling", new object[] { result.RequestUri.ToString() })));
                        }
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new EndpointNotFoundException(System.ServiceModel.SR.GetString("Hosting_ListenerNotFoundForActivation", new object[] { result.RequestUri.ToString() })));
                    }
                }
            }
        }

        internal override bool IsCompatible(HttpChannelListener factory) => 
            true;

        internal override void OnClose()
        {
        }

        internal override void OnOpen()
        {
        }

        public void TraceConnectionInformation(HostedHttpRequestAsyncResult result)
        {
            if (((result != null) && DiagnosticUtility.ShouldTraceInformation) && canTraceConnectionInformation)
            {
                try
                {
                    HttpWorkerRequest service = (HttpWorkerRequest) ((IServiceProvider) result.Application.Context).GetService(typeof(HttpWorkerRequest));
                    string localEndpoint = string.Format(CultureInfo.InvariantCulture, "{0}:{1}", new object[] { service.GetLocalAddress(), service.GetLocalPort() });
                    string remoteEndpoint = string.Format(CultureInfo.InvariantCulture, "{0}:{1}", new object[] { service.GetRemoteAddress(), service.GetRemotePort() });
                    TraceUtility.TraceHttpConnectionInformation(localEndpoint, remoteEndpoint, this);
                }
                catch (SecurityException exception)
                {
                    canTraceConnectionInformation = false;
                    if (DiagnosticUtility.ShouldTraceWarning)
                    {
                        DiagnosticUtility.ExceptionUtility.TraceHandledException(exception, TraceEventType.Warning);
                    }
                }
            }
        }

        internal override string Scheme =>
            base.ListenUri.Scheme;
    }
}

