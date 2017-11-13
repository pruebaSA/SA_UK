namespace System.ServiceModel.Channels
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using System.ServiceModel;
    using System.ServiceModel.Activation;

    internal class SharedTcpTransportManager : TcpTransportManager, ITransportManagerRegistration
    {
        private ConnectionDemuxer connectionDemuxer;
        private bool demuxerCreated;
        private System.ServiceModel.HostNameComparisonMode hostNameComparisonMode;
        private SharedConnectionListener listener;
        private Uri listenUri;
        private OnDuplicatedViaDelegate onDuplicatedViaCallback;
        private int queueId;
        private Guid token;

        protected SharedTcpTransportManager(Uri listenUri)
        {
            this.listenUri = listenUri;
        }

        public SharedTcpTransportManager(Uri listenUri, TcpChannelListener channelListener)
        {
            this.HostNameComparisonMode = channelListener.HostNameComparisonMode;
            this.listenUri = listenUri;
            base.ApplyListenerSettings(channelListener);
        }

        protected void CleanUp()
        {
            lock (base.ThisLock)
            {
                if (this.listener != null)
                {
                    this.listener.Stop();
                    this.listener = null;
                }
                if (this.connectionDemuxer != null)
                {
                    this.connectionDemuxer.Dispose();
                }
                this.demuxerCreated = false;
            }
        }

        private void CreateConnectionDemuxer()
        {
            IConnectionListener listener = new BufferedConnectionListener(this.listener, base.MaxOutputDelay, base.ConnectionBufferSize);
            if (DiagnosticUtility.ShouldUseActivity)
            {
                listener = new TracingConnectionListener(listener, this.ListenUri);
            }
            this.connectionDemuxer = new ConnectionDemuxer(listener, base.MaxPendingAccepts, base.MaxPendingConnections, base.ChannelInitializationTimeout, base.IdleTimeout, base.MaxPooledConnections, new TransportSettingsCallback(this.OnGetTransportFactorySettings), new SingletonPreambleDemuxCallback(this.OnGetSingletonMessageHandler), new ServerSessionPreambleDemuxCallback(this.OnHandleServerSessionPreamble), new ErrorCallback(this.OnDemuxerError));
            this.connectionDemuxer.StartDemuxing(this.GetOnViaCallback());
        }

        protected virtual OnViaDelegate GetOnViaCallback() => 
            null;

        protected override bool IsCompatible(TcpChannelListener channelListener)
        {
            if ((channelListener.HostedVirtualPath == null) && !channelListener.PortSharingEnabled)
            {
                return false;
            }
            return base.IsCompatible(channelListener);
        }

        internal override void OnClose()
        {
            this.CleanUp();
            TcpChannelListener.StaticTransportManagerTable.UnregisterUri(this.ListenUri, this.HostNameComparisonMode);
        }

        private void OnDuplicatedVia(Uri via, out int connectionBufferSize)
        {
            OnViaDelegate onViaCallback = this.GetOnViaCallback();
            if (onViaCallback != null)
            {
                onViaCallback(via);
            }
            if (!this.demuxerCreated)
            {
                lock (base.ThisLock)
                {
                    if (this.listener == null)
                    {
                        throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new CommunicationObjectAbortedException(System.ServiceModel.SR.GetString("Sharing_ListenerProxyStopped")));
                    }
                    if (!this.demuxerCreated)
                    {
                        this.CreateConnectionDemuxer();
                        this.demuxerCreated = true;
                    }
                }
            }
            connectionBufferSize = base.ConnectionBufferSize;
        }

        internal override void OnOpen()
        {
            this.OnOpenInternal(0, Guid.Empty);
        }

        internal void OnOpenInternal(int queueId, Guid token)
        {
            lock (base.ThisLock)
            {
                this.queueId = queueId;
                this.token = token;
                BaseUriWithWildcard baseAddress = new BaseUriWithWildcard(this.ListenUri, this.HostNameComparisonMode);
                if (this.onDuplicatedViaCallback == null)
                {
                    this.onDuplicatedViaCallback = new OnDuplicatedViaDelegate(this.OnDuplicatedVia);
                }
                this.listener = new SharedConnectionListener(baseAddress, queueId, token, this.onDuplicatedViaCallback);
            }
        }

        protected virtual void OnSelecting(TcpChannelListener channelListener)
        {
        }

        IList<TransportManager> ITransportManagerRegistration.Select(TransportChannelListener channelListener)
        {
            if (!channelListener.IsScopeIdCompatible(this.hostNameComparisonMode, this.listenUri))
            {
                return null;
            }
            this.OnSelecting((TcpChannelListener) channelListener);
            IList<TransportManager> list = null;
            if (this.IsCompatible((TcpChannelListener) channelListener))
            {
                list = new List<TransportManager> {
                    this
                };
            }
            return list;
        }

        public System.ServiceModel.HostNameComparisonMode HostNameComparisonMode
        {
            get => 
                this.hostNameComparisonMode;
            set
            {
                HostNameComparisonModeHelper.Validate(value);
                lock (base.ThisLock)
                {
                    base.ThrowIfOpen();
                    this.hostNameComparisonMode = value;
                }
            }
        }

        public Uri ListenUri =>
            this.listenUri;
    }
}

