namespace System.Web.Services.Description
{
    using System;
    using System.Xml.Serialization;

    public abstract class NamedItem : DocumentableItem
    {
        private string name;

        protected NamedItem()
        {
        }

        [XmlAttribute("name")]
        public string Name
        {
            get => 
                this.name;
            set
            {
                this.name = value;
            }
        }
    }
}

