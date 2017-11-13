namespace Microsoft.Transactions.Wsat.Protocol
{
    using System;

    internal abstract class ConfigurationProvider : IDisposable
    {
        protected ConfigurationProvider()
        {
        }

        public abstract void Dispose();
        public abstract ConfigurationProvider OpenKey(string key);
        public abstract int ReadInteger(string value, int defaultValue);
        public abstract string[] ReadMultiString(string value, string[] defaultValue);
        public abstract string ReadString(string value, string defaultValue);
    }
}

