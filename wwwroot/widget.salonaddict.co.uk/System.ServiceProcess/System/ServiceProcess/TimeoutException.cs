namespace System.ServiceProcess
{
    using System;
    using System.Runtime.Serialization;

    [Serializable]
    public class TimeoutException : SystemException
    {
        public TimeoutException()
        {
            base.HResult = -2146232058;
        }

        public TimeoutException(string message) : base(message)
        {
            base.HResult = -2146232058;
        }

        protected TimeoutException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public TimeoutException(string message, Exception innerException) : base(message, innerException)
        {
            base.HResult = -2146232058;
        }
    }
}

