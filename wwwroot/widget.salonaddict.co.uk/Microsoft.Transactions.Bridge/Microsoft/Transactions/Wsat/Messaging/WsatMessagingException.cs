namespace Microsoft.Transactions.Wsat.Messaging
{
    using System;

    internal abstract class WsatMessagingException : Exception
    {
        protected WsatMessagingException(string message) : base(message)
        {
        }

        protected WsatMessagingException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}

