namespace System.ServiceModel.Activation
{
    using System;
    using System.Runtime.InteropServices;
    using System.ServiceModel;
    using System.ServiceModel.Channels;

    internal class HostedNamedPipeTransportManager : NamedPipeTransportManager
    {
        private ConnectionDemuxer connectionDemuxer;
        private bool demuxerCreated;
        private SharedConnectionListener listener;
        private OnDuplicatedViaDelegate onDuplicatedViaCallback;
        private OnViaDelegate onViaCallback;
        private int queueId;
        private bool settingsApplied;
        private Guid token;

        public HostedNamedPipeTransportManager(BaseUriWithWildcard baseAddress) : base(baseAddress.BaseAddress)
        {
            base.HostNameComparisonMode = baseAddress.HostNameComparisonMode;
            this.onViaCallback = new OnViaDelegate(this.OnVia);
            this.onDuplicatedViaCallback = new OnDuplicatedViaDelegate(this.OnDuplicatedVia);
        }

        private void CreateConnectionDemuxer()
        {
            IConnectionListener listener = new BufferedConnectionListener(this.listener, base.MaxOutputDelay, base.ConnectionBufferSize);
            if (DiagnosticUtility.ShouldUseActivity)
            {
                listener = new TracingConnectionListener(listener, base.ListenUri);
            }
            this.connectionDemuxer = new ConnectionDemuxer(listener, base.MaxPendingAccepts, base.MaxPendingConnections, base.ChannelInitializationTimeout, base.IdleTimeout, base.MaxPooledConnections, new TransportSettingsCallback(this.OnGetTransportFactorySettings), new SingletonPreambleDemuxCallback(this.OnGetSingletonMessageHandler), new ServerSessionPreambleDemuxCallback(this.OnHandleServerSessionPreamble), new ErrorCallback(this.OnDemuxerError));
            this.connectionDemuxer.StartDemuxing(this.onViaCallback);
        }

        protected override bool IsCompatible(NamedPipeChannelListener channelListener)
        {
            if (channelListener.HostedVirtualPath == null)
            {
                return false;
            }
            return base.IsCompatible(channelListener);
        }

        internal override void OnClose()
        {
            this.Stop();
            base.OnClose();
        }

        private void OnDuplicatedVia(Uri via, out int connectionBufferSize)
        {
            this.OnVia(via);
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
        }

        private void OnOpenInternal(int queueId, Guid token)
        {
            lock (base.ThisLock)
            {
                this.queueId = queueId;
                this.token = token;
                BaseUriWithWildcard baseAddress = new BaseUriWithWildcard(base.ListenUri, base.HostNameComparisonMode);
                this.listener = new SharedConnectionListener(baseAddress, queueId, token, this.onDuplicatedViaCallback);
            }
        }

        protected override void OnSelecting(NamedPipeChannelListener channelListener)
        {
            if (!this.settingsApplied)
            {
                lock (base.ThisLock)
                {
                    if (!this.settingsApplied)
                    {
                        base.ApplyListenerSettings(channelListener);
                        this.settingsApplied = true;
                    }
                }
            }
        }

        private void OnVia(Uri address)
        {
            ServiceHostingEnvironment.EnsureServiceAvailable(address.LocalPath);
        }

        internal void Start(int queueId, Guid token, MessageReceivedCallback messageReceivedCallback)
        {
            base.SetMessageReceivedCallback(messageReceivedCallback);
            this.OnOpenInternal(queueId, token);
        }

        internal void Stop()
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
                this.settingsApplied = false;
            }
        }
    }
}

