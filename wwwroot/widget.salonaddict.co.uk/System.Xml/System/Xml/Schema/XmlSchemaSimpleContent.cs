namespace System.Xml.Schema
{
    using System;
    using System.Xml.Serialization;

    public class XmlSchemaSimpleContent : XmlSchemaContentModel
    {
        private XmlSchemaContent content;

        [XmlElement("extension", typeof(XmlSchemaSimpleContentExtension)), XmlElement("restriction", typeof(XmlSchemaSimpleContentRestriction))]
        public override XmlSchemaContent Content
        {
            get => 
                this.content;
            set
            {
                this.content = value;
            }
        }
    }
}

