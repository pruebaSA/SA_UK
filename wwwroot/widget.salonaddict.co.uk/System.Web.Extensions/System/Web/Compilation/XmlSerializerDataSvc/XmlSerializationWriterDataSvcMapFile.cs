namespace System.Web.Compilation.XmlSerializerDataSvc
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Web.Compilation.WCFModel;
    using System.Xml;
    using System.Xml.Serialization;

    internal class XmlSerializationWriterDataSvcMapFile : XmlSerializationWriter
    {
        protected override void InitCallbacks()
        {
        }

        private void Write3_ExtensionFile(string n, string ns, ExtensionFile o, bool isNullable, bool needType)
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
                    base.WriteXsiType("ExtensionFile", "urn:schemas-microsoft-com:xml-dataservicemap");
                }
                base.WriteAttribute("FileName", "", o.FileName);
                base.WriteAttribute("Name", "", o.Name);
                base.WriteEndElement(o);
            }
        }

        private string Write4_MetadataType(MetadataFile.MetadataType v)
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

                case MetadataFile.MetadataType.Edmx:
                    return "Edmx";
            }
            long num = (long) v;
            throw base.CreateInvalidEnumValueException(num.ToString(CultureInfo.InvariantCulture), "System.Web.Compilation.WCFModel.MetadataFile.MetadataType");
        }

        private void Write5_MetadataFile(string n, string ns, MetadataFile o, bool isNullable, bool needType)
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
                    base.WriteXsiType("MetadataFile", "urn:schemas-microsoft-com:xml-dataservicemap");
                }
                base.WriteAttribute("FileName", "", o.FileName);
                base.WriteAttribute("MetadataType", "", this.Write4_MetadataType(o.FileType));
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

        private void Write6_MetadataSource(string n, string ns, MetadataSource o, bool isNullable, bool needType)
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
                    base.WriteXsiType("MetadataSource", "urn:schemas-microsoft-com:xml-dataservicemap");
                }
                base.WriteAttribute("Address", "", o.Address);
                base.WriteAttribute("Protocol", "", o.Protocol);
                base.WriteAttribute("SourceId", "", XmlConvert.ToString(o.SourceId));
                base.WriteEndElement(o);
            }
        }

        private void Write7_DataSvcMapFile(string n, string ns, DataSvcMapFile o, bool isNullable, bool needType)
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
                if (!needType && (o.GetType() != typeof(DataSvcMapFile)))
                {
                    throw base.CreateUnknownTypeException(o);
                }
                base.WriteStartElement(n, ns, o, false, null);
                if (needType)
                {
                    base.WriteXsiType("DataSvcMapFile", "urn:schemas-microsoft-com:xml-dataservicemap");
                }
                base.WriteAttribute("ID", "", o.ID);
                List<MetadataSource> metadataSourceList = o.MetadataSourceList;
                if (metadataSourceList != null)
                {
                    base.WriteStartElement("MetadataSources", "urn:schemas-microsoft-com:xml-dataservicemap", null, false);
                    for (int i = 0; i < metadataSourceList.Count; i++)
                    {
                        this.Write6_MetadataSource("MetadataSource", "urn:schemas-microsoft-com:xml-dataservicemap", metadataSourceList[i], true, false);
                    }
                    base.WriteEndElement();
                }
                List<MetadataFile> metadataList = o.MetadataList;
                if (metadataList != null)
                {
                    base.WriteStartElement("Metadata", "urn:schemas-microsoft-com:xml-dataservicemap", null, false);
                    for (int j = 0; j < metadataList.Count; j++)
                    {
                        this.Write5_MetadataFile("MetadataFile", "urn:schemas-microsoft-com:xml-dataservicemap", metadataList[j], true, false);
                    }
                    base.WriteEndElement();
                }
                List<ExtensionFile> extensions = o.Extensions;
                if (extensions != null)
                {
                    base.WriteStartElement("Extensions", "urn:schemas-microsoft-com:xml-dataservicemap", null, false);
                    for (int k = 0; k < extensions.Count; k++)
                    {
                        this.Write3_ExtensionFile("ExtensionFile", "urn:schemas-microsoft-com:xml-dataservicemap", extensions[k], true, false);
                    }
                    base.WriteEndElement();
                }
                base.WriteEndElement(o);
            }
        }

        public void Write8_ReferenceGroup(object o)
        {
            base.WriteStartDocument();
            if (o == null)
            {
                base.WriteNullTagLiteral("ReferenceGroup", "urn:schemas-microsoft-com:xml-dataservicemap");
            }
            else
            {
                base.TopLevelElement();
                this.Write7_DataSvcMapFile("ReferenceGroup", "urn:schemas-microsoft-com:xml-dataservicemap", (DataSvcMapFile) o, true, false);
            }
        }
    }
}

