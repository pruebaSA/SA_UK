namespace System.Data
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    [Serializable]
    public sealed class OptimisticConcurrencyException : UpdateException
    {
        public OptimisticConcurrencyException()
        {
        }

        public OptimisticConcurrencyException(string message) : base(message)
        {
        }

        private OptimisticConcurrencyException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public OptimisticConcurrencyException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public OptimisticConcurrencyException(string message, Exception innerException, IEnumerable<ObjectStateEntry> stateEntries) : base(message, innerException, stateEntries)
        {
        }
    }
}

