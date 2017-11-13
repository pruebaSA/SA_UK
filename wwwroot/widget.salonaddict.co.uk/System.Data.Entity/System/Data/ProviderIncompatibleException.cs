namespace System.Data
{
    using System;
    using System.Runtime.Serialization;

    [Serializable]
    public sealed class ProviderIncompatibleException : EntityException
    {
        public ProviderIncompatibleException()
        {
        }

        public ProviderIncompatibleException(string message) : base(message)
        {
        }

        private ProviderIncompatibleException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public ProviderIncompatibleException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}

