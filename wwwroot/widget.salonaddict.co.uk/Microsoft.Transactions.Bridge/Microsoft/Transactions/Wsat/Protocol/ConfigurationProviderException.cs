namespace Microsoft.Transactions.Wsat.Protocol
{
    using System;

    internal class ConfigurationProviderException : Exception
    {
        public ConfigurationProviderException(string message) : base(message)
        {
        }

        public ConfigurationProviderException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}

