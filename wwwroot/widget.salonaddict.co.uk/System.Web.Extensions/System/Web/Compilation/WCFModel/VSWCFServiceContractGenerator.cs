namespace System.Web.Compilation.WCFModel
{
    using System;
    using System.CodeDom;
    using System.CodeDom.Compiler;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Configuration;
    using System.Data;
    using System.Globalization;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Runtime.Serialization;
    using System.Security.Permissions;
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Configuration;
    using System.ServiceModel.Description;
    using System.Web.Compilation;
    using System.Web.Resources;
    using System.Web.Services.Description;
    using System.Xml;
    using System.Xml.Schema;
    using System.Xml.Serialization;

    [PermissionSet(SecurityAction.LinkDemand, Name="FullTrust"), PermissionSet(SecurityAction.InheritanceDemand, Name="FullTrust")]
    internal class VSWCFServiceContractGenerator
    {
        private IEnumerable<System.ServiceModel.Channels.Binding> bindingCollection;
        private IEnumerable<ContractDescription> contractCollection;
        private const int FRAMEWORK_VERSION_35 = 0x30005;
        private IList<ProxyGenerationError> importErrors;
        private static CodeAttributeDeclaration outAttribute;
        private List<GeneratedContractType> proxyGeneratedContractTypes;
        private IEnumerable<ProxyGenerationError> proxyGenerationErrors;
        private List<ServiceEndpoint> serviceEndpointList;
        private Dictionary<ServiceEndpoint, ChannelEndpointElement> serviceEndpointToChannelEndpointElementMap;
        private CodeCompileUnit targetCompileUnit;
        private System.Configuration.Configuration targetConfiguration;
        private static Type[] unsupportedTypesInFramework30 = new Type[] { typeof(DateTimeOffset) };
        private const string VB_LANGUAGE_NAME = "vb";

        protected VSWCFServiceContractGenerator(List<ProxyGenerationError> importErrors, CodeCompileUnit targetCompileUnit, System.Configuration.Configuration targetConfiguration, IEnumerable<System.ServiceModel.Channels.Binding> bindingCollection, IEnumerable<ContractDescription> contractCollection, List<ServiceEndpoint> serviceEndpointList, Dictionary<ServiceEndpoint, ChannelEndpointElement> serviceEndpointToChannelEndpointElementMap, List<GeneratedContractType> proxyGeneratedContractTypes, IEnumerable<ProxyGenerationError> proxyGenerationErrors)
        {
            if (importErrors == null)
            {
                throw new ArgumentNullException("importErrors");
            }
            if (targetCompileUnit == null)
            {
                throw new ArgumentNullException("targetCompileUnit");
            }
            if (bindingCollection == null)
            {
                throw new ArgumentNullException("bindingCollection");
            }
            if (contractCollection == null)
            {
                throw new ArgumentNullException("contractCollection");
            }
            if (serviceEndpointList == null)
            {
                throw new ArgumentNullException("serviceEndpointList");
            }
            if (serviceEndpointToChannelEndpointElementMap == null)
            {
                throw new ArgumentNullException("serviceEndpointToChannelEndpointElementMap");
            }
            if (proxyGeneratedContractTypes == null)
            {
                throw new ArgumentNullException("proxyGeneratedContractTypes");
            }
            if (proxyGenerationErrors == null)
            {
                throw new ArgumentNullException("proxyGenerationErrors");
            }
            this.importErrors = importErrors;
            this.targetCompileUnit = targetCompileUnit;
            this.targetConfiguration = targetConfiguration;
            this.bindingCollection = bindingCollection;
            this.contractCollection = contractCollection;
            this.serviceEndpointList = serviceEndpointList;
            this.serviceEndpointToChannelEndpointElementMap = serviceEndpointToChannelEndpointElementMap;
            this.proxyGeneratedContractTypes = proxyGeneratedContractTypes;
            this.proxyGenerationErrors = proxyGenerationErrors;
        }

        private static void CheckDuplicatedWsdlItems(IList<MetadataSection> metadataCollection, IList<ProxyGenerationError> importErrors)
        {
            List<System.Web.Services.Description.ServiceDescription> wsdlFiles = new List<System.Web.Services.Description.ServiceDescription>();
            foreach (MetadataSection section in metadataCollection)
            {
                if (section.Dialect == MetadataSection.ServiceDescriptionDialect)
                {
                    System.Web.Services.Description.ServiceDescription metadata = (System.Web.Services.Description.ServiceDescription) section.Metadata;
                    wsdlFiles.Add(metadata);
                }
            }
            WsdlInspector.CheckDuplicatedWsdlItems(wsdlFiles, importErrors);
        }

        protected static List<MetadataSection> CollectMetadataDocuments(IEnumerable<MetadataFile> metadataList, IList<ProxyGenerationError> importErrors)
        {
            List<MetadataSection> metadataCollection = new List<MetadataSection>();
            foreach (MetadataFile file in metadataList)
            {
                if (!file.Ignore)
                {
                    try
                    {
                        MetadataSection item = file.CreateMetadataSection();
                        if (item != null)
                        {
                            metadataCollection.Add(item);
                        }
                    }
                    catch (Exception exception)
                    {
                        importErrors.Add(ConvertMetadataErrorToProxyGenerationError(file, exception));
                    }
                }
            }
            RemoveDuplicatedSchemaItems(metadataCollection, importErrors);
            CheckDuplicatedWsdlItems(metadataCollection, importErrors);
            return metadataCollection;
        }

        private static bool ContainsHttpBindings(IEnumerable<MetadataSection> metadataCollection)
        {
            foreach (MetadataSection section in metadataCollection)
            {
                if (section.Dialect == MetadataSection.ServiceDescriptionDialect)
                {
                    System.Web.Services.Description.ServiceDescription metadata = (System.Web.Services.Description.ServiceDescription) section.Metadata;
                    if (ContainsHttpBindings(metadata))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        internal static bool ContainsHttpBindings(System.Web.Services.Description.ServiceDescription wsdlFile)
        {
            foreach (System.Web.Services.Description.Binding binding in wsdlFile.Bindings)
            {
                foreach (object obj2 in binding.Extensions)
                {
                    if (obj2 is HttpBinding)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        internal static ProxyGenerationError ConvertMetadataErrorToProxyGenerationError(MetadataFile metadataItem, Exception ex)
        {
            if (ex is XmlSchemaException)
            {
                return new ProxyGenerationError(ProxyGenerationError.GeneratorState.LoadMetadata, metadataItem.FileName, (XmlSchemaException) ex);
            }
            if (ex is XmlException)
            {
                return new ProxyGenerationError(ProxyGenerationError.GeneratorState.LoadMetadata, metadataItem.FileName, (XmlException) ex);
            }
            if (ex is InvalidOperationException)
            {
                XmlSchemaException innerException = ex.InnerException as XmlSchemaException;
                if (innerException != null)
                {
                    return new ProxyGenerationError(ProxyGenerationError.GeneratorState.LoadMetadata, metadataItem.FileName, innerException);
                }
                XmlException errorException = ex.InnerException as XmlException;
                if (errorException != null)
                {
                    return new ProxyGenerationError(ProxyGenerationError.GeneratorState.LoadMetadata, metadataItem.FileName, errorException);
                }
                return new ProxyGenerationError(ProxyGenerationError.GeneratorState.LoadMetadata, metadataItem.FileName, (InvalidOperationException) ex);
            }
            return new ProxyGenerationError(ProxyGenerationError.GeneratorState.LoadMetadata, metadataItem.FileName, ex);
        }

        protected static ServiceContractGenerator CreateContractGenerator(ClientOptions proxyOptions, WsdlImporter wsdlImporter, CodeCompileUnit targetCompileUnit, string proxyNamespace, System.Configuration.Configuration targetConfiguration, IContractGeneratorReferenceTypeLoader typeLoader, int targetFrameworkVersion, IList<ProxyGenerationError> importErrors)
        {
            ServiceContractGenerator generator = new ServiceContractGenerator(targetCompileUnit, targetConfiguration) {
                NamespaceMappings = { { 
                    "*",
                    proxyNamespace
                } }
            };
            if (proxyOptions.GenerateInternalTypes)
            {
                generator.Options |= ServiceContractGenerationOptions.InternalTypes;
            }
            else
            {
                generator.Options &= ~ServiceContractGenerationOptions.InternalTypes;
            }
            if (proxyOptions.GenerateAsynchronousMethods)
            {
                generator.Options |= ServiceContractGenerationOptions.AsynchronousMethods;
                if (targetFrameworkVersion >= 0x30005)
                {
                    generator.Options |= ServiceContractGenerationOptions.EventBasedAsynchronousMethods;
                }
            }
            else
            {
                generator.Options &= ~ServiceContractGenerationOptions.AsynchronousMethods;
            }
            if (proxyOptions.GenerateMessageContracts)
            {
                generator.Options |= ServiceContractGenerationOptions.TypedMessages;
            }
            else
            {
                generator.Options &= ~ServiceContractGenerationOptions.TypedMessages;
            }
            if (typeLoader != null)
            {
                foreach (ContractMapping mapping in proxyOptions.ServiceContractMappingList)
                {
                    try
                    {
                        Type t = typeLoader.LoadType(mapping.TypeName);
                        if (!IsTypeShareable(t))
                        {
                            importErrors.Add(new ProxyGenerationError(ProxyGenerationError.GeneratorState.GenerateCode, string.Empty, new FormatException(string.Format(CultureInfo.CurrentCulture, WCFModelStrings.ReferenceGroup_SharedTypeMustBePublic, new object[] { mapping.TypeName }))));
                        }
                        else
                        {
                            ContractDescription contract = ContractDescription.GetContract(t);
                            if (!string.Equals(mapping.Name, contract.Name, StringComparison.Ordinal) || !string.Equals(mapping.TargetNamespace, contract.Namespace, StringComparison.Ordinal))
                            {
                                importErrors.Add(new ProxyGenerationError(ProxyGenerationError.GeneratorState.GenerateCode, string.Empty, new FormatException(string.Format(CultureInfo.CurrentCulture, WCFModelStrings.ReferenceGroup_ServiceContractMappingMissMatch, new object[] { mapping.TypeName, contract.Namespace, contract.Name, mapping.TargetNamespace, mapping.Name }))));
                            }
                            XmlQualifiedName key = new XmlQualifiedName(contract.Name, contract.Namespace);
                            wsdlImporter.KnownContracts.Add(key, contract);
                            generator.ReferencedTypes.Add(contract, t);
                        }
                    }
                    catch (Exception exception)
                    {
                        importErrors.Add(new ProxyGenerationError(ProxyGenerationError.GeneratorState.GenerateCode, string.Empty, exception));
                    }
                }
            }
            foreach (NamespaceMapping mapping2 in proxyOptions.NamespaceMappingList)
            {
                generator.NamespaceMappings.Add(mapping2.TargetNamespace, mapping2.ClrNamespace);
            }
            return generator;
        }

        protected static XsdDataContractImporter CreateDataContractImporter(ClientOptions proxyOptions, CodeCompileUnit targetCompileUnit, CodeDomProvider codeDomProvider, string proxyNamespace, IContractGeneratorReferenceTypeLoader typeLoader, int targetFrameworkVersion, IList<ProxyGenerationError> importErrors)
        {
            XsdDataContractImporter importer = new XsdDataContractImporter(targetCompileUnit);
            ImportOptions options = new ImportOptions {
                CodeProvider = codeDomProvider,
                Namespaces = { { 
                    "*",
                    proxyNamespace
                } },
                GenerateInternal = proxyOptions.GenerateInternalTypes,
                GenerateSerializable = proxyOptions.GenerateSerializableTypes,
                EnableDataBinding = proxyOptions.EnableDataBinding,
                ImportXmlType = proxyOptions.ImportXmlTypes
            };
            if (typeLoader != null)
            {
                IEnumerable<Type> enumerable = LoadSharedDataContractTypes(proxyOptions, typeLoader, targetFrameworkVersion, importErrors);
                if (enumerable != null)
                {
                    foreach (Type type in enumerable)
                    {
                        options.ReferencedTypes.Add(type);
                    }
                }
                IEnumerable<Type> enumerable2 = LoadSharedCollectionTypes(proxyOptions, typeLoader, importErrors);
                if (enumerable2 != null)
                {
                    foreach (Type type2 in enumerable2)
                    {
                        options.ReferencedCollectionTypes.Add(type2);
                    }
                }
            }
            foreach (NamespaceMapping mapping in proxyOptions.NamespaceMappingList)
            {
                options.Namespaces.Add(mapping.TargetNamespace, mapping.ClrNamespace);
            }
            importer.Options = options;
            return importer;
        }

        private static Dictionary<string, byte[]> CreateDictionaryOfCopiedExtensionFiles(SvcMapFile svcMapFile)
        {
            Dictionary<string, byte[]> dictionary = new Dictionary<string, byte[]>();
            foreach (ExtensionFile file in svcMapFile.Extensions)
            {
                if ((file.ContentBuffer != null) && file.IsBufferValid)
                {
                    dictionary.Add(file.Name, (byte[]) file.ContentBuffer.Clone());
                }
            }
            return dictionary;
        }

        protected static WsdlImporter CreateWsdlImporter(SvcMapFile svcMapFile, System.Configuration.Configuration toolConfiguration, CodeCompileUnit targetCompileUnit, CodeDomProvider codeDomProvider, string targetNamespace, IServiceProvider serviceProviderForImportExtensions, IContractGeneratorReferenceTypeLoader typeLoader, int targetFrameworkVersion, IList<ProxyGenerationError> importErrors, Type typedDataSetSchemaImporterExtension)
        {
            List<MetadataSection> metadataCollection = CollectMetadataDocuments(svcMapFile.MetadataList, importErrors);
            WsdlImporter importer = null;
            ClientOptions.ProxySerializerType serializer = svcMapFile.ClientOptions.Serializer;
            if ((serializer == ClientOptions.ProxySerializerType.Auto) && ContainsHttpBindings(metadataCollection))
            {
                serializer = ClientOptions.ProxySerializerType.XmlSerializer;
            }
            if (toolConfiguration != null)
            {
                ServiceModelSectionGroup sectionGroup = ServiceModelSectionGroup.GetSectionGroup(toolConfiguration);
                if (sectionGroup != null)
                {
                    Collection<IWsdlImportExtension> wsdlImportExtensions = sectionGroup.Client.Metadata.LoadWsdlImportExtensions();
                    Collection<IPolicyImportExtension> policyImportExtensions = sectionGroup.Client.Metadata.LoadPolicyImportExtensions();
                    switch (serializer)
                    {
                        case ClientOptions.ProxySerializerType.DataContractSerializer:
                            RemoveExtension(typeof(XmlSerializerMessageContractImporter), wsdlImportExtensions);
                            break;

                        case ClientOptions.ProxySerializerType.XmlSerializer:
                            RemoveExtension(typeof(DataContractSerializerMessageContractImporter), wsdlImportExtensions);
                            break;
                    }
                    ProvideImportExtensionsWithContextInformation(svcMapFile, serviceProviderForImportExtensions, wsdlImportExtensions, policyImportExtensions);
                    importer = new WsdlImporter(new MetadataSet(metadataCollection), policyImportExtensions, wsdlImportExtensions);
                }
            }
            if (importer == null)
            {
                importer = new WsdlImporter(new MetadataSet(metadataCollection));
            }
            importer.State.Add(typeof(XsdDataContractImporter), CreateDataContractImporter(svcMapFile.ClientOptions, targetCompileUnit, codeDomProvider, targetNamespace, typeLoader, targetFrameworkVersion, importErrors));
            if (serializer != ClientOptions.ProxySerializerType.DataContractSerializer)
            {
                importer.State.Add(typeof(XmlSerializerImportOptions), CreateXmlSerializerImportOptions(svcMapFile.ClientOptions, targetCompileUnit, codeDomProvider, targetNamespace, typedDataSetSchemaImporterExtension));
            }
            FaultImportOptions options = new FaultImportOptions {
                UseMessageFormat = svcMapFile.ClientOptions.UseSerializerForFaults
            };
            importer.State.Add(typeof(FaultImportOptions), options);
            WrappedOptions options2 = new WrappedOptions {
                WrappedFlag = svcMapFile.ClientOptions.Wrapped
            };
            importer.State.Add(typeof(WrappedOptions), options2);
            return importer;
        }

        protected static XmlSerializerImportOptions CreateXmlSerializerImportOptions(ClientOptions proxyOptions, CodeCompileUnit targetCompileUnit, CodeDomProvider codeDomProvider, string proxyNamespace, Type typedDataSetSchemaImporterExtension)
        {
            XmlSerializerImportOptions options = new XmlSerializerImportOptions(targetCompileUnit);
            WebReferenceOptions options2 = new WebReferenceOptions {
                CodeGenerationOptions = CodeGenerationOptions.GenerateOrder | CodeGenerationOptions.GenerateProperties
            };
            if (proxyOptions.EnableDataBinding)
            {
                options2.CodeGenerationOptions |= CodeGenerationOptions.EnableDataBinding;
            }
            options2.SchemaImporterExtensions.Add(typedDataSetSchemaImporterExtension.AssemblyQualifiedName);
            options2.SchemaImporterExtensions.Add(typeof(DataSetSchemaImporterExtension).AssemblyQualifiedName);
            options.WebReferenceOptions = options2;
            options.CodeProvider = codeDomProvider;
            options.ClrNamespace = proxyNamespace;
            return options;
        }

        public static VSWCFServiceContractGenerator GenerateCodeAndConfiguration(SvcMapFile svcMapFile, System.Configuration.Configuration toolConfiguration, CodeDomProvider codeDomProvider, string proxyNamespace, System.Configuration.Configuration targetConfiguration, string configurationNamespace, IServiceProvider serviceProviderForImportExtensions, IContractGeneratorReferenceTypeLoader typeLoader, int targetFrameworkVersion, Type typedDataSetSchemaImporterExtension)
        {
            if (svcMapFile == null)
            {
                throw new ArgumentNullException("svcMapFile");
            }
            if (codeDomProvider == null)
            {
                throw new ArgumentNullException("codeDomProvider");
            }
            if (typedDataSetSchemaImporterExtension == null)
            {
                throw new ArgumentNullException("typedDataSetSchemaImporterExtension");
            }
            List<ProxyGenerationError> importErrors = new List<ProxyGenerationError>();
            List<ProxyGenerationError> proxyGenerationErrors = new List<ProxyGenerationError>();
            CodeCompileUnit targetCompileUnit = new CodeCompileUnit();
            WsdlImporter wsdlImporter = CreateWsdlImporter(svcMapFile, toolConfiguration, targetCompileUnit, codeDomProvider, proxyNamespace, serviceProviderForImportExtensions, typeLoader, targetFrameworkVersion, importErrors, typedDataSetSchemaImporterExtension);
            ServiceContractGenerator contractGenerator = CreateContractGenerator(svcMapFile.ClientOptions, wsdlImporter, targetCompileUnit, proxyNamespace, targetConfiguration, typeLoader, targetFrameworkVersion, importErrors);
            try
            {
                IEnumerable<System.ServiceModel.Channels.Binding> enumerable;
                IEnumerable<ContractDescription> enumerable2;
                Dictionary<ServiceEndpoint, ChannelEndpointElement> dictionary;
                List<GeneratedContractType> list4;
                List<ServiceEndpoint> serviceEndpointList = new List<ServiceEndpoint>();
                ImportWCFModel(wsdlImporter, targetCompileUnit, importErrors, out serviceEndpointList, out enumerable, out enumerable2);
                GenerateProxy(contractGenerator, targetCompileUnit, proxyNamespace, configurationNamespace, enumerable2, enumerable, serviceEndpointList, proxyGenerationErrors, out dictionary, out list4);
                if (IsVBCodeDomProvider(codeDomProvider))
                {
                    PatchOutParametersInVB(targetCompileUnit);
                }
                return new VSWCFServiceContractGenerator(importErrors, targetCompileUnit, targetConfiguration, enumerable, enumerable2, serviceEndpointList, dictionary, list4, proxyGenerationErrors);
            }
            catch (Exception exception)
            {
                proxyGenerationErrors.Add(new ProxyGenerationError(ProxyGenerationError.GeneratorState.GenerateCode, string.Empty, exception, false));
                return new VSWCFServiceContractGenerator(importErrors, new CodeCompileUnit(), targetConfiguration, new List<System.ServiceModel.Channels.Binding>(), new List<ContractDescription>(), new List<ServiceEndpoint>(), new Dictionary<ServiceEndpoint, ChannelEndpointElement>(), new List<GeneratedContractType>(), proxyGenerationErrors);
            }
        }

        protected static void GenerateProxy(ServiceContractGenerator contractGenerator, CodeCompileUnit targetCompileUnit, string proxyNamespace, string configurationNamespace, IEnumerable<ContractDescription> contractCollection, IEnumerable<System.ServiceModel.Channels.Binding> bindingCollection, List<ServiceEndpoint> serviceEndpointList, IList<ProxyGenerationError> proxyGenerationErrors, out Dictionary<ServiceEndpoint, ChannelEndpointElement> serviceEndpointToChannelEndpointElementMap, out List<GeneratedContractType> proxyGeneratedContractTypes)
        {
            if (serviceEndpointList == null)
            {
                throw new ArgumentNullException("serviceEndpointList");
            }
            if (bindingCollection == null)
            {
                throw new ArgumentNullException("bindingCollection");
            }
            if (contractCollection == null)
            {
                throw new ArgumentNullException("contractCollection");
            }
            if (proxyGenerationErrors == null)
            {
                throw new ArgumentNullException("proxyGenerationErrors");
            }
            proxyGeneratedContractTypes = new List<GeneratedContractType>();
            serviceEndpointToChannelEndpointElementMap = new Dictionary<ServiceEndpoint, ChannelEndpointElement>();
            try
            {
                foreach (ContractDescription description in contractCollection)
                {
                    CodeTypeReference reference = contractGenerator.GenerateServiceContractType(description);
                    if (reference != null)
                    {
                        string baseType = reference.BaseType;
                        GeneratedContractType item = new GeneratedContractType(description.Namespace, description.Name, baseType, baseType);
                        proxyGeneratedContractTypes.Add(item);
                    }
                }
                if (contractGenerator.Configuration != null)
                {
                    foreach (ServiceEndpoint endpoint in serviceEndpointList)
                    {
                        ChannelEndpointElement channelElement = null;
                        contractGenerator.GenerateServiceEndpoint(endpoint, out channelElement);
                        serviceEndpointToChannelEndpointElementMap[endpoint] = channelElement;
                    }
                    foreach (System.ServiceModel.Channels.Binding binding in bindingCollection)
                    {
                        string bindingSectionName = null;
                        string configurationName = null;
                        contractGenerator.GenerateBinding(binding, out bindingSectionName, out configurationName);
                    }
                }
                PatchConfigurationName(proxyNamespace, configurationNamespace, proxyGeneratedContractTypes, serviceEndpointToChannelEndpointElementMap.Values, targetCompileUnit);
            }
            finally
            {
                foreach (MetadataConversionError error in contractGenerator.Errors)
                {
                    proxyGenerationErrors.Add(new ProxyGenerationError(error));
                }
            }
        }

        private static IEnumerable<Type> GetUnsupportedTypes(int targetFrameworkVersion)
        {
            if (targetFrameworkVersion < 0x30005)
            {
                return unsupportedTypesInFramework30;
            }
            return new Type[0];
        }

        protected static void ImportWCFModel(WsdlImporter importer, CodeCompileUnit compileUnit, IList<ProxyGenerationError> generationErrors, out List<ServiceEndpoint> serviceEndpointList, out IEnumerable<System.ServiceModel.Channels.Binding> bindingCollection, out IEnumerable<ContractDescription> contractCollection)
        {
            IWsdlImportExtension extension = new AsmxEndpointPickerExtension();
            extension.BeforeImport(importer.WsdlDocuments, null, null);
            serviceEndpointList = new List<ServiceEndpoint>();
            importer.ImportAllEndpoints();
            foreach (System.Web.Services.Description.ServiceDescription description in importer.WsdlDocuments)
            {
                foreach (Service service in description.Services)
                {
                    foreach (Port port in service.Ports)
                    {
                        try
                        {
                            ServiceEndpoint item = importer.ImportEndpoint(port);
                            serviceEndpointList.Add(item);
                        }
                        catch (InvalidOperationException)
                        {
                        }
                        catch (Exception exception)
                        {
                            generationErrors.Add(new ProxyGenerationError(ProxyGenerationError.GeneratorState.GenerateCode, description.RetrievalUrl, exception));
                        }
                    }
                }
            }
            bindingCollection = importer.ImportAllBindings();
            contractCollection = importer.ImportAllContracts();
            foreach (MetadataConversionError error in importer.Errors)
            {
                generationErrors.Add(new ProxyGenerationError(error));
            }
        }

        private static bool IsDefinedInCodeAttributeCollection(Type type, CodeAttributeDeclarationCollection metadata)
        {
            foreach (CodeAttributeDeclaration declaration in metadata)
            {
                if (string.Equals(declaration.Name, type.FullName, StringComparison.Ordinal) || string.Equals(declaration.Name, type.Name, StringComparison.Ordinal))
                {
                    return true;
                }
            }
            return false;
        }

        private static bool IsTypeShareable(Type t)
        {
            if (t == null)
            {
                return false;
            }
            if (!t.IsPublic)
            {
                return t.IsNestedPublic;
            }
            return true;
        }

        private static bool IsVBCodeDomProvider(CodeDomProvider codeDomProvider)
        {
            string fileExtension = codeDomProvider.FileExtension;
            try
            {
                return string.Equals(CodeDomProvider.GetLanguageFromExtension(fileExtension), "vb", StringComparison.OrdinalIgnoreCase);
            }
            catch (ConfigurationException)
            {
                return false;
            }
        }

        private static IEnumerable<Assembly> LoadReferenedAssemblies(ClientOptions proxyOptions, IContractGeneratorReferenceTypeLoader typeLoader, IList<ProxyGenerationError> importErrors)
        {
            List<Assembly> list = new List<Assembly>();
            if (proxyOptions.ReferenceAllAssemblies)
            {
                try
                {
                    IEnumerable<Exception> loadingErrors = null;
                    IEnumerable<Assembly> loadedAssemblies = null;
                    typeLoader.LoadAllAssemblies(out loadedAssemblies, out loadingErrors);
                    if (loadingErrors != null)
                    {
                        foreach (Exception exception in loadingErrors)
                        {
                            importErrors.Add(new ProxyGenerationError(ProxyGenerationError.GeneratorState.GenerateCode, string.Empty, exception, true));
                        }
                    }
                    if (loadedAssemblies != null)
                    {
                        list.AddRange(loadedAssemblies);
                    }
                }
                catch (Exception exception2)
                {
                    importErrors.Add(new ProxyGenerationError(ProxyGenerationError.GeneratorState.GenerateCode, string.Empty, exception2));
                }
            }
            foreach (ReferencedAssembly assembly in proxyOptions.ReferencedAssemblyList)
            {
                try
                {
                    Assembly item = typeLoader.LoadAssembly(assembly.AssemblyName);
                    if ((item != null) && !list.Contains(item))
                    {
                        list.Add(item);
                    }
                }
                catch (Exception exception3)
                {
                    importErrors.Add(new ProxyGenerationError(ProxyGenerationError.GeneratorState.GenerateCode, string.Empty, exception3));
                }
            }
            return list;
        }

        protected static IEnumerable<Type> LoadSharedCollectionTypes(ClientOptions proxyOptions, IContractGeneratorReferenceTypeLoader typeLoader, IList<ProxyGenerationError> importErrors)
        {
            List<Type> list = new List<Type>();
            foreach (ReferencedCollectionType type in proxyOptions.CollectionMappingList)
            {
                try
                {
                    Type t = typeLoader.LoadType(type.TypeName);
                    if (!IsTypeShareable(t))
                    {
                        importErrors.Add(new ProxyGenerationError(ProxyGenerationError.GeneratorState.GenerateCode, string.Empty, new FormatException(string.Format(CultureInfo.CurrentCulture, WCFModelStrings.ReferenceGroup_SharedTypeMustBePublic, new object[] { type.TypeName }))));
                    }
                    else
                    {
                        list.Add(t);
                    }
                }
                catch (Exception exception)
                {
                    importErrors.Add(new ProxyGenerationError(ProxyGenerationError.GeneratorState.GenerateCode, string.Empty, exception));
                }
            }
            return list;
        }

        protected static IEnumerable<Type> LoadSharedDataContractTypes(ClientOptions proxyOptions, IContractGeneratorReferenceTypeLoader typeLoader, int targetFrameworkVersion, IList<ProxyGenerationError> importErrors)
        {
            if (typeLoader == null)
            {
                throw new ArgumentNullException("typeLoader");
            }
            Dictionary<Type, ReferencedType> dictionary = new Dictionary<Type, ReferencedType>();
            IEnumerable<Assembly> enumerable = LoadReferenedAssemblies(proxyOptions, typeLoader, importErrors);
            if (enumerable != null)
            {
                foreach (Assembly assembly in enumerable)
                {
                    try
                    {
                        foreach (Type type in assembly.GetTypes())
                        {
                            if (IsTypeShareable(type))
                            {
                                dictionary.Add(type, null);
                            }
                        }
                    }
                    catch (Exception exception)
                    {
                        importErrors.Add(new ProxyGenerationError(ProxyGenerationError.GeneratorState.GenerateCode, string.Empty, exception, true));
                    }
                }
            }
            foreach (ReferencedType type2 in proxyOptions.ReferencedDataContractTypeList)
            {
                try
                {
                    Type t = typeLoader.LoadType(type2.TypeName);
                    if (!IsTypeShareable(t))
                    {
                        importErrors.Add(new ProxyGenerationError(ProxyGenerationError.GeneratorState.GenerateCode, string.Empty, new FormatException(string.Format(CultureInfo.CurrentCulture, WCFModelStrings.ReferenceGroup_SharedTypeMustBePublic, new object[] { type2.TypeName }))));
                    }
                    else
                    {
                        dictionary[t] = type2;
                    }
                }
                catch (Exception exception2)
                {
                    importErrors.Add(new ProxyGenerationError(ProxyGenerationError.GeneratorState.GenerateCode, string.Empty, exception2));
                }
            }
            foreach (ReferencedType type4 in proxyOptions.ExcludedTypeList)
            {
                try
                {
                    Type key = typeLoader.LoadType(type4.TypeName);
                    if (dictionary.ContainsKey(key))
                    {
                        if (dictionary[key] != null)
                        {
                            importErrors.Add(new ProxyGenerationError(ProxyGenerationError.GeneratorState.GenerateCode, string.Empty, new Exception(string.Format(CultureInfo.CurrentCulture, WCFModelStrings.ReferenceGroup_DataContractExcludedAndIncluded, new object[] { type4.TypeName }))));
                        }
                        dictionary.Remove(key);
                    }
                }
                catch (Exception exception3)
                {
                    importErrors.Add(new ProxyGenerationError(ProxyGenerationError.GeneratorState.GenerateCode, string.Empty, exception3, true));
                }
            }
            foreach (Type type6 in GetUnsupportedTypes(targetFrameworkVersion))
            {
                dictionary.Remove(type6);
            }
            return dictionary.Keys;
        }

        private static string MakePeriodTerminatedNamespacePrefix(string ns)
        {
            if (string.IsNullOrEmpty(ns))
            {
                return "";
            }
            if (!ns.EndsWith(".", StringComparison.Ordinal))
            {
                return (ns + ".");
            }
            return ns;
        }

        private static void PatchConfigurationName(string proxyNamespace, string configNamespace, IEnumerable<GeneratedContractType> generatedContracts, IEnumerable<ChannelEndpointElement> endpoints, CodeCompileUnit targetCompileUnit)
        {
            if ((configNamespace != null) && !configNamespace.Equals(proxyNamespace, StringComparison.Ordinal))
            {
                string originalNamespace = MakePeriodTerminatedNamespacePrefix(proxyNamespace);
                string replacementNamespace = MakePeriodTerminatedNamespacePrefix(configNamespace);
                foreach (GeneratedContractType type in generatedContracts)
                {
                    type.ConfigurationName = ReplaceNamespace(originalNamespace, replacementNamespace, type.ConfigurationName);
                }
                foreach (ChannelEndpointElement element in endpoints)
                {
                    element.Contract = ReplaceNamespace(originalNamespace, replacementNamespace, element.Contract);
                }
                PatchConfigurationNameInServiceContractAttribute(targetCompileUnit, proxyNamespace, configNamespace);
            }
        }

        private static void PatchConfigurationNameInServiceContractAttribute(CodeCompileUnit proxyCodeUnit, string proxyNamespace, string configNamespace)
        {
            if (proxyNamespace == null)
            {
                proxyNamespace = string.Empty;
            }
            string originalNamespace = MakePeriodTerminatedNamespacePrefix(proxyNamespace);
            string replacementNamespace = MakePeriodTerminatedNamespacePrefix(configNamespace);
            if (proxyCodeUnit != null)
            {
                foreach (CodeNamespace namespace2 in proxyCodeUnit.Namespaces)
                {
                    if (string.Equals(proxyNamespace, namespace2.Name, StringComparison.Ordinal))
                    {
                        foreach (CodeTypeDeclaration declaration in namespace2.Types)
                        {
                            if (declaration.IsInterface)
                            {
                                foreach (CodeAttributeDeclaration declaration2 in declaration.CustomAttributes)
                                {
                                    if (string.Equals(declaration2.AttributeType.BaseType, typeof(ServiceContractAttribute).FullName, StringComparison.Ordinal))
                                    {
                                        foreach (CodeAttributeArgument argument in declaration2.Arguments)
                                        {
                                            if (string.Equals(argument.Name, "ConfigurationName", StringComparison.Ordinal))
                                            {
                                                CodePrimitiveExpression expression = argument.Value as CodePrimitiveExpression;
                                                if ((expression != null) && (expression.Value is string))
                                                {
                                                    expression.Value = ReplaceNamespace(originalNamespace, replacementNamespace, (string) expression.Value);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private static void PatchOutParametersInVB(CodeCompileUnit codeCompileUnit)
        {
            foreach (CodeNamespace namespace2 in codeCompileUnit.Namespaces)
            {
                foreach (CodeTypeDeclaration declaration in namespace2.Types)
                {
                    PatchTypeDeclaration(declaration);
                }
            }
        }

        private static void PatchTypeDeclaration(CodeTypeDeclaration codeClass)
        {
            foreach (CodeTypeMember member in codeClass.Members)
            {
                if (member is CodeTypeDeclaration)
                {
                    PatchTypeDeclaration((CodeTypeDeclaration) member);
                }
                else if (member is CodeMemberMethod)
                {
                    CodeMemberMethod method = member as CodeMemberMethod;
                    foreach (CodeParameterDeclarationExpression expression in method.Parameters)
                    {
                        if ((expression.Direction == FieldDirection.Out) && !IsDefinedInCodeAttributeCollection(typeof(System.Runtime.InteropServices.OutAttribute), expression.CustomAttributes))
                        {
                            expression.CustomAttributes.Add(OutAttribute);
                        }
                    }
                }
            }
        }

        internal static void ProvideImportExtensionsWithContextInformation(SvcMapFile svcMapFile, IServiceProvider serviceProviderForImportExtensions, IEnumerable<IWsdlImportExtension> wsdlImportExtensions, IEnumerable<IPolicyImportExtension> policyImportExtensions)
        {
            Dictionary<string, byte[]> serviceReferenceExtensionFileContents = null;
            foreach (IWsdlImportExtension extension in wsdlImportExtensions)
            {
                IWcfReferenceReceiveContextInformation information = extension as IWcfReferenceReceiveContextInformation;
                if (information != null)
                {
                    if (serviceReferenceExtensionFileContents == null)
                    {
                        serviceReferenceExtensionFileContents = CreateDictionaryOfCopiedExtensionFiles(svcMapFile);
                    }
                    information.ReceiveImportContextInformation(serviceReferenceExtensionFileContents, serviceProviderForImportExtensions);
                }
            }
            foreach (IPolicyImportExtension extension2 in policyImportExtensions)
            {
                IWcfReferenceReceiveContextInformation information2 = extension2 as IWcfReferenceReceiveContextInformation;
                if (information2 != null)
                {
                    if (serviceReferenceExtensionFileContents == null)
                    {
                        serviceReferenceExtensionFileContents = CreateDictionaryOfCopiedExtensionFiles(svcMapFile);
                    }
                    information2.ReceiveImportContextInformation(serviceReferenceExtensionFileContents, serviceProviderForImportExtensions);
                }
            }
        }

        private static void RemoveDuplicatedSchemaItems(List<MetadataSection> metadataCollection, IList<ProxyGenerationError> importErrors)
        {
            IEnumerable<System.Xml.Schema.XmlSchema> enumerable;
            Dictionary<System.Xml.Schema.XmlSchema, MetadataSection> dictionary = new Dictionary<System.Xml.Schema.XmlSchema, MetadataSection>();
            foreach (MetadataSection section in metadataCollection)
            {
                if (section.Dialect == MetadataSection.XmlSchemaDialect)
                {
                    System.Xml.Schema.XmlSchema metadata = (System.Xml.Schema.XmlSchema) section.Metadata;
                    dictionary.Add(metadata, section);
                }
            }
            foreach (MetadataSection section2 in metadataCollection)
            {
                if (section2.Dialect == MetadataSection.ServiceDescriptionDialect)
                {
                    System.Web.Services.Description.ServiceDescription description = (System.Web.Services.Description.ServiceDescription) section2.Metadata;
                    foreach (System.Xml.Schema.XmlSchema schema2 in description.Types.Schemas)
                    {
                        schema2.SourceUri = description.RetrievalUrl;
                        dictionary.Add(schema2, section2);
                    }
                }
            }
            SchemaMerger.MergeSchemas(dictionary.Keys, importErrors, out enumerable);
            if (enumerable != null)
            {
                foreach (System.Xml.Schema.XmlSchema schema3 in enumerable)
                {
                    MetadataSection item = dictionary[schema3];
                    if (item.Dialect == MetadataSection.XmlSchemaDialect)
                    {
                        metadataCollection.Remove(item);
                    }
                    else if (item.Dialect == MetadataSection.ServiceDescriptionDialect)
                    {
                        System.Web.Services.Description.ServiceDescription description2 = (System.Web.Services.Description.ServiceDescription) item.Metadata;
                        description2.Types.Schemas.Remove(schema3);
                    }
                }
            }
        }

        private static void RemoveExtension(Type extensionType, Collection<IWsdlImportExtension> wsdlImportExtensions)
        {
            for (int i = 0; i < wsdlImportExtensions.Count; i++)
            {
                if (wsdlImportExtensions[i].GetType() == extensionType)
                {
                    wsdlImportExtensions.RemoveAt(i);
                }
            }
        }

        private static string ReplaceNamespace(string originalNamespace, string replacementNamespace, string typeName)
        {
            if (typeName.StartsWith(originalNamespace, StringComparison.Ordinal))
            {
                return (replacementNamespace + typeName.Substring(originalNamespace.Length));
            }
            return typeName;
        }

        public IEnumerable<System.ServiceModel.Channels.Binding> BindingCollection =>
            this.bindingCollection;

        public IEnumerable<ContractDescription> ContractCollection =>
            this.contractCollection;

        public IEnumerable<ServiceEndpoint> EndpointCollection =>
            this.serviceEndpointList;

        public Dictionary<ServiceEndpoint, ChannelEndpointElement> EndpointMap =>
            this.serviceEndpointToChannelEndpointElementMap;

        public IEnumerable<ProxyGenerationError> ImportErrors =>
            this.importErrors;

        private static CodeAttributeDeclaration OutAttribute
        {
            get
            {
                if (outAttribute == null)
                {
                    outAttribute = new CodeAttributeDeclaration(typeof(System.Runtime.InteropServices.OutAttribute).FullName);
                }
                return outAttribute;
            }
        }

        public IEnumerable<GeneratedContractType> ProxyGeneratedContractTypes =>
            this.proxyGeneratedContractTypes;

        public IEnumerable<ProxyGenerationError> ProxyGenerationErrors =>
            this.proxyGenerationErrors;

        public CodeCompileUnit TargetCompileUnit =>
            this.targetCompileUnit;

        public System.Configuration.Configuration TargetConfiguration =>
            this.targetConfiguration;
    }
}

