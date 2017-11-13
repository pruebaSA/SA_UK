namespace System.Web.Services.Description
{
    using System;
    using System.Web.Services.Configuration;
    using System.Xml.Serialization;

    [XmlFormatExtension("mimeXml", "http://schemas.xmlsoap.org/wsdl/mime/", typeof(MimePart), typeof(InputBinding), typeof(OutputBinding))]
    public sealed class MimeXmlBinding : ServiceDescriptionFormatExtension
    {
        private string part;

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
    }
}

