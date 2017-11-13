namespace System.ServiceModel.Channels
{
    using System;
    using System.Collections.ObjectModel;
    using System.ServiceModel;
    using System.ServiceModel.Description;
    using System.ServiceModel.Diagnostics;
    using System.Text;

    internal abstract class ConnectionOrientedTransportChannelListener : TransportChannelListener, IConnectionOrientedTransportFactorySettings, ITransportFactorySettings, IDefaultCommunicationTimeouts, IConnectionOrientedListenerSettings, IConnectionOrientedConnectionSettings
    {
        private TimeSpan channelInitializationTimeout;
        private int connectionBufferSize;
        private bool exposeConnectionProperty;
        private EndpointIdentity identity;
        private TimeSpan idleTimeout;
        private int maxBufferSize;
        private TimeSpan maxOutputDelay;
        private int maxPendingAccepts;
        private int maxPendingConnections;
        private int maxPooledConnections;
        private bool ownUpgrade;
        private ISecurityCapabilities securityCapabilities;
        private System.ServiceModel.TransferMode transferMode;
        private StreamUpgradeProvider upgrade;

        protected ConnectionOrientedTransportChannelListener(ConnectionOrientedTransportBindingElement bindingElement, BindingContext context) : base(bindingElement, context, bindingElement.HostNameComparisonMode)
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
            this.connectionBufferSize = bindingElement.ConnectionBufferSize;
            this.exposeConnectionProperty = bindingElement.ExposeConnectionProperty;
            base.InheritBaseAddressSettings = bindingElement.InheritBaseAddressSettings;
            this.channelInitializationTimeout = bindingElement.ChannelInitializationTimeout;
            this.maxBufferSize = bindingElement.MaxBufferSize;
            this.maxPendingConnections = bindingElement.MaxPendingConnections;
            this.maxOutputDelay = bindingElement.MaxOutputDelay;
            this.maxPendingAccepts = bindingElement.MaxPendingAccepts;
            this.transferMode = bindingElement.TransferMode;
            Collection<StreamUpgradeBindingElement> collection = context.BindingParameters.FindAll<StreamUpgradeBindingElement>();
            if (collection.Count > 1)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new InvalidOperationException(System.ServiceModel.SR.GetString("MultipleStreamUpgradeProvidersInParameters")));
            }
            if ((collection.Count == 1) && this.SupportsUpgrade(collection[0]))
            {
                this.upgrade = collection[0].BuildServerStreamUpgradeProvider(context);
                this.ownUpgrade = true;
                context.BindingParameters.Remove<StreamUpgradeBindingElement>();
                this.securityCapabilities = collection[0].GetProperty<ISecurityCapabilities>(context);
            }
        }

        internal override int GetMaxBufferSize() => 
            this.MaxBufferSize;

        public override T GetProperty<T>() where T: class
        {
            if (typeof(T) == typeof(EndpointIdentity))
            {
                return (T) this.identity;
            }
            if (typeof(T) == typeof(ISecurityCapabilities))
            {
                return (T) this.securityCapabilities;
            }
            T property = base.GetProperty<T>();
            if ((property == null) && (this.upgrade != null))
            {
                property = this.upgrade.GetProperty<T>();
            }
            return property;
        }

        private StreamUpgradeProvider GetUpgrade()
        {
            StreamUpgradeProvider upgrade = null;
            lock (base.ThisLock)
            {
                if (this.ownUpgrade)
                {
                    upgrade = this.upgrade;
                    this.ownUpgrade = false;
                }
            }
            return upgrade;
        }

        protected override void OnAbort()
        {
            StreamUpgradeProvider upgrade = this.GetUpgrade();
            if (upgrade != null)
            {
                upgrade.Abort();
            }
            base.OnAbort();
        }

        protected override IAsyncResult OnBeginClose(TimeSpan timeout, AsyncCallback callback, object state)
        {
            StreamUpgradeProvider upgrade = this.GetUpgrade();
            if (upgrade != null)
            {
                return new ChainedCloseAsyncResult(timeout, callback, state, new ChainedBeginHandler(this.OnBeginClose), new ChainedEndHandler(this.OnEndClose), new ICommunicationObject[] { upgrade });
            }
            return new ChainedCloseAsyncResult(timeout, callback, state, new ChainedBeginHandler(this.OnBeginClose), new ChainedEndHandler(this.OnEndClose), new ICommunicationObject[0]);
        }

        protected override IAsyncResult OnBeginOpen(TimeSpan timeout, AsyncCallback callback, object state)
        {
            StreamUpgradeProvider upgrade = this.Upgrade;
            if (upgrade != null)
            {
                return new ChainedOpenAsyncResult(timeout, callback, state, new ChainedBeginHandler(this.OnBeginOpen), new ChainedEndHandler(this.OnEndOpen), new ICommunicationObject[] { upgrade });
            }
            return base.OnBeginOpen(timeout, callback, state);
        }

        protected override void OnClose(TimeSpan timeout)
        {
            StreamUpgradeProvider upgrade = this.GetUpgrade();
            if (upgrade != null)
            {
                TimeoutHelper helper = new TimeoutHelper(timeout);
                upgrade.Close(helper.RemainingTime());
                base.OnClose(helper.RemainingTime());
            }
            else
            {
                base.OnClose(timeout);
            }
        }

        protected override void OnEndClose(IAsyncResult result)
        {
            ChainedAsyncResult.End(result);
        }

        protected override void OnEndOpen(IAsyncResult result)
        {
            if (result is ChainedOpenAsyncResult)
            {
                ChainedAsyncResult.End(result);
            }
            else
            {
                base.OnEndOpen(result);
            }
        }

        protected override void OnOpen(TimeSpan timeout)
        {
            TimeoutHelper helper = new TimeoutHelper(timeout);
            base.OnOpen(timeout);
            StreamUpgradeProvider upgrade = this.Upgrade;
            if (upgrade != null)
            {
                upgrade.Open(helper.RemainingTime());
            }
        }

        protected override void OnOpened()
        {
            base.OnOpened();
            StreamSecurityUpgradeProvider upgrade = this.Upgrade as StreamSecurityUpgradeProvider;
            if (upgrade != null)
            {
                this.identity = upgrade.Identity;
            }
        }

        internal void SetIdleTimeout(TimeSpan idleTimeout)
        {
            this.idleTimeout = idleTimeout;
        }

        internal void SetMaxPooledConnections(int maxPooledConnections)
        {
            this.maxPooledConnections = maxPooledConnections;
        }

        protected virtual bool SupportsUpgrade(StreamUpgradeBindingElement upgradeBindingElement) => 
            true;

        protected override void ValidateUri(Uri uri)
        {
            base.ValidateUri(uri);
            int num = 0x800;
            int byteCount = Encoding.UTF8.GetByteCount(uri.AbsoluteUri);
            if (byteCount > num)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new QuotaExceededException(System.ServiceModel.SR.GetString("UriLengthExceedsMaxSupportedSize", new object[] { uri, byteCount, num })));
            }
        }

        public TimeSpan ChannelInitializationTimeout =>
            this.channelInitializationTimeout;

        public int ConnectionBufferSize =>
            this.connectionBufferSize;

        internal bool ExposeConnectionProperty =>
            this.exposeConnectionProperty;

        public System.ServiceModel.HostNameComparisonMode HostNameComparisonMode =>
            base.HostNameComparisonModeInternal;

        public TimeSpan IdleTimeout =>
            this.idleTimeout;

        public int MaxBufferSize =>
            this.maxBufferSize;

        public TimeSpan MaxOutputDelay =>
            this.maxOutputDelay;

        public int MaxPendingAccepts =>
            this.maxPendingAccepts;

        public int MaxPendingConnections =>
            this.maxPendingConnections;

        public int MaxPooledConnections =>
            this.maxPooledConnections;

        internal abstract TraceCode MessageReceivedTraceCode { get; }

        internal abstract TraceCode MessageReceiveFailedTraceCode { get; }

        ServiceSecurityAuditBehavior IConnectionOrientedTransportFactorySettings.AuditBehavior =>
            base.AuditBehavior;

        int IConnectionOrientedTransportFactorySettings.MaxBufferSize =>
            this.MaxBufferSize;

        System.ServiceModel.TransferMode IConnectionOrientedTransportFactorySettings.TransferMode =>
            this.TransferMode;

        StreamUpgradeProvider IConnectionOrientedTransportFactorySettings.Upgrade =>
            this.Upgrade;

        public System.ServiceModel.TransferMode TransferMode =>
            this.transferMode;

        public StreamUpgradeProvider Upgrade =>
            this.upgrade;

        protected class ConnectionOrientedTransportReplyChannelAcceptor : TransportReplyChannelAcceptor
        {
            private StreamUpgradeProvider upgrade;

            public ConnectionOrientedTransportReplyChannelAcceptor(ConnectionOrientedTransportChannelListener listener) : base(listener)
            {
                this.upgrade = listener.GetUpgrade();
            }

            private IAsyncResult DummyBeginClose(TimeSpan timeout, AsyncCallback callback, object state) => 
                new CompletedAsyncResult(callback, state);

            private void DummyEndClose(IAsyncResult result)
            {
                CompletedAsyncResult.End(result);
            }

            protected override void OnAbort()
            {
                base.OnAbort();
                if ((this.upgrade != null) && !this.TransferUpgrade())
                {
                    this.upgrade.Abort();
                }
            }

            protected override IAsyncResult OnBeginClose(TimeSpan timeout, AsyncCallback callback, object state)
            {
                ChainedBeginHandler handler = new ChainedBeginHandler(this.DummyBeginClose);
                ChainedEndHandler handler2 = new ChainedEndHandler(this.DummyEndClose);
                if ((this.upgrade != null) && !this.TransferUpgrade())
                {
                    handler = new ChainedBeginHandler(this.upgrade.BeginClose);
                    handler2 = new ChainedEndHandler(this.upgrade.EndClose);
                }
                return new ChainedAsyncResult(timeout, callback, state, new ChainedBeginHandler(this.OnBeginClose), new ChainedEndHandler(this.OnEndClose), handler, handler2);
            }

            protected override void OnClose(TimeSpan timeout)
            {
                TimeoutHelper helper = new TimeoutHelper(timeout);
                base.OnClose(helper.RemainingTime());
                if ((this.upgrade != null) && !this.TransferUpgrade())
                {
                    this.upgrade.Close(helper.RemainingTime());
                }
            }

            protected override ReplyChannel OnCreateChannel() => 
                new ConnectionOrientedTransportReplyChannel(base.ChannelManager, null);

            protected override void OnEndClose(IAsyncResult result)
            {
                ChainedAsyncResult.End(result);
            }

            private bool TransferUpgrade()
            {
                ConnectionOrientedTransportReplyChannel currentChannel = (ConnectionOrientedTransportReplyChannel) base.GetCurrentChannel();
                return currentChannel?.TransferUpgrade(this.upgrade);
            }

            private class ConnectionOrientedTransportReplyChannel : TransportReplyChannelAcceptor.TransportReplyChannel
            {
                private StreamUpgradeProvider upgrade;

                public ConnectionOrientedTransportReplyChannel(ChannelManagerBase channelManager, EndpointAddress localAddress) : base(channelManager, localAddress)
                {
                }

                private IAsyncResult DummyBeginClose(TimeSpan timeout, AsyncCallback callback, object state) => 
                    new CompletedAsyncResult(callback, state);

                private void DummyEndClose(IAsyncResult result)
                {
                    CompletedAsyncResult.End(result);
                }

                protected override void OnAbort()
                {
                    if (this.upgrade != null)
                    {
                        this.upgrade.Abort();
                    }
                    base.OnAbort();
                }

                protected override IAsyncResult OnBeginClose(TimeSpan timeout, AsyncCallback callback, object state)
                {
                    ChainedBeginHandler handler = new ChainedBeginHandler(this.DummyBeginClose);
                    ChainedEndHandler handler2 = new ChainedEndHandler(this.DummyEndClose);
                    if (this.upgrade != null)
                    {
                        handler = new ChainedBeginHandler(this.upgrade.BeginClose);
                        handler2 = new ChainedEndHandler(this.upgrade.EndClose);
                    }
                    return new ChainedAsyncResult(timeout, callback, state, handler, handler2, new ChainedBeginHandler(this.OnBeginClose), new ChainedEndHandler(this.OnEndClose));
                }

                protected override void OnClose(TimeSpan timeout)
                {
                    TimeoutHelper helper = new TimeoutHelper(timeout);
                    if (this.upgrade != null)
                    {
                        this.upgrade.Close(helper.RemainingTime());
                    }
                    base.OnClose(helper.RemainingTime());
                }

                protected override void OnEndClose(IAsyncResult result)
                {
                    ChainedAsyncResult.End(result);
                }

                public bool TransferUpgrade(StreamUpgradeProvider upgrade)
                {
                    lock (base.ThisLock)
                    {
                        if (base.State != CommunicationState.Opened)
                        {
                            return false;
                        }
                        this.upgrade = upgrade;
                        return true;
                    }
                }
            }
        }
    }
}

