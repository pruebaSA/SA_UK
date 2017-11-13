namespace System.Xml.Schema
{
    using System;
    using System.ComponentModel;
    using System.Xml;
    using System.Xml.Serialization;

    public class XmlSchemaComplexType : XmlSchemaType
    {
        private XmlSchemaAnyAttribute anyAttribute;
        private static XmlSchemaComplexType anyTypeLax = CreateAnyType(XmlSchemaContentProcessing.Lax);
        private static XmlSchemaComplexType anyTypeSkip = CreateAnyType(XmlSchemaContentProcessing.Skip);
        private XmlSchemaObjectCollection attributes;
        private XmlSchemaObjectTable attributeUses;
        private XmlSchemaAnyAttribute attributeWildcard;
        private XmlSchemaDerivationMethod block = XmlSchemaDerivationMethod.None;
        private XmlSchemaDerivationMethod blockResolved;
        private XmlSchemaContentModel contentModel;
        private XmlSchemaParticle contentTypeParticle = XmlSchemaParticle.Empty;
        private const byte dupDeclMask = 2;
        private const byte isAbstractMask = 8;
        private const byte isMixedMask = 4;
        private XmlSchemaObjectTable localElements;
        private XmlSchemaParticle particle;
        private byte pvFlags;
        private static XmlSchemaComplexType untypedAnyType = new XmlSchemaComplexType();
        private const byte wildCardMask = 1;

        static XmlSchemaComplexType()
        {
            untypedAnyType.SetQualifiedName(new XmlQualifiedName("untypedAny", "http://www.w3.org/2003/11/xpath-datatypes"));
            untypedAnyType.IsMixed = true;
            untypedAnyType.SetContentTypeParticle(anyTypeLax.ContentTypeParticle);
            untypedAnyType.SetContentType(XmlSchemaContentType.Mixed);
            untypedAnyType.ElementDecl = SchemaElementDecl.CreateAnyTypeElementDecl();
            untypedAnyType.ElementDecl.SchemaType = untypedAnyType;
            untypedAnyType.ElementDecl.ContentValidator = AnyTypeContentValidator;
        }

        private void ClearCompiledState()
        {
            this.attributeUses = null;
            this.localElements = null;
            this.attributeWildcard = null;
            this.contentTypeParticle = XmlSchemaParticle.Empty;
            this.blockResolved = XmlSchemaDerivationMethod.None;
        }

        internal override XmlSchemaObject Clone()
        {
            XmlSchemaComplexType type = (XmlSchemaComplexType) base.MemberwiseClone();
            if (type.ContentModel != null)
            {
                XmlSchemaSimpleContent contentModel = type.ContentModel as XmlSchemaSimpleContent;
                if (contentModel != null)
                {
                    XmlSchemaSimpleContent content2 = (XmlSchemaSimpleContent) contentModel.Clone();
                    XmlSchemaSimpleContentExtension content = contentModel.Content as XmlSchemaSimpleContentExtension;
                    if (content != null)
                    {
                        XmlSchemaSimpleContentExtension extension2 = (XmlSchemaSimpleContentExtension) content.Clone();
                        extension2.BaseTypeName = content.BaseTypeName.Clone();
                        extension2.SetAttributes(CloneAttributes(content.Attributes));
                        content2.Content = extension2;
                    }
                    else
                    {
                        XmlSchemaSimpleContentRestriction restriction = (XmlSchemaSimpleContentRestriction) contentModel.Content;
                        XmlSchemaSimpleContentRestriction restriction2 = (XmlSchemaSimpleContentRestriction) restriction.Clone();
                        restriction2.BaseTypeName = restriction.BaseTypeName.Clone();
                        restriction2.SetAttributes(CloneAttributes(restriction.Attributes));
                        content2.Content = restriction2;
                    }
                    type.ContentModel = content2;
                }
                else
                {
                    XmlSchemaComplexContent content3 = (XmlSchemaComplexContent) type.ContentModel;
                    XmlSchemaComplexContent content4 = (XmlSchemaComplexContent) content3.Clone();
                    XmlSchemaComplexContentExtension extension3 = content3.Content as XmlSchemaComplexContentExtension;
                    if (extension3 != null)
                    {
                        XmlSchemaComplexContentExtension extension4 = (XmlSchemaComplexContentExtension) extension3.Clone();
                        extension4.BaseTypeName = extension3.BaseTypeName.Clone();
                        extension4.SetAttributes(CloneAttributes(extension3.Attributes));
                        if (HasParticleRef(extension3.Particle))
                        {
                            extension4.Particle = CloneParticle(extension3.Particle);
                        }
                        content4.Content = extension4;
                    }
                    else
                    {
                        XmlSchemaComplexContentRestriction restriction3 = content3.Content as XmlSchemaComplexContentRestriction;
                        XmlSchemaComplexContentRestriction restriction4 = (XmlSchemaComplexContentRestriction) restriction3.Clone();
                        restriction4.BaseTypeName = restriction3.BaseTypeName.Clone();
                        restriction4.SetAttributes(CloneAttributes(restriction3.Attributes));
                        if (HasParticleRef(restriction4.Particle))
                        {
                            restriction4.Particle = CloneParticle(restriction4.Particle);
                        }
                        content4.Content = restriction4;
                    }
                    type.ContentModel = content4;
                }
            }
            else
            {
                if (HasParticleRef(type.Particle))
                {
                    type.Particle = CloneParticle(type.Particle);
                }
                type.SetAttributes(CloneAttributes(type.Attributes));
            }
            type.ClearCompiledState();
            return type;
        }

        internal static XmlSchemaObjectCollection CloneAttributes(XmlSchemaObjectCollection attributes)
        {
            if (!HasAttributeQNameRef(attributes))
            {
                return attributes;
            }
            XmlSchemaObjectCollection objects = attributes.Clone();
            for (int i = 0; i < attributes.Count; i++)
            {
                XmlSchemaObject obj2 = attributes[i];
                XmlSchemaAttributeGroupRef ref2 = obj2 as XmlSchemaAttributeGroupRef;
                if (ref2 != null)
                {
                    XmlSchemaAttributeGroupRef ref3 = (XmlSchemaAttributeGroupRef) ref2.Clone();
                    ref3.RefName = ref2.RefName.Clone();
                    objects[i] = ref3;
                }
                else
                {
                    XmlSchemaAttribute attribute = obj2 as XmlSchemaAttribute;
                    if (!attribute.RefName.IsEmpty || !attribute.SchemaTypeName.IsEmpty)
                    {
                        objects[i] = attribute.Clone();
                    }
                }
            }
            return objects;
        }

        private static XmlSchemaObjectCollection CloneGroupBaseParticles(XmlSchemaObjectCollection groupBaseParticles)
        {
            XmlSchemaObjectCollection objects = groupBaseParticles.Clone();
            for (int i = 0; i < groupBaseParticles.Count; i++)
            {
                XmlSchemaParticle particle = (XmlSchemaParticle) groupBaseParticles[i];
                objects[i] = CloneParticle(particle);
            }
            return objects;
        }

        internal static XmlSchemaParticle CloneParticle(XmlSchemaParticle particle)
        {
            XmlSchemaGroupBase base2 = particle as XmlSchemaGroupBase;
            if ((base2 != null) && !(base2 is XmlSchemaAll))
            {
                XmlSchemaGroupBase base3 = base2;
                XmlSchemaObjectCollection newItems = CloneGroupBaseParticles(base2.Items);
                base3 = (XmlSchemaGroupBase) base2.Clone();
                base3.SetItems(newItems);
                return base3;
            }
            if (particle is XmlSchemaGroupRef)
            {
                XmlSchemaGroupRef ref2 = (XmlSchemaGroupRef) particle.Clone();
                ref2.RefName = ref2.RefName.Clone();
                return ref2;
            }
            XmlSchemaElement element = particle as XmlSchemaElement;
            if ((element == null) || (element.RefName.IsEmpty && element.SchemaTypeName.IsEmpty))
            {
                return particle;
            }
            return (XmlSchemaElement) element.Clone();
        }

        internal bool ContainsIdAttribute(bool findAll)
        {
            int num = 0;
            foreach (XmlSchemaAttribute attribute in this.AttributeUses.Values)
            {
                if (attribute.Use != XmlSchemaUse.Prohibited)
                {
                    XmlSchemaDatatype datatype = attribute.Datatype;
                    if ((datatype != null) && (datatype.TypeCode == XmlTypeCode.Id))
                    {
                        num++;
                        if (num > 1)
                        {
                            break;
                        }
                    }
                }
            }
            if (!findAll)
            {
                return (num > 0);
            }
            return (num > 1);
        }

        private static XmlSchemaComplexType CreateAnyType(XmlSchemaContentProcessing processContents)
        {
            XmlSchemaComplexType type = new XmlSchemaComplexType();
            type.SetQualifiedName(DatatypeImplementation.QnAnyType);
            XmlSchemaAny item = new XmlSchemaAny {
                MinOccurs = 0M,
                MaxOccurs = 79228162514264337593543950335M,
                ProcessContents = processContents
            };
            item.BuildNamespaceList(null);
            XmlSchemaSequence sequence = new XmlSchemaSequence();
            sequence.Items.Add(item);
            type.SetContentTypeParticle(sequence);
            type.SetContentType(XmlSchemaContentType.Mixed);
            type.ElementDecl = SchemaElementDecl.CreateAnyTypeElementDecl();
            type.ElementDecl.SchemaType = type;
            ParticleContentValidator validator = new ParticleContentValidator(XmlSchemaContentType.Mixed);
            validator.Start();
            validator.OpenGroup();
            validator.AddNamespaceList(item.NamespaceList, item);
            validator.AddStar();
            validator.CloseGroup();
            ContentValidator validator2 = validator.Finish(true);
            type.ElementDecl.ContentValidator = validator2;
            XmlSchemaAnyAttribute attribute = new XmlSchemaAnyAttribute {
                ProcessContents = processContents
            };
            attribute.BuildNamespaceList(null);
            type.SetAttributeWildcard(attribute);
            type.ElementDecl.AnyAttribute = attribute;
            return type;
        }

        internal static bool HasAttributeQNameRef(XmlSchemaObjectCollection attributes)
        {
            foreach (XmlSchemaObject obj2 in attributes)
            {
                if (obj2 is XmlSchemaAttributeGroupRef)
                {
                    return true;
                }
                XmlSchemaAttribute attribute = obj2 as XmlSchemaAttribute;
                if (!attribute.RefName.IsEmpty || !attribute.SchemaTypeName.IsEmpty)
                {
                    return true;
                }
            }
            return false;
        }

        internal static bool HasParticleRef(XmlSchemaParticle particle)
        {
            XmlSchemaGroupBase base2 = particle as XmlSchemaGroupBase;
            if ((base2 == null) || (base2 is XmlSchemaAll))
            {
                return (particle is XmlSchemaGroupRef);
            }
            bool flag = false;
            int num = 0;
            while ((num < base2.Items.Count) && !flag)
            {
                XmlSchemaParticle particle2 = (XmlSchemaParticle) base2.Items[num++];
                if (particle2 is XmlSchemaGroupRef)
                {
                    flag = true;
                }
                else
                {
                    XmlSchemaElement element = particle2 as XmlSchemaElement;
                    if ((element != null) && (!element.RefName.IsEmpty || !element.SchemaTypeName.IsEmpty))
                    {
                        flag = true;
                        continue;
                    }
                    flag = HasParticleRef(particle2);
                }
            }
            return flag;
        }

        internal void SetAttributes(XmlSchemaObjectCollection newAttributes)
        {
            this.attributes = newAttributes;
        }

        internal void SetAttributeWildcard(XmlSchemaAnyAttribute value)
        {
            this.attributeWildcard = value;
        }

        internal void SetBlockResolved(XmlSchemaDerivationMethod value)
        {
            this.blockResolved = value;
        }

        internal void SetContentTypeParticle(XmlSchemaParticle value)
        {
            this.contentTypeParticle = value;
        }

        [XmlElement("anyAttribute")]
        public XmlSchemaAnyAttribute AnyAttribute
        {
            get => 
                this.anyAttribute;
            set
            {
                this.anyAttribute = value;
            }
        }

        [XmlIgnore]
        internal static XmlSchemaComplexType AnyType =>
            anyTypeLax;

        internal static ContentValidator AnyTypeContentValidator =>
            anyTypeLax.ElementDecl.ContentValidator;

        [XmlIgnore]
        internal static XmlSchemaComplexType AnyTypeSkip =>
            anyTypeSkip;

        [XmlElement("attribute", typeof(XmlSchemaAttribute)), XmlElement("attributeGroup", typeof(XmlSchemaAttributeGroupRef))]
        public XmlSchemaObjectCollection Attributes
        {
            get
            {
                if (this.attributes == null)
                {
                    this.attributes = new XmlSchemaObjectCollection();
                }
                return this.attributes;
            }
        }

        [XmlIgnore]
        public XmlSchemaObjectTable AttributeUses
        {
            get
            {
                if (this.attributeUses == null)
                {
                    this.attributeUses = new XmlSchemaObjectTable();
                }
                return this.attributeUses;
            }
        }

        [XmlIgnore]
        public XmlSchemaAnyAttribute AttributeWildcard =>
            this.attributeWildcard;

        [XmlAttribute("block"), DefaultValue(0x100)]
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

        [XmlElement("complexContent", typeof(XmlSchemaComplexContent)), XmlElement("simpleContent", typeof(XmlSchemaSimpleContent))]
        public XmlSchemaContentModel ContentModel
        {
            get => 
                this.contentModel;
            set
            {
                this.contentModel = value;
            }
        }

        [XmlIgnore]
        public XmlSchemaContentType ContentType =>
            base.SchemaContentType;

        [XmlIgnore]
        public XmlSchemaParticle ContentTypeParticle =>
            this.contentTypeParticle;

        internal override XmlQualifiedName DerivedFrom
        {
            get
            {
                if (this.contentModel != null)
                {
                    if (this.contentModel.Content is XmlSchemaComplexContentRestriction)
                    {
                        return ((XmlSchemaComplexContentRestriction) this.contentModel.Content).BaseTypeName;
                    }
                    if (this.contentModel.Content is XmlSchemaComplexContentExtension)
                    {
                        return ((XmlSchemaComplexContentExtension) this.contentModel.Content).BaseTypeName;
                    }
                    if (this.contentModel.Content is XmlSchemaSimpleContentRestriction)
                    {
                        return ((XmlSchemaSimpleContentRestriction) this.contentModel.Content).BaseTypeName;
                    }
                    if (this.contentModel.Content is XmlSchemaSimpleContentExtension)
                    {
                        return ((XmlSchemaSimpleContentExtension) this.contentModel.Content).BaseTypeName;
                    }
                }
                return XmlQualifiedName.Empty;
            }
        }

        internal bool HasDuplicateDecls
        {
            get => 
                ((this.pvFlags & 2) != 0);
            set
            {
                if (value)
                {
                    this.pvFlags = (byte) (this.pvFlags | 2);
                }
                else
                {
                    this.pvFlags = (byte) (this.pvFlags & -3);
                }
            }
        }

        internal bool HasWildCard
        {
            get => 
                ((this.pvFlags & 1) != 0);
            set
            {
                if (value)
                {
                    this.pvFlags = (byte) (this.pvFlags | 1);
                }
                else
                {
                    this.pvFlags = (byte) (this.pvFlags & -2);
                }
            }
        }

        [XmlAttribute("abstract"), DefaultValue(false)]
        public bool IsAbstract
        {
            get => 
                ((this.pvFlags & 8) != 0);
            set
            {
                if (value)
                {
                    this.pvFlags = (byte) (this.pvFlags | 8);
                }
                else
                {
                    this.pvFlags = (byte) (this.pvFlags & -9);
                }
            }
        }

        [DefaultValue(false), XmlAttribute("mixed")]
        public override bool IsMixed
        {
            get => 
                ((this.pvFlags & 4) != 0);
            set
            {
                if (value)
                {
                    this.pvFlags = (byte) (this.pvFlags | 4);
                }
                else
                {
                    this.pvFlags = (byte) (this.pvFlags & -5);
                }
            }
        }

        [XmlIgnore]
        internal XmlSchemaObjectTable LocalElements
        {
            get
            {
                if (this.localElements == null)
                {
                    this.localElements = new XmlSchemaObjectTable();
                }
                return this.localElements;
            }
        }

        [XmlElement("sequence", typeof(XmlSchemaSequence)), XmlElement("all", typeof(XmlSchemaAll)), XmlElement("choice", typeof(XmlSchemaChoice)), XmlElement("group", typeof(XmlSchemaGroupRef))]
        public XmlSchemaParticle Particle
        {
            get => 
                this.particle;
            set
            {
                this.particle = value;
            }
        }

        [XmlIgnore]
        internal static XmlSchemaComplexType UntypedAnyType =>
            untypedAnyType;
    }
}

