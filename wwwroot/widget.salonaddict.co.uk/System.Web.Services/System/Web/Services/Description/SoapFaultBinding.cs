namespace System.Web.Services.Description
{
    using System;
    using System.ComponentModel;
    using System.Web.Services.Configuration;
    using System.Xml.Serialization;

    [XmlFormatExtension("fault", "http://schemas.xmlsoap.org/wsdl/soap/", typeof(FaultBinding))]
    public class SoapFaultBinding : ServiceDescriptionFormatExtension
    {
        private string encoding;
        private string name;
        private string ns;
        private SoapBindingUse use;

        [DefaultValue(""), XmlAttribute("encodingStyle")]
        public string Encoding
        {
            get
            {
                if (this.encoding != null)
                {
                    return this.encoding;
                }
                return string.Empty;
            }
            set
            {
                this.encoding = value;
            }
        }

        [XmlAttribute("name")]
        public string Name
        {
            get => 
                this.name;
            set
            {
                this.name = value;
            }
        }

        [XmlAttribute("namespace")]
        public string Namespace
        {
            get
            {
                if (this.ns != null)
                {
                    return this.ns;
                }
                return string.Empty;
            }
            set
            {
                this.ns = value;
            }
        }

        [XmlAttribute("use"), DefaultValue(0)]
        public SoapBindingUse Use
        {
            get => 
                this.use;
            set
            {
                this.use = value;
            }
        }
    }
}

