namespace System.Web.Caching
{
    using System;
    using System.Runtime.Serialization;
    using System.Security.Permissions;
    using System.Web;

    [Serializable, AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public sealed class TableNotEnabledForNotificationException : SystemException
    {
        public TableNotEnabledForNotificationException()
        {
        }

        public TableNotEnabledForNotificationException(string message) : base(message)
        {
        }

        internal TableNotEnabledForNotificationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public TableNotEnabledForNotificationException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}

