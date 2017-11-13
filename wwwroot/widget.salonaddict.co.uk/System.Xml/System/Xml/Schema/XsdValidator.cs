namespace System.Xml.Schema
{
    using System;
    using System.Collections;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Xml;

    internal sealed class XsdValidator : BaseValidator
    {
        private Hashtable attPresence;
        private bool bManageNamespaces;
        private static readonly XmlSchemaDatatype dtCDATA = XmlSchemaDatatype.FromXmlTokenizedType(XmlTokenizedType.CDATA);
        private static readonly XmlSchemaDatatype dtQName = XmlSchemaDatatype.FromXmlTokenizedTypeXsd(XmlTokenizedType.QName);
        private static readonly XmlSchemaDatatype dtStringArray = dtCDATA.DeriveByList(null);
        private IdRefNode idRefListHead;
        private Hashtable IDs;
        private System.Xml.Schema.Parser inlineSchemaParser;
        private XmlNamespaceManager nsManager;
        private string NsXmlNs;
        private string NsXs;
        private string NsXsi;
        private XmlSchemaContentProcessing processContents;
        private const int STACK_INCREMENT = 10;
        private int startIDConstraint;
        private HWStack validationStack;
        private string XsdSchema;
        private string XsiNil;
        private string XsiNoNamespaceSchemaLocation;
        private string XsiSchemaLocation;
        private string XsiType;

        internal XsdValidator(BaseValidator validator) : base(validator)
        {
            this.startIDConstraint = -1;
            this.Init();
        }

        internal XsdValidator(XmlValidatingReaderImpl reader, XmlSchemaCollection schemaCollection, ValidationEventHandler eventHandler) : base(reader, schemaCollection, eventHandler)
        {
            this.startIDConstraint = -1;
            this.Init();
        }

        internal void AddID(string name, object node)
        {
            if (this.IDs == null)
            {
                this.IDs = new Hashtable();
            }
            this.IDs.Add(name, node);
        }

        private void AddIdentityConstraints()
        {
            base.context.Constr = new ConstraintStruct[base.context.ElementDecl.Constraints.Length];
            int num = 0;
            foreach (CompiledIdentityConstraint constraint in base.context.ElementDecl.Constraints)
            {
                base.context.Constr[num++] = new ConstraintStruct(constraint);
            }
            foreach (ConstraintStruct struct2 in base.context.Constr)
            {
                if (struct2.constraint.Role != CompiledIdentityConstraint.ConstraintRole.Keyref)
                {
                    continue;
                }
                bool flag = false;
                for (int i = this.validationStack.Length - 1; i >= ((this.startIDConstraint >= 0) ? this.startIDConstraint : (this.validationStack.Length - 1)); i--)
                {
                    if (((ValidationState) this.validationStack[i]).Constr == null)
                    {
                        continue;
                    }
                    foreach (ConstraintStruct struct3 in ((ValidationState) this.validationStack[i]).Constr)
                    {
                        if (struct3.constraint.name == struct2.constraint.refer)
                        {
                            flag = true;
                            if (struct3.keyrefTable == null)
                            {
                                struct3.keyrefTable = new Hashtable();
                            }
                            struct2.qualifiedTable = struct3.keyrefTable;
                            break;
                        }
                    }
                    if (flag)
                    {
                        break;
                    }
                }
                if (!flag)
                {
                    base.SendValidationEvent("Sch_RefNotInScope", XmlSchemaValidator.QNameString(base.context.LocalName, base.context.Namespace));
                }
            }
            if (this.startIDConstraint == -1)
            {
                this.startIDConstraint = this.validationStack.Length - 1;
            }
        }

        private void AttributeIdentityConstraints(string name, string ns, object obj, string sobj, SchemaAttDef attdef)
        {
            for (int i = this.startIDConstraint; i < this.validationStack.Length; i++)
            {
                if (((ValidationState) this.validationStack[i]).Constr != null)
                {
                    foreach (ConstraintStruct struct2 in ((ValidationState) this.validationStack[i]).Constr)
                    {
                        foreach (LocatedActiveAxis axis in struct2.axisFields)
                        {
                            if (axis.MoveToAttribute(name, ns))
                            {
                                if (axis.Ks[axis.Column] != null)
                                {
                                    base.SendValidationEvent("Sch_FieldSingleValueExpected", name);
                                }
                                else if ((attdef != null) && (attdef.Datatype != null))
                                {
                                    axis.Ks[axis.Column] = new TypedObject(obj, sobj, attdef.Datatype);
                                }
                            }
                        }
                    }
                }
            }
        }

        private void CheckForwardRefs()
        {
            IdRefNode next;
            for (IdRefNode node = this.idRefListHead; node != null; node = next)
            {
                if (this.FindId(node.Id) == null)
                {
                    base.SendValidationEvent(new XmlSchemaException("Sch_UndeclaredId", node.Id, base.reader.BaseURI, node.LineNo, node.LinePos));
                }
                next = node.Next;
                node.Next = null;
            }
            this.idRefListHead = null;
        }

        private void CheckValue(string value, SchemaAttDef attdef)
        {
            try
            {
                base.reader.TypedValueObject = null;
                bool flag = attdef != null;
                XmlSchemaDatatype datatype = flag ? attdef.Datatype : base.context.ElementDecl.Datatype;
                if (datatype != null)
                {
                    object pVal = datatype.ParseValue(value, base.NameTable, this.nsManager, true);
                    switch (datatype.TokenizedType)
                    {
                        case XmlTokenizedType.ENTITY:
                        case XmlTokenizedType.ID:
                        case XmlTokenizedType.IDREF:
                            if (datatype.Variety == XmlSchemaDatatypeVariety.List)
                            {
                                string[] strArray = (string[]) pVal;
                                foreach (string str in strArray)
                                {
                                    this.ProcessTokenizedType(datatype.TokenizedType, str);
                                }
                            }
                            else
                            {
                                this.ProcessTokenizedType(datatype.TokenizedType, (string) pVal);
                            }
                            break;
                    }
                    SchemaDeclBase base2 = flag ? ((SchemaDeclBase) attdef) : ((SchemaDeclBase) base.context.ElementDecl);
                    if (!base2.CheckValue(pVal))
                    {
                        if (flag)
                        {
                            base.SendValidationEvent("Sch_FixedAttributeValue", attdef.Name.ToString());
                        }
                        else
                        {
                            base.SendValidationEvent("Sch_FixedElementValue", XmlSchemaValidator.QNameString(base.context.LocalName, base.context.Namespace));
                        }
                    }
                    if (datatype.Variety == XmlSchemaDatatypeVariety.Union)
                    {
                        pVal = this.UnWrapUnion(pVal);
                    }
                    base.reader.TypedValueObject = pVal;
                }
            }
            catch (XmlSchemaException)
            {
                if (attdef != null)
                {
                    base.SendValidationEvent("Sch_AttributeValueDataType", attdef.Name.ToString());
                }
                else
                {
                    base.SendValidationEvent("Sch_ElementValueDataType", XmlSchemaValidator.QNameString(base.context.LocalName, base.context.Namespace));
                }
            }
        }

        public override void CompleteValidation()
        {
            this.CheckForwardRefs();
        }

        private void ElementIdentityConstraints()
        {
            for (int i = this.startIDConstraint; i < this.validationStack.Length; i++)
            {
                if (((ValidationState) this.validationStack[i]).Constr != null)
                {
                    foreach (ConstraintStruct struct2 in ((ValidationState) this.validationStack[i]).Constr)
                    {
                        if (struct2.axisSelector.MoveToStartElement(base.reader.LocalName, base.reader.NamespaceURI))
                        {
                            struct2.axisSelector.PushKS(base.PositionInfo.LineNumber, base.PositionInfo.LinePosition);
                        }
                        foreach (LocatedActiveAxis axis in struct2.axisFields)
                        {
                            if (axis.MoveToStartElement(base.reader.LocalName, base.reader.NamespaceURI) && (base.context.ElementDecl != null))
                            {
                                if (base.context.ElementDecl.Datatype == null)
                                {
                                    base.SendValidationEvent("Sch_FieldSimpleTypeExpected", base.reader.LocalName);
                                }
                                else
                                {
                                    axis.isMatched = true;
                                }
                            }
                        }
                    }
                }
            }
        }

        private void EndElementIdentityConstraints()
        {
            for (int i = this.validationStack.Length - 1; i >= this.startIDConstraint; i--)
            {
                if (((ValidationState) this.validationStack[i]).Constr != null)
                {
                    foreach (ConstraintStruct struct2 in ((ValidationState) this.validationStack[i]).Constr)
                    {
                        KeySequence sequence;
                        foreach (LocatedActiveAxis axis in struct2.axisFields)
                        {
                            if (axis.isMatched)
                            {
                                axis.isMatched = false;
                                if (axis.Ks[axis.Column] != null)
                                {
                                    base.SendValidationEvent("Sch_FieldSingleValueExpected", base.reader.LocalName);
                                }
                                else
                                {
                                    string svalue = !base.hasSibling ? base.textString : base.textValue.ToString();
                                    if ((base.reader.TypedValueObject != null) && (svalue.Length != 0))
                                    {
                                        axis.Ks[axis.Column] = new TypedObject(base.reader.TypedValueObject, svalue, base.context.ElementDecl.Datatype);
                                    }
                                }
                            }
                            axis.EndElement(base.reader.LocalName, base.reader.NamespaceURI);
                        }
                        if (struct2.axisSelector.EndElement(base.reader.LocalName, base.reader.NamespaceURI))
                        {
                            sequence = struct2.axisSelector.PopKS();
                            switch (struct2.constraint.Role)
                            {
                                case CompiledIdentityConstraint.ConstraintRole.Unique:
                                    if (sequence.IsQualified())
                                    {
                                        if (!struct2.qualifiedTable.Contains(sequence))
                                        {
                                            goto Label_02DB;
                                        }
                                        base.SendValidationEvent(new XmlSchemaException("Sch_DuplicateKey", new string[] { sequence.ToString(), struct2.constraint.name.ToString() }, base.reader.BaseURI, sequence.PosLine, sequence.PosCol));
                                    }
                                    break;

                                case CompiledIdentityConstraint.ConstraintRole.Key:
                                    if (sequence.IsQualified())
                                    {
                                        goto Label_01F2;
                                    }
                                    base.SendValidationEvent(new XmlSchemaException("Sch_MissingKey", struct2.constraint.name.ToString(), base.reader.BaseURI, sequence.PosLine, sequence.PosCol));
                                    break;

                                case CompiledIdentityConstraint.ConstraintRole.Keyref:
                                    if (((struct2.qualifiedTable != null) && sequence.IsQualified()) && !struct2.qualifiedTable.Contains(sequence))
                                    {
                                        struct2.qualifiedTable.Add(sequence, sequence);
                                    }
                                    break;
                            }
                        }
                        continue;
                    Label_01F2:
                        if (struct2.qualifiedTable.Contains(sequence))
                        {
                            base.SendValidationEvent(new XmlSchemaException("Sch_DuplicateKey", new string[] { sequence.ToString(), struct2.constraint.name.ToString() }, base.reader.BaseURI, sequence.PosLine, sequence.PosCol));
                        }
                        else
                        {
                            struct2.qualifiedTable.Add(sequence, sequence);
                        }
                        continue;
                    Label_02DB:
                        struct2.qualifiedTable.Add(sequence, sequence);
                    }
                }
            }
            ConstraintStruct[] constr = ((ValidationState) this.validationStack[this.validationStack.Length - 1]).Constr;
            if (constr != null)
            {
                foreach (ConstraintStruct struct3 in constr)
                {
                    if ((struct3.constraint.Role != CompiledIdentityConstraint.ConstraintRole.Keyref) && (struct3.keyrefTable != null))
                    {
                        foreach (KeySequence sequence2 in struct3.keyrefTable.Keys)
                        {
                            if (!struct3.qualifiedTable.Contains(sequence2))
                            {
                                base.SendValidationEvent(new XmlSchemaException("Sch_UnresolvedKeyref", sequence2.ToString(), base.reader.BaseURI, sequence2.PosLine, sequence2.PosCol));
                            }
                        }
                    }
                }
            }
        }

        private SchemaElementDecl FastGetElementDecl(object particle)
        {
            if (particle != null)
            {
                XmlSchemaElement element = particle as XmlSchemaElement;
                if (element != null)
                {
                    return element.ElementDecl;
                }
                XmlSchemaAny any = (XmlSchemaAny) particle;
                this.processContents = any.ProcessContentsCorrect;
            }
            return null;
        }

        public override object FindId(string name)
        {
            if (this.IDs != null)
            {
                return this.IDs[name];
            }
            return null;
        }

        private void Init()
        {
            this.nsManager = base.reader.NamespaceManager;
            if (this.nsManager == null)
            {
                this.nsManager = new XmlNamespaceManager(base.NameTable);
                this.bManageNamespaces = true;
            }
            this.validationStack = new HWStack(10);
            base.textValue = new StringBuilder();
            this.attPresence = new Hashtable();
            base.schemaInfo = new SchemaInfo();
            base.checkDatatype = false;
            this.processContents = XmlSchemaContentProcessing.Strict;
            this.Push(XmlQualifiedName.Empty);
            this.NsXmlNs = base.NameTable.Add("http://www.w3.org/2000/xmlns/");
            this.NsXs = base.NameTable.Add("http://www.w3.org/2001/XMLSchema");
            this.NsXsi = base.NameTable.Add("http://www.w3.org/2001/XMLSchema-instance");
            this.XsiType = base.NameTable.Add("type");
            this.XsiNil = base.NameTable.Add("nil");
            this.XsiSchemaLocation = base.NameTable.Add("schemaLocation");
            this.XsiNoNamespaceSchemaLocation = base.NameTable.Add("noNamespaceSchemaLocation");
            this.XsdSchema = base.NameTable.Add("schema");
        }

        public bool IsXSDRoot(string localName, string ns) => 
            (Ref.Equal(ns, this.NsXs) && Ref.Equal(localName, this.XsdSchema));

        private void LoadSchema(string uri, string url)
        {
            if ((base.XmlResolver != null) && (!base.SchemaInfo.TargetNamespaces.Contains(uri) || (this.nsManager.LookupPrefix(uri) == null)))
            {
                SchemaInfo sinfo = null;
                if (base.SchemaCollection != null)
                {
                    sinfo = base.SchemaCollection.GetSchemaInfo(uri);
                }
                if (sinfo != null)
                {
                    if (sinfo.SchemaType != SchemaType.XSD)
                    {
                        throw new XmlException("Xml_MultipleValidaitonTypes", string.Empty, base.PositionInfo.LineNumber, base.PositionInfo.LinePosition);
                    }
                    base.SchemaInfo.Add(sinfo, base.EventHandler);
                }
                else if (url != null)
                {
                    this.LoadSchemaFromLocation(uri, url);
                }
            }
        }

        private void LoadSchemaFromLocation(string uri, string url)
        {
            XmlReader reader = null;
            SchemaInfo schemaInfo = null;
            try
            {
                Uri absoluteUri = base.XmlResolver.ResolveUri(base.BaseUri, url);
                Stream input = (Stream) base.XmlResolver.GetEntity(absoluteUri, null, null);
                reader = new XmlTextReader(absoluteUri.ToString(), input, base.NameTable);
                System.Xml.Schema.Parser parser = new System.Xml.Schema.Parser(SchemaType.XSD, base.NameTable, base.SchemaNames, base.EventHandler) {
                    XmlResolver = base.XmlResolver
                };
                SchemaType type = parser.Parse(reader, uri);
                schemaInfo = new SchemaInfo {
                    SchemaType = type
                };
                if (type == SchemaType.XSD)
                {
                    if (base.SchemaCollection.EventHandler == null)
                    {
                        base.SchemaCollection.EventHandler = base.EventHandler;
                    }
                    base.SchemaCollection.Add(uri, schemaInfo, parser.XmlSchema, true);
                }
                base.SchemaInfo.Add(schemaInfo, base.EventHandler);
                while (reader.Read())
                {
                }
            }
            catch (XmlSchemaException exception)
            {
                schemaInfo = null;
                base.SendValidationEvent("Sch_CannotLoadSchema", new string[] { uri, exception.Message }, XmlSeverityType.Error);
            }
            catch (Exception exception2)
            {
                schemaInfo = null;
                base.SendValidationEvent("Sch_CannotLoadSchema", new string[] { uri, exception2.Message }, XmlSeverityType.Warning);
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
            }
        }

        private void Pop()
        {
            if (this.validationStack.Length > 1)
            {
                this.validationStack.Pop();
                if (this.startIDConstraint == this.validationStack.Length)
                {
                    this.startIDConstraint = -1;
                }
                base.context = (ValidationState) this.validationStack.Peek();
                this.processContents = base.context.ProcessContents;
            }
        }

        private void ProcessElement(object particle)
        {
            XmlQualifiedName name;
            string str;
            SchemaElementDecl elementDecl = this.FastGetElementDecl(particle);
            this.Push(base.elementName);
            if (this.bManageNamespaces)
            {
                this.nsManager.PushScope();
            }
            this.ProcessXsiAttributes(out name, out str);
            if (this.processContents != XmlSchemaContentProcessing.Skip)
            {
                if (((elementDecl == null) || !name.IsEmpty) || (str != null))
                {
                    elementDecl = this.ThoroughGetElementDecl(elementDecl, name, str);
                }
                if (elementDecl == null)
                {
                    if (this.HasSchema && (this.processContents == XmlSchemaContentProcessing.Strict))
                    {
                        base.SendValidationEvent("Sch_UndeclaredElement", XmlSchemaValidator.QNameString(base.context.LocalName, base.context.Namespace));
                    }
                    else
                    {
                        base.SendValidationEvent("Sch_NoElementSchemaFound", XmlSchemaValidator.QNameString(base.context.LocalName, base.context.Namespace), XmlSeverityType.Warning);
                    }
                }
            }
            base.context.ElementDecl = elementDecl;
            this.ValidateStartElementIdentityConstraints();
            this.ValidateStartElement();
            if (base.context.ElementDecl != null)
            {
                this.ValidateEndStartElement();
                base.context.NeedValidateChildren = this.processContents != XmlSchemaContentProcessing.Skip;
                base.context.ElementDecl.ContentValidator.InitValidation(base.context);
            }
        }

        private void ProcessInlineSchema()
        {
            if (!this.inlineSchemaParser.ParseReaderNode())
            {
                this.inlineSchemaParser.FinishParsing();
                XmlSchema xmlSchema = this.inlineSchemaParser.XmlSchema;
                string key = null;
                if ((xmlSchema != null) && (xmlSchema.ErrorCount == 0))
                {
                    try
                    {
                        SchemaInfo schemaInfo = new SchemaInfo {
                            SchemaType = SchemaType.XSD
                        };
                        key = (xmlSchema.TargetNamespace == null) ? string.Empty : xmlSchema.TargetNamespace;
                        if (!base.SchemaInfo.TargetNamespaces.Contains(key) && (base.SchemaCollection.Add(key, schemaInfo, xmlSchema, true) != null))
                        {
                            base.SchemaInfo.Add(schemaInfo, base.EventHandler);
                        }
                    }
                    catch (XmlSchemaException exception)
                    {
                        base.SendValidationEvent("Sch_CannotLoadSchema", new string[] { base.BaseUri.AbsoluteUri, exception.Message }, XmlSeverityType.Error);
                    }
                }
                this.inlineSchemaParser = null;
            }
        }

        private void ProcessTokenizedType(XmlTokenizedType ttype, string name)
        {
            switch (ttype)
            {
                case XmlTokenizedType.ID:
                    if (this.FindId(name) == null)
                    {
                        this.AddID(name, base.context.LocalName);
                        return;
                    }
                    base.SendValidationEvent("Sch_DupId", name);
                    return;

                case XmlTokenizedType.IDREF:
                    if (this.FindId(name) != null)
                    {
                        break;
                    }
                    this.idRefListHead = new IdRefNode(this.idRefListHead, name, base.PositionInfo.LineNumber, base.PositionInfo.LinePosition);
                    return;

                case XmlTokenizedType.IDREFS:
                    break;

                case XmlTokenizedType.ENTITY:
                    BaseValidator.ProcessEntity(base.schemaInfo, name, this, base.EventHandler, base.reader.BaseURI, base.PositionInfo.LineNumber, base.PositionInfo.LinePosition);
                    break;

                default:
                    return;
            }
        }

        private void ProcessXsiAttributes(out XmlQualifiedName xsiType, out string xsiNil)
        {
            string[] strArray = null;
            string url = null;
            xsiType = XmlQualifiedName.Empty;
            xsiNil = null;
            if (base.reader.Depth == 0)
            {
                this.LoadSchema(string.Empty, null);
                foreach (string str2 in this.nsManager.GetNamespacesInScope(XmlNamespaceScope.ExcludeXml).Values)
                {
                    this.LoadSchema(str2, null);
                }
            }
            if (base.reader.MoveToFirstAttribute())
            {
                do
                {
                    string namespaceURI = base.reader.NamespaceURI;
                    string localName = base.reader.LocalName;
                    if (Ref.Equal(namespaceURI, this.NsXmlNs))
                    {
                        this.LoadSchema(base.reader.Value, null);
                        if (this.bManageNamespaces)
                        {
                            this.nsManager.AddNamespace((base.reader.Prefix.Length == 0) ? string.Empty : base.reader.LocalName, base.reader.Value);
                        }
                    }
                    else if (Ref.Equal(namespaceURI, this.NsXsi))
                    {
                        if (Ref.Equal(localName, this.XsiSchemaLocation))
                        {
                            strArray = (string[]) dtStringArray.ParseValue(base.reader.Value, base.NameTable, this.nsManager);
                        }
                        else if (Ref.Equal(localName, this.XsiNoNamespaceSchemaLocation))
                        {
                            url = base.reader.Value;
                        }
                        else if (Ref.Equal(localName, this.XsiType))
                        {
                            xsiType = (XmlQualifiedName) dtQName.ParseValue(base.reader.Value, base.NameTable, this.nsManager);
                        }
                        else if (Ref.Equal(localName, this.XsiNil))
                        {
                            xsiNil = base.reader.Value;
                        }
                    }
                }
                while (base.reader.MoveToNextAttribute());
                base.reader.MoveToElement();
            }
            if (url != null)
            {
                this.LoadSchema(string.Empty, url);
            }
            if (strArray != null)
            {
                for (int i = 0; i < (strArray.Length - 1); i += 2)
                {
                    this.LoadSchema(strArray[i], strArray[i + 1]);
                }
            }
        }

        private void Push(XmlQualifiedName elementName)
        {
            base.context = (ValidationState) this.validationStack.Push();
            if (base.context == null)
            {
                base.context = new ValidationState();
                this.validationStack.AddToTop(base.context);
            }
            base.context.LocalName = elementName.Name;
            base.context.Namespace = elementName.Namespace;
            base.context.HasMatched = false;
            base.context.IsNill = false;
            base.context.ProcessContents = this.processContents;
            base.context.NeedValidateChildren = false;
            base.context.Constr = null;
        }

        private SchemaElementDecl ThoroughGetElementDecl(SchemaElementDecl elementDecl, XmlQualifiedName xsiType, string xsiNil)
        {
            if (elementDecl == null)
            {
                elementDecl = base.schemaInfo.GetElementDecl(base.elementName);
            }
            if (elementDecl != null)
            {
                if (xsiType.IsEmpty)
                {
                    if (elementDecl.IsAbstract)
                    {
                        base.SendValidationEvent("Sch_AbstractElement", XmlSchemaValidator.QNameString(base.context.LocalName, base.context.Namespace));
                        elementDecl = null;
                    }
                }
                else if ((xsiNil != null) && xsiNil.Equals("true"))
                {
                    base.SendValidationEvent("Sch_XsiNilAndType");
                }
                else
                {
                    SchemaElementDecl decl = (SchemaElementDecl) base.schemaInfo.ElementDeclsByType[xsiType];
                    if ((decl == null) && (xsiType.Namespace == this.NsXs))
                    {
                        XmlSchemaSimpleType simpleTypeFromXsdType = DatatypeImplementation.GetSimpleTypeFromXsdType(new XmlQualifiedName(xsiType.Name, this.NsXs));
                        if (simpleTypeFromXsdType != null)
                        {
                            decl = simpleTypeFromXsdType.ElementDecl;
                        }
                    }
                    if (decl == null)
                    {
                        base.SendValidationEvent("Sch_XsiTypeNotFound", xsiType.ToString());
                        elementDecl = null;
                    }
                    else if (!XmlSchemaType.IsDerivedFrom(decl.SchemaType, elementDecl.SchemaType, elementDecl.Block))
                    {
                        base.SendValidationEvent("Sch_XsiTypeBlockedEx", new string[] { xsiType.ToString(), XmlSchemaValidator.QNameString(base.context.LocalName, base.context.Namespace) });
                        elementDecl = null;
                    }
                    else
                    {
                        elementDecl = decl;
                    }
                }
                if ((elementDecl != null) && elementDecl.IsNillable)
                {
                    if (xsiNil != null)
                    {
                        base.context.IsNill = XmlConvert.ToBoolean(xsiNil);
                        if (base.context.IsNill && (elementDecl.DefaultValueTyped != null))
                        {
                            base.SendValidationEvent("Sch_XsiNilAndFixed");
                        }
                    }
                    return elementDecl;
                }
                if (xsiNil != null)
                {
                    base.SendValidationEvent("Sch_InvalidXsiNill");
                }
            }
            return elementDecl;
        }

        private object UnWrapUnion(object typedValue)
        {
            XsdSimpleValue value2 = typedValue as XsdSimpleValue;
            if (value2 != null)
            {
                typedValue = value2.TypedValue;
            }
            return typedValue;
        }

        public override void Validate()
        {
            if (this.IsInlineSchemaStarted)
            {
                this.ProcessInlineSchema();
            }
            else
            {
                switch (base.reader.NodeType)
                {
                    case XmlNodeType.Element:
                        this.ValidateElement();
                        if (!base.reader.IsEmptyElement)
                        {
                            return;
                        }
                        break;

                    case XmlNodeType.Attribute:
                        return;

                    case XmlNodeType.Text:
                    case XmlNodeType.CDATA:
                    case XmlNodeType.SignificantWhitespace:
                        base.ValidateText();
                        return;

                    case XmlNodeType.Whitespace:
                        base.ValidateWhitespace();
                        return;

                    case XmlNodeType.EndElement:
                        break;

                    default:
                        return;
                }
                this.ValidateEndElement();
            }
        }

        private object ValidateChildElement()
        {
            object obj2 = null;
            int errorCode = 0;
            if (base.context.NeedValidateChildren)
            {
                if (base.context.IsNill)
                {
                    base.SendValidationEvent("Sch_ContentInNill", base.elementName.ToString());
                    return null;
                }
                obj2 = base.context.ElementDecl.ContentValidator.ValidateElement(base.elementName, base.context, out errorCode);
                if (obj2 != null)
                {
                    return obj2;
                }
                this.processContents = base.context.ProcessContents = XmlSchemaContentProcessing.Skip;
                if (errorCode == -2)
                {
                    base.SendValidationEvent("Sch_AllElement", base.elementName.ToString());
                }
                XmlSchemaValidator.ElementValidationError(base.elementName, base.context, base.EventHandler, base.reader, base.reader.BaseURI, base.PositionInfo.LineNumber, base.PositionInfo.LinePosition, false);
            }
            return obj2;
        }

        private void ValidateElement()
        {
            base.elementName.Init(base.reader.LocalName, base.reader.NamespaceURI);
            object particle = this.ValidateChildElement();
            if (this.IsXSDRoot(base.elementName.Name, base.elementName.Namespace) && (base.reader.Depth > 0))
            {
                this.inlineSchemaParser = new System.Xml.Schema.Parser(SchemaType.XSD, base.NameTable, base.SchemaNames, base.EventHandler);
                this.inlineSchemaParser.StartParsing(base.reader, null);
                this.inlineSchemaParser.ParseReaderNode();
            }
            else
            {
                this.ProcessElement(particle);
            }
        }

        private void ValidateEndElement()
        {
            if (this.bManageNamespaces)
            {
                this.nsManager.PopScope();
            }
            if (base.context.ElementDecl != null)
            {
                if (!base.context.IsNill)
                {
                    if (base.context.NeedValidateChildren && !base.context.ElementDecl.ContentValidator.CompleteValidation(base.context))
                    {
                        XmlSchemaValidator.CompleteValidationError(base.context, base.EventHandler, base.reader, base.reader.BaseURI, base.PositionInfo.LineNumber, base.PositionInfo.LinePosition, false);
                    }
                    if (base.checkDatatype && !base.context.IsNill)
                    {
                        string str = !base.hasSibling ? base.textString : base.textValue.ToString();
                        if ((str.Length != 0) || (base.context.ElementDecl.DefaultValueTyped == null))
                        {
                            this.CheckValue(str, null);
                            base.checkDatatype = false;
                        }
                    }
                }
                if (this.HasIdentityConstraints)
                {
                    this.EndElementIdentityConstraints();
                }
            }
            this.Pop();
        }

        private void ValidateEndStartElement()
        {
            if (base.context.ElementDecl.HasDefaultAttribute)
            {
                foreach (SchemaAttDef def in base.context.ElementDecl.DefaultAttDefs)
                {
                    base.reader.AddDefaultAttribute(def);
                    if (this.HasIdentityConstraints && !this.attPresence.Contains(def.Name))
                    {
                        this.AttributeIdentityConstraints(def.Name.Name, def.Name.Namespace, this.UnWrapUnion(def.DefaultValueTyped), def.DefaultValueRaw, def);
                    }
                }
            }
            if (base.context.ElementDecl.HasRequiredAttribute)
            {
                try
                {
                    base.context.ElementDecl.CheckAttributes(this.attPresence, base.reader.StandAlone);
                }
                catch (XmlSchemaException exception)
                {
                    exception.SetSource(base.reader.BaseURI, base.PositionInfo.LineNumber, base.PositionInfo.LinePosition);
                    base.SendValidationEvent(exception);
                }
            }
            if (base.context.ElementDecl.Datatype != null)
            {
                base.checkDatatype = true;
                base.hasSibling = false;
                base.textString = string.Empty;
                base.textValue.Length = 0;
            }
        }

        private void ValidateStartElement()
        {
            if (base.context.ElementDecl != null)
            {
                if (base.context.ElementDecl.IsAbstract)
                {
                    base.SendValidationEvent("Sch_AbstractElement", XmlSchemaValidator.QNameString(base.context.LocalName, base.context.Namespace));
                }
                base.reader.SchemaTypeObject = base.context.ElementDecl.SchemaType;
                if ((base.reader.IsEmptyElement && !base.context.IsNill) && (base.context.ElementDecl.DefaultValueTyped != null))
                {
                    base.reader.TypedValueObject = this.UnWrapUnion(base.context.ElementDecl.DefaultValueTyped);
                    base.context.IsNill = true;
                }
                else
                {
                    base.reader.TypedValueObject = null;
                }
                if (base.context.ElementDecl.HasRequiredAttribute || this.HasIdentityConstraints)
                {
                    this.attPresence.Clear();
                }
            }
            if (base.reader.MoveToFirstAttribute())
            {
                do
                {
                    if ((base.reader.NamespaceURI != this.NsXmlNs) && (base.reader.NamespaceURI != this.NsXsi))
                    {
                        try
                        {
                            base.reader.SchemaTypeObject = null;
                            XmlQualifiedName qname = new XmlQualifiedName(base.reader.LocalName, base.reader.NamespaceURI);
                            bool skip = this.processContents == XmlSchemaContentProcessing.Skip;
                            SchemaAttDef def = base.schemaInfo.GetAttributeXsd(base.context.ElementDecl, qname, ref skip);
                            if (def != null)
                            {
                                if ((base.context.ElementDecl != null) && (base.context.ElementDecl.HasRequiredAttribute || (this.startIDConstraint != -1)))
                                {
                                    this.attPresence.Add(def.Name, def);
                                }
                                base.reader.SchemaTypeObject = def.SchemaType;
                                if (def.Datatype != null)
                                {
                                    this.CheckValue(base.reader.Value, def);
                                }
                                if (this.HasIdentityConstraints)
                                {
                                    this.AttributeIdentityConstraints(base.reader.LocalName, base.reader.NamespaceURI, base.reader.TypedValueObject, base.reader.Value, def);
                                }
                            }
                            else if (!skip)
                            {
                                if (((base.context.ElementDecl == null) && (this.processContents == XmlSchemaContentProcessing.Strict)) && ((qname.Namespace.Length != 0) && base.schemaInfo.Contains(qname.Namespace)))
                                {
                                    base.SendValidationEvent("Sch_UndeclaredAttribute", qname.ToString());
                                }
                                else
                                {
                                    base.SendValidationEvent("Sch_NoAttributeSchemaFound", qname.ToString(), XmlSeverityType.Warning);
                                }
                            }
                        }
                        catch (XmlSchemaException exception)
                        {
                            exception.SetSource(base.reader.BaseURI, base.PositionInfo.LineNumber, base.PositionInfo.LinePosition);
                            base.SendValidationEvent(exception);
                        }
                    }
                }
                while (base.reader.MoveToNextAttribute());
                base.reader.MoveToElement();
            }
        }

        private void ValidateStartElementIdentityConstraints()
        {
            if (base.context.ElementDecl != null)
            {
                if (base.context.ElementDecl.Constraints != null)
                {
                    this.AddIdentityConstraints();
                }
                if (this.HasIdentityConstraints)
                {
                    this.ElementIdentityConstraints();
                }
            }
        }

        public ValidationState Context
        {
            set
            {
                base.context = value;
            }
        }

        public static XmlSchemaDatatype DtQName =>
            dtQName;

        private bool HasIdentityConstraints =>
            (this.startIDConstraint != -1);

        private bool HasSchema =>
            (base.schemaInfo.SchemaType != SchemaType.None);

        private bool IsInlineSchemaStarted =>
            (this.inlineSchemaParser != null);

        public override bool PreserveWhitespace =>
            base.context.ElementDecl?.ContentValidator.PreserveWhitespace;
    }
}

