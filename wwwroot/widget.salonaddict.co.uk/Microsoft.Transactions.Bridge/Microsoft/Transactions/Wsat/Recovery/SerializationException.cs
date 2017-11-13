namespace Microsoft.Transactions.Wsat.Recovery
{
    using System;
    using System.Runtime.Serialization;

    [Serializable]
    internal class SerializationException : Exception
    {
        public SerializationException()
        {
        }

        public SerializationException(string message) : base(message)
        {
        }

        protected SerializationException(SerializationInfo serInfo, StreamingContext streaming) : base(serInfo, streaming)
        {
        }

        public SerializationException(string message, Exception e) : base(message, e)
        {
        }
    }
}

