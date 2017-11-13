namespace System.Data.Services.Client
{
    using System;
    using System.Diagnostics;
    using System.Runtime.Serialization;
    using System.Security;
    using System.Security.Permissions;

    [Serializable, DebuggerDisplay("{Message}")]
    public sealed class DataServiceClientException : InvalidOperationException
    {
        private readonly int statusCode;

        public DataServiceClientException() : this(Strings.DataServiceException_GeneralError)
        {
        }

        public DataServiceClientException(string message) : this(message, (Exception) null)
        {
        }

        protected DataServiceClientException(SerializationInfo serializationInfo, StreamingContext context) : base(serializationInfo, context)
        {
            if (serializationInfo != null)
            {
                this.statusCode = serializationInfo.GetInt32("statusCode");
            }
        }

        public DataServiceClientException(string message, Exception innerException) : this(message, innerException, 500)
        {
        }

        public DataServiceClientException(string message, int statusCode) : this(message, null, statusCode)
        {
        }

        public DataServiceClientException(string message, Exception innerException, int statusCode) : base(message, innerException)
        {
            this.statusCode = statusCode;
        }

        [SecurityCritical, SecurityPermission(SecurityAction.Demand, SerializationFormatter=true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info != null)
            {
                info.AddValue("statusCode", this.statusCode);
            }
            base.GetObjectData(info, context);
        }

        public int StatusCode =>
            this.statusCode;
    }
}

