namespace System.Web.Services.Description
{
    using System;
    using System.ComponentModel;
    using System.Xml.Serialization;

    public abstract class ServiceDescriptionFormatExtension
    {
        private bool handled;
        private object parent;
        private bool required;

        protected ServiceDescriptionFormatExtension()
        {
        }

        internal void SetParent(object parent)
        {
            this.parent = parent;
        }

        [XmlIgnore]
        public bool Handled
        {
            get => 
                this.handled;
            set
            {
                this.handled = value;
            }
        }

        public object Parent =>
            this.parent;

        [DefaultValue(false), XmlAttribute("required", Namespace="http://schemas.xmlsoap.org/wsdl/")]
        public bool Required
        {
            get => 
                this.required;
            set
            {
                this.required = value;
            }
        }
    }
}

