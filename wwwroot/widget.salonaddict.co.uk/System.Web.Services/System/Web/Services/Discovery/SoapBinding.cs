namespace System.Web.Services.Discovery
{
    using System;
    using System.Xml;
    using System.Xml.Serialization;

    [XmlRoot("soap", Namespace="http://schemas.xmlsoap.org/disco/soap/")]
    public sealed class SoapBinding
    {
        private string address = "";
        private XmlQualifiedName binding;
        public const string Namespace = "http://schemas.xmlsoap.org/disco/soap/";

        [XmlAttribute("address")]
        public string Address
        {
            get => 
                this.address;
            set
            {
                if (value == null)
                {
                    this.address = "";
                }
                else
                {
                    this.address = value;
                }
            }
        }

        [XmlAttribute("binding")]
        public XmlQualifiedName Binding
        {
            get => 
                this.binding;
            set
            {
                this.binding = value;
            }
        }
    }
}

