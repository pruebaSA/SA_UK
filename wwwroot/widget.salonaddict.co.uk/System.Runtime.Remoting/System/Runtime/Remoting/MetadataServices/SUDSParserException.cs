namespace System.Runtime.Remoting.MetadataServices
{
    using System;
    using System.Runtime.Serialization;

    [Serializable]
    public class SUDSParserException : Exception
    {
        internal SUDSParserException(string message) : base(message)
        {
        }

        protected SUDSParserException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}

