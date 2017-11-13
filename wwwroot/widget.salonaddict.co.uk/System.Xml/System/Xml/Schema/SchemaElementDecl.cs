namespace System.Xml.Schema
{
    using System;
    using System.Collections;
    using System.Xml;

    internal sealed class SchemaElementDecl : SchemaDeclBase
    {
        private XmlSchemaAnyAttribute anyAttribute;
        private Hashtable attdefs;
        private XmlSchemaDerivationMethod block;
        private CompiledIdentityConstraint[] constraints;
        private System.Xml.Schema.ContentValidator contentValidator;
        private SchemaAttDef[] defaultAttdefs;
        public static readonly SchemaElementDecl Empty = new SchemaElementDecl();
        private bool hasNonCDataAttribute;
        private bool hasRequiredAttribute;
        private bool isAbstract;
        private bool isIdDeclared;
        private bool isNillable;
        private bool isNotationDeclared;
        private Hashtable prohibitedAttributes;
        private XmlSchemaElement schemaElement;
        private ArrayList tmpDefaultAttdefs;

        public SchemaElementDecl()
        {
            this.attdefs = new Hashtable();
            this.prohibitedAttributes = new Hashtable();
        }

        public SchemaElementDecl(XmlSchemaDatatype dtype)
        {
            this.attdefs = new Hashtable();
            this.prohibitedAttributes = new Hashtable();
            base.Datatype = dtype;
            this.contentValidator = System.Xml.Schema.ContentValidator.TextOnly;
        }

        public SchemaElementDecl(XmlQualifiedName name, string prefix, SchemaType schemaType) : base(name, prefix)
        {
            this.attdefs = new Hashtable();
            this.prohibitedAttributes = new Hashtable();
        }

        public void AddAttDef(SchemaAttDef attdef)
        {
            this.attdefs.Add(attdef.Name, attdef);
            if ((attdef.Presence == SchemaDeclBase.Use.Required) || (attdef.Presence == SchemaDeclBase.Use.RequiredFixed))
            {
                this.hasRequiredAttribute = true;
            }
            if ((attdef.Presence == SchemaDeclBase.Use.Default) || (attdef.Presence == SchemaDeclBase.Use.Fixed))
            {
                if (this.tmpDefaultAttdefs == null)
                {
                    this.tmpDefaultAttdefs = new ArrayList();
                }
                this.tmpDefaultAttdefs.Add(attdef);
            }
        }

        public void CheckAttributes(Hashtable presence, bool standalone)
        {
            foreach (SchemaAttDef def in this.attdefs.Values)
            {
                if (presence[def.Name] == null)
                {
                    if (def.Presence == SchemaDeclBase.Use.Required)
                    {
                        throw new XmlSchemaException("Sch_MissRequiredAttribute", def.Name.ToString());
                    }
                    if ((standalone && def.IsDeclaredInExternal) && ((def.Presence == SchemaDeclBase.Use.Default) || (def.Presence == SchemaDeclBase.Use.Fixed)))
                    {
                        throw new XmlSchemaException("Sch_StandAlone", string.Empty);
                    }
                }
            }
        }

        public SchemaElementDecl Clone() => 
            ((SchemaElementDecl) base.MemberwiseClone());

        public static SchemaElementDecl CreateAnyTypeElementDecl() => 
            new SchemaElementDecl { Datatype = DatatypeImplementation.AnySimpleType.Datatype };

        public void EndAddAttDef()
        {
            if (this.tmpDefaultAttdefs != null)
            {
                this.defaultAttdefs = (SchemaAttDef[]) this.tmpDefaultAttdefs.ToArray(typeof(SchemaAttDef));
                this.tmpDefaultAttdefs = null;
            }
        }

        public SchemaAttDef GetAttDef(XmlQualifiedName qname) => 
            ((SchemaAttDef) this.attdefs[qname]);

        public XmlSchemaAnyAttribute AnyAttribute
        {
            get => 
                this.anyAttribute;
            set
            {
                this.anyAttribute = value;
            }
        }

        public Hashtable AttDefs =>
            this.attdefs;

        public XmlSchemaDerivationMethod Block
        {
            get => 
                this.block;
            set
            {
                this.block = value;
            }
        }

        public CompiledIdentityConstraint[] Constraints
        {
            get => 
                this.constraints;
            set
            {
                this.constraints = value;
            }
        }

        public System.Xml.Schema.ContentValidator ContentValidator
        {
            get => 
                this.contentValidator;
            set
            {
                this.contentValidator = value;
            }
        }

        public SchemaAttDef[] DefaultAttDefs =>
            this.defaultAttdefs;

        public bool HasDefaultAttribute =>
            (this.defaultAttdefs != null);

        public bool HasNonCDataAttribute
        {
            get => 
                this.hasNonCDataAttribute;
            set
            {
                this.hasNonCDataAttribute = value;
            }
        }

        public bool HasRequiredAttribute
        {
            get => 
                this.hasRequiredAttribute;
            set
            {
                this.hasRequiredAttribute = value;
            }
        }

        public bool IsAbstract
        {
            get => 
                this.isAbstract;
            set
            {
                this.isAbstract = value;
            }
        }

        public bool IsIdDeclared
        {
            get => 
                this.isIdDeclared;
            set
            {
                this.isIdDeclared = value;
            }
        }

        public bool IsNillable
        {
            get => 
                this.isNillable;
            set
            {
                this.isNillable = value;
            }
        }

        public bool IsNotationDeclared
        {
            get => 
                this.isNotationDeclared;
            set
            {
                this.isNotationDeclared = value;
            }
        }

        public Hashtable ProhibitedAttributes =>
            this.prohibitedAttributes;

        public XmlSchemaElement SchemaElement
        {
            get => 
                this.schemaElement;
            set
            {
                this.schemaElement = value;
            }
        }
    }
}

