namespace System.Security.RightsManagement
{
    using System;
    using System.Runtime.Serialization;
    using System.Security;
    using System.Security.Permissions;

    [Serializable]
    public class RightsManagementException : Exception
    {
        private RightsManagementFailureCode _failureCode;
        private const string _serializationFailureCodeAttributeName = "FailureCode";

        public RightsManagementException() : base(SR.Get("RmExceptionGenericMessage"))
        {
        }

        public RightsManagementException(RightsManagementFailureCode failureCode) : base(Errors.GetLocalizedFailureCodeMessageWithDefault(failureCode))
        {
            this._failureCode = failureCode;
        }

        public RightsManagementException(string message) : base(message)
        {
        }

        protected RightsManagementException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            this._failureCode = (RightsManagementFailureCode) info.GetInt32("FailureCode");
        }

        public RightsManagementException(RightsManagementFailureCode failureCode, Exception innerException) : base(Errors.GetLocalizedFailureCodeMessageWithDefault(failureCode), innerException)
        {
            this._failureCode = failureCode;
        }

        public RightsManagementException(RightsManagementFailureCode failureCode, string message) : base(message)
        {
            this._failureCode = failureCode;
        }

        public RightsManagementException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public RightsManagementException(RightsManagementFailureCode failureCode, string message, Exception innerException) : base(message, innerException)
        {
            this._failureCode = failureCode;
        }

        [SecurityCritical, SecurityPermission(SecurityAction.Demand, Flags=SecurityPermissionFlag.SerializationFormatter)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
            {
                throw new ArgumentNullException("info");
            }
            base.GetObjectData(info, context);
            info.AddValue("FailureCode", (int) this._failureCode);
        }

        public RightsManagementFailureCode FailureCode =>
            this._failureCode;
    }
}

