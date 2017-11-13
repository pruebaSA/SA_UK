namespace System.Xml.Schema
{
    using System;
    using System.ComponentModel;
    using System.Xml;
    using System.Xml.Serialization;

    public class XmlSchemaAttribute : XmlSchemaAnnotated
    {
        private SchemaAttDef attDef;
        private XmlSchemaSimpleType attributeType;
        private string defaultValue;
        private string fixedValue;
        private XmlSchemaForm form;
        private string name;
        private string prefix;
        private XmlQualifiedName qualifiedName = XmlQualifiedName.Empty;
        private XmlQualifiedName refName = XmlQualifiedName.Empty;
        private XmlSchemaSimpleType type;
        private XmlQualifiedName typeName = XmlQualifiedName.Empty;
        private XmlSchemaUse use;

        internal override XmlSchemaObject Clone()
        {
            XmlSchemaAttribute attribute = (XmlSchemaAttribute) base.MemberwiseClone();
            attribute.refName = this.refName.Clone();
            attribute.typeName = this.typeName.Clone();
            return attribute;
        }

        internal void SetAttributeType(XmlSchemaSimpleType value)
        {
            this.attributeType = value;
        }

        internal void SetQualifiedName(XmlQualifiedName value)
        {
            this.qualifiedName = value;
        }

        internal XmlReader Validate(XmlReader reader, XmlResolver resolver, XmlSchemaSet schemaSet, ValidationEventHandler valEventHandler)
        {
            if (schemaSet != null)
            {
                XmlReaderSettings readerSettings = new XmlReaderSettings {
                    ValidationType = ValidationType.Schema,
                    Schemas = schemaSet
                };
                readerSettings.ValidationEventHandler += valEventHandler;
                return new XsdValidatingReader(reader, resolver, readerSettings, this);
            }
            return null;
        }

        internal SchemaAttDef AttDef
        {
            get => 
                this.attDef;
            set
            {
                this.attDef = value;
            }
        }

        [XmlIgnore]
        public XmlSchemaSimpleType AttributeSchemaType =>
            this.attributeType;

        [XmlIgnore, Obsolete("This property has been deprecated. Please use AttributeSchemaType property that returns a strongly typed attribute type. http://go.microsoft.com/fwlink/?linkid=14202")]
        public object AttributeType
        {
            get
            {
                if (this.attributeType.QualifiedName.Namespace == "http://www.w3.org/2001/XMLSchema")
                {
                    return this.attributeType.Datatype;
                }
                return this.attributeType;
            }
        }

        [XmlIgnore]
        internal XmlSchemaDatatype Datatype
        {
            get
            {
                if (this.attributeType != null)
                {
                    return this.attributeType.Datatype;
                }
                return null;
            }
        }

        [XmlAttribute("default"), DefaultValue((string) null)]
        public string DefaultValue
        {
            get => 
                this.defaultValue;
            set
            {
                this.defaultValue = value;
            }
        }

        [XmlAttribute("fixed"), DefaultValue((string) null)]
        public string FixedValue
        {
            get => 
                this.fixedValue;
            set
            {
                this.fixedValue = value;
            }
        }

        [DefaultValue(0), XmlAttribute("form")]
        public XmlSchemaForm Form
        {
            get => 
                this.form;
            set
            {
                this.form = value;
            }
        }

        internal bool HasDefault =>
            (this.defaultValue != null);

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

        internal string Prefix =>
            this.prefix;

        [XmlIgnore]
        public XmlQualifiedName QualifiedName =>
            this.qualifiedName;

        [XmlAttribute("ref")]
        public XmlQualifiedName RefName
        {
            get => 
                this.refName;
            set
            {
                this.refName = (value == null) ? XmlQualifiedName.Empty : value;
            }
        }

        [XmlElement("simpleType")]
        public XmlSchemaSimpleType SchemaType
        {
            get => 
                this.type;
            set
            {
                this.type = value;
            }
        }

        [XmlAttribute("type")]
        public XmlQualifiedName SchemaTypeName
        {
            get => 
                this.typeName;
            set
            {
                this.typeName = (value == null) ? XmlQualifiedName.Empty : value;
            }
        }

        [XmlAttribute("use"), DefaultValue(0)]
        public XmlSchemaUse Use
        {
            get => 
                this.use;
            set
            {
                this.use = value;
            }
        }
    }
}

