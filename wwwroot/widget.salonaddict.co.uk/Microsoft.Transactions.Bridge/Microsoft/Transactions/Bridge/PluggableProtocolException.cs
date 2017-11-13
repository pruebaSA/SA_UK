namespace Microsoft.Transactions.Bridge
{
    using System;
    using System.Runtime.Serialization;

    [Serializable]
    internal class PluggableProtocolException : Exception
    {
        public PluggableProtocolException()
        {
        }

        public PluggableProtocolException(string exception) : base(exception)
        {
        }

        protected PluggableProtocolException(SerializationInfo serInfo, StreamingContext streaming) : base(serInfo, streaming)
        {
        }

        public PluggableProtocolException(string exception, Exception e) : base(exception, e)
        {
        }
    }
}

