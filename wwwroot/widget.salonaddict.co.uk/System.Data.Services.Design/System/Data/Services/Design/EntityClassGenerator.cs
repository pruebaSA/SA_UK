namespace System.Data.Services.Design
{
    using System;
    using System.Collections.Generic;
    using System.Data.EntityModel;
    using System.Data.Metadata.Edm;
    using System.Data.Services.Design.Common;
    using System.Data.Services.Design.Xml;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Security.Permissions;
    using System.Xml;
    using System.Xml.Linq;
    using System.Xml.Schema;
    using System.Xml.XPath;

    public sealed class EntityClassGenerator
    {
        private System.Data.Services.Design.EdmToObjectNamespaceMap _edmToObjectNamespaceMap;
        private System.Data.Services.Design.LanguageOption _languageOption;
        private bool _useDataServiceCollection;
        private bool _useDataServiceCollectionExplicitlySet;
        private DataServiceCodeVersion _version;
        private bool _versionExplicitlySet;
        private const string UseDSC_EnvironmentVariable = "dscodegen_usedsc";
        private const string UseDSCTrue = "1";
        private const string Version_EnvironmentVariable = "dscodegen_version";
        private const string Version2Dot0 = "2.0";

        public event EventHandler<PropertyGeneratedEventArgs> OnPropertyGenerated;

        public event EventHandler<TypeGeneratedEventArgs> OnTypeGenerated;

        public EntityClassGenerator()
        {
            this._edmToObjectNamespaceMap = new System.Data.Services.Design.EdmToObjectNamespaceMap();
        }

        public EntityClassGenerator(System.Data.Services.Design.LanguageOption languageOption)
        {
            this._edmToObjectNamespaceMap = new System.Data.Services.Design.EdmToObjectNamespaceMap();
            this._languageOption = EDesignUtil.CheckLanguageOptionArgument(languageOption, "languageOption");
        }

        private static void AddCustomAttributesToEntityContainer(XmlSchema csdlSchema)
        {
            XmlSchemaElement element = csdlSchema.Elements[new XmlQualifiedName("EntityContainer", csdlSchema.TargetNamespace)] as XmlSchemaElement;
            XmlSchemaComplexType schemaType = element.SchemaType as XmlSchemaComplexType;
            schemaType.AnyAttribute = new XmlSchemaAnyAttribute();
            schemaType.AnyAttribute.Namespace = "##other";
            schemaType.AnyAttribute.ProcessContents = XmlSchemaContentProcessing.Lax;
        }

        private static XmlReader CreateEdmResourceXmlReader(string resourceName)
        {
            XmlReader reader2;
            bool flag = false;
            Stream input = null;
            XmlReader reader = null;
            try
            {
                input = typeof(EdmItemCollection).Assembly.GetManifestResourceStream(resourceName);
                reader = XmlReader.Create(input);
                flag = true;
                reader2 = reader;
            }
            finally
            {
                if (!flag)
                {
                    if (reader != null)
                    {
                        reader.Close();
                    }
                    if (input != null)
                    {
                        input.Dispose();
                    }
                }
            }
            return reader2;
        }

        private static List<XmlReader> CreateReaders(XmlReader sourceReader)
        {
            NameTable nameTable = new NameTable();
            XmlNamespaceManager namespaceManager = new XmlNamespaceManager(nameTable);
            namespaceManager.AddNamespace("m", "http://schemas.microsoft.com/ado/2007/08/dataservices/metadata");
            namespaceManager.AddNamespace("edmx", "http://schemas.microsoft.com/ado/2007/06/edmx");
            namespaceManager.AddNamespace("edmv1", "http://schemas.microsoft.com/ado/2006/04/edm");
            namespaceManager.AddNamespace("edmv1_1", "http://schemas.microsoft.com/ado/2007/05/edm");
            XDocument sourceDocument = XDocument.Load(sourceReader);
            List<XElement> schemaElements = new List<XElement>();
            List<XmlReader> readers = new List<XmlReader>();
            if (TryCreateReadersV2(sourceDocument, schemaElements))
            {
                string namespaceName = null;
                for (int i = 0; i < schemaElements.Count; i++)
                {
                    if (namespaceName == null)
                    {
                        namespaceName = schemaElements[i].Name.NamespaceName;
                    }
                    else if (namespaceName != schemaElements[i].Name.NamespaceName)
                    {
                        throw new NotSupportedException(System.Data.Services.Design.Strings.InvalidMetadataMultipleNamespaces(namespaceName, schemaElements[i].Name.NamespaceName));
                    }
                    readers.Add(FitElementToSchema(schemaElements[i], namespaceName, "http://schemas.microsoft.com/ado/2007/05/edm").CreateReader());
                }
                return readers;
            }
            CreateReadersV1(sourceDocument, namespaceManager, readers);
            return readers;
        }

        private static void CreateReadersV1(XDocument sourceDocument, XmlNamespaceManager namespaceManager, List<XmlReader> readers)
        {
            foreach (XElement element in sourceDocument.XPathSelectElements("//edmv1:Schema", namespaceManager))
            {
                readers.Add(element.CreateReader());
            }
            foreach (XElement element2 in sourceDocument.XPathSelectElements("//edmv1_1:Schema", namespaceManager))
            {
                readers.Add(element2.CreateReader());
            }
        }

        private static XmlSchemaSet CreateTargetSchemaSet()
        {
            XmlSchemaSet set = new XmlSchemaSet();
            using (XmlReader reader = CreateEdmResourceXmlReader("System.Data.Resources.CodeGenerationSchema.xsd"))
            {
                set.Add(null, reader);
            }
            using (XmlReader reader2 = CreateEdmResourceXmlReader("System.Data.Resources.CSDLSchema_1_1.xsd"))
            {
                XmlSchema csdlSchema = set.Add(null, reader2);
                RemoveReferentialConstraint(csdlSchema);
                AddCustomAttributesToEntityContainer(csdlSchema);
            }
            XmlSchemaSet set2 = new XmlSchemaSet();
            foreach (XmlSchema schema2 in set.Schemas())
            {
                set2.Add(schema2);
            }
            return set2;
        }

        private static XElement FitElementToSchema(XElement element, string schemaNamespace, string targetNamespace)
        {
            XmlSchemaSet schemas = CreateTargetSchemaSet();
            XElement element2 = UpdateNamespaces(element, schemaNamespace, targetNamespace);
            XNodeSchemaApplier.Apply(schemas, element2);
            return element2;
        }

        public IList<EdmSchemaError> GenerateCode(XmlReader sourceReader, string targetFilePath)
        {
            EntityUtil.CheckArgumentNull<XmlReader>(sourceReader, "sourceReader");
            EntityUtil.CheckStringArgument(targetFilePath, "targetPath");
            using (LazyTextWriterCreator creator = new LazyTextWriterCreator(targetFilePath))
            {
                return this.GenerateCode(sourceReader, creator, null);
            }
        }

        private IList<EdmSchemaError> GenerateCode(XmlReader sourceReader, LazyTextWriterCreator target, string namespacePrefix)
        {
            List<XmlReader> xmlReaders = CreateReaders(sourceReader);
            List<EdmSchemaError> errors = new List<EdmSchemaError>();
            EdmItemCollection edmItemCollection = new EdmItemCollection(xmlReaders);
            this._version = this._versionExplicitlySet ? this._version : GetDataServiceCodeVersionFromEnvironment();
            this._useDataServiceCollection = this._useDataServiceCollectionExplicitlySet ? this._useDataServiceCollection : GetUseDataServiceCollectionFromEnvironment();
            if (this._useDataServiceCollection && (this._version == DataServiceCodeVersion.V1))
            {
                throw new InvalidOperationException(System.Data.Services.Design.Strings.VersionV1RequiresUseDataServiceCollectionFalse);
            }
            using (ClientApiGenerator generator = new ClientApiGenerator(null, edmItemCollection, this, errors, namespacePrefix))
            {
                generator.GenerateCode(target);
            }
            return errors;
        }

        public IList<EdmSchemaError> GenerateCode(XmlReader sourceReader, TextWriter targetWriter, string namespacePrefix)
        {
            EntityUtil.CheckArgumentNull<XmlReader>(sourceReader, "sourceReader");
            EntityUtil.CheckArgumentNull<TextWriter>(targetWriter, "targetWriter");
            using (LazyTextWriterCreator creator = new LazyTextWriterCreator(targetWriter))
            {
                return this.GenerateCode(sourceReader, creator, namespacePrefix);
            }
        }

        [EnvironmentPermission(SecurityAction.Assert, Read="dscodegen_version")]
        private static DataServiceCodeVersion GetDataServiceCodeVersionFromEnvironment()
        {
            if (Environment.GetEnvironmentVariable("dscodegen_version") != "2.0")
            {
                return DataServiceCodeVersion.V1;
            }
            return DataServiceCodeVersion.V2;
        }

        [EnvironmentPermission(SecurityAction.Assert, Read="dscodegen_usedsc")]
        private static bool GetUseDataServiceCollectionFromEnvironment() => 
            (Environment.GetEnvironmentVariable("dscodegen_usedsc") == "1");

        private static bool IsKnownSchemaNamespace(string namespaceName)
        {
            if (namespaceName != "http://schemas.microsoft.com/ado/2006/04/edm")
            {
                return (namespaceName == "http://schemas.microsoft.com/ado/2007/05/edm");
            }
            return true;
        }

        private static bool IsOpenTypeAttribute(XAttribute attribute) => 
            ((attribute.Name.LocalName == "OpenType") && (attribute.Name.Namespace == XNamespace.None));

        private static bool IsSchemaV2(XElement element)
        {
            XName name = element.Name;
            return ((name.LocalName == "Schema") && !IsKnownSchemaNamespace(name.NamespaceName));
        }

        internal void RaisePropertyGeneratedEvent(PropertyGeneratedEventArgs eventArgs)
        {
            if (this.OnPropertyGenerated != null)
            {
                this.OnPropertyGenerated(this, eventArgs);
            }
        }

        internal void RaiseTypeGeneratedEvent(TypeGeneratedEventArgs eventArgs)
        {
            if (this.OnTypeGenerated != null)
            {
                this.OnTypeGenerated(this, eventArgs);
            }
        }

        private static void RemoveReferentialConstraint(XmlSchema csdlSchema)
        {
            XmlSchemaComplexType type = csdlSchema.SchemaTypes[new XmlQualifiedName("TAssociation", csdlSchema.TargetNamespace)] as XmlSchemaComplexType;
            XmlSchemaSequence particle = type.Particle as XmlSchemaSequence;
            XmlSchemaObject item = null;
            foreach (XmlSchemaElement element in particle.Items)
            {
                if (element.QualifiedName == new XmlQualifiedName("ReferentialConstraint", csdlSchema.TargetNamespace))
                {
                    item = element;
                    break;
                }
            }
            particle.Items.Remove(item);
        }

        private static bool TryCreateReadersV2(XDocument sourceDocument, List<XElement> schemaElements)
        {
            bool flag = false;
            XElement root = sourceDocument.Root;
            if (IsSchemaV2(root))
            {
                schemaElements.Add(root);
                return true;
            }
            if (root.Name.LocalName != "Edmx")
            {
                return flag;
            }
            XElement element2 = (from e in root.Elements()
                where e.Name.LocalName == "DataServices"
                select e).FirstOrDefault<XElement>();
            if (element2 == null)
            {
                return flag;
            }
            XNamespace namespace2 = "http://schemas.microsoft.com/ado/2007/08/dataservices/metadata";
            XAttribute attribute = element2.Attributes((XName) (namespace2 + "DataServiceVersion")).FirstOrDefault<XAttribute>();
            if (((attribute != null) && (attribute.Value != "1.0")) && (attribute.Value != "2.0"))
            {
                throw new InvalidOperationException(System.Data.Services.Design.Strings.InvalidMetadataDataServiceVersion(attribute.Value));
            }
            schemaElements.AddRange(element2.Elements().Where<XElement>(new Func<XElement, bool>(EntityClassGenerator.IsSchemaV2)));
            return (schemaElements.Count > 0);
        }

        private static XElement UpdateNamespaces(XElement element, string oldNamespaceName, string newNamespaceName)
        {
            XNamespace namespace2 = XNamespace.Get(oldNamespaceName);
            XNamespace namespace3 = XNamespace.Get(newNamespaceName);
            Stack<XElement> stack = new Stack<XElement>();
            stack.Push(element);
            do
            {
                XElement element2 = stack.Pop();
                if (element2.Name.Namespace == namespace2)
                {
                    element2.Name = namespace3.GetName(element2.Name.LocalName);
                }
                List<XAttribute> list = null;
                foreach (XAttribute attribute in element2.Attributes())
                {
                    if (attribute.IsNamespaceDeclaration)
                    {
                        if (attribute.Value == oldNamespaceName)
                        {
                            attribute.Value = newNamespaceName;
                        }
                    }
                    else if ((attribute.Name.Namespace == namespace2) || IsOpenTypeAttribute(attribute))
                    {
                        XNodeSchemaApplier.AppendWithCreation<XAttribute>(ref list, attribute);
                    }
                }
                if (list != null)
                {
                    list.Remove();
                    using (List<XAttribute>.Enumerator enumerator2 = list.GetEnumerator())
                    {
                        Func<XAttribute, bool> predicate = null;
                        XAttribute attribute;
                        while (enumerator2.MoveNext())
                        {
                            attribute = enumerator2.Current;
                            if (IsOpenTypeAttribute(attribute))
                            {
                                if (predicate == null)
                                {
                                    predicate = a => (a.Name.NamespaceName == "http://schemas.microsoft.com/ado/2008/01/edm") && (a.Name.LocalName == attribute.Name.LocalName);
                                }
                                XAttribute attribute2 = element2.Attributes().SingleOrDefault<XAttribute>(predicate);
                                if (attribute2 == null)
                                {
                                    element2.Add(new XAttribute((XName) (XNamespace.Get("http://schemas.microsoft.com/ado/2008/01/edm") + attribute.Name.LocalName), attribute.Value));
                                }
                                else
                                {
                                    attribute2.Value = attribute.Value;
                                }
                            }
                            else
                            {
                                element2.Add(new XAttribute(namespace3.GetName(attribute.Name.LocalName), attribute.Value));
                            }
                        }
                    }
                }
                foreach (XElement element3 in element2.Elements())
                {
                    stack.Push(element3);
                }
            }
            while (stack.Count > 0);
            return element;
        }

        public System.Data.Services.Design.EdmToObjectNamespaceMap EdmToObjectNamespaceMap =>
            this._edmToObjectNamespaceMap;

        public System.Data.Services.Design.LanguageOption LanguageOption
        {
            get => 
                this._languageOption;
            set
            {
                this._languageOption = EDesignUtil.CheckLanguageOptionArgument(value, "value");
            }
        }

        public bool UseDataServiceCollection
        {
            get => 
                this._useDataServiceCollection;
            set
            {
                this._useDataServiceCollection = value;
                this._useDataServiceCollectionExplicitlySet = true;
            }
        }

        public DataServiceCodeVersion Version
        {
            get => 
                this._version;
            set
            {
                this._version = EDesignUtil.CheckDataServiceCodeVersionArgument(value, "value");
                this._versionExplicitlySet = true;
            }
        }
    }
}

