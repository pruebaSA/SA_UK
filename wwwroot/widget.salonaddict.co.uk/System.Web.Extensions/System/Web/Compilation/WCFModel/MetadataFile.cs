namespace System.Web.Compilation.WCFModel
{
    using System;
    using System.IO;
    using System.ServiceModel.Description;
    using System.Web.Resources;
    using System.Web.Services.Description;
    using System.Web.Services.Discovery;
    using System.Xml;
    using System.Xml.Schema;
    using System.Xml.Serialization;

    internal sealed class MetadataFile : ExternalFile
    {
        public const string DEFAULT_FILE_NAME = "service";
        private byte[] m_BinaryContent;
        private MetadataContent m_CachedMetadata;
        private string m_ID;
        private bool m_Ignore;
        private bool m_IsMergeResult;
        private MetadataType m_MetadataType;
        private int m_SourceId;
        private string m_SourceUrl;
        private int SOURCE_ID_NOT_SPECIFIED;

        public MetadataFile()
        {
            this.m_ID = Guid.NewGuid().ToString();
            this.m_BinaryContent = new byte[0];
        }

        public MetadataFile(string name, string url, string content) : base(name)
        {
            this.m_ID = Guid.NewGuid().ToString();
            this.m_SourceUrl = url;
            if (content == null)
            {
                throw new ArgumentNullException("content");
            }
            this.LoadContent(content);
        }

        public MetadataFile(string name, string url, byte[] byteContent) : base(name)
        {
            this.m_ID = Guid.NewGuid().ToString();
            this.m_SourceUrl = url;
            if (byteContent == null)
            {
                throw new ArgumentNullException("byteContent");
            }
            this.LoadContent(byteContent);
        }

        internal void CleanUpContent()
        {
            base.ErrorInLoading = null;
            this.m_BinaryContent = new byte[0];
            this.m_CachedMetadata = null;
        }

        internal MetadataSection CreateMetadataSection()
        {
            MetadataContent content = this.LoadMetadataContent(this.m_MetadataType);
            if (content.MetadataFormatError != null)
            {
                throw content.MetadataFormatError;
            }
            MetadataSection section = null;
            switch (this.FileType)
            {
                case MetadataType.Unknown:
                    return section;

                case MetadataType.Disco:
                    if (content.MetadataServiceDescription != null)
                    {
                        section = MetadataSection.CreateFromServiceDescription(content.MetadataServiceDescription);
                    }
                    return section;

                case MetadataType.Wsdl:
                {
                    System.Web.Services.Description.ServiceDescription metadataServiceDescription = content.MetadataServiceDescription;
                    if (metadataServiceDescription != null)
                    {
                        section = MetadataSection.CreateFromServiceDescription(metadataServiceDescription);
                    }
                    return section;
                }
                case MetadataType.Schema:
                    if (content.MetadataXmlSchema != null)
                    {
                        section = MetadataSection.CreateFromSchema(content.MetadataXmlSchema);
                    }
                    return section;

                case MetadataType.Policy:
                    if (content.MetadataXmlDocument != null)
                    {
                        section = MetadataSection.CreateFromPolicy(content.MetadataXmlDocument.DocumentElement, null);
                    }
                    return section;

                case MetadataType.Xml:
                case MetadataType.Edmx:
                    if (content.MetadataXmlDocument != null)
                    {
                        section = new MetadataSection(null, null, content.MetadataXmlDocument.DocumentElement);
                    }
                    return section;
            }
            return section;
        }

        private MetadataType DetermineFileType(XmlReader reader)
        {
            try
            {
                if (reader.IsStartElement("definitions", "http://schemas.xmlsoap.org/wsdl/"))
                {
                    return MetadataType.Wsdl;
                }
                if (reader.IsStartElement("schema", "http://www.w3.org/2001/XMLSchema"))
                {
                    return MetadataType.Schema;
                }
                if (reader.IsStartElement("Policy", "http://schemas.xmlsoap.org/ws/2004/09/policy") || reader.IsStartElement("Policy", "http://www.w3.org/ns/ws-policy"))
                {
                    return MetadataType.Policy;
                }
                if (reader.IsStartElement("discovery", "http://schemas.xmlsoap.org/disco/"))
                {
                    return MetadataType.Disco;
                }
                if (reader.IsStartElement("Edmx", "http://schemas.microsoft.com/ado/2007/06/edmx"))
                {
                    return MetadataType.Edmx;
                }
                return MetadataType.Xml;
            }
            catch (XmlException)
            {
                return MetadataType.Unknown;
            }
        }

        public string GetDefaultExtension()
        {
            switch (this.m_MetadataType)
            {
                case MetadataType.Disco:
                    return "disco";

                case MetadataType.Wsdl:
                    return "wsdl";

                case MetadataType.Schema:
                    return "xsd";

                case MetadataType.Policy:
                    return "xml";

                case MetadataType.Xml:
                    return "xml";

                case MetadataType.Edmx:
                    return "Edmx";
            }
            return "data";
        }

        public string GetDefaultFileName()
        {
            if (!string.IsNullOrEmpty(this.TargetNamespace))
            {
                string targetNamespace = this.TargetNamespace;
                if (!targetNamespace.EndsWith("/", StringComparison.Ordinal))
                {
                    int num = targetNamespace.LastIndexOfAny(Path.GetInvalidFileNameChars());
                    if (num >= 0)
                    {
                        targetNamespace = targetNamespace.Substring(num + 1);
                    }
                    string str2 = "." + this.GetDefaultExtension();
                    if ((targetNamespace.Length > str2.Length) && targetNamespace.EndsWith(str2, StringComparison.OrdinalIgnoreCase))
                    {
                        targetNamespace = targetNamespace.Substring(0, targetNamespace.Length - str2.Length);
                    }
                    if (targetNamespace.Length > 0)
                    {
                        return targetNamespace;
                    }
                }
            }
            return "service";
        }

        internal string GetMetadataSourceUrl()
        {
            if (string.IsNullOrEmpty(this.SourceUrl))
            {
                return base.FileName;
            }
            return this.SourceUrl;
        }

        internal void LoadContent(byte[] byteContent)
        {
            this.m_BinaryContent = byteContent;
            this.LoadContentFromTextReader(new StreamReader(new MemoryStream(byteContent)));
        }

        internal void LoadContent(string content)
        {
            MemoryStream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);
            writer.Write(content);
            writer.Flush();
            this.m_BinaryContent = stream.ToArray();
            this.LoadContentFromTextReader(new StringReader(content));
        }

        private void LoadContentFromTextReader(TextReader contentReader)
        {
            if (contentReader == null)
            {
                throw new ArgumentNullException("contentReader");
            }
            base.ErrorInLoading = null;
            this.m_CachedMetadata = null;
            using (XmlTextReader reader = new XmlTextReader(contentReader))
            {
                if (this.m_MetadataType == MetadataType.Unknown)
                {
                    MetadataType fileType = this.DetermineFileType(reader);
                    this.m_CachedMetadata = this.LoadMetadataContent(fileType, reader);
                    if (this.m_CachedMetadata.MetadataFormatError == null)
                    {
                        this.m_MetadataType = fileType;
                    }
                }
            }
        }

        private MetadataContent LoadMetadataContent(MetadataType fileType)
        {
            if (base.ErrorInLoading != null)
            {
                return new MetadataContent(base.ErrorInLoading);
            }
            using (XmlTextReader reader = new XmlTextReader(new StreamReader(new MemoryStream(this.m_BinaryContent))))
            {
                return this.LoadMetadataContent(fileType, reader);
            }
        }

        private MetadataContent LoadMetadataContent(MetadataType fileType, XmlTextReader xmlReader)
        {
            MetadataContent content = new MetadataContent();
            try
            {
                switch (fileType)
                {
                    case MetadataType.Unknown:
                        return content;

                    case MetadataType.Disco:
                        return new MetadataContent(DiscoveryDocument.Read(xmlReader));

                    case MetadataType.Wsdl:
                        return new MetadataContent(System.Web.Services.Description.ServiceDescription.Read(xmlReader)) { MetadataServiceDescription = { RetrievalUrl = this.GetMetadataSourceUrl() } };

                    case MetadataType.Schema:
                        return new MetadataContent(System.Xml.Schema.XmlSchema.Read(xmlReader, null)) { MetadataXmlSchema = { SourceUri = this.GetMetadataSourceUrl() } };
                }
                XmlDocument document = new XmlDocument();
                document.Load(xmlReader);
                return new MetadataContent(document);
            }
            catch (Exception exception)
            {
                return new MetadataContent(exception);
            }
        }

        public byte[] BinaryContent =>
            this.m_BinaryContent;

        private MetadataContent CachedMetadata
        {
            get
            {
                if (this.m_CachedMetadata == null)
                {
                    this.m_CachedMetadata = this.LoadMetadataContent(this.m_MetadataType);
                }
                return this.m_CachedMetadata;
            }
        }

        public string Content
        {
            get
            {
                StreamReader reader = new StreamReader(new MemoryStream(this.m_BinaryContent));
                return reader.ReadToEnd();
            }
        }

        [XmlAttribute("MetadataType")]
        public MetadataType FileType
        {
            get => 
                this.m_MetadataType;
            set
            {
                this.m_MetadataType = value;
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

        [XmlAttribute]
        public bool Ignore
        {
            get => 
                this.m_Ignore;
            set
            {
                this.m_Ignore = value;
            }
        }

        [XmlIgnore]
        public bool IgnoreSpecified
        {
            get => 
                this.m_Ignore;
            set
            {
                if (!value)
                {
                    this.m_Ignore = false;
                }
            }
        }

        [XmlAttribute]
        public bool IsMergeResult
        {
            get => 
                this.m_IsMergeResult;
            set
            {
                this.m_IsMergeResult = value;
            }
        }

        [XmlIgnore]
        public bool IsMergeResultSpecified
        {
            get => 
                this.m_IsMergeResult;
            set
            {
                if (!value)
                {
                    this.m_IsMergeResult = false;
                }
            }
        }

        public DiscoveryDocument MetadataDiscoveryDocument =>
            this.CachedMetadata.MetadataDiscoveryDocument;

        [XmlIgnore]
        public Exception MetadataFormatError =>
            this.CachedMetadata.MetadataFormatError;

        public System.Web.Services.Description.ServiceDescription MetadataServiceDescription =>
            this.CachedMetadata.MetadataServiceDescription;

        public XmlDocument MetadataXmlDocument =>
            this.CachedMetadata.MetadataXmlDocument;

        public System.Xml.Schema.XmlSchema MetadataXmlSchema =>
            this.CachedMetadata.MetadataXmlSchema;

        [XmlAttribute]
        public int SourceId
        {
            get => 
                this.m_SourceId;
            set
            {
                if (value < 0)
                {
                    throw new ArgumentException(WCFModelStrings.ReferenceGroup_InvalidSourceId);
                }
                this.m_SourceId = value;
            }
        }

        [XmlIgnore]
        public bool SourceIdSpecified
        {
            get => 
                (this.m_SourceId != this.SOURCE_ID_NOT_SPECIFIED);
            set
            {
                if (!value)
                {
                    this.m_SourceId = this.SOURCE_ID_NOT_SPECIFIED;
                }
            }
        }

        [XmlAttribute]
        public string SourceUrl
        {
            get => 
                this.m_SourceUrl;
            set
            {
                this.m_SourceUrl = value;
            }
        }

        public string TargetNamespace =>
            this.CachedMetadata.TargetNamespace;

        private class MetadataContent
        {
            private DiscoveryDocument m_MetadataDiscoveryDocument;
            private Exception m_MetadataFormatError;
            private System.Web.Services.Description.ServiceDescription m_MetadataServiceDescription;
            private XmlDocument m_MetadataXmlDocument;
            private System.Xml.Schema.XmlSchema m_MetadataXmlSchema;
            private string m_TargetNamespace;

            internal MetadataContent()
            {
                this.m_TargetNamespace = string.Empty;
            }

            internal MetadataContent(Exception metadataFormatError)
            {
                this.m_MetadataFormatError = metadataFormatError;
            }

            internal MetadataContent(System.Web.Services.Description.ServiceDescription serviceDescription)
            {
                this.m_MetadataServiceDescription = serviceDescription;
                this.m_TargetNamespace = serviceDescription.TargetNamespace;
            }

            internal MetadataContent(DiscoveryDocument discoveryDocument)
            {
                this.m_MetadataDiscoveryDocument = discoveryDocument;
                this.m_TargetNamespace = string.Empty;
            }

            internal MetadataContent(System.Xml.Schema.XmlSchema schema)
            {
                this.m_MetadataXmlSchema = schema;
                this.m_TargetNamespace = schema.TargetNamespace;
            }

            internal MetadataContent(XmlDocument document)
            {
                this.m_MetadataXmlDocument = document;
                this.m_TargetNamespace = string.Empty;
            }

            public DiscoveryDocument MetadataDiscoveryDocument =>
                this.m_MetadataDiscoveryDocument;

            public Exception MetadataFormatError =>
                this.m_MetadataFormatError;

            public System.Web.Services.Description.ServiceDescription MetadataServiceDescription =>
                this.m_MetadataServiceDescription;

            public XmlDocument MetadataXmlDocument =>
                this.m_MetadataXmlDocument;

            public System.Xml.Schema.XmlSchema MetadataXmlSchema =>
                this.m_MetadataXmlSchema;

            public string TargetNamespace =>
                this.m_TargetNamespace;
        }

        public enum MetadataType
        {
            [XmlEnum(Name="Disco")]
            Disco = 1,
            [XmlEnum(Name="Edmx")]
            Edmx = 6,
            [XmlEnum(Name="Policy")]
            Policy = 4,
            [XmlEnum(Name="Schema")]
            Schema = 3,
            [XmlEnum(Name="Unknown")]
            Unknown = 0,
            [XmlEnum(Name="Wsdl")]
            Wsdl = 2,
            [XmlEnum(Name="Xml")]
            Xml = 5
        }
    }
}

