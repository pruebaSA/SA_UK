namespace System.Net
{
    using System;
    using System.Security.Authentication.ExtendedProtection;

    internal class NegotiateClient : ISessionAuthenticationModule, IAuthenticationModule
    {
        internal const string AuthType = "Negotiate";
        internal static string Signature = "Negotiate".ToLower(CultureInfo.InvariantCulture);
        internal static int SignatureSize = Signature.Length;

        public NegotiateClient()
        {
            if (!ComNetOS.IsWin2K)
            {
                throw new PlatformNotSupportedException(SR.GetString("Win2000Required"));
            }
        }

        public Authorization Authenticate(string challenge, WebRequest webRequest, ICredentials credentials) => 
            this.DoAuthenticate(challenge, webRequest, credentials, false);

        public void ClearSession(WebRequest webRequest)
        {
            HttpWebRequest request = webRequest as HttpWebRequest;
            request.CurrentAuthenticationState.ClearSession();
        }

        private Authorization DoAuthenticate(string challenge, WebRequest webRequest, ICredentials credentials, bool preAuthenticate)
        {
            if (credentials == null)
            {
                return null;
            }
            HttpWebRequest request = webRequest as HttpWebRequest;
            NTAuthentication securityContext = null;
            string incomingBlob = null;
            if (!preAuthenticate)
            {
                int index = AuthenticationManager.FindSubstringNotInQuotes(challenge, Signature);
                if (index < 0)
                {
                    return null;
                }
                int startIndex = index + SignatureSize;
                if ((challenge.Length > startIndex) && (challenge[startIndex] != ','))
                {
                    startIndex++;
                }
                else
                {
                    index = -1;
                }
                if ((index >= 0) && (challenge.Length > startIndex))
                {
                    index = challenge.IndexOf(',', startIndex);
                    if (index != -1)
                    {
                        incomingBlob = challenge.Substring(startIndex, index - startIndex);
                    }
                    else
                    {
                        incomingBlob = challenge.Substring(startIndex);
                    }
                }
                securityContext = request.CurrentAuthenticationState.GetSecurityContext(this);
            }
            if (securityContext == null)
            {
                NetworkCredential credential = credentials.GetCredential(request.ChallengedUri, Signature);
                string str2 = string.Empty;
                if ((credential == null) || (!(credential is SystemNetworkCredential) && ((str2 = credential.InternalGetUserName()).Length == 0)))
                {
                    return null;
                }
                if (((str2.Length + credential.InternalGetPassword().Length) + credential.InternalGetDomain().Length) > 0x20f)
                {
                    return null;
                }
                ICredentialPolicy credentialPolicy = AuthenticationManager.CredentialPolicy;
                if ((credentialPolicy != null) && !credentialPolicy.ShouldSendCredential(request.ChallengedUri, request, credential, this))
                {
                    return null;
                }
                string computeSpn = request.CurrentAuthenticationState.GetComputeSpn(request);
                ChannelBinding channelBinding = null;
                if (request.CurrentAuthenticationState.TransportContext != null)
                {
                    channelBinding = request.CurrentAuthenticationState.TransportContext.GetChannelBinding(ChannelBindingKind.Endpoint);
                }
                securityContext = new NTAuthentication("Negotiate", credential, computeSpn, request, channelBinding);
                request.CurrentAuthenticationState.SetSecurityContext(securityContext, this);
            }
            string outgoingBlob = securityContext.GetOutgoingBlob(incomingBlob);
            if (outgoingBlob == null)
            {
                return null;
            }
            bool unsafeOrProxyAuthenticatedConnectionSharing = request.UnsafeOrProxyAuthenticatedConnectionSharing;
            if (unsafeOrProxyAuthenticatedConnectionSharing)
            {
                request.LockConnection = true;
            }
            request.NtlmKeepAlive = ((incomingBlob == null) && securityContext.IsValidContext) && !securityContext.IsKerberos;
            return AuthenticationManager.GetGroupAuthorization(this, "Negotiate " + outgoingBlob, securityContext.IsCompleted, securityContext, unsafeOrProxyAuthenticatedConnectionSharing, securityContext.IsKerberos);
        }

        public Authorization PreAuthenticate(WebRequest webRequest, ICredentials credentials) => 
            this.DoAuthenticate(null, webRequest, credentials, true);

        public bool Update(string challenge, WebRequest webRequest)
        {
            HttpWebRequest request = webRequest as HttpWebRequest;
            NTAuthentication securityContext = request.CurrentAuthenticationState.GetSecurityContext(this);
            if (securityContext != null)
            {
                if (!securityContext.IsCompleted && (request.CurrentAuthenticationState.StatusCodeMatch == request.ResponseStatusCode))
                {
                    return false;
                }
                if (!request.UnsafeOrProxyAuthenticatedConnectionSharing)
                {
                    request.ServicePoint.ReleaseConnectionGroup(request.GetConnectionGroupLine());
                }
                int num = (challenge == null) ? -1 : AuthenticationManager.FindSubstringNotInQuotes(challenge, Signature);
                if (num >= 0)
                {
                    int startIndex = num + SignatureSize;
                    string incomingBlob = null;
                    if ((challenge.Length > startIndex) && (challenge[startIndex] != ','))
                    {
                        startIndex++;
                    }
                    else
                    {
                        num = -1;
                    }
                    if ((num >= 0) && (challenge.Length > startIndex))
                    {
                        incomingBlob = challenge.Substring(startIndex);
                    }
                    securityContext.GetOutgoingBlob(incomingBlob);
                    request.CurrentAuthenticationState.Authorization.MutuallyAuthenticated = securityContext.IsMutualAuthFlag;
                }
                request.ServicePoint.SetCachedChannelBinding(request.ChallengedUri, securityContext.ChannelBinding);
                this.ClearSession(request);
            }
            return true;
        }

        public string AuthenticationType =>
            "Negotiate";

        public bool CanPreAuthenticate =>
            true;

        public bool CanUseDefaultCredentials =>
            true;
    }
}

