namespace System.Xml.Schema
{
    using System;
    using System.Xml;
    using System.Xml.Serialization;

    public class XmlSchemaAnnotated : XmlSchemaObject
    {
        private XmlSchemaAnnotation annotation;
        private string id;
        private XmlAttribute[] moreAttributes;

        internal override void AddAnnotation(XmlSchemaAnnotation annotation)
        {
            this.annotation = annotation;
        }

        internal override void SetUnhandledAttributes(XmlAttribute[] moreAttributes)
        {
            this.moreAttributes = moreAttributes;
        }

        [XmlElement("annotation", typeof(XmlSchemaAnnotation))]
        public XmlSchemaAnnotation Annotation
        {
            get => 
                this.annotation;
            set
            {
                this.annotation = value;
            }
        }

        [XmlAttribute("id", DataType="ID")]
        public string Id
        {
            get => 
                this.id;
            set
            {
                this.id = value;
            }
        }

        [XmlIgnore]
        internal override string IdAttribute
        {
            get => 
                this.Id;
            set
            {
                this.Id = value;
            }
        }

        [XmlAnyAttribute]
        public XmlAttribute[] UnhandledAttributes
        {
            get => 
                this.moreAttributes;
            set
            {
                this.moreAttributes = value;
            }
        }
    }
}

