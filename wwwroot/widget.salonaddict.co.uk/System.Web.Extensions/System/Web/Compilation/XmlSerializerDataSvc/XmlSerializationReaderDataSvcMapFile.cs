namespace System.Web.Compilation.XmlSerializerDataSvc
{
    using System;
    using System.Collections.Generic;
    using System.Web.Compilation.WCFModel;
    using System.Xml;
    using System.Xml.Serialization;

    internal class XmlSerializationReaderDataSvcMapFile : XmlSerializationReader
    {
        private string id1_ReferenceGroup;
        private string id10_Extensions;
        private string id11_ExtensionFile;
        private string id12_FileName;
        private string id13_Name;
        private string id14_MetadataType;
        private string id15_Ignore;
        private string id16_IsMergeResult;
        private string id17_SourceId;
        private string id18_SourceUrl;
        private string id19_Address;
        private string id2_Item;
        private string id20_Protocol;
        private string id3_DataSvcMapFile;
        private string id4_ID;
        private string id5_Item;
        private string id6_MetadataSources;
        private string id7_MetadataSource;
        private string id8_Metadata;
        private string id9_MetadataFile;

        protected override void InitCallbacks()
        {
        }

        protected override void InitIDs()
        {
            this.id3_DataSvcMapFile = base.Reader.NameTable.Add("DataSvcMapFile");
            this.id9_MetadataFile = base.Reader.NameTable.Add("MetadataFile");
            this.id10_Extensions = base.Reader.NameTable.Add("Extensions");
            this.id11_ExtensionFile = base.Reader.NameTable.Add("ExtensionFile");
            this.id8_Metadata = base.Reader.NameTable.Add("Metadata");
            this.id6_MetadataSources = base.Reader.NameTable.Add("MetadataSources");
            this.id17_SourceId = base.Reader.NameTable.Add("SourceId");
            this.id1_ReferenceGroup = base.Reader.NameTable.Add("ReferenceGroup");
            this.id4_ID = base.Reader.NameTable.Add("ID");
            this.id5_Item = base.Reader.NameTable.Add("");
            this.id7_MetadataSource = base.Reader.NameTable.Add("MetadataSource");
            this.id13_Name = base.Reader.NameTable.Add("Name");
            this.id14_MetadataType = base.Reader.NameTable.Add("MetadataType");
            this.id19_Address = base.Reader.NameTable.Add("Address");
            this.id18_SourceUrl = base.Reader.NameTable.Add("SourceUrl");
            this.id16_IsMergeResult = base.Reader.NameTable.Add("IsMergeResult");
            this.id15_Ignore = base.Reader.NameTable.Add("Ignore");
            this.id20_Protocol = base.Reader.NameTable.Add("Protocol");
            this.id2_Item = base.Reader.NameTable.Add("urn:schemas-microsoft-com:xml-dataservicemap");
            this.id12_FileName = base.Reader.NameTable.Add("FileName");
        }

        private ExtensionFile Read3_ExtensionFile(bool isNullable, bool checkType)
        {
            XmlQualifiedName type = checkType ? base.GetXsiType() : null;
            bool flag = false;
            if (isNullable)
            {
                flag = base.ReadNull();
            }
            if ((checkType && (type != null)) && ((type.Name != this.id11_ExtensionFile) || (type.Namespace != this.id2_Item)))
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
                if ((!flagArray[0] && (base.Reader.LocalName == this.id12_FileName)) && (base.Reader.NamespaceURI == this.id5_Item))
                {
                    o.FileName = base.Reader.Value;
                    flagArray[0] = true;
                }
                else
                {
                    if ((!flagArray[1] && (base.Reader.LocalName == this.id13_Name)) && (base.Reader.NamespaceURI == this.id5_Item))
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

        private MetadataFile.MetadataType Read4_MetadataType(string s)
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

                case "Edmx":
                    return MetadataFile.MetadataType.Edmx;
            }
            throw base.CreateUnknownConstantException(s, typeof(MetadataFile.MetadataType));
        }

        private MetadataFile Read5_MetadataFile(bool isNullable, bool checkType)
        {
            XmlQualifiedName type = checkType ? base.GetXsiType() : null;
            bool flag = false;
            if (isNullable)
            {
                flag = base.ReadNull();
            }
            if ((checkType && (type != null)) && ((type.Name != this.id9_MetadataFile) || (type.Namespace != this.id2_Item)))
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
                if ((!flagArray[0] && (base.Reader.LocalName == this.id12_FileName)) && (base.Reader.NamespaceURI == this.id5_Item))
                {
                    o.FileName = base.Reader.Value;
                    flagArray[0] = true;
                }
                else
                {
                    if ((!flagArray[1] && (base.Reader.LocalName == this.id14_MetadataType)) && (base.Reader.NamespaceURI == this.id5_Item))
                    {
                        o.FileType = this.Read4_MetadataType(base.Reader.Value);
                        flagArray[1] = true;
                        continue;
                    }
                    if ((!flagArray[2] && (base.Reader.LocalName == this.id4_ID)) && (base.Reader.NamespaceURI == this.id5_Item))
                    {
                        o.ID = base.Reader.Value;
                        flagArray[2] = true;
                        continue;
                    }
                    if ((!flagArray[3] && (base.Reader.LocalName == this.id15_Ignore)) && (base.Reader.NamespaceURI == this.id5_Item))
                    {
                        o.Ignore = XmlConvert.ToBoolean(base.Reader.Value);
                        o.IgnoreSpecified = true;
                        flagArray[3] = true;
                        continue;
                    }
                    if ((!flagArray[4] && (base.Reader.LocalName == this.id16_IsMergeResult)) && (base.Reader.NamespaceURI == this.id5_Item))
                    {
                        o.IsMergeResult = XmlConvert.ToBoolean(base.Reader.Value);
                        o.IsMergeResultSpecified = true;
                        flagArray[4] = true;
                        continue;
                    }
                    if ((!flagArray[5] && (base.Reader.LocalName == this.id17_SourceId)) && (base.Reader.NamespaceURI == this.id5_Item))
                    {
                        o.SourceId = XmlConvert.ToInt32(base.Reader.Value);
                        o.SourceIdSpecified = true;
                        flagArray[5] = true;
                        continue;
                    }
                    if ((!flagArray[6] && (base.Reader.LocalName == this.id18_SourceUrl)) && (base.Reader.NamespaceURI == this.id5_Item))
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

        private MetadataSource Read6_MetadataSource(bool isNullable, bool checkType)
        {
            XmlQualifiedName type = checkType ? base.GetXsiType() : null;
            bool flag = false;
            if (isNullable)
            {
                flag = base.ReadNull();
            }
            if ((checkType && (type != null)) && ((type.Name != this.id7_MetadataSource) || (type.Namespace != this.id2_Item)))
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
                if ((!flagArray[0] && (base.Reader.LocalName == this.id19_Address)) && (base.Reader.NamespaceURI == this.id5_Item))
                {
                    o.Address = base.Reader.Value;
                    flagArray[0] = true;
                }
                else
                {
                    if ((!flagArray[1] && (base.Reader.LocalName == this.id20_Protocol)) && (base.Reader.NamespaceURI == this.id5_Item))
                    {
                        o.Protocol = base.Reader.Value;
                        flagArray[1] = true;
                        continue;
                    }
                    if ((!flagArray[2] && (base.Reader.LocalName == this.id17_SourceId)) && (base.Reader.NamespaceURI == this.id5_Item))
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

        private DataSvcMapFile Read7_DataSvcMapFile(bool isNullable, bool checkType)
        {
            XmlQualifiedName type = checkType ? base.GetXsiType() : null;
            bool flag = false;
            if (isNullable)
            {
                flag = base.ReadNull();
            }
            if ((checkType && (type != null)) && ((type.Name != this.id3_DataSvcMapFile) || (type.Namespace != this.id2_Item)))
            {
                throw base.CreateUnknownTypeException(type);
            }
            if (flag)
            {
                return null;
            }
            DataSvcMapFile o = new DataSvcMapFile();
            List<MetadataSource> metadataSourceList = o.MetadataSourceList;
            List<MetadataFile> metadataList = o.MetadataList;
            List<ExtensionFile> extensions = o.Extensions;
            bool[] flagArray = new bool[4];
            while (base.Reader.MoveToNextAttribute())
            {
                if ((!flagArray[3] && (base.Reader.LocalName == this.id4_ID)) && (base.Reader.NamespaceURI == this.id5_Item))
                {
                    o.ID = base.Reader.Value;
                    flagArray[3] = true;
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
                    goto Label_051E;
                }
                switch (num)
                {
                    case 0:
                        if ((base.Reader.LocalName != this.id6_MetadataSources) || (base.Reader.NamespaceURI != this.id2_Item))
                        {
                            goto Label_0294;
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
                        goto Label_0526;

                    case 1:
                        if ((base.Reader.LocalName != this.id8_Metadata) || (base.Reader.NamespaceURI != this.id2_Item))
                        {
                            goto Label_03D3;
                        }
                        if (!base.ReadNull())
                        {
                            list2 = o.MetadataList;
                            if ((list2 != null) && !base.Reader.IsEmptyElement)
                            {
                                goto Label_02FC;
                            }
                            base.Reader.Skip();
                        }
                        goto Label_0526;

                    case 2:
                        if ((base.Reader.LocalName != this.id10_Extensions) || (base.Reader.NamespaceURI != this.id2_Item))
                        {
                            goto Label_050F;
                        }
                        if (!base.ReadNull())
                        {
                            list3 = o.Extensions;
                            if ((list3 != null) && !base.Reader.IsEmptyElement)
                            {
                                goto Label_043B;
                            }
                            base.Reader.Skip();
                        }
                        goto Label_0526;

                    default:
                        base.UnknownNode(o, null);
                        goto Label_0526;
                }
                base.Reader.ReadStartElement();
                base.Reader.MoveToContent();
                int num4 = 0;
                int num5 = base.ReaderCount;
                while ((base.Reader.NodeType != XmlNodeType.EndElement) && (base.Reader.NodeType != XmlNodeType.None))
                {
                    if (base.Reader.NodeType == XmlNodeType.Element)
                    {
                        if ((base.Reader.LocalName == this.id7_MetadataSource) && (base.Reader.NamespaceURI == this.id2_Item))
                        {
                            if (list == null)
                            {
                                base.Reader.Skip();
                            }
                            else
                            {
                                list.Add(this.Read6_MetadataSource(true, true));
                            }
                        }
                        else
                        {
                            base.UnknownNode(null, "urn:schemas-microsoft-com:xml-dataservicemap:MetadataSource");
                        }
                    }
                    else
                    {
                        base.UnknownNode(null, "urn:schemas-microsoft-com:xml-dataservicemap:MetadataSource");
                    }
                    base.Reader.MoveToContent();
                    base.CheckReaderCount(ref num4, ref num5);
                }
                base.ReadEndElement();
                goto Label_0526;
            Label_0294:
                num = 1;
                goto Label_0526;
            Label_02FC:
                base.Reader.ReadStartElement();
                base.Reader.MoveToContent();
                int num6 = 0;
                int num7 = base.ReaderCount;
                while ((base.Reader.NodeType != XmlNodeType.EndElement) && (base.Reader.NodeType != XmlNodeType.None))
                {
                    if (base.Reader.NodeType == XmlNodeType.Element)
                    {
                        if ((base.Reader.LocalName == this.id9_MetadataFile) && (base.Reader.NamespaceURI == this.id2_Item))
                        {
                            if (list2 == null)
                            {
                                base.Reader.Skip();
                            }
                            else
                            {
                                list2.Add(this.Read5_MetadataFile(true, true));
                            }
                        }
                        else
                        {
                            base.UnknownNode(null, "urn:schemas-microsoft-com:xml-dataservicemap:MetadataFile");
                        }
                    }
                    else
                    {
                        base.UnknownNode(null, "urn:schemas-microsoft-com:xml-dataservicemap:MetadataFile");
                    }
                    base.Reader.MoveToContent();
                    base.CheckReaderCount(ref num6, ref num7);
                }
                base.ReadEndElement();
                goto Label_0526;
            Label_03D3:
                num = 2;
                goto Label_0526;
            Label_043B:
                base.Reader.ReadStartElement();
                base.Reader.MoveToContent();
                int num8 = 0;
                int num9 = base.ReaderCount;
                while ((base.Reader.NodeType != XmlNodeType.EndElement) && (base.Reader.NodeType != XmlNodeType.None))
                {
                    if (base.Reader.NodeType == XmlNodeType.Element)
                    {
                        if ((base.Reader.LocalName == this.id11_ExtensionFile) && (base.Reader.NamespaceURI == this.id2_Item))
                        {
                            if (list3 == null)
                            {
                                base.Reader.Skip();
                            }
                            else
                            {
                                list3.Add(this.Read3_ExtensionFile(true, true));
                            }
                        }
                        else
                        {
                            base.UnknownNode(null, "urn:schemas-microsoft-com:xml-dataservicemap:ExtensionFile");
                        }
                    }
                    else
                    {
                        base.UnknownNode(null, "urn:schemas-microsoft-com:xml-dataservicemap:ExtensionFile");
                    }
                    base.Reader.MoveToContent();
                    base.CheckReaderCount(ref num8, ref num9);
                }
                base.ReadEndElement();
                goto Label_0526;
            Label_050F:
                num = 3;
                goto Label_0526;
            Label_051E:
                base.UnknownNode(o, null);
            Label_0526:
                base.Reader.MoveToContent();
                base.CheckReaderCount(ref whileIterations, ref readerCount);
            }
            base.ReadEndElement();
            return o;
        }

        public object Read8_ReferenceGroup()
        {
            base.Reader.MoveToContent();
            if (base.Reader.NodeType == XmlNodeType.Element)
            {
                if ((base.Reader.LocalName != this.id1_ReferenceGroup) || (base.Reader.NamespaceURI != this.id2_Item))
                {
                    throw base.CreateUnknownNodeException();
                }
                return this.Read7_DataSvcMapFile(true, true);
            }
            base.UnknownNode(null, "urn:schemas-microsoft-com:xml-dataservicemap:ReferenceGroup");
            return null;
        }
    }
}

