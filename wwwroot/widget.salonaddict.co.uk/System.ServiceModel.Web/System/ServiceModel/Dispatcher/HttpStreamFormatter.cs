namespace System.ServiceModel.Dispatcher
{
    using System;
    using System.IO;
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Description;

    internal class HttpStreamFormatter : IDispatchMessageFormatter, IClientMessageFormatter
    {
        private string contractName;
        private string contractNs;
        private string operationName;

        public HttpStreamFormatter(OperationDescription operation)
        {
            if (operation == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("operation");
            }
            this.operationName = operation.Name;
            this.contractName = operation.DeclaringContract.Name;
            this.contractNs = operation.DeclaringContract.Namespace;
        }

        private Message CreateMessageFromStream(object data)
        {
            if (data == null)
            {
                return Message.CreateMessage(MessageVersion.None, (string) null);
            }
            Stream stream = data as Stream;
            if (stream == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new ArgumentException(SR2.GetString(SR2.ParameterIsNotStreamType, new object[] { data.GetType(), this.operationName, this.contractName, this.contractNs })));
            }
            return new HttpStreamMessage(stream) { Properties = { ["WebBodyFormatMessageProperty"] = WebBodyFormatMessageProperty.RawProperty } };
        }

        public object DeserializeReply(Message message, object[] parameters) => 
            this.GetStreamFromMessage(message, false);

        public void DeserializeRequest(Message message, object[] parameters)
        {
            parameters[0] = this.GetStreamFromMessage(message, true);
        }

        private Stream GetStreamFromMessage(Message message, bool isRequest)
        {
            object obj2;
            message.Properties.TryGetValue("WebBodyFormatMessageProperty", out obj2);
            WebBodyFormatMessageProperty property = obj2 as WebBodyFormatMessageProperty;
            if (property == null)
            {
                if (!IsEmptyMessage(message))
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperWarning(new InvalidOperationException(SR2.GetString(SR2.MessageFormatPropertyNotFound, new object[] { this.operationName, this.contractName, this.contractNs })));
                }
                return new MemoryStream();
            }
            if (property.Format != WebContentFormat.Raw)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperWarning(new InvalidOperationException(SR2.GetString(SR2.InvalidHttpMessageFormat, new object[] { this.operationName, this.contractName, this.contractNs, property.Format, WebContentFormat.Raw })));
            }
            return new StreamFormatter.MessageBodyStream(message, null, null, "Binary", string.Empty, isRequest);
        }

        internal static bool IsEmptyMessage(Message message) => 
            message.IsEmpty;

        public Message SerializeReply(MessageVersion messageVersion, object[] parameters, object result)
        {
            Message message = this.CreateMessageFromStream(result);
            if (result == null)
            {
                SingleBodyParameterMessageFormatter.SuppressReplyEntityBody(message);
            }
            return message;
        }

        public Message SerializeRequest(MessageVersion messageVersion, object[] parameters)
        {
            Message message = this.CreateMessageFromStream(parameters[0]);
            if (parameters[0] == null)
            {
                SingleBodyParameterMessageFormatter.SuppressRequestEntityBody(message);
            }
            return message;
        }
    }
}

