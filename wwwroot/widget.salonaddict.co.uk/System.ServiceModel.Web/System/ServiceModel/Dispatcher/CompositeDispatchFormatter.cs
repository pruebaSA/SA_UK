namespace System.ServiceModel.Dispatcher
{
    using System;
    using System.ServiceModel.Channels;

    internal class CompositeDispatchFormatter : IDispatchMessageFormatter
    {
        private IDispatchMessageFormatter reply;
        private IDispatchMessageFormatter request;

        public CompositeDispatchFormatter(IDispatchMessageFormatter request, IDispatchMessageFormatter reply)
        {
            this.request = request;
            this.reply = reply;
        }

        public void DeserializeRequest(Message message, object[] parameters)
        {
            this.request.DeserializeRequest(message, parameters);
        }

        public Message SerializeReply(MessageVersion messageVersion, object[] parameters, object result) => 
            this.reply.SerializeReply(messageVersion, parameters, result);
    }
}

