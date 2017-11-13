namespace System.ServiceModel.Channels
{
    using System;

    internal interface ISingletonChannelListener
    {
        void ReceiveRequest(RequestContext requestContext, ItemDequeuedCallback callback, bool canDispatchOnThisThread);

        TimeSpan ReceiveTimeout { get; }
    }
}

