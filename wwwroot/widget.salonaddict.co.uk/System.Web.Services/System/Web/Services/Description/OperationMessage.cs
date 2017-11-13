namespace System.Web.Services.Description
{
    using System;
    using System.Xml;
    using System.Xml.Serialization;

    public abstract class OperationMessage : NamedItem
    {
        private XmlQualifiedName message = XmlQualifiedName.Empty;
        private System.Web.Services.Description.Operation parent;

        protected OperationMessage()
        {
        }

        internal void SetParent(System.Web.Services.Description.Operation parent)
        {
            this.parent = parent;
        }

        [XmlAttribute("message")]
        public XmlQualifiedName Message
        {
            get => 
                this.message;
            set
            {
                this.message = value;
            }
        }

        public System.Web.Services.Description.Operation Operation =>
            this.parent;
    }
}

