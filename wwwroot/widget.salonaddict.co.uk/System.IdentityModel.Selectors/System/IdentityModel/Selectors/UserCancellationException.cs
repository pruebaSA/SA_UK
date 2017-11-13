namespace System.IdentityModel.Selectors
{
    using System;
    using System.Runtime.Serialization;

    [Serializable]
    public class UserCancellationException : Exception
    {
        public UserCancellationException()
        {
        }

        public UserCancellationException(string message) : base(message)
        {
        }

        protected UserCancellationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public UserCancellationException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}

