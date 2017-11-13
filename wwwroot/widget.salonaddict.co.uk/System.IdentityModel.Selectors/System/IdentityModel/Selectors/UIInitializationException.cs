namespace System.IdentityModel.Selectors
{
    using System;
    using System.Runtime.Serialization;

    [Serializable]
    internal class UIInitializationException : Exception
    {
        public UIInitializationException()
        {
        }

        public UIInitializationException(string message) : base(message)
        {
        }

        protected UIInitializationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public UIInitializationException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}

