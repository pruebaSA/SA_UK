namespace Microsoft.Transactions.Wsat.Messaging
{
    using System;
    using System.ServiceModel;

    internal class InvalidMessageException : CommunicationException
    {
        public InvalidMessageException(string message) : base(message)
        {
        }

        public InvalidMessageException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}

