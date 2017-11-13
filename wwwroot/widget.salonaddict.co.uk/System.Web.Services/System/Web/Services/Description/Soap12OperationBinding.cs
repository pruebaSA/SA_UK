namespace System.Web.Services.Description
{
    using System;
    using System.ComponentModel;
    using System.Web.Services.Configuration;
    using System.Web.Services.Protocols;
    using System.Xml.Serialization;

    [XmlFormatExtension("operation", "http://schemas.xmlsoap.org/wsdl/soap12/", typeof(OperationBinding))]
    public sealed class Soap12OperationBinding : SoapOperationBinding
    {
        private Soap12OperationBinding duplicateByRequestElement;
        private Soap12OperationBinding duplicateBySoapAction;
        private SoapReflectedMethod method;
        private bool soapActionRequired;

        internal Soap12OperationBinding DuplicateByRequestElement
        {
            get => 
                this.duplicateByRequestElement;
            set
            {
                this.duplicateByRequestElement = value;
            }
        }

        internal Soap12OperationBinding DuplicateBySoapAction
        {
            get => 
                this.duplicateBySoapAction;
            set
            {
                this.duplicateBySoapAction = value;
            }
        }

        internal SoapReflectedMethod Method
        {
            get => 
                this.method;
            set
            {
                this.method = value;
            }
        }

        [DefaultValue(false), XmlAttribute("soapActionRequired")]
        public bool SoapActionRequired
        {
            get => 
                this.soapActionRequired;
            set
            {
                this.soapActionRequired = value;
            }
        }
    }
}

