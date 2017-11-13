namespace Microsoft.Transactions.Wsat.Messaging
{
    using System;

    internal class MessagingInitializationException : Exception
    {
        public MessagingInitializationException(string message) : base(message)
        {
        }

        public MessagingInitializationException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}

