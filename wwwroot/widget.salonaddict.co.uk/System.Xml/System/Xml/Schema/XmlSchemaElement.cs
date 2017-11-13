namespace System.Xml.Schema
{
    using System;
    using System.ComponentModel;
    using System.Xml;
    using System.Xml.Serialization;

    public class XmlSchemaElement : XmlSchemaParticle
    {
        private XmlSchemaDerivationMethod block = XmlSchemaDerivationMethod.None;
        private XmlSchemaDerivationMethod blockResolved;
        private XmlSchemaObjectCollection constraints;
        private string defaultValue;
        private SchemaElementDecl elementDecl;
        private XmlSchemaType elementType;
        private XmlSchemaDerivationMethod final = XmlSchemaDerivationMethod.None;
        private XmlSchemaDerivationMethod finalResolved;
        private string fixedValue;
        private XmlSchemaForm form;
        private bool hasAbstractAttribute;
        private bool hasNillableAttribute;
        private bool isAbstract;
        private bool isLocalTypeDerivationChecked;
        private bool isNillable;
        private string name;
        private XmlQualifiedName qualifiedName = XmlQualifiedName.Empty;
        private XmlQualifiedName refName = XmlQualifiedName.Empty;
        private XmlQualifiedName substitutionGroup = XmlQualifiedName.Empty;
        private XmlSchemaType type;
        private XmlQualifiedName typeName = XmlQualifiedName.Empty;

        internal override XmlSchemaObject Clone()
        {
            XmlSchemaElement element = (XmlSchemaElement) base.MemberwiseClone();
            element.refName = this.refName.Clone();
            element.substitutionGroup = this.substitutionGroup.Clone();
            element.typeName = this.typeName.Clone();
            element.constraints = null;
            return element;
        }

        internal void SetBlockResolved(XmlSchemaDerivationMethod value)
        {
            this.blockResolved = value;
        }

        internal void SetElementType(XmlSchemaType value)
        {
            this.elementType = value;
        }

        internal void SetFinalResolved(XmlSchemaDerivationMethod value)
        {
            this.finalResolved = value;
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

        [DefaultValue(0x100), XmlAttribute("block")]
        public XmlSchemaDerivationMethod Block
        {
            get => 
                this.block;
            set
            {
                this.block = value;
            }
        }

        [XmlIgnore]
        public XmlSchemaDerivationMethod BlockResolved =>
            this.blockResolved;

        [XmlElement("key", typeof(XmlSchemaKey)), XmlElement("keyref", typeof(XmlSchemaKeyref)), XmlElement("unique", typeof(XmlSchemaUnique))]
        public XmlSchemaObjectCollection Constraints
        {
            get
            {
                if (this.constraints == null)
                {
                    this.constraints = new XmlSchemaObjectCollection();
                }
                return this.constraints;
            }
        }

        [DefaultValue((string) null), XmlAttribute("default")]
        public string DefaultValue
        {
            get => 
                this.defaultValue;
            set
            {
                this.defaultValue = value;
            }
        }

        internal SchemaElementDecl ElementDecl
        {
            get => 
                this.elementDecl;
            set
            {
                this.elementDecl = value;
            }
        }

        [XmlIgnore]
        public XmlSchemaType ElementSchemaType =>
            this.elementType;

        [Obsolete("This property has been deprecated. Please use ElementSchemaType property that returns a strongly typed element type. http://go.microsoft.com/fwlink/?linkid=14202"), XmlIgnore]
        public object ElementType
        {
            get
            {
                if (this.elementType.QualifiedName.Namespace == "http://www.w3.org/2001/XMLSchema")
                {
                    return this.elementType.Datatype;
                }
                return this.elementType;
            }
        }

        [DefaultValue(0x100), XmlAttribute("final")]
        public XmlSchemaDerivationMethod Final
        {
            get => 
                this.final;
            set
            {
                this.final = value;
            }
        }

        [XmlIgnore]
        public XmlSchemaDerivationMethod FinalResolved =>
            this.finalResolved;

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

        [XmlIgnore]
        internal bool HasAbstractAttribute =>
            this.hasAbstractAttribute;

        internal bool HasConstraints =>
            ((this.constraints != null) && (this.constraints.Count > 0));

        [XmlIgnore]
        internal bool HasDefault =>
            ((this.defaultValue != null) && (this.defaultValue.Length > 0));

        [XmlIgnore]
        internal bool HasNillableAttribute =>
            this.hasNillableAttribute;

        [XmlAttribute("abstract"), DefaultValue(false)]
        public bool IsAbstract
        {
            get => 
                this.isAbstract;
            set
            {
                this.isAbstract = value;
                this.hasAbstractAttribute = true;
            }
        }

        internal bool IsLocalTypeDerivationChecked
        {
            get => 
                this.isLocalTypeDerivationChecked;
            set
            {
                this.isLocalTypeDerivationChecked = value;
            }
        }

        [DefaultValue(false), XmlAttribute("nillable")]
        public bool IsNillable
        {
            get => 
                this.isNillable;
            set
            {
                this.isNillable = value;
                this.hasNillableAttribute = true;
            }
        }

        [XmlAttribute("name"), DefaultValue("")]
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
        internal override string NameString =>
            this.qualifiedName.ToString();

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

        [XmlElement("simpleType", typeof(XmlSchemaSimpleType)), XmlElement("complexType", typeof(XmlSchemaComplexType))]
        public XmlSchemaType SchemaType
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

        [XmlAttribute("substitutionGroup")]
        public XmlQualifiedName SubstitutionGroup
        {
            get => 
                this.substitutionGroup;
            set
            {
                this.substitutionGroup = (value == null) ? XmlQualifiedName.Empty : value;
            }
        }
    }
}

