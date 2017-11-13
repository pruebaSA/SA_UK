namespace System.ServiceModel.Dispatcher
{
    using System;
    using System.ServiceModel.Channels;

    internal class CompositeClientFormatter : IClientMessageFormatter
    {
        private IClientMessageFormatter reply;
        private IClientMessageFormatter request;

        public CompositeClientFormatter(IClientMessageFormatter request, IClientMessageFormatter reply)
        {
            this.request = request;
            this.reply = reply;
        }

        public object DeserializeReply(Message message, object[] parameters) => 
            this.reply.DeserializeReply(message, parameters);

        public Message SerializeRequest(MessageVersion messageVersion, object[] parameters) => 
            this.request.SerializeRequest(messageVersion, parameters);
    }
}

