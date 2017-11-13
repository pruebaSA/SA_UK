namespace System.Data
{
    using System;
    using System.Runtime.Serialization;

    [Serializable]
    public sealed class InvalidCommandTreeException : DataException
    {
        public InvalidCommandTreeException() : base(Strings.Cqt_Exceptions_InvalidCommandTree)
        {
        }

        public InvalidCommandTreeException(string message) : base(message)
        {
        }

        private InvalidCommandTreeException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public InvalidCommandTreeException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}

