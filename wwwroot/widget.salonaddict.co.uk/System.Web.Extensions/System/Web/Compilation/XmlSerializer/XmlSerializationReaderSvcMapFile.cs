namespace System.Web.Compilation.XmlSerializer
{
    using System;
    using System.Collections.Generic;
    using System.Web.Compilation.WCFModel;
    using System.Xml;
    using System.Xml.Serialization;

    internal class XmlSerializationReaderSvcMapFile : XmlSerializationReader
    {
        private string id1_ReferenceGroup;
        private string id10_MetadataFile;
        private string id11_Extensions;
        private string id12_ExtensionFile;
        private string id13_FileName;
        private string id14_Name;
        private string id15_MetadataType;
        private string id16_Ignore;
        private string id17_IsMergeResult;
        private string id18_SourceId;
        private string id19_SourceUrl;
        private string id2_Item;
        private string id20_Address;
        private string id21_Protocol;
        private string id22_GenerateAsynchronousMethods;
        private string id23_EnableDataBinding;
        private string id24_ExcludedTypes;
        private string id25_ExcludedType;
        private string id26_ImportXmlTypes;
        private string id27_GenerateInternalTypes;
        private string id28_GenerateMessageContracts;
        private string id29_NamespaceMappings;
        private string id3_SvcMapFile;
        private string id30_NamespaceMapping;
        private string id31_CollectionMappings;
        private string id32_CollectionMapping;
        private string id33_GenerateSerializableTypes;
        private string id34_Serializer;
        private string id35_UseSerializerForFaults;
        private string id36_Wrapped;
        private string id37_ReferenceAllAssemblies;
        private string id38_ReferencedAssemblies;
        private string id39_ReferencedAssembly;
        private string id4_ID;
        private string id40_ReferencedDataContractTypes;
        private string id41_ReferencedDataContractType;
        private string id42_ServiceContractMappings;
        private string id43_ServiceContractMapping;
        private string id44_ContractMapping;
        private string id45_TargetNamespace;
        private string id46_TypeName;
        private string id47_ReferencedType;
        private string id48_AssemblyName;
        private string id49_ReferencedCollectionType;
        private string id5_Item;
        private string id50_Category;
        private string id51_ClrNamespace;
        private string id6_ClientOptions;
        private string id7_MetadataSources;
        private string id8_MetadataSource;
        private string id9_Metadata;

        protected override void InitCallbacks()
        {
        }

        protected override void InitIDs()
        {
            this.id4_ID = base.Reader.NameTable.Add("ID");
            this.id11_Extensions = base.Reader.NameTable.Add("Extensions");
            this.id31_CollectionMappings = base.Reader.NameTable.Add("CollectionMappings");
            this.id5_Item = base.Reader.NameTable.Add("");
            this.id39_ReferencedAssembly = base.Reader.NameTable.Add("ReferencedAssembly");
            this.id50_Category = base.Reader.NameTable.Add("Category");
            this.id42_ServiceContractMappings = base.Reader.NameTable.Add("ServiceContractMappings");
            this.id16_Ignore = base.Reader.NameTable.Add("Ignore");
            this.id48_AssemblyName = base.Reader.NameTable.Add("AssemblyName");
            this.id10_MetadataFile = base.Reader.NameTable.Add("MetadataFile");
            this.id2_Item = base.Reader.NameTable.Add("urn:schemas-microsoft-com:xml-wcfservicemap");
            this.id44_ContractMapping = base.Reader.NameTable.Add("ContractMapping");
            this.id38_ReferencedAssemblies = base.Reader.NameTable.Add("ReferencedAssemblies");
            this.id33_GenerateSerializableTypes = base.Reader.NameTable.Add("GenerateSerializableTypes");
            this.id25_ExcludedType = base.Reader.NameTable.Add("ExcludedType");
            this.id27_GenerateInternalTypes = base.Reader.NameTable.Add("GenerateInternalTypes");
            this.id45_TargetNamespace = base.Reader.NameTable.Add("TargetNamespace");
            this.id46_TypeName = base.Reader.NameTable.Add("TypeName");
            this.id19_SourceUrl = base.Reader.NameTable.Add("SourceUrl");
            this.id51_ClrNamespace = base.Reader.NameTable.Add("ClrNamespace");
            this.id43_ServiceContractMapping = base.Reader.NameTable.Add("ServiceContractMapping");
            this.id40_ReferencedDataContractTypes = base.Reader.NameTable.Add("ReferencedDataContractTypes");
            this.id21_Protocol = base.Reader.NameTable.Add("Protocol");
            this.id14_Name = base.Reader.NameTable.Add("Name");
            this.id3_SvcMapFile = base.Reader.NameTable.Add("SvcMapFile");
            this.id41_ReferencedDataContractType = base.Reader.NameTable.Add("ReferencedDataContractType");
            this.id18_SourceId = base.Reader.NameTable.Add("SourceId");
            this.id1_ReferenceGroup = base.Reader.NameTable.Add("ReferenceGroup");
            this.id36_Wrapped = base.Reader.NameTable.Add("Wrapped");
            this.id28_GenerateMessageContracts = base.Reader.NameTable.Add("GenerateMessageContracts");
            this.id13_FileName = base.Reader.NameTable.Add("FileName");
            this.id6_ClientOptions = base.Reader.NameTable.Add("ClientOptions");
            this.id47_ReferencedType = base.Reader.NameTable.Add("ReferencedType");
            this.id30_NamespaceMapping = base.Reader.NameTable.Add("NamespaceMapping");
            this.id32_CollectionMapping = base.Reader.NameTable.Add("CollectionMapping");
            this.id23_EnableDataBinding = base.Reader.NameTable.Add("EnableDataBinding");
            this.id7_MetadataSources = base.Reader.NameTable.Add("MetadataSources");
            this.id9_Metadata = base.Reader.NameTable.Add("Metadata");
            this.id15_MetadataType = base.Reader.NameTable.Add("MetadataType");
            this.id8_MetadataSource = base.Reader.NameTable.Add("MetadataSource");
            this.id49_ReferencedCollectionType = base.Reader.NameTable.Add("ReferencedCollectionType");
            this.id12_ExtensionFile = base.Reader.NameTable.Add("ExtensionFile");
            this.id17_IsMergeResult = base.Reader.NameTable.Add("IsMergeResult");
            this.id26_ImportXmlTypes = base.Reader.NameTable.Add("ImportXmlTypes");
            this.id24_ExcludedTypes = base.Reader.NameTable.Add("ExcludedTypes");
            this.id29_NamespaceMappings = base.Reader.NameTable.Add("NamespaceMappings");
            this.id34_Serializer = base.Reader.NameTable.Add("Serializer");
            this.id22_GenerateAsynchronousMethods = base.Reader.NameTable.Add("GenerateAsynchronousMethods");
            this.id20_Address = base.Reader.NameTable.Add("Address");
            this.id37_ReferenceAllAssemblies = base.Reader.NameTable.Add("ReferenceAllAssemblies");
            this.id35_UseSerializerForFaults = base.Reader.NameTable.Add("UseSerializerForFaults");
        }

        private ExtensionFile Read11_ExtensionFile(bool isNullable, bool checkType)
        {
            XmlQualifiedName type = checkType ? base.GetXsiType() : null;
            bool flag = false;
            if (isNullable)
            {
                flag = base.ReadNull();
            }
            if ((checkType && (type != null)) && ((type.Name != this.id12_ExtensionFile) || (type.Namespace != this.id2_Item)))
            {
                throw base.CreateUnknownTypeException(type);
            }
            if (flag)
            {
                return null;
            }
            ExtensionFile o = new ExtensionFile();
            bool[] flagArray = new bool[2];
            while (base.Reader.MoveToNextAttribute())
            {
                if ((!flagArray[0] && (base.Reader.LocalName == this.id13_FileName)) && (base.Reader.NamespaceURI == this.id5_Item))
                {
                    o.FileName = base.Reader.Value;
                    flagArray[0] = true;
                }
                else
                {
                    if ((!flagArray[1] && (base.Reader.LocalName == this.id14_Name)) && (base.Reader.NamespaceURI == this.id5_Item))
                    {
                        o.Name = base.Reader.Value;
                        flagArray[1] = true;
                        continue;
                    }
                    if (!base.IsXmlnsAttribute(base.Reader.Name))
                    {
                        base.UnknownNode(o, ":FileName, :Name");
                    }
                }
            }
            base.Reader.MoveToElement();
            if (base.Reader.IsEmptyElement)
            {
                base.Reader.Skip();
                return o;
            }
            base.Reader.ReadStartElement();
            base.Reader.MoveToContent();
            int whileIterations = 0;
            int readerCount = base.ReaderCount;
            while ((base.Reader.NodeType != XmlNodeType.EndElement) && (base.Reader.NodeType != XmlNodeType.None))
            {
                if (base.Reader.NodeType == XmlNodeType.Element)
                {
                    base.UnknownNode(o, "");
                }
                else
                {
                    base.UnknownNode(o, "");
                }
                base.Reader.MoveToContent();
                base.CheckReaderCount(ref whileIterations, ref readerCount);
            }
            base.ReadEndElement();
            return o;
        }

        private MetadataFile.MetadataType Read12_MetadataType(string s)
        {
            switch (s)
            {
                case "Unknown":
                    return MetadataFile.MetadataType.Unknown;

                case "Disco":
                    return MetadataFile.MetadataType.Disco;

                case "Wsdl":
                    return MetadataFile.MetadataType.Wsdl;

                case "Schema":
                    return MetadataFile.MetadataType.Schema;

                case "Policy":
                    return MetadataFile.MetadataType.Policy;

                case "Xml":
                    return MetadataFile.MetadataType.Xml;
            }
            throw base.CreateUnknownConstantException(s, typeof(MetadataFile.MetadataType));
        }

        private MetadataFile Read13_MetadataFile(bool isNullable, bool checkType)
        {
            XmlQualifiedName type = checkType ? base.GetXsiType() : null;
            bool flag = false;
            if (isNullable)
            {
                flag = base.ReadNull();
            }
            if ((checkType && (type != null)) && ((type.Name != this.id10_MetadataFile) || (type.Namespace != this.id2_Item)))
            {
                throw base.CreateUnknownTypeException(type);
            }
            if (flag)
            {
                return null;
            }
            MetadataFile o = new MetadataFile();
            bool[] flagArray = new bool[7];
            while (base.Reader.MoveToNextAttribute())
            {
                if ((!flagArray[0] && (base.Reader.LocalName == this.id13_FileName)) && (base.Reader.NamespaceURI == this.id5_Item))
                {
                    o.FileName = base.Reader.Value;
                    flagArray[0] = true;
                }
                else
                {
                    if ((!flagArray[1] && (base.Reader.LocalName == this.id15_MetadataType)) && (base.Reader.NamespaceURI == this.id5_Item))
                    {
                        o.FileType = this.Read12_MetadataType(base.Reader.Value);
                        flagArray[1] = true;
                        continue;
                    }
                    if ((!flagArray[2] && (base.Reader.LocalName == this.id4_ID)) && (base.Reader.NamespaceURI == this.id5_Item))
                    {
                        o.ID = base.Reader.Value;
                        flagArray[2] = true;
                        continue;
                    }
                    if ((!flagArray[3] && (base.Reader.LocalName == this.id16_Ignore)) && (base.Reader.NamespaceURI == this.id5_Item))
                    {
                        o.Ignore = XmlConvert.ToBoolean(base.Reader.Value);
                        o.IgnoreSpecified = true;
                        flagArray[3] = true;
                        continue;
                    }
                    if ((!flagArray[4] && (base.Reader.LocalName == this.id17_IsMergeResult)) && (base.Reader.NamespaceURI == this.id5_Item))
                    {
                        o.IsMergeResult = XmlConvert.ToBoolean(base.Reader.Value);
                        o.IsMergeResultSpecified = true;
                        flagArray[4] = true;
                        continue;
                    }
                    if ((!flagArray[5] && (base.Reader.LocalName == this.id18_SourceId)) && (base.Reader.NamespaceURI == this.id5_Item))
                    {
                        o.SourceId = XmlConvert.ToInt32(base.Reader.Value);
                        o.SourceIdSpecified = true;
                        flagArray[5] = true;
                        continue;
                    }
                    if ((!flagArray[6] && (base.Reader.LocalName == this.id19_SourceUrl)) && (base.Reader.NamespaceURI == this.id5_Item))
                    {
                        o.SourceUrl = base.Reader.Value;
                        flagArray[6] = true;
                        continue;
                    }
                    if (!base.IsXmlnsAttribute(base.Reader.Name))
                    {
                        base.UnknownNode(o, ":FileName, :MetadataType, :ID, :Ignore, :IsMergeResult, :SourceId, :SourceUrl");
                    }
                }
            }
            base.Reader.MoveToElement();
            if (base.Reader.IsEmptyElement)
            {
                base.Reader.Skip();
                return o;
            }
            base.Reader.ReadStartElement();
            base.Reader.MoveToContent();
            int whileIterations = 0;
            int readerCount = base.ReaderCount;
            while ((base.Reader.NodeType != XmlNodeType.EndElement) && (base.Reader.NodeType != XmlNodeType.None))
            {
                if (base.Reader.NodeType == XmlNodeType.Element)
                {
                    base.UnknownNode(o, "");
                }
                else
                {
                    base.UnknownNode(o, "");
                }
                base.Reader.MoveToContent();
                base.CheckReaderCount(ref whileIterations, ref readerCount);
            }
            base.ReadEndElement();
            return o;
        }

        private MetadataSource Read14_MetadataSource(bool isNullable, bool checkType)
        {
            XmlQualifiedName type = checkType ? base.GetXsiType() : null;
            bool flag = false;
            if (isNullable)
            {
                flag = base.ReadNull();
            }
            if ((checkType && (type != null)) && ((type.Name != this.id8_MetadataSource) || (type.Namespace != this.id2_Item)))
            {
                throw base.CreateUnknownTypeException(type);
            }
            if (flag)
            {
                return null;
            }
            MetadataSource o = new MetadataSource();
            bool[] flagArray = new bool[3];
            while (base.Reader.MoveToNextAttribute())
            {
                if ((!flagArray[0] && (base.Reader.LocalName == this.id20_Address)) && (base.Reader.NamespaceURI == this.id5_Item))
                {
                    o.Address = base.Reader.Value;
                    flagArray[0] = true;
                }
                else
                {
                    if ((!flagArray[1] && (base.Reader.LocalName == this.id21_Protocol)) && (base.Reader.NamespaceURI == this.id5_Item))
                    {
                        o.Protocol = base.Reader.Value;
                        flagArray[1] = true;
                        continue;
                    }
                    if ((!flagArray[2] && (base.Reader.LocalName == this.id18_SourceId)) && (base.Reader.NamespaceURI == this.id5_Item))
                    {
                        o.SourceId = XmlConvert.ToInt32(base.Reader.Value);
                        flagArray[2] = true;
                        continue;
                    }
                    if (!base.IsXmlnsAttribute(base.Reader.Name))
                    {
                        base.UnknownNode(o, ":Address, :Protocol, :SourceId");
                    }
                }
            }
            base.Reader.MoveToElement();
            if (base.Reader.IsEmptyElement)
            {
                base.Reader.Skip();
                return o;
            }
            base.Reader.ReadStartElement();
            base.Reader.MoveToContent();
            int whileIterations = 0;
            int readerCount = base.ReaderCount;
            while ((base.Reader.NodeType != XmlNodeType.EndElement) && (base.Reader.NodeType != XmlNodeType.None))
            {
                if (base.Reader.NodeType == XmlNodeType.Element)
                {
                    base.UnknownNode(o, "");
                }
                else
                {
                    base.UnknownNode(o, "");
                }
                base.Reader.MoveToContent();
                base.CheckReaderCount(ref whileIterations, ref readerCount);
            }
            base.ReadEndElement();
            return o;
        }

        private SvcMapFile Read15_SvcMapFile(bool isNullable, bool checkType)
        {
            XmlQualifiedName type = checkType ? base.GetXsiType() : null;
            bool flag = false;
            if (isNullable)
            {
                flag = base.ReadNull();
            }
            if ((checkType && (type != null)) && ((type.Name != this.id3_SvcMapFile) || (type.Namespace != this.id2_Item)))
            {
                throw base.CreateUnknownTypeException(type);
            }
            if (flag)
            {
                return null;
            }
            SvcMapFile o = new SvcMapFile();
            List<MetadataSource> metadataSourceList = o.MetadataSourceList;
            List<MetadataFile> metadataList = o.MetadataList;
            List<ExtensionFile> extensions = o.Extensions;
            bool[] flagArray = new bool[5];
            while (base.Reader.MoveToNextAttribute())
            {
                if ((!flagArray[4] && (base.Reader.LocalName == this.id4_ID)) && (base.Reader.NamespaceURI == this.id5_Item))
                {
                    o.ID = base.Reader.Value;
                    flagArray[4] = true;
                }
                else if (!base.IsXmlnsAttribute(base.Reader.Name))
                {
                    base.UnknownNode(o, ":ID");
                }
            }
            base.Reader.MoveToElement();
            if (base.Reader.IsEmptyElement)
            {
                base.Reader.Skip();
                return o;
            }
            base.Reader.ReadStartElement();
            int num = 0;
            base.Reader.MoveToContent();
            int whileIterations = 0;
            int readerCount = base.ReaderCount;
            while ((base.Reader.NodeType != XmlNodeType.EndElement) && (base.Reader.NodeType != XmlNodeType.None))
            {
                List<MetadataSource> list;
                List<MetadataFile> list2;
                List<ExtensionFile> list3;
                if (base.Reader.NodeType != XmlNodeType.Element)
                {
                    goto Label_055E;
                }
                switch (num)
                {
                    case 0:
                        if ((base.Reader.LocalName == this.id6_ClientOptions) && (base.Reader.NamespaceURI == this.id2_Item))
                        {
                            o.ClientOptions = this.Read9_ClientOptions(false, true);
                        }
                        num = 1;
                        goto Label_0566;

                    case 1:
                        if ((base.Reader.LocalName != this.id7_MetadataSources) || (base.Reader.NamespaceURI != this.id2_Item))
                        {
                            goto Label_02D4;
                        }
                        if (!base.ReadNull())
                        {
                            list = o.MetadataSourceList;
                            if ((list != null) && !base.Reader.IsEmptyElement)
                            {
                                break;
                            }
                            base.Reader.Skip();
                        }
                        goto Label_0566;

                    case 2:
                        if ((base.Reader.LocalName != this.id9_Metadata) || (base.Reader.NamespaceURI != this.id2_Item))
                        {
                            goto Label_0413;
                        }
                        if (!base.ReadNull())
                        {
                            list2 = o.MetadataList;
                            if ((list2 != null) && !base.Reader.IsEmptyElement)
                            {
                                goto Label_033C;
                            }
                            base.Reader.Skip();
                        }
                        goto Label_0566;

                    case 3:
                        if ((base.Reader.LocalName != this.id11_Extensions) || (base.Reader.NamespaceURI != this.id2_Item))
                        {
                            goto Label_054F;
                        }
                        if (!base.ReadNull())
                        {
                            list3 = o.Extensions;
                            if ((list3 != null) && !base.Reader.IsEmptyElement)
                            {
                                goto Label_047B;
                            }
                            base.Reader.Skip();
                        }
                        goto Label_0566;

                    default:
                        base.UnknownNode(o, null);
                        goto Label_0566;
                }
                base.Reader.ReadStartElement();
                base.Reader.MoveToContent();
                int num4 = 0;
                int num5 = base.ReaderCount;
                while ((base.Reader.NodeType != XmlNodeType.EndElement) && (base.Reader.NodeType != XmlNodeType.None))
                {
                    if (base.Reader.NodeType == XmlNodeType.Element)
                    {
                        if ((base.Reader.LocalName == this.id8_MetadataSource) && (base.Reader.NamespaceURI == this.id2_Item))
                        {
                            if (list == null)
                            {
                                base.Reader.Skip();
                            }
                            else
                            {
                                list.Add(this.Read14_MetadataSource(true, true));
                            }
                        }
                        else
                        {
                            base.UnknownNode(null, "urn:schemas-microsoft-com:xml-wcfservicemap:MetadataSource");
                        }
                    }
                    else
                    {
                        base.UnknownNode(null, "urn:schemas-microsoft-com:xml-wcfservicemap:MetadataSource");
                    }
                    base.Reader.MoveToContent();
                    base.CheckReaderCount(ref num4, ref num5);
                }
                base.ReadEndElement();
                goto Label_0566;
            Label_02D4:
                num = 2;
                goto Label_0566;
            Label_033C:
                base.Reader.ReadStartElement();
                base.Reader.MoveToContent();
                int num6 = 0;
                int num7 = base.ReaderCount;
                while ((base.Reader.NodeType != XmlNodeType.EndElement) && (base.Reader.NodeType != XmlNodeType.None))
                {
                    if (base.Reader.NodeType == XmlNodeType.Element)
                    {
                        if ((base.Reader.LocalName == this.id10_MetadataFile) && (base.Reader.NamespaceURI == this.id2_Item))
                        {
                            if (list2 == null)
                            {
                                base.Reader.Skip();
                            }
                            else
                            {
                                list2.Add(this.Read13_MetadataFile(true, true));
                            }
                        }
                        else
                        {
                            base.UnknownNode(null, "urn:schemas-microsoft-com:xml-wcfservicemap:MetadataFile");
                        }
                    }
                    else
                    {
                        base.UnknownNode(null, "urn:schemas-microsoft-com:xml-wcfservicemap:MetadataFile");
                    }
                    base.Reader.MoveToContent();
                    base.CheckReaderCount(ref num6, ref num7);
                }
                base.ReadEndElement();
                goto Label_0566;
            Label_0413:
                num = 3;
                goto Label_0566;
            Label_047B:
                base.Reader.ReadStartElement();
                base.Reader.MoveToContent();
                int num8 = 0;
                int num9 = base.ReaderCount;
                while ((base.Reader.NodeType != XmlNodeType.EndElement) && (base.Reader.NodeType != XmlNodeType.None))
                {
                    if (base.Reader.NodeType == XmlNodeType.Element)
                    {
                        if ((base.Reader.LocalName == this.id12_ExtensionFile) && (base.Reader.NamespaceURI == this.id2_Item))
                        {
                            if (list3 == null)
                            {
                                base.Reader.Skip();
                            }
                            else
                            {
                                list3.Add(this.Read11_ExtensionFile(true, true));
                            }
                        }
                        else
                        {
                            base.UnknownNode(null, "urn:schemas-microsoft-com:xml-wcfservicemap:ExtensionFile");
                        }
                    }
                    else
                    {
                        base.UnknownNode(null, "urn:schemas-microsoft-com:xml-wcfservicemap:ExtensionFile");
                    }
                    base.Reader.MoveToContent();
                    base.CheckReaderCount(ref num8, ref num9);
                }
                base.ReadEndElement();
                goto Label_0566;
            Label_054F:
                num = 4;
                goto Label_0566;
            Label_055E:
                base.UnknownNode(o, null);
            Label_0566:
                base.Reader.MoveToContent();
                base.CheckReaderCount(ref whileIterations, ref readerCount);
            }
            base.ReadEndElement();
            return o;
        }

        public object Read16_ReferenceGroup()
        {
            base.Reader.MoveToContent();
            if (base.Reader.NodeType == XmlNodeType.Element)
            {
                if ((base.Reader.LocalName != this.id1_ReferenceGroup) || (base.Reader.NamespaceURI != this.id2_Item))
                {
                    throw base.CreateUnknownNodeException();
                }
                return this.Read15_SvcMapFile(true, true);
            }
            base.UnknownNode(null, "urn:schemas-microsoft-com:xml-wcfservicemap:ReferenceGroup");
            return null;
        }

        private ReferencedType Read2_ReferencedType(bool isNullable, bool checkType)
        {
            XmlQualifiedName name = checkType ? base.GetXsiType() : null;
            bool flag = false;
            if (isNullable)
            {
                flag = base.ReadNull();
            }
            if ((checkType && (name != null)) && ((name.Name != this.id47_ReferencedType) || (name.Namespace != this.id2_Item)))
            {
                throw base.CreateUnknownTypeException(name);
            }
            if (flag)
            {
                return null;
            }
            ReferencedType o = new ReferencedType();
            bool[] flagArray = new bool[1];
            while (base.Reader.MoveToNextAttribute())
            {
                if ((!flagArray[0] && (base.Reader.LocalName == this.id46_TypeName)) && (base.Reader.NamespaceURI == this.id5_Item))
                {
                    o.TypeName = base.Reader.Value;
                    flagArray[0] = true;
                }
                else if (!base.IsXmlnsAttribute(base.Reader.Name))
                {
                    base.UnknownNode(o, ":TypeName");
                }
            }
            base.Reader.MoveToElement();
            if (base.Reader.IsEmptyElement)
            {
                base.Reader.Skip();
                return o;
            }
            base.Reader.ReadStartElement();
            base.Reader.MoveToContent();
            int whileIterations = 0;
            int readerCount = base.ReaderCount;
            while ((base.Reader.NodeType != XmlNodeType.EndElement) && (base.Reader.NodeType != XmlNodeType.None))
            {
                if (base.Reader.NodeType == XmlNodeType.Element)
                {
                    base.UnknownNode(o, "");
                }
                else
                {
                    base.UnknownNode(o, "");
                }
                base.Reader.MoveToContent();
                base.CheckReaderCount(ref whileIterations, ref readerCount);
            }
            base.ReadEndElement();
            return o;
        }

        private NamespaceMapping Read3_NamespaceMapping(bool isNullable, bool checkType)
        {
            XmlQualifiedName type = checkType ? base.GetXsiType() : null;
            bool flag = false;
            if (isNullable)
            {
                flag = base.ReadNull();
            }
            if ((checkType && (type != null)) && ((type.Name != this.id30_NamespaceMapping) || (type.Namespace != this.id2_Item)))
            {
                throw base.CreateUnknownTypeException(type);
            }
            if (flag)
            {
                return null;
            }
            NamespaceMapping o = new NamespaceMapping();
            bool[] flagArray = new bool[2];
            while (base.Reader.MoveToNextAttribute())
            {
                if ((!flagArray[0] && (base.Reader.LocalName == this.id45_TargetNamespace)) && (base.Reader.NamespaceURI == this.id5_Item))
                {
                    o.TargetNamespace = base.Reader.Value;
                    flagArray[0] = true;
                }
                else
                {
                    if ((!flagArray[1] && (base.Reader.LocalName == this.id51_ClrNamespace)) && (base.Reader.NamespaceURI == this.id5_Item))
                    {
                        o.ClrNamespace = base.Reader.Value;
                        flagArray[1] = true;
                        continue;
                    }
                    if (!base.IsXmlnsAttribute(base.Reader.Name))
                    {
                        base.UnknownNode(o, ":TargetNamespace, :ClrNamespace");
                    }
                }
            }
            base.Reader.MoveToElement();
            if (base.Reader.IsEmptyElement)
            {
                base.Reader.Skip();
                return o;
            }
            base.Reader.ReadStartElement();
            base.Reader.MoveToContent();
            int whileIterations = 0;
            int readerCount = base.ReaderCount;
            while ((base.Reader.NodeType != XmlNodeType.EndElement) && (base.Reader.NodeType != XmlNodeType.None))
            {
                if (base.Reader.NodeType == XmlNodeType.Element)
                {
                    base.UnknownNode(o, "");
                }
                else
                {
                    base.UnknownNode(o, "");
                }
                base.Reader.MoveToContent();
                base.CheckReaderCount(ref whileIterations, ref readerCount);
            }
            base.ReadEndElement();
            return o;
        }

        private ReferencedCollectionType.CollectionCategory Read4_CollectionCategory(string s)
        {
            switch (s)
            {
                case "Unknown":
                    return ReferencedCollectionType.CollectionCategory.Unknown;

                case "List":
                    return ReferencedCollectionType.CollectionCategory.List;

                case "Dictionary":
                    return ReferencedCollectionType.CollectionCategory.Dictionary;
            }
            throw base.CreateUnknownConstantException(s, typeof(ReferencedCollectionType.CollectionCategory));
        }

        private ReferencedCollectionType Read5_ReferencedCollectionType(bool isNullable, bool checkType)
        {
            XmlQualifiedName name = checkType ? base.GetXsiType() : null;
            bool flag = false;
            if (isNullable)
            {
                flag = base.ReadNull();
            }
            if ((checkType && (name != null)) && ((name.Name != this.id49_ReferencedCollectionType) || (name.Namespace != this.id2_Item)))
            {
                throw base.CreateUnknownTypeException(name);
            }
            if (flag)
            {
                return null;
            }
            ReferencedCollectionType o = new ReferencedCollectionType();
            bool[] flagArray = new bool[2];
            while (base.Reader.MoveToNextAttribute())
            {
                if ((!flagArray[0] && (base.Reader.LocalName == this.id46_TypeName)) && (base.Reader.NamespaceURI == this.id5_Item))
                {
                    o.TypeName = base.Reader.Value;
                    flagArray[0] = true;
                }
                else
                {
                    if ((!flagArray[1] && (base.Reader.LocalName == this.id50_Category)) && (base.Reader.NamespaceURI == this.id5_Item))
                    {
                        o.Category = this.Read4_CollectionCategory(base.Reader.Value);
                        flagArray[1] = true;
                        continue;
                    }
                    if (!base.IsXmlnsAttribute(base.Reader.Name))
                    {
                        base.UnknownNode(o, ":TypeName, :Category");
                    }
                }
            }
            base.Reader.MoveToElement();
            if (base.Reader.IsEmptyElement)
            {
                base.Reader.Skip();
                return o;
            }
            base.Reader.ReadStartElement();
            base.Reader.MoveToContent();
            int whileIterations = 0;
            int readerCount = base.ReaderCount;
            while ((base.Reader.NodeType != XmlNodeType.EndElement) && (base.Reader.NodeType != XmlNodeType.None))
            {
                if (base.Reader.NodeType == XmlNodeType.Element)
                {
                    base.UnknownNode(o, "");
                }
                else
                {
                    base.UnknownNode(o, "");
                }
                base.Reader.MoveToContent();
                base.CheckReaderCount(ref whileIterations, ref readerCount);
            }
            base.ReadEndElement();
            return o;
        }

        private ClientOptions.ProxySerializerType Read6_ProxySerializerType(string s)
        {
            switch (s)
            {
                case "Auto":
                    return ClientOptions.ProxySerializerType.Auto;

                case "DataContractSerializer":
                    return ClientOptions.ProxySerializerType.DataContractSerializer;

                case "XmlSerializer":
                    return ClientOptions.ProxySerializerType.XmlSerializer;
            }
            throw base.CreateUnknownConstantException(s, typeof(ClientOptions.ProxySerializerType));
        }

        private ReferencedAssembly Read7_ReferencedAssembly(bool isNullable, bool checkType)
        {
            XmlQualifiedName type = checkType ? base.GetXsiType() : null;
            bool flag = false;
            if (isNullable)
            {
                flag = base.ReadNull();
            }
            if ((checkType && (type != null)) && ((type.Name != this.id39_ReferencedAssembly) || (type.Namespace != this.id2_Item)))
            {
                throw base.CreateUnknownTypeException(type);
            }
            if (flag)
            {
                return null;
            }
            ReferencedAssembly o = new ReferencedAssembly();
            bool[] flagArray = new bool[1];
            while (base.Reader.MoveToNextAttribute())
            {
                if ((!flagArray[0] && (base.Reader.LocalName == this.id48_AssemblyName)) && (base.Reader.NamespaceURI == this.id5_Item))
                {
                    o.AssemblyName = base.Reader.Value;
                    flagArray[0] = true;
                }
                else if (!base.IsXmlnsAttribute(base.Reader.Name))
                {
                    base.UnknownNode(o, ":AssemblyName");
                }
            }
            base.Reader.MoveToElement();
            if (base.Reader.IsEmptyElement)
            {
                base.Reader.Skip();
                return o;
            }
            base.Reader.ReadStartElement();
            base.Reader.MoveToContent();
            int whileIterations = 0;
            int readerCount = base.ReaderCount;
            while ((base.Reader.NodeType != XmlNodeType.EndElement) && (base.Reader.NodeType != XmlNodeType.None))
            {
                if (base.Reader.NodeType == XmlNodeType.Element)
                {
                    base.UnknownNode(o, "");
                }
                else
                {
                    base.UnknownNode(o, "");
                }
                base.Reader.MoveToContent();
                base.CheckReaderCount(ref whileIterations, ref readerCount);
            }
            base.ReadEndElement();
            return o;
        }

        private ContractMapping Read8_ContractMapping(bool isNullable, bool checkType)
        {
            XmlQualifiedName type = checkType ? base.GetXsiType() : null;
            bool flag = false;
            if (isNullable)
            {
                flag = base.ReadNull();
            }
            if ((checkType && (type != null)) && ((type.Name != this.id44_ContractMapping) || (type.Namespace != this.id2_Item)))
            {
                throw base.CreateUnknownTypeException(type);
            }
            if (flag)
            {
                return null;
            }
            ContractMapping o = new ContractMapping();
            bool[] flagArray = new bool[3];
            while (base.Reader.MoveToNextAttribute())
            {
                if ((!flagArray[0] && (base.Reader.LocalName == this.id14_Name)) && (base.Reader.NamespaceURI == this.id5_Item))
                {
                    o.Name = base.Reader.Value;
                    flagArray[0] = true;
                }
                else
                {
                    if ((!flagArray[1] && (base.Reader.LocalName == this.id45_TargetNamespace)) && (base.Reader.NamespaceURI == this.id5_Item))
                    {
                        o.TargetNamespace = base.Reader.Value;
                        flagArray[1] = true;
                        continue;
                    }
                    if ((!flagArray[2] && (base.Reader.LocalName == this.id46_TypeName)) && (base.Reader.NamespaceURI == this.id5_Item))
                    {
                        o.TypeName = base.Reader.Value;
                        flagArray[2] = true;
                        continue;
                    }
                    if (!base.IsXmlnsAttribute(base.Reader.Name))
                    {
                        base.UnknownNode(o, ":Name, :TargetNamespace, :TypeName");
                    }
                }
            }
            base.Reader.MoveToElement();
            if (base.Reader.IsEmptyElement)
            {
                base.Reader.Skip();
                return o;
            }
            base.Reader.ReadStartElement();
            base.Reader.MoveToContent();
            int whileIterations = 0;
            int readerCount = base.ReaderCount;
            while ((base.Reader.NodeType != XmlNodeType.EndElement) && (base.Reader.NodeType != XmlNodeType.None))
            {
                if (base.Reader.NodeType == XmlNodeType.Element)
                {
                    base.UnknownNode(o, "");
                }
                else
                {
                    base.UnknownNode(o, "");
                }
                base.Reader.MoveToContent();
                base.CheckReaderCount(ref whileIterations, ref readerCount);
            }
            base.ReadEndElement();
            return o;
        }

        private ClientOptions Read9_ClientOptions(bool isNullable, bool checkType)
        {
            XmlQualifiedName type = checkType ? base.GetXsiType() : null;
            bool flag = false;
            if (isNullable)
            {
                flag = base.ReadNull();
            }
            if ((checkType && (type != null)) && ((type.Name != this.id6_ClientOptions) || (type.Namespace != this.id2_Item)))
            {
                throw base.CreateUnknownTypeException(type);
            }
            if (flag)
            {
                return null;
            }
            ClientOptions o = new ClientOptions();
            List<ReferencedType> excludedTypeList = o.ExcludedTypeList;
            List<NamespaceMapping> namespaceMappingList = o.NamespaceMappingList;
            List<ReferencedCollectionType> collectionMappingList = o.CollectionMappingList;
            List<ReferencedAssembly> referencedAssemblyList = o.ReferencedAssemblyList;
            List<ReferencedType> referencedDataContractTypeList = o.ReferencedDataContractTypeList;
            List<ContractMapping> serviceContractMappingList = o.ServiceContractMappingList;
            bool[] flagArray = new bool[0x10];
            while (base.Reader.MoveToNextAttribute())
            {
                if (!base.IsXmlnsAttribute(base.Reader.Name))
                {
                    base.UnknownNode(o);
                }
            }
            base.Reader.MoveToElement();
            if (base.Reader.IsEmptyElement)
            {
                base.Reader.Skip();
                return o;
            }
            base.Reader.ReadStartElement();
            base.Reader.MoveToContent();
            int whileIterations = 0;
            int readerCount = base.ReaderCount;
            while ((base.Reader.NodeType != XmlNodeType.EndElement) && (base.Reader.NodeType != XmlNodeType.None))
            {
                if (base.Reader.NodeType == XmlNodeType.Element)
                {
                    if ((!flagArray[0] && (base.Reader.LocalName == this.id22_GenerateAsynchronousMethods)) && (base.Reader.NamespaceURI == this.id2_Item))
                    {
                        o.GenerateAsynchronousMethods = XmlConvert.ToBoolean(base.Reader.ReadElementString());
                        flagArray[0] = true;
                    }
                    else if ((!flagArray[1] && (base.Reader.LocalName == this.id23_EnableDataBinding)) && (base.Reader.NamespaceURI == this.id2_Item))
                    {
                        o.EnableDataBinding = XmlConvert.ToBoolean(base.Reader.ReadElementString());
                        flagArray[1] = true;
                    }
                    else if ((base.Reader.LocalName == this.id24_ExcludedTypes) && (base.Reader.NamespaceURI == this.id2_Item))
                    {
                        if (!base.ReadNull())
                        {
                            List<ReferencedType> list = o.ExcludedTypeList;
                            if ((list == null) || base.Reader.IsEmptyElement)
                            {
                                base.Reader.Skip();
                            }
                            else
                            {
                                base.Reader.ReadStartElement();
                                base.Reader.MoveToContent();
                                int num3 = 0;
                                int num4 = base.ReaderCount;
                                while ((base.Reader.NodeType != XmlNodeType.EndElement) && (base.Reader.NodeType != XmlNodeType.None))
                                {
                                    if (base.Reader.NodeType == XmlNodeType.Element)
                                    {
                                        if ((base.Reader.LocalName == this.id25_ExcludedType) && (base.Reader.NamespaceURI == this.id2_Item))
                                        {
                                            if (list == null)
                                            {
                                                base.Reader.Skip();
                                            }
                                            else
                                            {
                                                list.Add(this.Read2_ReferencedType(true, true));
                                            }
                                        }
                                        else
                                        {
                                            base.UnknownNode(null, "urn:schemas-microsoft-com:xml-wcfservicemap:ExcludedType");
                                        }
                                    }
                                    else
                                    {
                                        base.UnknownNode(null, "urn:schemas-microsoft-com:xml-wcfservicemap:ExcludedType");
                                    }
                                    base.Reader.MoveToContent();
                                    base.CheckReaderCount(ref num3, ref num4);
                                }
                                base.ReadEndElement();
                            }
                        }
                    }
                    else if ((!flagArray[3] && (base.Reader.LocalName == this.id26_ImportXmlTypes)) && (base.Reader.NamespaceURI == this.id2_Item))
                    {
                        o.ImportXmlTypes = XmlConvert.ToBoolean(base.Reader.ReadElementString());
                        flagArray[3] = true;
                    }
                    else if ((!flagArray[4] && (base.Reader.LocalName == this.id27_GenerateInternalTypes)) && (base.Reader.NamespaceURI == this.id2_Item))
                    {
                        o.GenerateInternalTypes = XmlConvert.ToBoolean(base.Reader.ReadElementString());
                        flagArray[4] = true;
                    }
                    else if ((!flagArray[5] && (base.Reader.LocalName == this.id28_GenerateMessageContracts)) && (base.Reader.NamespaceURI == this.id2_Item))
                    {
                        o.GenerateMessageContracts = XmlConvert.ToBoolean(base.Reader.ReadElementString());
                        flagArray[5] = true;
                    }
                    else if ((base.Reader.LocalName == this.id29_NamespaceMappings) && (base.Reader.NamespaceURI == this.id2_Item))
                    {
                        if (!base.ReadNull())
                        {
                            List<NamespaceMapping> list2 = o.NamespaceMappingList;
                            if ((list2 == null) || base.Reader.IsEmptyElement)
                            {
                                base.Reader.Skip();
                            }
                            else
                            {
                                base.Reader.ReadStartElement();
                                base.Reader.MoveToContent();
                                int num5 = 0;
                                int num6 = base.ReaderCount;
                                while ((base.Reader.NodeType != XmlNodeType.EndElement) && (base.Reader.NodeType != XmlNodeType.None))
                                {
                                    if (base.Reader.NodeType == XmlNodeType.Element)
                                    {
                                        if ((base.Reader.LocalName == this.id30_NamespaceMapping) && (base.Reader.NamespaceURI == this.id2_Item))
                                        {
                                            if (list2 == null)
                                            {
                                                base.Reader.Skip();
                                            }
                                            else
                                            {
                                                list2.Add(this.Read3_NamespaceMapping(true, true));
                                            }
                                        }
                                        else
                                        {
                                            base.UnknownNode(null, "urn:schemas-microsoft-com:xml-wcfservicemap:NamespaceMapping");
                                        }
                                    }
                                    else
                                    {
                                        base.UnknownNode(null, "urn:schemas-microsoft-com:xml-wcfservicemap:NamespaceMapping");
                                    }
                                    base.Reader.MoveToContent();
                                    base.CheckReaderCount(ref num5, ref num6);
                                }
                                base.ReadEndElement();
                            }
                        }
                    }
                    else if ((base.Reader.LocalName == this.id31_CollectionMappings) && (base.Reader.NamespaceURI == this.id2_Item))
                    {
                        if (!base.ReadNull())
                        {
                            List<ReferencedCollectionType> list3 = o.CollectionMappingList;
                            if ((list3 == null) || base.Reader.IsEmptyElement)
                            {
                                base.Reader.Skip();
                            }
                            else
                            {
                                base.Reader.ReadStartElement();
                                base.Reader.MoveToContent();
                                int num7 = 0;
                                int num8 = base.ReaderCount;
                                while ((base.Reader.NodeType != XmlNodeType.EndElement) && (base.Reader.NodeType != XmlNodeType.None))
                                {
                                    if (base.Reader.NodeType == XmlNodeType.Element)
                                    {
                                        if ((base.Reader.LocalName == this.id32_CollectionMapping) && (base.Reader.NamespaceURI == this.id2_Item))
                                        {
                                            if (list3 == null)
                                            {
                                                base.Reader.Skip();
                                            }
                                            else
                                            {
                                                list3.Add(this.Read5_ReferencedCollectionType(true, true));
                                            }
                                        }
                                        else
                                        {
                                            base.UnknownNode(null, "urn:schemas-microsoft-com:xml-wcfservicemap:CollectionMapping");
                                        }
                                    }
                                    else
                                    {
                                        base.UnknownNode(null, "urn:schemas-microsoft-com:xml-wcfservicemap:CollectionMapping");
                                    }
                                    base.Reader.MoveToContent();
                                    base.CheckReaderCount(ref num7, ref num8);
                                }
                                base.ReadEndElement();
                            }
                        }
                    }
                    else if ((!flagArray[8] && (base.Reader.LocalName == this.id33_GenerateSerializableTypes)) && (base.Reader.NamespaceURI == this.id2_Item))
                    {
                        o.GenerateSerializableTypes = XmlConvert.ToBoolean(base.Reader.ReadElementString());
                        flagArray[8] = true;
                    }
                    else if ((!flagArray[9] && (base.Reader.LocalName == this.id34_Serializer)) && (base.Reader.NamespaceURI == this.id2_Item))
                    {
                        o.Serializer = this.Read6_ProxySerializerType(base.Reader.ReadElementString());
                        flagArray[9] = true;
                    }
                    else if ((!flagArray[10] && (base.Reader.LocalName == this.id35_UseSerializerForFaults)) && (base.Reader.NamespaceURI == this.id2_Item))
                    {
                        o.UseSerializerForFaults = XmlConvert.ToBoolean(base.Reader.ReadElementString());
                        flagArray[10] = true;
                    }
                    else if ((!flagArray[11] && (base.Reader.LocalName == this.id36_Wrapped)) && (base.Reader.NamespaceURI == this.id2_Item))
                    {
                        o.Wrapped = XmlConvert.ToBoolean(base.Reader.ReadElementString());
                        flagArray[11] = true;
                    }
                    else if ((!flagArray[12] && (base.Reader.LocalName == this.id37_ReferenceAllAssemblies)) && (base.Reader.NamespaceURI == this.id2_Item))
                    {
                        o.ReferenceAllAssemblies = XmlConvert.ToBoolean(base.Reader.ReadElementString());
                        flagArray[12] = true;
                    }
                    else if ((base.Reader.LocalName == this.id38_ReferencedAssemblies) && (base.Reader.NamespaceURI == this.id2_Item))
                    {
                        if (!base.ReadNull())
                        {
                            List<ReferencedAssembly> list4 = o.ReferencedAssemblyList;
                            if ((list4 == null) || base.Reader.IsEmptyElement)
                            {
                                base.Reader.Skip();
                            }
                            else
                            {
                                base.Reader.ReadStartElement();
                                base.Reader.MoveToContent();
                                int num9 = 0;
                                int num10 = base.ReaderCount;
                                while ((base.Reader.NodeType != XmlNodeType.EndElement) && (base.Reader.NodeType != XmlNodeType.None))
                                {
                                    if (base.Reader.NodeType == XmlNodeType.Element)
                                    {
                                        if ((base.Reader.LocalName == this.id39_ReferencedAssembly) && (base.Reader.NamespaceURI == this.id2_Item))
                                        {
                                            if (list4 == null)
                                            {
                                                base.Reader.Skip();
                                            }
                                            else
                                            {
                                                list4.Add(this.Read7_ReferencedAssembly(true, true));
                                            }
                                        }
                                        else
                                        {
                                            base.UnknownNode(null, "urn:schemas-microsoft-com:xml-wcfservicemap:ReferencedAssembly");
                                        }
                                    }
                                    else
                                    {
                                        base.UnknownNode(null, "urn:schemas-microsoft-com:xml-wcfservicemap:ReferencedAssembly");
                                    }
                                    base.Reader.MoveToContent();
                                    base.CheckReaderCount(ref num9, ref num10);
                                }
                                base.ReadEndElement();
                            }
                        }
                    }
                    else if ((base.Reader.LocalName == this.id40_ReferencedDataContractTypes) && (base.Reader.NamespaceURI == this.id2_Item))
                    {
                        if (!base.ReadNull())
                        {
                            List<ReferencedType> list5 = o.ReferencedDataContractTypeList;
                            if ((list5 == null) || base.Reader.IsEmptyElement)
                            {
                                base.Reader.Skip();
                            }
                            else
                            {
                                base.Reader.ReadStartElement();
                                base.Reader.MoveToContent();
                                int num11 = 0;
                                int num12 = base.ReaderCount;
                                while ((base.Reader.NodeType != XmlNodeType.EndElement) && (base.Reader.NodeType != XmlNodeType.None))
                                {
                                    if (base.Reader.NodeType == XmlNodeType.Element)
                                    {
                                        if ((base.Reader.LocalName == this.id41_ReferencedDataContractType) && (base.Reader.NamespaceURI == this.id2_Item))
                                        {
                                            if (list5 == null)
                                            {
                                                base.Reader.Skip();
                                            }
                                            else
                                            {
                                                list5.Add(this.Read2_ReferencedType(true, true));
                                            }
                                        }
                                        else
                                        {
                                            base.UnknownNode(null, "urn:schemas-microsoft-com:xml-wcfservicemap:ReferencedDataContractType");
                                        }
                                    }
                                    else
                                    {
                                        base.UnknownNode(null, "urn:schemas-microsoft-com:xml-wcfservicemap:ReferencedDataContractType");
                                    }
                                    base.Reader.MoveToContent();
                                    base.CheckReaderCount(ref num11, ref num12);
                                }
                                base.ReadEndElement();
                            }
                        }
                    }
                    else if ((base.Reader.LocalName == this.id42_ServiceContractMappings) && (base.Reader.NamespaceURI == this.id2_Item))
                    {
                        if (!base.ReadNull())
                        {
                            List<ContractMapping> list6 = o.ServiceContractMappingList;
                            if ((list6 == null) || base.Reader.IsEmptyElement)
                            {
                                base.Reader.Skip();
                            }
                            else
                            {
                                base.Reader.ReadStartElement();
                                base.Reader.MoveToContent();
                                int num13 = 0;
                                int num14 = base.ReaderCount;
                                while ((base.Reader.NodeType != XmlNodeType.EndElement) && (base.Reader.NodeType != XmlNodeType.None))
                                {
                                    if (base.Reader.NodeType == XmlNodeType.Element)
                                    {
                                        if ((base.Reader.LocalName == this.id43_ServiceContractMapping) && (base.Reader.NamespaceURI == this.id2_Item))
                                        {
                                            if (list6 == null)
                                            {
                                                base.Reader.Skip();
                                            }
                                            else
                                            {
                                                list6.Add(this.Read8_ContractMapping(true, true));
                                            }
                                        }
                                        else
                                        {
                                            base.UnknownNode(null, "urn:schemas-microsoft-com:xml-wcfservicemap:ServiceContractMapping");
                                        }
                                    }
                                    else
                                    {
                                        base.UnknownNode(null, "urn:schemas-microsoft-com:xml-wcfservicemap:ServiceContractMapping");
                                    }
                                    base.Reader.MoveToContent();
                                    base.CheckReaderCount(ref num13, ref num14);
                                }
                                base.ReadEndElement();
                            }
                        }
                    }
                    else
                    {
                        base.UnknownNode(o, "urn:schemas-microsoft-com:xml-wcfservicemap:GenerateAsynchronousMethods, urn:schemas-microsoft-com:xml-wcfservicemap:EnableDataBinding, urn:schemas-microsoft-com:xml-wcfservicemap:ExcludedTypes, urn:schemas-microsoft-com:xml-wcfservicemap:ImportXmlTypes, urn:schemas-microsoft-com:xml-wcfservicemap:GenerateInternalTypes, urn:schemas-microsoft-com:xml-wcfservicemap:GenerateMessageContracts, urn:schemas-microsoft-com:xml-wcfservicemap:NamespaceMappings, urn:schemas-microsoft-com:xml-wcfservicemap:CollectionMappings, urn:schemas-microsoft-com:xml-wcfservicemap:GenerateSerializableTypes, urn:schemas-microsoft-com:xml-wcfservicemap:Serializer, urn:schemas-microsoft-com:xml-wcfservicemap:UseSerializerForFaults, urn:schemas-microsoft-com:xml-wcfservicemap:Wrapped, urn:schemas-microsoft-com:xml-wcfservicemap:ReferenceAllAssemblies, urn:schemas-microsoft-com:xml-wcfservicemap:ReferencedAssemblies, urn:schemas-microsoft-com:xml-wcfservicemap:ReferencedDataContractTypes, urn:schemas-microsoft-com:xml-wcfservicemap:ServiceContractMappings");
                    }
                }
                else
                {
                    base.UnknownNode(o, "urn:schemas-microsoft-com:xml-wcfservicemap:GenerateAsynchronousMethods, urn:schemas-microsoft-com:xml-wcfservicemap:EnableDataBinding, urn:schemas-microsoft-com:xml-wcfservicemap:ExcludedTypes, urn:schemas-microsoft-com:xml-wcfservicemap:ImportXmlTypes, urn:schemas-microsoft-com:xml-wcfservicemap:GenerateInternalTypes, urn:schemas-microsoft-com:xml-wcfservicemap:GenerateMessageContracts, urn:schemas-microsoft-com:xml-wcfservicemap:NamespaceMappings, urn:schemas-microsoft-com:xml-wcfservicemap:CollectionMappings, urn:schemas-microsoft-com:xml-wcfservicemap:GenerateSerializableTypes, urn:schemas-microsoft-com:xml-wcfservicemap:Serializer, urn:schemas-microsoft-com:xml-wcfservicemap:UseSerializerForFaults, urn:schemas-microsoft-com:xml-wcfservicemap:Wrapped, urn:schemas-microsoft-com:xml-wcfservicemap:ReferenceAllAssemblies, urn:schemas-microsoft-com:xml-wcfservicemap:ReferencedAssemblies, urn:schemas-microsoft-com:xml-wcfservicemap:ReferencedDataContractTypes, urn:schemas-microsoft-com:xml-wcfservicemap:ServiceContractMappings");
                }
                base.Reader.MoveToContent();
                base.CheckReaderCount(ref whileIterations, ref readerCount);
            }
            base.ReadEndElement();
            return o;
        }
    }
}

