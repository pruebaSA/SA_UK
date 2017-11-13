namespace System.IdentityModel.Selectors
{
    using System;
    using System.Runtime.Serialization;

    [Serializable]
    public class CardSpaceException : Exception
    {
        public CardSpaceException()
        {
        }

        public CardSpaceException(string message) : base(message)
        {
        }

        protected CardSpaceException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public CardSpaceException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}

