﻿namespace System.ServiceModel.Channels
{
    using System;
    using System.Collections.Generic;
    using System.ServiceModel.Diagnostics;

    internal abstract class NamedPipeChannelListener : ConnectionOrientedTransportChannelListener
    {
        private List<SecurityIdentifier> allowedUsers;
        private static UriPrefixTable<ITransportManagerRegistration> transportManagerTable = new UriPrefixTable<ITransportManagerRegistration>();

        protected NamedPipeChannelListener(NamedPipeTransportBindingElement bindingElement, BindingContext context) : base(bindingElement, context)
        {
            base.SetIdleTimeout(bindingElement.ConnectionPoolSettings.IdleTimeout);
            base.SetMaxPooledConnections(bindingElement.ConnectionPoolSettings.MaxOutboundConnectionsPerEndpoint);
        }

        internal override ITransportManagerRegistration CreateTransportManagerRegistration(Uri listenUri) => 
            new ExclusiveNamedPipeTransportManager(listenUri, this);

        protected override bool SupportsUpgrade(StreamUpgradeBindingElement upgradeBindingElement) => 
            !(upgradeBindingElement is SslStreamSecurityBindingElement);

        internal List<SecurityIdentifier> AllowedUsers
        {
            get => 
                this.allowedUsers;
            set
            {
                lock (base.ThisLock)
                {
                    base.ThrowIfDisposedOrImmutable();
                    this.allowedUsers = value;
                }
            }
        }

        internal override TraceCode MessageReceivedTraceCode =>
            TraceCode.NamedPipeChannelMessageReceived;

        internal override TraceCode MessageReceiveFailedTraceCode =>
            TraceCode.NamedPipeChannelMessageReceiveFailed;

        public override string Scheme =>
            Uri.UriSchemeNetPipe;

        internal static UriPrefixTable<ITransportManagerRegistration> StaticTransportManagerTable =>
            transportManagerTable;

        internal override UriPrefixTable<ITransportManagerRegistration> TransportManagerTable =>
            transportManagerTable;
    }
}

