namespace System.Web.Services.Protocols
{
    using System;
    using System.Runtime.Serialization;
    using System.Xml;

    [Serializable]
    public class SoapHeaderException : SoapException
    {
        public SoapHeaderException()
        {
        }

        protected SoapHeaderException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public SoapHeaderException(string message, XmlQualifiedName code) : base(message, code)
        {
        }

        public SoapHeaderException(string message, XmlQualifiedName code, Exception innerException) : base(message, code, innerException)
        {
        }

        public SoapHeaderException(string message, XmlQualifiedName code, string actor) : base(message, code, actor)
        {
        }

        public SoapHeaderException(string message, XmlQualifiedName code, string actor, Exception innerException) : base(message, code, actor, innerException)
        {
        }

        public SoapHeaderException(string message, XmlQualifiedName code, string actor, string role, SoapFaultSubCode subCode, Exception innerException) : base(message, code, actor, role, null, null, subCode, innerException)
        {
        }

        public SoapHeaderException(string message, XmlQualifiedName code, string actor, string role, string lang, SoapFaultSubCode subCode, Exception innerException) : base(message, code, actor, role, lang, null, subCode, innerException)
        {
        }
    }
}

