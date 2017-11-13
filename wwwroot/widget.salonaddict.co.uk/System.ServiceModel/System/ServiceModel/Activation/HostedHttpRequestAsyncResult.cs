namespace System.ServiceModel.Activation
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Security;
    using System.Security.Authentication.ExtendedProtection;
    using System.Security.Permissions;
    using System.Security.Principal;
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Diagnostics;
    using System.Threading;
    using System.Web;

    internal class HostedHttpRequestAsyncResult : AsyncResult
    {
        [SecurityCritical]
        private static WindowsIdentity anonymousIdentity;
        [SecurityCritical]
        private HttpApplication context;
        [SecurityCritical]
        private static ContextCallback contextOnBeginRequest;
        [SecurityCritical]
        private bool flowContext;
        [SecurityCritical]
        private System.ServiceModel.Activation.HostedThreadData hostedThreadData;
        private static bool? iisSupportsExtendedProtection;
        [SecurityCritical]
        private HostedImpersonationContext impersonationContext;
        private Uri originalRequestUri;
        [SecurityCritical]
        private static AsyncCallback processRequestCompleteCallback;
        private Uri requestUri;
        private int state;
        [ThreadStatic]
        private static AutoResetEvent waitObject;
        [SecurityCritical]
        private static WaitCallback waitOnBeginRequest;
        [SecurityCritical]
        private static WaitCallback waitOnBeginRequestWithFlow;

        [SecurityCritical]
        public HostedHttpRequestAsyncResult(HttpApplication context, bool flowContext, AsyncCallback callback, object state) : base(callback, state)
        {
            if (context == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("context"));
            }
            this.context = context;
            this.flowContext = flowContext;
            string strA = context.Request.HttpMethod ?? "";
            char ch = (strA.Length == 5) ? strA[0] : '\0';
            if (((ch == 'd') || (ch == 'D')) && (string.Compare(strA, "DEBUG", StringComparison.OrdinalIgnoreCase) == 0))
            {
                if (DiagnosticUtility.ShouldTraceVerbose)
                {
                    TraceUtility.TraceEvent(TraceEventType.Verbose, TraceCode.WebHostDebugRequest, this);
                }
                this.state = 1;
                base.Complete(true, null);
            }
            else
            {
                this.impersonationContext = new HostedImpersonationContext();
                if (flowContext && ServiceHostingEnvironment.AspNetCompatibilityEnabled)
                {
                    this.hostedThreadData = new System.ServiceModel.Activation.HostedThreadData();
                }
                WaitCallback callback2 = (PartialTrustHelpers.NeedPartialTrustInvoke || flowContext) ? WaitOnBeginRequestWithFlow : WaitOnBeginRequest;
                if (!ServiceHostingEnvironment.AspNetCompatibilityEnabled)
                {
                    context.CompleteRequest();
                }
                context.Server.ScriptTimeout = 0x7fffffff;
                ServiceHostingEnvironment.IncrementRequestCount();
                IOThreadScheduler.ScheduleCallbackLowPriNoFlow(callback2, this);
            }
        }

        public void Abort()
        {
            if ((this.state == 0) && (Interlocked.CompareExchange(ref this.state, 2, 0) == 0))
            {
                this.Application.Response.Close();
                base.Complete(false, null);
                ServiceHostingEnvironment.DecrementRequestCount();
            }
        }

        [SecurityCritical, SecurityTreatAsSafe]
        internal void AppendHeader(string name, string value)
        {
            this.context.Response.AppendHeader(name, value);
        }

        private void BeginRequest()
        {
            try
            {
                this.HandleRequest();
            }
            catch (EndpointNotFoundException exception)
            {
                if (string.Compare(this.GetHttpMethod(), "GET", StringComparison.OrdinalIgnoreCase) == 0)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new HttpException(0x194, exception.Message, exception));
                }
                this.SetStatusCode(0x194);
                this.CompleteOperation(null);
            }
            catch (ServiceActivationException exception2)
            {
                if (string.Compare(this.GetHttpMethod(), "GET", StringComparison.OrdinalIgnoreCase) == 0)
                {
                    if (exception2.InnerException is HttpException)
                    {
                        throw exception2.InnerException;
                    }
                    throw;
                }
                this.SetStatusCode(500);
                this.SetStatusDescription("System.ServiceModel.ServiceActivationException");
                this.CompleteOperation(null);
            }
            finally
            {
                this.ReleaseImpersonation();
            }
        }

        private void CompleteOperation(Exception exception)
        {
            if ((this.state == 0) && (Interlocked.CompareExchange(ref this.state, 1, 0) == 0))
            {
                base.Complete(false, exception);
                ServiceHostingEnvironment.DecrementRequestCount();
            }
        }

        public static void End(IAsyncResult result)
        {
            if (result == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentNullException("result"));
            }
            try
            {
                AsyncResult.End<HostedHttpRequestAsyncResult>(result);
            }
            catch (Exception exception)
            {
                if (!DiagnosticUtility.IsFatal(exception))
                {
                    DiagnosticUtility.EventLog.LogEvent(TraceEventType.Error, EventLogCategory.WebHost, (EventLogEventId) (-1073610749), new string[] { DiagnosticTrace.CreateSourceString(result), (exception == null) ? string.Empty : exception.ToString() });
                }
                throw;
            }
        }

        [SecurityCritical]
        public static void ExecuteSynchronous(HttpApplication context, bool flowContext)
        {
            HostedHttpRequestAsyncResult result;
            AutoResetEvent waitObject = HostedHttpRequestAsyncResult.waitObject;
            if (waitObject == null)
            {
                waitObject = new AutoResetEvent(false);
                HostedHttpRequestAsyncResult.waitObject = waitObject;
            }
            try
            {
                result = new HostedHttpRequestAsyncResult(context, flowContext, ProcessRequestCompleteCallback, waitObject);
                if (!result.CompletedSynchronously)
                {
                    waitObject.WaitOne();
                }
                waitObject = null;
            }
            finally
            {
                if (waitObject != null)
                {
                    HostedHttpRequestAsyncResult.waitObject = null;
                    waitObject.Close();
                }
            }
            End(result);
        }

        [SecurityTreatAsSafe, SecurityCritical]
        private string GetAppRelativeCurrentExecutionFilePath() => 
            this.context.Request.AppRelativeCurrentExecutionFilePath;

        [SecurityTreatAsSafe, SecurityCritical]
        internal ChannelBinding GetChannelBinding()
        {
            if (!this.IISSupportsExtendedProtection)
            {
                return null;
            }
            return this.context.Request.HttpChannelBinding;
        }

        [SecurityTreatAsSafe, SecurityCritical]
        internal int GetContentLength() => 
            this.context.Request.ContentLength;

        [SecurityTreatAsSafe, SecurityCritical]
        internal string GetContentType() => 
            this.context.Request.Headers["Content-Type"];

        [SecurityCritical, SecurityTreatAsSafe]
        internal string GetContentTypeFast() => 
            this.context.Request.ContentType;

        [SecurityCritical, SecurityTreatAsSafe]
        internal string GetHttpMethod() => 
            this.context.Request.HttpMethod;

        [SecurityTreatAsSafe, SecurityCritical]
        public Stream GetInputStream()
        {
            Stream inputStream;
            try
            {
                inputStream = this.context.Request.InputStream;
            }
            catch (HttpException exception)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new CommunicationException(exception.Message, exception));
            }
            return inputStream;
        }

        [SecurityTreatAsSafe, SecurityCritical]
        internal Stream GetOutputStream() => 
            this.context.Response.OutputStream;

        [SecurityCritical, SecurityTreatAsSafe]
        internal byte[] GetPrereadBuffer(ref int contentLength)
        {
            byte[] buffer = new byte[1];
            if (this.GetInputStream().Read(buffer, 0, 1) > 0)
            {
                contentLength = -1;
                return buffer;
            }
            return null;
        }

        [SecurityTreatAsSafe, SecurityCritical]
        internal string GetSoapAction() => 
            this.context.Request.Headers["SOAPAction"];

        [SecurityCritical, SecurityTreatAsSafe]
        private Uri GetUrl() => 
            this.context.Request.Url;

        private void HandleRequest()
        {
            this.originalRequestUri = this.GetUrl();
            string appRelativeCurrentExecutionFilePath = this.GetAppRelativeCurrentExecutionFilePath();
            if (ServiceHostingEnvironment.IsSimpleApplicationHost)
            {
                HostedTransportConfigurationManager.EnsureInitializedForSimpleApplicationHost(this);
            }
            HttpHostedTransportConfiguration configuration = HostedTransportConfigurationManager.GetConfiguration(this.originalRequestUri.Scheme) as HttpHostedTransportConfiguration;
            HostedHttpTransportManager httpTransportManager = null;
            if (configuration != null)
            {
                httpTransportManager = configuration.GetHttpTransportManager(this.originalRequestUri);
            }
            if (httpTransportManager == null)
            {
                InvalidOperationException innerException = new InvalidOperationException(System.ServiceModel.SR.GetString("Hosting_TransportBindingNotFound", new object[] { this.originalRequestUri.ToString() }));
                ServiceActivationException activationException = new ServiceActivationException(innerException.Message, innerException);
                this.LogServiceActivationException(activationException);
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(activationException);
            }
            this.requestUri = new Uri(httpTransportManager.ListenUri, this.originalRequestUri.PathAndQuery);
            ServiceHostingEnvironment.EnsureServiceAvailableFast(appRelativeCurrentExecutionFilePath);
            httpTransportManager.HttpContextReceived(this);
        }

        [MethodImpl(MethodImplOptions.NoInlining), SecurityCritical, SecurityTreatAsSafe, PermissionSet(SecurityAction.Assert, Unrestricted=true)]
        private bool IISSupportsExtendedProtectionInternal()
        {
            if (!ChannelBindingUtility.OSSupportsExtendedProtection)
            {
                return false;
            }
            try
            {
                ChannelBinding httpChannelBinding = this.context.Request.HttpChannelBinding;
                return true;
            }
            catch (PlatformNotSupportedException)
            {
                return false;
            }
            catch (COMException)
            {
                return true;
            }
        }

        [SecurityTreatAsSafe, SecurityCritical]
        private void LogServiceActivationException(ServiceActivationException activationException)
        {
            DiagnosticUtility.UnsafeEventLog.UnsafeLogEvent(TraceEventType.Error, EventLogCategory.WebHost, (EventLogEventId) (-1073610749), true, new string[] { DiagnosticTrace.CreateSourceString(this), (activationException == null) ? string.Empty : activationException.ToString() });
        }

        private static void OnBeginRequest(object state)
        {
            HostedHttpRequestAsyncResult result = (HostedHttpRequestAsyncResult) state;
            Exception exception = null;
            try
            {
                result.BeginRequest();
            }
            catch (Exception exception2)
            {
                if (DiagnosticUtility.IsFatal(exception2))
                {
                    throw;
                }
                exception = exception2;
            }
            if (exception != null)
            {
                result.CompleteOperation(exception);
            }
        }

        [SecurityTreatAsSafe, SecurityCritical]
        private static void OnBeginRequestWithFlow(object state)
        {
            HostedHttpRequestAsyncResult result = (HostedHttpRequestAsyncResult) state;
            using (IDisposable disposable = null)
            {
                if (result.flowContext && (result.hostedThreadData != null))
                {
                    disposable = result.hostedThreadData.CreateContext();
                }
                PartialTrustHelpers.PartialTrustInvoke(ContextOnBeginRequest, result);
            }
        }

        public void OnReplySent()
        {
            this.CompleteOperation(null);
        }

        private static void ProcessRequestComplete(IAsyncResult result)
        {
            if (!result.CompletedSynchronously)
            {
                try
                {
                    ((AutoResetEvent) result.AsyncState).Set();
                }
                catch (ObjectDisposedException exception)
                {
                    if (DiagnosticUtility.ShouldTraceWarning)
                    {
                        DiagnosticUtility.ExceptionUtility.TraceHandledException(exception, TraceEventType.Warning);
                    }
                }
            }
        }

        [SecurityCritical, SecurityTreatAsSafe]
        private void ReleaseImpersonation()
        {
            if (this.impersonationContext != null)
            {
                this.impersonationContext.Release();
            }
        }

        [SecurityTreatAsSafe, SecurityCritical]
        internal void SetConnectionClose()
        {
            this.context.Response.AppendHeader("Connection", "close");
        }

        [SecurityCritical, SecurityTreatAsSafe]
        internal void SetContentType(string contentType)
        {
            this.context.Response.ContentType = contentType;
        }

        [SecurityCritical, SecurityTreatAsSafe]
        internal void SetStatusCode(int statusCode)
        {
            this.context.Response.TrySkipIisCustomErrors = true;
            this.context.Response.StatusCode = statusCode;
        }

        [SecurityCritical, SecurityTreatAsSafe]
        internal void SetStatusDescription(string statusDescription)
        {
            this.context.Response.StatusDescription = statusDescription;
        }

        [SecurityTreatAsSafe, SecurityCritical]
        internal void SetTransferModeToStreaming()
        {
            this.context.Response.BufferOutput = false;
        }

        public static WindowsIdentity AnonymousIdentity
        {
            [SecurityTreatAsSafe, SecurityCritical]
            get
            {
                if (anonymousIdentity == null)
                {
                    anonymousIdentity = WindowsIdentity.GetAnonymous();
                }
                return anonymousIdentity;
            }
        }

        public HttpApplication Application =>
            this.context;

        public static ContextCallback ContextOnBeginRequest
        {
            [SecurityCritical, SecurityTreatAsSafe]
            get
            {
                if (contextOnBeginRequest == null)
                {
                    contextOnBeginRequest = new ContextCallback(HostedHttpRequestAsyncResult.OnBeginRequest);
                }
                return contextOnBeginRequest;
            }
        }

        public System.ServiceModel.Activation.HostedThreadData HostedThreadData =>
            this.hostedThreadData;

        public bool IISSupportsExtendedProtection
        {
            get
            {
                if (!iisSupportsExtendedProtection.HasValue)
                {
                    iisSupportsExtendedProtection = new bool?(this.IISSupportsExtendedProtectionInternal());
                }
                return iisSupportsExtendedProtection.Value;
            }
        }

        public HostedImpersonationContext ImpersonationContext =>
            this.impersonationContext;

        public WindowsIdentity LogonUserIdentity
        {
            get
            {
                if (this.Application.User.Identity is WindowsIdentity)
                {
                    return (WindowsIdentity) this.Application.User.Identity;
                }
                return AnonymousIdentity;
            }
        }

        public Uri OriginalRequestUri =>
            this.originalRequestUri;

        public static AsyncCallback ProcessRequestCompleteCallback
        {
            [SecurityCritical, SecurityTreatAsSafe]
            get
            {
                if (processRequestCompleteCallback == null)
                {
                    processRequestCompleteCallback = DiagnosticUtility.Utility.ThunkCallback(new AsyncCallback(HostedHttpRequestAsyncResult.ProcessRequestComplete));
                }
                return processRequestCompleteCallback;
            }
        }

        public Uri RequestUri =>
            this.requestUri;

        public static WaitCallback WaitOnBeginRequest
        {
            [SecurityCritical, SecurityTreatAsSafe]
            get
            {
                if (waitOnBeginRequest == null)
                {
                    waitOnBeginRequest = new WaitCallback(HostedHttpRequestAsyncResult.OnBeginRequest);
                }
                return waitOnBeginRequest;
            }
        }

        public static WaitCallback WaitOnBeginRequestWithFlow
        {
            [SecurityCritical, SecurityTreatAsSafe]
            get
            {
                if (waitOnBeginRequestWithFlow == null)
                {
                    waitOnBeginRequestWithFlow = new WaitCallback(HostedHttpRequestAsyncResult.OnBeginRequestWithFlow);
                }
                return waitOnBeginRequestWithFlow;
            }
        }

        private static class State
        {
            internal const int Aborted = 2;
            internal const int Completed = 1;
            internal const int Running = 0;
        }
    }
}

