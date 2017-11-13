namespace System.ServiceModel.Diagnostics
{
    using System;
    using System.Runtime.Serialization;

    [Serializable]
    internal class FatalException : SystemException
    {
        public FatalException()
        {
        }

        public FatalException(string message) : base(message)
        {
        }

        protected FatalException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public FatalException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}

