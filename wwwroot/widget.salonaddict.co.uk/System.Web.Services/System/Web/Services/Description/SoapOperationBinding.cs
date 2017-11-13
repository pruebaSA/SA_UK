namespace System.Web.Services.Description
{
    using System;
    using System.ComponentModel;
    using System.Web.Services.Configuration;
    using System.Xml.Serialization;

    [XmlFormatExtension("operation", "http://schemas.xmlsoap.org/wsdl/soap/", typeof(OperationBinding))]
    public class SoapOperationBinding : ServiceDescriptionFormatExtension
    {
        private string soapAction;
        private SoapBindingStyle style;

        [XmlAttribute("soapAction")]
        public string SoapAction
        {
            get
            {
                if (this.soapAction != null)
                {
                    return this.soapAction;
                }
                return string.Empty;
            }
            set
            {
                this.soapAction = value;
            }
        }

        [DefaultValue(0), XmlAttribute("style")]
        public SoapBindingStyle Style
        {
            get => 
                this.style;
            set
            {
                this.style = value;
            }
        }
    }
}

