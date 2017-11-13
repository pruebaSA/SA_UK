namespace Microsoft.Transactions.Wsat.Messaging
{
    using System;

    internal class CreateChannelFailureException : Exception
    {
        public CreateChannelFailureException(string message) : base(message)
        {
        }

        public CreateChannelFailureException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}

