namespace System.Data
{
    using System;
    using System.Runtime.Serialization;

    [Serializable]
    public sealed class MappingException : EntityException
    {
        public MappingException() : base(Strings.Mapping_General_Error_0)
        {
        }

        public MappingException(string message) : base(message)
        {
        }

        private MappingException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public MappingException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}

