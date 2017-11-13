namespace System.Xml.Schema
{
    using System;
    using System.Xml;

    internal sealed class SchemaAttDef : SchemaDeclBase
    {
        private bool defaultValueChecked;
        private string defExpanded;
        public static readonly SchemaAttDef Empty = new SchemaAttDef();
        private bool hasEntityRef;
        private int lineNum;
        private int linePos;
        private Reserve reserved;
        private XmlSchemaAttribute schemaAttribute;
        private int valueLineNum;
        private int valueLinePos;

        private SchemaAttDef()
        {
        }

        public SchemaAttDef(XmlQualifiedName name, string prefix) : base(name, prefix)
        {
            this.reserved = Reserve.None;
        }

        internal void CheckDefaultValue(SchemaInfo schemaInfo, IDtdParserAdapter readerAdapter)
        {
            DtdValidator.CheckDefaultValue(this, schemaInfo, readerAdapter);
            this.defaultValueChecked = true;
        }

        public void CheckXmlSpace(ValidationEventHandler eventhandler)
        {
            if (((base.datatype.TokenizedType == XmlTokenizedType.ENUMERATION) && (base.values != null)) && (base.values.Count <= 2))
            {
                string str = base.values[0].ToString();
                if (base.values.Count == 2)
                {
                    string str2 = base.values[1].ToString();
                    if (((str == "default") || (str2 == "default")) && ((str == "preserve") || (str2 == "preserve")))
                    {
                        return;
                    }
                }
                else
                {
                    switch (str)
                    {
                        case "default":
                        case "preserve":
                            return;
                    }
                }
            }
            eventhandler(this, new ValidationEventArgs(new XmlSchemaException("Sch_XmlSpace", string.Empty)));
        }

        public SchemaAttDef Clone() => 
            ((SchemaAttDef) base.MemberwiseClone());

        internal bool DefaultValueChecked =>
            this.defaultValueChecked;

        public string DefaultValueExpanded
        {
            get
            {
                if (this.defExpanded == null)
                {
                    return string.Empty;
                }
                return this.defExpanded;
            }
            set
            {
                this.defExpanded = value;
            }
        }

        public bool HasEntityRef
        {
            get => 
                this.hasEntityRef;
            set
            {
                this.hasEntityRef = value;
            }
        }

        internal int LineNum
        {
            get => 
                this.lineNum;
            set
            {
                this.lineNum = value;
            }
        }

        internal int LinePos
        {
            get => 
                this.linePos;
            set
            {
                this.linePos = value;
            }
        }

        public Reserve Reserved
        {
            get => 
                this.reserved;
            set
            {
                this.reserved = value;
            }
        }

        public XmlSchemaAttribute SchemaAttribute
        {
            get => 
                this.schemaAttribute;
            set
            {
                this.schemaAttribute = value;
            }
        }

        internal int ValueLineNum
        {
            get => 
                this.valueLineNum;
            set
            {
                this.valueLineNum = value;
            }
        }

        internal int ValueLinePos
        {
            get => 
                this.valueLinePos;
            set
            {
                this.valueLinePos = value;
            }
        }

        public enum Reserve
        {
            None,
            XmlSpace,
            XmlLang
        }
    }
}

