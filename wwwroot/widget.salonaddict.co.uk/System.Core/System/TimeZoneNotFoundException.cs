namespace System
{
    using System.Runtime.Serialization;
    using System.Security.Permissions;

    [Serializable, HostProtection(SecurityAction.LinkDemand, MayLeakOnAbort=true)]
    public class TimeZoneNotFoundException : Exception
    {
        public TimeZoneNotFoundException()
        {
        }

        public TimeZoneNotFoundException(string message) : base(message)
        {
        }

        protected TimeZoneNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public TimeZoneNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}

