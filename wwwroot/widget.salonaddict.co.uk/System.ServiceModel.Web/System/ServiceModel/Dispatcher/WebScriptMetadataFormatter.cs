namespace System.ServiceModel.Dispatcher
{
    using System;
    using System.ServiceModel.Channels;

    internal class WebScriptMetadataFormatter : IDispatchMessageFormatter
    {
        public void DeserializeRequest(Message message, object[] parameters)
        {
            parameters[0] = message;
        }

        public Message SerializeReply(MessageVersion messageVersion, object[] parameters, object result) => 
            (result as Message);
    }
}

