namespace System.ServiceModel.Dispatcher
{
    using System;
    using System.Net;
    using System.ServiceModel;
    using System.ServiceModel.Channels;

    internal class WebFaultClientMessageInspector : IClientMessageInspector
    {
        public virtual void AfterReceiveReply(ref Message reply, object correlationState)
        {
            if (reply != null)
            {
                HttpResponseMessageProperty property = (HttpResponseMessageProperty) reply.Properties[HttpResponseMessageProperty.Name];
                if ((property != null) && (property.StatusCode == HttpStatusCode.InternalServerError))
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperError(new CommunicationException(property.StatusDescription));
                }
            }
        }

        public object BeforeSendRequest(ref Message request, IClientChannel channel) => 
            null;
    }
}

