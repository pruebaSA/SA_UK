namespace System.ServiceModel.Channels
{
    using System;
    using System.Diagnostics;
    using System.ServiceModel;
    using System.ServiceModel.Diagnostics;

    public abstract class ChannelBase : CommunicationObject, IChannel, ICommunicationObject, IDefaultCommunicationTimeouts
    {
        private ChannelManagerBase channelManager;

        protected ChannelBase(ChannelManagerBase channelManager)
        {
            if (channelManager == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("channelManager");
            }
            this.channelManager = channelManager;
            if (DiagnosticUtility.ShouldTraceVerbose)
            {
                DiagnosticUtility.DiagnosticTrace.TraceEvent(TraceEventType.Verbose, TraceCode.ChannelCreated, System.ServiceModel.SR.GetString("TraceCodeChannelCreated", new object[] { DiagnosticTrace.CreateSourceString(this) }), null, null, this);
            }
        }

        public virtual T GetProperty<T>() where T: class
        {
            IChannelFactory channelManager = this.channelManager as IChannelFactory;
            if (channelManager != null)
            {
                return channelManager.GetProperty<T>();
            }
            IChannelListener listener = this.channelManager as IChannelListener;
            if (listener != null)
            {
                return listener.GetProperty<T>();
            }
            return default(T);
        }

        protected override void OnClosed()
        {
            base.OnClosed();
            if (DiagnosticUtility.ShouldTraceVerbose)
            {
                DiagnosticUtility.DiagnosticTrace.TraceEvent(TraceEventType.Verbose, TraceCode.ChannelDisposed, System.ServiceModel.SR.GetString("TraceCodeChannelDisposed", new object[] { DiagnosticTrace.CreateSourceString(this) }), null, null, this);
            }
        }

        protected override TimeSpan DefaultCloseTimeout =>
            ((IDefaultCommunicationTimeouts) this.channelManager).CloseTimeout;

        protected override TimeSpan DefaultOpenTimeout =>
            ((IDefaultCommunicationTimeouts) this.channelManager).OpenTimeout;

        protected TimeSpan DefaultReceiveTimeout =>
            ((IDefaultCommunicationTimeouts) this.channelManager).ReceiveTimeout;

        protected TimeSpan DefaultSendTimeout =>
            ((IDefaultCommunicationTimeouts) this.channelManager).SendTimeout;

        protected ChannelManagerBase Manager =>
            this.channelManager;

        TimeSpan IDefaultCommunicationTimeouts.CloseTimeout =>
            this.DefaultCloseTimeout;

        TimeSpan IDefaultCommunicationTimeouts.OpenTimeout =>
            this.DefaultOpenTimeout;

        TimeSpan IDefaultCommunicationTimeouts.ReceiveTimeout =>
            this.DefaultReceiveTimeout;

        TimeSpan IDefaultCommunicationTimeouts.SendTimeout =>
            this.DefaultSendTimeout;
    }
}

