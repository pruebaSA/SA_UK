namespace System.Xml.Schema
{
    using System;
    using System.Collections;
    using System.ComponentModel;
    using System.IO;
    using System.Threading;
    using System.Xml;
    using System.Xml.Serialization;
    using System.Xml.XmlConfiguration;

    [XmlRoot("schema", Namespace="http://www.w3.org/2001/XMLSchema")]
    public class XmlSchema : XmlSchemaObject
    {
        private XmlSchemaForm attributeFormDefault;
        private XmlSchemaObjectTable attributeGroups = new XmlSchemaObjectTable();
        private XmlSchemaObjectTable attributes;
        private Uri baseUri;
        private XmlSchemaDerivationMethod blockDefault = XmlSchemaDerivationMethod.None;
        private XmlDocument document;
        private XmlSchemaForm elementFormDefault;
        private XmlSchemaObjectTable elements = new XmlSchemaObjectTable();
        private int errorCount;
        private XmlSchemaDerivationMethod finalDefault = XmlSchemaDerivationMethod.None;
        private static int globalIdCounter = -1;
        private XmlSchemaObjectTable groups = new XmlSchemaObjectTable();
        private string id;
        private XmlSchemaObjectTable identityConstraints = new XmlSchemaObjectTable();
        private Hashtable ids = new Hashtable();
        private ArrayList importedNamespaces;
        private ArrayList importedSchemas;
        private XmlSchemaObjectCollection includes = new XmlSchemaObjectCollection();
        public const string InstanceNamespace = "http://www.w3.org/2001/XMLSchema-instance";
        private bool isChameleon;
        private bool isCompiled;
        private bool isCompiledBySet;
        private bool isPreprocessed;
        private bool isRedefined;
        private XmlSchemaObjectCollection items = new XmlSchemaObjectCollection();
        private XmlAttribute[] moreAttributes;
        public const string Namespace = "http://www.w3.org/2001/XMLSchema";
        private XmlNameTable nameTable;
        private XmlSchemaObjectTable notations = new XmlSchemaObjectTable();
        private int schemaId = -1;
        private string targetNs;
        private XmlSchemaObjectTable types = new XmlSchemaObjectTable();
        private string version;

        internal override void AddAnnotation(XmlSchemaAnnotation annotation)
        {
            this.items.Add(annotation);
        }

        internal void AddCompiledInfo(SchemaInfo schemaInfo)
        {
            XmlQualifiedName qualifiedName;
            foreach (XmlSchemaElement element in this.elements.Values)
            {
                qualifiedName = element.QualifiedName;
                schemaInfo.TargetNamespaces[qualifiedName.Namespace] = true;
                if (schemaInfo.ElementDecls[qualifiedName] == null)
                {
                    schemaInfo.ElementDecls.Add(qualifiedName, element.ElementDecl);
                }
            }
            foreach (XmlSchemaAttribute attribute in this.attributes.Values)
            {
                qualifiedName = attribute.QualifiedName;
                schemaInfo.TargetNamespaces[qualifiedName.Namespace] = true;
                if (schemaInfo.ElementDecls[qualifiedName] == null)
                {
                    schemaInfo.AttributeDecls.Add(qualifiedName, attribute.AttDef);
                }
            }
            foreach (XmlSchemaType type in this.types.Values)
            {
                qualifiedName = type.QualifiedName;
                schemaInfo.TargetNamespaces[qualifiedName.Namespace] = true;
                if ((!(type is XmlSchemaComplexType) || (type != XmlSchemaComplexType.AnyType)) && (schemaInfo.ElementDeclsByType[qualifiedName] == null))
                {
                    schemaInfo.ElementDeclsByType.Add(qualifiedName, type.ElementDecl);
                }
            }
            foreach (XmlSchemaNotation notation in this.notations.Values)
            {
                qualifiedName = notation.QualifiedName;
                schemaInfo.TargetNamespaces[qualifiedName.Namespace] = true;
                SchemaNotation notation2 = new SchemaNotation(qualifiedName) {
                    SystemLiteral = notation.System,
                    Pubid = notation.Public
                };
                if (schemaInfo.Notations[qualifiedName.Name] == null)
                {
                    schemaInfo.Notations.Add(qualifiedName.Name, notation2);
                }
            }
        }

        internal XmlSchema Clone()
        {
            XmlSchema schema = new XmlSchema {
                attributeFormDefault = this.attributeFormDefault,
                elementFormDefault = this.elementFormDefault,
                blockDefault = this.blockDefault,
                finalDefault = this.finalDefault,
                targetNs = this.targetNs,
                version = this.version,
                includes = this.includes,
                Namespaces = base.Namespaces,
                items = this.items,
                BaseUri = this.BaseUri
            };
            SchemaCollectionCompiler.Cleanup(schema);
            return schema;
        }

        [Obsolete("Use System.Xml.Schema.XmlSchemaSet for schema compilation and validation. http://go.microsoft.com/fwlink/?linkid=14202")]
        public void Compile(ValidationEventHandler validationEventHandler)
        {
            SchemaInfo schemaInfo = new SchemaInfo {
                SchemaType = SchemaType.XSD
            };
            this.CompileSchema(null, XmlReaderSection.CreateDefaultResolver(), schemaInfo, null, validationEventHandler, this.NameTable, false);
        }

        [Obsolete("Use System.Xml.Schema.XmlSchemaSet for schema compilation and validation. http://go.microsoft.com/fwlink/?linkid=14202")]
        public void Compile(ValidationEventHandler validationEventHandler, XmlResolver resolver)
        {
            SchemaInfo schemaInfo = new SchemaInfo {
                SchemaType = SchemaType.XSD
            };
            this.CompileSchema(null, resolver, schemaInfo, null, validationEventHandler, this.NameTable, false);
        }

        internal bool CompileSchema(XmlSchemaCollection xsc, XmlResolver resolver, SchemaInfo schemaInfo, string ns, ValidationEventHandler validationEventHandler, XmlNameTable nameTable, bool CompileContentModel)
        {
            lock (this)
            {
                SchemaCollectionPreprocessor preprocessor = new SchemaCollectionPreprocessor(nameTable, null, validationEventHandler) {
                    XmlResolver = resolver
                };
                if (!preprocessor.Execute(this, ns, true, xsc))
                {
                    return false;
                }
                this.isCompiled = new SchemaCollectionCompiler(nameTable, validationEventHandler).Execute(this, schemaInfo, CompileContentModel);
                this.SetIsCompiled(this.isCompiled);
                return this.isCompiled;
            }
        }

        internal void CompileSchemaInSet(XmlNameTable nameTable, ValidationEventHandler eventHandler, XmlSchemaCompilationSettings compilationSettings)
        {
            System.Xml.Schema.Compiler compiler = new System.Xml.Schema.Compiler(nameTable, eventHandler, null, compilationSettings);
            compiler.Prepare(this, true);
            this.isCompiledBySet = compiler.Compile();
        }

        internal XmlSchema DeepClone()
        {
            XmlSchema schema = new XmlSchema {
                attributeFormDefault = this.attributeFormDefault,
                elementFormDefault = this.elementFormDefault,
                blockDefault = this.blockDefault,
                finalDefault = this.finalDefault,
                targetNs = this.targetNs,
                version = this.version,
                isPreprocessed = this.isPreprocessed
            };
            using (XmlSchemaObjectEnumerator enumerator = this.items.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    XmlSchemaObject item = enumerator.Current.Clone();
                    schema.Items.Add(item);
                }
            }
            foreach (XmlSchemaExternal external in this.includes)
            {
                XmlSchemaExternal external2 = (XmlSchemaExternal) external.Clone();
                schema.Includes.Add(external2);
            }
            schema.Namespaces = base.Namespaces;
            schema.BaseUri = this.BaseUri;
            return schema;
        }

        internal void GetExternalSchemasList(IList extList, XmlSchema schema)
        {
            if (!extList.Contains(schema))
            {
                extList.Add(schema);
                foreach (XmlSchemaExternal external in schema.Includes)
                {
                    if (external.Schema != null)
                    {
                        this.GetExternalSchemasList(extList, external.Schema);
                    }
                }
            }
        }

        public static XmlSchema Read(Stream stream, ValidationEventHandler validationEventHandler) => 
            Read(new XmlTextReader(stream), validationEventHandler);

        public static XmlSchema Read(TextReader reader, ValidationEventHandler validationEventHandler) => 
            Read(new XmlTextReader(reader), validationEventHandler);

        public static XmlSchema Read(XmlReader reader, ValidationEventHandler validationEventHandler)
        {
            XmlNameTable nameTable = reader.NameTable;
            System.Xml.Schema.Parser parser = new System.Xml.Schema.Parser(SchemaType.XSD, nameTable, new SchemaNames(nameTable), validationEventHandler);
            try
            {
                parser.Parse(reader, null);
            }
            catch (XmlSchemaException exception)
            {
                if (validationEventHandler == null)
                {
                    throw exception;
                }
                validationEventHandler(null, new ValidationEventArgs(exception));
                return null;
            }
            return parser.XmlSchema;
        }

        internal void SetIsCompiled(bool isCompiled)
        {
            this.isCompiled = isCompiled;
        }

        internal override void SetUnhandledAttributes(XmlAttribute[] moreAttributes)
        {
            this.moreAttributes = moreAttributes;
        }

        public void Write(Stream stream)
        {
            this.Write(stream, null);
        }

        public void Write(TextWriter writer)
        {
            this.Write(writer, null);
        }

        public void Write(XmlWriter writer)
        {
            this.Write(writer, null);
        }

        public void Write(Stream stream, XmlNamespaceManager namespaceManager)
        {
            XmlTextWriter writer = new XmlTextWriter(stream, null) {
                Formatting = Formatting.Indented
            };
            this.Write(writer, namespaceManager);
        }

        public void Write(TextWriter writer, XmlNamespaceManager namespaceManager)
        {
            XmlTextWriter writer2 = new XmlTextWriter(writer) {
                Formatting = Formatting.Indented
            };
            this.Write(writer2, namespaceManager);
        }

        public void Write(XmlWriter writer, XmlNamespaceManager namespaceManager)
        {
            XmlSerializerNamespaces namespaces;
            XmlSerializer serializer = new XmlSerializer(typeof(XmlSchema));
            if (namespaceManager != null)
            {
                namespaces = new XmlSerializerNamespaces();
                bool flag = false;
                if (base.Namespaces != null)
                {
                    flag = (base.Namespaces.Namespaces["xs"] != null) || base.Namespaces.Namespaces.ContainsValue("http://www.w3.org/2001/XMLSchema");
                }
                if ((!flag && (namespaceManager.LookupPrefix("http://www.w3.org/2001/XMLSchema") == null)) && (namespaceManager.LookupNamespace("xs") == null))
                {
                    namespaces.Add("xs", "http://www.w3.org/2001/XMLSchema");
                }
                foreach (string str in namespaceManager)
                {
                    if ((str != "xml") && (str != "xmlns"))
                    {
                        namespaces.Add(str, namespaceManager.LookupNamespace(str));
                    }
                }
            }
            else if ((base.Namespaces != null) && (base.Namespaces.Count > 0))
            {
                Hashtable hashtable = base.Namespaces.Namespaces;
                if ((hashtable["xs"] == null) && !hashtable.ContainsValue("http://www.w3.org/2001/XMLSchema"))
                {
                    hashtable.Add("xs", "http://www.w3.org/2001/XMLSchema");
                }
                namespaces = base.Namespaces;
            }
            else
            {
                namespaces = new XmlSerializerNamespaces();
                namespaces.Add("xs", "http://www.w3.org/2001/XMLSchema");
                if ((this.targetNs != null) && (this.targetNs.Length != 0))
                {
                    namespaces.Add("tns", this.targetNs);
                }
            }
            serializer.Serialize(writer, this, namespaces);
        }

        [DefaultValue(0), XmlAttribute("attributeFormDefault")]
        public XmlSchemaForm AttributeFormDefault
        {
            get => 
                this.attributeFormDefault;
            set
            {
                this.attributeFormDefault = value;
            }
        }

        [XmlIgnore]
        public XmlSchemaObjectTable AttributeGroups
        {
            get
            {
                if (this.attributeGroups == null)
                {
                    this.attributeGroups = new XmlSchemaObjectTable();
                }
                return this.attributeGroups;
            }
        }

        [XmlIgnore]
        public XmlSchemaObjectTable Attributes
        {
            get
            {
                if (this.attributes == null)
                {
                    this.attributes = new XmlSchemaObjectTable();
                }
                return this.attributes;
            }
        }

        [XmlIgnore]
        internal Uri BaseUri
        {
            get => 
                this.baseUri;
            set
            {
                this.baseUri = value;
            }
        }

        [DefaultValue(0x100), XmlAttribute("blockDefault")]
        public XmlSchemaDerivationMethod BlockDefault
        {
            get => 
                this.blockDefault;
            set
            {
                this.blockDefault = value;
            }
        }

        [XmlIgnore]
        internal XmlDocument Document
        {
            get
            {
                if (this.document == null)
                {
                    this.document = new XmlDocument();
                }
                return this.document;
            }
        }

        [DefaultValue(0), XmlAttribute("elementFormDefault")]
        public XmlSchemaForm ElementFormDefault
        {
            get => 
                this.elementFormDefault;
            set
            {
                this.elementFormDefault = value;
            }
        }

        [XmlIgnore]
        public XmlSchemaObjectTable Elements
        {
            get
            {
                if (this.elements == null)
                {
                    this.elements = new XmlSchemaObjectTable();
                }
                return this.elements;
            }
        }

        [XmlIgnore]
        internal int ErrorCount
        {
            get => 
                this.errorCount;
            set
            {
                this.errorCount = value;
            }
        }

        [DefaultValue(0x100), XmlAttribute("finalDefault")]
        public XmlSchemaDerivationMethod FinalDefault
        {
            get => 
                this.finalDefault;
            set
            {
                this.finalDefault = value;
            }
        }

        [XmlIgnore]
        public XmlSchemaObjectTable Groups =>
            this.groups;

        [XmlAttribute("id", DataType="ID")]
        public string Id
        {
            get => 
                this.id;
            set
            {
                this.id = value;
            }
        }

        [XmlIgnore]
        internal override string IdAttribute
        {
            get => 
                this.Id;
            set
            {
                this.Id = value;
            }
        }

        [XmlIgnore]
        internal XmlSchemaObjectTable IdentityConstraints =>
            this.identityConstraints;

        [XmlIgnore]
        internal Hashtable Ids =>
            this.ids;

        internal ArrayList ImportedNamespaces
        {
            get
            {
                if (this.importedNamespaces == null)
                {
                    this.importedNamespaces = new ArrayList();
                }
                return this.importedNamespaces;
            }
        }

        internal ArrayList ImportedSchemas
        {
            get
            {
                if (this.importedSchemas == null)
                {
                    this.importedSchemas = new ArrayList();
                }
                return this.importedSchemas;
            }
        }

        [XmlElement("include", typeof(XmlSchemaInclude)), XmlElement("redefine", typeof(XmlSchemaRedefine)), XmlElement("import", typeof(XmlSchemaImport))]
        public XmlSchemaObjectCollection Includes =>
            this.includes;

        [XmlIgnore]
        internal bool IsChameleon
        {
            get => 
                this.isChameleon;
            set
            {
                this.isChameleon = value;
            }
        }

        [XmlIgnore]
        public bool IsCompiled
        {
            get
            {
                if (!this.isCompiled)
                {
                    return this.isCompiledBySet;
                }
                return true;
            }
        }

        [XmlIgnore]
        internal bool IsCompiledBySet
        {
            get => 
                this.isCompiledBySet;
            set
            {
                this.isCompiledBySet = value;
            }
        }

        [XmlIgnore]
        internal bool IsPreprocessed
        {
            get => 
                this.isPreprocessed;
            set
            {
                this.isPreprocessed = value;
            }
        }

        [XmlIgnore]
        internal bool IsRedefined
        {
            get => 
                this.isRedefined;
            set
            {
                this.isRedefined = value;
            }
        }

        [XmlElement("complexType", typeof(XmlSchemaComplexType)), XmlElement("annotation", typeof(XmlSchemaAnnotation)), XmlElement("attributeGroup", typeof(XmlSchemaAttributeGroup)), XmlElement("group", typeof(XmlSchemaGroup)), XmlElement("notation", typeof(XmlSchemaNotation)), XmlElement("element", typeof(XmlSchemaElement)), XmlElement("simpleType", typeof(XmlSchemaSimpleType)), XmlElement("attribute", typeof(XmlSchemaAttribute))]
        public XmlSchemaObjectCollection Items =>
            this.items;

        internal XmlNameTable NameTable
        {
            get
            {
                if (this.nameTable == null)
                {
                    this.nameTable = new System.Xml.NameTable();
                }
                return this.nameTable;
            }
        }

        [XmlIgnore]
        public XmlSchemaObjectTable Notations =>
            this.notations;

        [XmlIgnore]
        internal int SchemaId
        {
            get
            {
                if (this.schemaId == -1)
                {
                    this.schemaId = Interlocked.Increment(ref globalIdCounter);
                }
                return this.schemaId;
            }
        }

        [XmlIgnore]
        public XmlSchemaObjectTable SchemaTypes
        {
            get
            {
                if (this.types == null)
                {
                    this.types = new XmlSchemaObjectTable();
                }
                return this.types;
            }
        }

        [XmlAttribute("targetNamespace", DataType="anyURI")]
        public string TargetNamespace
        {
            get => 
                this.targetNs;
            set
            {
                this.targetNs = value;
            }
        }

        [XmlAnyAttribute]
        public XmlAttribute[] UnhandledAttributes
        {
            get => 
                this.moreAttributes;
            set
            {
                this.moreAttributes = value;
            }
        }

        [XmlAttribute("version", DataType="token")]
        public string Version
        {
            get => 
                this.version;
            set
            {
                this.version = value;
            }
        }
    }
}

