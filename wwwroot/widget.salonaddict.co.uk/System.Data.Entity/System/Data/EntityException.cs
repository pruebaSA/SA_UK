namespace System.Data
{
    using System;
    using System.Runtime.Serialization;

    [Serializable]
    public class EntityException : DataException
    {
        public EntityException() : base(Strings.EntityClient_ProviderGeneralError)
        {
        }

        public EntityException(string message) : base(message)
        {
        }

        protected EntityException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public EntityException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}

