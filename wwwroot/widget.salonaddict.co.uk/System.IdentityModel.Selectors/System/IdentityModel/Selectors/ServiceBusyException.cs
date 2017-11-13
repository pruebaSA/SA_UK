namespace System.IdentityModel.Selectors
{
    using System;
    using System.Runtime.Serialization;

    [Serializable]
    public class ServiceBusyException : Exception
    {
        public ServiceBusyException()
        {
        }

        public ServiceBusyException(string message) : base(message)
        {
        }

        protected ServiceBusyException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public ServiceBusyException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}

