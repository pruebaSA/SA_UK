namespace System.Xml.Schema
{
    using System;
    using System.Xml;
    using System.Xml.Serialization;

    public class XmlSchemaIdentityConstraint : XmlSchemaAnnotated
    {
        private CompiledIdentityConstraint compiledConstraint;
        private XmlSchemaObjectCollection fields = new XmlSchemaObjectCollection();
        private string name;
        private XmlQualifiedName qualifiedName = XmlQualifiedName.Empty;
        private XmlSchemaXPath selector;

        internal void SetQualifiedName(XmlQualifiedName value)
        {
            this.qualifiedName = value;
        }

        [XmlIgnore]
        internal CompiledIdentityConstraint CompiledConstraint
        {
            get => 
                this.compiledConstraint;
            set
            {
                this.compiledConstraint = value;
            }
        }

        [XmlElement("field", typeof(XmlSchemaXPath))]
        public XmlSchemaObjectCollection Fields =>
            this.fields;

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

        [XmlIgnore]
        internal override string NameAttribute
        {
            get => 
                this.Name;
            set
            {
                this.Name = value;
            }
        }

        [XmlIgnore]
        public XmlQualifiedName QualifiedName =>
            this.qualifiedName;

        [XmlElement("selector", typeof(XmlSchemaXPath))]
        public XmlSchemaXPath Selector
        {
            get => 
                this.selector;
            set
            {
                this.selector = value;
            }
        }
    }
}

