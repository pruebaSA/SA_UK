namespace SA.Payments.Realex.RealAuth
{
    using System;

    public class RealAuthException : ApplicationException
    {
        public RealAuthException(string message) : base(message)
        {
        }

        public RealAuthException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}

