namespace System.ServiceModel.Dispatcher
{
    using System;
    using System.Runtime.Serialization;
    using System.ServiceModel.Channels;

    [KnownType(typeof(ActionMessageFilter)), DataContract, KnownType(typeof(XPathMessageFilter)), KnownType(typeof(MatchAllMessageFilter)), KnownType(typeof(MatchNoneMessageFilter))]
    public abstract class MessageFilter
    {
        protected MessageFilter()
        {
        }

        protected internal virtual IMessageFilterTable<FilterData> CreateFilterTable<FilterData>() => 
            null;

        public abstract bool Match(Message message);
        public abstract bool Match(MessageBuffer buffer);
    }
}

