namespace System.ServiceModel.Channels
{
    using System;

    internal class ReplyFaultHelper : TypedFaultHelper<FaultState>
    {
        public ReplyFaultHelper(TimeSpan defaultSendTimeout, TimeSpan defaultCloseTimeout) : base(defaultSendTimeout, defaultCloseTimeout)
        {
        }

        protected override void AbortState(FaultState faultState)
        {
            faultState.FaultMessage.Close();
            faultState.RequestContext.Abort();
        }

        protected override IAsyncResult BeginSendFault(IReliableChannelBinder binder, FaultState faultState, TimeSpan timeout, AsyncCallback callback, object state) => 
            faultState.RequestContext.BeginReply(faultState.FaultMessage, timeout, callback, state);

        protected override void EndSendFault(IReliableChannelBinder binder, FaultState faultState, IAsyncResult result)
        {
            faultState.RequestContext.EndReply(result);
            faultState.FaultMessage.Close();
        }

        protected override FaultState GetState(RequestContext requestContext, Message faultMessage) => 
            new FaultState(requestContext, faultMessage);
    }
}

