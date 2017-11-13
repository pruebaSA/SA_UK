namespace System.ServiceModel.Security
{
    using System;
    using System.Runtime.Serialization;
    using System.Security;
    using System.Security.Permissions;

    [Serializable]
    public class ExpiredSecurityTokenException : MessageSecurityException
    {
        public ExpiredSecurityTokenException()
        {
        }

        public ExpiredSecurityTokenException(string message) : base(message)
        {
        }

        protected ExpiredSecurityTokenException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public ExpiredSecurityTokenException(string message, Exception innerException) : base(message, innerException)
        {
        }

        [SecurityCritical, SecurityPermission(SecurityAction.LinkDemand, SerializationFormatter=true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
        }
    }
}

