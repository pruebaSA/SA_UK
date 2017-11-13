namespace System.Web.Security
{
    using System;
    using System.Runtime.Serialization;
    using System.Security.Permissions;
    using System.Web;

    [Serializable, AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class MembershipPasswordException : Exception
    {
        public MembershipPasswordException()
        {
        }

        public MembershipPasswordException(string message) : base(message)
        {
        }

        protected MembershipPasswordException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public MembershipPasswordException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}

