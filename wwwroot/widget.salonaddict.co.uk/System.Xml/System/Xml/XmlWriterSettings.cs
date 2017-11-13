namespace System.Xml
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Xml.Xsl.Runtime;

    public sealed class XmlWriterSettings
    {
        private bool autoXmlDecl;
        private List<XmlQualifiedName> cdataSections;
        private bool checkCharacters;
        private bool closeOutput;
        private System.Xml.ConformanceLevel conformanceLevel;
        private string docTypePublic;
        private string docTypeSystem;
        private bool doNotEscapeUriAttributes;
        private System.Text.Encoding encoding;
        private TriState indent;
        private string indentChars;
        private bool isReadOnly;
        private string mediaType;
        private bool mergeCDataSections;
        private string newLineChars;
        private System.Xml.NewLineHandling newLineHandling;
        private bool newLineOnAttributes;
        private bool omitXmlDecl;
        private XmlOutputMethod outputMethod;
        private XmlStandalone standalone;

        public XmlWriterSettings()
        {
            this.cdataSections = new List<XmlQualifiedName>();
            this.Reset();
        }

        internal XmlWriterSettings(XmlQueryDataReader reader)
        {
            this.cdataSections = new List<XmlQualifiedName>();
            this.encoding = System.Text.Encoding.GetEncoding(reader.ReadInt32());
            this.omitXmlDecl = reader.ReadBoolean();
            this.newLineHandling = (System.Xml.NewLineHandling) reader.ReadSByte(0, 2);
            this.newLineChars = reader.ReadStringQ();
            this.indent = (TriState) reader.ReadSByte(-1, 1);
            this.indentChars = reader.ReadStringQ();
            this.newLineOnAttributes = reader.ReadBoolean();
            this.closeOutput = reader.ReadBoolean();
            this.conformanceLevel = (System.Xml.ConformanceLevel) reader.ReadSByte(0, 2);
            this.checkCharacters = reader.ReadBoolean();
            this.outputMethod = (XmlOutputMethod) reader.ReadSByte(0, 3);
            int capacity = reader.ReadInt32();
            this.cdataSections = new List<XmlQualifiedName>(capacity);
            for (int i = 0; i < capacity; i++)
            {
                this.cdataSections.Add(new XmlQualifiedName(reader.ReadString(), reader.ReadString()));
            }
            this.mergeCDataSections = reader.ReadBoolean();
            this.mediaType = reader.ReadStringQ();
            this.docTypeSystem = reader.ReadStringQ();
            this.docTypePublic = reader.ReadStringQ();
            this.Standalone = (XmlStandalone) reader.ReadSByte(0, 2);
            this.autoXmlDecl = reader.ReadBoolean();
            this.isReadOnly = reader.ReadBoolean();
        }

        private void CheckReadOnly(string propertyName)
        {
            if (this.isReadOnly)
            {
                throw new XmlException("Xml_ReadOnlyProperty", "XmlWriterSettings." + propertyName);
            }
        }

        public XmlWriterSettings Clone()
        {
            XmlWriterSettings settings = base.MemberwiseClone() as XmlWriterSettings;
            settings.cdataSections = new List<XmlQualifiedName>(this.cdataSections);
            settings.isReadOnly = false;
            return settings;
        }

        internal void GetObjectData(XmlQueryDataWriter writer)
        {
            writer.Write(this.encoding.CodePage);
            writer.Write(this.omitXmlDecl);
            writer.Write((sbyte) this.newLineHandling);
            writer.WriteStringQ(this.newLineChars);
            writer.Write((sbyte) this.indent);
            writer.WriteStringQ(this.indentChars);
            writer.Write(this.newLineOnAttributes);
            writer.Write(this.closeOutput);
            writer.Write((sbyte) this.conformanceLevel);
            writer.Write(this.checkCharacters);
            writer.Write((sbyte) this.outputMethod);
            writer.Write(this.cdataSections.Count);
            foreach (XmlQualifiedName name in this.cdataSections)
            {
                writer.Write(name.Name);
                writer.Write(name.Namespace);
            }
            writer.Write(this.mergeCDataSections);
            writer.WriteStringQ(this.mediaType);
            writer.WriteStringQ(this.docTypeSystem);
            writer.WriteStringQ(this.docTypePublic);
            writer.Write((sbyte) this.standalone);
            writer.Write(this.autoXmlDecl);
            writer.Write(this.isReadOnly);
        }

        public void Reset()
        {
            this.encoding = System.Text.Encoding.UTF8;
            this.omitXmlDecl = false;
            this.newLineHandling = System.Xml.NewLineHandling.Replace;
            this.newLineChars = "\r\n";
            this.indent = TriState.Unknown;
            this.indentChars = "  ";
            this.newLineOnAttributes = false;
            this.closeOutput = false;
            this.conformanceLevel = System.Xml.ConformanceLevel.Document;
            this.checkCharacters = true;
            this.outputMethod = XmlOutputMethod.Xml;
            this.cdataSections.Clear();
            this.mergeCDataSections = false;
            this.mediaType = null;
            this.docTypeSystem = null;
            this.docTypePublic = null;
            this.standalone = XmlStandalone.Omit;
            this.isReadOnly = false;
        }

        internal bool AutoXmlDeclaration
        {
            get => 
                this.autoXmlDecl;
            set
            {
                this.CheckReadOnly("AutoXmlDeclaration");
                this.autoXmlDecl = value;
            }
        }

        internal List<XmlQualifiedName> CDataSectionElements =>
            this.cdataSections;

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

        public bool CloseOutput
        {
            get => 
                this.closeOutput;
            set
            {
                this.CheckReadOnly("CloseOutput");
                this.closeOutput = value;
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

        internal string DocTypePublic
        {
            get => 
                this.docTypePublic;
            set
            {
                this.CheckReadOnly("DocTypePublic");
                this.docTypePublic = value;
            }
        }

        internal string DocTypeSystem
        {
            get => 
                this.docTypeSystem;
            set
            {
                this.CheckReadOnly("DocTypeSystem");
                this.docTypeSystem = value;
            }
        }

        public bool DoNotEscapeUriAttributes
        {
            get => 
                this.doNotEscapeUriAttributes;
            set
            {
                this.CheckReadOnly("DoNotEscapeUriAttributes");
                this.doNotEscapeUriAttributes = value;
            }
        }

        public System.Text.Encoding Encoding
        {
            get => 
                this.encoding;
            set
            {
                this.CheckReadOnly("Encoding");
                this.encoding = value;
            }
        }

        public bool Indent
        {
            get => 
                (this.indent == TriState.True);
            set
            {
                this.CheckReadOnly("Indent");
                this.indent = value ? TriState.True : TriState.False;
            }
        }

        public string IndentChars
        {
            get => 
                this.indentChars;
            set
            {
                this.CheckReadOnly("IndentChars");
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }
                this.indentChars = value;
            }
        }

        internal TriState InternalIndent =>
            this.indent;

        internal bool IsQuerySpecific
        {
            get
            {
                if (((this.cdataSections.Count == 0) && (this.docTypePublic == null)) && (this.docTypeSystem == null))
                {
                    return (this.standalone == XmlStandalone.Yes);
                }
                return true;
            }
        }

        internal string MediaType
        {
            get => 
                this.mediaType;
            set
            {
                this.CheckReadOnly("MediaType");
                this.mediaType = value;
            }
        }

        internal bool MergeCDataSections
        {
            get => 
                this.mergeCDataSections;
            set
            {
                this.CheckReadOnly("MergeCDataSections");
                this.mergeCDataSections = value;
            }
        }

        public string NewLineChars
        {
            get => 
                this.newLineChars;
            set
            {
                this.CheckReadOnly("NewLineChars");
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }
                this.newLineChars = value;
            }
        }

        public System.Xml.NewLineHandling NewLineHandling
        {
            get => 
                this.newLineHandling;
            set
            {
                this.CheckReadOnly("NewLineHandling");
                if (value > System.Xml.NewLineHandling.None)
                {
                    throw new ArgumentOutOfRangeException("value");
                }
                this.newLineHandling = value;
            }
        }

        public bool NewLineOnAttributes
        {
            get => 
                this.newLineOnAttributes;
            set
            {
                this.CheckReadOnly("NewLineOnAttributes");
                this.newLineOnAttributes = value;
            }
        }

        public bool OmitXmlDeclaration
        {
            get => 
                this.omitXmlDecl;
            set
            {
                this.CheckReadOnly("OmitXmlDeclaration");
                this.omitXmlDecl = value;
            }
        }

        public XmlOutputMethod OutputMethod
        {
            get => 
                this.outputMethod;
            internal set
            {
                this.outputMethod = value;
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

        internal XmlStandalone Standalone
        {
            get => 
                this.standalone;
            set
            {
                this.CheckReadOnly("Standalone");
                this.standalone = value;
            }
        }
    }
}

