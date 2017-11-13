namespace System.IO
{
    using System;
    using System.Runtime.InteropServices;
    using System.Runtime.Serialization;

    [Serializable, ComVisible(true)]
    public class EndOfStreamException : IOException
    {
        public EndOfStreamException() : base(Environment.GetResourceString("Arg_EndOfStreamException"))
        {
            base.SetErrorCode(-2147024858);
        }

        public EndOfStreamException(string message) : base(message)
        {
            base.SetErrorCode(-2147024858);
        }

        protected EndOfStreamException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public EndOfStreamException(string message, Exception innerException) : base(message, innerException)
        {
            base.SetErrorCode(-2147024858);
        }
    }
}

