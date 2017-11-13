namespace System.ServiceModel.Diagnostics
{
    using System;
    using System.Runtime.Serialization;

    [Serializable]
    internal class CallbackException : FatalException
    {
        public CallbackException()
        {
        }

        protected CallbackException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public CallbackException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}

