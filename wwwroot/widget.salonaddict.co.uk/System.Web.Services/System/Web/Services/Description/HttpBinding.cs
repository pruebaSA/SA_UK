namespace System.Web.Services.Description
{
    using System;
    using System.Web.Services.Configuration;
    using System.Xml.Serialization;

    [XmlFormatExtensionPrefix("http", "http://schemas.xmlsoap.org/wsdl/http/"), XmlFormatExtension("binding", "http://schemas.xmlsoap.org/wsdl/http/", typeof(Binding))]
    public sealed class HttpBinding : ServiceDescriptionFormatExtension
    {
        public const string Namespace = "http://schemas.xmlsoap.org/wsdl/http/";
        private string verb;

        [XmlAttribute("verb")]
        public string Verb
        {
            get => 
                this.verb;
            set
            {
                this.verb = value;
            }
        }
    }
}

