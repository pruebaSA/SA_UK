namespace System.ServiceModel.Channels
{
    using System;
    using System.IdentityModel.Selectors;
    using System.IdentityModel.Tokens;
    using System.Net;
    using System.Net.Security;
    using System.Runtime.InteropServices;
    using System.Security.Principal;
    using System.ServiceModel;
    using System.ServiceModel.Security;
    using System.ServiceModel.Security.Tokens;

    internal static class TransportSecurityHelpers
    {
        public static IAsyncResult BeginGetSspiCredential(SecurityTokenProvider tokenProvider, TimeSpan timeout, AsyncCallback callback, object state) => 
            new GetSspiCredentialAsyncResult((SspiSecurityTokenProvider) tokenProvider, timeout, callback, state);

        public static IAsyncResult BeginGetSspiCredential(SecurityTokenProviderContainer tokenProvider, TimeSpan timeout, AsyncCallback callback, object state) => 
            new GetSspiCredentialAsyncResult(tokenProvider.TokenProvider as SspiSecurityTokenProvider, timeout, callback, state);

        public static IAsyncResult BeginGetUserNameCredential(SecurityTokenProviderContainer tokenProvider, TimeSpan timeout, AsyncCallback callback, object state) => 
            new GetUserNameCredentialAsyncResult(tokenProvider, timeout, callback, state);

        public static SecurityTokenRequirement CreateSspiTokenRequirement(string transportScheme) => 
            new RecipientServiceModelSecurityTokenRequirement { 
                TransportScheme = transportScheme,
                RequireCryptographicToken = false,
                TokenType = ServiceModelSecurityTokenTypes.SspiCredential
            };

        private static SecurityTokenRequirement CreateSspiTokenRequirement(EndpointAddress target, Uri via, string transportScheme) => 
            new InitiatorServiceModelSecurityTokenRequirement { 
                TokenType = ServiceModelSecurityTokenTypes.SspiCredential,
                RequireCryptographicToken = false,
                TransportScheme = transportScheme,
                TargetAddress = target,
                Via = via
            };

        private static InitiatorServiceModelSecurityTokenRequirement CreateUserNameTokenRequirement(EndpointAddress target, Uri via, string transportScheme) => 
            new InitiatorServiceModelSecurityTokenRequirement { 
                RequireCryptographicToken = false,
                TokenType = SecurityTokenTypes.UserName,
                TargetAddress = target,
                Via = via,
                TransportScheme = transportScheme
            };

        public static NetworkCredential EndGetSspiCredential(IAsyncResult result, out TokenImpersonationLevel impersonationLevel, out bool allowNtlm) => 
            GetSspiCredentialAsyncResult.End(result, out impersonationLevel, out allowNtlm);

        public static NetworkCredential EndGetSspiCredential(IAsyncResult result, out TokenImpersonationLevel impersonationLevel, out AuthenticationLevel authenticationLevel) => 
            GetSspiCredentialAsyncResult.End(result, out impersonationLevel, out authenticationLevel);

        public static NetworkCredential EndGetUserNameCredential(IAsyncResult result) => 
            GetUserNameCredentialAsyncResult.End(result);

        public static SecurityTokenAuthenticator GetCertificateTokenAuthenticator(SecurityTokenManager tokenManager, string transportScheme)
        {
            SecurityTokenResolver resolver;
            RecipientServiceModelSecurityTokenRequirement tokenRequirement = new RecipientServiceModelSecurityTokenRequirement {
                TokenType = SecurityTokenTypes.X509Certificate,
                RequireCryptographicToken = true,
                KeyUsage = SecurityKeyUsage.Signature,
                TransportScheme = transportScheme
            };
            return tokenManager.CreateSecurityTokenAuthenticator(tokenRequirement, out resolver);
        }

        public static SecurityTokenProvider GetCertificateTokenProvider(SecurityTokenManager tokenManager, EndpointAddress target, Uri via, string transportScheme, ChannelParameterCollection channelParameters)
        {
            if (tokenManager == null)
            {
                return null;
            }
            InitiatorServiceModelSecurityTokenRequirement tokenRequirement = new InitiatorServiceModelSecurityTokenRequirement {
                TokenType = SecurityTokenTypes.X509Certificate,
                TargetAddress = target,
                Via = via,
                RequireCryptographicToken = false,
                TransportScheme = transportScheme
            };
            if (channelParameters != null)
            {
                tokenRequirement.Properties[ServiceModelSecurityTokenRequirement.ChannelParametersCollectionProperty] = channelParameters;
            }
            return tokenManager.CreateSecurityTokenProvider(tokenRequirement);
        }

        public static SecurityTokenProvider GetDigestTokenProvider(SecurityTokenManager tokenManager, EndpointAddress target, Uri via, string transportScheme, AuthenticationSchemes authenticationScheme, ChannelParameterCollection channelParameters)
        {
            if (tokenManager == null)
            {
                return null;
            }
            InitiatorServiceModelSecurityTokenRequirement tokenRequirement = new InitiatorServiceModelSecurityTokenRequirement {
                TokenType = ServiceModelSecurityTokenTypes.SspiCredential,
                TargetAddress = target,
                Via = via,
                RequireCryptographicToken = false,
                TransportScheme = transportScheme,
                Properties = { [ServiceModelSecurityTokenRequirement.HttpAuthenticationSchemeProperty] = authenticationScheme }
            };
            if (channelParameters != null)
            {
                tokenRequirement.Properties[ServiceModelSecurityTokenRequirement.ChannelParametersCollectionProperty] = channelParameters;
            }
            return (tokenManager.CreateSecurityTokenProvider(tokenRequirement) as SspiSecurityTokenProvider);
        }

        public static NetworkCredential GetSspiCredential(SecurityTokenManager credentialProvider, SecurityTokenRequirement sspiTokenRequirement, TimeSpan timeout, out bool extractGroupsForWindowsAccounts)
        {
            extractGroupsForWindowsAccounts = true;
            NetworkCredential credential = null;
            if (credentialProvider != null)
            {
                SecurityTokenProvider tokenProvider = credentialProvider.CreateSecurityTokenProvider(sspiTokenRequirement);
                if (tokenProvider == null)
                {
                    return credential;
                }
                TimeoutHelper helper = new TimeoutHelper(timeout);
                System.ServiceModel.Security.SecurityUtils.OpenTokenProviderIfRequired(tokenProvider, helper.RemainingTime());
                bool flag = false;
                try
                {
                    TokenImpersonationLevel level;
                    bool flag2;
                    credential = GetSspiCredential((SspiSecurityTokenProvider) tokenProvider, helper.RemainingTime(), out extractGroupsForWindowsAccounts, out level, out flag2);
                    flag = true;
                }
                finally
                {
                    if (!flag)
                    {
                        System.ServiceModel.Security.SecurityUtils.AbortTokenProviderIfRequired(tokenProvider);
                    }
                }
                System.ServiceModel.Security.SecurityUtils.CloseTokenProviderIfRequired(tokenProvider, helper.RemainingTime());
            }
            return credential;
        }

        public static NetworkCredential GetSspiCredential(SspiSecurityTokenProvider tokenProvider, TimeSpan timeout, out TokenImpersonationLevel impersonationLevel, out bool allowNtlm)
        {
            bool flag;
            return GetSspiCredential(tokenProvider, timeout, out flag, out impersonationLevel, out allowNtlm);
        }

        public static NetworkCredential GetSspiCredential(SecurityTokenProviderContainer tokenProvider, TimeSpan timeout, out TokenImpersonationLevel impersonationLevel, out AuthenticationLevel authenticationLevel)
        {
            bool flag;
            bool flag2;
            NetworkCredential credential = GetSspiCredential(tokenProvider.TokenProvider as SspiSecurityTokenProvider, timeout, out flag, out impersonationLevel, out flag2);
            authenticationLevel = flag2 ? AuthenticationLevel.MutualAuthRequested : AuthenticationLevel.MutualAuthRequired;
            return credential;
        }

        private static NetworkCredential GetSspiCredential(SspiSecurityTokenProvider tokenProvider, TimeSpan timeout, out bool extractGroupsForWindowsAccounts, out TokenImpersonationLevel impersonationLevel, out bool allowNtlm)
        {
            NetworkCredential networkCredential = null;
            extractGroupsForWindowsAccounts = true;
            impersonationLevel = TokenImpersonationLevel.Identification;
            allowNtlm = true;
            if (tokenProvider != null)
            {
                SspiSecurityToken token = GetToken<SspiSecurityToken>(tokenProvider, timeout);
                if (token != null)
                {
                    extractGroupsForWindowsAccounts = token.ExtractGroupsForWindowsAccounts;
                    impersonationLevel = token.ImpersonationLevel;
                    allowNtlm = token.AllowNtlm;
                    if (token.NetworkCredential != null)
                    {
                        networkCredential = token.NetworkCredential;
                        System.ServiceModel.Security.SecurityUtils.FixNetworkCredential(ref networkCredential);
                    }
                }
            }
            if (networkCredential == null)
            {
                networkCredential = CredentialCache.DefaultNetworkCredentials;
            }
            return networkCredential;
        }

        public static SspiSecurityTokenProvider GetSspiTokenProvider(SecurityTokenManager tokenManager, EndpointAddress target, Uri via, string transportScheme, out IdentityVerifier identityVerifier)
        {
            identityVerifier = null;
            if (tokenManager == null)
            {
                return null;
            }
            SspiSecurityTokenProvider provider = tokenManager.CreateSecurityTokenProvider(CreateSspiTokenRequirement(target, via, transportScheme)) as SspiSecurityTokenProvider;
            if (provider != null)
            {
                identityVerifier = IdentityVerifier.CreateDefault();
            }
            return provider;
        }

        public static SspiSecurityTokenProvider GetSspiTokenProvider(SecurityTokenManager tokenManager, EndpointAddress target, Uri via, string transportScheme, AuthenticationSchemes authenticationScheme, ChannelParameterCollection channelParameters)
        {
            if (tokenManager == null)
            {
                return null;
            }
            SecurityTokenRequirement tokenRequirement = CreateSspiTokenRequirement(target, via, transportScheme);
            tokenRequirement.Properties[ServiceModelSecurityTokenRequirement.HttpAuthenticationSchemeProperty] = authenticationScheme;
            if (channelParameters != null)
            {
                tokenRequirement.Properties[ServiceModelSecurityTokenRequirement.ChannelParametersCollectionProperty] = channelParameters;
            }
            return (tokenManager.CreateSecurityTokenProvider(tokenRequirement) as SspiSecurityTokenProvider);
        }

        private static T GetToken<T>(SecurityTokenProvider tokenProvider, TimeSpan timeout) where T: SecurityToken
        {
            SecurityToken token = tokenProvider.GetToken(timeout);
            if ((token != null) && !(token is T))
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(System.ServiceModel.SR.GetString("InvalidTokenProvided", new object[] { tokenProvider.GetType(), typeof(T) })));
            }
            return (token as T);
        }

        public static NetworkCredential GetUserNameCredential(SecurityTokenProviderContainer tokenProvider, TimeSpan timeout)
        {
            NetworkCredential credential = null;
            if ((tokenProvider != null) && (tokenProvider.TokenProvider != null))
            {
                UserNameSecurityToken token = GetToken<UserNameSecurityToken>(tokenProvider.TokenProvider, timeout);
                if (token != null)
                {
                    System.ServiceModel.Security.SecurityUtils.PrepareNetworkCredential();
                    credential = new NetworkCredential(token.UserName, token.Password);
                }
            }
            if (credential == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(System.ServiceModel.SR.GetString("NoUserNameTokenProvided")));
            }
            return credential;
        }

        public static SecurityTokenProvider GetUserNameTokenProvider(SecurityTokenManager tokenManager, EndpointAddress target, Uri via, string transportScheme, AuthenticationSchemes authenticationScheme, ChannelParameterCollection channelParameters)
        {
            SecurityTokenProvider provider = null;
            if (tokenManager == null)
            {
                return provider;
            }
            SecurityTokenRequirement tokenRequirement = CreateUserNameTokenRequirement(target, via, transportScheme);
            tokenRequirement.Properties[ServiceModelSecurityTokenRequirement.HttpAuthenticationSchemeProperty] = authenticationScheme;
            if (channelParameters != null)
            {
                tokenRequirement.Properties[ServiceModelSecurityTokenRequirement.ChannelParametersCollectionProperty] = channelParameters;
            }
            return tokenManager.CreateSecurityTokenProvider(tokenRequirement);
        }

        private class GetSspiCredentialAsyncResult : AsyncResult
        {
            private bool allowNtlm;
            private NetworkCredential credential;
            private SspiSecurityTokenProvider credentialProvider;
            private TokenImpersonationLevel impersonationLevel;
            private static AsyncCallback onGetToken;

            public GetSspiCredentialAsyncResult(SspiSecurityTokenProvider credentialProvider, TimeSpan timeout, AsyncCallback callback, object state) : base(callback, state)
            {
                this.allowNtlm = true;
                this.impersonationLevel = TokenImpersonationLevel.Identification;
                if (credentialProvider == null)
                {
                    this.EnsureCredentialInitialized();
                    base.Complete(true);
                }
                else
                {
                    this.credentialProvider = credentialProvider;
                    if (onGetToken == null)
                    {
                        onGetToken = DiagnosticUtility.ThunkAsyncCallback(new AsyncCallback(TransportSecurityHelpers.GetSspiCredentialAsyncResult.OnGetToken));
                    }
                    IAsyncResult result = credentialProvider.BeginGetToken(timeout, onGetToken, this);
                    if (result.CompletedSynchronously)
                    {
                        this.CompleteGetToken(result);
                        base.Complete(true);
                    }
                }
            }

            private void CompleteGetToken(IAsyncResult result)
            {
                SspiSecurityToken token = (SspiSecurityToken) this.credentialProvider.EndGetToken(result);
                if (token != null)
                {
                    this.impersonationLevel = token.ImpersonationLevel;
                    this.allowNtlm = token.AllowNtlm;
                    if (token.NetworkCredential != null)
                    {
                        this.credential = token.NetworkCredential;
                        System.ServiceModel.Security.SecurityUtils.FixNetworkCredential(ref this.credential);
                    }
                }
                this.EnsureCredentialInitialized();
            }

            public static NetworkCredential End(IAsyncResult result, out TokenImpersonationLevel impersonationLevel, out bool allowNtlm)
            {
                TransportSecurityHelpers.GetSspiCredentialAsyncResult result2 = AsyncResult.End<TransportSecurityHelpers.GetSspiCredentialAsyncResult>(result);
                impersonationLevel = result2.impersonationLevel;
                allowNtlm = result2.allowNtlm;
                return result2.credential;
            }

            public static NetworkCredential End(IAsyncResult result, out TokenImpersonationLevel impersonationLevel, out AuthenticationLevel authenticationLevel)
            {
                TransportSecurityHelpers.GetSspiCredentialAsyncResult result2 = AsyncResult.End<TransportSecurityHelpers.GetSspiCredentialAsyncResult>(result);
                impersonationLevel = result2.impersonationLevel;
                authenticationLevel = result2.allowNtlm ? AuthenticationLevel.MutualAuthRequested : AuthenticationLevel.MutualAuthRequired;
                return result2.credential;
            }

            private void EnsureCredentialInitialized()
            {
                if (this.credential == null)
                {
                    this.credential = CredentialCache.DefaultNetworkCredentials;
                }
            }

            private static void OnGetToken(IAsyncResult result)
            {
                if (!result.CompletedSynchronously)
                {
                    TransportSecurityHelpers.GetSspiCredentialAsyncResult asyncState = (TransportSecurityHelpers.GetSspiCredentialAsyncResult) result.AsyncState;
                    Exception exception = null;
                    try
                    {
                        asyncState.CompleteGetToken(result);
                    }
                    catch (Exception exception2)
                    {
                        if (DiagnosticUtility.IsFatal(exception2))
                        {
                            throw;
                        }
                        exception = exception2;
                    }
                    asyncState.Complete(false, exception);
                }
            }
        }

        private class GetUserNameCredentialAsyncResult : AsyncResult
        {
            private NetworkCredential credential;
            private static AsyncCallback onGetToken = DiagnosticUtility.ThunkAsyncCallback(new AsyncCallback(TransportSecurityHelpers.GetUserNameCredentialAsyncResult.OnGetToken));
            private SecurityTokenProvider tokenProvider;

            public GetUserNameCredentialAsyncResult(SecurityTokenProviderContainer tokenProvider, TimeSpan timeout, AsyncCallback callback, object state) : base(callback, state)
            {
                if ((tokenProvider == null) || (tokenProvider.TokenProvider == null))
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(System.ServiceModel.SR.GetString("NoUserNameTokenProvided")));
                }
                this.tokenProvider = tokenProvider.TokenProvider;
                IAsyncResult result = this.tokenProvider.BeginGetToken(timeout, onGetToken, this);
                if (result.CompletedSynchronously)
                {
                    this.CompleteGetToken(result);
                    base.Complete(true);
                }
            }

            private void CompleteGetToken(IAsyncResult result)
            {
                UserNameSecurityToken token = (UserNameSecurityToken) this.tokenProvider.EndGetToken(result);
                if (token == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(System.ServiceModel.SR.GetString("NoUserNameTokenProvided")));
                }
                System.ServiceModel.Security.SecurityUtils.PrepareNetworkCredential();
                this.credential = new NetworkCredential(token.UserName, token.Password);
            }

            public static NetworkCredential End(IAsyncResult result) => 
                AsyncResult.End<TransportSecurityHelpers.GetUserNameCredentialAsyncResult>(result).credential;

            private static void OnGetToken(IAsyncResult result)
            {
                if (!result.CompletedSynchronously)
                {
                    TransportSecurityHelpers.GetUserNameCredentialAsyncResult asyncState = (TransportSecurityHelpers.GetUserNameCredentialAsyncResult) result.AsyncState;
                    Exception exception = null;
                    try
                    {
                        asyncState.CompleteGetToken(result);
                    }
                    catch (Exception exception2)
                    {
                        if (DiagnosticUtility.IsFatal(exception2))
                        {
                            throw;
                        }
                        exception = exception2;
                    }
                    asyncState.Complete(false, exception);
                }
            }
        }
    }
}

