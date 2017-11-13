﻿namespace System.Xml.Schema
{
    using System;
    using System.Collections;
    using System.IO;
    using System.Xml;

    internal sealed class SchemaCollectionPreprocessor : BaseProcessor
    {
        private XmlSchemaForm attributeFormDefault;
        private XmlSchemaDerivationMethod blockDefault;
        private bool buildinIncluded;
        private const XmlSchemaDerivationMethod complexTypeBlockAllowed = (XmlSchemaDerivationMethod.Restriction | XmlSchemaDerivationMethod.Extension);
        private const XmlSchemaDerivationMethod complexTypeFinalAllowed = (XmlSchemaDerivationMethod.Restriction | XmlSchemaDerivationMethod.Extension);
        private const XmlSchemaDerivationMethod elementBlockAllowed = (XmlSchemaDerivationMethod.Restriction | XmlSchemaDerivationMethod.Extension | XmlSchemaDerivationMethod.Substitution);
        private const XmlSchemaDerivationMethod elementFinalAllowed = (XmlSchemaDerivationMethod.Restriction | XmlSchemaDerivationMethod.Extension);
        private XmlSchemaForm elementFormDefault;
        private XmlSchemaDerivationMethod finalDefault;
        private Hashtable referenceNamespaces;
        private XmlSchema schema;
        private const XmlSchemaDerivationMethod schemaBlockDefaultAllowed = (XmlSchemaDerivationMethod.Restriction | XmlSchemaDerivationMethod.Extension | XmlSchemaDerivationMethod.Substitution);
        private const XmlSchemaDerivationMethod schemaFinalDefaultAllowed = (XmlSchemaDerivationMethod.Union | XmlSchemaDerivationMethod.List | XmlSchemaDerivationMethod.Restriction | XmlSchemaDerivationMethod.Extension);
        private Hashtable schemaLocations;
        private const XmlSchemaDerivationMethod simpleTypeFinalAllowed = (XmlSchemaDerivationMethod.Union | XmlSchemaDerivationMethod.List | XmlSchemaDerivationMethod.Restriction);
        private string targetNamespace;
        private string Xmlns;
        private System.Xml.XmlResolver xmlResolver;

        public SchemaCollectionPreprocessor(XmlNameTable nameTable, SchemaNames schemaNames, ValidationEventHandler eventHandler) : base(nameTable, schemaNames, eventHandler)
        {
        }

        private void BuildRefNamespaces(XmlSchema schema)
        {
            this.referenceNamespaces = new Hashtable();
            this.referenceNamespaces.Add("http://www.w3.org/2001/XMLSchema", "http://www.w3.org/2001/XMLSchema");
            this.referenceNamespaces.Add(string.Empty, string.Empty);
            foreach (XmlSchemaExternal external in schema.Includes)
            {
                if (external is XmlSchemaImport)
                {
                    XmlSchemaImport import = external as XmlSchemaImport;
                    string key = import.Namespace;
                    if ((key != null) && (this.referenceNamespaces[key] == null))
                    {
                        this.referenceNamespaces.Add(key, key);
                    }
                }
            }
            if ((schema.TargetNamespace != null) && (this.referenceNamespaces[schema.TargetNamespace] == null))
            {
                this.referenceNamespaces.Add(schema.TargetNamespace, schema.TargetNamespace);
            }
        }

        private void CheckRefinedAttributeGroup(XmlSchemaAttributeGroup attributeGroup)
        {
            int num = 0;
            foreach (object obj2 in attributeGroup.Attributes)
            {
                if ((obj2 is XmlSchemaAttributeGroupRef) && (((XmlSchemaAttributeGroupRef) obj2).RefName == attributeGroup.QualifiedName))
                {
                    num++;
                }
            }
            if (num > 1)
            {
                base.SendValidationEvent("Sch_MultipleAttrGroupSelfRef", attributeGroup);
            }
        }

        private void CheckRefinedComplexType(XmlSchemaComplexType ctype)
        {
            if (ctype.ContentModel != null)
            {
                XmlQualifiedName baseTypeName;
                if (ctype.ContentModel is XmlSchemaComplexContent)
                {
                    XmlSchemaComplexContent contentModel = (XmlSchemaComplexContent) ctype.ContentModel;
                    if (contentModel.Content is XmlSchemaComplexContentRestriction)
                    {
                        baseTypeName = ((XmlSchemaComplexContentRestriction) contentModel.Content).BaseTypeName;
                    }
                    else
                    {
                        baseTypeName = ((XmlSchemaComplexContentExtension) contentModel.Content).BaseTypeName;
                    }
                }
                else
                {
                    XmlSchemaSimpleContent content2 = (XmlSchemaSimpleContent) ctype.ContentModel;
                    if (content2.Content is XmlSchemaSimpleContentRestriction)
                    {
                        baseTypeName = ((XmlSchemaSimpleContentRestriction) content2.Content).BaseTypeName;
                    }
                    else
                    {
                        baseTypeName = ((XmlSchemaSimpleContentExtension) content2.Content).BaseTypeName;
                    }
                }
                if (baseTypeName == ctype.QualifiedName)
                {
                    return;
                }
            }
            base.SendValidationEvent("Sch_InvalidTypeRedefine", ctype);
        }

        private void CheckRefinedGroup(XmlSchemaGroup group)
        {
            int num = 0;
            if (group.Particle != null)
            {
                num = this.CountGroupSelfReference(group.Particle.Items, group.QualifiedName);
            }
            if (num > 1)
            {
                base.SendValidationEvent("Sch_MultipleGroupSelfRef", group);
            }
        }

        private void CheckRefinedSimpleType(XmlSchemaSimpleType stype)
        {
            if ((stype.Content != null) && (stype.Content is XmlSchemaSimpleTypeRestriction))
            {
                XmlSchemaSimpleTypeRestriction content = (XmlSchemaSimpleTypeRestriction) stype.Content;
                if (content.BaseTypeName == stype.QualifiedName)
                {
                    return;
                }
            }
            base.SendValidationEvent("Sch_InvalidTypeRedefine", stype);
        }

        private void Cleanup(XmlSchema schema)
        {
            if (!schema.IsProcessing)
            {
                schema.IsProcessing = true;
                foreach (XmlSchemaExternal external in schema.Includes)
                {
                    if (external.Schema != null)
                    {
                        this.Cleanup(external.Schema);
                    }
                    if (external is XmlSchemaRedefine)
                    {
                        XmlSchemaRedefine redefine = external as XmlSchemaRedefine;
                        redefine.AttributeGroups.Clear();
                        redefine.Groups.Clear();
                        redefine.SchemaTypes.Clear();
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
                schema.IsProcessing = false;
            }
        }

        private int CountGroupSelfReference(XmlSchemaObjectCollection items, XmlQualifiedName name)
        {
            int num = 0;
            foreach (XmlSchemaParticle particle in items)
            {
                if (particle is XmlSchemaGroupRef)
                {
                    XmlSchemaGroupRef source = (XmlSchemaGroupRef) particle;
                    if (source.RefName == name)
                    {
                        if ((source.MinOccurs != 1M) || (source.MaxOccurs != 1M))
                        {
                            base.SendValidationEvent("Sch_MinMaxGroupRedefine", source);
                        }
                        num++;
                    }
                }
                else if (particle is XmlSchemaGroupBase)
                {
                    num += this.CountGroupSelfReference(((XmlSchemaGroupBase) particle).Items, name);
                }
                if (num > 1)
                {
                    return num;
                }
            }
            return num;
        }

        public bool Execute(XmlSchema schema, string targetNamespace, bool loadExternals, XmlSchemaCollection xsc)
        {
            this.schema = schema;
            this.Xmlns = base.NameTable.Add("xmlns");
            this.Cleanup(schema);
            if (loadExternals && (this.xmlResolver != null))
            {
                this.schemaLocations = new Hashtable();
                if (schema.BaseUri != null)
                {
                    this.schemaLocations.Add(schema.BaseUri, schema.BaseUri);
                }
                this.LoadExternals(schema, xsc);
            }
            this.ValidateIdAttribute(schema);
            this.Preprocess(schema, targetNamespace, Compositor.Root);
            if (!base.HasErrors)
            {
                schema.IsPreprocessed = true;
                foreach (XmlSchemaExternal external in schema.Includes)
                {
                    if (external.Schema != null)
                    {
                        external.Schema.IsPreprocessed = true;
                    }
                }
            }
            return !base.HasErrors;
        }

        private Stream GetSchemaEntity(Uri ruri)
        {
            try
            {
                return (Stream) this.xmlResolver.GetEntity(ruri, null, null);
            }
            catch
            {
                return null;
            }
        }

        private void LoadExternals(XmlSchema schema, XmlSchemaCollection xsc)
        {
            if (!schema.IsProcessing)
            {
                schema.IsProcessing = true;
                foreach (XmlSchemaExternal external in schema.Includes)
                {
                    Uri key = null;
                    if (external.Schema != null)
                    {
                        if ((external is XmlSchemaImport) && (((XmlSchemaImport) external).Namespace == "http://www.w3.org/XML/1998/namespace"))
                        {
                            this.buildinIncluded = true;
                        }
                        else
                        {
                            key = external.BaseUri;
                            if ((key != null) && (this.schemaLocations[key] == null))
                            {
                                this.schemaLocations.Add(key, key);
                            }
                            this.LoadExternals(external.Schema, xsc);
                        }
                    }
                    else
                    {
                        if ((xsc != null) && (external is XmlSchemaImport))
                        {
                            XmlSchemaImport import = (XmlSchemaImport) external;
                            string str = (import.Namespace != null) ? import.Namespace : string.Empty;
                            external.Schema = xsc[str];
                            if (external.Schema != null)
                            {
                                external.Schema = external.Schema.Clone();
                                if ((external.Schema.BaseUri != null) && (this.schemaLocations[external.Schema.BaseUri] == null))
                                {
                                    this.schemaLocations.Add(external.Schema.BaseUri, external.Schema.BaseUri);
                                }
                                Uri uri2 = null;
                                foreach (XmlSchemaExternal external2 in external.Schema.Includes)
                                {
                                    if (external2 is XmlSchemaImport)
                                    {
                                        XmlSchemaImport import2 = (XmlSchemaImport) external2;
                                        uri2 = (import2.BaseUri != null) ? import2.BaseUri : (((import2.Schema != null) && (import2.Schema.BaseUri != null)) ? import2.Schema.BaseUri : null);
                                        if (uri2 != null)
                                        {
                                            if (this.schemaLocations[uri2] != null)
                                            {
                                                import2.Schema = null;
                                            }
                                            else
                                            {
                                                this.schemaLocations.Add(uri2, uri2);
                                            }
                                        }
                                    }
                                }
                                goto Label_039A;
                            }
                        }
                        if ((external is XmlSchemaImport) && (((XmlSchemaImport) external).Namespace == "http://www.w3.org/XML/1998/namespace"))
                        {
                            if (!this.buildinIncluded)
                            {
                                this.buildinIncluded = true;
                                external.Schema = Preprocessor.GetBuildInSchema();
                            }
                        }
                        else
                        {
                            string schemaLocation = external.SchemaLocation;
                            if (schemaLocation != null)
                            {
                                Uri ruri = this.ResolveSchemaLocationUri(schema, schemaLocation);
                                if ((ruri != null) && (this.schemaLocations[ruri] == null))
                                {
                                    Stream schemaEntity = this.GetSchemaEntity(ruri);
                                    if (schemaEntity != null)
                                    {
                                        external.BaseUri = ruri;
                                        this.schemaLocations.Add(ruri, ruri);
                                        XmlTextReader reader = new XmlTextReader(ruri.ToString(), schemaEntity, base.NameTable) {
                                            XmlResolver = this.xmlResolver
                                        };
                                        try
                                        {
                                            try
                                            {
                                                System.Xml.Schema.Parser parser = new System.Xml.Schema.Parser(SchemaType.XSD, base.NameTable, base.SchemaNames, base.EventHandler);
                                                parser.Parse(reader, null);
                                                while (reader.Read())
                                                {
                                                }
                                                external.Schema = parser.XmlSchema;
                                                this.LoadExternals(external.Schema, xsc);
                                            }
                                            catch (XmlSchemaException exception)
                                            {
                                                base.SendValidationEventNoThrow(new XmlSchemaException("Sch_CannotLoadSchema", new string[] { schemaLocation, exception.Message }, exception.SourceUri, exception.LineNumber, exception.LinePosition), XmlSeverityType.Error);
                                            }
                                            catch (Exception)
                                            {
                                                base.SendValidationEvent("Sch_InvalidIncludeLocation", external, XmlSeverityType.Warning);
                                            }
                                            goto Label_039A;
                                        }
                                        finally
                                        {
                                            reader.Close();
                                        }
                                    }
                                    base.SendValidationEvent("Sch_InvalidIncludeLocation", external, XmlSeverityType.Warning);
                                }
                            }
                        }
                    Label_039A:;
                    }
                }
                schema.IsProcessing = false;
            }
        }

        private void Preprocess(XmlSchema schema, string targetNamespace, Compositor compositor)
        {
            if (schema.IsProcessing)
            {
                return;
            }
            schema.IsProcessing = true;
            string array = schema.TargetNamespace;
            if (array != null)
            {
                schema.TargetNamespace = array = base.NameTable.Add(array);
                if (array.Length == 0)
                {
                    base.SendValidationEvent("Sch_InvalidTargetNamespaceAttribute", schema);
                }
                else
                {
                    try
                    {
                        XmlConvert.ToUri(array);
                    }
                    catch
                    {
                        base.SendValidationEvent("Sch_InvalidNamespace", schema.TargetNamespace, schema);
                    }
                }
            }
            if (schema.Version != null)
            {
                try
                {
                    XmlConvert.VerifyTOKEN(schema.Version);
                }
                catch (Exception)
                {
                    base.SendValidationEvent("Sch_AttributeValueDataType", "version", schema);
                }
            }
            switch (compositor)
            {
                case Compositor.Root:
                    if ((targetNamespace != null) || (schema.TargetNamespace == null))
                    {
                        if (((schema.TargetNamespace == null) && (targetNamespace != null)) && (targetNamespace.Length == 0))
                        {
                            targetNamespace = null;
                        }
                        break;
                    }
                    targetNamespace = schema.TargetNamespace;
                    break;

                case Compositor.Include:
                    if ((schema.TargetNamespace != null) && (targetNamespace != schema.TargetNamespace))
                    {
                        base.SendValidationEvent("Sch_MismatchTargetNamespaceInclude", targetNamespace, schema.TargetNamespace, schema);
                    }
                    goto Label_0141;

                case Compositor.Import:
                    if (targetNamespace != schema.TargetNamespace)
                    {
                        base.SendValidationEvent("Sch_MismatchTargetNamespaceImport", targetNamespace, schema.TargetNamespace, schema);
                    }
                    goto Label_0141;

                default:
                    goto Label_0141;
            }
            if (targetNamespace != schema.TargetNamespace)
            {
                base.SendValidationEvent("Sch_MismatchTargetNamespaceEx", targetNamespace, schema.TargetNamespace, schema);
            }
        Label_0141:
            foreach (XmlSchemaExternal external in schema.Includes)
            {
                this.SetParent(external, schema);
                this.PreprocessAnnotation(external);
                string schemaLocation = external.SchemaLocation;
                if (schemaLocation != null)
                {
                    try
                    {
                        XmlConvert.ToUri(schemaLocation);
                    }
                    catch
                    {
                        base.SendValidationEvent("Sch_InvalidSchemaLocation", schemaLocation, external);
                    }
                }
                else if (((external is XmlSchemaRedefine) || (external is XmlSchemaInclude)) && (external.Schema == null))
                {
                    base.SendValidationEvent("Sch_MissRequiredAttribute", "schemaLocation", external);
                }
                if (external.Schema != null)
                {
                    if (external is XmlSchemaRedefine)
                    {
                        this.Preprocess(external.Schema, schema.TargetNamespace, Compositor.Include);
                    }
                    else if (external is XmlSchemaImport)
                    {
                        if ((((XmlSchemaImport) external).Namespace == null) && (schema.TargetNamespace == null))
                        {
                            base.SendValidationEvent("Sch_ImportTargetNamespaceNull", external);
                        }
                        else if (((XmlSchemaImport) external).Namespace == schema.TargetNamespace)
                        {
                            base.SendValidationEvent("Sch_ImportTargetNamespace", external);
                        }
                        this.Preprocess(external.Schema, ((XmlSchemaImport) external).Namespace, Compositor.Import);
                    }
                    else
                    {
                        this.Preprocess(external.Schema, schema.TargetNamespace, Compositor.Include);
                    }
                }
                else if (external is XmlSchemaImport)
                {
                    string msg = ((XmlSchemaImport) external).Namespace;
                    if (msg != null)
                    {
                        if (msg.Length == 0)
                        {
                            base.SendValidationEvent("Sch_InvalidNamespaceAttribute", msg, external);
                        }
                        else
                        {
                            try
                            {
                                XmlConvert.ToUri(msg);
                            }
                            catch (FormatException)
                            {
                                base.SendValidationEvent("Sch_InvalidNamespace", msg, external);
                            }
                        }
                    }
                }
            }
            this.BuildRefNamespaces(schema);
            this.targetNamespace = (targetNamespace == null) ? string.Empty : targetNamespace;
            if (schema.BlockDefault == XmlSchemaDerivationMethod.All)
            {
                this.blockDefault = XmlSchemaDerivationMethod.All;
            }
            else if (schema.BlockDefault == XmlSchemaDerivationMethod.None)
            {
                this.blockDefault = XmlSchemaDerivationMethod.Empty;
            }
            else
            {
                if ((schema.BlockDefault & ~(XmlSchemaDerivationMethod.Restriction | XmlSchemaDerivationMethod.Extension | XmlSchemaDerivationMethod.Substitution)) != XmlSchemaDerivationMethod.Empty)
                {
                    base.SendValidationEvent("Sch_InvalidBlockDefaultValue", schema);
                }
                this.blockDefault = schema.BlockDefault & (XmlSchemaDerivationMethod.Restriction | XmlSchemaDerivationMethod.Extension | XmlSchemaDerivationMethod.Substitution);
            }
            if (schema.FinalDefault == XmlSchemaDerivationMethod.All)
            {
                this.finalDefault = XmlSchemaDerivationMethod.All;
            }
            else if (schema.FinalDefault == XmlSchemaDerivationMethod.None)
            {
                this.finalDefault = XmlSchemaDerivationMethod.Empty;
            }
            else
            {
                if ((schema.FinalDefault & ~(XmlSchemaDerivationMethod.Union | XmlSchemaDerivationMethod.List | XmlSchemaDerivationMethod.Restriction | XmlSchemaDerivationMethod.Extension)) != XmlSchemaDerivationMethod.Empty)
                {
                    base.SendValidationEvent("Sch_InvalidFinalDefaultValue", schema);
                }
                this.finalDefault = schema.FinalDefault & (XmlSchemaDerivationMethod.Union | XmlSchemaDerivationMethod.List | XmlSchemaDerivationMethod.Restriction | XmlSchemaDerivationMethod.Extension);
            }
            this.elementFormDefault = schema.ElementFormDefault;
            if (this.elementFormDefault == XmlSchemaForm.None)
            {
                this.elementFormDefault = XmlSchemaForm.Unqualified;
            }
            this.attributeFormDefault = schema.AttributeFormDefault;
            if (this.attributeFormDefault == XmlSchemaForm.None)
            {
                this.attributeFormDefault = XmlSchemaForm.Unqualified;
            }
            foreach (XmlSchemaExternal external2 in schema.Includes)
            {
                XmlSchema schema2;
                if (external2 is XmlSchemaRedefine)
                {
                    XmlSchemaRedefine redefine = (XmlSchemaRedefine) external2;
                    if (external2.Schema != null)
                    {
                        this.PreprocessRedefine(redefine);
                    }
                    else
                    {
                        using (XmlSchemaObjectEnumerator enumerator3 = redefine.Items.GetEnumerator())
                        {
                            while (enumerator3.MoveNext())
                            {
                                if (!(enumerator3.Current is XmlSchemaAnnotation))
                                {
                                    base.SendValidationEvent("Sch_RedefineNoSchema", redefine);
                                    goto Label_045D;
                                }
                            }
                        }
                    }
                }
            Label_045D:
                schema2 = external2.Schema;
                if (schema2 != null)
                {
                    foreach (XmlSchemaElement element in schema2.Elements.Values)
                    {
                        base.AddToTable(schema.Elements, element.QualifiedName, element);
                    }
                    foreach (XmlSchemaAttribute attribute in schema2.Attributes.Values)
                    {
                        base.AddToTable(schema.Attributes, attribute.QualifiedName, attribute);
                    }
                    foreach (XmlSchemaGroup group in schema2.Groups.Values)
                    {
                        base.AddToTable(schema.Groups, group.QualifiedName, group);
                    }
                    foreach (XmlSchemaAttributeGroup group2 in schema2.AttributeGroups.Values)
                    {
                        base.AddToTable(schema.AttributeGroups, group2.QualifiedName, group2);
                    }
                    foreach (XmlSchemaType type in schema2.SchemaTypes.Values)
                    {
                        base.AddToTable(schema.SchemaTypes, type.QualifiedName, type);
                    }
                    foreach (XmlSchemaNotation notation in schema2.Notations.Values)
                    {
                        base.AddToTable(schema.Notations, notation.QualifiedName, notation);
                    }
                }
                this.ValidateIdAttribute(external2);
            }
            ArrayList list = new ArrayList();
            foreach (XmlSchemaObject obj3 in schema.Items)
            {
                this.SetParent(obj3, schema);
                if (obj3 is XmlSchemaAttribute)
                {
                    XmlSchemaAttribute attribute2 = (XmlSchemaAttribute) obj3;
                    this.PreprocessAttribute(attribute2);
                    base.AddToTable(schema.Attributes, attribute2.QualifiedName, attribute2);
                }
                else if (obj3 is XmlSchemaAttributeGroup)
                {
                    XmlSchemaAttributeGroup attributeGroup = (XmlSchemaAttributeGroup) obj3;
                    this.PreprocessAttributeGroup(attributeGroup);
                    base.AddToTable(schema.AttributeGroups, attributeGroup.QualifiedName, attributeGroup);
                }
                else if (obj3 is XmlSchemaComplexType)
                {
                    XmlSchemaComplexType complexType = (XmlSchemaComplexType) obj3;
                    this.PreprocessComplexType(complexType, false);
                    base.AddToTable(schema.SchemaTypes, complexType.QualifiedName, complexType);
                }
                else if (obj3 is XmlSchemaSimpleType)
                {
                    XmlSchemaSimpleType simpleType = (XmlSchemaSimpleType) obj3;
                    this.PreprocessSimpleType(simpleType, false);
                    base.AddToTable(schema.SchemaTypes, simpleType.QualifiedName, simpleType);
                }
                else if (obj3 is XmlSchemaElement)
                {
                    XmlSchemaElement element2 = (XmlSchemaElement) obj3;
                    this.PreprocessElement(element2);
                    base.AddToTable(schema.Elements, element2.QualifiedName, element2);
                }
                else if (obj3 is XmlSchemaGroup)
                {
                    XmlSchemaGroup group4 = (XmlSchemaGroup) obj3;
                    this.PreprocessGroup(group4);
                    base.AddToTable(schema.Groups, group4.QualifiedName, group4);
                }
                else if (obj3 is XmlSchemaNotation)
                {
                    XmlSchemaNotation notation2 = (XmlSchemaNotation) obj3;
                    this.PreprocessNotation(notation2);
                    base.AddToTable(schema.Notations, notation2.QualifiedName, notation2);
                }
                else if (!(obj3 is XmlSchemaAnnotation))
                {
                    base.SendValidationEvent("Sch_InvalidCollection", obj3);
                    list.Add(obj3);
                }
            }
            foreach (XmlSchemaObject obj4 in list)
            {
                schema.Items.Remove(obj4);
            }
            schema.IsProcessing = false;
        }

        private void PreprocessAnnotation(XmlSchemaObject schemaObject)
        {
            if (schemaObject is XmlSchemaAnnotated)
            {
                XmlSchemaAnnotated annotated = schemaObject as XmlSchemaAnnotated;
                if (annotated.Annotation != null)
                {
                    annotated.Annotation.Parent = schemaObject;
                    using (XmlSchemaObjectEnumerator enumerator = annotated.Annotation.Items.GetEnumerator())
                    {
                        while (enumerator.MoveNext())
                        {
                            enumerator.Current.Parent = annotated.Annotation;
                        }
                    }
                }
            }
        }

        private void PreprocessAttribute(XmlSchemaAttribute attribute)
        {
            if (attribute.Name != null)
            {
                this.ValidateNameAttribute(attribute);
                attribute.SetQualifiedName(new XmlQualifiedName(attribute.Name, this.targetNamespace));
            }
            else
            {
                base.SendValidationEvent("Sch_MissRequiredAttribute", "name", attribute);
            }
            if (attribute.Use != XmlSchemaUse.None)
            {
                base.SendValidationEvent("Sch_ForbiddenAttribute", "use", attribute);
            }
            if (attribute.Form != XmlSchemaForm.None)
            {
                base.SendValidationEvent("Sch_ForbiddenAttribute", "form", attribute);
            }
            this.PreprocessAttributeContent(attribute);
            this.ValidateIdAttribute(attribute);
        }

        private void PreprocessAttributeContent(XmlSchemaAttribute attribute)
        {
            this.PreprocessAnnotation(attribute);
            if (this.schema.TargetNamespace == "http://www.w3.org/2001/XMLSchema-instance")
            {
                base.SendValidationEvent("Sch_TargetNamespaceXsi", attribute);
            }
            if (!attribute.RefName.IsEmpty)
            {
                base.SendValidationEvent("Sch_ForbiddenAttribute", "ref", attribute);
            }
            if ((attribute.DefaultValue != null) && (attribute.FixedValue != null))
            {
                base.SendValidationEvent("Sch_DefaultFixedAttributes", attribute);
            }
            if (((attribute.DefaultValue != null) && (attribute.Use != XmlSchemaUse.Optional)) && (attribute.Use != XmlSchemaUse.None))
            {
                base.SendValidationEvent("Sch_OptionalDefaultAttribute", attribute);
            }
            if (attribute.Name == this.Xmlns)
            {
                base.SendValidationEvent("Sch_XmlNsAttribute", attribute);
            }
            if (attribute.SchemaType != null)
            {
                this.SetParent(attribute.SchemaType, attribute);
                if (!attribute.SchemaTypeName.IsEmpty)
                {
                    base.SendValidationEvent("Sch_TypeMutualExclusive", attribute);
                }
                this.PreprocessSimpleType(attribute.SchemaType, true);
            }
            if (!attribute.SchemaTypeName.IsEmpty)
            {
                this.ValidateQNameAttribute(attribute, "type", attribute.SchemaTypeName);
            }
        }

        private void PreprocessAttributeGroup(XmlSchemaAttributeGroup attributeGroup)
        {
            if (attributeGroup.Name != null)
            {
                this.ValidateNameAttribute(attributeGroup);
                attributeGroup.SetQualifiedName(new XmlQualifiedName(attributeGroup.Name, this.targetNamespace));
            }
            else
            {
                base.SendValidationEvent("Sch_MissRequiredAttribute", "name", attributeGroup);
            }
            this.PreprocessAttributes(attributeGroup.Attributes, attributeGroup.AnyAttribute, attributeGroup);
            this.PreprocessAnnotation(attributeGroup);
            this.ValidateIdAttribute(attributeGroup);
        }

        private void PreprocessAttributes(XmlSchemaObjectCollection attributes, XmlSchemaAnyAttribute anyAttribute, XmlSchemaObject parent)
        {
            foreach (XmlSchemaAnnotated annotated in attributes)
            {
                this.SetParent(annotated, parent);
                if (annotated is XmlSchemaAttribute)
                {
                    this.PreprocessLocalAttribute((XmlSchemaAttribute) annotated);
                }
                else
                {
                    XmlSchemaAttributeGroupRef source = (XmlSchemaAttributeGroupRef) annotated;
                    if (source.RefName.IsEmpty)
                    {
                        base.SendValidationEvent("Sch_MissAttribute", "ref", source);
                    }
                    else
                    {
                        this.ValidateQNameAttribute(source, "ref", source.RefName);
                    }
                    this.PreprocessAnnotation(annotated);
                    this.ValidateIdAttribute(annotated);
                }
            }
            if (anyAttribute != null)
            {
                try
                {
                    this.SetParent(anyAttribute, parent);
                    this.PreprocessAnnotation(anyAttribute);
                    anyAttribute.BuildNamespaceListV1Compat(this.targetNamespace);
                }
                catch
                {
                    base.SendValidationEvent("Sch_InvalidAnyAttribute", anyAttribute);
                }
                this.ValidateIdAttribute(anyAttribute);
            }
        }

        private void PreprocessComplexType(XmlSchemaComplexType complexType, bool local)
        {
            if (local)
            {
                if (complexType.Name != null)
                {
                    base.SendValidationEvent("Sch_ForbiddenAttribute", "name", complexType);
                }
            }
            else
            {
                if (complexType.Name != null)
                {
                    this.ValidateNameAttribute(complexType);
                    complexType.SetQualifiedName(new XmlQualifiedName(complexType.Name, this.targetNamespace));
                }
                else
                {
                    base.SendValidationEvent("Sch_MissRequiredAttribute", "name", complexType);
                }
                if (complexType.Block == XmlSchemaDerivationMethod.All)
                {
                    complexType.SetBlockResolved(XmlSchemaDerivationMethod.All);
                }
                else if (complexType.Block == XmlSchemaDerivationMethod.None)
                {
                    complexType.SetBlockResolved(this.blockDefault & (XmlSchemaDerivationMethod.Restriction | XmlSchemaDerivationMethod.Extension));
                }
                else
                {
                    if ((complexType.Block & ~(XmlSchemaDerivationMethod.Restriction | XmlSchemaDerivationMethod.Extension)) != XmlSchemaDerivationMethod.Empty)
                    {
                        base.SendValidationEvent("Sch_InvalidComplexTypeBlockValue", complexType);
                    }
                    complexType.SetBlockResolved(complexType.Block & (XmlSchemaDerivationMethod.Restriction | XmlSchemaDerivationMethod.Extension));
                }
                if (complexType.Final == XmlSchemaDerivationMethod.All)
                {
                    complexType.SetFinalResolved(XmlSchemaDerivationMethod.All);
                }
                else if (complexType.Final == XmlSchemaDerivationMethod.None)
                {
                    if (this.finalDefault == XmlSchemaDerivationMethod.All)
                    {
                        complexType.SetFinalResolved(XmlSchemaDerivationMethod.All);
                    }
                    else
                    {
                        complexType.SetFinalResolved(this.finalDefault & (XmlSchemaDerivationMethod.Restriction | XmlSchemaDerivationMethod.Extension));
                    }
                }
                else
                {
                    if ((complexType.Final & ~(XmlSchemaDerivationMethod.Restriction | XmlSchemaDerivationMethod.Extension)) != XmlSchemaDerivationMethod.Empty)
                    {
                        base.SendValidationEvent("Sch_InvalidComplexTypeFinalValue", complexType);
                    }
                    complexType.SetFinalResolved(complexType.Final & (XmlSchemaDerivationMethod.Restriction | XmlSchemaDerivationMethod.Extension));
                }
            }
            if (complexType.ContentModel != null)
            {
                this.SetParent(complexType.ContentModel, complexType);
                this.PreprocessAnnotation(complexType.ContentModel);
                if (complexType.Particle == null)
                {
                    XmlSchemaObjectCollection attributes = complexType.Attributes;
                }
                if (complexType.ContentModel is XmlSchemaSimpleContent)
                {
                    XmlSchemaSimpleContent contentModel = (XmlSchemaSimpleContent) complexType.ContentModel;
                    if (contentModel.Content == null)
                    {
                        if (complexType.QualifiedName == XmlQualifiedName.Empty)
                        {
                            base.SendValidationEvent("Sch_NoRestOrExt", complexType);
                        }
                        else
                        {
                            base.SendValidationEvent("Sch_NoRestOrExtQName", complexType.QualifiedName.Name, complexType.QualifiedName.Namespace, complexType);
                        }
                    }
                    else
                    {
                        this.SetParent(contentModel.Content, contentModel);
                        this.PreprocessAnnotation(contentModel.Content);
                        if (contentModel.Content is XmlSchemaSimpleContentExtension)
                        {
                            XmlSchemaSimpleContentExtension content = (XmlSchemaSimpleContentExtension) contentModel.Content;
                            if (content.BaseTypeName.IsEmpty)
                            {
                                base.SendValidationEvent("Sch_MissAttribute", "base", content);
                            }
                            else
                            {
                                this.ValidateQNameAttribute(content, "base", content.BaseTypeName);
                            }
                            this.PreprocessAttributes(content.Attributes, content.AnyAttribute, content);
                            this.ValidateIdAttribute(content);
                        }
                        else
                        {
                            XmlSchemaSimpleContentRestriction source = (XmlSchemaSimpleContentRestriction) contentModel.Content;
                            if (source.BaseTypeName.IsEmpty)
                            {
                                base.SendValidationEvent("Sch_MissAttribute", "base", source);
                            }
                            else
                            {
                                this.ValidateQNameAttribute(source, "base", source.BaseTypeName);
                            }
                            if (source.BaseType != null)
                            {
                                this.SetParent(source.BaseType, source);
                                this.PreprocessSimpleType(source.BaseType, true);
                            }
                            this.PreprocessAttributes(source.Attributes, source.AnyAttribute, source);
                            this.ValidateIdAttribute(source);
                        }
                    }
                    this.ValidateIdAttribute(contentModel);
                }
                else
                {
                    XmlSchemaComplexContent parent = (XmlSchemaComplexContent) complexType.ContentModel;
                    if (parent.Content == null)
                    {
                        if (complexType.QualifiedName == XmlQualifiedName.Empty)
                        {
                            base.SendValidationEvent("Sch_NoRestOrExt", complexType);
                        }
                        else
                        {
                            base.SendValidationEvent("Sch_NoRestOrExtQName", complexType.QualifiedName.Name, complexType.QualifiedName.Namespace, complexType);
                        }
                    }
                    else
                    {
                        if (!parent.HasMixedAttribute && complexType.IsMixed)
                        {
                            parent.IsMixed = true;
                        }
                        this.SetParent(parent.Content, parent);
                        this.PreprocessAnnotation(parent.Content);
                        if (parent.Content is XmlSchemaComplexContentExtension)
                        {
                            XmlSchemaComplexContentExtension extension2 = (XmlSchemaComplexContentExtension) parent.Content;
                            if (extension2.BaseTypeName.IsEmpty)
                            {
                                base.SendValidationEvent("Sch_MissAttribute", "base", extension2);
                            }
                            else
                            {
                                this.ValidateQNameAttribute(extension2, "base", extension2.BaseTypeName);
                            }
                            if (extension2.Particle != null)
                            {
                                this.SetParent(extension2.Particle, extension2);
                                this.PreprocessParticle(extension2.Particle);
                            }
                            this.PreprocessAttributes(extension2.Attributes, extension2.AnyAttribute, extension2);
                            this.ValidateIdAttribute(extension2);
                        }
                        else
                        {
                            XmlSchemaComplexContentRestriction restriction2 = (XmlSchemaComplexContentRestriction) parent.Content;
                            if (restriction2.BaseTypeName.IsEmpty)
                            {
                                base.SendValidationEvent("Sch_MissAttribute", "base", restriction2);
                            }
                            else
                            {
                                this.ValidateQNameAttribute(restriction2, "base", restriction2.BaseTypeName);
                            }
                            if (restriction2.Particle != null)
                            {
                                this.SetParent(restriction2.Particle, restriction2);
                                this.PreprocessParticle(restriction2.Particle);
                            }
                            this.PreprocessAttributes(restriction2.Attributes, restriction2.AnyAttribute, restriction2);
                            this.ValidateIdAttribute(restriction2);
                        }
                        this.ValidateIdAttribute(parent);
                    }
                }
            }
            else
            {
                if (complexType.Particle != null)
                {
                    this.SetParent(complexType.Particle, complexType);
                    this.PreprocessParticle(complexType.Particle);
                }
                this.PreprocessAttributes(complexType.Attributes, complexType.AnyAttribute, complexType);
            }
            this.ValidateIdAttribute(complexType);
        }

        private void PreprocessElement(XmlSchemaElement element)
        {
            if (element.Name != null)
            {
                this.ValidateNameAttribute(element);
                element.SetQualifiedName(new XmlQualifiedName(element.Name, this.targetNamespace));
            }
            else
            {
                base.SendValidationEvent("Sch_MissRequiredAttribute", "name", element);
            }
            this.PreprocessElementContent(element);
            if (element.Final == XmlSchemaDerivationMethod.All)
            {
                element.SetFinalResolved(XmlSchemaDerivationMethod.All);
            }
            else if (element.Final == XmlSchemaDerivationMethod.None)
            {
                if (this.finalDefault == XmlSchemaDerivationMethod.All)
                {
                    element.SetFinalResolved(XmlSchemaDerivationMethod.All);
                }
                else
                {
                    element.SetFinalResolved(this.finalDefault & (XmlSchemaDerivationMethod.Restriction | XmlSchemaDerivationMethod.Extension));
                }
            }
            else
            {
                if ((element.Final & ~(XmlSchemaDerivationMethod.Restriction | XmlSchemaDerivationMethod.Extension)) != XmlSchemaDerivationMethod.Empty)
                {
                    base.SendValidationEvent("Sch_InvalidElementFinalValue", element);
                }
                element.SetFinalResolved(element.Final & (XmlSchemaDerivationMethod.Restriction | XmlSchemaDerivationMethod.Extension));
            }
            if (element.Form != XmlSchemaForm.None)
            {
                base.SendValidationEvent("Sch_ForbiddenAttribute", "form", element);
            }
            if (element.MinOccursString != null)
            {
                base.SendValidationEvent("Sch_ForbiddenAttribute", "minOccurs", element);
            }
            if (element.MaxOccursString != null)
            {
                base.SendValidationEvent("Sch_ForbiddenAttribute", "maxOccurs", element);
            }
            if (!element.SubstitutionGroup.IsEmpty)
            {
                this.ValidateQNameAttribute(element, "type", element.SubstitutionGroup);
            }
            this.ValidateIdAttribute(element);
        }

        private void PreprocessElementContent(XmlSchemaElement element)
        {
            this.PreprocessAnnotation(element);
            if (!element.RefName.IsEmpty)
            {
                base.SendValidationEvent("Sch_ForbiddenAttribute", "ref", element);
            }
            if (element.Block == XmlSchemaDerivationMethod.All)
            {
                element.SetBlockResolved(XmlSchemaDerivationMethod.All);
            }
            else if (element.Block == XmlSchemaDerivationMethod.None)
            {
                if (this.blockDefault == XmlSchemaDerivationMethod.All)
                {
                    element.SetBlockResolved(XmlSchemaDerivationMethod.All);
                }
                else
                {
                    element.SetBlockResolved(this.blockDefault & (XmlSchemaDerivationMethod.Restriction | XmlSchemaDerivationMethod.Extension | XmlSchemaDerivationMethod.Substitution));
                }
            }
            else
            {
                if ((element.Block & ~(XmlSchemaDerivationMethod.Restriction | XmlSchemaDerivationMethod.Extension | XmlSchemaDerivationMethod.Substitution)) != XmlSchemaDerivationMethod.Empty)
                {
                    base.SendValidationEvent("Sch_InvalidElementBlockValue", element);
                }
                element.SetBlockResolved(element.Block & (XmlSchemaDerivationMethod.Restriction | XmlSchemaDerivationMethod.Extension | XmlSchemaDerivationMethod.Substitution));
            }
            if (element.SchemaType != null)
            {
                this.SetParent(element.SchemaType, element);
                if (!element.SchemaTypeName.IsEmpty)
                {
                    base.SendValidationEvent("Sch_TypeMutualExclusive", element);
                }
                if (element.SchemaType is XmlSchemaComplexType)
                {
                    this.PreprocessComplexType((XmlSchemaComplexType) element.SchemaType, true);
                }
                else
                {
                    this.PreprocessSimpleType((XmlSchemaSimpleType) element.SchemaType, true);
                }
            }
            if (!element.SchemaTypeName.IsEmpty)
            {
                this.ValidateQNameAttribute(element, "type", element.SchemaTypeName);
            }
            if ((element.DefaultValue != null) && (element.FixedValue != null))
            {
                base.SendValidationEvent("Sch_DefaultFixedAttributes", element);
            }
            foreach (XmlSchemaIdentityConstraint constraint in element.Constraints)
            {
                this.SetParent(constraint, element);
                this.PreprocessIdentityConstraint(constraint);
            }
        }

        private void PreprocessGroup(XmlSchemaGroup group)
        {
            if (group.Name != null)
            {
                this.ValidateNameAttribute(group);
                group.SetQualifiedName(new XmlQualifiedName(group.Name, this.targetNamespace));
            }
            else
            {
                base.SendValidationEvent("Sch_MissRequiredAttribute", "name", group);
            }
            if (group.Particle == null)
            {
                base.SendValidationEvent("Sch_NoGroupParticle", group);
            }
            else
            {
                if (group.Particle.MinOccursString != null)
                {
                    base.SendValidationEvent("Sch_ForbiddenAttribute", "minOccurs", group.Particle);
                }
                if (group.Particle.MaxOccursString != null)
                {
                    base.SendValidationEvent("Sch_ForbiddenAttribute", "maxOccurs", group.Particle);
                }
                this.PreprocessParticle(group.Particle);
                this.PreprocessAnnotation(group);
                this.ValidateIdAttribute(group);
            }
        }

        private void PreprocessIdentityConstraint(XmlSchemaIdentityConstraint constraint)
        {
            bool flag = true;
            this.PreprocessAnnotation(constraint);
            if (constraint.Name != null)
            {
                this.ValidateNameAttribute(constraint);
                constraint.SetQualifiedName(new XmlQualifiedName(constraint.Name, this.targetNamespace));
            }
            else
            {
                base.SendValidationEvent("Sch_MissRequiredAttribute", "name", constraint);
                flag = false;
            }
            if (this.schema.IdentityConstraints[constraint.QualifiedName] != null)
            {
                base.SendValidationEvent("Sch_DupIdentityConstraint", constraint.QualifiedName.ToString(), constraint);
                flag = false;
            }
            else
            {
                this.schema.IdentityConstraints.Add(constraint.QualifiedName, constraint);
            }
            if (constraint.Selector == null)
            {
                base.SendValidationEvent("Sch_IdConstraintNoSelector", constraint);
                flag = false;
            }
            if (constraint.Fields.Count == 0)
            {
                base.SendValidationEvent("Sch_IdConstraintNoFields", constraint);
                flag = false;
            }
            if (constraint is XmlSchemaKeyref)
            {
                XmlSchemaKeyref xso = (XmlSchemaKeyref) constraint;
                if (xso.Refer.IsEmpty)
                {
                    base.SendValidationEvent("Sch_IdConstraintNoRefer", constraint);
                    flag = false;
                }
                else
                {
                    this.ValidateQNameAttribute(xso, "refer", xso.Refer);
                }
            }
            if (flag)
            {
                this.ValidateIdAttribute(constraint);
                this.ValidateIdAttribute(constraint.Selector);
                this.SetParent(constraint.Selector, constraint);
                foreach (XmlSchemaXPath path in constraint.Fields)
                {
                    this.SetParent(path, constraint);
                    this.ValidateIdAttribute(path);
                }
            }
        }

        private void PreprocessLocalAttribute(XmlSchemaAttribute attribute)
        {
            if (attribute.Name != null)
            {
                this.ValidateNameAttribute(attribute);
                this.PreprocessAttributeContent(attribute);
                attribute.SetQualifiedName(new XmlQualifiedName(attribute.Name, ((attribute.Form == XmlSchemaForm.Qualified) || ((attribute.Form == XmlSchemaForm.None) && (this.attributeFormDefault == XmlSchemaForm.Qualified))) ? this.targetNamespace : null));
            }
            else
            {
                this.PreprocessAnnotation(attribute);
                if (attribute.RefName.IsEmpty)
                {
                    base.SendValidationEvent("Sch_AttributeNameRef", "???", attribute);
                }
                else
                {
                    this.ValidateQNameAttribute(attribute, "ref", attribute.RefName);
                }
                if ((!attribute.SchemaTypeName.IsEmpty || (attribute.SchemaType != null)) || (attribute.Form != XmlSchemaForm.None))
                {
                    base.SendValidationEvent("Sch_InvalidAttributeRef", attribute);
                }
                attribute.SetQualifiedName(attribute.RefName);
            }
            this.ValidateIdAttribute(attribute);
        }

        private void PreprocessLocalElement(XmlSchemaElement element)
        {
            if (element.Name != null)
            {
                this.ValidateNameAttribute(element);
                this.PreprocessElementContent(element);
                element.SetQualifiedName(new XmlQualifiedName(element.Name, ((element.Form == XmlSchemaForm.Qualified) || ((element.Form == XmlSchemaForm.None) && (this.elementFormDefault == XmlSchemaForm.Qualified))) ? this.targetNamespace : null));
            }
            else
            {
                this.PreprocessAnnotation(element);
                if (element.RefName.IsEmpty)
                {
                    base.SendValidationEvent("Sch_ElementNameRef", element);
                }
                else
                {
                    this.ValidateQNameAttribute(element, "ref", element.RefName);
                }
                if (((!element.SchemaTypeName.IsEmpty || element.IsAbstract) || ((element.Block != XmlSchemaDerivationMethod.None) || (element.SchemaType != null))) || ((element.HasConstraints || (element.DefaultValue != null)) || (((element.Form != XmlSchemaForm.None) || (element.FixedValue != null)) || element.HasNillableAttribute)))
                {
                    base.SendValidationEvent("Sch_InvalidElementRef", element);
                }
                if ((element.DefaultValue != null) && (element.FixedValue != null))
                {
                    base.SendValidationEvent("Sch_DefaultFixedAttributes", element);
                }
                element.SetQualifiedName(element.RefName);
            }
            if (element.MinOccurs > element.MaxOccurs)
            {
                element.MinOccurs = 0M;
                base.SendValidationEvent("Sch_MinGtMax", element);
            }
            if (element.IsAbstract)
            {
                base.SendValidationEvent("Sch_ForbiddenAttribute", "abstract", element);
            }
            if (element.Final != XmlSchemaDerivationMethod.None)
            {
                base.SendValidationEvent("Sch_ForbiddenAttribute", "final", element);
            }
            if (!element.SubstitutionGroup.IsEmpty)
            {
                base.SendValidationEvent("Sch_ForbiddenAttribute", "substitutionGroup", element);
            }
            this.ValidateIdAttribute(element);
        }

        private void PreprocessNotation(XmlSchemaNotation notation)
        {
            if (notation.Name != null)
            {
                this.ValidateNameAttribute(notation);
                notation.QualifiedName = new XmlQualifiedName(notation.Name, this.targetNamespace);
            }
            else
            {
                base.SendValidationEvent("Sch_MissRequiredAttribute", "name", notation);
            }
            if (notation.Public != null)
            {
                try
                {
                    XmlConvert.ToUri(notation.Public);
                }
                catch
                {
                    base.SendValidationEvent("Sch_InvalidPublicAttribute", notation.Public, notation);
                }
            }
            else
            {
                base.SendValidationEvent("Sch_MissRequiredAttribute", "public", notation);
            }
            if (notation.System != null)
            {
                try
                {
                    XmlConvert.ToUri(notation.System);
                }
                catch
                {
                    base.SendValidationEvent("Sch_InvalidSystemAttribute", notation.System, notation);
                }
            }
            this.PreprocessAnnotation(notation);
            this.ValidateIdAttribute(notation);
        }

        private void PreprocessParticle(XmlSchemaParticle particle)
        {
            if (particle is XmlSchemaAll)
            {
                if ((particle.MinOccurs != 0M) && (particle.MinOccurs != 1M))
                {
                    particle.MinOccurs = 1M;
                    base.SendValidationEvent("Sch_InvalidAllMin", particle);
                }
                if (particle.MaxOccurs != 1M)
                {
                    particle.MaxOccurs = 1M;
                    base.SendValidationEvent("Sch_InvalidAllMax", particle);
                }
                foreach (XmlSchemaElement element in ((XmlSchemaAll) particle).Items)
                {
                    if ((element.MaxOccurs != 0M) && (element.MaxOccurs != 1M))
                    {
                        element.MaxOccurs = 1M;
                        base.SendValidationEvent("Sch_InvalidAllElementMax", element);
                    }
                    this.SetParent(element, particle);
                    this.PreprocessLocalElement(element);
                }
            }
            else
            {
                if (particle.MinOccurs > particle.MaxOccurs)
                {
                    particle.MinOccurs = particle.MaxOccurs;
                    base.SendValidationEvent("Sch_MinGtMax", particle);
                }
                if (particle is XmlSchemaChoice)
                {
                    foreach (XmlSchemaObject obj2 in ((XmlSchemaChoice) particle).Items)
                    {
                        this.SetParent(obj2, particle);
                        if (obj2 is XmlSchemaElement)
                        {
                            this.PreprocessLocalElement((XmlSchemaElement) obj2);
                        }
                        else
                        {
                            this.PreprocessParticle((XmlSchemaParticle) obj2);
                        }
                    }
                }
                else if (particle is XmlSchemaSequence)
                {
                    foreach (XmlSchemaObject obj3 in ((XmlSchemaSequence) particle).Items)
                    {
                        this.SetParent(obj3, particle);
                        if (obj3 is XmlSchemaElement)
                        {
                            this.PreprocessLocalElement((XmlSchemaElement) obj3);
                        }
                        else
                        {
                            this.PreprocessParticle((XmlSchemaParticle) obj3);
                        }
                    }
                }
                else if (particle is XmlSchemaGroupRef)
                {
                    XmlSchemaGroupRef source = (XmlSchemaGroupRef) particle;
                    if (source.RefName.IsEmpty)
                    {
                        base.SendValidationEvent("Sch_MissAttribute", "ref", source);
                    }
                    else
                    {
                        this.ValidateQNameAttribute(source, "ref", source.RefName);
                    }
                }
                else if (particle is XmlSchemaAny)
                {
                    try
                    {
                        ((XmlSchemaAny) particle).BuildNamespaceListV1Compat(this.targetNamespace);
                    }
                    catch
                    {
                        base.SendValidationEvent("Sch_InvalidAny", particle);
                    }
                }
            }
            this.PreprocessAnnotation(particle);
            this.ValidateIdAttribute(particle);
        }

        private void PreprocessRedefine(XmlSchemaRedefine redefine)
        {
            foreach (XmlSchemaObject obj2 in redefine.Items)
            {
                this.SetParent(obj2, redefine);
                if (obj2 is XmlSchemaGroup)
                {
                    XmlSchemaGroup group = (XmlSchemaGroup) obj2;
                    this.PreprocessGroup(group);
                    if (redefine.Groups[group.QualifiedName] != null)
                    {
                        base.SendValidationEvent("Sch_GroupDoubleRedefine", group);
                    }
                    else
                    {
                        base.AddToTable(redefine.Groups, group.QualifiedName, group);
                        group.Redefined = (XmlSchemaGroup) redefine.Schema.Groups[group.QualifiedName];
                        if (group.Redefined != null)
                        {
                            this.CheckRefinedGroup(group);
                        }
                        else
                        {
                            base.SendValidationEvent("Sch_GroupRedefineNotFound", group);
                        }
                    }
                }
                else if (obj2 is XmlSchemaAttributeGroup)
                {
                    XmlSchemaAttributeGroup attributeGroup = (XmlSchemaAttributeGroup) obj2;
                    this.PreprocessAttributeGroup(attributeGroup);
                    if (redefine.AttributeGroups[attributeGroup.QualifiedName] != null)
                    {
                        base.SendValidationEvent("Sch_AttrGroupDoubleRedefine", attributeGroup);
                    }
                    else
                    {
                        base.AddToTable(redefine.AttributeGroups, attributeGroup.QualifiedName, attributeGroup);
                        attributeGroup.Redefined = (XmlSchemaAttributeGroup) redefine.Schema.AttributeGroups[attributeGroup.QualifiedName];
                        if (attributeGroup.Redefined != null)
                        {
                            this.CheckRefinedAttributeGroup(attributeGroup);
                        }
                        else
                        {
                            base.SendValidationEvent("Sch_AttrGroupRedefineNotFound", attributeGroup);
                        }
                    }
                }
                else if (obj2 is XmlSchemaComplexType)
                {
                    XmlSchemaComplexType complexType = (XmlSchemaComplexType) obj2;
                    this.PreprocessComplexType(complexType, false);
                    if (redefine.SchemaTypes[complexType.QualifiedName] != null)
                    {
                        base.SendValidationEvent("Sch_ComplexTypeDoubleRedefine", complexType);
                    }
                    else
                    {
                        base.AddToTable(redefine.SchemaTypes, complexType.QualifiedName, complexType);
                        XmlSchemaType type2 = (XmlSchemaType) redefine.Schema.SchemaTypes[complexType.QualifiedName];
                        if (type2 != null)
                        {
                            if (type2 is XmlSchemaComplexType)
                            {
                                complexType.Redefined = type2;
                                this.CheckRefinedComplexType(complexType);
                            }
                            else
                            {
                                base.SendValidationEvent("Sch_SimpleToComplexTypeRedefine", complexType);
                            }
                        }
                        else
                        {
                            base.SendValidationEvent("Sch_ComplexTypeRedefineNotFound", complexType);
                        }
                    }
                }
                else if (obj2 is XmlSchemaSimpleType)
                {
                    XmlSchemaSimpleType simpleType = (XmlSchemaSimpleType) obj2;
                    this.PreprocessSimpleType(simpleType, false);
                    if (redefine.SchemaTypes[simpleType.QualifiedName] != null)
                    {
                        base.SendValidationEvent("Sch_SimpleTypeDoubleRedefine", simpleType);
                    }
                    else
                    {
                        base.AddToTable(redefine.SchemaTypes, simpleType.QualifiedName, simpleType);
                        XmlSchemaType type4 = (XmlSchemaType) redefine.Schema.SchemaTypes[simpleType.QualifiedName];
                        if (type4 != null)
                        {
                            if (type4 is XmlSchemaSimpleType)
                            {
                                simpleType.Redefined = type4;
                                this.CheckRefinedSimpleType(simpleType);
                            }
                            else
                            {
                                base.SendValidationEvent("Sch_ComplexToSimpleTypeRedefine", simpleType);
                            }
                        }
                        else
                        {
                            base.SendValidationEvent("Sch_SimpleTypeRedefineNotFound", simpleType);
                        }
                    }
                }
            }
            foreach (DictionaryEntry entry in redefine.Groups)
            {
                redefine.Schema.Groups.Insert((XmlQualifiedName) entry.Key, (XmlSchemaObject) entry.Value);
            }
            foreach (DictionaryEntry entry2 in redefine.AttributeGroups)
            {
                redefine.Schema.AttributeGroups.Insert((XmlQualifiedName) entry2.Key, (XmlSchemaObject) entry2.Value);
            }
            foreach (DictionaryEntry entry3 in redefine.SchemaTypes)
            {
                redefine.Schema.SchemaTypes.Insert((XmlQualifiedName) entry3.Key, (XmlSchemaObject) entry3.Value);
            }
        }

        private void PreprocessSimpleType(XmlSchemaSimpleType simpleType, bool local)
        {
            if (local)
            {
                if (simpleType.Name != null)
                {
                    base.SendValidationEvent("Sch_ForbiddenAttribute", "name", simpleType);
                }
            }
            else
            {
                if (simpleType.Name != null)
                {
                    this.ValidateNameAttribute(simpleType);
                    simpleType.SetQualifiedName(new XmlQualifiedName(simpleType.Name, this.targetNamespace));
                }
                else
                {
                    base.SendValidationEvent("Sch_MissRequiredAttribute", "name", simpleType);
                }
                if (simpleType.Final == XmlSchemaDerivationMethod.All)
                {
                    simpleType.SetFinalResolved(XmlSchemaDerivationMethod.All);
                }
                else if (simpleType.Final == XmlSchemaDerivationMethod.None)
                {
                    if (this.finalDefault == XmlSchemaDerivationMethod.All)
                    {
                        simpleType.SetFinalResolved(XmlSchemaDerivationMethod.All);
                    }
                    else
                    {
                        simpleType.SetFinalResolved(this.finalDefault & (XmlSchemaDerivationMethod.Union | XmlSchemaDerivationMethod.List | XmlSchemaDerivationMethod.Restriction));
                    }
                }
                else
                {
                    if ((simpleType.Final & ~(XmlSchemaDerivationMethod.Union | XmlSchemaDerivationMethod.List | XmlSchemaDerivationMethod.Restriction)) != XmlSchemaDerivationMethod.Empty)
                    {
                        base.SendValidationEvent("Sch_InvalidSimpleTypeFinalValue", simpleType);
                    }
                    simpleType.SetFinalResolved(simpleType.Final & (XmlSchemaDerivationMethod.Union | XmlSchemaDerivationMethod.List | XmlSchemaDerivationMethod.Restriction));
                }
            }
            if (simpleType.Content == null)
            {
                base.SendValidationEvent("Sch_NoSimpleTypeContent", simpleType);
            }
            else if (simpleType.Content is XmlSchemaSimpleTypeRestriction)
            {
                XmlSchemaSimpleTypeRestriction content = (XmlSchemaSimpleTypeRestriction) simpleType.Content;
                this.SetParent(content, simpleType);
                foreach (XmlSchemaObject obj2 in content.Facets)
                {
                    this.SetParent(obj2, content);
                }
                if (content.BaseType != null)
                {
                    if (!content.BaseTypeName.IsEmpty)
                    {
                        base.SendValidationEvent("Sch_SimpleTypeRestRefBase", content);
                    }
                    this.PreprocessSimpleType(content.BaseType, true);
                }
                else if (content.BaseTypeName.IsEmpty)
                {
                    base.SendValidationEvent("Sch_SimpleTypeRestRefBaseNone", content);
                }
                else
                {
                    this.ValidateQNameAttribute(content, "base", content.BaseTypeName);
                }
                this.PreprocessAnnotation(content);
                this.ValidateIdAttribute(content);
            }
            else if (simpleType.Content is XmlSchemaSimpleTypeList)
            {
                XmlSchemaSimpleTypeList child = (XmlSchemaSimpleTypeList) simpleType.Content;
                this.SetParent(child, simpleType);
                if (child.ItemType != null)
                {
                    if (!child.ItemTypeName.IsEmpty)
                    {
                        base.SendValidationEvent("Sch_SimpleTypeListRefBase", child);
                    }
                    this.SetParent(child.ItemType, child);
                    this.PreprocessSimpleType(child.ItemType, true);
                }
                else if (child.ItemTypeName.IsEmpty)
                {
                    base.SendValidationEvent("Sch_SimpleTypeListRefBaseNone", child);
                }
                else
                {
                    this.ValidateQNameAttribute(child, "itemType", child.ItemTypeName);
                }
                this.PreprocessAnnotation(child);
                this.ValidateIdAttribute(child);
            }
            else
            {
                XmlSchemaSimpleTypeUnion union = (XmlSchemaSimpleTypeUnion) simpleType.Content;
                this.SetParent(union, simpleType);
                int count = union.BaseTypes.Count;
                if (union.MemberTypes != null)
                {
                    count += union.MemberTypes.Length;
                    foreach (XmlQualifiedName name in union.MemberTypes)
                    {
                        this.ValidateQNameAttribute(union, "memberTypes", name);
                    }
                }
                if (count == 0)
                {
                    base.SendValidationEvent("Sch_SimpleTypeUnionNoBase", union);
                }
                foreach (XmlSchemaSimpleType type in union.BaseTypes)
                {
                    this.SetParent(type, union);
                    this.PreprocessSimpleType(type, true);
                }
                this.PreprocessAnnotation(union);
                this.ValidateIdAttribute(union);
            }
            this.ValidateIdAttribute(simpleType);
        }

        private Uri ResolveSchemaLocationUri(XmlSchema enclosingSchema, string location)
        {
            try
            {
                return this.xmlResolver.ResolveUri(enclosingSchema.BaseUri, location);
            }
            catch
            {
                return null;
            }
        }

        private void SetParent(XmlSchemaObject child, XmlSchemaObject parent)
        {
            child.Parent = parent;
        }

        private void ValidateIdAttribute(XmlSchemaObject xso)
        {
            if (xso.IdAttribute != null)
            {
                try
                {
                    xso.IdAttribute = base.NameTable.Add(XmlConvert.VerifyNCName(xso.IdAttribute));
                    if (this.schema.Ids[xso.IdAttribute] != null)
                    {
                        base.SendValidationEvent("Sch_DupIdAttribute", xso);
                    }
                    else
                    {
                        this.schema.Ids.Add(xso.IdAttribute, xso);
                    }
                }
                catch (Exception exception)
                {
                    base.SendValidationEvent("Sch_InvalidIdAttribute", exception.Message, xso);
                }
            }
        }

        private void ValidateNameAttribute(XmlSchemaObject xso)
        {
            string nameAttribute = xso.NameAttribute;
            if ((nameAttribute == null) || (nameAttribute.Length == 0))
            {
                base.SendValidationEvent("Sch_InvalidNameAttributeEx", null, Res.GetString("Sch_NullValue"), xso);
            }
            nameAttribute = XmlComplianceUtil.NonCDataNormalize(nameAttribute);
            int num = ValidateNames.ParseNCName(nameAttribute, 0);
            if (num != nameAttribute.Length)
            {
                string str2 = Res.GetString("Xml_BadNameCharWithPos", new object[] { XmlException.BuildCharExceptionStr(nameAttribute[num])[0], XmlException.BuildCharExceptionStr(nameAttribute[num])[1], num });
                base.SendValidationEvent("Sch_InvalidNameAttributeEx", nameAttribute, str2, xso);
            }
            else
            {
                xso.NameAttribute = base.NameTable.Add(nameAttribute);
            }
        }

        private void ValidateQNameAttribute(XmlSchemaObject xso, string attributeName, XmlQualifiedName value)
        {
            try
            {
                value.Verify();
                value.Atomize(base.NameTable);
                if (this.referenceNamespaces[value.Namespace] == null)
                {
                    base.SendValidationEvent("Sch_UnrefNS", value.Namespace, xso, XmlSeverityType.Warning);
                }
            }
            catch (Exception exception)
            {
                base.SendValidationEvent("Sch_InvalidAttribute", attributeName, exception.Message, xso);
            }
        }

        internal System.Xml.XmlResolver XmlResolver
        {
            set
            {
                this.xmlResolver = value;
            }
        }

        private enum Compositor
        {
            Root,
            Include,
            Import
        }
    }
}

