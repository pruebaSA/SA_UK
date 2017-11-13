namespace System.Xml.Schema
{
    using System;
    using System.Xml.Serialization;

    public class XmlSchemaRedefine : XmlSchemaExternal
    {
        private XmlSchemaObjectTable attributeGroups = new XmlSchemaObjectTable();
        private XmlSchemaObjectTable groups = new XmlSchemaObjectTable();
        private XmlSchemaObjectCollection items = new XmlSchemaObjectCollection();
        private XmlSchemaObjectTable types = new XmlSchemaObjectTable();

        public XmlSchemaRedefine()
        {
            base.Compositor = System.Xml.Schema.Compositor.Redefine;
        }

        internal override void AddAnnotation(XmlSchemaAnnotation annotation)
        {
            this.items.Add(annotation);
        }

        [XmlIgnore]
        public XmlSchemaObjectTable AttributeGroups =>
            this.attributeGroups;

        [XmlIgnore]
        public XmlSchemaObjectTable Groups =>
            this.groups;

        [XmlElement("annotation", typeof(XmlSchemaAnnotation)), XmlElement("complexType", typeof(XmlSchemaComplexType)), XmlElement("attributeGroup", typeof(XmlSchemaAttributeGroup)), XmlElement("simpleType", typeof(XmlSchemaSimpleType)), XmlElement("group", typeof(XmlSchemaGroup))]
        public XmlSchemaObjectCollection Items =>
            this.items;

        [XmlIgnore]
        public XmlSchemaObjectTable SchemaTypes =>
            this.types;
    }
}

