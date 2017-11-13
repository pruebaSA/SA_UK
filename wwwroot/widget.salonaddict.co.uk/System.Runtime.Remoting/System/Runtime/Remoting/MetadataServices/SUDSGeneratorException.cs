namespace System.Runtime.Remoting.MetadataServices
{
    using System;
    using System.Runtime.Serialization;

    [Serializable]
    public class SUDSGeneratorException : Exception
    {
        internal SUDSGeneratorException(string msg) : base(msg)
        {
        }

        protected SUDSGeneratorException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}

