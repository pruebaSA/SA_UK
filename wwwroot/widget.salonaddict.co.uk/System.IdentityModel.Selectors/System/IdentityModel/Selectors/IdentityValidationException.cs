namespace System.IdentityModel.Selectors
{
    using System;
    using System.Runtime.Serialization;

    [Serializable]
    public class IdentityValidationException : Exception
    {
        public IdentityValidationException()
        {
        }

        public IdentityValidationException(string message) : base(message)
        {
        }

        protected IdentityValidationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public IdentityValidationException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}

