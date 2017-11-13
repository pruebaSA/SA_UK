namespace System.Web.Services.Description
{
    using System;
    using System.Web.Services.Configuration;
    using System.Xml.Serialization;

    [XmlFormatExtension("content", "http://schemas.xmlsoap.org/wsdl/mime/", typeof(MimePart), typeof(InputBinding), typeof(OutputBinding)), XmlFormatExtensionPrefix("mime", "http://schemas.xmlsoap.org/wsdl/mime/")]
    public sealed class MimeContentBinding : ServiceDescriptionFormatExtension
    {
        public const string Namespace = "http://schemas.xmlsoap.org/wsdl/mime/";
        private string part;
        private string type;

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

        [XmlAttribute("type")]
        public string Type
        {
            get
            {
                if (this.type != null)
                {
                    return this.type;
                }
                return string.Empty;
            }
            set
            {
                this.type = value;
            }
        }
    }
}

