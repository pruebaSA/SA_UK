namespace System.IdentityModel.Selectors
{
    using System;
    using System.Runtime.Serialization;

    [Serializable]
    public class ServiceNotStartedException : Exception
    {
        public ServiceNotStartedException()
        {
        }

        public ServiceNotStartedException(string message) : base(message)
        {
        }

        protected ServiceNotStartedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public ServiceNotStartedException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}

