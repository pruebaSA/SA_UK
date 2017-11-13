namespace System.Xml.Schema
{
    using System;
    using System.Collections;
    using System.Xml;

    internal abstract class SchemaDeclBase
    {
        protected XmlSchemaDatatype datatype;
        protected string defaultValueRaw;
        protected object defaultValueTyped;
        protected bool isDeclaredInExternal;
        protected long maxLength;
        protected long minLength;
        protected XmlQualifiedName name;
        protected string prefix;
        protected Use presence;
        protected XmlSchemaType schemaType;
        protected ArrayList values;

        protected SchemaDeclBase()
        {
            this.name = XmlQualifiedName.Empty;
        }

        protected SchemaDeclBase(XmlQualifiedName name, string prefix)
        {
            this.name = XmlQualifiedName.Empty;
            this.name = name;
            this.prefix = prefix;
            this.maxLength = -1L;
            this.minLength = -1L;
        }

        public void AddValue(string value)
        {
            if (this.values == null)
            {
                this.values = new ArrayList();
            }
            this.values.Add(value);
        }

        public bool CheckEnumeration(object pVal) => 
            (((this.datatype.TokenizedType != XmlTokenizedType.NOTATION) && (this.datatype.TokenizedType != XmlTokenizedType.ENUMERATION)) || this.values.Contains(pVal.ToString()));

        public bool CheckValue(object pVal) => 
            (((this.presence != Use.Fixed) && (this.presence != Use.RequiredFixed)) || ((this.defaultValueTyped != null) && this.datatype.IsEqual(pVal, this.defaultValueTyped)));

        public XmlSchemaDatatype Datatype
        {
            get => 
                this.datatype;
            set
            {
                this.datatype = value;
            }
        }

        public string DefaultValueRaw
        {
            get
            {
                if (this.defaultValueRaw == null)
                {
                    return string.Empty;
                }
                return this.defaultValueRaw;
            }
            set
            {
                this.defaultValueRaw = value;
            }
        }

        public object DefaultValueTyped
        {
            get => 
                this.defaultValueTyped;
            set
            {
                this.defaultValueTyped = value;
            }
        }

        public bool IsDeclaredInExternal
        {
            get => 
                this.isDeclaredInExternal;
            set
            {
                this.isDeclaredInExternal = value;
            }
        }

        public long MaxLength
        {
            get => 
                this.maxLength;
            set
            {
                this.maxLength = value;
            }
        }

        public long MinLength
        {
            get => 
                this.minLength;
            set
            {
                this.minLength = value;
            }
        }

        public XmlQualifiedName Name
        {
            get => 
                this.name;
            set
            {
                this.name = value;
            }
        }

        public string Prefix
        {
            get
            {
                if (this.prefix != null)
                {
                    return this.prefix;
                }
                return string.Empty;
            }
            set
            {
                this.prefix = value;
            }
        }

        public Use Presence
        {
            get => 
                this.presence;
            set
            {
                this.presence = value;
            }
        }

        public XmlSchemaType SchemaType
        {
            get => 
                this.schemaType;
            set
            {
                this.schemaType = value;
            }
        }

        public ArrayList Values
        {
            get => 
                this.values;
            set
            {
                this.values = value;
            }
        }

        public enum Use
        {
            Default,
            Required,
            Implied,
            Fixed,
            RequiredFixed
        }
    }
}

