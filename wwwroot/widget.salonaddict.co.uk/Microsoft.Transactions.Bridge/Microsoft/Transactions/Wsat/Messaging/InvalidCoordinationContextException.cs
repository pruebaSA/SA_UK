namespace Microsoft.Transactions.Wsat.Messaging
{
    using System;
    using System.ServiceModel;

    internal class InvalidCoordinationContextException : CommunicationException
    {
        public InvalidCoordinationContextException(string message) : base(message)
        {
        }

        public InvalidCoordinationContextException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}

