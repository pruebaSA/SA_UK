namespace System.ServiceModel.Channels
{
    using System;
    using System.Collections.Generic;
    using System.ServiceModel;
    using System.ServiceModel.Dispatcher;

    internal class ChannelDemuxer
    {
        private TypedChannelDemuxer inputDemuxer;
        private int maxPendingSessions = 10;
        private TimeSpan peekTimeout = TimeSpan.MinValue;
        private TypedChannelDemuxer replyDemuxer;
        private Dictionary<Type, TypedChannelDemuxer> typeDemuxers = new Dictionary<Type, TypedChannelDemuxer>();

        public IChannelListener<TChannel> BuildChannelListener<TChannel>(BindingContext context) where TChannel: class, IChannel => 
            this.BuildChannelListener<TChannel>(context, new ChannelDemuxerFilter(new MatchAllMessageFilter(), 0));

        public IChannelListener<TChannel> BuildChannelListener<TChannel>(BindingContext context, ChannelDemuxerFilter filter) where TChannel: class, IChannel => 
            this.GetTypedDemuxer(typeof(TChannel), context).BuildChannelListener<TChannel>(filter);

        private TypedChannelDemuxer CreateTypedDemuxer(Type channelType, BindingContext context)
        {
            if (channelType == typeof(IDuplexChannel))
            {
                return (TypedChannelDemuxer) new DuplexChannelDemuxer(context);
            }
            if (channelType == typeof(IInputSessionChannel))
            {
                return (TypedChannelDemuxer) new InputSessionChannelDemuxer(context, this.peekTimeout, this.maxPendingSessions);
            }
            if (channelType == typeof(IReplySessionChannel))
            {
                return (TypedChannelDemuxer) new ReplySessionChannelDemuxer(context, this.peekTimeout, this.maxPendingSessions);
            }
            if (channelType != typeof(IDuplexSessionChannel))
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new NotSupportedException());
            }
            return (TypedChannelDemuxer) new DuplexSessionChannelDemuxer(context, this.peekTimeout, this.maxPendingSessions);
        }

        private TypedChannelDemuxer GetTypedDemuxer(Type channelType, BindingContext context)
        {
            TypedChannelDemuxer inputDemuxer = null;
            bool flag = false;
            if (channelType == typeof(IInputChannel))
            {
                if (this.inputDemuxer == null)
                {
                    if (context.CanBuildInnerChannelListener<IReplyChannel>())
                    {
                        this.inputDemuxer = this.replyDemuxer = new ReplyChannelDemuxer(context);
                    }
                    else
                    {
                        this.inputDemuxer = new InputChannelDemuxer(context);
                    }
                    flag = true;
                }
                inputDemuxer = this.inputDemuxer;
            }
            else if (channelType == typeof(IReplyChannel))
            {
                if (this.replyDemuxer == null)
                {
                    this.inputDemuxer = this.replyDemuxer = new ReplyChannelDemuxer(context);
                    flag = true;
                }
                inputDemuxer = this.replyDemuxer;
            }
            else if (!this.typeDemuxers.TryGetValue(channelType, out inputDemuxer))
            {
                inputDemuxer = this.CreateTypedDemuxer(channelType, context);
                this.typeDemuxers.Add(channelType, inputDemuxer);
                flag = true;
            }
            if (!flag)
            {
                context.RemainingBindingElements.Clear();
            }
            return inputDemuxer;
        }

        public int MaxPendingSessions
        {
            get => 
                this.maxPendingSessions;
            set
            {
                if (value < 1)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("value"));
                }
                this.maxPendingSessions = value;
            }
        }

        public TimeSpan PeekTimeout
        {
            get => 
                this.peekTimeout;
            set
            {
                if ((value < TimeSpan.Zero) && (value != TimeSpan.MinValue))
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentOutOfRangeException("value"));
                }
                this.peekTimeout = value;
            }
        }
    }
}

