namespace System.IdentityModel.Selectors
{
    using System;
    using System.Runtime.Serialization;

    [Serializable]
    public class PolicyValidationException : Exception
    {
        public PolicyValidationException()
        {
        }

        public PolicyValidationException(string message) : base(message)
        {
        }

        protected PolicyValidationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public PolicyValidationException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}

