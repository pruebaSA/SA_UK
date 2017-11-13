namespace System.ServiceModel.Dispatcher
{
    using System;
    using System.Net;
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Web;

    internal class ContentTypeSettingClientMessageFormatter : IClientMessageFormatter
    {
        private IClientMessageFormatter innerFormatter;
        private string outgoingContentType;

        public ContentTypeSettingClientMessageFormatter(string outgoingContentType, IClientMessageFormatter innerFormatter)
        {
            if (outgoingContentType == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("outgoingContentType");
            }
            if (innerFormatter == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("innerFormatter");
            }
            this.outgoingContentType = outgoingContentType;
            this.innerFormatter = innerFormatter;
        }

        private static void AddRequestContentTypeProperty(Message message, string contentType)
        {
            if (message == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("message");
            }
            if (contentType == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("contentType");
            }
            if ((OperationContext.Current != null) && OperationContext.Current.HasOutgoingMessageProperties)
            {
                if (string.IsNullOrEmpty(WebOperationContext.Current.OutgoingRequest.ContentType))
                {
                    WebOperationContext.Current.OutgoingRequest.ContentType = contentType;
                }
            }
            else
            {
                object obj2;
                HttpRequestMessageProperty property;
                message.Properties.TryGetValue(HttpRequestMessageProperty.Name, out obj2);
                if (obj2 != null)
                {
                    property = (HttpRequestMessageProperty) obj2;
                }
                else
                {
                    property = new HttpRequestMessageProperty();
                    message.Properties.Add(HttpRequestMessageProperty.Name, property);
                }
                if (string.IsNullOrEmpty(property.Headers[HttpRequestHeader.ContentType]))
                {
                    property.Headers[HttpRequestHeader.ContentType] = contentType;
                }
            }
        }

        public object DeserializeReply(Message message, object[] parameters) => 
            this.innerFormatter.DeserializeReply(message, parameters);

        public Message SerializeRequest(MessageVersion messageVersion, object[] parameters)
        {
            Message message = this.innerFormatter.SerializeRequest(messageVersion, parameters);
            if (message != null)
            {
                AddRequestContentTypeProperty(message, this.outgoingContentType);
            }
            return message;
        }
    }
}

