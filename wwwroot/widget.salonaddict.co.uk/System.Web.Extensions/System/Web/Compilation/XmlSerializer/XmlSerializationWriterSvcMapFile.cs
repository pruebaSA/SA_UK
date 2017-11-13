namespace System.Web.Compilation.XmlSerializer
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Web.Compilation.WCFModel;
    using System.Xml;
    using System.Xml.Serialization;

    internal class XmlSerializationWriterSvcMapFile : XmlSerializationWriter
    {
        protected override void InitCallbacks()
        {
        }

        private void Write11_ExtensionFile(string n, string ns, ExtensionFile o, bool isNullable, bool needType)
        {
            if (o == null)
            {
                if (isNullable)
                {
                    base.WriteNullTagLiteral(n, ns);
                }
            }
            else
            {
                if (!needType && (o.GetType() != typeof(ExtensionFile)))
                {
                    throw base.CreateUnknownTypeException(o);
                }
                base.WriteStartElement(n, ns, o, false, null);
                if (needType)
                {
                    base.WriteXsiType("ExtensionFile", "urn:schemas-microsoft-com:xml-wcfservicemap");
                }
                base.WriteAttribute("FileName", "", o.FileName);
                base.WriteAttribute("Name", "", o.Name);
                base.WriteEndElement(o);
            }
        }

        private string Write12_MetadataType(MetadataFile.MetadataType v)
        {
            switch (v)
            {
                case MetadataFile.MetadataType.Unknown:
                    return "Unknown";

                case MetadataFile.MetadataType.Disco:
                    return "Disco";

                case MetadataFile.MetadataType.Wsdl:
                    return "Wsdl";

                case MetadataFile.MetadataType.Schema:
                    return "Schema";

                case MetadataFile.MetadataType.Policy:
                    return "Policy";

                case MetadataFile.MetadataType.Xml:
                    return "Xml";
            }
            long num = (long) v;
            throw base.CreateInvalidEnumValueException(num.ToString(CultureInfo.InvariantCulture), "System.Web.Compilation.WCFModel.MetadataFile.MetadataType");
        }

        private void Write13_MetadataFile(string n, string ns, MetadataFile o, bool isNullable, bool needType)
        {
            if (o == null)
            {
                if (isNullable)
                {
                    base.WriteNullTagLiteral(n, ns);
                }
            }
            else
            {
                if (!needType && (o.GetType() != typeof(MetadataFile)))
                {
                    throw base.CreateUnknownTypeException(o);
                }
                base.WriteStartElement(n, ns, o, false, null);
                if (needType)
                {
                    base.WriteXsiType("MetadataFile", "urn:schemas-microsoft-com:xml-wcfservicemap");
                }
                base.WriteAttribute("FileName", "", o.FileName);
                base.WriteAttribute("MetadataType", "", this.Write12_MetadataType(o.FileType));
                base.WriteAttribute("ID", "", o.ID);
                if (o.IgnoreSpecified)
                {
                    base.WriteAttribute("Ignore", "", XmlConvert.ToString(o.Ignore));
                }
                if (o.IsMergeResultSpecified)
                {
                    base.WriteAttribute("IsMergeResult", "", XmlConvert.ToString(o.IsMergeResult));
                }
                if (o.SourceIdSpecified)
                {
                    base.WriteAttribute("SourceId", "", XmlConvert.ToString(o.SourceId));
                }
                base.WriteAttribute("SourceUrl", "", o.SourceUrl);
                bool ignoreSpecified = o.IgnoreSpecified;
                bool isMergeResultSpecified = o.IsMergeResultSpecified;
                bool sourceIdSpecified = o.SourceIdSpecified;
                base.WriteEndElement(o);
            }
        }

        private void Write14_MetadataSource(string n, string ns, MetadataSource o, bool isNullable, bool needType)
        {
            if (o == null)
            {
                if (isNullable)
                {
                    base.WriteNullTagLiteral(n, ns);
                }
            }
            else
            {
                if (!needType && (o.GetType() != typeof(MetadataSource)))
                {
                    throw base.CreateUnknownTypeException(o);
                }
                base.WriteStartElement(n, ns, o, false, null);
                if (needType)
                {
                    base.WriteXsiType("MetadataSource", "urn:schemas-microsoft-com:xml-wcfservicemap");
                }
                base.WriteAttribute("Address", "", o.Address);
                base.WriteAttribute("Protocol", "", o.Protocol);
                base.WriteAttribute("SourceId", "", XmlConvert.ToString(o.SourceId));
                base.WriteEndElement(o);
            }
        }

        private void Write15_SvcMapFile(string n, string ns, SvcMapFile o, bool isNullable, bool needType)
        {
            if (o == null)
            {
                if (isNullable)
                {
                    base.WriteNullTagLiteral(n, ns);
                }
            }
            else
            {
                if (!needType && (o.GetType() != typeof(SvcMapFile)))
                {
                    throw base.CreateUnknownTypeException(o);
                }
                base.WriteStartElement(n, ns, o, false, null);
                if (needType)
                {
                    base.WriteXsiType("SvcMapFile", "urn:schemas-microsoft-com:xml-wcfservicemap");
                }
                base.WriteAttribute("ID", "", o.ID);
                this.Write9_ClientOptions("ClientOptions", "urn:schemas-microsoft-com:xml-wcfservicemap", o.ClientOptions, false, false);
                List<MetadataSource> metadataSourceList = o.MetadataSourceList;
                if (metadataSourceList != null)
                {
                    base.WriteStartElement("MetadataSources", "urn:schemas-microsoft-com:xml-wcfservicemap", null, false);
                    for (int i = 0; i < metadataSourceList.Count; i++)
                    {
                        this.Write14_MetadataSource("MetadataSource", "urn:schemas-microsoft-com:xml-wcfservicemap", metadataSourceList[i], true, false);
                    }
                    base.WriteEndElement();
                }
                List<MetadataFile> metadataList = o.MetadataList;
                if (metadataList != null)
                {
                    base.WriteStartElement("Metadata", "urn:schemas-microsoft-com:xml-wcfservicemap", null, false);
                    for (int j = 0; j < metadataList.Count; j++)
                    {
                        this.Write13_MetadataFile("MetadataFile", "urn:schemas-microsoft-com:xml-wcfservicemap", metadataList[j], true, false);
                    }
                    base.WriteEndElement();
                }
                List<ExtensionFile> extensions = o.Extensions;
                if (extensions != null)
                {
                    base.WriteStartElement("Extensions", "urn:schemas-microsoft-com:xml-wcfservicemap", null, false);
                    for (int k = 0; k < extensions.Count; k++)
                    {
                        this.Write11_ExtensionFile("ExtensionFile", "urn:schemas-microsoft-com:xml-wcfservicemap", extensions[k], true, false);
                    }
                    base.WriteEndElement();
                }
                base.WriteEndElement(o);
            }
        }

        public void Write16_ReferenceGroup(object o)
        {
            base.WriteStartDocument();
            if (o == null)
            {
                base.WriteNullTagLiteral("ReferenceGroup", "urn:schemas-microsoft-com:xml-wcfservicemap");
            }
            else
            {
                base.TopLevelElement();
                this.Write15_SvcMapFile("ReferenceGroup", "urn:schemas-microsoft-com:xml-wcfservicemap", (SvcMapFile) o, true, false);
            }
        }

        private void Write2_ReferencedType(string n, string ns, ReferencedType o, bool isNullable, bool needType)
        {
            if (o == null)
            {
                if (isNullable)
                {
                    base.WriteNullTagLiteral(n, ns);
                }
            }
            else
            {
                if (!needType && (o.GetType() != typeof(ReferencedType)))
                {
                    throw base.CreateUnknownTypeException(o);
                }
                base.WriteStartElement(n, ns, o, false, null);
                if (needType)
                {
                    base.WriteXsiType("ReferencedType", "urn:schemas-microsoft-com:xml-wcfservicemap");
                }
                base.WriteAttribute("TypeName", "", o.TypeName);
                base.WriteEndElement(o);
            }
        }

        private void Write3_NamespaceMapping(string n, string ns, NamespaceMapping o, bool isNullable, bool needType)
        {
            if (o == null)
            {
                if (isNullable)
                {
                    base.WriteNullTagLiteral(n, ns);
                }
            }
            else
            {
                if (!needType && (o.GetType() != typeof(NamespaceMapping)))
                {
                    throw base.CreateUnknownTypeException(o);
                }
                base.WriteStartElement(n, ns, o, false, null);
                if (needType)
                {
                    base.WriteXsiType("NamespaceMapping", "urn:schemas-microsoft-com:xml-wcfservicemap");
                }
                base.WriteAttribute("TargetNamespace", "", o.TargetNamespace);
                base.WriteAttribute("ClrNamespace", "", o.ClrNamespace);
                base.WriteEndElement(o);
            }
        }

        private string Write4_CollectionCategory(ReferencedCollectionType.CollectionCategory v)
        {
            switch (v)
            {
                case ReferencedCollectionType.CollectionCategory.Unknown:
                    return "Unknown";

                case ReferencedCollectionType.CollectionCategory.List:
                    return "List";

                case ReferencedCollectionType.CollectionCategory.Dictionary:
                    return "Dictionary";
            }
            long num = (long) v;
            throw base.CreateInvalidEnumValueException(num.ToString(CultureInfo.InvariantCulture), "System.Web.Compilation.WCFModel.ReferencedCollectionType.CollectionCategory");
        }

        private void Write5_ReferencedCollectionType(string n, string ns, ReferencedCollectionType o, bool isNullable, bool needType)
        {
            if (o == null)
            {
                if (isNullable)
                {
                    base.WriteNullTagLiteral(n, ns);
                }
            }
            else
            {
                if (!needType && (o.GetType() != typeof(ReferencedCollectionType)))
                {
                    throw base.CreateUnknownTypeException(o);
                }
                base.WriteStartElement(n, ns, o, false, null);
                if (needType)
                {
                    base.WriteXsiType("ReferencedCollectionType", "urn:schemas-microsoft-com:xml-wcfservicemap");
                }
                base.WriteAttribute("TypeName", "", o.TypeName);
                base.WriteAttribute("Category", "", this.Write4_CollectionCategory(o.Category));
                base.WriteEndElement(o);
            }
        }

        private string Write6_ProxySerializerType(ClientOptions.ProxySerializerType v)
        {
            switch (v)
            {
                case ClientOptions.ProxySerializerType.Auto:
                    return "Auto";

                case ClientOptions.ProxySerializerType.DataContractSerializer:
                    return "DataContractSerializer";

                case ClientOptions.ProxySerializerType.XmlSerializer:
                    return "XmlSerializer";
            }
            long num = (long) v;
            throw base.CreateInvalidEnumValueException(num.ToString(CultureInfo.InvariantCulture), "System.Web.Compilation.WCFModel.ClientOptions.ProxySerializerType");
        }

        private void Write7_ReferencedAssembly(string n, string ns, ReferencedAssembly o, bool isNullable, bool needType)
        {
            if (o == null)
            {
                if (isNullable)
                {
                    base.WriteNullTagLiteral(n, ns);
                }
            }
            else
            {
                if (!needType && (o.GetType() != typeof(ReferencedAssembly)))
                {
                    throw base.CreateUnknownTypeException(o);
                }
                base.WriteStartElement(n, ns, o, false, null);
                if (needType)
                {
                    base.WriteXsiType("ReferencedAssembly", "urn:schemas-microsoft-com:xml-wcfservicemap");
                }
                base.WriteAttribute("AssemblyName", "", o.AssemblyName);
                base.WriteEndElement(o);
            }
        }

        private void Write8_ContractMapping(string n, string ns, ContractMapping o, bool isNullable, bool needType)
        {
            if (o == null)
            {
                if (isNullable)
                {
                    base.WriteNullTagLiteral(n, ns);
                }
            }
            else
            {
                if (!needType && (o.GetType() != typeof(ContractMapping)))
                {
                    throw base.CreateUnknownTypeException(o);
                }
                base.WriteStartElement(n, ns, o, false, null);
                if (needType)
                {
                    base.WriteXsiType("ContractMapping", "urn:schemas-microsoft-com:xml-wcfservicemap");
                }
                base.WriteAttribute("Name", "", o.Name);
                base.WriteAttribute("TargetNamespace", "", o.TargetNamespace);
                base.WriteAttribute("TypeName", "", o.TypeName);
                base.WriteEndElement(o);
            }
        }

        private void Write9_ClientOptions(string n, string ns, ClientOptions o, bool isNullable, bool needType)
        {
            if (o == null)
            {
                if (isNullable)
                {
                    base.WriteNullTagLiteral(n, ns);
                }
            }
            else
            {
                if (!needType && (o.GetType() != typeof(ClientOptions)))
                {
                    throw base.CreateUnknownTypeException(o);
                }
                base.WriteStartElement(n, ns, o, false, null);
                if (needType)
                {
                    base.WriteXsiType("ClientOptions", "urn:schemas-microsoft-com:xml-wcfservicemap");
                }
                base.WriteElementStringRaw("GenerateAsynchronousMethods", "urn:schemas-microsoft-com:xml-wcfservicemap", XmlConvert.ToString(o.GenerateAsynchronousMethods));
                base.WriteElementStringRaw("EnableDataBinding", "urn:schemas-microsoft-com:xml-wcfservicemap", XmlConvert.ToString(o.EnableDataBinding));
                List<ReferencedType> excludedTypeList = o.ExcludedTypeList;
                if (excludedTypeList != null)
                {
                    base.WriteStartElement("ExcludedTypes", "urn:schemas-microsoft-com:xml-wcfservicemap", null, false);
                    for (int i = 0; i < excludedTypeList.Count; i++)
                    {
                        this.Write2_ReferencedType("ExcludedType", "urn:schemas-microsoft-com:xml-wcfservicemap", excludedTypeList[i], true, false);
                    }
                    base.WriteEndElement();
                }
                base.WriteElementStringRaw("ImportXmlTypes", "urn:schemas-microsoft-com:xml-wcfservicemap", XmlConvert.ToString(o.ImportXmlTypes));
                base.WriteElementStringRaw("GenerateInternalTypes", "urn:schemas-microsoft-com:xml-wcfservicemap", XmlConvert.ToString(o.GenerateInternalTypes));
                base.WriteElementStringRaw("GenerateMessageContracts", "urn:schemas-microsoft-com:xml-wcfservicemap", XmlConvert.ToString(o.GenerateMessageContracts));
                List<NamespaceMapping> namespaceMappingList = o.NamespaceMappingList;
                if (namespaceMappingList != null)
                {
                    base.WriteStartElement("NamespaceMappings", "urn:schemas-microsoft-com:xml-wcfservicemap", null, false);
                    for (int j = 0; j < namespaceMappingList.Count; j++)
                    {
                        this.Write3_NamespaceMapping("NamespaceMapping", "urn:schemas-microsoft-com:xml-wcfservicemap", namespaceMappingList[j], true, false);
                    }
                    base.WriteEndElement();
                }
                List<ReferencedCollectionType> collectionMappingList = o.CollectionMappingList;
                if (collectionMappingList != null)
                {
                    base.WriteStartElement("CollectionMappings", "urn:schemas-microsoft-com:xml-wcfservicemap", null, false);
                    for (int k = 0; k < collectionMappingList.Count; k++)
                    {
                        this.Write5_ReferencedCollectionType("CollectionMapping", "urn:schemas-microsoft-com:xml-wcfservicemap", collectionMappingList[k], true, false);
                    }
                    base.WriteEndElement();
                }
                base.WriteElementStringRaw("GenerateSerializableTypes", "urn:schemas-microsoft-com:xml-wcfservicemap", XmlConvert.ToString(o.GenerateSerializableTypes));
                base.WriteElementString("Serializer", "urn:schemas-microsoft-com:xml-wcfservicemap", this.Write6_ProxySerializerType(o.Serializer));
                if (o.UseSerializerForFaultsSpecified)
                {
                    base.WriteElementStringRaw("UseSerializerForFaults", "urn:schemas-microsoft-com:xml-wcfservicemap", XmlConvert.ToString(o.UseSerializerForFaults));
                }
                if (o.WrappedSpecified)
                {
                    base.WriteElementStringRaw("Wrapped", "urn:schemas-microsoft-com:xml-wcfservicemap", XmlConvert.ToString(o.Wrapped));
                }
                base.WriteElementStringRaw("ReferenceAllAssemblies", "urn:schemas-microsoft-com:xml-wcfservicemap", XmlConvert.ToString(o.ReferenceAllAssemblies));
                List<ReferencedAssembly> referencedAssemblyList = o.ReferencedAssemblyList;
                if (referencedAssemblyList != null)
                {
                    base.WriteStartElement("ReferencedAssemblies", "urn:schemas-microsoft-com:xml-wcfservicemap", null, false);
                    for (int m = 0; m < referencedAssemblyList.Count; m++)
                    {
                        this.Write7_ReferencedAssembly("ReferencedAssembly", "urn:schemas-microsoft-com:xml-wcfservicemap", referencedAssemblyList[m], true, false);
                    }
                    base.WriteEndElement();
                }
                List<ReferencedType> referencedDataContractTypeList = o.ReferencedDataContractTypeList;
                if (referencedDataContractTypeList != null)
                {
                    base.WriteStartElement("ReferencedDataContractTypes", "urn:schemas-microsoft-com:xml-wcfservicemap", null, false);
                    for (int num5 = 0; num5 < referencedDataContractTypeList.Count; num5++)
                    {
                        this.Write2_ReferencedType("ReferencedDataContractType", "urn:schemas-microsoft-com:xml-wcfservicemap", referencedDataContractTypeList[num5], true, false);
                    }
                    base.WriteEndElement();
                }
                List<ContractMapping> serviceContractMappingList = o.ServiceContractMappingList;
                if (serviceContractMappingList != null)
                {
                    base.WriteStartElement("ServiceContractMappings", "urn:schemas-microsoft-com:xml-wcfservicemap", null, false);
                    for (int num6 = 0; num6 < serviceContractMappingList.Count; num6++)
                    {
                        this.Write8_ContractMapping("ServiceContractMapping", "urn:schemas-microsoft-com:xml-wcfservicemap", serviceContractMappingList[num6], true, false);
                    }
                    base.WriteEndElement();
                }
                base.WriteEndElement(o);
            }
        }
    }
}

