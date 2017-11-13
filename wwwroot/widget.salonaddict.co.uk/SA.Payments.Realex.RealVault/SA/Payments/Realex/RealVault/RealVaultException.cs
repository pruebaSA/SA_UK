namespace SA.Payments.Realex.RealVault
{
    using System;

    public class RealVaultException : ApplicationException
    {
        public RealVaultException(string message) : base(message)
        {
        }

        public RealVaultException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}

