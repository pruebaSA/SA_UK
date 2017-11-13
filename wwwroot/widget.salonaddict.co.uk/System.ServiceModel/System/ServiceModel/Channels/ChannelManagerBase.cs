namespace System.ServiceModel.Channels
{
    using System;
    using System.ServiceModel;

    public abstract class ChannelManagerBase : CommunicationObject, IDefaultCommunicationTimeouts
    {
        protected ChannelManagerBase()
        {
        }

        internal Exception CreateChannelTypeNotSupportedException(Type type) => 
            new ArgumentException(System.ServiceModel.SR.GetString("ChannelTypeNotSupported", new object[] { type }), "TChannel");

        protected abstract TimeSpan DefaultReceiveTimeout { get; }

        protected abstract TimeSpan DefaultSendTimeout { get; }

        internal TimeSpan InternalReceiveTimeout =>
            this.DefaultReceiveTimeout;

        internal TimeSpan InternalSendTimeout =>
            this.DefaultSendTimeout;

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

