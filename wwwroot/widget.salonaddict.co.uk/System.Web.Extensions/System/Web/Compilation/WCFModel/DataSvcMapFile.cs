namespace System.Web.Compilation.WCFModel
{
    using System;
    using System.Collections.Generic;
    using System.Xml.Serialization;

    [XmlRoot(Namespace="urn:schemas-microsoft-com:xml-dataservicemap", ElementName="ReferenceGroup")]
    internal class DataSvcMapFile
    {
        private IEnumerable<ProxyGenerationError> loadErrors;
        private List<ExtensionFile> m_ExtensionFileList;
        private string m_ID = Guid.NewGuid().ToString();
        private List<MetadataFile> m_MetadataList;
        private List<MetadataSource> m_MetadataSourceList;
        public const string NamespaceUri = "urn:schemas-microsoft-com:xml-dataservicemap";

        internal void SetLoadErrors(IEnumerable<ProxyGenerationError> loadErrors)
        {
            this.loadErrors = loadErrors;
        }

        [XmlArray(ElementName="Extensions", Order=2), XmlArrayItem("ExtensionFile", typeof(ExtensionFile))]
        public List<ExtensionFile> Extensions
        {
            get
            {
                if (this.m_ExtensionFileList == null)
                {
                    this.m_ExtensionFileList = new List<ExtensionFile>();
                }
                return this.m_ExtensionFileList;
            }
        }

        [XmlAttribute]
        public string ID
        {
            get => 
                this.m_ID;
            set
            {
                this.m_ID = value;
            }
        }

        [XmlIgnore]
        public IEnumerable<ProxyGenerationError> LoadErrors
        {
            get
            {
                List<ProxyGenerationError> list = new List<ProxyGenerationError>();
                if (this.loadErrors != null)
                {
                    list.AddRange(this.loadErrors);
                }
                return list;
            }
        }

        [XmlArrayItem("MetadataFile", typeof(MetadataFile)), XmlArray(ElementName="Metadata", Order=1)]
        public List<MetadataFile> MetadataList
        {
            get
            {
                if (this.m_MetadataList == null)
                {
                    this.m_MetadataList = new List<MetadataFile>();
                }
                return this.m_MetadataList;
            }
        }

        [XmlArray(ElementName="MetadataSources", Order=0), XmlArrayItem("MetadataSource", typeof(MetadataSource))]
        public List<MetadataSource> MetadataSourceList
        {
            get
            {
                if (this.m_MetadataSourceList == null)
                {
                    this.m_MetadataSourceList = new List<MetadataSource>();
                }
                return this.m_MetadataSourceList;
            }
        }
    }
}

