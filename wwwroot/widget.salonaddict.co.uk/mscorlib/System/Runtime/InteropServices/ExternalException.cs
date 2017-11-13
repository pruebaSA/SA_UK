namespace System.Runtime.InteropServices
{
    using System;
    using System.Runtime.Serialization;

    [Serializable, ComVisible(true)]
    public class ExternalException : SystemException
    {
        public ExternalException() : base(Environment.GetResourceString("Arg_ExternalException"))
        {
            base.SetErrorCode(-2147467259);
        }

        public ExternalException(string message) : base(message)
        {
            base.SetErrorCode(-2147467259);
        }

        protected ExternalException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public ExternalException(string message, Exception inner) : base(message, inner)
        {
            base.SetErrorCode(-2147467259);
        }

        public ExternalException(string message, int errorCode) : base(message)
        {
            base.SetErrorCode(errorCode);
        }

        public virtual int ErrorCode =>
            base.HResult;
    }
}

