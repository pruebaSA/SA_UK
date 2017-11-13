namespace System.Data.EntityModel.SchemaObjectModel
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Data.Common.Utils;
    using System.Data.Entity;
    using System.Data.Metadata.Edm;
    using System.Data.Objects.DataClasses;
    using System.Diagnostics;
    using System.IO;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Security;
    using System.Xml;
    using System.Xml.Schema;

    [DebuggerDisplay("Namespace={Namespace}, PublicKeyToken={PublicKeyToken}, Version={Version}")]
    internal class Schema : SchemaElement
    {
        private string _alias;
        private System.Data.EntityModel.SchemaObjectModel.AliasResolver _aliasResolver;
        private int _depth;
        private double _edmVersion;
        private static IList<string> _emptyPathList = new List<string>(0).AsReadOnly();
        private List<EdmSchemaError> _errors;
        private List<System.Data.EntityModel.SchemaObjectModel.Function> _functions;
        private string _location;
        protected string _namespaceName;
        private HashSet<string> _parseableXmlNamespaces;
        private System.Data.EntityModel.SchemaObjectModel.SchemaManager _schemaManager;
        private List<System.Data.EntityModel.SchemaObjectModel.SchemaType> _schemaTypes;
        private string _schemaXmlNamespace;
        private HashSet<string> _validatableXmlNamespaces;
        private static readonly string[] ClientNamespaceOfSchemasMissingStoreSuffix = new string[] { "System.Storage.Sync.Utility", "System.Storage.Sync.Services" };
        private const int RootDepth = 2;

        public Schema(System.Data.EntityModel.SchemaObjectModel.SchemaManager schemaManager) : base(null)
        {
            this._errors = new List<EdmSchemaError>();
            this._edmVersion = 1.0;
            this._schemaManager = schemaManager;
            this._errors = new List<EdmSchemaError>();
        }

        private static void AddAllSchemaResourceNamespaceNames(HashSet<string> hashSet, SomSchemaSetHelper.XmlSchemaResource schemaResource)
        {
            hashSet.Add(schemaResource.NamespaceUri);
            foreach (SomSchemaSetHelper.XmlSchemaResource resource in schemaResource.ImportedSchemas)
            {
                AddAllSchemaResourceNamespaceNames(hashSet, resource);
            }
        }

        internal void AddError(EdmSchemaError error)
        {
            this._errors.Add(error);
        }

        protected void AddFunctionType(System.Data.EntityModel.SchemaObjectModel.Function function)
        {
            if (this.SchemaManager.SchemaTypes.TryAdd(function) != AddErrorKind.Succeeded)
            {
                function.AddError(ErrorCode.AlreadyDefined, EdmSchemaErrorSeverity.Error, Strings.AmbiguousFunctionOverload(function.FQName));
            }
            else
            {
                this.SchemaTypes.Add(function);
            }
        }

        internal static XmlReaderSettings CreateEdmStandardXmlReaderSettings()
        {
            XmlReaderSettings settings = new XmlReaderSettings {
                CheckCharacters = true,
                CloseInput = false,
                IgnoreWhitespace = true,
                ConformanceLevel = ConformanceLevel.Auto,
                IgnoreComments = true,
                IgnoreProcessingInstructions = true,
                ProhibitDtd = true
            };
            settings.ValidationFlags &= ~XmlSchemaValidationFlags.ProcessIdentityConstraints;
            settings.ValidationFlags &= ~XmlSchemaValidationFlags.ProcessSchemaLocation;
            settings.ValidationFlags &= ~XmlSchemaValidationFlags.ProcessInlineSchema;
            return settings;
        }

        private XmlReaderSettings CreateXmlReaderSettings()
        {
            XmlReaderSettings settings = CreateEdmStandardXmlReaderSettings();
            settings.ValidationFlags |= XmlSchemaValidationFlags.ReportValidationWarnings;
            settings.ValidationEventHandler += new ValidationEventHandler(this.OnSchemaValidationEvent);
            settings.ValidationType = ValidationType.Schema;
            XmlSchemaSet schemaSet = SomSchemaSetHelper.GetSchemaSet(this.DataModel);
            settings.Schemas = schemaSet;
            return settings;
        }

        private void HandleAliasAttribute(XmlReader reader)
        {
            this.Alias = base.HandleUndottedNameAttribute(reader, this.Alias);
        }

        private void HandleAssociationElement(XmlReader reader)
        {
            Relationship schemaType = new Relationship(this, RelationshipKind.Association);
            schemaType.Parse(reader);
            this.TryAddType(schemaType, true);
        }

        protected override bool HandleAttribute(XmlReader reader)
        {
            if (this._depth != 1)
            {
                if (base.HandleAttribute(reader))
                {
                    return true;
                }
                if (SchemaElement.CanHandleAttribute(reader, "Alias"))
                {
                    this.HandleAliasAttribute(reader);
                    return true;
                }
                if (SchemaElement.CanHandleAttribute(reader, "Namespace"))
                {
                    this.HandleNamespaceAttribute(reader);
                    return true;
                }
                if (SchemaElement.CanHandleAttribute(reader, "Provider"))
                {
                    this.HandleProviderAttribute(reader);
                    return true;
                }
                if (SchemaElement.CanHandleAttribute(reader, "ProviderManifestToken"))
                {
                    this.HandleProviderManifestTokenAttribute(reader);
                    return true;
                }
            }
            return false;
        }

        protected override void HandleAttributesComplete()
        {
            if (this._depth >= 2)
            {
                if (this._depth == 2)
                {
                    this._schemaManager.EnsurePrimitiveSchemaIsLoaded();
                }
                base.HandleAttributesComplete();
            }
        }

        protected override bool HandleElement(XmlReader reader)
        {
            if (base.HandleElement(reader))
            {
                return true;
            }
            if (base.CanHandleElement(reader, "EntityType"))
            {
                this.HandleEntityTypeElement(reader);
                return true;
            }
            if (base.CanHandleElement(reader, "ComplexType"))
            {
                this.HandleInlineTypeElement(reader);
                return true;
            }
            if (base.CanHandleElement(reader, "Association"))
            {
                this.HandleAssociationElement(reader);
                return true;
            }
            if ((this.DataModel == SchemaDataModelOption.EntityDataModel) && base.CanHandleElement(reader, "Using"))
            {
                this.HandleUsingElement(reader);
                return true;
            }
            if ((this.DataModel == SchemaDataModelOption.EntityDataModel) || (this.DataModel == SchemaDataModelOption.ProviderDataModel))
            {
                if (base.CanHandleElement(reader, "EntityContainer"))
                {
                    this.HandleEntityContainerTypeElement(reader);
                    return true;
                }
                if ((this.DataModel == SchemaDataModelOption.ProviderDataModel) && base.CanHandleElement(reader, "Function"))
                {
                    this.HandleFunctionElement(reader);
                    return true;
                }
            }
            else
            {
                if (base.CanHandleElement(reader, "Types"))
                {
                    this.SkipThroughElement(reader);
                    return true;
                }
                if (base.CanHandleElement(reader, "Functions"))
                {
                    this.SkipThroughElement(reader);
                    return true;
                }
                if (base.CanHandleElement(reader, "Function"))
                {
                    this.HandleFunctionElement(reader);
                    return true;
                }
                if (base.CanHandleElement(reader, "Type"))
                {
                    this.HandleTypeInformationElement(reader);
                    return true;
                }
            }
            return false;
        }

        private void HandleEntityContainerTypeElement(XmlReader reader)
        {
            System.Data.EntityModel.SchemaObjectModel.EntityContainer schemaType = new System.Data.EntityModel.SchemaObjectModel.EntityContainer(this);
            schemaType.Parse(reader);
            this.TryAddContainer(schemaType, true);
        }

        private void HandleEntityTypeElement(XmlReader reader)
        {
            SchemaEntityType schemaType = new SchemaEntityType(this);
            schemaType.Parse(reader);
            this.TryAddType(schemaType, true);
        }

        private void HandleFunctionElement(XmlReader reader)
        {
            System.Data.EntityModel.SchemaObjectModel.Function item = new System.Data.EntityModel.SchemaObjectModel.Function(this);
            item.Parse(reader);
            this.Functions.Add(item);
        }

        private void HandleInlineTypeElement(XmlReader reader)
        {
            SchemaComplexType schemaType = new SchemaComplexType(this);
            schemaType.Parse(reader);
            this.TryAddType(schemaType, true);
        }

        private void HandleNamespaceAttribute(XmlReader reader)
        {
            ReturnValue<string> value2 = base.HandleDottedNameAttribute(reader, this.Namespace, new Func<object, string>(Strings.AlreadyDefined));
            if (value2.Succeeded)
            {
                this.Namespace = value2.Value;
            }
        }

        private void HandleProviderAttribute(XmlReader reader)
        {
            string token = reader.Value;
            this._schemaManager.ProviderNotification(token, (message, code, severity) => this.AddError(code, severity, reader, message));
        }

        private void HandleProviderManifestTokenAttribute(XmlReader reader)
        {
            string token = reader.Value;
            this._schemaManager.ProviderManifestTokenNotification(token, (message, code, severity) => this.AddError(code, severity, reader, message));
        }

        private void HandleTopLevelSchemaElement(XmlReader reader)
        {
            try
            {
                this._depth += 2;
                base.Parse(reader);
            }
            finally
            {
                this._depth -= 2;
            }
        }

        private void HandleTypeInformationElement(XmlReader reader)
        {
            TypeElement schemaType = new TypeElement(this);
            schemaType.Parse(reader);
            this.TryAddType(schemaType, true);
        }

        private void HandleUsingElement(XmlReader reader)
        {
            UsingElement usingElement = new UsingElement(this);
            usingElement.Parse(reader);
            this.AliasResolver.Add(usingElement);
        }

        private IList<EdmSchemaError> InternalParse(XmlReader sourceReader, string sourceLocation)
        {
            base.Schema = this;
            this.Location = sourceLocation;
            try
            {
                if (sourceReader.NodeType != XmlNodeType.Element)
                {
                    while (sourceReader.Read() && (sourceReader.NodeType != XmlNodeType.Element))
                    {
                    }
                }
                base.GetPositionInfo(sourceReader);
                List<string> primarySchemaNamespaces = SomSchemaSetHelper.GetPrimarySchemaNamespaces(this.DataModel);
                if (sourceReader.EOF)
                {
                    if (sourceLocation != null)
                    {
                        base.AddError(ErrorCode.EmptyFile, EdmSchemaErrorSeverity.Error, Strings.EmptyFile(sourceLocation));
                    }
                    else
                    {
                        base.AddError(ErrorCode.EmptyFile, EdmSchemaErrorSeverity.Error, Strings.EmptySchemaTextReader);
                    }
                }
                else if (!primarySchemaNamespaces.Contains(sourceReader.NamespaceURI))
                {
                    Func<object, object, object, string> func = new Func<object, object, object, string>(Strings.UnexpectedRootElement);
                    if (string.IsNullOrEmpty(sourceReader.NamespaceURI))
                    {
                        func = new Func<object, object, object, string>(Strings.UnexpectedRootElementNoNamespace);
                    }
                    string commaDelimitedString = Helper.GetCommaDelimitedString(primarySchemaNamespaces);
                    base.AddError(ErrorCode.UnexpectedXmlElement, EdmSchemaErrorSeverity.Error, func(sourceReader.NamespaceURI, sourceReader.LocalName, commaDelimitedString));
                }
                else
                {
                    string str2;
                    this.SchemaXmlNamespace = sourceReader.NamespaceURI;
                    if (this.DataModel == SchemaDataModelOption.EntityDataModel)
                    {
                        if (this.SchemaXmlNamespace == "http://schemas.microsoft.com/ado/2006/04/edm")
                        {
                            this.EdmVersion = 1.0;
                        }
                        else
                        {
                            this.EdmVersion = 1.1;
                        }
                    }
                    if (((str2 = sourceReader.LocalName) != null) && ((str2 == "Schema") || (str2 == "ProviderManifest")))
                    {
                        this.HandleTopLevelSchemaElement(sourceReader);
                        sourceReader.Read();
                    }
                    else
                    {
                        base.AddError(ErrorCode.UnexpectedXmlElement, EdmSchemaErrorSeverity.Error, Strings.UnexpectedRootElement(sourceReader.NamespaceURI, sourceReader.LocalName, this.SchemaXmlNamespace));
                    }
                }
            }
            catch (InvalidOperationException exception)
            {
                base.AddError(ErrorCode.InternalError, EdmSchemaErrorSeverity.Error, exception.Message);
            }
            catch (UnauthorizedAccessException exception2)
            {
                base.AddError(ErrorCode.UnauthorizedAccessException, EdmSchemaErrorSeverity.Error, sourceReader, exception2);
            }
            catch (IOException exception3)
            {
                base.AddError(ErrorCode.IOException, EdmSchemaErrorSeverity.Error, sourceReader, exception3);
            }
            catch (SecurityException exception4)
            {
                base.AddError(ErrorCode.SecurityError, EdmSchemaErrorSeverity.Error, sourceReader, exception4);
            }
            catch (XmlException exception5)
            {
                base.AddError(ErrorCode.XmlError, EdmSchemaErrorSeverity.Error, sourceReader, exception5);
            }
            return this.ResetErrors();
        }

        public bool IsParseableXmlNamespace(string xmlNamespaceUri, bool isAttribute) => 
            ((string.IsNullOrEmpty(xmlNamespaceUri) && isAttribute) || this._parseableXmlNamespaces?.Contains(xmlNamespaceUri));

        public bool IsValidateableXmlNamespace(string xmlNamespaceUri, bool isAttribute) => 
            ((string.IsNullOrEmpty(xmlNamespaceUri) && isAttribute) || this._validatableXmlNamespaces?.Contains(xmlNamespaceUri));

        internal void OnSchemaValidationEvent(object sender, ValidationEventArgs e)
        {
            XmlReader reader = sender as XmlReader;
            if ((reader == null) || this.IsValidateableXmlNamespace(reader.NamespaceURI, reader.NodeType == XmlNodeType.Attribute))
            {
                EdmSchemaErrorSeverity error = EdmSchemaErrorSeverity.Error;
                if (e.Severity == XmlSeverityType.Warning)
                {
                    error = EdmSchemaErrorSeverity.Warning;
                }
                base.AddError(ErrorCode.XmlError, error, e.Exception.LineNumber, e.Exception.LinePosition, e.Message);
            }
        }

        internal IList<EdmSchemaError> Parse(XmlReader sourceReader, string sourceLocation)
        {
            XmlReader reader = null;
            try
            {
                XmlReaderSettings settings = this.CreateXmlReaderSettings();
                reader = XmlReader.Create(sourceReader, settings);
                return this.InternalParse(reader, sourceLocation);
            }
            catch (IOException exception)
            {
                base.AddError(ErrorCode.IOException, EdmSchemaErrorSeverity.Error, sourceReader, exception);
            }
            return this.ResetErrors();
        }

        protected override bool ProhibitAttribute(string namespaceUri, string localName) => 
            (base.ProhibitAttribute(namespaceUri, localName) || (((namespaceUri == null) && (localName == "Name")) && false));

        private List<EdmSchemaError> ResetErrors()
        {
            List<EdmSchemaError> list = this._errors;
            this._errors = new List<EdmSchemaError>();
            return list;
        }

        internal IList<EdmSchemaError> Resolve()
        {
            this.ResolveTopLevelNames();
            if (this._errors.Count == 0)
            {
                this.ResolveSecondLevelNames();
            }
            return this.ResetErrors();
        }

        internal override void ResolveSecondLevelNames()
        {
            base.ResolveSecondLevelNames();
            foreach (SchemaElement element in this.SchemaTypes)
            {
                element.ResolveSecondLevelNames();
            }
            foreach (System.Data.EntityModel.SchemaObjectModel.Function function in this.Functions)
            {
                function.ResolveSecondLevelNames();
            }
        }

        internal override void ResolveTopLevelNames()
        {
            base.ResolveTopLevelNames();
            this.AliasResolver.ResolveNamespaces();
            foreach (SchemaElement element in this.SchemaTypes)
            {
                element.ResolveTopLevelNames();
            }
            foreach (System.Data.EntityModel.SchemaObjectModel.Function function in this.Functions)
            {
                function.ResolveTopLevelNames();
            }
        }

        internal bool ResolveTypeName(SchemaElement usingElement, string typeName, out System.Data.EntityModel.SchemaObjectModel.SchemaType type)
        {
            string str;
            string str2;
            string str4;
            type = null;
            Utils.ExtractNamespaceAndName(this.DataModel, typeName, out str, out str2);
            string alias = str;
            if (alias == null)
            {
                alias = (this.ProviderManifest == null) ? this._namespaceName : this.ProviderManifest.NamespaceName;
            }
            if ((str == null) || !this.AliasResolver.TryResolveAlias(alias, out str4))
            {
                str4 = alias;
            }
            if (!this.SchemaManager.TryResolveType(str4, str2, out type))
            {
                if (str == null)
                {
                    usingElement.AddError(ErrorCode.NotInNamespace, EdmSchemaErrorSeverity.Error, Strings.NotNamespaceQualified(typeName));
                }
                else if (!this.SchemaManager.IsValidNamespaceName(str4))
                {
                    usingElement.AddError(ErrorCode.BadNamespace, EdmSchemaErrorSeverity.Error, Strings.BadNamespaceOrAlias(str));
                }
                else if (str4 != alias)
                {
                    usingElement.AddError(ErrorCode.NotInNamespace, EdmSchemaErrorSeverity.Error, Strings.NotInNamespaceAlias(str2, str4, alias));
                }
                else
                {
                    usingElement.AddError(ErrorCode.NotInNamespace, EdmSchemaErrorSeverity.Error, Strings.NotInNamespaceNoAlias(str2, str4));
                }
                return false;
            }
            if (((this.DataModel == SchemaDataModelOption.EntityDataModel) || (type.Schema == this)) || (type.Schema == this.SchemaManager.PrimitiveSchema))
            {
                return true;
            }
            if (type.Namespace != this.Namespace)
            {
                usingElement.AddError(ErrorCode.InvalidNamespaceOrAliasSpecified, EdmSchemaErrorSeverity.Error, Strings.InvalidNamespaceOrAliasSpecified(str));
            }
            else
            {
                usingElement.AddError(ErrorCode.CannotReferTypeAcrossSchema, EdmSchemaErrorSeverity.Error, Strings.CannotReferTypeAcrossSchema(str2));
            }
            return false;
        }

        protected override void SkipThroughElement(XmlReader reader)
        {
            try
            {
                this._depth++;
                base.SkipThroughElement(reader);
            }
            finally
            {
                this._depth--;
            }
        }

        protected void TryAddContainer(System.Data.EntityModel.SchemaObjectModel.SchemaType schemaType, bool doNotAddErrorForEmptyName)
        {
            this.SchemaManager.SchemaTypes.Add(schemaType, doNotAddErrorForEmptyName, new Func<object, string>(Strings.EntityContainerAlreadyExists));
            this.SchemaTypes.Add(schemaType);
        }

        protected void TryAddType(System.Data.EntityModel.SchemaObjectModel.SchemaType schemaType, bool doNotAddErrorForEmptyName)
        {
            this.SchemaManager.SchemaTypes.Add(schemaType, doNotAddErrorForEmptyName, new Func<object, string>(Strings.TypeNameAlreadyDefinedDuplicate));
            this.SchemaTypes.Add(schemaType);
        }

        internal override void Validate()
        {
            if (!string.IsNullOrEmpty(this.Alias) && EdmItemCollection.IsSystemNamespace(this.ProviderManifest, this.Alias))
            {
                base.AddError(ErrorCode.CannotUseSystemNamespaceAsAlias, EdmSchemaErrorSeverity.Error, Strings.CannotUseSystemNamespaceAsAlias(this.Alias));
            }
            if ((this.ProviderManifest != null) && EdmItemCollection.IsSystemNamespace(this.ProviderManifest, this.Namespace))
            {
                base.AddError(ErrorCode.SystemNamespace, EdmSchemaErrorSeverity.Error, Strings.SystemNamespaceEncountered(this.Namespace));
            }
            foreach (SchemaElement element in this.SchemaTypes)
            {
                element.Validate();
            }
            foreach (System.Data.EntityModel.SchemaObjectModel.Function function in this.Functions)
            {
                this.AddFunctionType(function);
                function.Validate();
            }
        }

        internal IList<EdmSchemaError> ValidateSchema()
        {
            this.Validate();
            return this.ResetErrors();
        }

        internal string Alias
        {
            virtual get => 
                this._alias;
            private set
            {
                this._alias = value;
            }
        }

        internal System.Data.EntityModel.SchemaObjectModel.AliasResolver AliasResolver
        {
            get
            {
                if (this._aliasResolver == null)
                {
                    this._aliasResolver = new System.Data.EntityModel.SchemaObjectModel.AliasResolver(this);
                }
                return this._aliasResolver;
            }
        }

        internal SchemaDataModelOption DataModel =>
            this.SchemaManager.DataModel;

        internal double EdmVersion
        {
            get => 
                this._edmVersion;
            set
            {
                this._edmVersion = value;
            }
        }

        public override string FQName =>
            this.Namespace;

        private List<System.Data.EntityModel.SchemaObjectModel.Function> Functions
        {
            get
            {
                if (this._functions == null)
                {
                    this._functions = new List<System.Data.EntityModel.SchemaObjectModel.Function>();
                }
                return this._functions;
            }
        }

        internal string Location
        {
            get => 
                this._location;
            private set
            {
                this._location = value;
            }
        }

        internal string Namespace
        {
            virtual get => 
                this._namespaceName;
            private set
            {
                this._namespaceName = value;
            }
        }

        internal DbProviderManifest ProviderManifest =>
            this._schemaManager.GetProviderManifest(delegate (string message, ErrorCode code, EdmSchemaErrorSeverity severity) {
                base.AddError(code, severity, message);
            });

        internal System.Data.EntityModel.SchemaObjectModel.SchemaManager SchemaManager =>
            this._schemaManager;

        internal List<System.Data.EntityModel.SchemaObjectModel.SchemaType> SchemaTypes
        {
            get
            {
                if (this._schemaTypes == null)
                {
                    this._schemaTypes = new List<System.Data.EntityModel.SchemaObjectModel.SchemaType>();
                }
                return this._schemaTypes;
            }
        }

        internal string SchemaXmlNamespace
        {
            get => 
                this._schemaXmlNamespace;
            private set
            {
                this._schemaXmlNamespace = value;
            }
        }

        private static class SomSchemaSetHelper
        {
            private static Memoizer<SchemaDataModelOption, XmlSchemaSet> _cachedSchemaSets = new Memoizer<SchemaDataModelOption, XmlSchemaSet>(new Func<SchemaDataModelOption, XmlSchemaSet>(Schema.SomSchemaSetHelper.ComputeSchemaSet), EqualityComparer<SchemaDataModelOption>.Default);

            private static void AddXmlSchemaToSet(XmlSchemaSet schemaSet, XmlSchemaResource schemaResource, HashSet<string> schemasAlreadyAdded)
            {
                foreach (XmlSchemaResource resource in schemaResource.ImportedSchemas)
                {
                    AddXmlSchemaToSet(schemaSet, resource, schemasAlreadyAdded);
                }
                if (!schemasAlreadyAdded.Contains(schemaResource.NamespaceUri))
                {
                    XmlSchema schema = XmlSchema.Read(GetResourceStream(schemaResource.ResourceName), null);
                    schemaSet.Add(schema);
                    schemasAlreadyAdded.Add(schemaResource.NamespaceUri);
                }
            }

            private static XmlSchemaSet ComputeSchemaSet(SchemaDataModelOption dataModel)
            {
                List<string> primarySchemaNamespaces = GetPrimarySchemaNamespaces(dataModel);
                XmlSchemaSet schemaSet = new XmlSchemaSet();
                Dictionary<string, XmlSchemaResource> schemaResourceMap = GetSchemaResourceMap();
                HashSet<string> schemasAlreadyAdded = new HashSet<string>();
                foreach (string str in primarySchemaNamespaces)
                {
                    XmlSchemaResource schemaResource = schemaResourceMap[str];
                    AddXmlSchemaToSet(schemaSet, schemaResource, schemasAlreadyAdded);
                }
                schemaSet.Compile();
                return schemaSet;
            }

            internal static List<string> GetPrimarySchemaNamespaces(SchemaDataModelOption dataModel)
            {
                List<string> list = new List<string>();
                if (dataModel == SchemaDataModelOption.EntityDataModel)
                {
                    list.Add("http://schemas.microsoft.com/ado/2006/04/edm");
                    list.Add("http://schemas.microsoft.com/ado/2007/05/edm");
                    return list;
                }
                if (dataModel == SchemaDataModelOption.ProviderDataModel)
                {
                    list.Add("http://schemas.microsoft.com/ado/2006/04/edm/ssdl");
                    return list;
                }
                list.Add("http://schemas.microsoft.com/ado/2006/04/edm/providermanifest");
                return list;
            }

            private static Stream GetResourceStream(string resourceName)
            {
                Stream manifestResourceStream = null;
                Assembly executingAssembly = Assembly.GetExecutingAssembly();
                if (executingAssembly != null)
                {
                    manifestResourceStream = executingAssembly.GetManifestResourceStream(resourceName);
                }
                if (manifestResourceStream == null)
                {
                    throw EntityUtil.Data(Strings.MissingAssemblyResource(resourceName));
                }
                return manifestResourceStream;
            }

            public static Dictionary<string, XmlSchemaResource> GetSchemaResourceMap()
            {
                XmlSchemaResource[] importedSchemas = new XmlSchemaResource[] { new XmlSchemaResource("http://schemas.microsoft.com/ado/2006/04/codegeneration", "System.Data.Resources.CodeGenerationSchema.xsd") };
                Dictionary<string, XmlSchemaResource> dictionary = new Dictionary<string, XmlSchemaResource>(StringComparer.Ordinal);
                XmlSchemaResource resource = new XmlSchemaResource("http://schemas.microsoft.com/ado/2006/04/edm", "System.Data.Resources.CSDLSchema.xsd", importedSchemas);
                dictionary.Add(resource.NamespaceUri, resource);
                XmlSchemaResource resource2 = new XmlSchemaResource("http://schemas.microsoft.com/ado/2007/05/edm", "System.Data.Resources.CSDLSchema_1_1.xsd", importedSchemas);
                dictionary.Add(resource2.NamespaceUri, resource2);
                XmlSchemaResource[] resourceArray2 = new XmlSchemaResource[] { new XmlSchemaResource("http://schemas.microsoft.com/ado/2007/12/edm/EntityStoreSchemaGenerator", "System.Data.Resources.EntityStoreSchemaGenerator.xsd") };
                XmlSchemaResource resource3 = new XmlSchemaResource("http://schemas.microsoft.com/ado/2006/04/edm/ssdl", "System.Data.Resources.SSDLSchema.xsd", resourceArray2);
                dictionary.Add(resource3.NamespaceUri, resource3);
                XmlSchemaResource resource4 = new XmlSchemaResource("http://schemas.microsoft.com/ado/2006/04/edm/providermanifest", "System.Data.Resources.ProviderServices.ProviderManifest.xsd");
                dictionary.Add(resource4.NamespaceUri, resource4);
                return dictionary;
            }

            internal static XmlSchemaSet GetSchemaSet(SchemaDataModelOption dataModel) => 
                _cachedSchemaSets.Evaluate(dataModel);

            [StructLayout(LayoutKind.Sequential)]
            public struct XmlSchemaResource
            {
                private static Schema.SomSchemaSetHelper.XmlSchemaResource[] EmptyImportList;
                public string NamespaceUri;
                public string ResourceName;
                public Schema.SomSchemaSetHelper.XmlSchemaResource[] ImportedSchemas;
                public XmlSchemaResource(string namespaceUri, string resourceName, Schema.SomSchemaSetHelper.XmlSchemaResource[] importedSchemas)
                {
                    this.NamespaceUri = namespaceUri;
                    this.ResourceName = resourceName;
                    this.ImportedSchemas = importedSchemas;
                }

                public XmlSchemaResource(string namespaceUri, string resourceName)
                {
                    this.NamespaceUri = namespaceUri;
                    this.ResourceName = resourceName;
                    this.ImportedSchemas = EmptyImportList;
                }

                static XmlSchemaResource()
                {
                    EmptyImportList = new Schema.SomSchemaSetHelper.XmlSchemaResource[0];
                }
            }
        }
    }
}

