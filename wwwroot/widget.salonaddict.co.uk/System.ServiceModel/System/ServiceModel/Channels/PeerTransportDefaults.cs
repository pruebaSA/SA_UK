namespace System.ServiceModel.Channels
{
    using System;
    using System.Net;
    using System.ServiceModel;

    internal static class PeerTransportDefaults
    {
        internal const IPAddress ListenIPAddress = null;
        internal const bool MessageAuthentication = false;
        internal const PeerAuthenticationMode PeerNodeAuthenticationMode = PeerAuthenticationMode.Password;
        internal const int Port = 0;
        internal const string ResolverTypeString = null;

        internal static PeerResolver CreateResolver() => 
            new PnrpPeerResolver();

        internal static bool ResolverAvailable =>
            PnrpPeerResolver.IsPnrpAvailable;

        internal static Type ResolverBindingElementType =>
            typeof(PnrpPeerResolverBindingElement);

        internal static bool ResolverInstalled =>
            PnrpPeerResolver.IsPnrpInstalled;

        internal static Type ResolverType =>
            typeof(PnrpPeerResolver);
    }
}

