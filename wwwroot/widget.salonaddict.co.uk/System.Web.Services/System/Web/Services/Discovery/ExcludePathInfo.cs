namespace System.Web.Services.Discovery
{
    using System;
    using System.Xml.Serialization;

    public sealed class ExcludePathInfo
    {
        private string path;

        public ExcludePathInfo()
        {
        }

        public ExcludePathInfo(string path)
        {
            this.path = path;
        }

        [XmlAttribute("path")]
        public string Path
        {
            get => 
                this.path;
            set
            {
                this.path = value;
            }
        }
    }
}

