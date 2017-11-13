﻿namespace System.ServiceModel.Channels
{
    using System;
    using System.Diagnostics;
    using System.Net;
    using System.Security.Authentication.ExtendedProtection;
    using System.ServiceModel;
    using System.ServiceModel.Diagnostics;
    using System.ServiceModel.Dispatcher;
    using System.Threading;

    internal class SharedHttpTransportManager : HttpTransportManager
    {
        private HttpListener listener;
        private ManualResetEvent listenStartedEvent;
        private Exception listenStartedException;
        private const int maxPendingGetContexts = 10;
        private WaitCallback onCompleteGetContextLater;
        private AsyncCallback onGetContext;
        private ItemDequeuedCallback onMessageDequeued;
        private bool unsafeConnectionNtlmAuthentication;

        internal SharedHttpTransportManager(Uri listenUri, HttpChannelListener channelListener) : base(listenUri, channelListener.HostNameComparisonMode, channelListener.Realm)
        {
            this.onGetContext = DiagnosticUtility.ThunkAsyncCallback(new AsyncCallback(this.OnGetContext));
            this.onMessageDequeued = new ItemDequeuedCallback(this.OnMessageDequeued);
            this.unsafeConnectionNtlmAuthentication = channelListener.UnsafeConnectionNtlmAuthentication;
        }

        private IAsyncResult BeginGetContext(AsyncCallback callback, object state, bool startListening)
        {
            Exception exception;
            do
            {
                exception = null;
                try
                {
                    try
                    {
                        if (ExecutionContext.IsFlowSuppressed())
                        {
                            return this.listener.BeginGetContext(callback, state);
                        }
                        using (ExecutionContext.SuppressFlow())
                        {
                            return this.listener.BeginGetContext(callback, state);
                        }
                    }
                    catch (HttpListenerException exception2)
                    {
                        switch (exception2.ErrorCode)
                        {
                            case 8:
                            case 14:
                            case 0x5aa:
                                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InsufficientMemoryException(System.ServiceModel.SR.GetString("InsufficentMemory"), exception2));
                        }
                        if (!ExceptionHandler.HandleTransportExceptionHelper(exception2))
                        {
                            throw;
                        }
                    }
                }
                catch (Exception exception3)
                {
                    if (DiagnosticUtility.IsFatal(exception3))
                    {
                        throw;
                    }
                    if (startListening)
                    {
                        throw;
                    }
                    exception = exception3;
                }
            }
            while (exception == null);
            base.Fault(exception);
            return null;
        }

        internal override bool IsCompatible(HttpChannelListener channelListener)
        {
            if (channelListener.InheritBaseAddressSettings)
            {
                return true;
            }
            if (!channelListener.IsScopeIdCompatible(base.HostNameComparisonMode, base.ListenUri))
            {
                return false;
            }
            return ((channelListener.UnsafeConnectionNtlmAuthentication == this.unsafeConnectionNtlmAuthentication) && base.IsCompatible(channelListener));
        }

        internal override void OnClose()
        {
            try
            {
                this.listener.Stop();
            }
            finally
            {
                try
                {
                    this.listener.Close();
                }
                finally
                {
                    base.OnClose();
                }
            }
            this.listener = null;
        }

        private void OnCompleteGetContextLater(object state)
        {
            this.OnGetContextCore((IAsyncResult) state);
        }

        private void OnGetContext(IAsyncResult result)
        {
            if (!result.CompletedSynchronously)
            {
                this.OnGetContextCore(result);
            }
        }

        private void OnGetContextCore(IAsyncResult result)
        {
            bool flag = false;
            while (!flag)
            {
                Exception exception = null;
                try
                {
                    try
                    {
                        HttpListenerContext listenerContext = null;
                        lock (base.ThisLock)
                        {
                            if (this.listener == null)
                            {
                                break;
                            }
                            listenerContext = this.listener.EndGetContext(result);
                        }
                        HttpChannelListener listener = null;
                        using (DiagnosticUtility.ShouldUseActivity ? ServiceModelActivity.BoundOperation(base.Activity) : null)
                        {
                            using (ServiceModelActivity activity = DiagnosticUtility.ShouldUseActivity ? ServiceModelActivity.CreateBoundedActivityWithTransferInOnly(listenerContext.Request.RequestTraceIdentifier) : null)
                            {
                                if ((activity != null) && DiagnosticUtility.ShouldUseActivity)
                                {
                                    ServiceModelActivity.Start(activity, System.ServiceModel.SR.GetString("ActivityReceiveBytes", new object[] { listenerContext.Request.Url.ToString() }), ActivityType.ReceiveBytes);
                                }
                                if (DiagnosticUtility.ShouldTraceInformation)
                                {
                                    TraceUtility.TraceHttpConnectionInformation(listenerContext.Request.LocalEndPoint.ToString(), listenerContext.Request.RemoteEndPoint.ToString(), this);
                                }
                                if (base.TryLookupUri(listenerContext.Request.Url, listenerContext.Request.HttpMethod, base.HostNameComparisonMode, out listener))
                                {
                                    flag = listener.HttpContextReceived(HttpRequestContext.CreateContext(listener, listenerContext), this.onMessageDequeued);
                                }
                                else
                                {
                                    if (DiagnosticUtility.ShouldTraceWarning)
                                    {
                                        TraceUtility.TraceEvent(TraceEventType.Warning, TraceCode.HttpChannelMessageReceiveFailed, (Message) null);
                                    }
                                    if (string.Compare(listenerContext.Request.HttpMethod, "POST", StringComparison.OrdinalIgnoreCase) != 0)
                                    {
                                        listenerContext.Response.StatusCode = 0x195;
                                        listenerContext.Response.Headers.Add(HttpResponseHeader.Allow, "POST");
                                    }
                                    else
                                    {
                                        listenerContext.Response.StatusCode = 0x194;
                                    }
                                    listenerContext.Response.ContentLength64 = 0L;
                                    listenerContext.Response.Close();
                                }
                            }
                        }
                    }
                    catch (HttpListenerException exception2)
                    {
                        switch (exception2.ErrorCode)
                        {
                            case 8:
                            case 14:
                            case 0x5aa:
                                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InsufficientMemoryException(System.ServiceModel.SR.GetString("InsufficentMemory"), exception2));
                        }
                        if (!ExceptionHandler.HandleTransportExceptionHelper(exception2))
                        {
                            throw;
                        }
                    }
                }
                catch (Exception exception3)
                {
                    if (DiagnosticUtility.IsFatal(exception3))
                    {
                        throw;
                    }
                    exception = exception3;
                }
                if (exception != null)
                {
                    base.Fault(exception);
                }
                if (!flag)
                {
                    lock (base.ThisLock)
                    {
                        if (this.listener == null)
                        {
                            continue;
                        }
                        result = this.BeginGetContext(this.onGetContext, null, false);
                        if ((result != null) && result.CompletedSynchronously)
                        {
                            continue;
                        }
                        break;
                    }
                }
            }
        }

        private void OnListening(object state)
        {
            try
            {
                this.StartListening();
            }
            catch (Exception exception)
            {
                if (DiagnosticUtility.IsFatal(exception))
                {
                    throw;
                }
                this.listenStartedException = exception;
            }
            finally
            {
                this.listenStartedEvent.Set();
            }
        }

        private void OnMessageDequeued()
        {
            ThreadTrace.Trace("message dequeued");
            IAsyncResult state = null;
            lock (base.ThisLock)
            {
                if (this.listener != null)
                {
                    state = this.BeginGetContext(this.onGetContext, null, false);
                }
            }
            if ((state != null) && state.CompletedSynchronously)
            {
                if (this.onCompleteGetContextLater == null)
                {
                    this.onCompleteGetContextLater = new WaitCallback(this.OnCompleteGetContextLater);
                }
                IOThreadScheduler.ScheduleCallback(this.onCompleteGetContextLater, state);
            }
        }

        internal override void OnOpen()
        {
            string dnsSafeHost;
            this.listener = new HttpListener();
            switch (base.HostNameComparisonMode)
            {
                case HostNameComparisonMode.StrongWildcard:
                    dnsSafeHost = "+";
                    break;

                case HostNameComparisonMode.Exact:
                    if (base.ListenUri.HostNameType != UriHostNameType.IPv6)
                    {
                        dnsSafeHost = base.ListenUri.DnsSafeHost;
                        break;
                    }
                    dnsSafeHost = "[" + base.ListenUri.DnsSafeHost + "]";
                    break;

                case HostNameComparisonMode.WeakWildcard:
                    dnsSafeHost = "*";
                    break;

                default:
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(System.ServiceModel.SR.GetString("UnrecognizedHostNameComparisonMode", new object[] { base.HostNameComparisonMode.ToString() })));
            }
            string components = base.ListenUri.GetComponents(UriComponents.Path, UriFormat.Unescaped);
            if (!components.StartsWith("/", StringComparison.Ordinal))
            {
                components = "/" + components;
            }
            if (!components.EndsWith("/", StringComparison.Ordinal))
            {
                components = components + "/";
            }
            string uriPrefix = string.Concat(new object[] { this.Scheme, "://", dnsSafeHost, ":", base.ListenUri.Port, components });
            this.listener.UnsafeConnectionNtlmAuthentication = this.unsafeConnectionNtlmAuthentication;
            this.listener.AuthenticationSchemeSelectorDelegate = new AuthenticationSchemeSelector(this.SelectAuthenticationScheme);
            if (ChannelBindingUtility.OSSupportsExtendedProtection)
            {
                this.listener.ExtendedProtectionSelectorDelegate = new HttpListener.ExtendedProtectionSelector(this.SelectExtendedProtectionPolicy);
            }
            if (base.Realm != null)
            {
                this.listener.Realm = base.Realm;
            }
            bool flag = false;
            try
            {
                this.listener.Prefixes.Add(uriPrefix);
                this.listener.Start();
                bool flag2 = false;
                try
                {
                    if (Thread.CurrentThread.IsThreadPoolThread)
                    {
                        this.StartListening();
                    }
                    else
                    {
                        this.listenStartedEvent = new ManualResetEvent(false);
                        IOThreadScheduler.ScheduleCallback(new WaitCallback(this.OnListening), null);
                        this.listenStartedEvent.WaitOne();
                        this.listenStartedEvent.Close();
                        this.listenStartedEvent = null;
                        if (this.listenStartedException != null)
                        {
                            throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(this.listenStartedException);
                        }
                    }
                    flag2 = true;
                }
                finally
                {
                    if (!flag2)
                    {
                        this.listener.Stop();
                    }
                }
                flag = true;
            }
            catch (HttpListenerException exception)
            {
                switch (exception.NativeErrorCode)
                {
                    case 0x57:
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(System.ServiceModel.SR.GetString("HttpInvalidListenURI", new object[] { base.ListenUri.OriginalString }), exception));

                    case 0xb7:
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new AddressAlreadyInUseException(System.ServiceModel.SR.GetString("HttpRegistrationAlreadyExists", new object[] { uriPrefix }), exception));

                    case 0x540:
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new CommunicationException(System.ServiceModel.SR.GetString("HttpRegistrationLimitExceeded", new object[] { uriPrefix }), exception));

                    case 5:
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new AddressAccessDeniedException(System.ServiceModel.SR.GetString("HttpRegistrationAccessDenied", new object[] { uriPrefix }), exception));

                    case 0x20:
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new AddressAlreadyInUseException(System.ServiceModel.SR.GetString("HttpRegistrationPortInUse", new object[] { uriPrefix, base.ListenUri.Port }), exception));
                }
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(HttpChannelUtilities.CreateCommunicationException(exception));
            }
            finally
            {
                if (!flag)
                {
                    this.listener.Abort();
                }
            }
        }

        private AuthenticationSchemes SelectAuthenticationScheme(HttpListenerRequest request)
        {
            AuthenticationSchemes schemes2;
            try
            {
                AuthenticationSchemes authenticationScheme;
                HttpChannelListener listener;
                if (base.TryLookupUri(request.Url, request.HttpMethod, base.HostNameComparisonMode, out listener))
                {
                    authenticationScheme = listener.AuthenticationScheme;
                }
                else
                {
                    authenticationScheme = AuthenticationSchemes.Anonymous;
                }
                schemes2 = authenticationScheme;
            }
            catch (Exception exception)
            {
                DiagnosticUtility.ExceptionUtility.TraceHandledException(exception, TraceEventType.Error);
                throw;
            }
            return schemes2;
        }

        private ExtendedProtectionPolicy SelectExtendedProtectionPolicy(HttpListenerRequest request)
        {
            ExtendedProtectionPolicy extendedProtectionPolicy = null;
            ExtendedProtectionPolicy policy2;
            try
            {
                HttpChannelListener listener;
                if (base.TryLookupUri(request.Url, request.HttpMethod, base.HostNameComparisonMode, out listener))
                {
                    extendedProtectionPolicy = listener.ExtendedProtectionPolicy;
                }
                else
                {
                    extendedProtectionPolicy = ChannelBindingUtility.DisabledPolicy;
                }
                policy2 = extendedProtectionPolicy;
            }
            catch (Exception exception)
            {
                DiagnosticUtility.ExceptionUtility.TraceHandledException(exception, TraceEventType.Error);
                throw;
            }
            return policy2;
        }

        private void StartListening()
        {
            for (int i = 0; i < 10; i++)
            {
                IAsyncResult state = this.BeginGetContext(this.onGetContext, null, true);
                if (state.CompletedSynchronously)
                {
                    if (this.onCompleteGetContextLater == null)
                    {
                        this.onCompleteGetContextLater = new WaitCallback(this.OnCompleteGetContextLater);
                    }
                    IOThreadScheduler.ScheduleCallback(this.onCompleteGetContextLater, state);
                }
            }
        }
    }
}

