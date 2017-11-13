﻿namespace System.Xml.Schema
{
    using System;
    using System.ComponentModel;
    using System.Xml;
    using System.Xml.Serialization;

    public class XmlSchemaType : XmlSchemaAnnotated
    {
        private XmlSchemaType baseSchemaType;
        private XmlSchemaContentType contentType;
        private XmlSchemaDatatype datatype;
        private XmlSchemaDerivationMethod derivedBy;
        private SchemaElementDecl elementDecl;
        private XmlSchemaDerivationMethod final = XmlSchemaDerivationMethod.None;
        private XmlSchemaDerivationMethod finalResolved;
        private string name;
        private XmlQualifiedName qname = XmlQualifiedName.Empty;
        private XmlSchemaType redefined;

        public static XmlSchemaComplexType GetBuiltInComplexType(XmlTypeCode typeCode)
        {
            if (typeCode == XmlTypeCode.Item)
            {
                return XmlSchemaComplexType.AnyType;
            }
            return null;
        }

        public static XmlSchemaComplexType GetBuiltInComplexType(XmlQualifiedName qualifiedName)
        {
            if (qualifiedName == null)
            {
                throw new ArgumentNullException("qualifiedName");
            }
            if (qualifiedName.Equals(XmlSchemaComplexType.AnyType.QualifiedName))
            {
                return XmlSchemaComplexType.AnyType;
            }
            if (qualifiedName.Equals(XmlSchemaComplexType.UntypedAnyType.QualifiedName))
            {
                return XmlSchemaComplexType.UntypedAnyType;
            }
            return null;
        }

        public static XmlSchemaSimpleType GetBuiltInSimpleType(XmlTypeCode typeCode) => 
            DatatypeImplementation.GetSimpleTypeFromTypeCode(typeCode);

        public static XmlSchemaSimpleType GetBuiltInSimpleType(XmlQualifiedName qualifiedName)
        {
            if (qualifiedName == null)
            {
                throw new ArgumentNullException("qualifiedName");
            }
            return DatatypeImplementation.GetSimpleTypeFromXsdType(qualifiedName);
        }

        public static bool IsDerivedFrom(XmlSchemaType derivedType, XmlSchemaType baseType, XmlSchemaDerivationMethod except)
        {
            XmlSchemaSimpleType type;
            if ((derivedType == null) || (baseType == null))
            {
                return false;
            }
            if (derivedType == baseType)
            {
                return true;
            }
            if (baseType == XmlSchemaComplexType.AnyType)
            {
                return true;
            }
        Label_0018:
            type = derivedType as XmlSchemaSimpleType;
            XmlSchemaSimpleType type2 = baseType as XmlSchemaSimpleType;
            if ((type2 != null) && (type != null))
            {
                return ((type2 == DatatypeImplementation.AnySimpleType) || (((except & derivedType.DerivedBy) == XmlSchemaDerivationMethod.Empty) && type.Datatype.IsDerivedFrom(type2.Datatype)));
            }
            if ((except & derivedType.DerivedBy) == XmlSchemaDerivationMethod.Empty)
            {
                derivedType = derivedType.BaseXmlSchemaType;
                if (derivedType == baseType)
                {
                    return true;
                }
                if (derivedType != null)
                {
                    goto Label_0018;
                }
            }
            return false;
        }

        internal static bool IsDerivedFromDatatype(XmlSchemaDatatype derivedDataType, XmlSchemaDatatype baseDataType, XmlSchemaDerivationMethod except) => 
            ((DatatypeImplementation.AnySimpleType.Datatype == baseDataType) || derivedDataType.IsDerivedFrom(baseDataType));

        internal void SetBaseSchemaType(XmlSchemaType value)
        {
            this.baseSchemaType = value;
        }

        internal void SetContentType(XmlSchemaContentType value)
        {
            this.contentType = value;
        }

        internal void SetDatatype(XmlSchemaDatatype value)
        {
            this.datatype = value;
        }

        internal void SetDerivedBy(XmlSchemaDerivationMethod value)
        {
            this.derivedBy = value;
        }

        internal void SetFinalResolved(XmlSchemaDerivationMethod value)
        {
            this.finalResolved = value;
        }

        internal void SetQualifiedName(XmlQualifiedName value)
        {
            this.qname = value;
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

        [XmlIgnore, Obsolete("This property has been deprecated. Please use BaseXmlSchemaType property that returns a strongly typed base schema type. http://go.microsoft.com/fwlink/?linkid=14202")]
        public object BaseSchemaType
        {
            get
            {
                if (this.baseSchemaType.QualifiedName.Namespace == "http://www.w3.org/2001/XMLSchema")
                {
                    return this.baseSchemaType.Datatype;
                }
                return this.baseSchemaType;
            }
        }

        [XmlIgnore]
        public XmlSchemaType BaseXmlSchemaType =>
            this.baseSchemaType;

        [XmlIgnore]
        public XmlSchemaDatatype Datatype =>
            this.datatype;

        [XmlIgnore]
        public XmlSchemaDerivationMethod DerivedBy =>
            this.derivedBy;

        internal virtual XmlQualifiedName DerivedFrom =>
            XmlQualifiedName.Empty;

        internal SchemaElementDecl ElementDecl
        {
            get => 
                this.elementDecl;
            set
            {
                this.elementDecl = value;
            }
        }

        [XmlAttribute("final"), DefaultValue(0x100)]
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

        [XmlIgnore]
        public virtual bool IsMixed
        {
            get => 
                false;
            set
            {
            }
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
            this.qname;

        [XmlIgnore]
        internal XmlSchemaType Redefined
        {
            get => 
                this.redefined;
            set
            {
                this.redefined = value;
            }
        }

        internal XmlSchemaContentType SchemaContentType =>
            this.contentType;

        [XmlIgnore]
        public XmlTypeCode TypeCode
        {
            get
            {
                if (this == XmlSchemaComplexType.AnyType)
                {
                    return XmlTypeCode.Item;
                }
                return this.datatype?.TypeCode;
            }
        }

        [XmlIgnore]
        internal XmlValueConverter ValueConverter =>
            this.datatype?.ValueConverter;
    }
}

