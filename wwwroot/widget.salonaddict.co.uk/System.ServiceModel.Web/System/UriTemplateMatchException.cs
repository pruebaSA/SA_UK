namespace System
{
    using System.Runtime.Serialization;

    [Serializable]
    public class UriTemplateMatchException : SystemException
    {
        public UriTemplateMatchException()
        {
        }

        public UriTemplateMatchException(string message) : base(message)
        {
        }

        protected UriTemplateMatchException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public UriTemplateMatchException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}

