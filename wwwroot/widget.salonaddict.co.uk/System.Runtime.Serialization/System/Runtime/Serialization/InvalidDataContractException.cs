namespace System.Runtime.Serialization
{
    using System;

    [Serializable]
    public class InvalidDataContractException : Exception
    {
        public InvalidDataContractException()
        {
        }

        public InvalidDataContractException(string message) : base(message)
        {
        }

        protected InvalidDataContractException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public InvalidDataContractException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}

