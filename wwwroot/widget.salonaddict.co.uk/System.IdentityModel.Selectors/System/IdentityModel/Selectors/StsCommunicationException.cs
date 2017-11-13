namespace System.IdentityModel.Selectors
{
    using System;
    using System.Runtime.Serialization;

    [Serializable]
    public class StsCommunicationException : Exception
    {
        public StsCommunicationException()
        {
        }

        public StsCommunicationException(string message) : base(message)
        {
        }

        protected StsCommunicationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public StsCommunicationException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}

