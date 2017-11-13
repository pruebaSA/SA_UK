namespace System.Web.Services.Discovery
{
    using System;
    using System.Xml.Serialization;

    public sealed class DiscoveryClientResult
    {
        private string filename;
        private string referenceTypeName;
        private string url;

        public DiscoveryClientResult()
        {
        }

        public DiscoveryClientResult(Type referenceType, string url, string filename)
        {
            this.referenceTypeName = (referenceType == null) ? string.Empty : referenceType.FullName;
            this.url = url;
            this.filename = filename;
        }

        [XmlAttribute("filename")]
        public string Filename
        {
            get => 
                this.filename;
            set
            {
                this.filename = value;
            }
        }

        [XmlAttribute("referenceType")]
        public string ReferenceTypeName
        {
            get => 
                this.referenceTypeName;
            set
            {
                this.referenceTypeName = value;
            }
        }

        [XmlAttribute("url")]
        public string Url
        {
            get => 
                this.url;
            set
            {
                this.url = value;
            }
        }
    }
}

