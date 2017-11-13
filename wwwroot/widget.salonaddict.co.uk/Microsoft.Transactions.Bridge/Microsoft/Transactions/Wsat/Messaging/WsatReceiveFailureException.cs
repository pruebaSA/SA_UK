namespace Microsoft.Transactions.Wsat.Messaging
{
    using System;

    internal class WsatReceiveFailureException : WsatMessagingException
    {
        public WsatReceiveFailureException(Exception inner) : base(inner.Message, inner)
        {
        }

        public WsatReceiveFailureException(string message) : base(message)
        {
        }
    }
}

