namespace System.ServiceModel.Channels
{
    using System;
    using System.Diagnostics;
    using System.ServiceModel;
    using System.ServiceModel.Diagnostics;

    internal class ReplyChannelAcceptor : SingletonChannelAcceptor<IReplyChannel, ReplyChannel, RequestContext>
    {
        public ReplyChannelAcceptor(ChannelManagerBase channelManager) : base(channelManager)
        {
        }

        protected override ReplyChannel OnCreateChannel() => 
            new ReplyChannel(base.ChannelManager, null);

        protected override void OnTraceMessageReceived(RequestContext requestContext)
        {
            if (DiagnosticUtility.ShouldTraceInformation)
            {
                TraceUtility.TraceEvent(TraceEventType.Information, TraceCode.MessageReceived, MessageTransmitTraceRecord.CreateReceiveTraceRecord(requestContext?.RequestMessage), this, null);
            }
        }
    }
}

