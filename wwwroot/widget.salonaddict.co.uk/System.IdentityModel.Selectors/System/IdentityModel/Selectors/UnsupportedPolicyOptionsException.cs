namespace System.IdentityModel.Selectors
{
    using System;
    using System.Runtime.Serialization;

    [Serializable]
    public class UnsupportedPolicyOptionsException : Exception
    {
        public UnsupportedPolicyOptionsException()
        {
        }

        public UnsupportedPolicyOptionsException(string message) : base(message)
        {
        }

        protected UnsupportedPolicyOptionsException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public UnsupportedPolicyOptionsException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}

