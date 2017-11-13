namespace Microsoft.Practices.ObjectBuilder2
{
    using System;
    using System.Runtime.Serialization;

    [Serializable]
    public class IllegalInjectionMethodException : Exception
    {
        public IllegalInjectionMethodException()
        {
        }

        public IllegalInjectionMethodException(string message) : base(message)
        {
        }

        protected IllegalInjectionMethodException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public IllegalInjectionMethodException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}

