namespace System.Web.Services.Protocols
{
    using System;
    using System.Xml;

    [Serializable]
    public class SoapFaultSubCode
    {
        private XmlQualifiedName code;
        private SoapFaultSubCode subCode;

        public SoapFaultSubCode(XmlQualifiedName code) : this(code, null)
        {
        }

        public SoapFaultSubCode(XmlQualifiedName code, SoapFaultSubCode subCode)
        {
            this.code = code;
            this.subCode = subCode;
        }

        public XmlQualifiedName Code =>
            this.code;

        public SoapFaultSubCode SubCode =>
            this.subCode;
    }
}

