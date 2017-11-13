namespace Microsoft.Transactions.Wsat.Messaging
{
    using Microsoft.Transactions;
    using System;
    using System.ServiceModel.Channels;

    internal class WsatFaultException : WsatMessagingException
    {
        private string action;
        private MessageFault fault;

        public WsatFaultException(MessageFault fault, string action) : base(GetExceptionMessage(fault))
        {
            this.fault = fault;
            this.action = action;
        }

        private static string GetExceptionMessage(MessageFault fault) => 
            Microsoft.Transactions.SR.GetString("RequestReplyFault", new object[] { Library.GetFaultCodeName(fault), Library.GetFaultCodeReason(fault) });

        public string Action =>
            this.action;

        public MessageFault Fault =>
            this.fault;
    }
}

