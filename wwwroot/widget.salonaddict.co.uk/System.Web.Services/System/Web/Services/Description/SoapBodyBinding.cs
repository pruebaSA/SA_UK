﻿namespace System.Web.Services.Description
{
    using System;
    using System.ComponentModel;
    using System.Text;
    using System.Web.Services.Configuration;
    using System.Xml.Serialization;

    [XmlFormatExtension("body", "http://schemas.xmlsoap.org/wsdl/soap/", typeof(InputBinding), typeof(OutputBinding), typeof(MimePart))]
    public class SoapBodyBinding : ServiceDescriptionFormatExtension
    {
        private string encoding;
        private string ns;
        private string[] parts;
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

        [XmlIgnore]
        public string[] Parts
        {
            get => 
                this.parts;
            set
            {
                this.parts = value;
            }
        }

        [XmlAttribute("parts")]
        public string PartsString
        {
            get
            {
                if (this.parts == null)
                {
                    return null;
                }
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < this.parts.Length; i++)
                {
                    if (i > 0)
                    {
                        builder.Append(' ');
                    }
                    builder.Append(this.parts[i]);
                }
                return builder.ToString();
            }
            set
            {
                if (value == null)
                {
                    this.parts = null;
                }
                else
                {
                    this.parts = value.Split(new char[] { ' ' });
                }
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

