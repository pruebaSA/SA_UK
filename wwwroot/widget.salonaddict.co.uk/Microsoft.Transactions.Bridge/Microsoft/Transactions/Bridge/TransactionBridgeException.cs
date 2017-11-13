namespace Microsoft.Transactions.Bridge
{
    using System;
    using System.Runtime.Serialization;

    [Serializable]
    internal class TransactionBridgeException : Exception
    {
        public TransactionBridgeException()
        {
        }

        public TransactionBridgeException(string exception) : base(exception)
        {
        }

        protected TransactionBridgeException(SerializationInfo serInfo, StreamingContext streaming) : base(serInfo, streaming)
        {
        }
    }
}

