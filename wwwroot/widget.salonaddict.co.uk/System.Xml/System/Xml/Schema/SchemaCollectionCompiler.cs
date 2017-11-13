namespace System.Xml.Schema
{
    using System;
    using System.Collections;
    using System.Globalization;
    using System.Runtime.InteropServices;
    using System.Xml;

    internal sealed class SchemaCollectionCompiler : BaseProcessor
    {
        private bool compileContentModel;
        private Stack complexTypeStack;
        private XmlSchemaObjectTable examplars;
        private XmlSchema schema;

        public SchemaCollectionCompiler(XmlNameTable nameTable, ValidationEventHandler eventHandler) : base(nameTable, null, eventHandler)
        {
            this.examplars = new XmlSchemaObjectTable();
            this.complexTypeStack = new Stack();
        }

        private void BuildParticleContentModel(ParticleContentValidator contentValidator, XmlSchemaParticle particle)
        {
            if (particle is XmlSchemaElement)
            {
                XmlSchemaElement element = (XmlSchemaElement) particle;
                contentValidator.AddName(element.QualifiedName, element);
            }
            else if (particle is XmlSchemaAny)
            {
                XmlSchemaAny any = (XmlSchemaAny) particle;
                contentValidator.AddNamespaceList(any.NamespaceList, any);
            }
            else if (particle is XmlSchemaGroupBase)
            {
                XmlSchemaObjectCollection items = ((XmlSchemaGroupBase) particle).Items;
                bool flag = particle is XmlSchemaChoice;
                contentValidator.OpenGroup();
                bool flag2 = true;
                foreach (XmlSchemaParticle particle2 in items)
                {
                    if (flag2)
                    {
                        flag2 = false;
                    }
                    else if (flag)
                    {
                        contentValidator.AddChoice();
                    }
                    else
                    {
                        contentValidator.AddSequence();
                    }
                    this.BuildParticleContentModel(contentValidator, particle2);
                }
                contentValidator.CloseGroup();
            }
            if ((particle.MinOccurs != 1M) || (particle.MaxOccurs != 1M))
            {
                if ((particle.MinOccurs == 0M) && (particle.MaxOccurs == 1M))
                {
                    contentValidator.AddQMark();
                }
                else if ((particle.MinOccurs == 0M) && (particle.MaxOccurs == 79228162514264337593543950335M))
                {
                    contentValidator.AddStar();
                }
                else if ((particle.MinOccurs == 1M) && (particle.MaxOccurs == 79228162514264337593543950335M))
                {
                    contentValidator.AddPlus();
                }
                else
                {
                    contentValidator.AddLeafRange(particle.MinOccurs, particle.MaxOccurs);
                }
            }
        }

        private void CalculateEffectiveTotalRange(XmlSchemaParticle particle, out decimal minOccurs, out decimal maxOccurs)
        {
            if ((particle is XmlSchemaElement) || (particle is XmlSchemaAny))
            {
                minOccurs = particle.MinOccurs;
                maxOccurs = particle.MaxOccurs;
            }
            else if (particle is XmlSchemaChoice)
            {
                if (((XmlSchemaChoice) particle).Items.Count == 0)
                {
                    minOccurs = maxOccurs = 0M;
                }
                else
                {
                    minOccurs = 79228162514264337593543950335M;
                    maxOccurs = 0M;
                    foreach (XmlSchemaParticle particle2 in ((XmlSchemaChoice) particle).Items)
                    {
                        decimal num;
                        decimal num2;
                        this.CalculateEffectiveTotalRange(particle2, out num, out num2);
                        if (num < minOccurs)
                        {
                            minOccurs = num;
                        }
                        if (num2 > maxOccurs)
                        {
                            maxOccurs = num2;
                        }
                    }
                    minOccurs *= particle.MinOccurs;
                    if (maxOccurs != 79228162514264337593543950335M)
                    {
                        if (particle.MaxOccurs == 79228162514264337593543950335M)
                        {
                            maxOccurs = 79228162514264337593543950335M;
                        }
                        else
                        {
                            maxOccurs *= particle.MaxOccurs;
                        }
                    }
                }
            }
            else
            {
                XmlSchemaObjectCollection items = ((XmlSchemaGroupBase) particle).Items;
                if (items.Count == 0)
                {
                    minOccurs = maxOccurs = 0M;
                }
                else
                {
                    minOccurs = 0M;
                    maxOccurs = 0M;
                    foreach (XmlSchemaParticle particle3 in items)
                    {
                        decimal num3;
                        decimal num4;
                        this.CalculateEffectiveTotalRange(particle3, out num3, out num4);
                        minOccurs += num3;
                        if (maxOccurs != 79228162514264337593543950335M)
                        {
                            if (num4 == 79228162514264337593543950335M)
                            {
                                maxOccurs = 79228162514264337593543950335M;
                            }
                            else
                            {
                                maxOccurs += num4;
                            }
                        }
                    }
                    minOccurs *= particle.MinOccurs;
                    if (maxOccurs != 79228162514264337593543950335M)
                    {
                        if (particle.MaxOccurs == 79228162514264337593543950335M)
                        {
                            maxOccurs = 79228162514264337593543950335M;
                        }
                        else
                        {
                            maxOccurs *= particle.MaxOccurs;
                        }
                    }
                }
            }
        }

        private void CalculateSequenceRange(XmlSchemaSequence sequence, out decimal minOccurs, out decimal maxOccurs)
        {
            minOccurs = 0M;
            maxOccurs = 0M;
            foreach (XmlSchemaParticle particle in sequence.Items)
            {
                minOccurs += particle.MinOccurs;
                if (particle.MaxOccurs == 79228162514264337593543950335M)
                {
                    maxOccurs = 79228162514264337593543950335M;
                }
                else if (maxOccurs != 79228162514264337593543950335M)
                {
                    maxOccurs += particle.MaxOccurs;
                }
            }
            minOccurs *= sequence.MinOccurs;
            if (sequence.MaxOccurs == 79228162514264337593543950335M)
            {
                maxOccurs = 79228162514264337593543950335M;
            }
            else if (maxOccurs != 79228162514264337593543950335M)
            {
                maxOccurs *= sequence.MaxOccurs;
            }
        }

        private XmlSchemaParticle CannonicalizeAll(XmlSchemaAll all, bool root, bool substitution)
        {
            if (all.Items.Count > 0)
            {
                XmlSchemaAll all2 = new XmlSchemaAll {
                    MinOccurs = all.MinOccurs,
                    MaxOccurs = all.MaxOccurs,
                    SourceUri = all.SourceUri,
                    LineNumber = all.LineNumber,
                    LinePosition = all.LinePosition
                };
                foreach (XmlSchemaElement element in all.Items)
                {
                    XmlSchemaParticle item = this.CannonicalizeParticle(element, false, substitution);
                    if (item != XmlSchemaParticle.Empty)
                    {
                        all2.Items.Add(item);
                    }
                }
                all = all2;
            }
            if (all.Items.Count == 0)
            {
                return XmlSchemaParticle.Empty;
            }
            if (root && (all.Items.Count == 1))
            {
                XmlSchemaSequence sequence = new XmlSchemaSequence {
                    MinOccurs = all.MinOccurs,
                    MaxOccurs = all.MaxOccurs
                };
                sequence.Items.Add((XmlSchemaParticle) all.Items[0]);
                return sequence;
            }
            if ((!root && (all.Items.Count == 1)) && ((all.MinOccurs == 1M) && (all.MaxOccurs == 1M)))
            {
                return (XmlSchemaParticle) all.Items[0];
            }
            if (!root)
            {
                base.SendValidationEvent("Sch_NotAllAlone", all);
                return XmlSchemaParticle.Empty;
            }
            return all;
        }

        private XmlSchemaParticle CannonicalizeChoice(XmlSchemaChoice choice, bool root, bool substitution)
        {
            XmlSchemaChoice source = choice;
            if (choice.Items.Count > 0)
            {
                XmlSchemaChoice choice3 = new XmlSchemaChoice {
                    MinOccurs = choice.MinOccurs,
                    MaxOccurs = choice.MaxOccurs
                };
                foreach (XmlSchemaParticle particle in choice.Items)
                {
                    XmlSchemaParticle item = this.CannonicalizeParticle(particle, false, substitution);
                    if (item != XmlSchemaParticle.Empty)
                    {
                        if (((item.MinOccurs == 1M) && (item.MaxOccurs == 1M)) && (item is XmlSchemaChoice))
                        {
                            foreach (XmlSchemaParticle particle3 in ((XmlSchemaChoice) item).Items)
                            {
                                choice3.Items.Add(particle3);
                            }
                        }
                        else
                        {
                            choice3.Items.Add(item);
                        }
                    }
                }
                choice = choice3;
            }
            if (!root && (choice.Items.Count == 0))
            {
                if (choice.MinOccurs != 0M)
                {
                    base.SendValidationEvent("Sch_EmptyChoice", source, XmlSeverityType.Warning);
                }
                return XmlSchemaParticle.Empty;
            }
            if ((!root && (choice.Items.Count == 1)) && ((choice.MinOccurs == 1M) && (choice.MaxOccurs == 1M)))
            {
                return (XmlSchemaParticle) choice.Items[0];
            }
            return choice;
        }

        private XmlSchemaParticle CannonicalizeElement(XmlSchemaElement element, bool substitution)
        {
            if ((element.RefName.IsEmpty || !substitution) || ((element.BlockResolved & XmlSchemaDerivationMethod.Substitution) != XmlSchemaDerivationMethod.Empty))
            {
                return element;
            }
            XmlSchemaSubstitutionGroupV1Compat compat = (XmlSchemaSubstitutionGroupV1Compat) this.examplars[element.QualifiedName];
            if (compat == null)
            {
                return element;
            }
            XmlSchemaChoice choice = (XmlSchemaChoice) compat.Choice.Clone();
            choice.MinOccurs = element.MinOccurs;
            choice.MaxOccurs = element.MaxOccurs;
            return choice;
        }

        private XmlSchemaParticle CannonicalizeGroupRef(XmlSchemaGroupRef groupRef, bool root, bool substitution)
        {
            XmlSchemaGroup redefined;
            if (groupRef.Redefined != null)
            {
                redefined = groupRef.Redefined;
            }
            else
            {
                redefined = (XmlSchemaGroup) this.schema.Groups[groupRef.RefName];
            }
            if (redefined == null)
            {
                base.SendValidationEvent("Sch_UndefGroupRef", groupRef.RefName.ToString(), groupRef);
                return XmlSchemaParticle.Empty;
            }
            if (redefined.CanonicalParticle == null)
            {
                this.CompileGroup(redefined);
            }
            if (redefined.CanonicalParticle == XmlSchemaParticle.Empty)
            {
                return XmlSchemaParticle.Empty;
            }
            XmlSchemaGroupBase canonicalParticle = (XmlSchemaGroupBase) redefined.CanonicalParticle;
            if (canonicalParticle is XmlSchemaAll)
            {
                if (!root)
                {
                    base.SendValidationEvent("Sch_AllRefNotRoot", "", groupRef);
                    return XmlSchemaParticle.Empty;
                }
                if ((groupRef.MinOccurs != 1M) || (groupRef.MaxOccurs != 1M))
                {
                    base.SendValidationEvent("Sch_AllRefMinMax", groupRef);
                    return XmlSchemaParticle.Empty;
                }
            }
            else if ((canonicalParticle is XmlSchemaChoice) && (canonicalParticle.Items.Count == 0))
            {
                if (groupRef.MinOccurs != 0M)
                {
                    base.SendValidationEvent("Sch_EmptyChoice", groupRef, XmlSeverityType.Warning);
                }
                return XmlSchemaParticle.Empty;
            }
            XmlSchemaGroupBase base3 = (canonicalParticle is XmlSchemaSequence) ? ((XmlSchemaGroupBase) new XmlSchemaSequence()) : ((canonicalParticle is XmlSchemaChoice) ? ((XmlSchemaGroupBase) new XmlSchemaChoice()) : ((XmlSchemaGroupBase) new XmlSchemaAll()));
            base3.MinOccurs = groupRef.MinOccurs;
            base3.MaxOccurs = groupRef.MaxOccurs;
            foreach (XmlSchemaParticle particle in canonicalParticle.Items)
            {
                base3.Items.Add(particle);
            }
            groupRef.SetParticle(base3);
            return base3;
        }

        private XmlSchemaParticle CannonicalizeParticle(XmlSchemaParticle particle, bool root, bool substitution)
        {
            if ((particle == null) || particle.IsEmpty)
            {
                return XmlSchemaParticle.Empty;
            }
            if (particle is XmlSchemaElement)
            {
                return this.CannonicalizeElement((XmlSchemaElement) particle, substitution);
            }
            if (particle is XmlSchemaGroupRef)
            {
                return this.CannonicalizeGroupRef((XmlSchemaGroupRef) particle, root, substitution);
            }
            if (particle is XmlSchemaAll)
            {
                return this.CannonicalizeAll((XmlSchemaAll) particle, root, substitution);
            }
            if (particle is XmlSchemaChoice)
            {
                return this.CannonicalizeChoice((XmlSchemaChoice) particle, root, substitution);
            }
            if (particle is XmlSchemaSequence)
            {
                return this.CannonicalizeSequence((XmlSchemaSequence) particle, root, substitution);
            }
            return particle;
        }

        private XmlSchemaParticle CannonicalizeSequence(XmlSchemaSequence sequence, bool root, bool substitution)
        {
            if (sequence.Items.Count > 0)
            {
                XmlSchemaSequence sequence2 = new XmlSchemaSequence {
                    MinOccurs = sequence.MinOccurs,
                    MaxOccurs = sequence.MaxOccurs
                };
                foreach (XmlSchemaParticle particle in sequence.Items)
                {
                    XmlSchemaParticle item = this.CannonicalizeParticle(particle, false, substitution);
                    if (item != XmlSchemaParticle.Empty)
                    {
                        if (((item.MinOccurs == 1M) && (item.MaxOccurs == 1M)) && (item is XmlSchemaSequence))
                        {
                            foreach (XmlSchemaParticle particle3 in ((XmlSchemaSequence) item).Items)
                            {
                                sequence2.Items.Add(particle3);
                            }
                        }
                        else
                        {
                            sequence2.Items.Add(item);
                        }
                    }
                }
                sequence = sequence2;
            }
            if (sequence.Items.Count == 0)
            {
                return XmlSchemaParticle.Empty;
            }
            if ((!root && (sequence.Items.Count == 1)) && ((sequence.MinOccurs == 1M) && (sequence.MaxOccurs == 1M)))
            {
                return (XmlSchemaParticle) sequence.Items[0];
            }
            return sequence;
        }

        private void CheckParticleDerivation(XmlSchemaComplexType complexType)
        {
            XmlSchemaComplexType baseXmlSchemaType = complexType.BaseXmlSchemaType as XmlSchemaComplexType;
            if (((baseXmlSchemaType != null) && (baseXmlSchemaType != XmlSchemaComplexType.AnyType)) && ((complexType.DerivedBy == XmlSchemaDerivationMethod.Restriction) && !this.IsValidRestriction(complexType.ContentTypeParticle, baseXmlSchemaType.ContentTypeParticle)))
            {
                base.SendValidationEvent("Sch_InvalidParticleRestriction", complexType);
            }
        }

        private void CheckSubstitutionGroup(XmlSchemaSubstitutionGroup substitutionGroup)
        {
            XmlSchemaElement element = (XmlSchemaElement) this.schema.Elements[substitutionGroup.Examplar];
            if (element != null)
            {
                foreach (XmlSchemaElement element2 in substitutionGroup.Members)
                {
                    if ((element2 != element) && !XmlSchemaType.IsDerivedFrom(element2.ElementSchemaType, element.ElementSchemaType, element.FinalResolved))
                    {
                        base.SendValidationEvent("Sch_InvalidSubstitutionMember", element2.QualifiedName.ToString(), element.QualifiedName.ToString(), element2);
                    }
                }
            }
        }

        private void CheckUnionType(XmlSchemaSimpleType unionMember, ArrayList memberTypeDefinitions, XmlSchemaSimpleType parentType)
        {
            XmlSchemaDatatype datatype = unionMember.Datatype;
            if ((unionMember.DerivedBy == XmlSchemaDerivationMethod.Restriction) && (datatype.HasLexicalFacets || datatype.HasValueFacets))
            {
                base.SendValidationEvent("Sch_UnionFromUnion", parentType);
            }
            else
            {
                Datatype_union _union = unionMember.Datatype as Datatype_union;
                memberTypeDefinitions.AddRange(_union.BaseMemberTypes);
            }
        }

        private void Cleanup()
        {
            foreach (XmlSchemaGroup group in this.schema.Groups.Values)
            {
                CleanupGroup(group);
            }
            foreach (XmlSchemaAttributeGroup group2 in this.schema.AttributeGroups.Values)
            {
                CleanupAttributeGroup(group2);
            }
            foreach (XmlSchemaType type in this.schema.SchemaTypes.Values)
            {
                if (type is XmlSchemaComplexType)
                {
                    CleanupComplexType((XmlSchemaComplexType) type);
                }
                else
                {
                    CleanupSimpleType((XmlSchemaSimpleType) type);
                }
            }
            foreach (XmlSchemaElement element in this.schema.Elements.Values)
            {
                CleanupElement(element);
            }
            foreach (XmlSchemaAttribute attribute in this.schema.Attributes.Values)
            {
                CleanupAttribute(attribute);
            }
        }

        internal static void Cleanup(XmlSchema schema)
        {
            foreach (XmlSchemaExternal external in schema.Includes)
            {
                if (external.Schema != null)
                {
                    Cleanup(external.Schema);
                }
                if (external is XmlSchemaRedefine)
                {
                    XmlSchemaRedefine redefine = external as XmlSchemaRedefine;
                    redefine.AttributeGroups.Clear();
                    redefine.Groups.Clear();
                    redefine.SchemaTypes.Clear();
                    foreach (object obj2 in redefine.Items)
                    {
                        if (obj2 is XmlSchemaAttribute)
                        {
                            CleanupAttribute((XmlSchemaAttribute) obj2);
                        }
                        else if (obj2 is XmlSchemaAttributeGroup)
                        {
                            CleanupAttributeGroup((XmlSchemaAttributeGroup) obj2);
                        }
                        else if (obj2 is XmlSchemaComplexType)
                        {
                            CleanupComplexType((XmlSchemaComplexType) obj2);
                        }
                        else if (obj2 is XmlSchemaSimpleType)
                        {
                            CleanupSimpleType((XmlSchemaSimpleType) obj2);
                        }
                        else if (obj2 is XmlSchemaElement)
                        {
                            CleanupElement((XmlSchemaElement) obj2);
                        }
                        else if (obj2 is XmlSchemaGroup)
                        {
                            CleanupGroup((XmlSchemaGroup) obj2);
                        }
                    }
                }
            }
            foreach (object obj3 in schema.Items)
            {
                if (obj3 is XmlSchemaAttribute)
                {
                    CleanupAttribute((XmlSchemaAttribute) obj3);
                }
                else if (obj3 is XmlSchemaAttributeGroup)
                {
                    CleanupAttributeGroup((XmlSchemaAttributeGroup) obj3);
                }
                else if (obj3 is XmlSchemaComplexType)
                {
                    CleanupComplexType((XmlSchemaComplexType) obj3);
                }
                else if (obj3 is XmlSchemaSimpleType)
                {
                    CleanupSimpleType((XmlSchemaSimpleType) obj3);
                }
                else if (obj3 is XmlSchemaElement)
                {
                    CleanupElement((XmlSchemaElement) obj3);
                }
                else if (obj3 is XmlSchemaGroup)
                {
                    CleanupGroup((XmlSchemaGroup) obj3);
                }
            }
            schema.Attributes.Clear();
            schema.AttributeGroups.Clear();
            schema.SchemaTypes.Clear();
            schema.Elements.Clear();
            schema.Groups.Clear();
            schema.Notations.Clear();
            schema.Ids.Clear();
            schema.IdentityConstraints.Clear();
        }

        private static void CleanupAttribute(XmlSchemaAttribute attribute)
        {
            if (attribute.SchemaType != null)
            {
                CleanupSimpleType(attribute.SchemaType);
            }
            attribute.AttDef = null;
        }

        private static void CleanupAttributeGroup(XmlSchemaAttributeGroup attributeGroup)
        {
            CleanupAttributes(attributeGroup.Attributes);
            attributeGroup.AttributeUses.Clear();
            attributeGroup.AttributeWildcard = null;
        }

        private static void CleanupAttributes(XmlSchemaObjectCollection attributes)
        {
            foreach (XmlSchemaObject obj2 in attributes)
            {
                if (obj2 is XmlSchemaAttribute)
                {
                    CleanupAttribute((XmlSchemaAttribute) obj2);
                }
            }
        }

        private static void CleanupComplexType(XmlSchemaComplexType complexType)
        {
            if (complexType.ContentModel != null)
            {
                if (complexType.ContentModel is XmlSchemaSimpleContent)
                {
                    XmlSchemaSimpleContent contentModel = (XmlSchemaSimpleContent) complexType.ContentModel;
                    if (contentModel.Content is XmlSchemaSimpleContentExtension)
                    {
                        XmlSchemaSimpleContentExtension content = (XmlSchemaSimpleContentExtension) contentModel.Content;
                        CleanupAttributes(content.Attributes);
                    }
                    else
                    {
                        XmlSchemaSimpleContentRestriction restriction = (XmlSchemaSimpleContentRestriction) contentModel.Content;
                        CleanupAttributes(restriction.Attributes);
                    }
                }
                else
                {
                    XmlSchemaComplexContent content2 = (XmlSchemaComplexContent) complexType.ContentModel;
                    if (content2.Content is XmlSchemaComplexContentExtension)
                    {
                        XmlSchemaComplexContentExtension extension2 = (XmlSchemaComplexContentExtension) content2.Content;
                        CleanupParticle(extension2.Particle);
                        CleanupAttributes(extension2.Attributes);
                    }
                    else
                    {
                        XmlSchemaComplexContentRestriction restriction2 = (XmlSchemaComplexContentRestriction) content2.Content;
                        CleanupParticle(restriction2.Particle);
                        CleanupAttributes(restriction2.Attributes);
                    }
                }
            }
            else
            {
                CleanupParticle(complexType.Particle);
                CleanupAttributes(complexType.Attributes);
            }
            complexType.LocalElements.Clear();
            complexType.AttributeUses.Clear();
            complexType.SetAttributeWildcard(null);
            complexType.SetContentTypeParticle(XmlSchemaParticle.Empty);
            complexType.ElementDecl = null;
        }

        private static void CleanupElement(XmlSchemaElement element)
        {
            if (element.SchemaType != null)
            {
                XmlSchemaComplexType schemaType = element.SchemaType as XmlSchemaComplexType;
                if (schemaType != null)
                {
                    CleanupComplexType(schemaType);
                }
                else
                {
                    CleanupSimpleType((XmlSchemaSimpleType) element.SchemaType);
                }
            }
            foreach (XmlSchemaIdentityConstraint constraint in element.Constraints)
            {
                constraint.CompiledConstraint = null;
            }
            element.ElementDecl = null;
        }

        private static void CleanupGroup(XmlSchemaGroup group)
        {
            CleanupParticle(group.Particle);
            group.CanonicalParticle = null;
        }

        private static void CleanupParticle(XmlSchemaParticle particle)
        {
            if (particle is XmlSchemaElement)
            {
                CleanupElement((XmlSchemaElement) particle);
            }
            else if (particle is XmlSchemaGroupBase)
            {
                foreach (XmlSchemaParticle particle2 in ((XmlSchemaGroupBase) particle).Items)
                {
                    CleanupParticle(particle2);
                }
            }
        }

        private static void CleanupSimpleType(XmlSchemaSimpleType simpleType)
        {
            simpleType.ElementDecl = null;
        }

        private void Compile()
        {
            this.schema.SchemaTypes.Insert(DatatypeImplementation.QnAnyType, XmlSchemaComplexType.AnyType);
            foreach (XmlSchemaSubstitutionGroupV1Compat compat in this.examplars.Values)
            {
                this.CompileSubstitutionGroup(compat);
            }
            foreach (XmlSchemaGroup group in this.schema.Groups.Values)
            {
                this.CompileGroup(group);
            }
            foreach (XmlSchemaAttributeGroup group2 in this.schema.AttributeGroups.Values)
            {
                this.CompileAttributeGroup(group2);
            }
            foreach (XmlSchemaType type in this.schema.SchemaTypes.Values)
            {
                if (type is XmlSchemaComplexType)
                {
                    this.CompileComplexType((XmlSchemaComplexType) type);
                }
                else
                {
                    this.CompileSimpleType((XmlSchemaSimpleType) type);
                }
            }
            foreach (XmlSchemaElement element in this.schema.Elements.Values)
            {
                if (element.ElementDecl == null)
                {
                    this.CompileElement(element);
                }
            }
            foreach (XmlSchemaAttribute attribute in this.schema.Attributes.Values)
            {
                if (attribute.AttDef == null)
                {
                    this.CompileAttribute(attribute);
                }
            }
            foreach (XmlSchemaIdentityConstraint constraint in this.schema.IdentityConstraints.Values)
            {
                if (constraint.CompiledConstraint == null)
                {
                    this.CompileIdentityConstraint(constraint);
                }
            }
            while (this.complexTypeStack.Count > 0)
            {
                XmlSchemaComplexType complexType = (XmlSchemaComplexType) this.complexTypeStack.Pop();
                this.CompileCompexTypeElements(complexType);
            }
            foreach (XmlSchemaType type3 in this.schema.SchemaTypes.Values)
            {
                if (type3 is XmlSchemaComplexType)
                {
                    this.CheckParticleDerivation((XmlSchemaComplexType) type3);
                }
            }
            foreach (XmlSchemaElement element2 in this.schema.Elements.Values)
            {
                if ((element2.ElementSchemaType is XmlSchemaComplexType) && (element2.SchemaTypeName == XmlQualifiedName.Empty))
                {
                    this.CheckParticleDerivation((XmlSchemaComplexType) element2.ElementSchemaType);
                }
            }
            foreach (XmlSchemaSubstitutionGroup group3 in this.examplars.Values)
            {
                this.CheckSubstitutionGroup(group3);
            }
            this.schema.SchemaTypes.Remove(DatatypeImplementation.QnAnyType);
        }

        private XmlSchemaAnyAttribute CompileAnyAttributeIntersection(XmlSchemaAnyAttribute a, XmlSchemaAnyAttribute b)
        {
            if (a == null)
            {
                return b;
            }
            if (b == null)
            {
                return a;
            }
            XmlSchemaAnyAttribute attribute = XmlSchemaAnyAttribute.Intersection(a, b, true);
            if (attribute == null)
            {
                base.SendValidationEvent("Sch_UnexpressibleAnyAttribute", a);
            }
            return attribute;
        }

        private XmlSchemaAnyAttribute CompileAnyAttributeUnion(XmlSchemaAnyAttribute a, XmlSchemaAnyAttribute b)
        {
            if (a == null)
            {
                return b;
            }
            if (b == null)
            {
                return a;
            }
            XmlSchemaAnyAttribute attribute = XmlSchemaAnyAttribute.Union(a, b, true);
            if (attribute == null)
            {
                base.SendValidationEvent("Sch_UnexpressibleAnyAttribute", a);
            }
            return attribute;
        }

        private void CompileAttribute(XmlSchemaAttribute xa)
        {
            if (xa.IsProcessing)
            {
                base.SendValidationEvent("Sch_AttributeCircularRef", xa);
            }
            else if (xa.AttDef == null)
            {
                xa.IsProcessing = true;
                SchemaAttDef def = null;
                try
                {
                    if (!xa.RefName.IsEmpty)
                    {
                        XmlSchemaAttribute attribute = (XmlSchemaAttribute) this.schema.Attributes[xa.RefName];
                        if (attribute == null)
                        {
                            throw new XmlSchemaException("Sch_UndeclaredAttribute", xa.RefName.ToString(), xa);
                        }
                        this.CompileAttribute(attribute);
                        if (attribute.AttDef == null)
                        {
                            throw new XmlSchemaException("Sch_RefInvalidAttribute", xa.RefName.ToString(), xa);
                        }
                        def = attribute.AttDef.Clone();
                        if (def.Datatype != null)
                        {
                            if (attribute.FixedValue != null)
                            {
                                if (xa.DefaultValue != null)
                                {
                                    throw new XmlSchemaException("Sch_FixedDefaultInRef", xa.RefName.ToString(), xa);
                                }
                                if (xa.FixedValue != null)
                                {
                                    if (xa.FixedValue != attribute.FixedValue)
                                    {
                                        throw new XmlSchemaException("Sch_FixedInRef", xa.RefName.ToString(), xa);
                                    }
                                }
                                else
                                {
                                    def.Presence = SchemaDeclBase.Use.Fixed;
                                    def.DefaultValueRaw = def.DefaultValueExpanded = attribute.FixedValue;
                                    def.DefaultValueTyped = def.Datatype.ParseValue(def.DefaultValueRaw, base.NameTable, new SchemaNamespaceManager(xa), true);
                                }
                            }
                            else if (((attribute.DefaultValue != null) && (xa.DefaultValue == null)) && (xa.FixedValue == null))
                            {
                                def.Presence = SchemaDeclBase.Use.Default;
                                def.DefaultValueRaw = def.DefaultValueExpanded = attribute.DefaultValue;
                                def.DefaultValueTyped = def.Datatype.ParseValue(def.DefaultValueRaw, base.NameTable, new SchemaNamespaceManager(xa), true);
                            }
                        }
                        xa.SetAttributeType(attribute.AttributeSchemaType);
                    }
                    else
                    {
                        def = new SchemaAttDef(xa.QualifiedName, xa.Prefix);
                        if (xa.SchemaType != null)
                        {
                            this.CompileSimpleType(xa.SchemaType);
                            xa.SetAttributeType(xa.SchemaType);
                            def.SchemaType = xa.SchemaType;
                            def.Datatype = xa.SchemaType.Datatype;
                        }
                        else if (!xa.SchemaTypeName.IsEmpty)
                        {
                            XmlSchemaSimpleType simpleType = this.GetSimpleType(xa.SchemaTypeName);
                            if (simpleType == null)
                            {
                                throw new XmlSchemaException("Sch_UndeclaredSimpleType", xa.SchemaTypeName.ToString(), xa);
                            }
                            xa.SetAttributeType(simpleType);
                            def.Datatype = simpleType.Datatype;
                            def.SchemaType = simpleType;
                        }
                        else
                        {
                            def.SchemaType = DatatypeImplementation.AnySimpleType;
                            def.Datatype = DatatypeImplementation.AnySimpleType.Datatype;
                            xa.SetAttributeType(DatatypeImplementation.AnySimpleType);
                        }
                    }
                    if (def.Datatype != null)
                    {
                        def.Datatype.VerifySchemaValid(this.schema.Notations, xa);
                    }
                    if ((xa.DefaultValue != null) || (xa.FixedValue != null))
                    {
                        if (xa.DefaultValue != null)
                        {
                            def.Presence = SchemaDeclBase.Use.Default;
                            def.DefaultValueRaw = def.DefaultValueExpanded = xa.DefaultValue;
                        }
                        else
                        {
                            def.Presence = SchemaDeclBase.Use.Fixed;
                            def.DefaultValueRaw = def.DefaultValueExpanded = xa.FixedValue;
                        }
                        if (def.Datatype != null)
                        {
                            def.DefaultValueTyped = def.Datatype.ParseValue(def.DefaultValueRaw, base.NameTable, new SchemaNamespaceManager(xa), true);
                        }
                    }
                    else
                    {
                        switch (xa.Use)
                        {
                            case XmlSchemaUse.None:
                            case XmlSchemaUse.Optional:
                                def.Presence = SchemaDeclBase.Use.Implied;
                                break;

                            case XmlSchemaUse.Required:
                                def.Presence = SchemaDeclBase.Use.Required;
                                break;
                        }
                    }
                    def.SchemaAttribute = xa;
                    xa.AttDef = def;
                }
                catch (XmlSchemaException exception)
                {
                    if (exception.SourceSchemaObject == null)
                    {
                        exception.SetSource(xa);
                    }
                    base.SendValidationEvent(exception);
                    xa.AttDef = SchemaAttDef.Empty;
                }
                finally
                {
                    xa.IsProcessing = false;
                }
            }
        }

        private void CompileAttributeGroup(XmlSchemaAttributeGroup attributeGroup)
        {
            if (attributeGroup.IsProcessing)
            {
                base.SendValidationEvent("Sch_AttributeGroupCircularRef", attributeGroup);
            }
            else if (attributeGroup.AttributeUses.Count <= 0)
            {
                attributeGroup.IsProcessing = true;
                XmlSchemaAnyAttribute anyAttribute = attributeGroup.AnyAttribute;
                foreach (XmlSchemaObject obj2 in attributeGroup.Attributes)
                {
                    if (obj2 is XmlSchemaAttribute)
                    {
                        XmlSchemaAttribute xa = (XmlSchemaAttribute) obj2;
                        if (xa.Use != XmlSchemaUse.Prohibited)
                        {
                            this.CompileAttribute(xa);
                        }
                        if (attributeGroup.AttributeUses[xa.QualifiedName] == null)
                        {
                            attributeGroup.AttributeUses.Add(xa.QualifiedName, xa);
                        }
                        else
                        {
                            base.SendValidationEvent("Sch_DupAttributeUse", xa.QualifiedName.ToString(), xa);
                        }
                    }
                    else
                    {
                        XmlSchemaAttributeGroup redefined;
                        XmlSchemaAttributeGroupRef source = (XmlSchemaAttributeGroupRef) obj2;
                        if ((attributeGroup.Redefined != null) && (source.RefName == attributeGroup.Redefined.QualifiedName))
                        {
                            redefined = attributeGroup.Redefined;
                        }
                        else
                        {
                            redefined = (XmlSchemaAttributeGroup) this.schema.AttributeGroups[source.RefName];
                        }
                        if (redefined != null)
                        {
                            this.CompileAttributeGroup(redefined);
                            foreach (XmlSchemaAttribute attribute3 in redefined.AttributeUses.Values)
                            {
                                if (attributeGroup.AttributeUses[attribute3.QualifiedName] == null)
                                {
                                    attributeGroup.AttributeUses.Add(attribute3.QualifiedName, attribute3);
                                }
                                else
                                {
                                    base.SendValidationEvent("Sch_DupAttributeUse", attribute3.QualifiedName.ToString(), attribute3);
                                }
                            }
                            anyAttribute = this.CompileAnyAttributeIntersection(anyAttribute, redefined.AttributeWildcard);
                        }
                        else
                        {
                            base.SendValidationEvent("Sch_UndefAttributeGroupRef", source.RefName.ToString(), source);
                        }
                    }
                }
                attributeGroup.AttributeWildcard = anyAttribute;
                attributeGroup.IsProcessing = false;
            }
        }

        private XmlSchemaSimpleType[] CompileBaseMemberTypes(XmlSchemaSimpleType simpleType)
        {
            ArrayList memberTypeDefinitions = new ArrayList();
            XmlSchemaSimpleTypeUnion content = (XmlSchemaSimpleTypeUnion) simpleType.Content;
            Array memberTypes = content.MemberTypes;
            if (memberTypes != null)
            {
                foreach (XmlQualifiedName name in memberTypes)
                {
                    XmlSchemaSimpleType unionMember = this.GetSimpleType(name);
                    if (unionMember == null)
                    {
                        throw new XmlSchemaException("Sch_UndeclaredSimpleType", name.ToString(), simpleType);
                    }
                    if (unionMember.Datatype.Variety == XmlSchemaDatatypeVariety.Union)
                    {
                        this.CheckUnionType(unionMember, memberTypeDefinitions, simpleType);
                    }
                    else
                    {
                        memberTypeDefinitions.Add(unionMember);
                    }
                    if ((unionMember.FinalResolved & XmlSchemaDerivationMethod.Union) != XmlSchemaDerivationMethod.Empty)
                    {
                        base.SendValidationEvent("Sch_BaseFinalUnion", simpleType);
                    }
                }
            }
            XmlSchemaObjectCollection baseTypes = content.BaseTypes;
            if (baseTypes != null)
            {
                foreach (XmlSchemaSimpleType type2 in baseTypes)
                {
                    this.CompileSimpleType(type2);
                    if (type2.Datatype.Variety == XmlSchemaDatatypeVariety.Union)
                    {
                        this.CheckUnionType(type2, memberTypeDefinitions, simpleType);
                    }
                    else
                    {
                        memberTypeDefinitions.Add(type2);
                    }
                }
            }
            content.SetBaseMemberTypes(memberTypeDefinitions.ToArray(typeof(XmlSchemaSimpleType)) as XmlSchemaSimpleType[]);
            return content.BaseMemberTypes;
        }

        private void CompileCompexTypeElements(XmlSchemaComplexType complexType)
        {
            if (complexType.IsProcessing)
            {
                base.SendValidationEvent("Sch_TypeCircularRef", complexType);
            }
            else
            {
                complexType.IsProcessing = true;
                if (complexType.ContentTypeParticle != XmlSchemaParticle.Empty)
                {
                    this.CompileParticleElements(complexType, complexType.ContentTypeParticle);
                }
                complexType.IsProcessing = false;
            }
        }

        private ContentValidator CompileComplexContent(XmlSchemaComplexType complexType)
        {
            if (complexType.ContentType == XmlSchemaContentType.Empty)
            {
                return ContentValidator.Empty;
            }
            if (complexType.ContentType == XmlSchemaContentType.TextOnly)
            {
                return ContentValidator.TextOnly;
            }
            XmlSchemaParticle contentTypeParticle = complexType.ContentTypeParticle;
            if ((contentTypeParticle == null) || (contentTypeParticle == XmlSchemaParticle.Empty))
            {
                if (complexType.ContentType == XmlSchemaContentType.ElementOnly)
                {
                    return ContentValidator.Empty;
                }
                return ContentValidator.Mixed;
            }
            this.PushComplexType(complexType);
            if (contentTypeParticle is XmlSchemaAll)
            {
                XmlSchemaAll all = (XmlSchemaAll) contentTypeParticle;
                AllElementsContentValidator validator = new AllElementsContentValidator(complexType.ContentType, all.Items.Count, all.MinOccurs == 0M);
                foreach (XmlSchemaElement element in all.Items)
                {
                    if (!validator.AddElement(element.QualifiedName, element, element.MinOccurs == 0M))
                    {
                        base.SendValidationEvent("Sch_DupElement", element.QualifiedName.ToString(), element);
                    }
                }
                return validator;
            }
            ParticleContentValidator contentValidator = new ParticleContentValidator(complexType.ContentType);
            try
            {
                contentValidator.Start();
                this.BuildParticleContentModel(contentValidator, contentTypeParticle);
                return contentValidator.Finish(this.compileContentModel);
            }
            catch (UpaException exception)
            {
                if (exception.Particle1 is XmlSchemaElement)
                {
                    if (exception.Particle2 is XmlSchemaElement)
                    {
                        base.SendValidationEvent("Sch_NonDeterministic", ((XmlSchemaElement) exception.Particle1).QualifiedName.ToString(), (XmlSchemaElement) exception.Particle2);
                    }
                    else
                    {
                        base.SendValidationEvent("Sch_NonDeterministicAnyEx", ((XmlSchemaAny) exception.Particle2).NamespaceList.ToString(), ((XmlSchemaElement) exception.Particle1).QualifiedName.ToString(), (XmlSchemaAny) exception.Particle2);
                    }
                }
                else if (exception.Particle2 is XmlSchemaElement)
                {
                    base.SendValidationEvent("Sch_NonDeterministicAnyEx", ((XmlSchemaAny) exception.Particle1).NamespaceList.ToString(), ((XmlSchemaElement) exception.Particle2).QualifiedName.ToString(), (XmlSchemaAny) exception.Particle1);
                }
                else
                {
                    base.SendValidationEvent("Sch_NonDeterministicAnyAny", ((XmlSchemaAny) exception.Particle1).NamespaceList.ToString(), ((XmlSchemaAny) exception.Particle2).NamespaceList.ToString(), (XmlSchemaAny) exception.Particle1);
                }
                return XmlSchemaComplexType.AnyTypeContentValidator;
            }
            catch (NotSupportedException)
            {
                base.SendValidationEvent("Sch_ComplexContentModel", complexType, XmlSeverityType.Warning);
                return XmlSchemaComplexType.AnyTypeContentValidator;
            }
        }

        private void CompileComplexContentExtension(XmlSchemaComplexType complexType, XmlSchemaComplexContent complexContent, XmlSchemaComplexContentExtension complexExtension)
        {
            XmlSchemaComplexType redefined = null;
            if ((complexType.Redefined != null) && (complexExtension.BaseTypeName == complexType.Redefined.QualifiedName))
            {
                redefined = (XmlSchemaComplexType) complexType.Redefined;
                this.CompileComplexType(redefined);
            }
            else
            {
                redefined = this.GetComplexType(complexExtension.BaseTypeName);
                if (redefined == null)
                {
                    base.SendValidationEvent("Sch_UndefBaseExtension", complexExtension.BaseTypeName.ToString(), complexExtension);
                    return;
                }
            }
            if (((redefined != null) && (redefined.ElementDecl != null)) && (redefined.ContentType == XmlSchemaContentType.TextOnly))
            {
                base.SendValidationEvent("Sch_NotComplexContent", complexType);
            }
            else
            {
                complexType.SetBaseSchemaType(redefined);
                if ((redefined.FinalResolved & XmlSchemaDerivationMethod.Extension) != XmlSchemaDerivationMethod.Empty)
                {
                    base.SendValidationEvent("Sch_BaseFinalExtension", complexType);
                }
                this.CompileLocalAttributes(redefined, complexType, complexExtension.Attributes, complexExtension.AnyAttribute, XmlSchemaDerivationMethod.Extension);
                XmlSchemaParticle contentTypeParticle = redefined.ContentTypeParticle;
                XmlSchemaParticle item = this.CannonicalizeParticle(complexExtension.Particle, true, true);
                if (contentTypeParticle != XmlSchemaParticle.Empty)
                {
                    if (item != XmlSchemaParticle.Empty)
                    {
                        XmlSchemaSequence particle = new XmlSchemaSequence();
                        particle.Items.Add(contentTypeParticle);
                        particle.Items.Add(item);
                        complexType.SetContentTypeParticle(this.CompileContentTypeParticle(particle, false));
                    }
                    else
                    {
                        complexType.SetContentTypeParticle(contentTypeParticle);
                    }
                    XmlSchemaContentType contentType = this.GetSchemaContentType(complexType, complexContent, item);
                    if (contentType == XmlSchemaContentType.Empty)
                    {
                        contentType = redefined.ContentType;
                    }
                    complexType.SetContentType(contentType);
                    if (complexType.ContentType != redefined.ContentType)
                    {
                        base.SendValidationEvent("Sch_DifContentType", complexType);
                    }
                }
                else
                {
                    complexType.SetContentTypeParticle(item);
                    complexType.SetContentType(this.GetSchemaContentType(complexType, complexContent, complexType.ContentTypeParticle));
                }
                complexType.SetDerivedBy(XmlSchemaDerivationMethod.Extension);
            }
        }

        private void CompileComplexContentRestriction(XmlSchemaComplexType complexType, XmlSchemaComplexContent complexContent, XmlSchemaComplexContentRestriction complexRestriction)
        {
            XmlSchemaComplexType redefined = null;
            if ((complexType.Redefined != null) && (complexRestriction.BaseTypeName == complexType.Redefined.QualifiedName))
            {
                redefined = (XmlSchemaComplexType) complexType.Redefined;
                this.CompileComplexType(redefined);
            }
            else
            {
                redefined = this.GetComplexType(complexRestriction.BaseTypeName);
                if (redefined == null)
                {
                    base.SendValidationEvent("Sch_UndefBaseRestriction", complexRestriction.BaseTypeName.ToString(), complexRestriction);
                    return;
                }
            }
            if (((redefined != null) && (redefined.ElementDecl != null)) && (redefined.ContentType == XmlSchemaContentType.TextOnly))
            {
                base.SendValidationEvent("Sch_NotComplexContent", complexType);
            }
            else
            {
                complexType.SetBaseSchemaType(redefined);
                if ((redefined.FinalResolved & XmlSchemaDerivationMethod.Restriction) != XmlSchemaDerivationMethod.Empty)
                {
                    base.SendValidationEvent("Sch_BaseFinalRestriction", complexType);
                }
                this.CompileLocalAttributes(redefined, complexType, complexRestriction.Attributes, complexRestriction.AnyAttribute, XmlSchemaDerivationMethod.Restriction);
                complexType.SetContentTypeParticle(this.CompileContentTypeParticle(complexRestriction.Particle, true));
                complexType.SetContentType(this.GetSchemaContentType(complexType, complexContent, complexType.ContentTypeParticle));
                if (complexType.ContentType == XmlSchemaContentType.Empty)
                {
                    SchemaElementDecl elementDecl = redefined.ElementDecl;
                    if ((redefined.ElementDecl != null) && !redefined.ElementDecl.ContentValidator.IsEmptiable)
                    {
                        base.SendValidationEvent("Sch_InvalidContentRestriction", complexType);
                    }
                }
                complexType.SetDerivedBy(XmlSchemaDerivationMethod.Restriction);
            }
        }

        private void CompileComplexType(XmlSchemaComplexType complexType)
        {
            if (complexType.ElementDecl == null)
            {
                if (complexType.IsProcessing)
                {
                    base.SendValidationEvent("Sch_TypeCircularRef", complexType);
                }
                else
                {
                    complexType.IsProcessing = true;
                    if (complexType.ContentModel != null)
                    {
                        if (complexType.ContentModel is XmlSchemaSimpleContent)
                        {
                            XmlSchemaSimpleContent contentModel = (XmlSchemaSimpleContent) complexType.ContentModel;
                            complexType.SetContentType(XmlSchemaContentType.TextOnly);
                            if (contentModel.Content is XmlSchemaSimpleContentExtension)
                            {
                                this.CompileSimpleContentExtension(complexType, (XmlSchemaSimpleContentExtension) contentModel.Content);
                            }
                            else
                            {
                                this.CompileSimpleContentRestriction(complexType, (XmlSchemaSimpleContentRestriction) contentModel.Content);
                            }
                        }
                        else
                        {
                            XmlSchemaComplexContent complexContent = (XmlSchemaComplexContent) complexType.ContentModel;
                            if (complexContent.Content is XmlSchemaComplexContentExtension)
                            {
                                this.CompileComplexContentExtension(complexType, complexContent, (XmlSchemaComplexContentExtension) complexContent.Content);
                            }
                            else
                            {
                                this.CompileComplexContentRestriction(complexType, complexContent, (XmlSchemaComplexContentRestriction) complexContent.Content);
                            }
                        }
                    }
                    else
                    {
                        complexType.SetBaseSchemaType(XmlSchemaComplexType.AnyType);
                        this.CompileLocalAttributes(XmlSchemaComplexType.AnyType, complexType, complexType.Attributes, complexType.AnyAttribute, XmlSchemaDerivationMethod.Restriction);
                        complexType.SetDerivedBy(XmlSchemaDerivationMethod.Restriction);
                        complexType.SetContentTypeParticle(this.CompileContentTypeParticle(complexType.Particle, true));
                        complexType.SetContentType(this.GetSchemaContentType(complexType, null, complexType.ContentTypeParticle));
                    }
                    bool flag = false;
                    foreach (XmlSchemaAttribute attribute in complexType.AttributeUses.Values)
                    {
                        if (attribute.Use != XmlSchemaUse.Prohibited)
                        {
                            XmlSchemaDatatype datatype = attribute.Datatype;
                            if ((datatype != null) && (datatype.TokenizedType == XmlTokenizedType.ID))
                            {
                                if (flag)
                                {
                                    base.SendValidationEvent("Sch_TwoIdAttrUses", complexType);
                                }
                                else
                                {
                                    flag = true;
                                }
                            }
                        }
                    }
                    SchemaElementDecl decl = new SchemaElementDecl {
                        ContentValidator = this.CompileComplexContent(complexType),
                        SchemaType = complexType,
                        IsAbstract = complexType.IsAbstract,
                        Datatype = complexType.Datatype,
                        Block = complexType.BlockResolved,
                        AnyAttribute = complexType.AttributeWildcard
                    };
                    foreach (XmlSchemaAttribute attribute2 in complexType.AttributeUses.Values)
                    {
                        if (attribute2.Use == XmlSchemaUse.Prohibited)
                        {
                            if (decl.ProhibitedAttributes[attribute2.QualifiedName] == null)
                            {
                                decl.ProhibitedAttributes.Add(attribute2.QualifiedName, attribute2.QualifiedName);
                            }
                        }
                        else if (((decl.AttDefs[attribute2.QualifiedName] == null) && (attribute2.AttDef != null)) && ((attribute2.AttDef.Name != XmlQualifiedName.Empty) && (attribute2.AttDef != SchemaAttDef.Empty)))
                        {
                            decl.AddAttDef(attribute2.AttDef);
                        }
                    }
                    decl.EndAddAttDef();
                    complexType.ElementDecl = decl;
                    complexType.IsProcessing = false;
                }
            }
        }

        private XmlSchemaParticle CompileContentTypeParticle(XmlSchemaParticle particle, bool substitution)
        {
            XmlSchemaParticle particle2 = this.CannonicalizeParticle(particle, true, substitution);
            XmlSchemaChoice source = particle2 as XmlSchemaChoice;
            if ((source == null) || (source.Items.Count != 0))
            {
                return particle2;
            }
            if (source.MinOccurs != 0M)
            {
                base.SendValidationEvent("Sch_EmptyChoice", source, XmlSeverityType.Warning);
            }
            return XmlSchemaParticle.Empty;
        }

        private void CompileElement(XmlSchemaElement xe)
        {
            if (xe.IsProcessing)
            {
                base.SendValidationEvent("Sch_ElementCircularRef", xe);
            }
            else if (xe.ElementDecl == null)
            {
                xe.IsProcessing = true;
                SchemaElementDecl decl = null;
                try
                {
                    if (!xe.RefName.IsEmpty)
                    {
                        XmlSchemaElement element = (XmlSchemaElement) this.schema.Elements[xe.RefName];
                        if (element == null)
                        {
                            throw new XmlSchemaException("Sch_UndeclaredElement", xe.RefName.ToString(), xe);
                        }
                        this.CompileElement(element);
                        if (element.ElementDecl == null)
                        {
                            throw new XmlSchemaException("Sch_RefInvalidElement", xe.RefName.ToString(), xe);
                        }
                        xe.SetElementType(element.ElementSchemaType);
                        decl = element.ElementDecl.Clone();
                    }
                    else
                    {
                        if (xe.SchemaType != null)
                        {
                            xe.SetElementType(xe.SchemaType);
                        }
                        else if (!xe.SchemaTypeName.IsEmpty)
                        {
                            xe.SetElementType(this.GetAnySchemaType(xe.SchemaTypeName));
                            if (xe.ElementSchemaType == null)
                            {
                                throw new XmlSchemaException("Sch_UndeclaredType", xe.SchemaTypeName.ToString(), xe);
                            }
                        }
                        else if (!xe.SubstitutionGroup.IsEmpty)
                        {
                            XmlSchemaElement element2 = (XmlSchemaElement) this.schema.Elements[xe.SubstitutionGroup];
                            if (element2 == null)
                            {
                                throw new XmlSchemaException("Sch_UndeclaredEquivClass", xe.SubstitutionGroup.Name.ToString(CultureInfo.InvariantCulture), xe);
                            }
                            if (element2.IsProcessing)
                            {
                                return;
                            }
                            this.CompileElement(element2);
                            if (element2.ElementDecl == null)
                            {
                                xe.SetElementType(XmlSchemaComplexType.AnyType);
                                decl = XmlSchemaComplexType.AnyType.ElementDecl.Clone();
                            }
                            else
                            {
                                xe.SetElementType(element2.ElementSchemaType);
                                decl = element2.ElementDecl.Clone();
                            }
                        }
                        else
                        {
                            xe.SetElementType(XmlSchemaComplexType.AnyType);
                            decl = XmlSchemaComplexType.AnyType.ElementDecl.Clone();
                        }
                        if (decl == null)
                        {
                            if (xe.ElementSchemaType is XmlSchemaComplexType)
                            {
                                XmlSchemaComplexType complexType = (XmlSchemaComplexType) xe.ElementSchemaType;
                                this.CompileComplexType(complexType);
                                if (complexType.ElementDecl != null)
                                {
                                    decl = complexType.ElementDecl.Clone();
                                }
                            }
                            else if (xe.ElementSchemaType is XmlSchemaSimpleType)
                            {
                                XmlSchemaSimpleType simpleType = (XmlSchemaSimpleType) xe.ElementSchemaType;
                                this.CompileSimpleType(simpleType);
                                if (simpleType.ElementDecl != null)
                                {
                                    decl = simpleType.ElementDecl.Clone();
                                }
                            }
                        }
                        decl.Name = xe.QualifiedName;
                        decl.IsAbstract = xe.IsAbstract;
                        XmlSchemaComplexType elementSchemaType = xe.ElementSchemaType as XmlSchemaComplexType;
                        if (elementSchemaType != null)
                        {
                            decl.IsAbstract |= elementSchemaType.IsAbstract;
                        }
                        decl.IsNillable = xe.IsNillable;
                        decl.Block |= xe.BlockResolved;
                    }
                    if (decl.Datatype != null)
                    {
                        decl.Datatype.VerifySchemaValid(this.schema.Notations, xe);
                    }
                    if (((xe.DefaultValue != null) || (xe.FixedValue != null)) && (decl.ContentValidator != null))
                    {
                        if (decl.ContentValidator.ContentType == XmlSchemaContentType.TextOnly)
                        {
                            if (xe.DefaultValue != null)
                            {
                                decl.Presence = SchemaDeclBase.Use.Default;
                                decl.DefaultValueRaw = xe.DefaultValue;
                            }
                            else
                            {
                                decl.Presence = SchemaDeclBase.Use.Fixed;
                                decl.DefaultValueRaw = xe.FixedValue;
                            }
                            if (decl.Datatype != null)
                            {
                                decl.DefaultValueTyped = decl.Datatype.ParseValue(decl.DefaultValueRaw, base.NameTable, new SchemaNamespaceManager(xe), true);
                            }
                        }
                        else if ((decl.ContentValidator.ContentType != XmlSchemaContentType.Mixed) || !decl.ContentValidator.IsEmptiable)
                        {
                            throw new XmlSchemaException("Sch_ElementCannotHaveValue", xe);
                        }
                    }
                    if (xe.HasConstraints)
                    {
                        XmlSchemaObjectCollection constraints = xe.Constraints;
                        CompiledIdentityConstraint[] constraintArray = new CompiledIdentityConstraint[constraints.Count];
                        int num = 0;
                        foreach (XmlSchemaIdentityConstraint constraint in constraints)
                        {
                            this.CompileIdentityConstraint(constraint);
                            constraintArray[num++] = constraint.CompiledConstraint;
                        }
                        decl.Constraints = constraintArray;
                    }
                    decl.SchemaElement = xe;
                    xe.ElementDecl = decl;
                }
                catch (XmlSchemaException exception)
                {
                    if (exception.SourceSchemaObject == null)
                    {
                        exception.SetSource(xe);
                    }
                    base.SendValidationEvent(exception);
                    xe.ElementDecl = SchemaElementDecl.Empty;
                }
                finally
                {
                    xe.IsProcessing = false;
                }
            }
        }

        private void CompileGroup(XmlSchemaGroup group)
        {
            if (group.IsProcessing)
            {
                base.SendValidationEvent("Sch_GroupCircularRef", group);
                group.CanonicalParticle = XmlSchemaParticle.Empty;
            }
            else
            {
                group.IsProcessing = true;
                if (group.CanonicalParticle == null)
                {
                    group.CanonicalParticle = this.CannonicalizeParticle(group.Particle, true, true);
                }
                group.IsProcessing = false;
            }
        }

        private void CompileIdentityConstraint(XmlSchemaIdentityConstraint xi)
        {
            if (xi.IsProcessing)
            {
                xi.CompiledConstraint = CompiledIdentityConstraint.Empty;
                base.SendValidationEvent("Sch_IdentityConstraintCircularRef", xi);
            }
            else if (xi.CompiledConstraint == null)
            {
                xi.IsProcessing = true;
                CompiledIdentityConstraint constraint = null;
                try
                {
                    SchemaNamespaceManager nsmgr = new SchemaNamespaceManager(xi);
                    constraint = new CompiledIdentityConstraint(xi, nsmgr);
                    if (xi is XmlSchemaKeyref)
                    {
                        XmlSchemaIdentityConstraint constraint2 = (XmlSchemaIdentityConstraint) this.schema.IdentityConstraints[((XmlSchemaKeyref) xi).Refer];
                        if (constraint2 == null)
                        {
                            throw new XmlSchemaException("Sch_UndeclaredIdentityConstraint", ((XmlSchemaKeyref) xi).Refer.ToString(), xi);
                        }
                        this.CompileIdentityConstraint(constraint2);
                        if (constraint2.CompiledConstraint == null)
                        {
                            throw new XmlSchemaException("Sch_RefInvalidIdentityConstraint", ((XmlSchemaKeyref) xi).Refer.ToString(), xi);
                        }
                        if (constraint2.Fields.Count != xi.Fields.Count)
                        {
                            throw new XmlSchemaException("Sch_RefInvalidCardin", xi.QualifiedName.ToString(), xi);
                        }
                        if (constraint2.CompiledConstraint.Role == CompiledIdentityConstraint.ConstraintRole.Keyref)
                        {
                            throw new XmlSchemaException("Sch_ReftoKeyref", xi.QualifiedName.ToString(), xi);
                        }
                    }
                    xi.CompiledConstraint = constraint;
                }
                catch (XmlSchemaException exception)
                {
                    if (exception.SourceSchemaObject == null)
                    {
                        exception.SetSource(xi);
                    }
                    base.SendValidationEvent(exception);
                    xi.CompiledConstraint = CompiledIdentityConstraint.Empty;
                }
                finally
                {
                    xi.IsProcessing = false;
                }
            }
        }

        private void CompileLocalAttributes(XmlSchemaComplexType baseType, XmlSchemaComplexType derivedType, XmlSchemaObjectCollection attributes, XmlSchemaAnyAttribute anyAttribute, XmlSchemaDerivationMethod derivedBy)
        {
            XmlSchemaAnyAttribute attributeWildcard = baseType?.AttributeWildcard;
            foreach (XmlSchemaObject obj2 in attributes)
            {
                if (obj2 is XmlSchemaAttribute)
                {
                    XmlSchemaAttribute xa = (XmlSchemaAttribute) obj2;
                    if (xa.Use != XmlSchemaUse.Prohibited)
                    {
                        this.CompileAttribute(xa);
                    }
                    if ((xa.Use != XmlSchemaUse.Prohibited) || (((xa.Use == XmlSchemaUse.Prohibited) && (derivedBy == XmlSchemaDerivationMethod.Restriction)) && (baseType != XmlSchemaComplexType.AnyType)))
                    {
                        if (derivedType.AttributeUses[xa.QualifiedName] == null)
                        {
                            derivedType.AttributeUses.Add(xa.QualifiedName, xa);
                        }
                        else
                        {
                            base.SendValidationEvent("Sch_DupAttributeUse", xa.QualifiedName.ToString(), xa);
                        }
                    }
                    else
                    {
                        base.SendValidationEvent("Sch_AttributeIgnored", xa.QualifiedName.ToString(), xa, XmlSeverityType.Warning);
                    }
                }
                else
                {
                    XmlSchemaAttributeGroupRef source = (XmlSchemaAttributeGroupRef) obj2;
                    XmlSchemaAttributeGroup attributeGroup = (XmlSchemaAttributeGroup) this.schema.AttributeGroups[source.RefName];
                    if (attributeGroup != null)
                    {
                        this.CompileAttributeGroup(attributeGroup);
                        foreach (XmlSchemaAttribute attribute3 in attributeGroup.AttributeUses.Values)
                        {
                            if ((attribute3.Use != XmlSchemaUse.Prohibited) || (((attribute3.Use == XmlSchemaUse.Prohibited) && (derivedBy == XmlSchemaDerivationMethod.Restriction)) && (baseType != XmlSchemaComplexType.AnyType)))
                            {
                                if (derivedType.AttributeUses[attribute3.QualifiedName] == null)
                                {
                                    derivedType.AttributeUses.Add(attribute3.QualifiedName, attribute3);
                                }
                                else
                                {
                                    base.SendValidationEvent("Sch_DupAttributeUse", attribute3.QualifiedName.ToString(), source);
                                }
                            }
                            else
                            {
                                base.SendValidationEvent("Sch_AttributeIgnored", attribute3.QualifiedName.ToString(), attribute3, XmlSeverityType.Warning);
                            }
                        }
                        anyAttribute = this.CompileAnyAttributeIntersection(anyAttribute, attributeGroup.AttributeWildcard);
                    }
                    else
                    {
                        base.SendValidationEvent("Sch_UndefAttributeGroupRef", source.RefName.ToString(), source);
                    }
                }
            }
            if (baseType != null)
            {
                if (derivedBy == XmlSchemaDerivationMethod.Extension)
                {
                    derivedType.SetAttributeWildcard(this.CompileAnyAttributeUnion(anyAttribute, attributeWildcard));
                    foreach (XmlSchemaAttribute attribute4 in baseType.AttributeUses.Values)
                    {
                        XmlSchemaAttribute attribute5 = (XmlSchemaAttribute) derivedType.AttributeUses[attribute4.QualifiedName];
                        if (attribute5 != null)
                        {
                            if ((attribute5.AttributeSchemaType != attribute4.AttributeSchemaType) || (attribute4.Use == XmlSchemaUse.Prohibited))
                            {
                                base.SendValidationEvent("Sch_InvalidAttributeExtension", attribute5);
                            }
                        }
                        else
                        {
                            derivedType.AttributeUses.Add(attribute4.QualifiedName, attribute4);
                        }
                    }
                }
                else
                {
                    if ((anyAttribute != null) && ((attributeWildcard == null) || !XmlSchemaAnyAttribute.IsSubset(anyAttribute, attributeWildcard)))
                    {
                        base.SendValidationEvent("Sch_InvalidAnyAttributeRestriction", derivedType);
                    }
                    else
                    {
                        derivedType.SetAttributeWildcard(anyAttribute);
                    }
                    foreach (XmlSchemaAttribute attribute6 in baseType.AttributeUses.Values)
                    {
                        XmlSchemaAttribute attribute7 = (XmlSchemaAttribute) derivedType.AttributeUses[attribute6.QualifiedName];
                        if (attribute7 == null)
                        {
                            derivedType.AttributeUses.Add(attribute6.QualifiedName, attribute6);
                        }
                        else if ((attribute6.Use == XmlSchemaUse.Prohibited) && (attribute7.Use != XmlSchemaUse.Prohibited))
                        {
                            base.SendValidationEvent("Sch_AttributeRestrictionProhibited", attribute7);
                        }
                        else if ((attribute7.Use != XmlSchemaUse.Prohibited) && (((attribute6.AttributeSchemaType == null) || (attribute7.AttributeSchemaType == null)) || !XmlSchemaType.IsDerivedFrom(attribute7.AttributeSchemaType, attribute6.AttributeSchemaType, XmlSchemaDerivationMethod.Empty)))
                        {
                            base.SendValidationEvent("Sch_AttributeRestrictionInvalid", attribute7);
                        }
                    }
                    foreach (XmlSchemaAttribute attribute8 in derivedType.AttributeUses.Values)
                    {
                        if ((((XmlSchemaAttribute) baseType.AttributeUses[attribute8.QualifiedName]) == null) && ((attributeWildcard == null) || !attributeWildcard.Allows(attribute8.QualifiedName)))
                        {
                            base.SendValidationEvent("Sch_AttributeRestrictionInvalidFromWildcard", attribute8);
                        }
                    }
                }
            }
            else
            {
                derivedType.SetAttributeWildcard(anyAttribute);
            }
        }

        private void CompileParticleElements(XmlSchemaComplexType complexType, XmlSchemaParticle particle)
        {
            if (particle is XmlSchemaElement)
            {
                XmlSchemaElement xe = (XmlSchemaElement) particle;
                this.CompileElement(xe);
                if (complexType.LocalElements[xe.QualifiedName] == null)
                {
                    complexType.LocalElements.Add(xe.QualifiedName, xe);
                }
                else
                {
                    XmlSchemaElement element2 = (XmlSchemaElement) complexType.LocalElements[xe.QualifiedName];
                    if (element2.ElementSchemaType != xe.ElementSchemaType)
                    {
                        base.SendValidationEvent("Sch_ElementTypeCollision", particle);
                    }
                }
            }
            else if (particle is XmlSchemaGroupBase)
            {
                foreach (XmlSchemaParticle particle2 in ((XmlSchemaGroupBase) particle).Items)
                {
                    this.CompileParticleElements(complexType, particle2);
                }
            }
        }

        private void CompileSimpleContentExtension(XmlSchemaComplexType complexType, XmlSchemaSimpleContentExtension simpleExtension)
        {
            XmlSchemaComplexType redefined = null;
            if ((complexType.Redefined != null) && (simpleExtension.BaseTypeName == complexType.Redefined.QualifiedName))
            {
                redefined = (XmlSchemaComplexType) complexType.Redefined;
                this.CompileComplexType(redefined);
                complexType.SetBaseSchemaType(redefined);
                complexType.SetDatatype(redefined.Datatype);
            }
            else
            {
                XmlSchemaType anySchemaType = this.GetAnySchemaType(simpleExtension.BaseTypeName);
                if (anySchemaType == null)
                {
                    base.SendValidationEvent("Sch_UndeclaredType", simpleExtension.BaseTypeName.ToString(), complexType);
                }
                else
                {
                    complexType.SetBaseSchemaType(anySchemaType);
                    complexType.SetDatatype(anySchemaType.Datatype);
                }
                redefined = anySchemaType as XmlSchemaComplexType;
            }
            if (redefined != null)
            {
                if ((redefined.FinalResolved & XmlSchemaDerivationMethod.Extension) != XmlSchemaDerivationMethod.Empty)
                {
                    base.SendValidationEvent("Sch_BaseFinalExtension", complexType);
                }
                if (redefined.ContentType != XmlSchemaContentType.TextOnly)
                {
                    base.SendValidationEvent("Sch_NotSimpleContent", complexType);
                }
            }
            complexType.SetDerivedBy(XmlSchemaDerivationMethod.Extension);
            this.CompileLocalAttributes(redefined, complexType, simpleExtension.Attributes, simpleExtension.AnyAttribute, XmlSchemaDerivationMethod.Extension);
        }

        private void CompileSimpleContentRestriction(XmlSchemaComplexType complexType, XmlSchemaSimpleContentRestriction simpleRestriction)
        {
            XmlSchemaComplexType redefined = null;
            XmlSchemaDatatype datatype = null;
            if ((complexType.Redefined != null) && (simpleRestriction.BaseTypeName == complexType.Redefined.QualifiedName))
            {
                redefined = (XmlSchemaComplexType) complexType.Redefined;
                this.CompileComplexType(redefined);
                datatype = redefined.Datatype;
            }
            else
            {
                redefined = this.GetComplexType(simpleRestriction.BaseTypeName);
                if (redefined == null)
                {
                    base.SendValidationEvent("Sch_UndefBaseRestriction", simpleRestriction.BaseTypeName.ToString(), simpleRestriction);
                    return;
                }
                if (redefined.ContentType == XmlSchemaContentType.TextOnly)
                {
                    if (simpleRestriction.BaseType == null)
                    {
                        datatype = redefined.Datatype;
                    }
                    else
                    {
                        this.CompileSimpleType(simpleRestriction.BaseType);
                        if (!XmlSchemaType.IsDerivedFromDatatype(simpleRestriction.BaseType.Datatype, redefined.Datatype, XmlSchemaDerivationMethod.None))
                        {
                            base.SendValidationEvent("Sch_DerivedNotFromBase", simpleRestriction);
                        }
                        datatype = simpleRestriction.BaseType.Datatype;
                    }
                }
                else if ((redefined.ContentType == XmlSchemaContentType.Mixed) && redefined.ElementDecl.ContentValidator.IsEmptiable)
                {
                    if (simpleRestriction.BaseType != null)
                    {
                        this.CompileSimpleType(simpleRestriction.BaseType);
                        complexType.SetBaseSchemaType(simpleRestriction.BaseType);
                        datatype = simpleRestriction.BaseType.Datatype;
                    }
                    else
                    {
                        base.SendValidationEvent("Sch_NeedSimpleTypeChild", simpleRestriction);
                    }
                }
                else
                {
                    base.SendValidationEvent("Sch_NotSimpleContent", complexType);
                }
            }
            if (((redefined != null) && (redefined.ElementDecl != null)) && ((redefined.FinalResolved & XmlSchemaDerivationMethod.Restriction) != XmlSchemaDerivationMethod.Empty))
            {
                base.SendValidationEvent("Sch_BaseFinalRestriction", complexType);
            }
            if (redefined != null)
            {
                complexType.SetBaseSchemaType(redefined);
            }
            if (datatype != null)
            {
                try
                {
                    complexType.SetDatatype(datatype.DeriveByRestriction(simpleRestriction.Facets, base.NameTable, complexType));
                }
                catch (XmlSchemaException exception)
                {
                    if (exception.SourceSchemaObject == null)
                    {
                        exception.SetSource(complexType);
                    }
                    base.SendValidationEvent(exception);
                    complexType.SetDatatype(DatatypeImplementation.AnySimpleType.Datatype);
                }
            }
            complexType.SetDerivedBy(XmlSchemaDerivationMethod.Restriction);
            this.CompileLocalAttributes(redefined, complexType, simpleRestriction.Attributes, simpleRestriction.AnyAttribute, XmlSchemaDerivationMethod.Restriction);
        }

        private void CompileSimpleType(XmlSchemaSimpleType simpleType)
        {
            if (simpleType.IsProcessing)
            {
                throw new XmlSchemaException("Sch_TypeCircularRef", simpleType);
            }
            if (simpleType.ElementDecl == null)
            {
                simpleType.IsProcessing = true;
                try
                {
                    if (simpleType.Content is XmlSchemaSimpleTypeList)
                    {
                        XmlSchemaDatatype datatype;
                        XmlSchemaSimpleTypeList content = (XmlSchemaSimpleTypeList) simpleType.Content;
                        simpleType.SetBaseSchemaType(DatatypeImplementation.AnySimpleType);
                        if (content.ItemTypeName.IsEmpty)
                        {
                            this.CompileSimpleType(content.ItemType);
                            content.BaseItemType = content.ItemType;
                            datatype = content.ItemType.Datatype;
                        }
                        else
                        {
                            XmlSchemaSimpleType type = this.GetSimpleType(content.ItemTypeName);
                            if (type == null)
                            {
                                throw new XmlSchemaException("Sch_UndeclaredSimpleType", content.ItemTypeName.ToString(), simpleType);
                            }
                            if ((type.FinalResolved & XmlSchemaDerivationMethod.List) != XmlSchemaDerivationMethod.Empty)
                            {
                                base.SendValidationEvent("Sch_BaseFinalList", simpleType);
                            }
                            content.BaseItemType = type;
                            datatype = type.Datatype;
                        }
                        simpleType.SetDatatype(datatype.DeriveByList(simpleType));
                        simpleType.SetDerivedBy(XmlSchemaDerivationMethod.List);
                    }
                    else if (simpleType.Content is XmlSchemaSimpleTypeRestriction)
                    {
                        XmlSchemaDatatype datatype2;
                        XmlSchemaSimpleTypeRestriction restriction = (XmlSchemaSimpleTypeRestriction) simpleType.Content;
                        if (restriction.BaseTypeName.IsEmpty)
                        {
                            this.CompileSimpleType(restriction.BaseType);
                            simpleType.SetBaseSchemaType(restriction.BaseType);
                            datatype2 = restriction.BaseType.Datatype;
                        }
                        else if ((simpleType.Redefined != null) && (restriction.BaseTypeName == simpleType.Redefined.QualifiedName))
                        {
                            this.CompileSimpleType((XmlSchemaSimpleType) simpleType.Redefined);
                            simpleType.SetBaseSchemaType(simpleType.Redefined.BaseXmlSchemaType);
                            datatype2 = simpleType.Redefined.Datatype;
                        }
                        else
                        {
                            if (restriction.BaseTypeName.Equals(DatatypeImplementation.QnAnySimpleType))
                            {
                                throw new XmlSchemaException("Sch_InvalidSimpleTypeRestriction", restriction.BaseTypeName.ToString(), simpleType);
                            }
                            XmlSchemaSimpleType type2 = this.GetSimpleType(restriction.BaseTypeName);
                            if (type2 == null)
                            {
                                throw new XmlSchemaException("Sch_UndeclaredSimpleType", restriction.BaseTypeName.ToString(), simpleType);
                            }
                            if ((type2.FinalResolved & XmlSchemaDerivationMethod.Restriction) != XmlSchemaDerivationMethod.Empty)
                            {
                                base.SendValidationEvent("Sch_BaseFinalRestriction", simpleType);
                            }
                            simpleType.SetBaseSchemaType(type2);
                            datatype2 = type2.Datatype;
                        }
                        simpleType.SetDatatype(datatype2.DeriveByRestriction(restriction.Facets, base.NameTable, simpleType));
                        simpleType.SetDerivedBy(XmlSchemaDerivationMethod.Restriction);
                    }
                    else
                    {
                        XmlSchemaSimpleType[] types = this.CompileBaseMemberTypes(simpleType);
                        simpleType.SetBaseSchemaType(DatatypeImplementation.AnySimpleType);
                        simpleType.SetDatatype(XmlSchemaDatatype.DeriveByUnion(types, simpleType));
                        simpleType.SetDerivedBy(XmlSchemaDerivationMethod.Union);
                    }
                }
                catch (XmlSchemaException exception)
                {
                    if (exception.SourceSchemaObject == null)
                    {
                        exception.SetSource(simpleType);
                    }
                    base.SendValidationEvent(exception);
                    simpleType.SetDatatype(DatatypeImplementation.AnySimpleType.Datatype);
                }
                finally
                {
                    SchemaElementDecl decl = new SchemaElementDecl {
                        ContentValidator = ContentValidator.TextOnly,
                        SchemaType = simpleType,
                        Datatype = simpleType.Datatype
                    };
                    simpleType.ElementDecl = decl;
                    simpleType.IsProcessing = false;
                }
            }
        }

        private void CompileSubstitutionGroup(XmlSchemaSubstitutionGroupV1Compat substitutionGroup)
        {
            if (substitutionGroup.IsProcessing)
            {
                foreach (XmlSchemaElement element in substitutionGroup.Members)
                {
                    base.SendValidationEvent("Sch_SubstitutionCircularRef", element);
                    return;
                }
            }
            XmlSchemaElement item = (XmlSchemaElement) this.schema.Elements[substitutionGroup.Examplar];
            if (!substitutionGroup.Members.Contains(item))
            {
                substitutionGroup.IsProcessing = true;
                if (item != null)
                {
                    if (item.FinalResolved == XmlSchemaDerivationMethod.All)
                    {
                        base.SendValidationEvent("Sch_InvalidExamplar", item);
                    }
                    foreach (XmlSchemaElement element3 in substitutionGroup.Members)
                    {
                        XmlSchemaSubstitutionGroupV1Compat compat = (XmlSchemaSubstitutionGroupV1Compat) this.examplars[element3.QualifiedName];
                        if (compat != null)
                        {
                            this.CompileSubstitutionGroup(compat);
                            foreach (XmlSchemaElement element4 in compat.Choice.Items)
                            {
                                substitutionGroup.Choice.Items.Add(element4);
                            }
                        }
                        else
                        {
                            substitutionGroup.Choice.Items.Add(element3);
                        }
                    }
                    substitutionGroup.Choice.Items.Add(item);
                    substitutionGroup.Members.Add(item);
                }
                else
                {
                    foreach (XmlSchemaElement element5 in substitutionGroup.Members)
                    {
                        base.SendValidationEvent("Sch_NoExamplar", element5);
                        break;
                    }
                }
            }
            else
            {
                return;
            }
            substitutionGroup.IsProcessing = false;
        }

        public bool Execute(XmlSchema schema, SchemaInfo schemaInfo, bool compileContentModel)
        {
            this.compileContentModel = compileContentModel;
            this.schema = schema;
            this.Prepare();
            this.Cleanup();
            this.Compile();
            if (!base.HasErrors)
            {
                this.Output(schemaInfo);
            }
            return !base.HasErrors;
        }

        private XmlSchemaType GetAnySchemaType(XmlQualifiedName name)
        {
            XmlSchemaType type = (XmlSchemaType) this.schema.SchemaTypes[name];
            if (type == null)
            {
                return DatatypeImplementation.GetSimpleTypeFromXsdType(name);
            }
            if (type is XmlSchemaComplexType)
            {
                this.CompileComplexType((XmlSchemaComplexType) type);
                return type;
            }
            this.CompileSimpleType((XmlSchemaSimpleType) type);
            return type;
        }

        private XmlSchemaComplexType GetComplexType(XmlQualifiedName name)
        {
            XmlSchemaComplexType complexType = this.schema.SchemaTypes[name] as XmlSchemaComplexType;
            if (complexType != null)
            {
                this.CompileComplexType(complexType);
            }
            return complexType;
        }

        private int GetMappingParticle(XmlSchemaParticle particle, XmlSchemaObjectCollection collection)
        {
            for (int i = 0; i < collection.Count; i++)
            {
                if (this.IsValidRestriction(particle, (XmlSchemaParticle) collection[i]))
                {
                    return i;
                }
            }
            return -1;
        }

        private XmlSchemaContentType GetSchemaContentType(XmlSchemaComplexType complexType, XmlSchemaComplexContent complexContent, XmlSchemaParticle particle)
        {
            if (((complexContent != null) && complexContent.IsMixed) || ((complexContent == null) && complexType.IsMixed))
            {
                return XmlSchemaContentType.Mixed;
            }
            if ((particle != null) && !particle.IsEmpty)
            {
                return XmlSchemaContentType.ElementOnly;
            }
            return XmlSchemaContentType.Empty;
        }

        private XmlSchemaSimpleType GetSimpleType(XmlQualifiedName name)
        {
            XmlSchemaSimpleType simpleType = this.schema.SchemaTypes[name] as XmlSchemaSimpleType;
            if (simpleType != null)
            {
                this.CompileSimpleType(simpleType);
                return simpleType;
            }
            simpleType = DatatypeImplementation.GetSimpleTypeFromXsdType(name);
            if (simpleType != null)
            {
                if (simpleType.TypeCode == XmlTypeCode.NormalizedString)
                {
                    return DatatypeImplementation.GetNormalizedStringTypeV1Compat();
                }
                if (simpleType.TypeCode == XmlTypeCode.Token)
                {
                    simpleType = DatatypeImplementation.GetTokenTypeV1Compat();
                }
            }
            return simpleType;
        }

        private bool IsAnyFromAny(XmlSchemaAny derivedAny, XmlSchemaAny baseAny) => 
            (this.IsValidOccurrenceRangeRestriction(derivedAny, baseAny) && NamespaceList.IsSubset(derivedAny.NamespaceList, baseAny.NamespaceList));

        private bool IsElementFromAny(XmlSchemaElement derivedElement, XmlSchemaAny baseAny) => 
            (baseAny.Allows(derivedElement.QualifiedName) && this.IsValidOccurrenceRangeRestriction(derivedElement, baseAny));

        private bool IsElementFromElement(XmlSchemaElement derivedElement, XmlSchemaElement baseElement) => 
            ((((((derivedElement.QualifiedName == baseElement.QualifiedName) && (derivedElement.IsNillable == baseElement.IsNillable)) && this.IsValidOccurrenceRangeRestriction(derivedElement, baseElement)) && ((baseElement.FixedValue == null) || (baseElement.FixedValue == derivedElement.FixedValue))) && ((((derivedElement.BlockResolved | baseElement.BlockResolved) == derivedElement.BlockResolved) && (derivedElement.ElementSchemaType != null)) && (baseElement.ElementSchemaType != null))) && XmlSchemaType.IsDerivedFrom(derivedElement.ElementSchemaType, baseElement.ElementSchemaType, ~XmlSchemaDerivationMethod.Restriction));

        private bool IsElementFromGroupBase(XmlSchemaElement derivedElement, XmlSchemaGroupBase baseGroupBase, bool skipEmptableOnly)
        {
            bool flag = false;
            foreach (XmlSchemaParticle particle in baseGroupBase.Items)
            {
                if (!flag)
                {
                    string minOccursString = particle.MinOccursString;
                    string maxOccursString = particle.MaxOccursString;
                    particle.MinOccurs *= baseGroupBase.MinOccurs;
                    if (particle.MaxOccurs != 79228162514264337593543950335M)
                    {
                        if (baseGroupBase.MaxOccurs == 79228162514264337593543950335M)
                        {
                            particle.MaxOccurs = 79228162514264337593543950335M;
                        }
                        else
                        {
                            particle.MaxOccurs *= baseGroupBase.MaxOccurs;
                        }
                    }
                    flag = this.IsValidRestriction(derivedElement, particle);
                    particle.MinOccursString = minOccursString;
                    particle.MaxOccursString = maxOccursString;
                }
                else if (skipEmptableOnly && !this.IsParticleEmptiable(particle))
                {
                    return false;
                }
            }
            return flag;
        }

        private bool IsGroupBaseFromAny(XmlSchemaGroupBase derivedGroupBase, XmlSchemaAny baseAny)
        {
            decimal num;
            decimal num2;
            this.CalculateEffectiveTotalRange(derivedGroupBase, out num, out num2);
            if (!this.IsValidOccurrenceRangeRestriction(num, num2, baseAny.MinOccurs, baseAny.MaxOccurs))
            {
                return false;
            }
            string minOccursString = baseAny.MinOccursString;
            baseAny.MinOccurs = 0M;
            foreach (XmlSchemaParticle particle in derivedGroupBase.Items)
            {
                if (!this.IsValidRestriction(particle, baseAny))
                {
                    baseAny.MinOccursString = minOccursString;
                    return false;
                }
            }
            baseAny.MinOccursString = minOccursString;
            return true;
        }

        private bool IsGroupBaseFromGroupBase(XmlSchemaGroupBase derivedGroupBase, XmlSchemaGroupBase baseGroupBase, bool skipEmptableOnly)
        {
            if (!this.IsValidOccurrenceRangeRestriction(derivedGroupBase, baseGroupBase) || (derivedGroupBase.Items.Count > baseGroupBase.Items.Count))
            {
                return false;
            }
            int num = 0;
            foreach (XmlSchemaParticle particle in baseGroupBase.Items)
            {
                if ((num < derivedGroupBase.Items.Count) && this.IsValidRestriction((XmlSchemaParticle) derivedGroupBase.Items[num], particle))
                {
                    num++;
                }
                else if (skipEmptableOnly && !this.IsParticleEmptiable(particle))
                {
                    return false;
                }
            }
            if (num < derivedGroupBase.Items.Count)
            {
                return false;
            }
            return true;
        }

        private bool IsParticleEmptiable(XmlSchemaParticle particle)
        {
            decimal num;
            decimal num2;
            this.CalculateEffectiveTotalRange(particle, out num, out num2);
            return (num == 0M);
        }

        private bool IsSequenceFromAll(XmlSchemaSequence derivedSequence, XmlSchemaAll baseAll)
        {
            if (!this.IsValidOccurrenceRangeRestriction(derivedSequence, baseAll) || (derivedSequence.Items.Count > baseAll.Items.Count))
            {
                return false;
            }
            BitSet set = new BitSet(baseAll.Items.Count);
            foreach (XmlSchemaParticle particle in derivedSequence.Items)
            {
                int mappingParticle = this.GetMappingParticle(particle, baseAll.Items);
                if (mappingParticle >= 0)
                {
                    if (set[mappingParticle])
                    {
                        return false;
                    }
                    set.Set(mappingParticle);
                }
                else
                {
                    return false;
                }
            }
            for (int i = 0; i < baseAll.Items.Count; i++)
            {
                if (!set[i] && !this.IsParticleEmptiable((XmlSchemaParticle) baseAll.Items[i]))
                {
                    return false;
                }
            }
            return true;
        }

        private bool IsSequenceFromChoice(XmlSchemaSequence derivedSequence, XmlSchemaChoice baseChoice)
        {
            decimal num;
            decimal num2;
            this.CalculateSequenceRange(derivedSequence, out num, out num2);
            if (!this.IsValidOccurrenceRangeRestriction(num, num2, baseChoice.MinOccurs, baseChoice.MaxOccurs) || (derivedSequence.Items.Count > baseChoice.Items.Count))
            {
                return false;
            }
            foreach (XmlSchemaParticle particle in derivedSequence.Items)
            {
                if (this.GetMappingParticle(particle, baseChoice.Items) < 0)
                {
                    return false;
                }
            }
            return true;
        }

        private bool IsValidOccurrenceRangeRestriction(XmlSchemaParticle derivedParticle, XmlSchemaParticle baseParticle) => 
            this.IsValidOccurrenceRangeRestriction(derivedParticle.MinOccurs, derivedParticle.MaxOccurs, baseParticle.MinOccurs, baseParticle.MaxOccurs);

        private bool IsValidOccurrenceRangeRestriction(decimal minOccurs, decimal maxOccurs, decimal baseMinOccurs, decimal baseMaxOccurs) => 
            ((baseMinOccurs <= minOccurs) && (maxOccurs <= baseMaxOccurs));

        private bool IsValidRestriction(XmlSchemaParticle derivedParticle, XmlSchemaParticle baseParticle)
        {
            if (derivedParticle == baseParticle)
            {
                return true;
            }
            if ((derivedParticle == null) || (derivedParticle == XmlSchemaParticle.Empty))
            {
                return this.IsParticleEmptiable(baseParticle);
            }
            if ((baseParticle != null) && (baseParticle != XmlSchemaParticle.Empty))
            {
                if (baseParticle is XmlSchemaElement)
                {
                    return ((derivedParticle is XmlSchemaElement) && this.IsElementFromElement((XmlSchemaElement) derivedParticle, (XmlSchemaElement) baseParticle));
                }
                if (baseParticle is XmlSchemaAny)
                {
                    if (derivedParticle is XmlSchemaElement)
                    {
                        return this.IsElementFromAny((XmlSchemaElement) derivedParticle, (XmlSchemaAny) baseParticle);
                    }
                    if (derivedParticle is XmlSchemaAny)
                    {
                        return this.IsAnyFromAny((XmlSchemaAny) derivedParticle, (XmlSchemaAny) baseParticle);
                    }
                    return this.IsGroupBaseFromAny((XmlSchemaGroupBase) derivedParticle, (XmlSchemaAny) baseParticle);
                }
                if (baseParticle is XmlSchemaAll)
                {
                    if (derivedParticle is XmlSchemaElement)
                    {
                        return this.IsElementFromGroupBase((XmlSchemaElement) derivedParticle, (XmlSchemaGroupBase) baseParticle, true);
                    }
                    if (derivedParticle is XmlSchemaAll)
                    {
                        return this.IsGroupBaseFromGroupBase((XmlSchemaGroupBase) derivedParticle, (XmlSchemaGroupBase) baseParticle, true);
                    }
                    if (derivedParticle is XmlSchemaSequence)
                    {
                        return this.IsSequenceFromAll((XmlSchemaSequence) derivedParticle, (XmlSchemaAll) baseParticle);
                    }
                }
                else if (baseParticle is XmlSchemaChoice)
                {
                    if (derivedParticle is XmlSchemaElement)
                    {
                        return this.IsElementFromGroupBase((XmlSchemaElement) derivedParticle, (XmlSchemaGroupBase) baseParticle, false);
                    }
                    if (derivedParticle is XmlSchemaChoice)
                    {
                        return this.IsGroupBaseFromGroupBase((XmlSchemaGroupBase) derivedParticle, (XmlSchemaGroupBase) baseParticle, false);
                    }
                    if (derivedParticle is XmlSchemaSequence)
                    {
                        return this.IsSequenceFromChoice((XmlSchemaSequence) derivedParticle, (XmlSchemaChoice) baseParticle);
                    }
                }
                else if (baseParticle is XmlSchemaSequence)
                {
                    if (derivedParticle is XmlSchemaElement)
                    {
                        return this.IsElementFromGroupBase((XmlSchemaElement) derivedParticle, (XmlSchemaGroupBase) baseParticle, true);
                    }
                    if (derivedParticle is XmlSchemaSequence)
                    {
                        return this.IsGroupBaseFromGroupBase((XmlSchemaGroupBase) derivedParticle, (XmlSchemaGroupBase) baseParticle, true);
                    }
                }
            }
            return false;
        }

        private void Output(SchemaInfo schemaInfo)
        {
            foreach (XmlSchemaElement element in this.schema.Elements.Values)
            {
                schemaInfo.TargetNamespaces[element.QualifiedName.Namespace] = true;
                schemaInfo.ElementDecls.Add(element.QualifiedName, element.ElementDecl);
            }
            foreach (XmlSchemaAttribute attribute in this.schema.Attributes.Values)
            {
                schemaInfo.TargetNamespaces[attribute.QualifiedName.Namespace] = true;
                schemaInfo.AttributeDecls.Add(attribute.QualifiedName, attribute.AttDef);
            }
            foreach (XmlSchemaType type in this.schema.SchemaTypes.Values)
            {
                schemaInfo.TargetNamespaces[type.QualifiedName.Namespace] = true;
                XmlSchemaComplexType type2 = type as XmlSchemaComplexType;
                if ((type2 == null) || (!type2.IsAbstract && (type != XmlSchemaComplexType.AnyType)))
                {
                    schemaInfo.ElementDeclsByType.Add(type.QualifiedName, type.ElementDecl);
                }
            }
            foreach (XmlSchemaNotation notation in this.schema.Notations.Values)
            {
                schemaInfo.TargetNamespaces[notation.QualifiedName.Namespace] = true;
                SchemaNotation notation2 = new SchemaNotation(notation.QualifiedName) {
                    SystemLiteral = notation.System,
                    Pubid = notation.Public
                };
                if (schemaInfo.Notations[notation2.Name.Name] == null)
                {
                    schemaInfo.Notations.Add(notation2.Name.Name, notation2);
                }
            }
        }

        private void Prepare()
        {
            foreach (XmlSchemaElement element in this.schema.Elements.Values)
            {
                if (!element.SubstitutionGroup.IsEmpty)
                {
                    XmlSchemaSubstitutionGroup group = (XmlSchemaSubstitutionGroup) this.examplars[element.SubstitutionGroup];
                    if (group == null)
                    {
                        group = new XmlSchemaSubstitutionGroupV1Compat {
                            Examplar = element.SubstitutionGroup
                        };
                        this.examplars.Add(element.SubstitutionGroup, group);
                    }
                    group.Members.Add(element);
                }
            }
        }

        private void PushComplexType(XmlSchemaComplexType complexType)
        {
            this.complexTypeStack.Push(complexType);
        }
    }
}

