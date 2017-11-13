namespace System.ServiceModel.Diagnostics
{
    using System;
    using System.Diagnostics;
    using System.Globalization;
    using System.IdentityModel.Claims;
    using System.IdentityModel.Policy;
    using System.IdentityModel.Selectors;
    using System.IdentityModel.Tokens;
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Dispatcher;
    using System.ServiceModel.Security;
    using System.ServiceModel.Security.Tokens;
    using System.Xml;

    internal static class SecurityTraceRecordHelper
    {
        internal static void TraceActiveSessionRemoved(UniqueId sessionId, Uri listenAddress)
        {
            if (DiagnosticUtility.ShouldTraceInformation)
            {
                DiagnosticUtility.DiagnosticTrace.TraceEvent(TraceEventType.Information, TraceCode.SecurityActiveServerSessionRemoved, System.ServiceModel.SR.GetString("TraceCodeSecurityActiveServerSessionRemoved"), new ServerSessionTraceRecord(sessionId, listenAddress));
            }
        }

        internal static void TraceBeginSecurityNegotiation<T>(IssuanceTokenProviderBase<T> provider, EndpointAddress target) where T: IssuanceTokenProviderState
        {
            if (DiagnosticUtility.ShouldTraceInformation)
            {
                DiagnosticUtility.DiagnosticTrace.TraceEvent(TraceEventType.Information, TraceCode.IssuanceTokenProviderBeginSecurityNegotiation, System.ServiceModel.SR.GetString("TraceCodeIssuanceTokenProviderBeginSecurityNegotiation"), new IssuanceProviderTraceRecord<T>(provider, target));
            }
        }

        internal static void TraceBeginSecuritySessionOperation(SecuritySessionOperation operation, EndpointAddress target, SecurityToken currentToken)
        {
            if (DiagnosticUtility.ShouldTraceInformation)
            {
                DiagnosticUtility.DiagnosticTrace.TraceEvent(TraceEventType.Information, TraceCode.SecuritySessionRequestorStartOperation, System.ServiceModel.SR.GetString("TraceCodeSecuritySessionRequestorStartOperation"), new SessionRequestorTraceRecord(operation, currentToken, null, target));
            }
        }

        internal static void TraceClientServiceTokenCacheFull<T>(IssuanceTokenProviderBase<T> provider, int cacheSize) where T: IssuanceTokenProviderState
        {
            if (DiagnosticUtility.ShouldTraceInformation)
            {
                DiagnosticUtility.DiagnosticTrace.TraceEvent(TraceEventType.Information, TraceCode.IssuanceTokenProviderServiceTokenCacheFull, System.ServiceModel.SR.GetString("TraceCodeIssuanceTokenProviderServiceTokenCacheFull"), new IssuanceProviderTraceRecord<T>(provider, cacheSize));
            }
        }

        internal static void TraceClientSpnego(WindowsSspiNegotiation windowsNegotiation)
        {
            if (DiagnosticUtility.ShouldTraceInformation)
            {
                DiagnosticUtility.DiagnosticTrace.TraceEvent(TraceEventType.Information, TraceCode.SpnegoClientNegotiationCompleted, System.ServiceModel.SR.GetString("TraceCodeSpnegoClientNegotiationCompleted"), new WindowsSspiNegotiationTraceRecord(windowsNegotiation, true));
            }
        }

        internal static void TraceCloseMessageReceived(SecurityToken sessionToken, EndpointAddress remoteTarget)
        {
            if (DiagnosticUtility.ShouldTraceInformation)
            {
                DiagnosticUtility.DiagnosticTrace.TraceEvent(TraceEventType.Information, TraceCode.SecurityClientSessionCloseMessageReceived, System.ServiceModel.SR.GetString("TraceCodeSecurityClientSessionCloseMessageReceived"), new ClientSessionTraceRecord(sessionToken, null, remoteTarget));
            }
        }

        internal static void TraceCloseMessageSent(SecurityToken sessionToken, EndpointAddress remoteTarget)
        {
            if (DiagnosticUtility.ShouldTraceInformation)
            {
                DiagnosticUtility.DiagnosticTrace.TraceEvent(TraceEventType.Information, TraceCode.SecurityClientSessionCloseSent, System.ServiceModel.SR.GetString("TraceCodeSecurityClientSessionCloseSent"), new ClientSessionTraceRecord(sessionToken, null, remoteTarget));
            }
        }

        internal static void TraceCloseResponseMessageSent(SecurityToken sessionToken, EndpointAddress remoteTarget)
        {
            if (DiagnosticUtility.ShouldTraceInformation)
            {
                DiagnosticUtility.DiagnosticTrace.TraceEvent(TraceEventType.Information, TraceCode.SecurityClientSessionCloseResponseSent, System.ServiceModel.SR.GetString("TraceCodeSecurityClientSessionCloseResponseSent"), new ClientSessionTraceRecord(sessionToken, null, remoteTarget));
            }
        }

        internal static void TraceCloseResponseReceived(SecurityToken sessionToken, EndpointAddress remoteTarget)
        {
            if (DiagnosticUtility.ShouldTraceInformation)
            {
                DiagnosticUtility.DiagnosticTrace.TraceEvent(TraceEventType.Information, TraceCode.SecuritySessionClosedResponseReceived, System.ServiceModel.SR.GetString("TraceCodeSecuritySessionClosedResponseReceived"), new ClientSessionTraceRecord(sessionToken, null, remoteTarget));
            }
        }

        internal static void TraceEndSecurityNegotiation<T>(IssuanceTokenProviderBase<T> provider, SecurityToken serviceToken, EndpointAddress target) where T: IssuanceTokenProviderState
        {
            if (DiagnosticUtility.ShouldTraceInformation)
            {
                DiagnosticUtility.DiagnosticTrace.TraceEvent(TraceEventType.Information, TraceCode.IssuanceTokenProviderEndSecurityNegotiation, System.ServiceModel.SR.GetString("TraceCodeIssuanceTokenProviderEndSecurityNegotiation"), new IssuanceProviderTraceRecord<T>(provider, serviceToken, target));
            }
        }

        internal static void TraceExportChannelBindingEntry()
        {
            if (DiagnosticUtility.ShouldTraceInformation)
            {
                DiagnosticUtility.DiagnosticTrace.TraceEvent(TraceEventType.Information, TraceCode.ExportSecurityChannelBindingEntry, System.ServiceModel.SR.GetString("TraceCodeExportSecurityChannelBindingEntry"), null);
            }
        }

        internal static void TraceExportChannelBindingExit()
        {
            if (DiagnosticUtility.ShouldTraceInformation)
            {
                DiagnosticUtility.DiagnosticTrace.TraceEvent(TraceEventType.Information, TraceCode.ExportSecurityChannelBindingExit, System.ServiceModel.SR.GetString("TraceCodeExportSecurityChannelBindingExit"));
            }
        }

        internal static void TraceIdentityDeterminationFailure(EndpointAddress epr, Type identityVerifier)
        {
            if (DiagnosticUtility.ShouldTraceInformation)
            {
                DiagnosticUtility.DiagnosticTrace.TraceEvent(TraceEventType.Information, TraceCode.SecurityIdentityDeterminationFailure, System.ServiceModel.SR.GetString("TraceCodeSecurityIdentityDeterminationFailure"), new IdentityDeterminationFailureTraceRecord(epr, identityVerifier));
            }
        }

        internal static void TraceIdentityDeterminationSuccess(EndpointAddress epr, EndpointIdentity identity, Type identityVerifier)
        {
            if (DiagnosticUtility.ShouldTraceInformation)
            {
                DiagnosticUtility.DiagnosticTrace.TraceEvent(TraceEventType.Information, TraceCode.SecurityIdentityDeterminationSuccess, System.ServiceModel.SR.GetString("TraceCodeSecurityIdentityDeterminationSuccess"), new IdentityDeterminationSuccessTraceRecord(epr, identity, identityVerifier));
            }
        }

        internal static void TraceIdentityHostNameNormalizationFailure(EndpointAddress epr, Type identityVerifier, Exception e)
        {
            if (DiagnosticUtility.ShouldTraceInformation)
            {
                DiagnosticUtility.DiagnosticTrace.TraceEvent(TraceEventType.Information, TraceCode.SecurityIdentityHostNameNormalizationFailure, System.ServiceModel.SR.GetString("TraceCodeSecurityIdentityHostNameNormalizationFailure"), new IdentityHostNameNormalizationFailureTraceRecord(epr, identityVerifier, e));
            }
        }

        internal static void TraceIdentityVerificationFailure(EndpointIdentity identity, AuthorizationContext authContext, Type identityVerifier)
        {
            if (DiagnosticUtility.ShouldTraceInformation)
            {
                DiagnosticUtility.DiagnosticTrace.TraceEvent(TraceEventType.Information, TraceCode.SecurityIdentityVerificationFailure, System.ServiceModel.SR.GetString("TraceCodeSecurityIdentityVerificationFailure"), new IdentityVerificationFailureTraceRecord(identity, authContext, identityVerifier));
            }
        }

        internal static void TraceIdentityVerificationSuccess(EndpointIdentity identity, Claim claim, Type identityVerifier)
        {
            if (DiagnosticUtility.ShouldTraceInformation)
            {
                DiagnosticUtility.DiagnosticTrace.TraceEvent(TraceEventType.Information, TraceCode.SecurityIdentityVerificationSuccess, System.ServiceModel.SR.GetString("TraceCodeSecurityIdentityVerificationSuccess"), new IdentityVerificationSuccessTraceRecord(identity, claim, identityVerifier));
            }
        }

        internal static void TraceImpersonationFailed(DispatchOperationRuntime operation, Exception e)
        {
            if (DiagnosticUtility.ShouldTraceWarning)
            {
                DiagnosticUtility.DiagnosticTrace.TraceEvent(TraceEventType.Warning, TraceCode.SecurityImpersonationFailure, System.ServiceModel.SR.GetString("TraceCodeSecurityImpersonationFailure"), new ImpersonationTraceRecord(operation), e);
            }
        }

        internal static void TraceImpersonationSucceeded(DispatchOperationRuntime operation)
        {
            if (DiagnosticUtility.ShouldTraceInformation)
            {
                DiagnosticUtility.DiagnosticTrace.TraceEvent(TraceEventType.Information, TraceCode.SecurityImpersonationSuccess, System.ServiceModel.SR.GetString("TraceCodeSecurityImpersonationSuccess"), new ImpersonationTraceRecord(operation));
            }
        }

        internal static void TraceImportChannelBindingEntry()
        {
            if (DiagnosticUtility.ShouldTraceInformation)
            {
                DiagnosticUtility.DiagnosticTrace.TraceEvent(TraceEventType.Information, TraceCode.ImportSecurityChannelBindingEntry, System.ServiceModel.SR.GetString("TraceCodeImportSecurityChannelBindingEntry"), null);
            }
        }

        internal static void TraceImportChannelBindingExit()
        {
            if (DiagnosticUtility.ShouldTraceInformation)
            {
                DiagnosticUtility.DiagnosticTrace.TraceEvent(TraceEventType.Information, TraceCode.ImportSecurityChannelBindingExit, System.ServiceModel.SR.GetString("TraceCodeImportSecurityChannelBindingExit"));
            }
        }

        internal static void TraceInactiveSessionFaulted(SecurityContextSecurityToken sessionToken, Uri listenAddress)
        {
            if (DiagnosticUtility.ShouldTraceWarning)
            {
                DiagnosticUtility.DiagnosticTrace.TraceEvent(TraceEventType.Warning, TraceCode.SecurityInactiveSessionFaulted, System.ServiceModel.SR.GetString("TraceCodeSecurityInactiveSessionFaulted"), new ServerSessionTraceRecord(sessionToken, null, listenAddress));
            }
        }

        internal static void TraceIncomingMessageVerified(SecurityProtocol binding, Message message)
        {
            if (DiagnosticUtility.ShouldTraceInformation)
            {
                TraceUtility.TraceEvent(TraceEventType.Information, TraceCode.SecurityBindingIncomingMessageVerified, new MessageSecurityTraceRecord(binding, message), null, null, message);
            }
        }

        internal static void TraceNegotiationTokenAuthenticatorAttached<T>(NegotiationTokenAuthenticator<T> authenticator, IChannelListener transportChannelListener) where T: NegotiationTokenAuthenticatorState
        {
            if (DiagnosticUtility.ShouldTraceInformation)
            {
                DiagnosticUtility.DiagnosticTrace.TraceEvent(TraceEventType.Information, TraceCode.NegotiationAuthenticatorAttached, System.ServiceModel.SR.GetString("TraceCodeNegotiationAuthenticatorAttached"), new NegotiationAuthenticatorTraceRecord<T>(authenticator, transportChannelListener));
            }
        }

        internal static void TraceNewServerSessionKeyIssued(SecurityContextSecurityToken newToken, SecurityContextSecurityToken supportingToken, Uri listenAddress)
        {
            if (DiagnosticUtility.ShouldTraceInformation)
            {
                DiagnosticUtility.DiagnosticTrace.TraceEvent(TraceEventType.Information, TraceCode.SecurityNewServerSessionKeyIssued, System.ServiceModel.SR.GetString("TraceCodeSecurityNewServerSessionKeyIssued"), new ServerSessionTraceRecord(newToken, supportingToken, listenAddress));
            }
        }

        internal static void TraceOutgoingMessageSecured(SecurityProtocol binding, Message message)
        {
            if (DiagnosticUtility.ShouldTraceInformation)
            {
                TraceUtility.TraceEvent(TraceEventType.Information, TraceCode.SecurityBindingOutgoingMessageSecured, new MessageSecurityTraceRecord(binding, message), null, null, message);
            }
        }

        internal static void TracePendingSessionActivated(UniqueId sessionId, Uri listenAddress)
        {
            if (DiagnosticUtility.ShouldTraceInformation)
            {
                DiagnosticUtility.DiagnosticTrace.TraceEvent(TraceEventType.Information, TraceCode.SecurityPendingServerSessionActivated, System.ServiceModel.SR.GetString("TraceCodeSecurityPendingServerSessionActivated"), new ServerSessionTraceRecord(sessionId, listenAddress));
            }
        }

        internal static void TracePendingSessionAdded(UniqueId sessionId, Uri listenAddress)
        {
            if (DiagnosticUtility.ShouldTraceInformation)
            {
                DiagnosticUtility.DiagnosticTrace.TraceEvent(TraceEventType.Information, TraceCode.SecurityPendingServerSessionAdded, System.ServiceModel.SR.GetString("TraceCodeSecurityPendingServerSessionAdded"), new ServerSessionTraceRecord(sessionId, listenAddress));
            }
        }

        internal static void TracePendingSessionClosed(UniqueId sessionId, Uri listenAddress)
        {
            if (DiagnosticUtility.ShouldTraceWarning)
            {
                DiagnosticUtility.DiagnosticTrace.TraceEvent(TraceEventType.Warning, TraceCode.SecurityPendingServerSessionClosed, System.ServiceModel.SR.GetString("TraceCodeSecurityPendingServerSessionClosed"), new ServerSessionTraceRecord(sessionId, listenAddress));
            }
        }

        internal static void TracePreviousSessionKeyDiscarded(SecurityToken previousSessionToken, SecurityToken currentSessionToken, EndpointAddress remoteAddress)
        {
            if (DiagnosticUtility.ShouldTraceInformation)
            {
                DiagnosticUtility.DiagnosticTrace.TraceEvent(TraceEventType.Information, TraceCode.SecurityClientSessionPreviousKeyDiscarded, System.ServiceModel.SR.GetString("TraceCodeSecurityClientSessionPreviousKeyDiscarded"), new ClientSessionTraceRecord(currentSessionToken, previousSessionToken, remoteAddress));
            }
        }

        internal static void TraceRedirectApplied<T>(IssuanceTokenProviderBase<T> provider, EndpointAddress newTarget, EndpointAddress oldTarget) where T: IssuanceTokenProviderState
        {
            if (DiagnosticUtility.ShouldTraceInformation)
            {
                DiagnosticUtility.DiagnosticTrace.TraceEvent(TraceEventType.Information, TraceCode.IssuanceTokenProviderRedirectApplied, System.ServiceModel.SR.GetString("TraceCodeIssuanceTokenProviderRedirectApplied"), new IssuanceProviderTraceRecord<T>(provider, newTarget, oldTarget));
            }
        }

        internal static void TraceRemoteSessionAbortedFault(SecurityToken sessionToken, EndpointAddress remoteTarget)
        {
            if (DiagnosticUtility.ShouldTraceWarning)
            {
                DiagnosticUtility.DiagnosticTrace.TraceEvent(TraceEventType.Warning, TraceCode.SecuritySessionAbortedFaultReceived, System.ServiceModel.SR.GetString("TraceCodeSecuritySessionAbortedFaultReceived"), new ClientSessionTraceRecord(sessionToken, null, remoteTarget));
            }
        }

        internal static void TraceRemovedCachedServiceToken<T>(IssuanceTokenProviderBase<T> provider, SecurityToken serviceToken) where T: IssuanceTokenProviderState
        {
            if (DiagnosticUtility.ShouldTraceInformation)
            {
                DiagnosticUtility.DiagnosticTrace.TraceEvent(TraceEventType.Information, TraceCode.IssuanceTokenProviderRemovedCachedToken, System.ServiceModel.SR.GetString("TraceCodeIssuanceTokenProviderRemovedCachedToken"), new IssuanceProviderTraceRecord<T>(provider, serviceToken));
            }
        }

        internal static void TraceRenewFaultSendFailure(SecurityContextSecurityToken sessionToken, Uri listenAddress, Exception e)
        {
            if (DiagnosticUtility.ShouldTraceWarning)
            {
                DiagnosticUtility.DiagnosticTrace.TraceEvent(TraceEventType.Warning, TraceCode.SecuritySessionRenewFaultSendFailure, System.ServiceModel.SR.GetString("TraceCodeSecuritySessionRenewFaultSendFailure"), new ServerSessionTraceRecord(sessionToken, listenAddress), e);
            }
        }

        internal static void TraceSecureOutgoingMessageFailure(SecurityProtocol binding, Message message)
        {
            if (DiagnosticUtility.ShouldTraceWarning)
            {
                TraceUtility.TraceEvent(TraceEventType.Warning, TraceCode.SecurityBindingSecureOutgoingMessageFailure, new MessageSecurityTraceRecord(binding, message), null, null, message);
            }
        }

        internal static void TraceSecurityContextTokenCacheFull(int capacity, int pruningAmount)
        {
            if (DiagnosticUtility.ShouldTraceInformation)
            {
                DiagnosticUtility.DiagnosticTrace.TraceEvent(TraceEventType.Information, TraceCode.SecurityContextTokenCacheFull, System.ServiceModel.SR.GetString("TraceCodeSecurityContextTokenCacheFull"), new SecurityContextTokenCacheTraceRecord(capacity, pruningAmount));
            }
        }

        internal static void TraceSecuritySessionOperationFailure(SecuritySessionOperation operation, EndpointAddress target, SecurityToken currentToken, Exception e)
        {
            if (DiagnosticUtility.ShouldTraceWarning)
            {
                DiagnosticUtility.DiagnosticTrace.TraceEvent(TraceEventType.Warning, TraceCode.SecuritySessionRequestorOperationFailure, System.ServiceModel.SR.GetString("TraceCodeSecuritySessionRequestorOperationFailure"), new SessionRequestorTraceRecord(operation, currentToken, e, target));
            }
        }

        internal static void TraceSecuritySessionOperationSuccess(SecuritySessionOperation operation, EndpointAddress target, SecurityToken currentToken, SecurityToken issuedToken)
        {
            if (DiagnosticUtility.ShouldTraceInformation)
            {
                DiagnosticUtility.DiagnosticTrace.TraceEvent(TraceEventType.Information, TraceCode.SecuritySessionRequestorOperationSuccess, System.ServiceModel.SR.GetString("TraceCodeSecuritySessionRequestorOperationSuccess"), new SessionRequestorTraceRecord(operation, currentToken, issuedToken, target));
            }
        }

        internal static void TraceServerSessionCloseReceived(SecurityContextSecurityToken sessionToken, Uri listenAddress)
        {
            if (DiagnosticUtility.ShouldTraceInformation)
            {
                DiagnosticUtility.DiagnosticTrace.TraceEvent(TraceEventType.Information, TraceCode.SecurityServerSessionCloseReceived, System.ServiceModel.SR.GetString("TraceCodeSecurityServerSessionCloseReceived"), new ServerSessionTraceRecord(sessionToken, null, listenAddress));
            }
        }

        internal static void TraceServerSessionCloseResponseReceived(SecurityContextSecurityToken sessionToken, Uri listenAddress)
        {
            if (DiagnosticUtility.ShouldTraceInformation)
            {
                DiagnosticUtility.DiagnosticTrace.TraceEvent(TraceEventType.Information, TraceCode.SecurityServerSessionCloseResponseReceived, System.ServiceModel.SR.GetString("TraceCodeSecurityServerSessionCloseResponseReceived"), new ServerSessionTraceRecord(sessionToken, null, listenAddress));
            }
        }

        internal static void TraceServerSessionKeyUpdated(SecurityContextSecurityToken sessionToken, Uri listenAddress)
        {
            if (DiagnosticUtility.ShouldTraceInformation)
            {
                DiagnosticUtility.DiagnosticTrace.TraceEvent(TraceEventType.Information, TraceCode.SecurityServerSessionKeyUpdated, System.ServiceModel.SR.GetString("TraceCodeSecurityServerSessionKeyUpdated"), new ServerSessionTraceRecord(sessionToken, null, listenAddress));
            }
        }

        internal static void TraceServerSessionOperationException(SecuritySessionOperation operation, Exception e, Uri listenAddress)
        {
            if (DiagnosticUtility.ShouldTraceWarning)
            {
                DiagnosticUtility.DiagnosticTrace.TraceEvent(TraceEventType.Warning, TraceCode.SecuritySessionResponderOperationFailure, System.ServiceModel.SR.GetString("TraceCodeSecuritySessionResponderOperationFailure"), new SessionResponderTraceRecord(operation, e, listenAddress));
            }
        }

        internal static void TraceServiceSecurityNegotiationCompleted<T>(NegotiationTokenAuthenticator<T> authenticator, SecurityContextSecurityToken serviceToken) where T: NegotiationTokenAuthenticatorState
        {
            if (DiagnosticUtility.ShouldTraceInformation)
            {
                DiagnosticUtility.DiagnosticTrace.TraceEvent(TraceEventType.Information, TraceCode.ServiceSecurityNegotiationCompleted, System.ServiceModel.SR.GetString("TraceCodeServiceSecurityNegotiationCompleted"), new NegotiationAuthenticatorTraceRecord<T>(authenticator, serviceToken));
            }
        }

        internal static void TraceServiceSecurityNegotiationFailure<T>(NegotiationTokenAuthenticator<T> authenticator, Exception e) where T: NegotiationTokenAuthenticatorState
        {
            if (DiagnosticUtility.ShouldTraceWarning)
            {
                DiagnosticUtility.DiagnosticTrace.TraceEvent(TraceEventType.Warning, TraceCode.SecurityNegotiationProcessingFailure, System.ServiceModel.SR.GetString("TraceCodeSecurityNegotiationProcessingFailure"), new NegotiationAuthenticatorTraceRecord<T>(authenticator, e));
            }
        }

        internal static void TraceServiceSpnego(WindowsSspiNegotiation windowsNegotiation)
        {
            if (DiagnosticUtility.ShouldTraceInformation)
            {
                DiagnosticUtility.DiagnosticTrace.TraceEvent(TraceEventType.Information, TraceCode.SpnegoServiceNegotiationCompleted, System.ServiceModel.SR.GetString("TraceCodeSpnegoServiceNegotiationCompleted"), new WindowsSspiNegotiationTraceRecord(windowsNegotiation, false));
            }
        }

        internal static void TraceSessionAbortedFaultSendFailure(SecurityContextSecurityToken sessionToken, Uri listenAddress, Exception e)
        {
            if (DiagnosticUtility.ShouldTraceWarning)
            {
                DiagnosticUtility.DiagnosticTrace.TraceEvent(TraceEventType.Warning, TraceCode.SecuritySessionAbortedFaultSendFailure, System.ServiceModel.SR.GetString("TraceCodeSecuritySessionAbortedFaultSendFailure"), new ServerSessionTraceRecord(sessionToken, listenAddress), e);
            }
        }

        internal static void TraceSessionAbortedFaultSent(SecurityContextSecurityToken sessionToken, Uri listenAddress)
        {
            if (DiagnosticUtility.ShouldTraceWarning)
            {
                DiagnosticUtility.DiagnosticTrace.TraceEvent(TraceEventType.Warning, TraceCode.SecurityServerSessionAbortedFaultSent, System.ServiceModel.SR.GetString("TraceCodeSecurityServerSessionAbortedFaultSent"), new ServerSessionTraceRecord(sessionToken, null, listenAddress));
            }
        }

        internal static void TraceSessionClosedResponseSendFailure(SecurityContextSecurityToken sessionToken, Uri listenAddress, Exception e)
        {
            if (DiagnosticUtility.ShouldTraceWarning)
            {
                DiagnosticUtility.DiagnosticTrace.TraceEvent(TraceEventType.Warning, TraceCode.SecuritySessionClosedResponseSendFailure, System.ServiceModel.SR.GetString("TraceCodeSecuritySessionClosedResponseSendFailure"), new ServerSessionTraceRecord(sessionToken, listenAddress), e);
            }
        }

        internal static void TraceSessionClosedResponseSent(SecurityContextSecurityToken sessionToken, Uri listenAddress)
        {
            if (DiagnosticUtility.ShouldTraceInformation)
            {
                DiagnosticUtility.DiagnosticTrace.TraceEvent(TraceEventType.Information, TraceCode.SecuritySessionCloseResponseSent, System.ServiceModel.SR.GetString("TraceCodeSecuritySessionCloseResponseSent"), new ServerSessionTraceRecord(sessionToken, null, listenAddress));
            }
        }

        internal static void TraceSessionClosedSent(SecurityContextSecurityToken sessionToken, Uri listenAddress)
        {
            if (DiagnosticUtility.ShouldTraceInformation)
            {
                DiagnosticUtility.DiagnosticTrace.TraceEvent(TraceEventType.Information, TraceCode.SecuritySessionServerCloseSent, System.ServiceModel.SR.GetString("TraceCodeSecuritySessionServerCloseSent"), new ServerSessionTraceRecord(sessionToken, null, listenAddress));
            }
        }

        internal static void TraceSessionCloseSendFailure(SecurityContextSecurityToken sessionToken, Uri listenAddress, Exception e)
        {
            if (DiagnosticUtility.ShouldTraceWarning)
            {
                DiagnosticUtility.DiagnosticTrace.TraceEvent(TraceEventType.Warning, TraceCode.SecuritySessionServerCloseSendFailure, System.ServiceModel.SR.GetString("TraceCodeSecuritySessionServerCloseSendFailure"), new ServerSessionTraceRecord(sessionToken, listenAddress), e);
            }
        }

        internal static void TraceSessionKeyRenewalFault(SecurityToken sessionToken, EndpointAddress remoteTarget)
        {
            if (DiagnosticUtility.ShouldTraceWarning)
            {
                DiagnosticUtility.DiagnosticTrace.TraceEvent(TraceEventType.Warning, TraceCode.SecuritySessionKeyRenewalFaultReceived, System.ServiceModel.SR.GetString("TraceCodeSecuritySessionKeyRenewalFaultReceived"), new ClientSessionTraceRecord(sessionToken, null, remoteTarget));
            }
        }

        internal static void TraceSessionKeyRenewed(SecurityToken newSessionToken, SecurityToken currentSessionToken, EndpointAddress remoteAddress)
        {
            if (DiagnosticUtility.ShouldTraceInformation)
            {
                DiagnosticUtility.DiagnosticTrace.TraceEvent(TraceEventType.Information, TraceCode.SecurityClientSessionKeyRenewed, System.ServiceModel.SR.GetString("TraceCodeSecurityClientSessionKeyRenewed"), new ClientSessionTraceRecord(newSessionToken, currentSessionToken, remoteAddress));
            }
        }

        internal static void TraceSessionRedirectApplied(EndpointAddress previousTarget, EndpointAddress newTarget, GenericXmlSecurityToken sessionToken)
        {
            if (DiagnosticUtility.ShouldTraceInformation)
            {
                DiagnosticUtility.DiagnosticTrace.TraceEvent(TraceEventType.Information, TraceCode.SecuritySessionRedirectApplied, System.ServiceModel.SR.GetString("TraceCodeSecuritySessionRedirectApplied"), new SessionRedirectAppliedTraceRecord(previousTarget, newTarget, sessionToken));
            }
        }

        internal static void TraceSessionRenewalFaultSent(SecurityContextSecurityToken sessionToken, Uri listenAddress, Message message)
        {
            if (DiagnosticUtility.ShouldTraceWarning)
            {
                TraceUtility.TraceEvent(TraceEventType.Warning, TraceCode.SecurityServerSessionRenewalFaultSent, new ServerSessionTraceRecord(sessionToken, message, listenAddress), null, null, message);
            }
        }

        internal static void TraceSpnToSidMappingFailure(string spn, Exception e)
        {
            if (DiagnosticUtility.ShouldTraceWarning)
            {
                DiagnosticUtility.DiagnosticTrace.TraceEvent(TraceEventType.Warning, TraceCode.SecuritySpnToSidMappingFailure, System.ServiceModel.SR.GetString("TraceCodeSecuritySpnToSidMappingFailure"), new SpnToSidMappingTraceRecord(spn, e));
            }
        }

        internal static void TraceTokenAuthenticatorClosed(SecurityTokenAuthenticator authenticator)
        {
            if (DiagnosticUtility.ShouldTraceInformation)
            {
                DiagnosticUtility.DiagnosticTrace.TraceEvent(TraceEventType.Information, TraceCode.SecurityTokenAuthenticatorClosed, System.ServiceModel.SR.GetString("TraceCodeSecurityTokenAuthenticatorClosed"), new TokenAuthenticatorTraceRecord(authenticator));
            }
        }

        internal static void TraceTokenAuthenticatorOpened(SecurityTokenAuthenticator authenticator)
        {
            if (DiagnosticUtility.ShouldTraceVerbose)
            {
                DiagnosticUtility.DiagnosticTrace.TraceEvent(TraceEventType.Verbose, TraceCode.SecurityTokenAuthenticatorOpened, System.ServiceModel.SR.GetString("TraceCodeSecurityTokenAuthenticatorOpened"), new TokenAuthenticatorTraceRecord(authenticator));
            }
        }

        internal static void TraceTokenProviderClosed(SecurityTokenProvider provider)
        {
            if (DiagnosticUtility.ShouldTraceInformation)
            {
                DiagnosticUtility.DiagnosticTrace.TraceEvent(TraceEventType.Information, TraceCode.SecurityTokenProviderClosed, System.ServiceModel.SR.GetString("TraceCodeSecurityTokenProviderClosed"), new TokenProviderTraceRecord(provider));
            }
        }

        internal static void TraceTokenProviderOpened(SecurityTokenProvider provider)
        {
            if (DiagnosticUtility.ShouldTraceInformation)
            {
                DiagnosticUtility.DiagnosticTrace.TraceEvent(TraceEventType.Information, TraceCode.SecurityTokenProviderOpened, System.ServiceModel.SR.GetString("TraceCodeSecurityTokenProviderOpened"), new TokenProviderTraceRecord(provider));
            }
        }

        internal static void TraceUsingCachedServiceToken<T>(IssuanceTokenProviderBase<T> provider, SecurityToken serviceToken, EndpointAddress target) where T: IssuanceTokenProviderState
        {
            if (DiagnosticUtility.ShouldTraceInformation)
            {
                DiagnosticUtility.DiagnosticTrace.TraceEvent(TraceEventType.Information, TraceCode.IssuanceTokenProviderUsingCachedToken, System.ServiceModel.SR.GetString("TraceCodeIssuanceTokenProviderUsingCachedToken"), new IssuanceProviderTraceRecord<T>(provider, serviceToken, target));
            }
        }

        internal static void TraceVerifyIncomingMessageFailure(SecurityProtocol binding, Message message)
        {
            if (DiagnosticUtility.ShouldTraceWarning)
            {
                TraceUtility.TraceEvent(TraceEventType.Warning, TraceCode.SecurityBindingVerifyIncomingMessageFailure, new MessageSecurityTraceRecord(binding, message), null, null, message);
            }
        }

        internal static void WriteClaim(XmlWriter xml, Claim claim)
        {
            if (xml == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("xml");
            }
            if (claim != null)
            {
                xml.WriteStartElement("Claim");
                if (((DiagnosticUtility.DiagnosticTrace != null) && (DiagnosticUtility.DiagnosticTrace.TraceSource != null)) && DiagnosticUtility.DiagnosticTrace.TraceSource.ShouldLogPii)
                {
                    xml.WriteElementString("ClaimType", claim.ClaimType);
                    xml.WriteElementString("Right", claim.Right);
                    if (claim.Resource != null)
                    {
                        xml.WriteElementString("ResourceType", claim.Resource.GetType().ToString());
                    }
                    else
                    {
                        xml.WriteElementString("Resource", "null");
                    }
                }
                else
                {
                    xml.WriteString(claim.GetType().AssemblyQualifiedName);
                }
                xml.WriteEndElement();
            }
        }

        private static void WriteGenericXmlToken(XmlWriter xml, SecurityToken sessiontoken)
        {
            if ((xml != null) && (sessiontoken != null))
            {
                xml.WriteElementString("SessionTokenType", sessiontoken.GetType().ToString());
                xml.WriteElementString("ValidFrom", XmlConvert.ToString(sessiontoken.ValidFrom, XmlDateTimeSerializationMode.Utc));
                xml.WriteElementString("ValidTo", XmlConvert.ToString(sessiontoken.ValidTo, XmlDateTimeSerializationMode.Utc));
                GenericXmlSecurityToken token = sessiontoken as GenericXmlSecurityToken;
                if (token != null)
                {
                    if (token.InternalTokenReference != null)
                    {
                        xml.WriteElementString("InternalTokenReference", token.InternalTokenReference.ToString());
                    }
                    if (token.ExternalTokenReference != null)
                    {
                        xml.WriteElementString("ExternalTokenReference", token.ExternalTokenReference.ToString());
                    }
                    xml.WriteElementString("IssuedTokenElementName", token.TokenXml.LocalName);
                    xml.WriteElementString("IssuedTokenElementNamespace", token.TokenXml.NamespaceURI);
                }
            }
        }

        private static void WritePossibleGenericXmlToken(XmlWriter writer, string startElement, SecurityToken token)
        {
            if (writer != null)
            {
                writer.WriteStartElement(startElement);
                GenericXmlSecurityToken sessiontoken = token as GenericXmlSecurityToken;
                if (sessiontoken != null)
                {
                    WriteGenericXmlToken(writer, sessiontoken);
                }
                else if (token != null)
                {
                    writer.WriteElementString("TokenType", token.GetType().ToString());
                }
                writer.WriteEndElement();
            }
        }

        private static void WriteSecurityContextToken(XmlWriter xml, SecurityContextSecurityToken token)
        {
            xml.WriteElementString("ContextId", token.ContextId.ToString());
            if (token.KeyGeneration != null)
            {
                xml.WriteElementString("KeyGeneration", token.KeyGeneration.ToString());
            }
        }

        private class ClientSessionTraceRecord : SecurityTraceRecord
        {
            private SecurityToken currentSessionToken;
            private SecurityToken previousSessionToken;
            private EndpointAddress remoteAddress;

            public ClientSessionTraceRecord(SecurityToken currentSessionToken, SecurityToken previousSessionToken, EndpointAddress remoteAddress) : base("SecuritySession")
            {
                this.currentSessionToken = currentSessionToken;
                this.previousSessionToken = previousSessionToken;
                this.remoteAddress = remoteAddress;
            }

            internal override void WriteTo(XmlWriter xml)
            {
                if (xml != null)
                {
                    if (this.remoteAddress != null)
                    {
                        xml.WriteElementString("RemoteAddress", this.remoteAddress.ToString());
                    }
                    if (this.currentSessionToken != null)
                    {
                        xml.WriteStartElement("CurrentSessionToken");
                        SecurityTraceRecordHelper.WriteGenericXmlToken(xml, this.currentSessionToken);
                        xml.WriteEndElement();
                    }
                    if (this.previousSessionToken != null)
                    {
                        xml.WriteStartElement("PreviousSessionToken");
                        SecurityTraceRecordHelper.WriteGenericXmlToken(xml, this.previousSessionToken);
                        xml.WriteEndElement();
                    }
                }
            }
        }

        private class IdentityDeterminationFailureTraceRecord : SecurityTraceRecord
        {
            private EndpointAddress epr;
            private Type identityVerifier;

            public IdentityDeterminationFailureTraceRecord(EndpointAddress epr, Type identityVerifier) : base("ServiceIdentityDetermination")
            {
                this.epr = epr;
                this.identityVerifier = identityVerifier;
            }

            internal override void WriteTo(XmlWriter xml)
            {
                if (xml != null)
                {
                    if (this.identityVerifier != null)
                    {
                        xml.WriteElementString("IdentityVerifierType", this.identityVerifier.ToString());
                    }
                    if (this.epr != null)
                    {
                        this.epr.WriteTo(AddressingVersion.WSAddressing10, xml);
                    }
                }
            }
        }

        private class IdentityDeterminationSuccessTraceRecord : SecurityTraceRecord
        {
            private EndpointAddress epr;
            private EndpointIdentity identity;
            private Type identityVerifier;

            public IdentityDeterminationSuccessTraceRecord(EndpointAddress epr, EndpointIdentity identity, Type identityVerifier) : base("ServiceIdentityDetermination")
            {
                this.identity = identity;
                this.epr = epr;
                this.identityVerifier = identityVerifier;
            }

            internal override void WriteTo(XmlWriter xml)
            {
                if (xml != null)
                {
                    if (this.identityVerifier != null)
                    {
                        xml.WriteElementString("IdentityVerifierType", this.identityVerifier.ToString());
                    }
                    if (this.identity != null)
                    {
                        this.identity.WriteTo(XmlDictionaryWriter.CreateDictionaryWriter(xml));
                    }
                    if (this.epr != null)
                    {
                        this.epr.WriteTo(AddressingVersion.WSAddressing10, xml);
                    }
                }
            }
        }

        private class IdentityHostNameNormalizationFailureTraceRecord : SecurityTraceRecord
        {
            private Exception e;
            private EndpointAddress epr;
            private Type identityVerifier;

            public IdentityHostNameNormalizationFailureTraceRecord(EndpointAddress epr, Type identityVerifier, Exception e) : base("ServiceIdentityDetermination")
            {
                this.epr = epr;
                this.identityVerifier = identityVerifier;
                this.e = e;
            }

            internal override void WriteTo(XmlWriter xml)
            {
                if (xml != null)
                {
                    if (this.identityVerifier != null)
                    {
                        xml.WriteElementString("IdentityVerifierType", this.identityVerifier.ToString());
                    }
                    if (this.epr != null)
                    {
                        this.epr.WriteTo(AddressingVersion.WSAddressing10, xml);
                    }
                    if (this.e != null)
                    {
                        xml.WriteElementString("Exception", this.e.ToString());
                    }
                }
            }
        }

        private class IdentityVerificationFailureTraceRecord : SecurityTraceRecord
        {
            private AuthorizationContext authContext;
            private EndpointIdentity identity;
            private Type identityVerifier;

            public IdentityVerificationFailureTraceRecord(EndpointIdentity identity, AuthorizationContext authContext, Type identityVerifier) : base("ServiceIdentityVerification")
            {
                this.identity = identity;
                this.authContext = authContext;
                this.identityVerifier = identityVerifier;
            }

            internal override void WriteTo(XmlWriter xml)
            {
                if (xml != null)
                {
                    XmlDictionaryWriter writer = XmlDictionaryWriter.CreateDictionaryWriter(xml);
                    if (this.identityVerifier != null)
                    {
                        xml.WriteElementString("IdentityVerifierType", this.identityVerifier.ToString());
                    }
                    if (this.identity != null)
                    {
                        this.identity.WriteTo(writer);
                    }
                    if (this.authContext != null)
                    {
                        for (int i = 0; i < this.authContext.ClaimSets.Count; i++)
                        {
                            ClaimSet set = this.authContext.ClaimSets[i];
                            if (this.authContext.ClaimSets[i] != null)
                            {
                                for (int j = 0; j < set.Count; j++)
                                {
                                    Claim claim = set[j];
                                    if (set[j] != null)
                                    {
                                        xml.WriteStartElement("Claim");
                                        if (claim.ClaimType != null)
                                        {
                                            xml.WriteElementString("ClaimType", claim.ClaimType);
                                        }
                                        else
                                        {
                                            xml.WriteElementString("ClaimType", "null");
                                        }
                                        if (claim.Right != null)
                                        {
                                            xml.WriteElementString("Right", claim.Right);
                                        }
                                        else
                                        {
                                            xml.WriteElementString("Right", "null");
                                        }
                                        if (claim.Resource != null)
                                        {
                                            xml.WriteElementString("ResourceType", claim.Resource.GetType().ToString());
                                        }
                                        else
                                        {
                                            xml.WriteElementString("Resource", "null");
                                        }
                                        xml.WriteEndElement();
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private class IdentityVerificationSuccessTraceRecord : SecurityTraceRecord
        {
            private Claim claim;
            private EndpointIdentity identity;
            private Type identityVerifier;

            public IdentityVerificationSuccessTraceRecord(EndpointIdentity identity, Claim claim, Type identityVerifier) : base("ServiceIdentityVerification")
            {
                this.identity = identity;
                this.claim = claim;
                this.identityVerifier = identityVerifier;
            }

            internal override void WriteTo(XmlWriter xml)
            {
                if (xml != null)
                {
                    XmlDictionaryWriter writer = XmlDictionaryWriter.CreateDictionaryWriter(xml);
                    if (this.identityVerifier != null)
                    {
                        xml.WriteElementString("IdentityVerifierType", this.identityVerifier.ToString());
                    }
                    if (this.identity != null)
                    {
                        this.identity.WriteTo(writer);
                    }
                    if (this.claim != null)
                    {
                        SecurityTraceRecordHelper.WriteClaim(writer, this.claim);
                    }
                }
            }
        }

        private class ImpersonationTraceRecord : SecurityTraceRecord
        {
            private DispatchOperationRuntime operation;

            internal ImpersonationTraceRecord(DispatchOperationRuntime operation) : base("SecurityImpersonation")
            {
                this.operation = operation;
            }

            internal override void WriteTo(XmlWriter xml)
            {
                if ((xml != null) && (this.operation != null))
                {
                    xml.WriteElementString("OperationAction", this.operation.Action);
                    xml.WriteElementString("OperationName", this.operation.Name);
                }
            }
        }

        private class IssuanceProviderTraceRecord<T> : SecurityTraceRecord where T: IssuanceTokenProviderState
        {
            private int cacheSize;
            private EndpointAddress newTarget;
            private IssuanceTokenProviderBase<T> provider;
            private SecurityToken serviceToken;
            private EndpointAddress target;

            public IssuanceProviderTraceRecord(IssuanceTokenProviderBase<T> provider, SecurityToken serviceToken) : this(provider, serviceToken, null)
            {
            }

            public IssuanceProviderTraceRecord(IssuanceTokenProviderBase<T> provider, int cacheSize) : base("ClientSecurityNegotiation")
            {
                this.provider = provider;
                this.cacheSize = cacheSize;
            }

            public IssuanceProviderTraceRecord(IssuanceTokenProviderBase<T> provider, EndpointAddress target) : this(provider, (SecurityToken) null, target)
            {
            }

            public IssuanceProviderTraceRecord(IssuanceTokenProviderBase<T> provider, SecurityToken serviceToken, EndpointAddress target) : base("ClientSecurityNegotiation")
            {
                this.provider = provider;
                this.serviceToken = serviceToken;
                this.target = target;
            }

            public IssuanceProviderTraceRecord(IssuanceTokenProviderBase<T> provider, EndpointAddress newTarget, EndpointAddress oldTarget) : base("ClientSecurityNegotiation")
            {
                this.provider = provider;
                this.newTarget = newTarget;
                this.target = oldTarget;
            }

            internal override void WriteTo(XmlWriter xml)
            {
                if (xml != null)
                {
                    if (this.provider != null)
                    {
                        xml.WriteElementString("IssuanceTokenProvider", this.provider.ToString());
                    }
                    if (this.serviceToken != null)
                    {
                        SecurityTraceRecordHelper.WritePossibleGenericXmlToken(xml, "ServiceToken", this.serviceToken);
                    }
                    if (this.target != null)
                    {
                        xml.WriteStartElement("Target");
                        this.target.WriteTo(AddressingVersion.WSAddressing10, xml);
                        xml.WriteEndElement();
                    }
                    if (this.newTarget != null)
                    {
                        xml.WriteStartElement("PinnedTarget");
                        this.newTarget.WriteTo(AddressingVersion.WSAddressing10, xml);
                        xml.WriteEndElement();
                    }
                    if (this.cacheSize != 0)
                    {
                        xml.WriteElementString("CacheSize", this.cacheSize.ToString(NumberFormatInfo.InvariantInfo));
                    }
                }
            }
        }

        private class MessageSecurityTraceRecord : SecurityTraceRecord
        {
            private SecurityProtocol binding;
            private Message message;

            public MessageSecurityTraceRecord(SecurityProtocol binding, Message message) : base("SecurityProtocol")
            {
                this.binding = binding;
                this.message = message;
            }

            internal override void WriteTo(XmlWriter xml)
            {
                if (xml != null)
                {
                    if (this.binding != null)
                    {
                        xml.WriteElementString("SecurityProtocol", this.binding.ToString());
                    }
                    if (this.message != null)
                    {
                        string action = this.message.Headers.Action;
                        Uri to = this.message.Headers.To;
                        EndpointAddress replyTo = this.message.Headers.ReplyTo;
                        UniqueId messageId = this.message.Headers.MessageId;
                        if (!string.IsNullOrEmpty(action))
                        {
                            xml.WriteElementString("Action", action);
                        }
                        if (to != null)
                        {
                            xml.WriteElementString("To", to.AbsoluteUri);
                        }
                        if (replyTo != null)
                        {
                            replyTo.WriteTo(this.message.Version.Addressing, xml);
                        }
                        if (messageId != null)
                        {
                            xml.WriteElementString("MessageId", messageId.ToString());
                        }
                    }
                    else
                    {
                        xml.WriteElementString("Message", "null");
                    }
                }
            }
        }

        private class NegotiationAuthenticatorTraceRecord<T> : SecurityTraceRecord where T: NegotiationTokenAuthenticatorState
        {
            private NegotiationTokenAuthenticator<T> authenticator;
            private Exception e;
            private SecurityContextSecurityToken serviceToken;
            private IChannelListener transportChannelListener;

            public NegotiationAuthenticatorTraceRecord(NegotiationTokenAuthenticator<T> authenticator, Exception e) : base("NegotiationTokenAuthenticator")
            {
                this.authenticator = authenticator;
                this.e = e;
            }

            public NegotiationAuthenticatorTraceRecord(NegotiationTokenAuthenticator<T> authenticator, IChannelListener transportChannelListener) : base("NegotiationTokenAuthenticator")
            {
                this.authenticator = authenticator;
                this.transportChannelListener = transportChannelListener;
            }

            public NegotiationAuthenticatorTraceRecord(NegotiationTokenAuthenticator<T> authenticator, SecurityContextSecurityToken serviceToken) : base("NegotiationTokenAuthenticator")
            {
                this.authenticator = authenticator;
                this.serviceToken = serviceToken;
            }

            internal override void WriteTo(XmlWriter xml)
            {
                if (xml != null)
                {
                    if (this.authenticator != null)
                    {
                        xml.WriteElementString("NegotiationTokenAuthenticator", DiagnosticTrace.XmlEncode(this.authenticator.ToString()));
                    }
                    if ((this.authenticator != null) && (this.authenticator.ListenUri != null))
                    {
                        xml.WriteElementString("AuthenticatorListenUri", this.authenticator.ListenUri.AbsoluteUri);
                    }
                    if (this.serviceToken != null)
                    {
                        xml.WriteStartElement("SecurityContextSecurityToken");
                        SecurityTraceRecordHelper.WriteSecurityContextToken(xml, this.serviceToken);
                        xml.WriteEndElement();
                    }
                    if (this.transportChannelListener != null)
                    {
                        xml.WriteElementString("TransportChannelListener", DiagnosticTrace.XmlEncode(this.transportChannelListener.ToString()));
                        if (this.transportChannelListener.Uri != null)
                        {
                            xml.WriteElementString("ListenUri", this.transportChannelListener.Uri.AbsoluteUri);
                        }
                    }
                    if (this.e != null)
                    {
                        xml.WriteElementString("Exception", DiagnosticTrace.XmlEncode(this.e.ToString()));
                    }
                }
            }
        }

        private class SecurityContextTokenCacheTraceRecord : SecurityTraceRecord
        {
            private int capacity;
            private int pruningAmount;

            public SecurityContextTokenCacheTraceRecord(int capacity, int pruningAmount) : base("ServiceSecurityNegotiation")
            {
                this.capacity = capacity;
                this.pruningAmount = pruningAmount;
            }

            internal override void WriteTo(XmlWriter xml)
            {
                if (xml != null)
                {
                    xml.WriteElementString("Capacity", this.capacity.ToString(NumberFormatInfo.InvariantInfo));
                    xml.WriteElementString("PruningAmount", this.pruningAmount.ToString(NumberFormatInfo.InvariantInfo));
                }
            }
        }

        private class ServerSessionTraceRecord : SecurityTraceRecord
        {
            private SecurityContextSecurityToken currentSessionToken;
            private Uri listenAddress;
            private Message message;
            private SecurityContextSecurityToken newSessionToken;
            private UniqueId sessionId;

            public ServerSessionTraceRecord(SecurityContextSecurityToken currentSessionToken, Uri listenAddress) : base("SecuritySession")
            {
                this.currentSessionToken = currentSessionToken;
                this.listenAddress = listenAddress;
            }

            public ServerSessionTraceRecord(UniqueId sessionId, Uri listenAddress) : base("SecuritySession")
            {
                this.sessionId = sessionId;
                this.listenAddress = listenAddress;
            }

            public ServerSessionTraceRecord(SecurityContextSecurityToken currentSessionToken, Message message, Uri listenAddress) : base("SecuritySession")
            {
                this.currentSessionToken = currentSessionToken;
                this.message = message;
                this.listenAddress = listenAddress;
            }

            public ServerSessionTraceRecord(SecurityContextSecurityToken currentSessionToken, SecurityContextSecurityToken newSessionToken, Uri listenAddress) : base("SecuritySession")
            {
                this.currentSessionToken = currentSessionToken;
                this.newSessionToken = newSessionToken;
                this.listenAddress = listenAddress;
            }

            internal override void WriteTo(XmlWriter xml)
            {
                if (xml != null)
                {
                    if (this.currentSessionToken != null)
                    {
                        xml.WriteStartElement("CurrentSessionToken");
                        SecurityTraceRecordHelper.WriteSecurityContextToken(xml, this.currentSessionToken);
                        xml.WriteEndElement();
                    }
                    if (this.newSessionToken != null)
                    {
                        xml.WriteStartElement("NewSessionToken");
                        SecurityTraceRecordHelper.WriteSecurityContextToken(xml, this.newSessionToken);
                        xml.WriteEndElement();
                    }
                    if (this.sessionId != null)
                    {
                        XmlHelper.WriteElementStringAsUniqueId(xml, "SessionId", this.sessionId);
                    }
                    if (this.message != null)
                    {
                        xml.WriteElementString("MessageAction", this.message.Headers.Action);
                    }
                    if (this.listenAddress != null)
                    {
                        xml.WriteElementString("ListenAddress", this.listenAddress.ToString());
                    }
                }
            }
        }

        private class SessionRedirectAppliedTraceRecord : SecurityTraceRecord
        {
            private EndpointAddress newTarget;
            private EndpointAddress previousTarget;
            private GenericXmlSecurityToken sessionToken;

            public SessionRedirectAppliedTraceRecord(EndpointAddress previousTarget, EndpointAddress newTarget, GenericXmlSecurityToken sessionToken) : base("SecuritySession")
            {
                this.previousTarget = previousTarget;
                this.newTarget = newTarget;
                this.sessionToken = sessionToken;
            }

            internal override void WriteTo(XmlWriter xml)
            {
                if (xml != null)
                {
                    if (this.previousTarget != null)
                    {
                        xml.WriteElementString("OriginalRemoteAddress", this.previousTarget.ToString());
                    }
                    if (this.newTarget != null)
                    {
                        xml.WriteElementString("NewRemoteAddress", this.newTarget.ToString());
                    }
                    if (this.sessionToken != null)
                    {
                        xml.WriteStartElement("SessionToken");
                        SecurityTraceRecordHelper.WriteGenericXmlToken(xml, this.sessionToken);
                        xml.WriteEndElement();
                    }
                }
            }
        }

        private class SessionRequestorTraceRecord : SecurityTraceRecord
        {
            private SecurityToken currentToken;
            private Exception e;
            private SecurityToken issuedToken;
            private SecuritySessionOperation operation;
            private EndpointAddress target;

            public SessionRequestorTraceRecord(SecuritySessionOperation operation, SecurityToken currentToken, Exception e, EndpointAddress target) : base("SecuritySession")
            {
                this.operation = operation;
                this.currentToken = currentToken;
                this.e = e;
                this.target = target;
            }

            public SessionRequestorTraceRecord(SecuritySessionOperation operation, SecurityToken currentToken, SecurityToken issuedToken, EndpointAddress target) : base("SecuritySession")
            {
                this.operation = operation;
                this.currentToken = currentToken;
                this.issuedToken = issuedToken;
                this.target = target;
            }

            internal override void WriteTo(XmlWriter xml)
            {
                if (xml != null)
                {
                    xml.WriteElementString("Operation", this.operation.ToString());
                    if (this.currentToken != null)
                    {
                        SecurityTraceRecordHelper.WritePossibleGenericXmlToken(xml, "SupportingToken", this.currentToken);
                    }
                    if (this.issuedToken != null)
                    {
                        SecurityTraceRecordHelper.WritePossibleGenericXmlToken(xml, "IssuedToken", this.issuedToken);
                    }
                    if (this.e != null)
                    {
                        xml.WriteElementString("Exception", this.e.ToString());
                    }
                    if (this.target != null)
                    {
                        xml.WriteElementString("RemoteAddress", this.target.ToString());
                    }
                }
            }
        }

        private class SessionResponderTraceRecord : SecurityTraceRecord
        {
            private Exception e;
            private Uri listenAddress;
            private SecuritySessionOperation operation;

            public SessionResponderTraceRecord(SecuritySessionOperation operation, Exception e, Uri listenAddress) : base("SecuritySession")
            {
                this.operation = operation;
                this.e = e;
                this.listenAddress = listenAddress;
            }

            internal override void WriteTo(XmlWriter xml)
            {
                if (xml != null)
                {
                    xml.WriteElementString("Operation", this.operation.ToString());
                    if (this.e != null)
                    {
                        xml.WriteElementString("Exception", this.e.ToString());
                    }
                    if (this.listenAddress != null)
                    {
                        xml.WriteElementString("ListenAddress", this.listenAddress.ToString());
                    }
                }
            }
        }

        private class SpnToSidMappingTraceRecord : SecurityTraceRecord
        {
            private Exception e;
            private string spn;

            public SpnToSidMappingTraceRecord(string spn, Exception e) : base("SecurityIdentity")
            {
                this.spn = spn;
                this.e = e;
            }

            internal override void WriteTo(XmlWriter xml)
            {
                if (xml != null)
                {
                    if (this.spn != null)
                    {
                        xml.WriteElementString("ServicePrincipalName", this.spn);
                    }
                    if (this.e != null)
                    {
                        xml.WriteElementString("Exception", this.e.ToString());
                    }
                }
            }
        }

        private class TokenAuthenticatorTraceRecord : SecurityTraceRecord
        {
            private SecurityTokenAuthenticator authenticator;

            public TokenAuthenticatorTraceRecord(SecurityTokenAuthenticator authenticator) : base("SecurityTokenAuthenticator")
            {
                this.authenticator = authenticator;
            }

            internal override void WriteTo(XmlWriter xml)
            {
                if ((xml != null) && (this.authenticator != null))
                {
                    xml.WriteElementString("SecurityTokenAuthenticator", this.authenticator.ToString());
                }
            }
        }

        private class TokenProviderTraceRecord : SecurityTraceRecord
        {
            private SecurityTokenProvider provider;

            public TokenProviderTraceRecord(SecurityTokenProvider provider) : base("SecurityTokenProvider")
            {
                this.provider = provider;
            }

            internal override void WriteTo(XmlWriter xml)
            {
                if ((xml != null) && (this.provider != null))
                {
                    xml.WriteElementString("SecurityTokenProvider", this.provider.ToString());
                }
            }
        }

        private class WindowsSspiNegotiationTraceRecord : SecurityTraceRecord
        {
            private bool isClient;
            private WindowsSspiNegotiation windowsNegotiation;

            public WindowsSspiNegotiationTraceRecord(WindowsSspiNegotiation windowsNegotiation, bool isClient) : base("SpnegoSecurityNegotiation")
            {
                this.windowsNegotiation = windowsNegotiation;
                this.isClient = isClient;
            }

            internal override void WriteTo(XmlWriter xml)
            {
                if ((xml != null) && (this.windowsNegotiation != null))
                {
                    xml.WriteElementString("Protocol", this.windowsNegotiation.ProtocolName);
                    if (this.isClient)
                    {
                        xml.WriteElementString("ServicePrincipalName", this.windowsNegotiation.ServicePrincipalName);
                        xml.WriteElementString("MutualAuthentication", this.windowsNegotiation.IsMutualAuthFlag.ToString());
                    }
                    else if (this.windowsNegotiation.IsIdentifyFlag)
                    {
                        xml.WriteElementString("ImpersonationLevel", "Identify");
                    }
                    else if (this.windowsNegotiation.IsDelegationFlag)
                    {
                        xml.WriteElementString("ImpersonationLevel", "Delegate");
                    }
                    else
                    {
                        xml.WriteElementString("ImpersonationLevel", "Impersonate");
                    }
                }
            }
        }
    }
}

