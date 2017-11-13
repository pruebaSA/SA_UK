namespace System.IO
{
    using System;
    using System.Runtime.InteropServices;
    using System.Runtime.Serialization;

    [Serializable, ComVisible(true)]
    public class PathTooLongException : IOException
    {
        public PathTooLongException() : base(Environment.GetResourceString("IO.PathTooLong"))
        {
            base.SetErrorCode(-2147024690);
        }

        public PathTooLongException(string message) : base(message)
        {
            base.SetErrorCode(-2147024690);
        }

        protected PathTooLongException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public PathTooLongException(string message, Exception innerException) : base(message, innerException)
        {
            base.SetErrorCode(-2147024690);
        }
    }
}

