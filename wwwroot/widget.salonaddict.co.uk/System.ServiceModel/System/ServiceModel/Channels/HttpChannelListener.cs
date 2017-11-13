namespace System.ServiceModel.Channels
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Globalization;
    using System.IdentityModel.Selectors;
    using System.IdentityModel.Tokens;
    using System.IO;
    using System.Net;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Security.Authentication.ExtendedProtection;
    using System.Security.Principal;
    using System.ServiceModel;
    using System.ServiceModel.Activation;
    using System.ServiceModel.Description;
    using System.ServiceModel.Diagnostics;
    using System.ServiceModel.Dispatcher;
    using System.ServiceModel.Security;
    using System.ServiceModel.Security.Tokens;
    using System.Xml;

    internal class HttpChannelListener : TransportChannelListener, IHttpTransportFactorySettings, ITransportFactorySettings, IDefaultCommunicationTimeouts, IChannelListener<IReplyChannel>, IChannelListener, ICommunicationObject
    {
        private ReplyChannelAcceptor acceptor;
        private AuthenticationSchemes authenticationScheme;
        private SecurityCredentialsManager credentialProvider;
        private System.Security.Authentication.ExtendedProtection.ExtendedProtectionPolicy extendedProtectionPolicy;
        private bool extractGroupsForWindowsAccounts;
        private EndpointIdentity identity;
        private bool keepAliveEnabled;
        private int maxBufferSize;
        private string method;
        private string realm;
        private ISecurityCapabilities securityCapabilities;
        private System.ServiceModel.TransferMode transferMode;
        private static UriPrefixTable<ITransportManagerRegistration> transportManagerTable = new UriPrefixTable<ITransportManagerRegistration>(true);
        private bool unsafeConnectionNtlmAuthentication;
        private SecurityTokenAuthenticator userNameTokenAuthenticator;
        private bool usingDefaultSpnList;
        private SecurityTokenAuthenticator windowsTokenAuthenticator;

        public HttpChannelListener(HttpTransportBindingElement bindingElement, BindingContext context) : base(bindingElement, context, HttpTransportDefaults.GetDefaultMessageEncoderFactory(), bindingElement.HostNameComparisonMode)
        {
            if (bindingElement.TransferMode == System.ServiceModel.TransferMode.Buffered)
            {
                if (bindingElement.MaxReceivedMessageSize > 0x7fffffffL)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("bindingElement.MaxReceivedMessageSize", System.ServiceModel.SR.GetString("MaxReceivedMessageSizeMustBeInIntegerRange")));
                }
                if (bindingElement.MaxBufferSize != bindingElement.MaxReceivedMessageSize)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument("bindingElement", System.ServiceModel.SR.GetString("MaxBufferSizeMustMatchMaxReceivedMessageSize"));
                }
            }
            else if (bindingElement.MaxBufferSize > bindingElement.MaxReceivedMessageSize)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument("bindingElement", System.ServiceModel.SR.GetString("MaxBufferSizeMustNotExceedMaxReceivedMessageSize"));
            }
            if ((bindingElement.AuthenticationScheme == AuthenticationSchemes.Basic) && (bindingElement.ExtendedProtectionPolicy.PolicyEnforcement == PolicyEnforcement.Always))
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException(System.ServiceModel.SR.GetString("ExtendedProtectionPolicyBasicAuthNotSupported")));
            }
            this.authenticationScheme = bindingElement.AuthenticationScheme;
            this.keepAliveEnabled = bindingElement.KeepAliveEnabled;
            base.InheritBaseAddressSettings = bindingElement.InheritBaseAddressSettings;
            this.maxBufferSize = bindingElement.MaxBufferSize;
            this.method = bindingElement.Method;
            this.realm = bindingElement.Realm;
            this.transferMode = bindingElement.TransferMode;
            this.unsafeConnectionNtlmAuthentication = bindingElement.UnsafeConnectionNtlmAuthentication;
            this.credentialProvider = context.BindingParameters.Find<SecurityCredentialsManager>();
            this.acceptor = new TransportReplyChannelAcceptor(this);
            this.securityCapabilities = bindingElement.GetProperty<ISecurityCapabilities>(context);
            this.extendedProtectionPolicy = this.GetPolicyWithDefaultSpnCollection(bindingElement.ExtendedProtectionPolicy);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private void AbortUserNameTokenAuthenticator()
        {
            System.ServiceModel.Security.SecurityUtils.AbortTokenAuthenticatorIfRequired(this.userNameTokenAuthenticator);
        }

        public IReplyChannel AcceptChannel() => 
            this.AcceptChannel(this.DefaultReceiveTimeout);

        public IReplyChannel AcceptChannel(TimeSpan timeout)
        {
            base.ThrowIfNotOpened();
            return this.acceptor.AcceptChannel(timeout);
        }

        private static void AddSpn(Dictionary<string, string> list, string value)
        {
            string key = value.ToLowerInvariant();
            if (!list.ContainsKey(key))
            {
                list.Add(key, value);
            }
        }

        internal override void ApplyHostedContext(VirtualPathExtension virtualPathExtension, bool isMetadataListener)
        {
            base.ApplyHostedContext(virtualPathExtension, isMetadataListener);
            AuthenticationSchemes authenticationSchemes = HostedTransportConfigurationManager.MetabaseSettings.GetAuthenticationSchemes(base.HostedVirtualPath);
            if (((this.AuthenticationScheme == AuthenticationSchemes.Anonymous) && ((authenticationSchemes & AuthenticationSchemes.Anonymous) == AuthenticationSchemes.None)) && isMetadataListener)
            {
                if ((authenticationSchemes & AuthenticationSchemes.Negotiate) != AuthenticationSchemes.None)
                {
                    this.authenticationScheme = AuthenticationSchemes.Negotiate;
                }
                else
                {
                    this.authenticationScheme = authenticationSchemes;
                }
            }
            if ((authenticationSchemes & this.AuthenticationScheme) == AuthenticationSchemes.None)
            {
                if (AuthenticationSchemesHelper.IsWindowsAuth(this.AuthenticationScheme))
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException(System.ServiceModel.SR.GetString("Hosting_AuthSchemesRequireWindowsAuth")));
                }
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException(System.ServiceModel.SR.GetString("Hosting_AuthSchemesRequireOtherAuth", new object[] { this.AuthenticationScheme.ToString() })));
            }
            if (this.AuthenticationScheme != AuthenticationSchemes.Anonymous)
            {
                System.Security.Authentication.ExtendedProtection.ExtendedProtectionPolicy extendedProtectionPolicy = HostedTransportConfigurationManager.MetabaseSettings.GetExtendedProtectionPolicy(base.HostedVirtualPath);
                if (extendedProtectionPolicy == null)
                {
                    if (this.extendedProtectionPolicy.PolicyEnforcement == PolicyEnforcement.Always)
                    {
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException(System.ServiceModel.SR.GetString("ExtendedProtectionNotSupported")));
                    }
                }
                else if (isMetadataListener && ChannelBindingUtility.IsDefaultPolicy(this.extendedProtectionPolicy))
                {
                    this.extendedProtectionPolicy = extendedProtectionPolicy;
                }
                else
                {
                    ChannelBindingUtility.ValidatePolicies(extendedProtectionPolicy, this.extendedProtectionPolicy, true);
                    ServiceNameCollection subset = this.usingDefaultSpnList ? null : this.extendedProtectionPolicy.CustomServiceNames;
                    if (!ChannelBindingUtility.IsSubset(extendedProtectionPolicy.CustomServiceNames, subset))
                    {
                        string message = System.ServiceModel.SR.GetString("Hosting_ExtendedProtectionPoliciesMustMatch2", new object[] { System.ServiceModel.SR.GetString("Hosting_ExtendedProtectionSPNListNotSubset") });
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException(message));
                    }
                }
            }
            if (!ServiceHostingEnvironment.IsSimpleApplicationHost)
            {
                this.realm = HostedTransportConfigurationManager.MetabaseSettings.GetRealm(virtualPathExtension.VirtualPath);
            }
        }

        public IAsyncResult BeginAcceptChannel(AsyncCallback callback, object state) => 
            this.BeginAcceptChannel(this.DefaultReceiveTimeout, callback, state);

        public IAsyncResult BeginAcceptChannel(TimeSpan timeout, AsyncCallback callback, object state)
        {
            base.ThrowIfNotOpened();
            return this.acceptor.BeginAcceptChannel(timeout, callback, state);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private void CloseUserNameTokenAuthenticator(TimeSpan timeout)
        {
            System.ServiceModel.Security.SecurityUtils.CloseTokenAuthenticatorIfRequired(this.userNameTokenAuthenticator, timeout);
        }

        internal override ITransportManagerRegistration CreateTransportManagerRegistration(Uri listenUri) => 
            new SharedHttpTransportManager(listenUri, this);

        public IReplyChannel EndAcceptChannel(IAsyncResult result)
        {
            base.ThrowPending();
            return this.acceptor.EndAcceptChannel(result);
        }

        private string GetAuthType(HttpListenerContext listenerContext)
        {
            string authenticationType = null;
            IPrincipal user = listenerContext.User;
            if ((user != null) && (user.Identity != null))
            {
                authenticationType = user.Identity.AuthenticationType;
            }
            return authenticationType;
        }

        private string GetAuthType(HostedHttpRequestAsyncResult hostedAsyncResult)
        {
            string authenticationType = null;
            if (hostedAsyncResult.LogonUserIdentity != null)
            {
                authenticationType = hostedAsyncResult.LogonUserIdentity.AuthenticationType;
            }
            return authenticationType;
        }

        private string[] GetDefaultSpnList()
        {
            Dictionary<string, string> list = new Dictionary<string, string>();
            this.usingDefaultSpnList = true;
            string hostName = null;
            switch (base.HostNameComparisonModeInternal)
            {
                case System.ServiceModel.HostNameComparisonMode.StrongWildcard:
                case System.ServiceModel.HostNameComparisonMode.WeakWildcard:
                    hostName = Dns.GetHostEntry(string.Empty).HostName;
                    AddSpn(list, string.Format(CultureInfo.InvariantCulture, "HOST/{0}", new object[] { hostName }));
                    AddSpn(list, string.Format(CultureInfo.InvariantCulture, "HTTP/{0}", new object[] { hostName }));
                    break;

                case System.ServiceModel.HostNameComparisonMode.Exact:
                {
                    UriHostNameType hostNameType = base.Uri.HostNameType;
                    if ((hostNameType != UriHostNameType.IPv4) && (hostNameType != UriHostNameType.IPv6))
                    {
                        if (base.Uri.DnsSafeHost.Contains("."))
                        {
                            AddSpn(list, string.Format(CultureInfo.InvariantCulture, "HOST/{0}", new object[] { base.Uri.DnsSafeHost }));
                            AddSpn(list, string.Format(CultureInfo.InvariantCulture, "HTTP/{0}", new object[] { base.Uri.DnsSafeHost }));
                        }
                        else
                        {
                            hostName = Dns.GetHostEntry(string.Empty).HostName;
                            AddSpn(list, string.Format(CultureInfo.InvariantCulture, "HOST/{0}", new object[] { base.Uri.DnsSafeHost }));
                            AddSpn(list, string.Format(CultureInfo.InvariantCulture, "HTTP/{0}", new object[] { base.Uri.DnsSafeHost }));
                            AddSpn(list, string.Format(CultureInfo.InvariantCulture, "HOST/{0}", new object[] { hostName }));
                            AddSpn(list, string.Format(CultureInfo.InvariantCulture, "HTTP/{0}", new object[] { hostName }));
                        }
                        break;
                    }
                    hostName = Dns.GetHostEntry(string.Empty).HostName;
                    AddSpn(list, string.Format(CultureInfo.InvariantCulture, "HOST/{0}", new object[] { hostName }));
                    AddSpn(list, string.Format(CultureInfo.InvariantCulture, "HTTP/{0}", new object[] { hostName }));
                    break;
                }
            }
            AddSpn(list, string.Format(CultureInfo.InvariantCulture, "HOST/{0}", new object[] { "localhost" }));
            AddSpn(list, string.Format(CultureInfo.InvariantCulture, "HTTP/{0}", new object[] { "localhost" }));
            string[] array = new string[list.Values.Count];
            list.Values.CopyTo(array, 0);
            return array;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private T GetIdentityModelProperty<T>()
        {
            if (typeof(T) == typeof(EndpointIdentity))
            {
                if ((this.identity == null) && AuthenticationSchemesHelper.IsWindowsAuth(this.authenticationScheme))
                {
                    this.identity = System.ServiceModel.Security.SecurityUtils.CreateWindowsIdentity();
                }
                return (T) this.identity;
            }
            if ((typeof(T) == typeof(ILogonTokenCacheManager)) && (this.userNameTokenAuthenticator != null))
            {
                ILogonTokenCacheManager userNameTokenAuthenticator = this.userNameTokenAuthenticator as ILogonTokenCacheManager;
                if (userNameTokenAuthenticator != null)
                {
                    return (T) userNameTokenAuthenticator;
                }
            }
            return default(T);
        }

        internal override int GetMaxBufferSize() => 
            this.MaxBufferSize;

        private System.Security.Authentication.ExtendedProtection.ExtendedProtectionPolicy GetPolicyWithDefaultSpnCollection(System.Security.Authentication.ExtendedProtection.ExtendedProtectionPolicy policy)
        {
            if (((policy.PolicyEnforcement != PolicyEnforcement.Never) && (policy.CustomServiceNames == null)) && ((this.authenticationScheme != AuthenticationSchemes.Anonymous) && string.Equals(this.Scheme, Uri.UriSchemeHttp, StringComparison.OrdinalIgnoreCase)))
            {
                return new System.Security.Authentication.ExtendedProtection.ExtendedProtectionPolicy(policy.PolicyEnforcement, policy.ProtectionScenario, new ServiceNameCollection(this.GetDefaultSpnList()));
            }
            return policy;
        }

        public override T GetProperty<T>() where T: class
        {
            if (typeof(T) == typeof(EndpointIdentity))
            {
                return (T) this.identity;
            }
            if (typeof(T) == typeof(ILogonTokenCacheManager))
            {
                object identityModelProperty = this.GetIdentityModelProperty<T>();
                if (identityModelProperty != null)
                {
                    return (T) identityModelProperty;
                }
            }
            else
            {
                if (typeof(T) == typeof(ISecurityCapabilities))
                {
                    return (T) this.securityCapabilities;
                }
                if (typeof(T) == typeof(System.Security.Authentication.ExtendedProtection.ExtendedProtectionPolicy))
                {
                    return (T) this.extendedProtectionPolicy;
                }
            }
            return base.GetProperty<T>();
        }

        internal bool HttpContextReceived(HttpRequestContext context, ItemDequeuedCallback callback)
        {
            bool flag = false;
            bool flag2 = false;
            try
            {
                if (!context.ProcessAuthentication())
                {
                    if (DiagnosticUtility.ShouldTraceInformation)
                    {
                        TraceUtility.TraceEvent(TraceEventType.Information, TraceCode.HttpAuthFailed, this);
                    }
                    flag2 = true;
                    return false;
                }
                try
                {
                    context.CreateMessage();
                }
                catch (ProtocolException exception)
                {
                    HttpStatusCode badRequest = HttpStatusCode.BadRequest;
                    string statusDescription = string.Empty;
                    if (exception.Data.Contains("System.ServiceModel.Channels.HttpInput.HttpStatusCode"))
                    {
                        badRequest = (HttpStatusCode) exception.Data["System.ServiceModel.Channels.HttpInput.HttpStatusCode"];
                        exception.Data.Remove("System.ServiceModel.Channels.HttpInput.HttpStatusCode");
                    }
                    if (exception.Data.Contains("System.ServiceModel.Channels.HttpInput.HttpStatusDescription"))
                    {
                        statusDescription = (string) exception.Data["System.ServiceModel.Channels.HttpInput.HttpStatusDescription"];
                        exception.Data.Remove("System.ServiceModel.Channels.HttpInput.HttpStatusDescription");
                    }
                    context.SendResponseAndClose(badRequest, statusDescription);
                    throw;
                }
                catch (Exception exception2)
                {
                    if (DiagnosticUtility.IsFatal(exception2))
                    {
                        throw;
                    }
                    try
                    {
                        context.SendResponseAndClose(HttpStatusCode.BadRequest);
                    }
                    catch (Exception exception3)
                    {
                        if (DiagnosticUtility.IsFatal(exception3))
                        {
                            throw;
                        }
                        if (DiagnosticUtility.ShouldTraceError)
                        {
                            DiagnosticUtility.ExceptionUtility.TraceHandledException(exception3, TraceEventType.Error);
                        }
                    }
                    throw;
                }
                flag = true;
                this.acceptor.Enqueue(context, callback);
                flag2 = true;
            }
            catch (CommunicationException exception4)
            {
                if (DiagnosticUtility.ShouldTraceInformation)
                {
                    DiagnosticUtility.ExceptionUtility.TraceHandledException(exception4, TraceEventType.Information);
                }
            }
            catch (XmlException exception5)
            {
                if (DiagnosticUtility.ShouldTraceInformation)
                {
                    DiagnosticUtility.ExceptionUtility.TraceHandledException(exception5, TraceEventType.Information);
                }
            }
            catch (IOException exception6)
            {
                if (DiagnosticUtility.ShouldTraceInformation)
                {
                    DiagnosticUtility.ExceptionUtility.TraceHandledException(exception6, TraceEventType.Information);
                }
            }
            catch (TimeoutException exception7)
            {
                if (DiagnosticUtility.ShouldTraceInformation)
                {
                    DiagnosticUtility.ExceptionUtility.TraceHandledException(exception7, TraceEventType.Information);
                }
            }
            catch (Exception exception8)
            {
                if (DiagnosticUtility.IsFatal(exception8))
                {
                    throw;
                }
                if (!ExceptionHandler.HandleTransportExceptionHelper(exception8))
                {
                    throw;
                }
            }
            finally
            {
                if (!flag2)
                {
                    context.Abort();
                }
            }
            return flag;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private void InitializeSecurityTokenAuthenticator()
        {
            ServiceCredentials credentialProvider = this.credentialProvider as ServiceCredentials;
            if (credentialProvider != null)
            {
                this.extractGroupsForWindowsAccounts = (this.AuthenticationScheme == AuthenticationSchemes.Basic) ? credentialProvider.UserNameAuthentication.IncludeWindowsGroups : credentialProvider.WindowsAuthentication.IncludeWindowsGroups;
                if (credentialProvider.UserNameAuthentication.UserNamePasswordValidationMode == UserNamePasswordValidationMode.Custom)
                {
                    this.userNameTokenAuthenticator = new CustomUserNameSecurityTokenAuthenticator(credentialProvider.UserNameAuthentication.GetUserNamePasswordValidator());
                }
                else if (credentialProvider.UserNameAuthentication.CacheLogonTokens)
                {
                    this.userNameTokenAuthenticator = new WindowsUserNameCachingSecurityTokenAuthenticator(this.extractGroupsForWindowsAccounts, credentialProvider.UserNameAuthentication.MaxCachedLogonTokens, credentialProvider.UserNameAuthentication.CachedLogonTokenLifetime);
                }
                else
                {
                    this.userNameTokenAuthenticator = new WindowsUserNameSecurityTokenAuthenticator(this.extractGroupsForWindowsAccounts);
                }
            }
            else
            {
                this.extractGroupsForWindowsAccounts = true;
                this.userNameTokenAuthenticator = new WindowsUserNameSecurityTokenAuthenticator(this.extractGroupsForWindowsAccounts);
            }
            this.windowsTokenAuthenticator = new WindowsSecurityTokenAuthenticator(this.extractGroupsForWindowsAccounts);
        }

        private bool IsAuthSchemeValid(string authType) => 
            AuthenticationSchemesHelper.DoesAuthTypeMatch(this.authenticationScheme, authType);

        protected override void OnAbort()
        {
            if (this.IsAuthenticationRequired)
            {
                this.AbortUserNameTokenAuthenticator();
            }
            this.acceptor.Abort();
            base.OnAbort();
        }

        protected override IAsyncResult OnBeginClose(TimeSpan timeout, AsyncCallback callback, object state)
        {
            ICommunicationObject[] objArray;
            TimeoutHelper helper = new TimeoutHelper(timeout);
            ICommunicationObject userNameTokenAuthenticator = this.userNameTokenAuthenticator as ICommunicationObject;
            if (userNameTokenAuthenticator == null)
            {
                if (this.IsAuthenticationRequired)
                {
                    this.CloseUserNameTokenAuthenticator(helper.RemainingTime());
                }
                objArray = new ICommunicationObject[] { this.acceptor };
            }
            else
            {
                objArray = new ICommunicationObject[] { this.acceptor, userNameTokenAuthenticator };
            }
            return new ChainedCloseAsyncResult(helper.RemainingTime(), callback, state, new ChainedBeginHandler(this.OnBeginClose), new ChainedEndHandler(this.OnEndClose), objArray);
        }

        protected override IAsyncResult OnBeginOpen(TimeSpan timeout, AsyncCallback callback, object state) => 
            new ChainedOpenAsyncResult(timeout, callback, state, new ChainedBeginHandler(this.OnBeginOpen), new ChainedEndHandler(this.OnEndOpen), new ICommunicationObject[] { this.acceptor });

        protected override IAsyncResult OnBeginWaitForChannel(TimeSpan timeout, AsyncCallback callback, object state) => 
            this.acceptor.BeginWaitForChannel(timeout, callback, state);

        protected override void OnClose(TimeSpan timeout)
        {
            TimeoutHelper helper = new TimeoutHelper(timeout);
            this.acceptor.Close(helper.RemainingTime());
            if (this.IsAuthenticationRequired)
            {
                this.CloseUserNameTokenAuthenticator(helper.RemainingTime());
            }
            base.OnClose(helper.RemainingTime());
        }

        protected override void OnEndClose(IAsyncResult result)
        {
            ChainedAsyncResult.End(result);
        }

        protected override void OnEndOpen(IAsyncResult result)
        {
            ChainedAsyncResult.End(result);
        }

        protected override bool OnEndWaitForChannel(IAsyncResult result) => 
            this.acceptor.EndWaitForChannel(result);

        protected override void OnOpen(TimeSpan timeout)
        {
            TimeoutHelper helper = new TimeoutHelper(timeout);
            base.OnOpen(helper.RemainingTime());
            this.acceptor.Open(helper.RemainingTime());
        }

        protected override void OnOpened()
        {
            base.OnOpened();
            if (this.IsAuthenticationRequired)
            {
                this.InitializeSecurityTokenAuthenticator();
                this.identity = this.GetIdentityModelProperty<EndpointIdentity>();
            }
        }

        protected override bool OnWaitForChannel(TimeSpan timeout) => 
            this.acceptor.WaitForChannel(timeout);

        private SecurityMessageProperty ProcessAuthentication(HttpListenerBasicIdentity identity)
        {
            SecurityToken token = new UserNameSecurityToken(identity.Name, identity.Password);
            ReadOnlyCollection<IAuthorizationPolicy> tokenPolicies = this.userNameTokenAuthenticator.ValidateToken(token);
            return new SecurityMessageProperty { 
                TransportToken = new SecurityTokenSpecification(token, tokenPolicies),
                ServiceSecurityContext = new ServiceSecurityContext(tokenPolicies)
            };
        }

        public virtual SecurityMessageProperty ProcessAuthentication(HttpListenerContext listenerContext)
        {
            if (this.IsAuthenticationRequired)
            {
                return this.ProcessRequiredAuthentication(listenerContext);
            }
            return null;
        }

        private SecurityMessageProperty ProcessAuthentication(WindowsIdentity identity)
        {
            System.ServiceModel.Security.SecurityUtils.ValidateAnonymityConstraint(identity, false);
            SecurityToken token = new WindowsSecurityToken(identity);
            ReadOnlyCollection<IAuthorizationPolicy> tokenPolicies = this.windowsTokenAuthenticator.ValidateToken(token);
            return new SecurityMessageProperty { 
                TransportToken = new SecurityTokenSpecification(token, tokenPolicies),
                ServiceSecurityContext = new ServiceSecurityContext(tokenPolicies)
            };
        }

        public virtual SecurityMessageProperty ProcessAuthentication(HostedHttpRequestAsyncResult result)
        {
            SecurityMessageProperty property;
            if (!this.IsAuthenticationRequired)
            {
                return null;
            }
            try
            {
                property = this.ProcessAuthentication(result.LogonUserIdentity);
            }
            catch (Exception exception)
            {
                if (DiagnosticUtility.IsFatal(exception))
                {
                    throw;
                }
                if (AuditLevel.Failure == (base.AuditBehavior.MessageAuthenticationAuditLevel & AuditLevel.Failure))
                {
                    this.WriteAuditEvent(AuditLevel.Failure, (result.LogonUserIdentity != null) ? result.LogonUserIdentity.Name : string.Empty, exception);
                }
                throw;
            }
            if (AuditLevel.Success == (base.AuditBehavior.MessageAuthenticationAuditLevel & AuditLevel.Success))
            {
                this.WriteAuditEvent(AuditLevel.Success, (result.LogonUserIdentity != null) ? result.LogonUserIdentity.Name : string.Empty, null);
            }
            return property;
        }

        private SecurityMessageProperty ProcessRequiredAuthentication(HttpListenerContext listenerContext)
        {
            SecurityMessageProperty property;
            HttpListenerBasicIdentity identity = null;
            WindowsIdentity identity2 = null;
            try
            {
                if (this.AuthenticationScheme == AuthenticationSchemes.Basic)
                {
                    identity = listenerContext.User.Identity as HttpListenerBasicIdentity;
                    property = this.ProcessAuthentication(identity);
                }
                else
                {
                    identity2 = listenerContext.User.Identity as WindowsIdentity;
                    property = this.ProcessAuthentication(identity2);
                }
            }
            catch (Exception exception)
            {
                if (DiagnosticUtility.IsFatal(exception))
                {
                    throw;
                }
                if (AuditLevel.Failure == (base.AuditBehavior.MessageAuthenticationAuditLevel & AuditLevel.Failure))
                {
                    this.WriteAuditEvent(AuditLevel.Failure, (identity != null) ? identity.Name : ((identity2 != null) ? identity2.Name : string.Empty), exception);
                }
                throw;
            }
            if (AuditLevel.Success == (base.AuditBehavior.MessageAuthenticationAuditLevel & AuditLevel.Success))
            {
                this.WriteAuditEvent(AuditLevel.Success, (identity != null) ? identity.Name : ((identity2 != null) ? identity2.Name : string.Empty), null);
            }
            return property;
        }

        protected override bool TryGetTransportManagerRegistration(System.ServiceModel.HostNameComparisonMode hostNameComparisonMode, out ITransportManagerRegistration registration)
        {
            if (this.TransportManagerTable.TryLookupUri(this.Uri, hostNameComparisonMode, out registration))
            {
                if (registration is HostedHttpTransportManager)
                {
                    return true;
                }
                if (registration.ListenUri.Segments.Length >= base.BaseUri.Segments.Length)
                {
                    return true;
                }
            }
            return false;
        }

        public virtual HttpStatusCode ValidateAuthentication(HttpListenerContext listenerContext)
        {
            HttpStatusCode oK = HttpStatusCode.OK;
            if (this.IsAuthenticationRequired)
            {
                string authType = this.GetAuthType(listenerContext);
                oK = this.ValidateAuthentication(authType);
            }
            return oK;
        }

        public virtual HttpStatusCode ValidateAuthentication(HostedHttpRequestAsyncResult hostedAsyncResult)
        {
            HttpStatusCode oK = HttpStatusCode.OK;
            if (this.IsAuthenticationRequired)
            {
                string authType = this.GetAuthType(hostedAsyncResult);
                oK = this.ValidateAuthentication(authType);
            }
            if (((oK == HttpStatusCode.OK) && (this.ExtendedProtectionPolicy.PolicyEnforcement == PolicyEnforcement.Always)) && (ChannelBindingUtility.OSSupportsExtendedProtection && !hostedAsyncResult.IISSupportsExtendedProtection))
            {
                Exception exception = DiagnosticUtility.ExceptionUtility.ThrowHelperError(new PlatformNotSupportedException(System.ServiceModel.SR.GetString("ExtendedProtectionNotSupported")));
                this.WriteAuditEvent(AuditLevel.Failure, string.Empty, exception);
                oK = HttpStatusCode.Unauthorized;
            }
            return oK;
        }

        private HttpStatusCode ValidateAuthentication(string authType)
        {
            if (this.IsAuthSchemeValid(authType))
            {
                return HttpStatusCode.OK;
            }
            if (AuditLevel.Failure == (base.AuditBehavior.MessageAuthenticationAuditLevel & AuditLevel.Failure))
            {
                string message = System.ServiceModel.SR.GetString("HttpAuthenticationFailed", new object[] { this.AuthenticationScheme, HttpStatusCode.Unauthorized });
                Exception exception = DiagnosticUtility.ExceptionUtility.ThrowHelperError(new MessageSecurityException(message));
                this.WriteAuditEvent(AuditLevel.Failure, string.Empty, exception);
            }
            return HttpStatusCode.Unauthorized;
        }

        protected void WriteAuditEvent(AuditLevel auditLevel, string primaryIdentity, Exception exception)
        {
            try
            {
                if (auditLevel == AuditLevel.Success)
                {
                    SecurityAuditHelper.WriteTransportAuthenticationSuccessEvent(base.AuditBehavior.AuditLogLocation, base.AuditBehavior.SuppressAuditFailure, null, this.Uri, primaryIdentity);
                }
                else
                {
                    SecurityAuditHelper.WriteTransportAuthenticationFailureEvent(base.AuditBehavior.AuditLogLocation, base.AuditBehavior.SuppressAuditFailure, null, this.Uri, primaryIdentity, exception);
                }
            }
            catch (Exception exception2)
            {
                if (DiagnosticUtility.IsFatal(exception2) || (auditLevel == AuditLevel.Success))
                {
                    throw;
                }
                DiagnosticUtility.ExceptionUtility.TraceHandledException(exception2, TraceEventType.Error);
            }
        }

        public AuthenticationSchemes AuthenticationScheme =>
            this.authenticationScheme;

        public System.Security.Authentication.ExtendedProtection.ExtendedProtectionPolicy ExtendedProtectionPolicy =>
            this.extendedProtectionPolicy;

        public bool ExtractGroupsForWindowsAccounts =>
            this.extractGroupsForWindowsAccounts;

        public System.ServiceModel.HostNameComparisonMode HostNameComparisonMode =>
            base.HostNameComparisonModeInternal;

        private bool IsAuthenticationRequired =>
            (this.AuthenticationScheme != AuthenticationSchemes.Anonymous);

        public virtual bool IsChannelBindingSupportEnabled =>
            false;

        public bool KeepAliveEnabled =>
            this.keepAliveEnabled;

        public int MaxBufferSize =>
            this.maxBufferSize;

        public string Method =>
            this.method;

        public string Realm =>
            this.realm;

        public override string Scheme =>
            Uri.UriSchemeHttp;

        internal static UriPrefixTable<ITransportManagerRegistration> StaticTransportManagerTable =>
            transportManagerTable;

        int IHttpTransportFactorySettings.MaxBufferSize =>
            this.MaxBufferSize;

        System.ServiceModel.TransferMode IHttpTransportFactorySettings.TransferMode =>
            this.TransferMode;

        public System.ServiceModel.TransferMode TransferMode =>
            this.transferMode;

        internal override UriPrefixTable<ITransportManagerRegistration> TransportManagerTable =>
            transportManagerTable;

        public bool UnsafeConnectionNtlmAuthentication =>
            this.unsafeConnectionNtlmAuthentication;
    }
}

