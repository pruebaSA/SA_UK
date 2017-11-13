namespace System.Xml.Schema
{
    using System;
    using System.Collections;
    using System.Runtime.InteropServices;
    using System.Xml;

    internal class SchemaInfo
    {
        private Hashtable attributeDecls = new Hashtable();
        private XmlQualifiedName docTypeName = XmlQualifiedName.Empty;
        private Hashtable elementDecls = new Hashtable();
        private Hashtable elementDeclsByType = new Hashtable();
        private int errorCount;
        private Hashtable generalEntities;
        private bool hasDefaultAttributes;
        private bool hasNonCDataAttributes;
        private Hashtable notations;
        private Hashtable parameterEntities;
        private System.Xml.Schema.SchemaType schemaType = System.Xml.Schema.SchemaType.None;
        private Hashtable targetNamespaces = new Hashtable();
        private Hashtable undeclaredElementDecls = new Hashtable();

        public void Add(SchemaInfo sinfo, ValidationEventHandler eventhandler)
        {
            if (this.schemaType == System.Xml.Schema.SchemaType.None)
            {
                this.schemaType = sinfo.SchemaType;
            }
            else if (this.schemaType != sinfo.SchemaType)
            {
                if (eventhandler != null)
                {
                    eventhandler(this, new ValidationEventArgs(new XmlSchemaException("Sch_MixSchemaTypes", string.Empty)));
                }
                return;
            }
            foreach (string str in sinfo.TargetNamespaces.Keys)
            {
                if (!this.targetNamespaces.ContainsKey(str))
                {
                    this.targetNamespaces.Add(str, true);
                }
            }
            foreach (DictionaryEntry entry in sinfo.elementDecls)
            {
                if (!this.elementDecls.ContainsKey(entry.Key))
                {
                    this.elementDecls.Add(entry.Key, entry.Value);
                }
            }
            foreach (DictionaryEntry entry2 in sinfo.elementDeclsByType)
            {
                if (!this.elementDeclsByType.ContainsKey(entry2.Key))
                {
                    this.elementDeclsByType.Add(entry2.Key, entry2.Value);
                }
            }
            foreach (SchemaAttDef def in sinfo.AttributeDecls.Values)
            {
                if (!this.attributeDecls.ContainsKey(def.Name))
                {
                    this.attributeDecls.Add(def.Name, def);
                }
            }
            foreach (SchemaNotation notation in sinfo.Notations.Values)
            {
                if (!this.Notations.ContainsKey(notation.Name.Name))
                {
                    this.Notations.Add(notation.Name.Name, notation);
                }
            }
        }

        public bool Contains(string ns) => 
            (this.targetNamespaces[ns] != null);

        public void Finish()
        {
            Hashtable elementDecls = this.elementDecls;
            for (int i = 0; i < 2; i++)
            {
                foreach (SchemaElementDecl decl in elementDecls.Values)
                {
                    decl.EndAddAttDef();
                    if (decl.HasNonCDataAttribute)
                    {
                        this.hasNonCDataAttributes = true;
                    }
                    if (decl.DefaultAttDefs != null)
                    {
                        this.hasDefaultAttributes = true;
                    }
                }
                elementDecls = this.undeclaredElementDecls;
            }
        }

        public XmlSchemaAttribute GetAttribute(XmlQualifiedName qname)
        {
            SchemaAttDef def = (SchemaAttDef) this.attributeDecls[qname];
            if (def != null)
            {
                return def.SchemaAttribute;
            }
            return null;
        }

        public SchemaAttDef GetAttributeXdr(SchemaElementDecl ed, XmlQualifiedName qname)
        {
            SchemaAttDef attDef = null;
            if (ed != null)
            {
                attDef = ed.GetAttDef(qname);
                if (attDef != null)
                {
                    return attDef;
                }
                if (!ed.ContentValidator.IsOpen || (qname.Namespace.Length == 0))
                {
                    throw new XmlSchemaException("Sch_UndeclaredAttribute", qname.ToString());
                }
                attDef = (SchemaAttDef) this.attributeDecls[qname];
                if ((attDef == null) && this.targetNamespaces.Contains(qname.Namespace))
                {
                    throw new XmlSchemaException("Sch_UndeclaredAttribute", qname.ToString());
                }
            }
            return attDef;
        }

        public SchemaAttDef GetAttributeXsd(SchemaElementDecl ed, XmlQualifiedName qname, ref bool skip)
        {
            AttributeMatchState state;
            SchemaAttDef def = this.GetAttributeXsd(ed, qname, null, out state);
            switch (state)
            {
                case AttributeMatchState.AttributeFound:
                case AttributeMatchState.AnyIdAttributeFound:
                case AttributeMatchState.UndeclaredElementAndAttribute:
                case AttributeMatchState.AnyAttributeLax:
                    return def;

                case AttributeMatchState.UndeclaredAttribute:
                    throw new XmlSchemaException("Sch_UndeclaredAttribute", qname.ToString());

                case AttributeMatchState.AnyAttributeSkip:
                    skip = true;
                    return def;

                case AttributeMatchState.ProhibitedAnyAttribute:
                case AttributeMatchState.ProhibitedAttribute:
                    throw new XmlSchemaException("Sch_ProhibitedAttribute", qname.ToString());
            }
            return def;
        }

        public SchemaAttDef GetAttributeXsd(SchemaElementDecl ed, XmlQualifiedName qname, XmlSchemaObject partialValidationType, out AttributeMatchState attributeMatchState)
        {
            SchemaAttDef attDef = null;
            attributeMatchState = AttributeMatchState.UndeclaredAttribute;
            if (ed != null)
            {
                attDef = ed.GetAttDef(qname);
                if (attDef != null)
                {
                    attributeMatchState = AttributeMatchState.AttributeFound;
                    return attDef;
                }
                XmlSchemaAnyAttribute anyAttribute = ed.AnyAttribute;
                if (anyAttribute != null)
                {
                    if (!anyAttribute.NamespaceList.Allows(qname))
                    {
                        attributeMatchState = AttributeMatchState.ProhibitedAnyAttribute;
                        return attDef;
                    }
                    if (anyAttribute.ProcessContentsCorrect != XmlSchemaContentProcessing.Skip)
                    {
                        attDef = (SchemaAttDef) this.attributeDecls[qname];
                        if (attDef != null)
                        {
                            if (attDef.Datatype.TypeCode == XmlTypeCode.Id)
                            {
                                attributeMatchState = AttributeMatchState.AnyIdAttributeFound;
                                return attDef;
                            }
                            attributeMatchState = AttributeMatchState.AttributeFound;
                            return attDef;
                        }
                        if (anyAttribute.ProcessContentsCorrect == XmlSchemaContentProcessing.Lax)
                        {
                            attributeMatchState = AttributeMatchState.AnyAttributeLax;
                        }
                        return attDef;
                    }
                    attributeMatchState = AttributeMatchState.AnyAttributeSkip;
                    return attDef;
                }
                if (ed.ProhibitedAttributes[qname] != null)
                {
                    attributeMatchState = AttributeMatchState.ProhibitedAttribute;
                }
                return attDef;
            }
            if (partialValidationType != null)
            {
                XmlSchemaAttribute attribute2 = partialValidationType as XmlSchemaAttribute;
                if (attribute2 != null)
                {
                    if (qname.Equals(attribute2.QualifiedName))
                    {
                        attDef = attribute2.AttDef;
                        attributeMatchState = AttributeMatchState.AttributeFound;
                        return attDef;
                    }
                    attributeMatchState = AttributeMatchState.AttributeNameMismatch;
                    return attDef;
                }
                attributeMatchState = AttributeMatchState.ValidateAttributeInvalidCall;
                return attDef;
            }
            attDef = (SchemaAttDef) this.attributeDecls[qname];
            if (attDef != null)
            {
                attributeMatchState = AttributeMatchState.AttributeFound;
                return attDef;
            }
            attributeMatchState = AttributeMatchState.UndeclaredElementAndAttribute;
            return attDef;
        }

        public XmlSchemaElement GetElement(XmlQualifiedName qname)
        {
            SchemaElementDecl elementDecl = this.GetElementDecl(qname);
            if (elementDecl != null)
            {
                return elementDecl.SchemaElement;
            }
            return null;
        }

        public SchemaElementDecl GetElementDecl(XmlQualifiedName qname) => 
            ((SchemaElementDecl) this.elementDecls[qname]);

        public XmlSchemaElement GetType(XmlQualifiedName qname)
        {
            SchemaElementDecl elementDecl = this.GetElementDecl(qname);
            if (elementDecl != null)
            {
                return elementDecl.SchemaElement;
            }
            return null;
        }

        public SchemaElementDecl GetTypeDecl(XmlQualifiedName qname) => 
            ((SchemaElementDecl) this.elementDeclsByType[qname]);

        public bool HasSchema(string ns) => 
            (this.targetNamespaces[ns] != null);

        public Hashtable AttributeDecls =>
            this.attributeDecls;

        public XmlQualifiedName DocTypeName
        {
            get => 
                this.docTypeName;
            set
            {
                this.docTypeName = value;
            }
        }

        public Hashtable ElementDecls =>
            this.elementDecls;

        public Hashtable ElementDeclsByType =>
            this.elementDeclsByType;

        public int ErrorCount
        {
            get => 
                this.errorCount;
            set
            {
                this.errorCount = value;
            }
        }

        public Hashtable GeneralEntities
        {
            get
            {
                if (this.generalEntities == null)
                {
                    this.generalEntities = new Hashtable();
                }
                return this.generalEntities;
            }
        }

        internal bool HasDefaultAttributes
        {
            get => 
                this.hasDefaultAttributes;
            set
            {
                this.hasDefaultAttributes = value;
            }
        }

        internal bool HasNonCDataAttributes
        {
            get => 
                this.hasNonCDataAttributes;
            set
            {
                this.hasNonCDataAttributes = value;
            }
        }

        public Hashtable Notations
        {
            get
            {
                if (this.notations == null)
                {
                    this.notations = new Hashtable();
                }
                return this.notations;
            }
        }

        public Hashtable ParameterEntities
        {
            get
            {
                if (this.parameterEntities == null)
                {
                    this.parameterEntities = new Hashtable();
                }
                return this.parameterEntities;
            }
        }

        public System.Xml.Schema.SchemaType SchemaType
        {
            get => 
                this.schemaType;
            set
            {
                this.schemaType = value;
            }
        }

        public Hashtable TargetNamespaces =>
            this.targetNamespaces;

        public Hashtable UndeclaredElementDecls =>
            this.undeclaredElementDecls;
    }
}

