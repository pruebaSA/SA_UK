namespace System.Web.Services.Description
{
    using System.Web.Services.Configuration;
    using System.Xml.Serialization;

    [XmlFormatExtension("multipartRelated", "http://schemas.xmlsoap.org/wsdl/mime/", typeof(InputBinding), typeof(OutputBinding))]
    public sealed class MimeMultipartRelatedBinding : ServiceDescriptionFormatExtension
    {
        private MimePartCollection parts = new MimePartCollection();

        [XmlElement("part")]
        public MimePartCollection Parts =>
            this.parts;
    }
}

