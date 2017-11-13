namespace System.Data.Linq
{
    using System;

    public class ForeignKeyReferenceAlreadyHasValueException : InvalidOperationException
    {
        public ForeignKeyReferenceAlreadyHasValueException()
        {
        }

        public ForeignKeyReferenceAlreadyHasValueException(string message) : base(message)
        {
        }

        public ForeignKeyReferenceAlreadyHasValueException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}

