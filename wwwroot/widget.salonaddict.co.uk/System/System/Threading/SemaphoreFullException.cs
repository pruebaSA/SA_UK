namespace System.Threading
{
    using System;
    using System.Runtime.InteropServices;
    using System.Runtime.Serialization;

    [Serializable, ComVisible(false)]
    public class SemaphoreFullException : SystemException
    {
        public SemaphoreFullException() : base(SR.GetString("Threading_SemaphoreFullException"))
        {
        }

        public SemaphoreFullException(string message) : base(message)
        {
        }

        protected SemaphoreFullException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public SemaphoreFullException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}

