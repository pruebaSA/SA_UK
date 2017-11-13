namespace System.Data.Linq
{
    using System;

    public class DuplicateKeyException : InvalidOperationException
    {
        private object duplicate;

        public DuplicateKeyException(object duplicate)
        {
            this.duplicate = duplicate;
        }

        public DuplicateKeyException(object duplicate, string message) : base(message)
        {
            this.duplicate = duplicate;
        }

        public DuplicateKeyException(object duplicate, string message, Exception innerException) : base(message, innerException)
        {
            this.duplicate = duplicate;
        }

        public object Object =>
            this.duplicate;
    }
}

