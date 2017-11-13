namespace System.Data
{
    using System;
    using System.Runtime.Serialization;
    using System.Security.Permissions;

    [Serializable]
    public sealed class MetadataException : EntityException
    {
        public MetadataException() : base(Strings.Metadata_General_Error)
        {
            base.HResult = -2146232007;
        }

        public MetadataException(string message) : base(message)
        {
            base.HResult = -2146232007;
        }

        private MetadataException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public MetadataException(string message, Exception innerException) : base(message, innerException)
        {
            base.HResult = -2146232007;
        }

        [SecurityPermission(SecurityAction.Demand, SerializationFormatter=true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
        }
    }
}

