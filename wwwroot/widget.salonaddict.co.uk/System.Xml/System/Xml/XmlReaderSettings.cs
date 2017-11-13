namespace System.Xml
{
    using System;
    using System.Xml.Schema;
    using System.Xml.XmlConfiguration;

    public sealed class XmlReaderSettings
    {
        private bool checkCharacters;
        private bool closeInput;
        private System.Xml.ConformanceLevel conformanceLevel;
        private bool ignoreComments;
        private bool ignorePIs;
        private bool ignoreWhitespace;
        private bool isReadOnly;
        private bool isXmlResolverSet;
        private int lineNumberOffset;
        private int linePositionOffset;
        private long maxCharactersFromEntities;
        private long maxCharactersInDocument;
        private XmlNameTable nameTable;
        private bool prohibitDtd;
        private XmlSchemaSet schemas;
        private System.Xml.Schema.ValidationEventHandler valEventHandler;
        private XmlSchemaValidationFlags validationFlags;
        private System.Xml.ValidationType validationType;
        private System.Xml.XmlResolver xmlResolver;

        public event System.Xml.Schema.ValidationEventHandler ValidationEventHandler
        {
            add
            {
                this.CheckReadOnly("ValidationEventHandler");
                this.valEventHandler = (System.Xml.Schema.ValidationEventHandler) Delegate.Combine(this.valEventHandler, value);
            }
            remove
            {
                this.CheckReadOnly("ValidationEventHandler");
                this.valEventHandler = (System.Xml.Schema.ValidationEventHandler) Delegate.Remove(this.valEventHandler, value);
            }
        }

        public XmlReaderSettings()
        {
            this.Reset();
        }

        private void CheckReadOnly(string propertyName)
        {
            if (this.isReadOnly)
            {
                throw new XmlException("Xml_ReadOnlyProperty", "XmlReaderSettings." + propertyName);
            }
        }

        public XmlReaderSettings Clone()
        {
            XmlReaderSettings settings = base.MemberwiseClone() as XmlReaderSettings;
            settings.isReadOnly = false;
            return settings;
        }

        private static System.Xml.XmlResolver CreateDefaultResolver() => 
            new XmlUrlResolver();

        internal System.Xml.Schema.ValidationEventHandler GetEventHandler() => 
            this.valEventHandler;

        internal System.Xml.XmlResolver GetXmlResolver() => 
            this.xmlResolver;

        internal System.Xml.XmlResolver GetXmlResolver_CheckConfig()
        {
            if (XmlReaderSection.ProhibitDefaultUrlResolver && !this.IsXmlResolverSet)
            {
                return null;
            }
            return this.xmlResolver;
        }

        public void Reset()
        {
            this.CheckReadOnly("Reset");
            this.nameTable = null;
            this.xmlResolver = CreateDefaultResolver();
            this.lineNumberOffset = 0;
            this.linePositionOffset = 0;
            this.checkCharacters = true;
            this.conformanceLevel = System.Xml.ConformanceLevel.Document;
            this.schemas = null;
            this.validationType = System.Xml.ValidationType.None;
            this.validationFlags = XmlSchemaValidationFlags.ProcessIdentityConstraints;
            this.validationFlags |= XmlSchemaValidationFlags.AllowXmlAttributes;
            this.ignoreWhitespace = false;
            this.ignorePIs = false;
            this.ignoreComments = false;
            this.prohibitDtd = true;
            this.closeInput = false;
            this.maxCharactersFromEntities = 0L;
            this.maxCharactersInDocument = 0L;
            this.isReadOnly = false;
            this.IsXmlResolverSet = false;
        }

        internal bool CanResolveExternals =>
            (!this.prohibitDtd && (this.xmlResolver != null));

        public bool CheckCharacters
        {
            get => 
                this.checkCharacters;
            set
            {
                this.CheckReadOnly("CheckCharacters");
                this.checkCharacters = value;
            }
        }

        public bool CloseInput
        {
            get => 
                this.closeInput;
            set
            {
                this.CheckReadOnly("CloseInput");
                this.closeInput = value;
            }
        }

        public System.Xml.ConformanceLevel ConformanceLevel
        {
            get => 
                this.conformanceLevel;
            set
            {
                this.CheckReadOnly("ConformanceLevel");
                if (value > System.Xml.ConformanceLevel.Document)
                {
                    throw new ArgumentOutOfRangeException("value");
                }
                this.conformanceLevel = value;
            }
        }

        public bool IgnoreComments
        {
            get => 
                this.ignoreComments;
            set
            {
                this.CheckReadOnly("IgnoreComments");
                this.ignoreComments = value;
            }
        }

        public bool IgnoreProcessingInstructions
        {
            get => 
                this.ignorePIs;
            set
            {
                this.CheckReadOnly("IgnoreProcessingInstructions");
                this.ignorePIs = value;
            }
        }

        public bool IgnoreWhitespace
        {
            get => 
                this.ignoreWhitespace;
            set
            {
                this.CheckReadOnly("IgnoreWhitespace");
                this.ignoreWhitespace = value;
            }
        }

        internal bool IsXmlResolverSet
        {
            get => 
                this.isXmlResolverSet;
            private set
            {
                this.isXmlResolverSet = value;
            }
        }

        public int LineNumberOffset
        {
            get => 
                this.lineNumberOffset;
            set
            {
                this.CheckReadOnly("LineNumberOffset");
                if (this.lineNumberOffset < 0)
                {
                    throw new ArgumentOutOfRangeException("value");
                }
                this.lineNumberOffset = value;
            }
        }

        public int LinePositionOffset
        {
            get => 
                this.linePositionOffset;
            set
            {
                this.CheckReadOnly("LinePositionOffset");
                if (this.linePositionOffset < 0)
                {
                    throw new ArgumentOutOfRangeException("value");
                }
                this.linePositionOffset = value;
            }
        }

        public long MaxCharactersFromEntities
        {
            get => 
                this.maxCharactersFromEntities;
            set
            {
                this.CheckReadOnly("MaxCharactersFromEntities");
                if (value < 0L)
                {
                    throw new ArgumentOutOfRangeException("value");
                }
                this.maxCharactersFromEntities = value;
            }
        }

        public long MaxCharactersInDocument
        {
            get => 
                this.maxCharactersInDocument;
            set
            {
                this.CheckReadOnly("MaxCharactersInDocument");
                if (value < 0L)
                {
                    throw new ArgumentOutOfRangeException("value");
                }
                this.maxCharactersInDocument = value;
            }
        }

        public XmlNameTable NameTable
        {
            get => 
                this.nameTable;
            set
            {
                this.CheckReadOnly("NameTable");
                this.nameTable = value;
            }
        }

        public bool ProhibitDtd
        {
            get => 
                this.prohibitDtd;
            set
            {
                this.CheckReadOnly("ProhibitDtd");
                this.prohibitDtd = value;
            }
        }

        internal bool ReadOnly
        {
            get => 
                this.isReadOnly;
            set
            {
                this.isReadOnly = value;
            }
        }

        public XmlSchemaSet Schemas
        {
            get
            {
                if (this.schemas == null)
                {
                    this.schemas = new XmlSchemaSet();
                }
                return this.schemas;
            }
            set
            {
                this.CheckReadOnly("Schemas");
                this.schemas = value;
            }
        }

        public XmlSchemaValidationFlags ValidationFlags
        {
            get => 
                this.validationFlags;
            set
            {
                this.CheckReadOnly("ValidationFlags");
                if (value > (XmlSchemaValidationFlags.AllowXmlAttributes | XmlSchemaValidationFlags.ProcessIdentityConstraints | XmlSchemaValidationFlags.ReportValidationWarnings | XmlSchemaValidationFlags.ProcessSchemaLocation | XmlSchemaValidationFlags.ProcessInlineSchema))
                {
                    throw new ArgumentOutOfRangeException("value");
                }
                this.validationFlags = value;
            }
        }

        public System.Xml.ValidationType ValidationType
        {
            get => 
                this.validationType;
            set
            {
                this.CheckReadOnly("ValidationType");
                if (value > System.Xml.ValidationType.Schema)
                {
                    throw new ArgumentOutOfRangeException("value");
                }
                this.validationType = value;
            }
        }

        public System.Xml.XmlResolver XmlResolver
        {
            set
            {
                this.CheckReadOnly("XmlResolver");
                this.xmlResolver = value;
                this.IsXmlResolverSet = true;
            }
        }
    }
}

