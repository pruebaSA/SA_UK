namespace Microsoft.Transactions.Wsat.Messaging
{
    using System;
    using System.ServiceModel;

    internal class InvalidEnlistmentHeaderException : CommunicationException
    {
        public InvalidEnlistmentHeaderException(string message) : base(message)
        {
        }

        public InvalidEnlistmentHeaderException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}

