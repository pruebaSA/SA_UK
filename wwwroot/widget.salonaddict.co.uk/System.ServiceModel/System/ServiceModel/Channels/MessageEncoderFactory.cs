namespace System.ServiceModel.Channels
{
    using System;

    public abstract class MessageEncoderFactory
    {
        protected MessageEncoderFactory()
        {
        }

        public virtual MessageEncoder CreateSessionEncoder() => 
            this.Encoder;

        public abstract MessageEncoder Encoder { get; }

        public abstract System.ServiceModel.Channels.MessageVersion MessageVersion { get; }
    }
}

