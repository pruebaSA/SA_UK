namespace System.Web.Services.Description
{
    using System;
    using System.Web.Services.Configuration;
    using System.Xml.Serialization;

    [XmlFormatExtension("operation", "http://schemas.xmlsoap.org/wsdl/http/", typeof(OperationBinding))]
    public sealed class HttpOperationBinding : ServiceDescriptionFormatExtension
    {
        private string location;

        [XmlAttribute("location")]
        public string Location
        {
            get
            {
                if (this.location != null)
                {
                    return this.location;
                }
                return string.Empty;
            }
            set
            {
                this.location = value;
            }
        }
    }
}

