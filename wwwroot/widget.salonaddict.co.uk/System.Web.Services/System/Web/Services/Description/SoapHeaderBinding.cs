namespace System.Web.Services.Description
{
    using System;
    using System.ComponentModel;
    using System.Web.Services.Configuration;
    using System.Xml;
    using System.Xml.Serialization;

    [XmlFormatExtension("header", "http://schemas.xmlsoap.org/wsdl/soap/", typeof(InputBinding), typeof(OutputBinding))]
    public class SoapHeaderBinding : ServiceDescriptionFormatExtension
    {
        private string encoding;
        private SoapHeaderFaultBinding fault;
        private bool mapToProperty;
        private XmlQualifiedName message = XmlQualifiedName.Empty;
        private string ns;
        private string part;
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

        [XmlElement("headerfault")]
        public SoapHeaderFaultBinding Fault
        {
            get => 
                this.fault;
            set
            {
                this.fault = value;
            }
        }

        [XmlIgnore]
        public bool MapToProperty
        {
            get => 
                this.mapToProperty;
            set
            {
                this.mapToProperty = value;
            }
        }

        [XmlAttribute("message")]
        public XmlQualifiedName Message
        {
            get => 
                this.message;
            set
            {
                this.message = value;
            }
        }

        [XmlAttribute("namespace"), DefaultValue("")]
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

        [XmlAttribute("part")]
        public string Part
        {
            get => 
                this.part;
            set
            {
                this.part = value;
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

