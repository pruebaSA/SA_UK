namespace System.Data
{
    using System;
    using System.Runtime.Serialization;

    [Serializable]
    public sealed class ObjectNotFoundException : DataException
    {
        public ObjectNotFoundException()
        {
        }

        public ObjectNotFoundException(string message) : base(message)
        {
        }

        private ObjectNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public ObjectNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}

