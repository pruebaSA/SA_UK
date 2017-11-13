namespace System.Web.UI.WebControls
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using System.Security.Permissions;
    using System.Web;
    using System.Web.DynamicData;

    [Serializable, AspNetHostingPermission(SecurityAction.LinkDemand, Level=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level=AspNetHostingPermissionLevel.Minimal)]
    public class LinqDataSourceValidationException : Exception, IDynamicValidatorException, ISerializable
    {
        private IDictionary<string, Exception> _innerExceptions;

        public LinqDataSourceValidationException() : base(AtlasWeb.LinqDataSourceValidationException_ValidationFailed)
        {
        }

        public LinqDataSourceValidationException(string message) : base(message)
        {
        }

        protected LinqDataSourceValidationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            this._innerExceptions = (IDictionary<string, Exception>) info.GetValue("InnerExceptions", typeof(IDictionary<string, Exception>));
        }

        public LinqDataSourceValidationException(string message, IDictionary<string, Exception> innerExceptions) : this(message)
        {
            this._innerExceptions = innerExceptions;
        }

        public LinqDataSourceValidationException(string message, Exception innerException) : base(message, innerException)
        {
        }

        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.SerializationFormatter)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("InnerExceptions", this.InnerExceptions, typeof(IDictionary<string, Exception>));
        }

        public IDictionary<string, Exception> InnerExceptions
        {
            get
            {
                if (this._innerExceptions == null)
                {
                    this._innerExceptions = new Dictionary<string, Exception>(StringComparer.OrdinalIgnoreCase);
                }
                return this._innerExceptions;
            }
        }
    }
}

