namespace System.Web.Services.Description
{
    using System;
    using System.ComponentModel;
    using System.Xml;
    using System.Xml.Serialization;

    public class SoapHeaderFaultBinding : ServiceDescriptionFormatExtension
    {
        private string encoding;
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

        [DefaultValue(""), XmlAttribute("namespace")]
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

