namespace System.IdentityModel.Selectors
{
    using System;
    using System.Runtime.Serialization;

    [Serializable]
    public class UntrustedRecipientException : Exception
    {
        public UntrustedRecipientException()
        {
        }

        public UntrustedRecipientException(string message) : base(message)
        {
        }

        protected UntrustedRecipientException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public UntrustedRecipientException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}

