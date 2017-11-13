namespace System.Web.Security
{
    using System;
    using System.Runtime.Serialization;
    using System.Security.Permissions;
    using System.Web;

    [Serializable, AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class MembershipCreateUserException : Exception
    {
        private MembershipCreateStatus _StatusCode;

        public MembershipCreateUserException()
        {
            this._StatusCode = MembershipCreateStatus.ProviderError;
        }

        public MembershipCreateUserException(string message) : base(message)
        {
            this._StatusCode = MembershipCreateStatus.ProviderError;
        }

        public MembershipCreateUserException(MembershipCreateStatus statusCode) : base(GetMessageFromStatusCode(statusCode))
        {
            this._StatusCode = MembershipCreateStatus.ProviderError;
            this._StatusCode = statusCode;
        }

        protected MembershipCreateUserException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            this._StatusCode = MembershipCreateStatus.ProviderError;
            this._StatusCode = (MembershipCreateStatus) info.GetInt32("_StatusCode");
        }

        public MembershipCreateUserException(string message, Exception innerException) : base(message, innerException)
        {
            this._StatusCode = MembershipCreateStatus.ProviderError;
        }

        internal static string GetMessageFromStatusCode(MembershipCreateStatus statusCode)
        {
            string name = "Provider_Error";
            switch (statusCode)
            {
                case MembershipCreateStatus.Success:
                    name = "Membership_No_error";
                    break;

                case MembershipCreateStatus.InvalidUserName:
                    name = "Membership_InvalidUserName";
                    break;

                case MembershipCreateStatus.InvalidPassword:
                    name = "Membership_InvalidPassword";
                    break;

                case MembershipCreateStatus.InvalidQuestion:
                    name = "Membership_InvalidQuestion";
                    break;

                case MembershipCreateStatus.InvalidAnswer:
                    name = "Membership_InvalidAnswer";
                    break;

                case MembershipCreateStatus.InvalidEmail:
                    name = "Membership_InvalidEmail";
                    break;

                case MembershipCreateStatus.DuplicateUserName:
                    name = "Membership_DuplicateUserName";
                    break;

                case MembershipCreateStatus.DuplicateEmail:
                    name = "Membership_DuplicateEmail";
                    break;

                case MembershipCreateStatus.UserRejected:
                    name = "Membership_UserRejected";
                    break;

                case MembershipCreateStatus.InvalidProviderUserKey:
                    name = "Membership_InvalidProviderUserKey";
                    break;

                case MembershipCreateStatus.DuplicateProviderUserKey:
                    name = "Membership_DuplicateProviderUserKey";
                    break;
            }
            return System.Web.SR.GetString(name);
        }

        [SecurityPermission(SecurityAction.Demand, Flags=SecurityPermissionFlag.SerializationFormatter, SerializationFormatter=true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("_StatusCode", this._StatusCode);
        }

        public MembershipCreateStatus StatusCode =>
            this._StatusCode;
    }
}

