namespace System.ServiceModel.Channels
{
    using System;
    using System.ServiceModel.Dispatcher;

    internal class ChannelDemuxerFilter
    {
        private MessageFilter filter;
        private int priority;

        public ChannelDemuxerFilter(MessageFilter filter, int priority)
        {
            this.filter = filter;
            this.priority = priority;
        }

        public MessageFilter Filter =>
            this.filter;

        public int Priority =>
            this.priority;
    }
}

