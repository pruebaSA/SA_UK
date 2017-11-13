namespace System.Web.Compilation.WCFModel
{
    using System;
    using System.Collections.Generic;
    using System.Xml.Serialization;

    [XmlRoot(Namespace="urn:schemas-microsoft-com:xml-wcfservicemap", ElementName="ReferenceGroup")]
    internal class SvcMapFile
    {
        private IEnumerable<ProxyGenerationError> loadErrors;
        private System.Web.Compilation.WCFModel.ClientOptions m_ClientOptions;
        private List<ExtensionFile> m_ExtensionFileList;
        private string m_ID = Guid.NewGuid().ToString();
        private List<MetadataFile> m_MetadataList;
        private List<MetadataSource> m_MetadataSourceList;
        public const string NamespaceUri = "urn:schemas-microsoft-com:xml-wcfservicemap";

        internal void SetLoadErrors(IEnumerable<ProxyGenerationError> loadErrors)
        {
            this.loadErrors = loadErrors;
        }

        [XmlElement(Order=0)]
        public System.Web.Compilation.WCFModel.ClientOptions ClientOptions
        {
            get
            {
                if (this.m_ClientOptions == null)
                {
                    this.m_ClientOptions = new System.Web.Compilation.WCFModel.ClientOptions();
                }
                return this.m_ClientOptions;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }
                this.m_ClientOptions = value;
            }
        }

        [XmlArrayItem("ExtensionFile", typeof(ExtensionFile)), XmlArray(ElementName="Extensions", Order=3)]
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

        [XmlArray(ElementName="Metadata", Order=2), XmlArrayItem("MetadataFile", typeof(MetadataFile))]
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

        [XmlArray(ElementName="MetadataSources", Order=1), XmlArrayItem("MetadataSource", typeof(MetadataSource))]
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

