namespace System.Data.Linq
{
    using System;

    public class ChangeConflictException : Exception
    {
        public ChangeConflictException()
        {
        }

        public ChangeConflictException(string message) : base(message)
        {
        }

        public ChangeConflictException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}

