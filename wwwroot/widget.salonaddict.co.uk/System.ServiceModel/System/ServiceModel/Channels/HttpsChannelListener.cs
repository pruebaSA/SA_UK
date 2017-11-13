namespace System.ServiceModel.Channels
{
    using System;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.IdentityModel.Selectors;
    using System.IdentityModel.Tokens;
    using System.Net;
    using System.Security.Cryptography.X509Certificates;
    using System.Security.Principal;
    using System.ServiceModel;
    using System.ServiceModel.Activation;
    using System.ServiceModel.Description;
    using System.ServiceModel.Diagnostics;
    using System.ServiceModel.Security;
    using System.Web;

    internal class HttpsChannelListener : HttpChannelListener
    {
        private SecurityTokenAuthenticator certificateAuthenticator;
        private const HttpStatusCode CertificateErrorStatusCode = HttpStatusCode.Forbidden;
        private IChannelBindingProvider channelBindingProvider;
        private bool requireClientCertificate;
        private readonly bool useCustomClientCertificateVerification;
        private bool useHostedClientCertificateMapping;

        public HttpsChannelListener(HttpsTransportBindingElement httpsBindingElement, BindingContext context) : base(httpsBindingElement, context)
        {
            this.requireClientCertificate = httpsBindingElement.RequireClientCertificate;
            SecurityCredentialsManager manager = context.BindingParameters.Find<SecurityCredentialsManager>();
            if (manager == null)
            {
                manager = ServiceCredentials.CreateDefaultCredentials();
            }
            SecurityTokenManager tokenManager = manager.CreateSecurityTokenManager();
            this.certificateAuthenticator = TransportSecurityHelpers.GetCertificateTokenAuthenticator(tokenManager, context.Binding.Scheme) as X509SecurityTokenAuthenticator;
            ServiceCredentials credentials = manager as ServiceCredentials;
            if ((credentials != null) && (credentials.ClientCertificate.Authentication.CertificateValidationMode == X509CertificateValidationMode.Custom))
            {
                this.useCustomClientCertificateVerification = true;
            }
            else
            {
                this.useCustomClientCertificateVerification = false;
                X509SecurityTokenAuthenticator certificateAuthenticator = this.certificateAuthenticator as X509SecurityTokenAuthenticator;
                if (certificateAuthenticator != null)
                {
                    this.certificateAuthenticator = new X509SecurityTokenAuthenticator(X509CertificateValidator.None, certificateAuthenticator.MapCertificateToWindowsAccount, base.ExtractGroupsForWindowsAccounts, false);
                }
            }
            if (this.RequireClientCertificate && (base.AuthenticationScheme != AuthenticationSchemes.Anonymous))
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelper(new InvalidOperationException(System.ServiceModel.SR.GetString("HttpAuthSchemeAndClientCert", new object[] { base.AuthenticationScheme })), TraceEventType.Error);
            }
            this.channelBindingProvider = new ChannelBindingProviderHelper();
        }

        internal override void ApplyHostedContext(VirtualPathExtension virtualPathExtension, bool isMetadataListener)
        {
            base.ApplyHostedContext(virtualPathExtension, isMetadataListener);
            if (!ServiceHostingEnvironment.IsSimpleApplicationHost)
            {
                HttpAccessSslFlags accessSslFlags = HostedTransportConfigurationManager.MetabaseSettings.GetAccessSslFlags(virtualPathExtension.VirtualPath);
                HttpAccessSslFlags none = HttpAccessSslFlags.None;
                bool flag = false;
                if ((accessSslFlags & HttpAccessSslFlags.SslRequireCert) != HttpAccessSslFlags.None)
                {
                    this.requireClientCertificate = true;
                }
                else if (this.RequireClientCertificate)
                {
                    none |= HttpAccessSslFlags.SslRequireCert;
                    flag = true;
                }
                if (!flag && ((accessSslFlags & HttpAccessSslFlags.SslMapCert) != HttpAccessSslFlags.None))
                {
                    this.useHostedClientCertificateMapping = true;
                }
                if (flag)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException(System.ServiceModel.SR.GetString("Hosting_SslSettingsMisconfigured", new object[] { none.ToString(), accessSslFlags.ToString() })));
                }
            }
        }

        private SecurityMessageProperty CreateSecurityProperty(X509Certificate2 certificate, WindowsIdentity identity)
        {
            SecurityToken token;
            if (identity != null)
            {
                token = new X509WindowsSecurityToken(certificate, identity, false);
            }
            else
            {
                token = new X509SecurityToken(certificate, false);
            }
            ReadOnlyCollection<IAuthorizationPolicy> tokenPolicies = this.certificateAuthenticator.ValidateToken(token);
            return new SecurityMessageProperty { 
                TransportToken = new SecurityTokenSpecification(token, tokenPolicies),
                ServiceSecurityContext = new ServiceSecurityContext(tokenPolicies)
            };
        }

        internal override ITransportManagerRegistration CreateTransportManagerRegistration(Uri listenUri) => 
            new SharedHttpsTransportManager(listenUri, this);

        public override T GetProperty<T>() where T: class
        {
            if (typeof(T) == typeof(IChannelBindingProvider))
            {
                return (T) this.channelBindingProvider;
            }
            return base.GetProperty<T>();
        }

        public override SecurityMessageProperty ProcessAuthentication(HttpListenerContext listenerContext)
        {
            if (this.requireClientCertificate)
            {
                SecurityMessageProperty property;
                X509Certificate2 certificate = null;
                try
                {
                    X509Certificate clientCertificate = listenerContext.Request.GetClientCertificate();
                    bool useCustomClientCertificateVerification = this.useCustomClientCertificateVerification;
                    certificate = new X509Certificate2(clientCertificate.Handle);
                    property = this.CreateSecurityProperty(certificate, null);
                }
                catch (Exception exception)
                {
                    if (DiagnosticUtility.IsFatal(exception))
                    {
                        throw;
                    }
                    if (AuditLevel.Failure == (base.AuditBehavior.MessageAuthenticationAuditLevel & AuditLevel.Failure))
                    {
                        base.WriteAuditEvent(AuditLevel.Failure, (certificate != null) ? System.ServiceModel.Security.SecurityUtils.GetCertificateId(certificate) : string.Empty, exception);
                    }
                    throw;
                }
                if (AuditLevel.Success == (base.AuditBehavior.MessageAuthenticationAuditLevel & AuditLevel.Success))
                {
                    base.WriteAuditEvent(AuditLevel.Success, (certificate != null) ? System.ServiceModel.Security.SecurityUtils.GetCertificateId(certificate) : string.Empty, null);
                }
                return property;
            }
            if (base.AuthenticationScheme == AuthenticationSchemes.Anonymous)
            {
                return new SecurityMessageProperty();
            }
            return base.ProcessAuthentication(listenerContext);
        }

        public override SecurityMessageProperty ProcessAuthentication(HostedHttpRequestAsyncResult result)
        {
            if (this.requireClientCertificate)
            {
                SecurityMessageProperty property;
                X509Certificate2 certificate = null;
                try
                {
                    HttpClientCertificate clientCertificate = result.Application.Request.ClientCertificate;
                    bool useCustomClientCertificateVerification = this.useCustomClientCertificateVerification;
                    certificate = new X509Certificate2(clientCertificate.Certificate);
                    WindowsIdentity wid = null;
                    if (this.useHostedClientCertificateMapping)
                    {
                        wid = result.LogonUserIdentity;
                        if ((wid == null) || !wid.IsAuthenticated)
                        {
                            wid = WindowsIdentity.GetAnonymous();
                        }
                        else
                        {
                            wid = System.ServiceModel.Security.SecurityUtils.CloneWindowsIdentityIfNecessary(wid);
                        }
                    }
                    property = this.CreateSecurityProperty(certificate, wid);
                }
                catch (Exception exception)
                {
                    if (DiagnosticUtility.IsFatal(exception))
                    {
                        throw;
                    }
                    if (AuditLevel.Failure == (base.AuditBehavior.MessageAuthenticationAuditLevel & AuditLevel.Failure))
                    {
                        base.WriteAuditEvent(AuditLevel.Failure, (certificate != null) ? System.ServiceModel.Security.SecurityUtils.GetCertificateId(certificate) : string.Empty, exception);
                    }
                    throw;
                }
                if (AuditLevel.Success == (base.AuditBehavior.MessageAuthenticationAuditLevel & AuditLevel.Success))
                {
                    base.WriteAuditEvent(AuditLevel.Success, (certificate != null) ? System.ServiceModel.Security.SecurityUtils.GetCertificateId(certificate) : string.Empty, null);
                }
                return property;
            }
            if (base.AuthenticationScheme == AuthenticationSchemes.Anonymous)
            {
                return new SecurityMessageProperty();
            }
            return base.ProcessAuthentication(result);
        }

        public override HttpStatusCode ValidateAuthentication(HttpListenerContext listenerContext)
        {
            HttpStatusCode forbidden = base.ValidateAuthentication(listenerContext);
            if ((forbidden == HttpStatusCode.OK) && this.RequireClientCertificate)
            {
                HttpListenerRequest request = listenerContext.Request;
                X509Certificate2 clientCertificate = request.GetClientCertificate();
                if (clientCertificate == null)
                {
                    if (DiagnosticUtility.ShouldTraceWarning)
                    {
                        TraceUtility.TraceEvent(TraceEventType.Warning, TraceCode.HttpsClientCertificateNotPresent, new HttpListenerRequestTraceRecord(listenerContext.Request), this, null);
                    }
                    forbidden = HttpStatusCode.Forbidden;
                }
                else if ((request.ClientCertificateError != 0) && !this.useCustomClientCertificateVerification)
                {
                    if (DiagnosticUtility.ShouldTraceWarning)
                    {
                        TraceUtility.TraceEvent(TraceEventType.Warning, TraceCode.HttpsClientCertificateInvalid, new HttpListenerRequestTraceRecord(listenerContext.Request), this, null);
                    }
                    forbidden = HttpStatusCode.Forbidden;
                }
                if ((forbidden != HttpStatusCode.OK) && (AuditLevel.Failure == (base.AuditBehavior.MessageAuthenticationAuditLevel & AuditLevel.Failure)))
                {
                    string message = System.ServiceModel.SR.GetString("HttpAuthenticationFailed", new object[] { base.AuthenticationScheme, forbidden });
                    Exception exception = DiagnosticUtility.ExceptionUtility.ThrowHelperError(new MessageSecurityException(message));
                    base.WriteAuditEvent(AuditLevel.Failure, (clientCertificate != null) ? System.ServiceModel.Security.SecurityUtils.GetCertificateId(clientCertificate) : string.Empty, exception);
                }
            }
            return forbidden;
        }

        public override HttpStatusCode ValidateAuthentication(HostedHttpRequestAsyncResult hostedAsyncResult)
        {
            HttpStatusCode forbidden = base.ValidateAuthentication(hostedAsyncResult);
            HttpRequest request = hostedAsyncResult.Application.Request;
            if ((forbidden == HttpStatusCode.OK) && this.RequireClientCertificate)
            {
                HttpClientCertificate clientCertificate = request.ClientCertificate;
                if (!clientCertificate.IsPresent)
                {
                    if (DiagnosticUtility.ShouldTraceError)
                    {
                        TraceUtility.TraceEvent(TraceEventType.Error, TraceCode.HttpsClientCertificateNotPresent, new HttpRequestTraceRecord(request), this, null);
                    }
                    forbidden = HttpStatusCode.Forbidden;
                }
                else if (!clientCertificate.IsValid && !this.useCustomClientCertificateVerification)
                {
                    if (DiagnosticUtility.ShouldTraceError)
                    {
                        TraceUtility.TraceEvent(TraceEventType.Error, TraceCode.HttpsClientCertificateInvalid, new HttpRequestTraceRecord(request), this, null);
                    }
                    forbidden = HttpStatusCode.Forbidden;
                }
                if ((forbidden != HttpStatusCode.OK) && (AuditLevel.Failure == (base.AuditBehavior.MessageAuthenticationAuditLevel & AuditLevel.Failure)))
                {
                    string message = System.ServiceModel.SR.GetString("HttpAuthenticationFailed", new object[] { base.AuthenticationScheme, forbidden });
                    X509Certificate2 certificate = clientCertificate.IsPresent ? new X509Certificate2(clientCertificate.Certificate) : null;
                    Exception exception = DiagnosticUtility.ExceptionUtility.ThrowHelperError(new MessageSecurityException(message));
                    base.WriteAuditEvent(AuditLevel.Failure, (certificate != null) ? System.ServiceModel.Security.SecurityUtils.GetCertificateId(certificate) : string.Empty, exception);
                }
            }
            return forbidden;
        }

        public override bool IsChannelBindingSupportEnabled =>
            this.channelBindingProvider.IsChannelBindingSupportEnabled;

        public bool RequireClientCertificate =>
            this.requireClientCertificate;

        public override string Scheme =>
            Uri.UriSchemeHttps;

        internal override UriPrefixTable<ITransportManagerRegistration> TransportManagerTable =>
            SharedHttpsTransportManager.StaticTransportManagerTable;
    }
}

