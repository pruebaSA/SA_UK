namespace System.ServiceModel.Activation
{
    using System;
    using System.ServiceModel;
    using System.ServiceModel.Channels;

    internal class HostedTcpTransportManager : SharedTcpTransportManager
    {
        private OnViaDelegate onViaCallback;
        private bool settingsApplied;

        public HostedTcpTransportManager(BaseUriWithWildcard baseAddress) : base(baseAddress.BaseAddress)
        {
            base.HostNameComparisonMode = baseAddress.HostNameComparisonMode;
            this.onViaCallback = new OnViaDelegate(this.OnVia);
        }

        protected override OnViaDelegate GetOnViaCallback() => 
            this.onViaCallback;

        internal override void OnOpen()
        {
        }

        protected override void OnSelecting(TcpChannelListener channelListener)
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
            base.OnOpenInternal(queueId, token);
        }

        internal void Stop()
        {
            base.CleanUp();
            this.settingsApplied = false;
        }
    }
}

